// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Client.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Tests.Connected
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    using ExitGames.Client.Photon;
    using ExitGames.Concurrency.Fibers;

    using ExitGames.Logging;

    using Photon.MmoDemo.Common;

    public class Client : IDisposable
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly AutoResetEvent connectResetEvent;

        private readonly IFiber fiber;

        private readonly PhotonPeer peer;

        private readonly PeerListener peerListener;

        private readonly AutoResetEvent resetEvent = new AutoResetEvent(false);

        private readonly Stopwatch stopWatch = new Stopwatch();

        private readonly string username;

        private static long eventReceiveTimeFast;

        private static long eventReceiveTimeMax;

        private static long eventReceiveTimeMiddle;

        private static long eventReceiveTimeSlow;

        private static long eventsReceivedFast;

        private static long eventsReceivedMiddle;

        private static long eventsReceivedSent;

        private static long eventsReceivedSlow;

        private static long exceptions;

        private static long operationsSent;

        private bool connected;

        private EventData receivedEvent;

        private OperationResponse receivedResponse;

        public Client(string username)
        {
            this.username = username;
            this.fiber = new PoolFiber();
            this.fiber.Start();
            this.peerListener = new PeerListener(username, this.OnConnectCallback);
            this.peer = new PhotonPeer(this.peerListener, Settings.UseTcp ? ConnectionProtocol.Tcp : ConnectionProtocol.Udp);

            this.connectResetEvent = new AutoResetEvent(false);
        }

        public Client(string name, Vector position)
            : this(name)
        {
            this.Position = position;
        }

        public static long EventsReceivedFast
        {
            get
            {
                return Interlocked.Read(ref eventsReceivedFast);
            }
        }

        public static long EventsReceivedMiddle
        {
            get
            {
                return Interlocked.Read(ref eventsReceivedMiddle);
            }
        }

        public static long EventsReceivedSlow
        {
            get
            {
                return Interlocked.Read(ref eventsReceivedSlow);
            }
        }

        public static long EventsReceivedTimeMax
        {
            get
            {
                return Interlocked.Read(ref eventReceiveTimeMax);
            }
        }

        public static long EventsReceivedTimeTotalFast
        {
            get
            {
                return Interlocked.Read(ref eventReceiveTimeFast);
            }
        }

        public static long EventsReceivedTimeTotalMiddle
        {
            get
            {
                return Interlocked.Read(ref eventReceiveTimeMiddle);
            }
        }

        public static long EventsReceivedTimeTotalSlow
        {
            get
            {
                return Interlocked.Read(ref eventReceiveTimeSlow);
            }
        }

        public static long Exceptions
        {
            get
            {
                return Interlocked.Read(ref exceptions);
            }
        }

        public static long OperationsSent
        {
            get
            {
                return Interlocked.Read(ref operationsSent);
            }
        }

        public IFiber OperationFiber
        {
            get
            {
                return this.fiber;
            }
        }

        public Vector Position { get; set; }

        public string Username
        {
            get
            {
                return this.username;
            }
        }

        public static void ResetStats()
        {
            Interlocked.Exchange(ref exceptions, 0);
            Interlocked.Exchange(ref eventsReceivedSent, 0);
            Interlocked.Exchange(ref eventReceiveTimeFast, 0);
            Interlocked.Exchange(ref eventReceiveTimeMiddle, 0);
            Interlocked.Exchange(ref eventReceiveTimeSlow, 0);
            Interlocked.Exchange(ref eventReceiveTimeMax, 0);
            Interlocked.Exchange(ref eventsReceivedFast, 0);
            Interlocked.Exchange(ref eventsReceivedMiddle, 0);
            Interlocked.Exchange(ref eventsReceivedSlow, 0);
            Interlocked.Exchange(ref operationsSent, 0);
            PeerListener.ResetStats();
        }

        public void BeginConnect()
        {
            PhotonPeer.RegisterType(typeof(Vector), (byte)Common.Protocol.CustomTypeCodes.Vector, Common.Protocol.SerializeVector, Common.Protocol.DeserializeVector);
            PhotonPeer.RegisterType(typeof(BoundingBox), (byte)Common.Protocol.CustomTypeCodes.BoundingBox, Common.Protocol.SerializeBoundingBox, Common.Protocol.DeserializeBoundingBox);

            this.peer.Connect(Settings.ServerAddress, Settings.ApplicationId);
            this.fiber.Enqueue(this.WaitForConnect);
        }

        public void BeginDisconnect()
        {
            this.fiber.Enqueue(this.DoDisconnect);
        }

        public void BeginReceiveEvent(EventCode eventCode, Func<EventData, bool> checkAction, int delay)
        {
            this.stopWatch.Reset();
            this.fiber.Schedule(
                () =>
                    {
                        this.stopWatch.Start();
                        this.ReceiveEvent(eventCode, checkAction);
                    }, 
                delay);
        }

        public void BeginReceiveResponse(int delay)
        {
            this.stopWatch.Reset();
            this.fiber.Schedule(
                () =>
                    {
                        this.stopWatch.Start();
                        this.ReceiveResponse();
                    }, 
                delay);
        }

        public bool Connect()
        {
            this.BeginConnect();
            return this.EndConnect();
        }

        public bool Disconnect()
        {
            this.BeginDisconnect();
            return this.EndDisconnect();
        }

        public bool EndConnect()
        {
            if (this.connectResetEvent.WaitOne(Settings.ConnectTimeoutMilliseconds))
            {
                return this.connected;
            }

            return false;
        }

        public bool EndDisconnect()
        {
            return this.connectResetEvent.WaitOne(Settings.ConnectTimeoutMilliseconds);
        }

        public bool EndReceiveEvent(int timeoutMilliseconds, out EventData data)
        {
            if (this.resetEvent.WaitOne(timeoutMilliseconds))
            {
                data = this.receivedEvent;
                return true;
            }

            data = null;
            return false;
        }

        public bool EndReceiveResponse(int timeoutMilliseconds, out OperationResponse data)
        {
            if (this.resetEvent.WaitOne(timeoutMilliseconds))
            {
                data = this.receivedResponse;
                return true;
            }

            data = null;
            return false;
        }

        public void SendOperation(byte operationCode, Dictionary<byte, object> parameter, bool reliable)
        {
            this.fiber.Enqueue(() => this.DoSendOperation(operationCode, parameter, reliable));
        }

        #region Implemented Interfaces

        #region IDisposable

        public void Dispose()
        {
            this.Disconnect();
        }

        #endregion

        #endregion

        internal void HandleException(Exception e)
        {
            Interlocked.Increment(ref exceptions);
            log.Error(e);
        }

        private void DoDisconnect()
        {
            try
            {
                if (this.connected)
                {
                    this.peer.Disconnect();
                    this.connected = false;
                }

                this.connectResetEvent.Set();
            }
            catch (Exception e)
            {
                log.Error(e);
                Interlocked.Increment(ref exceptions);
            }
        }

        private void DoSendOperation(byte operationCode, Dictionary<byte, object> parameter, bool reliable)
        {
            try
            {
                Interlocked.Increment(ref operationsSent);
                this.peer.OpCustom(operationCode, parameter, reliable);
                this.peer.Service();
            }
            catch (Exception e)
            {
                log.Error(e);
                Interlocked.Increment(ref exceptions);
            }
        }

        private void OnConnectCallback(PeerListener obj, bool success)
        {
            this.connected = success;
            this.connectResetEvent.Set();
        }

        private void OnEventReceived()
        {
            this.stopWatch.Stop();
            Stopwatch t = this.stopWatch;
            if (t.ElapsedMilliseconds < 50)
            {
                Interlocked.Increment(ref eventsReceivedFast);
                Interlocked.Add(ref eventReceiveTimeFast, t.ElapsedMilliseconds);
            }
            else if (t.ElapsedMilliseconds < 200)
            {
                Interlocked.Increment(ref eventsReceivedMiddle);
                Interlocked.Add(ref eventReceiveTimeMiddle, t.ElapsedMilliseconds);
            }
            else
            {
                Interlocked.Increment(ref eventsReceivedSlow);
                Interlocked.Add(ref eventReceiveTimeSlow, t.ElapsedMilliseconds);
            }

            if (t.ElapsedMilliseconds > EventsReceivedTimeMax)
            {
                Interlocked.Exchange(ref eventReceiveTimeMax, t.ElapsedMilliseconds);
            }
        }

        private void OnResponseReceived()
        {
            this.stopWatch.Stop();
            Stopwatch t = this.stopWatch;
            if (t.ElapsedMilliseconds < 50)
            {
                Interlocked.Increment(ref eventsReceivedFast);
                Interlocked.Add(ref eventReceiveTimeFast, t.ElapsedMilliseconds);
            }
            else if (t.ElapsedMilliseconds < 200)
            {
                Interlocked.Increment(ref eventsReceivedMiddle);
                Interlocked.Add(ref eventReceiveTimeMiddle, t.ElapsedMilliseconds);
            }
            else
            {
                Interlocked.Increment(ref eventsReceivedSlow);
                Interlocked.Add(ref eventReceiveTimeSlow, t.ElapsedMilliseconds);
            }

            if (t.ElapsedMilliseconds > EventsReceivedTimeMax)
            {
                Interlocked.Exchange(ref eventReceiveTimeMax, t.ElapsedMilliseconds);
            }
        }

        private void ReceiveEvent(EventCode eventCode, Func<EventData, bool> checkAction)
        {
            try
            {
                this.peer.Service();

                lock (this.peerListener.EventList)
                {
                    while (this.peerListener.EventList.Count > 0)
                    {
                        var ev = this.peerListener.EventList[0];
                        this.peerListener.EventList.RemoveAt(0);
                        if ((byte)ev.Code == (byte)eventCode && checkAction(ev))
                        {
                            this.receivedEvent = ev;
                            this.OnEventReceived();
                            this.resetEvent.Set();
                            return;
                        }
                    }
                }

                if (this.stopWatch.ElapsedMilliseconds > Settings.WaitTimeMultiOp)
                {
                    Interlocked.Increment(ref exceptions);
                    log.ErrorFormat("client {0} did not receive event {2} in time. {1}ms waited", this.Username, this.stopWatch.ElapsedMilliseconds, eventCode);
                }
                else
                {
                    this.fiber.Schedule(() => this.ReceiveEvent(eventCode, checkAction), 10);
                }
            }
            catch (Exception e)
            {
                this.HandleException(e);
            }
        }

        private void ReceiveResponse()
        {
            try
            {
                this.peer.Service();

                lock (this.peerListener.ResponseList)
                {
                    if (this.peerListener.ResponseList.Count > 0)
                    {
                        var ev = this.peerListener.ResponseList[0];
                        this.peerListener.ResponseList.RemoveAt(0);

                        this.receivedResponse = ev;
                        this.OnResponseReceived();
                        this.resetEvent.Set();

                        if (this.peerListener.ResponseList.Count > 0)
                        {
                            log.WarnFormat("more responses in queue: {0}", this.peerListener.ResponseList.Count);
                        }

                        return;
                    }
                }

                if (this.stopWatch.ElapsedMilliseconds > Settings.WaitTimeMultiOp)
                {
                    Interlocked.Increment(ref exceptions);
                    log.ErrorFormat("client {0} did not receive response in time. {1}ms waited", this.Username, this.stopWatch.ElapsedMilliseconds);
                }
                else
                {
                    this.fiber.Schedule(this.ReceiveResponse, 10);
                }
            }
            catch (Exception e)
            {
                this.HandleException(e);
            }
        }

        private void WaitForConnect()
        {
            try
            {
                if (this.connected == false)
                {
                    this.peer.Service();
                    this.fiber.Enqueue(this.WaitForConnect);
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }
    }
}