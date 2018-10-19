//// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NUnitClient.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Plain Photon Client for online tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Threading;
using ExitGames.Client.Photon;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using ExitGames.Threading;

namespace Photon.UnitTest.Utils.Basic.NUnitClients
{
    /// <summary>
    ///   The test client.
    /// </summary>
    public class OnlineNUnitClient : INUnitClient, IPhotonPeerListener
    {
        #region Constants and Fields

        /// <summary>
        ///   The log.
        /// </summary>
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static int curentId;

        /// <summary>
        ///   The auto reset event init.
        /// </summary>
        private readonly AutoResetEvent autoResetEventInit;

        /// <summary>
        ///   The event queue.
        /// </summary>
        public Queue<EventData> EventQueue { get; private set; }


        /// <summary>
        ///   The fiber.
        /// </summary>
        private readonly PoolFiber fiber = new PoolFiber(new FailSafeBatchExecutor());

        /// <summary>
        ///   The operation response queue.
        /// </summary>
        public Queue<OperationResponse> OperationResponseQueue { get; private set; }

        // TODO: currently the client is sending debug messages after 
        // it is disconnected. The logging for this client may went to the next
        // test currently running. 
        // The Rsharper 8 nunit test plugin for VisualStudio for example hangs if 
        // logging comes from a test not currently running.
        public readonly bool LogPhotonClientMessages;

        /// <summary>
        ///   The service.
        /// </summary>
        private long service;

        #endregion

        #region Constructors and Destructors

