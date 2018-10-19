
namespace Photon.LoadBalancing.GameServer
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    using ExitGames.Logging;

    using Photon.SocketServer;

    public abstract class MasterServerConnectionBase : IDisposable
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public readonly GameApplication Application;

        public readonly string Address;

        public readonly int Port;

        private readonly int connectRetryIntervalSeconds;

        private byte isReconnecting;

        private Timer reconnectTimer;

        private OutgoingMasterServerPeer peer;

        protected MasterServerConnectionBase(GameApplication controller, string address, int port, int connectRetryIntervalSeconds)
        {
            this.Application = controller;
            this.Address = address;
            this.Port = port;
            this.connectRetryIntervalSeconds = connectRetryIntervalSeconds;
        }

        public IPEndPoint EndPoint { get; private set; }

        public OutgoingMasterServerPeer GetPeer()
        {
            return this.peer;
        }

        public void Initialize()
        {
            this.ConnectToMaster();
        }

        public SendResult SendEventIfRegistered(IEventData eventData, SendParameters sendParameters)
        {
            var masterPeer = this.peer;
            if (masterPeer == null || masterPeer.IsRegistered == false)
            {
                return SendResult.Disconnected;
            }

            return masterPeer.SendEvent(eventData, sendParameters);
        }

        public SendResult SendEvent(IEventData eventData, SendParameters sendParameters)
        {
            var masterPeer = this.peer;
            if (masterPeer == null || masterPeer.Connected == false)
            {
                return SendResult.Disconnected;
            }

            return masterPeer.SendEvent(eventData, sendParameters);
        }

        public virtual void UpdateAllGameStates()
        {
        }

        public void ConnectToMaster()
        {
            if (this.reconnectTimer != null)
            {
                this.reconnectTimer.Dispose();
                this.reconnectTimer = null;
            }

            // check if the photon application is shuting down
            if (this.Application.Running == false)
            {
                return;
            }

            try
            {
                this.UpdateEndpoint();
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("MasterServer endpoint for address {0} updated to {1}", this.Address, this.EndPoint);
                }

                this.ConnectToMaster(this.EndPoint);
            }
            catch(Exception e)
            {
                log.Error(e);
                if (this.isReconnecting == 1)
                {
                    this.ReconnectToMaster();
                }
                else
                {
                    throw;
                }
            }
        }

        public void ReconnectToMaster()
        {
            if (this.Application.Running == false)
            {
                return;
            }

            Thread.VolatileWrite(ref this.isReconnecting, 1);
            this.reconnectTimer = new Timer(o => this.ConnectToMaster(), null, this.connectRetryIntervalSeconds * 1000, System.Threading.Timeout.Infinite);
        }

        public void ConnectToMaster(IPEndPoint endPoint)
        {
            if (this.Application.Running == false)
            {
                return;
            }
            if (this.peer == null)
            {
                this.peer = this.CreateServerPeer();
            }
            if (this.peer.ConnectTcp(endPoint, "Master"))
            {
                if (log.IsInfoEnabled)
                {
                    log.InfoFormat("Connecting to master at {0}, serverId={1}", endPoint, this.Application.ServerId);
                }
            }
            else
            {
                log.WarnFormat("master connection refused - is the process shutting down ? {0}", this.Application.ServerId);
            }
        }

        public void UpdateEndpoint()
        {
            IPAddress masterAddress;
            if (!IPAddress.TryParse(this.Address, out masterAddress))
            {
                var hostEntry = Dns.GetHostEntry(this.Address);
                if (hostEntry.AddressList == null || hostEntry.AddressList.Length == 0)
                {
                    throw new ExitGames.Configuration.ConfigurationException(
                        "MasterIPAddress setting is neither an IP nor an DNS entry: " + this.Address);
                }

                masterAddress = hostEntry.AddressList.First(address => address.AddressFamily == AddressFamily.InterNetwork);

                if (masterAddress == null)
                {
                    throw new ExitGames.Configuration.ConfigurationException(
                        "MasterIPAddress does not resolve to an IPv4 address! Found: "
                        + string.Join(", ", hostEntry.AddressList.Select(a => a.ToString()).ToArray()));
                }
            }

            this.EndPoint = new IPEndPoint(masterAddress, this.Port);
        }

        protected abstract OutgoingMasterServerPeer CreateServerPeer();

        public void OnConnectionEstablished(object responseObject)
        {
            if (log.IsInfoEnabled)
            {
                log.InfoFormat("Master connection established: address:{0}", this.Address);
            }
            this.Application.OnMasterConnectionEstablished(this);
            Thread.VolatileWrite(ref this.isReconnecting, 0);
        }

        public void OnConnectionFailed(int errorCode, string errorMessage) 
        {
            if (this.isReconnecting == 0)
            {
                log.ErrorFormat(
                    "Master connection failed: address={0}, errorCode={1}, msg={2}", 
                    this.EndPoint,
                    errorCode, 
                    errorMessage);
            }
            else if (log.IsWarnEnabled)
            {
                log.WarnFormat(
                    "Master connection failed: address={0}, errorCode={1}, msg={2}",
                    this.EndPoint,
                    errorCode,
                    errorMessage);
            }

            this.ReconnectToMaster();
        }

        public void Dispose()
        {
            var timer = this.reconnectTimer;
            if (timer != null)
            {
                timer.Dispose();
                this.reconnectTimer = null;
            }

            var masterPeer = this.peer;
            if (masterPeer != null)
            {
                masterPeer.Disconnect();
                masterPeer.Dispose();
                this.peer = null;
            }
        }
    }
}
