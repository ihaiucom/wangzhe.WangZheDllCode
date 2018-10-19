// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DummyPeer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The network peer dummy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Tests.Disconnected
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using ExitGames.Concurrency.Fibers;
    using ExitGames.Logging;
    using ExitGames.Threading;

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using Photon.SocketServer.Rpc.Protocols;
    using Photon.SocketServer.Rpc.Reflection;

    using PhotonHostRuntimeInterfaces;

    /// <summary>
    ///   The network peer dummy.
    /// </summary>
    public class DummyPeer : IPhotonPeer
    {
        #region Constants and Fields

        /// <summary>
        ///   The logger.
        /// </summary>
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///   The connection id.
        /// </summary>
        private readonly int connectionId;

        /// <summary>
        ///   The event queue.
        /// </summary>
        private readonly List<EventData> eventList = new List<EventData>();

        /// <summary>
        ///   The fiber.
        /// </summary>
        private readonly PoolFiber fiber = new PoolFiber(new FailSafeBatchExecutor());

        /// <summary>
        ///   The protocol.
        /// </summary>
        private readonly IRpcProtocol protocol;

        /// <summary>
        ///   The reset event.
        /// </summary>
        private readonly AutoResetEvent resetEvent = new AutoResetEvent(false);

        /// <summary>
        ///   The auto response.
        /// </summary>
        private readonly AutoResetEvent resetResponse = new AutoResetEvent(false);

        /// <summary>
        ///   The response list.
        /// </summary>
        private readonly List<OperationResponse> responseList = new List<OperationResponse>();

        /// <summary>
        ///   The sync root events.
        /// </summary>
        private readonly object syncRootEvents = new object();

        /// <summary>
        ///   The sync root response.
        /// </summary>
        private readonly object syncRootResponse = new object();

        /// <summary>
        ///   The connection ids.
        /// </summary>
        private static int connectionIds;

        /// <summary>
        ///   The user data object.
        /// </summary>
        private object userData;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DummyPeer" /> class.
        /// </summary>
        /// <param name = "protocol">
        ///   The protocol.
        /// </param>
        public DummyPeer(IRpcProtocol protocol)
        {
            this.protocol = protocol;
            this.connectionId = Interlocked.Increment(ref connectionIds);
            this.fiber.Start();
            this.SessionId = this.connectionId.ToString();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DummyPeer" /> class.
        /// </summary>
        public DummyPeer()
            : this(Photon.SocketServer.Protocol.GpBinaryV162)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Protocol.
        /// </summary>
        public IRpcProtocol Protocol
        {
            get
            {
                return this.protocol;
            }
        }

        /// <summary>
        ///   Gets or sets SessionId.
        /// </summary>
        public string SessionId { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///   The get event list.
        /// </summary>
        /// <returns>
        ///   the event list
        /// </returns>
        public List<EventData> GetEventList()
        {
            lock (this.syncRootEvents)
            {
                var result = new List<EventData>(this.eventList);
                this.eventList.Clear();
                return result;
            }
        }

        /// <summary>
        ///   The get response list.
        /// </summary>
        /// <returns>
        ///   the response list
        /// </returns>
        public List<OperationResponse> GetResponseList()
        {
            lock (this.syncRootResponse)
            {
                var result = new List<OperationResponse>(this.responseList);
                this.responseList.Clear();
                return result;
            }
        }

        /// <summary>
        ///   The on disconnect.
        /// </summary>
        public void OnDisconnect()
        {
        }

        /// <summary>
        ///   The serialize event.
        /// </summary>
        /// <param name = "eventData">
        ///   The event data.
        /// </param>
        /// <returns>
        ///   0 bytes
        /// </returns>
        public byte[] SerializeEventData(EventData eventData)
        {
            this.fiber.Enqueue(
                () =>
                    {
                        lock (this.syncRootEvents)
                        {
                            this.eventList.Add(eventData);

                            if (log.IsDebugEnabled)
                            {
                                log.DebugFormat("{0} receives event, {1} total - code {2}", this.SessionId, this.eventList.Count, eventData.Code);
                            }
                        }

                        this.resetEvent.Set();
                    });

            return new byte[0];
        }

        /// <summary>
        ///   The serialize init request.
        /// </summary>
        /// <param name = "applicationId">
        ///   The application id.
        /// </param>
        /// <param name = "invocationId">
        ///   The invocation id.
        /// </param>
        /// <returns>
        ///   0 bytes
        /// </returns>
        public byte[] SerializeInitRequest(string applicationId, short invocationId)
        {
            return new byte[0];
        }

        /// <summary>
        ///   The serialize init response.
        /// </summary>
        /// <returns>
        ///   0 bytes
        /// </returns>
        public byte[] SerializeInitResponse()
        {
            return new byte[0];
        }

        /// <summary>
        ///   The serialize init response.
        /// </summary>
        /// <param name = "invocationId">
        ///   The invocation id.
        /// </param>
        /// <returns>
        ///   0 bytes
        /// </returns>
        public byte[] SerializeInitResponse(short invocationId)
        {
            return new byte[0];
        }

        /// <summary>
        ///   The serialize operation request.
        /// </summary>
        /// <param name = "operationRequest">
        ///   The operation request.
        /// </param>
        /// <returns>
        ///   0 bytes
        /// </returns>
        public byte[] SerializeOperationRequest(OperationRequest operationRequest)
        {
            return new byte[0];
        }

        /// <summary>
        ///   The serialize operation response.
        /// </summary>
        /// <param name = "operationResponse">
        ///   The operation response.
        /// </param>
        /// <returns>
        ///   0 bytes
        /// </returns>
        public byte[] SerializeOperationResponse(OperationResponse operationResponse)
        {
            this.fiber.Enqueue(
                () =>
                    {
                        lock (this.syncRootResponse)
                        {
                            this.responseList.Add(operationResponse);

                            if (log.IsDebugEnabled)
                            {
                                log.DebugFormat(
                                    "{0} receives response, {1} total - code {2}", this.SessionId, this.responseList.Count, operationResponse.OperationCode);
                            }
                        }

                        this.resetResponse.Set();
                    });

            return new byte[0];
        }

        /// <summary>
        ///   The try convert operation parameter.
        /// </summary>
        /// <param name = "paramterInfo">
        ///   The paramter info.
        /// </param>
        /// <param name = "value">
        ///   The value.
        /// </param>
        /// <returns>
        ///   Always true.
        /// </returns>
        public bool TryConvertOperationParameter(ObjectMemberInfo<DataMemberAttribute> paramterInfo, ref object value)
        {
            return true;
        }

        /// <summary>
        ///   The try parse event data.
        /// </summary>
        /// <param name = "data">
        ///   The data.
        /// </param>
        /// <param name = "reliability">
        ///   The reliability.
        /// </param>
        /// <param name = "channelId">
        ///   The channel id.
        /// </param>
        /// <param name = "eventData">
        ///   The event data.
        /// </param>
        /// <returns>
        ///   Always false.
        /// </returns>
        public bool TryParseEventData(byte[] data, Reliability reliability, byte channelId, out EventData eventData)
        {
            eventData = null;
            return false;
        }

        /// <summary>
        ///   The try parse init request.
        /// </summary>
        /// <param name = "data">
        ///   The data.
        /// </param>
        /// <returns>
        ///   An exception.
        /// </returns>
        /// <exception cref = "NotSupportedException">
        /// This method is not supported.
        /// </exception>
        public bool TryParseInitRequest(byte[] data)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   The try parse init request.
        /// </summary>
        /// <param name = "data">
        ///   The data.
        /// </param>
        /// <param name = "invocationId">
        ///   The invocation id.
        /// </param>
        /// <returns>
        ///   Always false.
        /// </returns>
        public bool TryParseInitRequest(byte[] data, out short invocationId)
        {
            invocationId = 0;
            return false;
        }

        /// <summary>
        ///   The try parse init response.
        /// </summary>
        /// <param name = "data">
        ///   The data.
        /// </param>
        /// <returns>
        ///   Always false.
        /// </returns>
        public bool TryParseInitResponse(byte[] data)
        {
            return false;
        }

        /// <summary>
        ///   The try parse operation request.
        /// </summary>
        /// <param name = "data">
        ///   The data.
        /// </param>
        /// <param name = "reliability">
        ///   The reliability.
        /// </param>
        /// <param name = "channelId">
        ///   The channel id.
        /// </param>
        /// <param name = "operationRequest">
        ///   The operation request.
        /// </param>
        /// <returns>
        ///   An exception.
        /// </returns>
        /// <exception cref = "NotSupportedException">
        /// This method is not supported.
        /// </exception>
        public bool TryParseOperationRequest(byte[] data, Reliability reliability, byte channelId, out OperationRequest operationRequest)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   The try parse operation response.
        /// </summary>
        /// <param name = "data">
        ///   The data.
        /// </param>
        /// <param name = "reliability">
        ///   The reliability.
        /// </param>
        /// <param name = "channelId">
        ///   The channel id.
        /// </param>
        /// <param name = "operationResponse">
        ///   The operation response.
        /// </param>
        /// <returns>
        ///   An exception.
        /// </returns>
        /// <exception cref = "NotSupportedException">
        /// This method is not supported.
        /// </exception>
        public bool TryParseOperationResponse(byte[] data, Reliability reliability, byte channelId, out OperationResponse operationResponse)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   The wait for next event.
        /// </summary>
        /// <param name = "timeout">
        ///   The timeout.
        /// </param>
        /// <returns>
        ///   true if event received.
        /// </returns>
        public bool WaitForNextEvent(int timeout)
        {
            return this.resetEvent.WaitOne(timeout);
        }

        /// <summary>
        ///   The wait for next response.
        /// </summary>
        /// <param name = "timeout">
        ///   The timeout.
        /// </param>
        /// <returns>
        ///   true if response received
        /// </returns>
        public bool WaitForNextResponse(int timeout)
        {
            return this.resetResponse.WaitOne(timeout);
        }

        #endregion

        #region Implemented Interfaces

        #region IPhotonPeer

        IntPtr IPhotonPeer._InternalGetPeerInfo(int why)
        {
            return IntPtr.Zero;
        }

        public void SetDebugString(string debugString)
        {
        }

        public void GetStats(out int rtt, out int rttVariance, out int numFailures)
        {
            rtt = 0;
            rttVariance = 0;
            numFailures = 0;
        }

        SendResults IPhotonPeer._InternalBroadcastSend(byte[] data, MessageReliablity reliability, byte channelId, MessageContentType messageContentType)
        {
            return new SendResults();
        }

        public int GetLastTouched()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   The disconnect client.
        /// </summary>
        public void DisconnectClient()
        {
        }

        public void AbortClient()
        {
        }

        /// <summary>
        /// Gets the local port.
        /// </summary>
        /// <returns>
        /// The localhost IP.
        /// </returns>
        public string GetLocalIP()
        {
            return "127.0.0.1";
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Flush()
        {
        }

        /// <summary>
        ///   Gets the connection id.
        /// </summary>
        /// <returns>
        ///   The connection id.
        /// </returns>
        public int GetConnectionID()
        {
            return this.connectionId;
        }

        /// <summary>
        ///   The get local port.
        /// </summary>
        /// <returns>
        ///   Always Zero.
        /// </returns>
        public ushort GetLocalPort()
        {
            return 0;
        }

        /// <summary>
        ///   The get remote ip.
        /// </summary>
        /// <returns>
        ///   empty sring.
        /// </returns>
        public string GetRemoteIP()
        {
            return string.Empty;
        }

        /// <summary>
        ///   Gets the remote port.
        /// </summary>
        /// <returns>
        ///   Always zero.
        /// </returns>
        public ushort GetRemotePort()
        {
            return 0;
        }

        /// <summary>
        ///   Gets the user data object.
        /// </summary>
        /// <returns>
        ///   The user data object;
        /// </returns>
        public object GetUserData()
        {
            return this.userData;
        }

        /// <summary>
        /// Gets the <see cref="ListenerType" />.
        /// </summary>
        /// <returns>
        /// Value <see cref="ListenerType.TCPListener"/>.
        /// </returns>
        public ListenerType GetListenerType()
        {
            return ListenerType.TCPListener;
        }

        /// <summary>
        /// Gets the peer type.
        /// </summary>
        /// <returns>
        /// Value <see cref="PeerType.TCPPeer"/>.
        /// </returns>
        public PeerType GetPeerType()
        {
            return PeerType.TCPPeer;
        }

        /// <summary>
        ///   The send.
        /// </summary>
        /// <param name = "data">
        ///   The data.
        /// </param>
        /// <param name = "reliability">
        ///   The reliability.
        /// </param>
        /// <param name = "channelId">
        ///   The channel id.
        /// </param>
        /// <param name="messageContentType">The message content type.</param>
        /// <returns>
        ///   Always Ok.
        /// </returns>
        public SendResults Send(byte[] data, MessageReliablity reliability, byte channelId, MessageContentType messageContentType)
        {
            RtsMessageHeader messageType;
            if (this.Protocol.TryParseMessageHeader(data, out messageType) == false)
            {
                throw new InvalidOperationException();
            }

            switch (messageType.MessageType)
            {
                case RtsMessageType.Event:
                    {
                        EventData eventData;
                        if (this.Protocol.TryParseEventData(data, out eventData))
                        {
                            this.fiber.Enqueue(
                                () =>
                                    {
                                        lock (this.syncRootEvents)
                                        {
                                            this.eventList.Add(eventData);

                                            if (log.IsDebugEnabled)
                                            {
                                                log.DebugFormat(
                                                    "{0} receives event, {1} total - code {2}", this.SessionId, this.eventList.Count, eventData.Code);
                                            }
                                        }

                                        this.resetEvent.Set();
                                    });
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }

                        break;
                    }

                case RtsMessageType.OperationResponse:
                    {
                        OperationResponse operationResponse;
                        if (this.Protocol.TryParseOperationResponse(data, out operationResponse))
                        {
                            this.fiber.Enqueue(
                                () =>
                                    {
                                        lock (this.syncRootResponse)
                                        {
                                            this.responseList.Add(operationResponse);

                                            if (log.IsDebugEnabled)
                                            {
                                                log.DebugFormat(
                                                    "{0} receives response, {1} total - code {2}", 
                                                    this.SessionId, 
                                                    this.responseList.Count, 
                                                    operationResponse.OperationCode);
                                            }
                                        }

                                        this.resetResponse.Set();
                                    });
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }

                        break;
                    }
            }

            return SendResults.SentOk;
        }

        /// <summary>
        ///   Sets the user data object.
        /// </summary>
        /// <param name = "userDataObject">
        ///   The user data object.
        /// </param>
        public void SetUserData(object userDataObject)
        {
            Interlocked.Exchange(ref this.userData, userDataObject);
        }

        #endregion

        #endregion
    }
}