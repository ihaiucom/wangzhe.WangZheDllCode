// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DummyPeer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The network peer dummy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Tests.Disconnected
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using ExitGames.Logging;

    using Photon.MmoDemo.Common;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc.Protocols;

    using PhotonHostRuntimeInterfaces;

    public class DummyPeer : IPhotonPeer
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly int connectionId;

        private readonly List<EventData> eventList = new List<EventData>();

        private readonly IRpcProtocol protocol;

        private readonly List<OperationResponse> responseList = new List<OperationResponse>();

        private readonly string username;

        private static int connectionIds;

        private static long eventsReceived;

        private static long responseReceived;

        private object userData;

        #endregion

        #region Constructors and Destructors

        public DummyPeer(IRpcProtocol protocol, string username)
        {
            this.protocol = protocol;
            this.username = username;
            this.connectionId = Interlocked.Increment(ref connectionIds);
        }

        public DummyPeer(string username)
            : this(SocketServer.Protocol.GpBinaryV162, username)
        {
        }

        #endregion

        #region Properties

        public static long EventsReceived
        {
            get
            {
                return Interlocked.Read(ref eventsReceived);
            }
        }

        public List<EventData> EventList
        {
            get
            {
                return this.eventList;
            }
        }

        public IRpcProtocol Protocol
        {
            get
            {
                return this.protocol;
            }
        }

        public List<OperationResponse> ResponseList
        {
            get
            {
                return this.responseList;
            }
        }

        #endregion

        #region Public Methods

        public static void ResetStats()
        {
            Interlocked.Exchange(ref eventsReceived, 0);
        }

        public void OnDisconnect()
        {
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

        public void DisconnectClient()
        {
        }

        public void AbortClient()
        {
        }

        public void Flush()
        {
        }

        public int GetConnectionID()
        {
            return this.connectionId;
        }

        public ushort GetLocalPort()
        {
            return 0;
        }

        public string GetRemoteIP()
        {
            return "127.0.0.1";
        }

        public string GetLocalIP()
        {
            return "127.0.0.1";
        }

        public ushort GetRemotePort()
        {
            return 0;
        }

        public object GetUserData()
        {
            return this.userData;
        }

        public ListenerType GetListenerType()
        {
            return ListenerType.TCPListener;
        }

        public PeerType GetPeerType()
        {
            return PeerType.TCPPeer;
        }

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
                            Interlocked.Increment(ref eventsReceived);
                            lock (this.eventList)
                            {
                                this.eventList.Add(eventData);
                            }

                            if (log.IsDebugEnabled)
                            {
                                if (eventData.Parameters.ContainsKey((byte)ParameterCode.ItemId))
                                {
                                    log.DebugFormat(
                                        "{0} receives event, {1} total - code {2}, source {3}", 
                                        this.username, 
                                        this.eventList.Count, 
                                        (EventCode)eventData.Code,
                                        eventData.Parameters[(byte)ParameterCode.ItemId]);
                                }
                                else
                                {
                                    log.DebugFormat(
                                        "{0} receives event, {1} total - code {2}", 
                                        this.username, 
                                        this.eventList.Count,
                                        (EventCode)eventData.Code);
                                }
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }

                        break;
                    }

                case RtsMessageType.OperationResponse:
                    {
                        OperationResponse response;
                        if (this.Protocol.TryParseOperationResponse(data, out response))
                        {
                            Interlocked.Increment(ref responseReceived);
                            lock (responseList)
                            {
                                this.responseList.Add(response);
                            }

                            if (response.ReturnCode != (int)ReturnCode.Ok)
                            {
                                log.ErrorFormat(
                                    "ERR {0}, OP {1}, DBG {2}", (ReturnCode)response.ReturnCode, (OperationCode)response.OperationCode, response.DebugMessage);
                            }
                            else if (log.IsDebugEnabled)
                            {
                                if (response.Parameters.ContainsKey((byte)ParameterCode.ItemId))
                                {
                                    log.DebugFormat(
                                        "{0} receives response, {1} total - code {2}, source {3}", 
                                        this.username, 
                                        this.responseList.Count, 
                                        (OperationCode)response.OperationCode,
                                        response.Parameters[(byte)ParameterCode.ItemId]);
                                }
                                else
                                {
                                    log.DebugFormat(
                                        "{0} receives response, {1} total - code {2}", 
                                        this.username, 
                                        this.responseList.Count, 
                                        (OperationCode)response.OperationCode);
                                }
                            }
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

        public void SetUserData(object userDataObject)
        {
            Interlocked.Exchange(ref this.userData, userDataObject);
        }

        #endregion

        #endregion
    }
}