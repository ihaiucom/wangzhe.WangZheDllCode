// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientConnection.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The client connection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ExitGames.Client.Photon.LoadBalancing;

namespace Photon.StarDust.Client.Connections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    using ExitGames.Client.Photon;
    using ExitGames.Concurrency.Fibers;
    using ExitGames.Threading;

    using log4net;

    using Photon.StarDust.Client.ConnectionStates;
    using Photon.StarDust.Client.ConnectionStates.Lite;

    public abstract class ClientConnection : IPhotonPeerListener
    {
        #region Constants and Fields

        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected static readonly Stopwatch watch = Stopwatch.StartNew();

        protected readonly PoolFiber fiber;

        public string GameName { get; private set; }

        public int Number { get; set; }

        public string AuthenticationTicket { get; set; }

        public PhotonPeer Peer { get; private set; }

        protected IDisposable counterTimer;

        protected IDisposable sendFlushTimer;

        protected IDisposable sendReliableTimer;

        protected IDisposable sendUnreliableTimer;

        #endregion

        #region Constructors and Destructors
        
        /// <summary>
        ///   Initializes a new instance of the <see cref = "ClientConnection" /> class.
        /// </summary>
        /// <param name = "gameName">
        ///   The game Name.
        /// </param>
        /// <param name = "number">the player number</param>
        public ClientConnection(string gameName, int number)
        {
            // movement channel + data channel
            this.Peer = new PhotonPeer(this, Settings.Protocol) { ChannelCount = 2, DebugOut = Settings.LogLevel, TimePingInterval = Settings.PingInterval };
            this.GameName = gameName;
            this.Number = number;
            this.State = Disconnected.Instance;
            this.fiber = new PoolFiber(new FailSafeBatchExecutor());
            this.fiber.Start();
            this.Peer.DebugOut = DebugLevel.WARNING;
        }

        #endregion

        #region Events
      
        #endregion

        #region Properties

        public IConnectionState State { get; set; }

        #endregion

        #region Public Methods

        public void EnqueueUpdate()
        {
            this.fiber.Schedule(this.Update, 5);
        }

        public void OnEncryptionEstablished()
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Encryption established");
            }

            // continue: to "connected"
            this.State.TransitState(this);
        }

        public void OnConnected()
        {
            if (Settings.UseEncryption)
            {
                // wait for encryption callback before join!
                this.Peer.EstablishEncryption();
            }
            else
            {
                // continue: to "connected"
                this.State.TransitState(this);
            }
        }
        
        /// <summary>
        /// Called by a OnStatusChanged, if the peer was Connected. 
        /// Overridden by subclasses to modify performance counters
        /// </summary>
        public virtual void OnDisconnected()
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Disconnected");
            }

            this.StopTimers();
        }

        /// <summary>
        ///   The peer service.
        /// </summary>
        public void PeerService()
        {
            this.Peer.Service();
        }

        /// <summary>
        ///   The start.
        /// </summary>
        public abstract void Start();

        /// <summary>
        ///   stops the timers and disconnects.
        /// </summary>
        public virtual void Stop()
        {
            // stop position sending
            this.StopTimers();

            if (Settings.ActiveDisconnect)
            {
                this.fiber.Enqueue(this.Peer.Disconnect);
            }
            else
            {
                this.fiber.Enqueue(() => this.State.StopClient(this));
            }
        }
        

        /// <summary>
        ///   The update.
        /// </summary>
        public void Update()
        {
            this.State.OnUpdate(this);
        }
        #endregion

        #region Implemented Interfaces

        #region IPhotonPeerListener

        /// <summary>
        ///   The debug return.
        /// </summary>
        /// <param name = "debugLevel">
        ///   The debug Level.
        /// </param>
        /// <param name = "debug">
        ///   The debug.
        /// </param>
        public void DebugReturn(DebugLevel debugLevel, string debug)
        {
            switch (debugLevel)
            {
                case DebugLevel.ALL:
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("{0}: {1} ", this.GetHashCode(), debug);
                    }

                    break;
                case DebugLevel.INFO:
                    log.InfoFormat("{0}: {1} ", this.GetHashCode(), debug);
                    break;
                case DebugLevel.ERROR:
                    log.ErrorFormat("{0}: {1} ", this.GetHashCode(), debug);
                    break;
                case DebugLevel.WARNING:
                    log.WarnFormat("{0}: {1} ", this.GetHashCode(), debug);
                    break;
            }
        }

        public virtual void OnEvent(EventData eventData)
        {
            switch (eventData.Code)
            {
                    // unreliable event
                case 101:
                    {
                        var data = (Hashtable)eventData[ParameterCode.Data];
                        long now = watch.ElapsedMilliseconds;
                        var sendTime = (long)data[0];
                        long diff = now - sendTime;
                        Counters.UnreliableEventRoundTripTime.IncrementBy(diff);
                        WindowsCounters.UnreliableEventRoundTripTime.IncrementBy(diff);
                        WindowsCounters.UnreliableEventRoundtripTimeBase.Increment();

                        Counters.UnreliableEventsReceived.Increment();
                        WindowsCounters.UnreliableEventsReceived.Increment();
                        break;
                    }

                    // reliable event
                case 102:
                    {
                        var data = (Hashtable)eventData[ParameterCode.Data];
                        long now = watch.ElapsedMilliseconds;
                        var sendTime = (long)data[0];
                        long diff = now - sendTime;

                        Counters.ReliableEventRoundTripTime.IncrementBy(diff);
                        WindowsCounters.ReliableEventRoundTripTime.IncrementBy(diff);
                        WindowsCounters.ReliableEventRoundtripTimeBase.Increment();

                        Counters.ReliableEventsReceived.Increment();
                        WindowsCounters.ReliableEventsReceived.Increment();
                        break;
                    }

                case 103:
                    {
                        var data = (Hashtable)eventData[ParameterCode.Data];
                        long now = watch.ElapsedMilliseconds;
                        var sendTime = (long)data[0];
                        long diff = now - sendTime;
                        Counters.FlushEventRoundTripTime.IncrementBy(diff);
                        WindowsCounters.FlushEventRoundTripTime.IncrementBy(diff);
                        WindowsCounters.FlusheventRoundtripTimeBase.Increment();
                        Counters.FlushEventsReceived.Increment();
                        WindowsCounters.FlushEventsReceived.Increment();
                        break;
                    }
                    
                case EventCode.Leave:
                case EventCode.Join:
                case EventCode.PropertiesChanged:
                    {
                        Counters.ReliableEventsReceived.Increment();
                        WindowsCounters.ReliableEventsReceived.Increment();
                        break;
                    }

                default:
                    {
                        log.WarnFormat("OnEventReceive: unexpected event {0}", eventData.Code);
                        break;
                    }
            }
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            // for stardust server
            switch (operationResponse.OperationCode)
            {
                    // unreliable operation
                case 101:
                    {
                        var data = (Hashtable)operationResponse.Parameters[ParameterCode.Data];
                        long now = watch.ElapsedMilliseconds;
                        var sendTime = (long)data[0];
                        long diff = now - sendTime;
                        Counters.UnreliableEventRoundTripTime.IncrementBy(diff);
                        WindowsCounters.UnreliableEventRoundTripTime.IncrementBy(diff);
                        WindowsCounters.UnreliableEventRoundtripTimeBase.Increment();

                        Counters.UnreliableEventsReceived.Increment();
                        WindowsCounters.UnreliableEventsReceived.Increment();
                        break;
                    }

                    // reliable operation
                case 102:
                    {
                        var data = (Hashtable)operationResponse.Parameters[ParameterCode.Data];
                        long now = watch.ElapsedMilliseconds;
                        var sendTime = (long)data[0];
                        long diff = now - sendTime;
                        Counters.ReliableEventRoundTripTime.IncrementBy(diff);
                        WindowsCounters.ReliableEventRoundTripTime.IncrementBy(diff);
                        WindowsCounters.ReliableEventRoundtripTimeBase.Increment();
                        Counters.ReliableEventsReceived.Increment();
                        WindowsCounters.ReliableEventsReceived.Increment();
                        break;
                    }

                default:
                    {
                        this.State.OnOperationReturn(this, operationResponse);
                        break;
                    }
            }
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
            this.State.OnPeerStatusCallback(this, statusCode);
        }

        public void OnMessage(object messages)
        {
            
        }

        #endregion

        #endregion

        #region Methods
        /// <summary>
        ///   The op raise event.
        /// </summary>
        /// <param name = "eventCode">
        ///   The event code.
        /// </param>
        /// <param name = "eventData">
        ///   The event data.
        /// </param>
        /// <param name = "sendReliable">
        ///   The send reliable.
        /// </param>
        /// <param name = "channelId">
        ///   The channel id.
        /// </param>
        protected void OpRaiseEvent(byte eventCode, Hashtable eventData, bool sendReliable, byte channelId, bool useEncryption)
        {
            var wrap = new Dictionary<byte, object> { { ParameterCode.Data, eventData }, { ParameterCode.Code, eventCode } };
            this.Peer.OpCustom(OperationCode.RaiseEvent, wrap, sendReliable, channelId, useEncryption);
        }

        /// <summary>
        ///   Sends and event that has a minimal RTT.
        /// </summary>
        private void SendFlush()
        {
            const byte EventCode = 103;
            var eventData = new Hashtable { { 0, watch.ElapsedMilliseconds } };
            var wrap = new Dictionary<byte, object> { { ParameterCode.Data, eventData }, { ParameterCode.Code, EventCode }, { 243, true } };
            this.Peer.OpCustom(OperationCode.RaiseEvent, wrap, Settings.FlushReliable, Settings.FlushChannel);

            Counters.FlushOperationsSent.Increment();
            WindowsCounters.FlushOperationsSent.Increment();
        }

        
        /// <summary>
        ///   The send reliable event.
        /// </summary>
        private void SendReliableEvent()
        {
            var data = new byte[Settings.ReliableDataSize];
            this.OpRaiseEvent(102, new Hashtable { { 0, watch.ElapsedMilliseconds }, { ParameterCode.Data, data } }, true, Settings.ReliableDataChannel, Settings.UseEncryption);
            Counters.ReliableOperationsSent.Increment();
            WindowsCounters.ReliableOperationsSent.Increment();
        }

        /// <summary>
        ///   The send unreliable event.
        /// </summary>
        private void SendUnreliableEvent()
        {
            var data = new byte[Settings.UnreliableDataSize];
            this.OpRaiseEvent(101, new Hashtable { { 0, watch.ElapsedMilliseconds }, { ParameterCode.Data, data } }, false, Settings.UnreliableDataChannel, Settings.UseEncryption);
            Counters.UnreliableOperationsSent.Increment();
            WindowsCounters.UnreliableOperationsSent.Increment();
        }

        /// <summary>
        ///   The start timers.
        /// </summary>
        public void StartTimers()
        {
            if (Settings.SendUnreliableData)
            {
                this.sendUnreliableTimer = this.fiber.ScheduleOnInterval(
                    this.SendUnreliableEvent, Settings.UnreliableDataSendInterval, Settings.UnreliableDataSendInterval);
            }

            if (Settings.SendReliableData)
            {
                this.sendReliableTimer = this.fiber.ScheduleOnInterval(
                    this.SendReliableEvent, Settings.ReliableDataSendInterval, Settings.ReliableDataSendInterval);
            }

            // only the first client sends the flush event
            if (this.Number == 0 && Settings.FlushInterval > 0)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat(
                        "Scheduled flush with interval {0} sending {1}reliable", Settings.FlushInterval, Settings.FlushReliable ? string.Empty : "un");
                }

                this.sendFlushTimer = this.fiber.ScheduleOnInterval(this.SendFlush, Settings.FlushInterval, Settings.FlushInterval);
            }

            this.counterTimer = this.fiber.ScheduleOnInterval(
                () =>
                    {
                        Counters.RoundTripTimeVariance.IncrementBy(this.Peer.RoundTripTimeVariance);
                        WindowsCounters.RoundTripTimeVariance.IncrementBy(this.Peer.RoundTripTimeVariance);
                        WindowsCounters.RoundtripTimeVarianceBase.Increment();

                        Counters.RoundTripTime.IncrementBy(this.Peer.RoundTripTime);
                        WindowsCounters.RoundTripTime.IncrementBy(this.Peer.RoundTripTime);
                        WindowsCounters.RoundtripTimeBase.Increment();
                    }, 
                0, 
                1000);
        }

        /// <summary>
        ///   The stop timers.
        /// </summary>
        public void StopTimers()
        {
            if (this.sendUnreliableTimer != null)
            {
                this.sendUnreliableTimer.Dispose();
                this.sendUnreliableTimer = null;
            }

            // stop data sending
            if (this.sendReliableTimer != null)
            {
                this.sendReliableTimer.Dispose();
                this.sendReliableTimer = null;
            }

            if (this.sendFlushTimer != null)
            {
                this.sendFlushTimer.Dispose();
                this.sendFlushTimer = null;
            }

            if (this.counterTimer != null)
            {
                this.counterTimer.Dispose();
                this.counterTimer = null;
            }
        }

        #endregion
    }
}