        public OnlineNUnitClient(ConnectPolicy policy, bool logPhotonClientMessages = false)
            : this(policy.Protocol, logPhotonClientMessages)
        {
            this.Policy = policy;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "OnlineNUnitClient" /> class.
        /// </summary>
        public OnlineNUnitClient(ConnectionProtocol connectionProtocol, bool logPhotonClientMessages = false)
        {
            this.Id = Interlocked.Increment(ref curentId);
            this.LogPhotonClientMessages = logPhotonClientMessages;

            this.autoResetEventInit = new AutoResetEvent(false);
            this.OperationResponseQueue = new Queue<OperationResponse>(1000);
            this.EventQueue = new Queue<EventData>(1000);

            this.Peer = new PhotonPeer(this, connectionProtocol) { DebugOut = DebugLevel.INFO };
            this.fiber.Start();
        }

        #endregion

        #region Events

        /// <summary>
        ///   The connected.
        /// </summary>
        public static event Action<OnlineNUnitClient> Connected;

        /// <summary>
        ///   The disconnected.
        /// </summary>
        public static event Action<OnlineNUnitClient> Disconnected;

        /// <summary>
        ///   The event received.
        /// </summary>
        public static event Action<OnlineNUnitClient, EventData> EventReceived;

        /// <summary>
        ///   The response received.
        /// </summary>
        public static event Action<OnlineNUnitClient, OperationResponse> ResponseReceived;

        #endregion

        #region Properties

        public ConnectPolicy Policy { get; private set; }

        public int Id { get; private set; }

        /// <summary>
        ///   Gets the underling <see cref = "Peer" />.
        /// </summary>
        public PhotonPeer Peer { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///   The close.
        /// </summary>
        public void CloseConnection(int waitForDisconnect = ConnectPolicy.WaitTime)
        {
            this.Disconnect();
            this.WaitForDisconnect(waitForDisconnect);
            this.EventQueue.Clear();
            this.OperationResponseQueue.Clear();
        }

        public void Disconnect()
        {
            this.fiber.Enqueue(
                () =>
                {
                    this.Peer.Disconnect();
                    this.StopService();
                });
        }

        /// <summary>
        ///   The connect.
        /// </summary>
        /// <param name = "address">
        ///   The server address (format:  IP:Port or http://hostname:port, depending on protocol)
        /// </param>
        /// <param name = "applicationId">
        ///   The application Id.
        /// </param>
        public void Connect(string address, string applicationId)
        {
            this.fiber.Enqueue(
                () =>
                {
                    this.Peer.Connect(address, applicationId);
                    this.StartService();
                });
        }

        /// <summary>
        ///   The send operation request.
        /// </summary>
        /// <param name = "operationRequest">
        ///   The operation request.
        /// </param>
        public bool SendOperationRequest(OperationRequest operationRequest)
        {
            NUnit.Framework.Assert.IsTrue(this.Peer.OpCustom(operationRequest.OperationCode, operationRequest.Parameters, true, 0), 
                "Operation not sent.");
            return true;
        }

        /// <summary>
        ///   The wait for connect.
        /// </summary>
        /// <param name = "millisecondsWaitTime">
        ///   The milliseconds wait time.
        /// </param>
        /// <returns>
        ///   true if connected.
        /// </returns>
        public bool WaitForConnect(int millisecondsWaitTime)
        {
            return this.autoResetEventInit.WaitOne(millisecondsWaitTime);
        }

        public bool WaitForDisconnect(int millisecondsWaitTime)
        {
            var timeout = Environment.TickCount + millisecondsWaitTime;

            while (Environment.TickCount < timeout)
            {
                if (this.Peer.PeerState == PeerStateValue.Disconnected)
                {
                    return true;
                }
            }

            log.WarnFormat("Wait for disconnect returned false: id = {0}", this.Id);
            return false;
        }

        /// <summary>
        ///   The wait for event.
        /// </summary>
        /// <param name = "millisecodsWaitTime">
        ///   The millisecods wait time.
        /// </param>
        /// <returns>
        ///   the event
        /// </returns>
        public EventData WaitForEvent(int millisecodsWaitTime)
        {
            var timeout = Environment.TickCount + millisecodsWaitTime;

            while (Environment.TickCount < timeout)
            {
                if (this.EventQueue.Count > 0)
                {
                    return this.EventQueue.Dequeue();
                }
            }

            throw new TimeoutException();
        }

        /// <summary>
        ///   Wait for an event with a specific event code. Discard all other events that arrive in the meantime.
        /// </summary>
        /// <param name = "millisecodsWaitTime">
        ///   The millisecods wait time.
        /// </param>
        /// <param name="eventCode">Wait for an event with this event code.</param>
        /// <returns>
        ///   the event
        /// </returns>
        public EventData WaitForEvent(byte eventCode, int millisecodsWaitTime)
        {
            var timeout = Environment.TickCount + millisecodsWaitTime;

            while (Environment.TickCount < timeout)
            {
                if (this.EventQueue.Count > 0)
                {
                    var ev = this.EventQueue.Dequeue();
                    if (ev.Code == eventCode)
                    {
                        return ev;
                    }
                }
            }

            throw new TimeoutException();
        }

        public bool TryWaitForEvent(byte eventCode, int waitTime, out EventData eventData)
        {
            eventData = null;
            try
            {
                eventData = this.WaitForEvent(eventCode, waitTime);
            }
            catch (TimeoutException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///   The wait for operation response.
        /// </summary>
        /// <param name = "millisecodsWaitTime">
        ///   The millisecods wait time.
        /// </param>
        /// <returns>
        ///   the response
        /// </returns>
        public OperationResponse WaitForOperationResponse(int millisecodsWaitTime)
        {
            var timeout = Environment.TickCount + millisecodsWaitTime;

            while (Environment.TickCount < timeout)
            {
                if (this.OperationResponseQueue.Count > 0)
                {
                    return this.OperationResponseQueue.Dequeue();
                }
            }

            throw new TimeoutException();
        }

        public bool SendRequest(OperationRequest op)
        {
            return this.Peer.OpCustom(op, true, 0, false);
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        ///   The dispose.
        /// </summary>
        public void Dispose()
        {
            this.fiber.Enqueue(
                () =>
                {
                    this.Peer.Disconnect();
                    this.StopService();
                    this.fiber.Dispose();
                });
        }

        #endregion

        #region IPhotonPeerListener

        /// <summary>
        ///   The debug return.
        /// </summary>
        /// <param name = "debugLevel">
        ///   The debug Level.
        /// </param>
        /// <param name = "debug">
        ///   The debug message.
        /// </param>
        public void DebugReturn(DebugLevel debugLevel, string debug)
        {
            if (this.LogPhotonClientMessages == false)
            {
                return;
            }

            switch (debugLevel)
            {
                case DebugLevel.ALL:
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("DebugReturn: id={0}, msg={1}", this.Id, debug);
                    }

                    break;

                case DebugLevel.INFO:
                    log.InfoFormat("DebugReturn(INFO): id={0}, msg={1}", this.Id, debug);
                    break;

                case DebugLevel.WARNING:
                    log.WarnFormat("DebugReturn(WARN): id={0}, msg={1}", this.Id, debug);
                    break;

                case DebugLevel.ERROR:
                    log.ErrorFormat("DebugReturn(ERROR): id={0}, msg={1}", this.Id, debug);
                    break;
            }
        }

        public void OnEvent(EventData @event)
        {
            var eventData = new EventData { Code = @event.Code, Parameters = @event.Parameters };
            if (log.IsDebugEnabled)
            {
                log.Debug("EventReceived");
                //LogHelper.WriteDictionaryContent(eventData.Parameters, 0);
            }

            this.EventQueue.Enqueue(eventData);

            OnEventReceived(this, eventData);
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            var response = new OperationResponse
            {
                OperationCode = operationResponse.OperationCode,
                Parameters = operationResponse.Parameters,
                ReturnCode = operationResponse.ReturnCode,
                DebugMessage = operationResponse.DebugMessage,
            };

            if (log.IsDebugEnabled)
            {
                LogOperationResponse(response);
            }

            this.OperationResponseQueue.Enqueue(response);

            OnResponseReceived(this, response);
        }

        public void OnMessage(object messages)
        {
        }

        public void OnStatusChanged(StatusCode returnCode)
        {
            switch (returnCode)
            {
                case StatusCode.Connect:
                    {
                        this.autoResetEventInit.Set();
                        OnConnected(this);
                        break;
                    }

                case StatusCode.Disconnect:
                    {
                        OnDisonnected(this);
                        break;
                    }

                case StatusCode.DisconnectByServerLogic:
                case StatusCode.DisconnectByServer:
                case StatusCode.DisconnectByServerUserLimit:
                    {
                        log.Warn(returnCode);
                        OnDisonnected(this);
                        break;
                    }

                case StatusCode.QueueOutgoingReliableWarning:
                case StatusCode.QueueIncomingReliableWarning:
                    {
                        break;
                    }

                default:
                    {
                        log.Warn(returnCode);
                        break;
                    }
            }
        }

        #endregion

        #region INUnitClient

        bool INUnitClient.Connected { get { return this.Peer.PeerState == PeerStateValue.Connected; } }
        public string RemoteEndPoint { get { return this.Peer.ServerAddress; } }
        public void EventQueueClear()
        {
            this.EventQueue.Clear();
        }

        public void OperationResponseQueueClear()
        {
            this.OperationResponseQueue.Clear();
        }

        void INUnitClient.Connect(string address)
        {
            if (this.Policy != null)
            {
                this.Policy.ConnectToServer(this, address);
            }
            else
            {
                throw new Exception("Policy is not set");
            }
        }
        #endregion
        #endregion

        #region Methods

        /// <summary>
        ///   The log operation response.
        /// </summary>
        /// <param name = "response">
        ///   The response.
        /// </param>
        private static void LogOperationResponse(OperationResponse response)
        {
            foreach (var item in response.Parameters)
            {
                log.DebugFormat(string.Format("{0}({1}): {2}", item.Key, item.Key, item.Value));
            }
        }

        /// <summary>
        ///   The on connected.
        /// </summary>
        /// <param name = "client">
        ///   The client.
        /// </param>
        private static void OnConnected(OnlineNUnitClient client)
        {
            Action<OnlineNUnitClient> connected = Connected;
            if (connected != null)
            {
                connected(client);
            }
        }

        /// <summary>
        ///   invokes <see cref = "Disconnected" />.
        /// </summary>
        /// <param name = "client">
        ///   The client.
        /// </param>
        private static void OnDisonnected(OnlineNUnitClient client)
        {
            Action<OnlineNUnitClient> handler = Disconnected;
            if (handler != null)
            {
                handler(client);
            }
        }

        /// <summary>
        ///   The on event received.
        /// </summary>
        /// <param name = "client">
        ///   The client.
        /// </param>
        /// <param name = "data">
        ///   The data.
        /// </param>
        private static void OnEventReceived(OnlineNUnitClient client, EventData data)
        {
            Action<OnlineNUnitClient, EventData> received = EventReceived;
            if (received != null)
            {
                received(client, data);
            }
        }

        /// <summary>
        ///   The on response received.
        /// </summary>
        /// <param name = "client">
        ///   The client.
        /// </param>
        /// <param name = "response">
        ///   The response.
        /// </param>
        private static void OnResponseReceived(OnlineNUnitClient client, OperationResponse response)
        {
            Action<OnlineNUnitClient, OperationResponse> received = ResponseReceived;
            if (received != null)
            {
                received(client, response);
            }
        }

        /// <summary>
        ///   The service.
        /// </summary>
        private void Service()
        {
            if (Interlocked.Read(ref this.service) == 1)
            {
                this.Peer.Service();
                this.fiber.Schedule(this.Service, 50);
            }
        }

        /// <summary>
        ///   The start service.
        /// </summary>
        private void StartService()
        {
            Interlocked.Exchange(ref this.service, 1);
            this.fiber.Enqueue(this.Service);
        }

        /// <summary>
        ///   The stop service.
        /// </summary>
        private void StopService()
        {
            Interlocked.Exchange(ref this.service, 0);
        }

        #endregion
    }
}
