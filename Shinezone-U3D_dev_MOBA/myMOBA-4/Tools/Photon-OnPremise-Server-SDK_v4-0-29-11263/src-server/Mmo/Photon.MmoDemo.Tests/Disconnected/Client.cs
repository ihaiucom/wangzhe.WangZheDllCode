// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Client.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Tests.Disconnected
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using ExitGames.Logging;

    using Photon.MmoDemo.Common;
    using Photon.MmoDemo.Server;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    using Settings = Photon.MmoDemo.Tests.Settings;

    public class Client : IDisposable
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Peer peer;

        private readonly DummyPeer dummyPeer;

        private readonly IRpcProtocol protocol;

        private readonly ManualResetEvent resetEvent = new ManualResetEvent(false);

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

        private EventData receivedEvent;

        private OperationResponse receivedResponse;

        public Client(string username)
        {
            this.username = username;
            this.protocol = SocketServer.Protocol.GpBinaryV162;
            this.dummyPeer = new DummyPeer(this.protocol, username);
            var initRequest = new InitRequest(this.protocol, dummyPeer);
            this.peer = new MmoPeer(initRequest);
            this.peer.Initialize(initRequest);
        }

        public void ResetEvent()
        {
            this.resetEvent.Reset();
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

        public static long EventsReceivedTotal
        {
            get
            {
                return DummyPeer.EventsReceived;
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
                return Interlocked.Read(ref eventsReceivedSent);
            }
        }

        public Peer Peer
        {
            get
            {
                return this.peer;
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
            DummyPeer.ResetStats();
        }

        public void BeginReceiveEvent(EventCode eventCode, Func<EventData, bool> checkAction)
        {
            this.stopWatch.Reset();

            this.peer.RequestFiber.Schedule(
                () =>
                    {
                        this.stopWatch.Start();
                        this.ReceiveEvent(eventCode, checkAction);
                    }, 
                10);
        }

        public void BeginReceiveResponse()
        {
            this.stopWatch.Reset();

            this.peer.RequestFiber.Schedule(
                () =>
                    {
                        this.stopWatch.Start();
                        this.ReceiveResponse();
                    }, 
                10);
        }

        public void Disconnect()
        {
            PeerHelper.SimulateDisconnect(this.peer);
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

        public void SendOperation(OperationRequest request)
        {
            Interlocked.Increment(ref eventsReceivedSent);
            PeerHelper.InvokeOnOperationRequest(this.peer, request, new SendParameters());
        }

        #region Implemented Interfaces

        #region IDisposable

        public void Dispose()
        {
            this.peer.Dispose();
        }

        #endregion

        #endregion

        internal void HandleException(Exception e)
        {
            Interlocked.Increment(ref exceptions);
            log.Error(e);
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

        private void ReceiveEvent(EventCode eventCode, Func<EventData, bool> checkAction)
        {
            try
            {
                lock (this.dummyPeer.EventList)
                {
                    while (this.dummyPeer.EventList.Count > 0)
                    {
                        EventData ev = this.dummyPeer.EventList[0];
                        this.dummyPeer.EventList.RemoveAt(0);
                        if (ev.Code == (short)eventCode && checkAction(ev))
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
                    this.peer.RequestFiber.Schedule(() => this.ReceiveEvent(eventCode, checkAction), 10);
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
                lock (this.dummyPeer.ResponseList)
                {
                    if (this.dummyPeer.ResponseList.Count > 0)
                    {
                        OperationResponse ev = this.dummyPeer.ResponseList[0];
                        this.dummyPeer.ResponseList.RemoveAt(0);

                        this.receivedResponse = ev;
                        this.OnEventReceived();
                        this.resetEvent.Set();

                        if (this.dummyPeer.ResponseList.Count > 0)
                        {
                            log.WarnFormat("more responses in queue: {0}", this.dummyPeer.ResponseList.Count);
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
                    this.peer.RequestFiber.Schedule(this.ReceiveResponse, 10);
                }
            }
            catch (Exception e)
            {
                this.HandleException(e);
            }
        }
    }
}