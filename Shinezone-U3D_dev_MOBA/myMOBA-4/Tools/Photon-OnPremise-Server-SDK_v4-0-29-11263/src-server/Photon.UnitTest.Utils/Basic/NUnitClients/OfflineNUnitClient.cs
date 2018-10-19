using System.Threading;
using NUnit.Framework;
using Photon.SocketServer;
using Photon.SocketServer.UnitTesting;
using EventData = ExitGames.Client.Photon.EventData;
using OperationRequest = ExitGames.Client.Photon.OperationRequest;
using OperationResponse = ExitGames.Client.Photon.OperationResponse;

namespace Photon.UnitTest.Utils.Basic.NUnitClients
{
    public class OfflineNUnitClient : UnitTestClient, INUnitClient
    {
        public string RemoteEndPoint 
        {
            get
            {
                return this.ServerAppProxy.EndPoint.ToString();
            }
        }

        public PhotonApplicationProxy ServerAppProxy { get; private set; }

        public OfflineNUnitClient(int defaultTimeout, ConnectPolicy policy)
            : base(defaultTimeout, policy.ClientVersion, policy.sdkId)
        {
            this.Policy = policy;
        }

        public ConnectPolicy Policy { get; set; }

        void INUnitClient.Connect(string serverAddress)
        {
            this.Policy.ConnectToServer(this, serverAddress);
        }

        public new bool Connect(PhotonApplicationProxy serverAppProxy)
        {
            Assert.IsNotNull(serverAppProxy);

            if (base.Connect(serverAppProxy))
            {
                this.ServerAppProxy = serverAppProxy;
                return true;
            }
            return false;
        }

        public bool SendRequest(OperationRequest op)
        {
            Thread.Sleep(40);
            var r = new Photon.SocketServer.OperationRequest
            {
                OperationCode = op.OperationCode,
                Parameters = op.Parameters
            };
            return (this.SendOperationRequest(r) == SendResult.Ok);
        }

        public new EventData WaitForEvent(int millisecodsWaitTime = ConnectPolicy.WaitTime)
        {
            Thread.Sleep(40);
            var res = base.WaitForEvent(millisecodsWaitTime);

            if (res == null)
            {
                return null;
            }

            return new EventData
            {
                Code = res.Code,
                Parameters = res.Parameters,
            };
        }
        public new OperationResponse WaitForOperationResponse(int milliseconsWaitTime = ConnectPolicy.WaitTime)
        {
            Thread.Sleep(40);
            var res = base.WaitForOperationResponse(milliseconsWaitTime);
            if (res == null)
            {
                return null;
            }

            return new OperationResponse
            {
                DebugMessage = res.DebugMessage,
                OperationCode = res.OperationCode,
                Parameters = res.Parameters,
                ReturnCode = res.ReturnCode,
            };
        }

        public void EventQueueClear()
        {
            this.EventQueue.Clear();
        }
        public void OperationResponseQueueClear()
        {
            this.ResponseQueue.Clear();
        }

        public bool WaitForConnect(int timeout = ConnectPolicy.WaitTime)
        {
            Thread.Sleep(40);
            return true;
        }
    }
}
