// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicUseCases.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The basic use cases.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Tests.Disconnected
{
    using System.Collections;
    using System.Collections.Generic;

    using ExitGames.Client.Photon.Lite;

    using Lite.Operations;

    using NUnit.Framework;

    using Photon.SocketServer;

    using EventData = Photon.SocketServer.EventData;
    using LitePeer = Lite.LitePeer;
    using OperationRequest = Photon.SocketServer.OperationRequest;
    using OperationResponse = Photon.SocketServer.OperationResponse;
    using ReceiverGroup = Lite.Operations.ReceiverGroup;

    /// <summary>
    ///   The basic use cases.
    /// </summary>
    [TestFixture]
    public class BasicUseCases
    {
        #region Constants and Fields

        /// <summary>
        ///   The wait timeout.
        /// </summary>
        private const int WaitTimeout = 5000;

        #endregion

        #region Public Methods

        private LiteApplication application; 

        [SetUp]
        public void Setup()
        {
            this.application = new LiteApplication();
            this.application.OnStart("NUnit", "Lite", new DummyApplicationSink(), null, null, string.Empty); 
        }

        /// <summary>
        ///   The join and leave.
        /// </summary>
        [Test]
        public void JoinAndJoin()
        {
            var peerOne = new DummyPeer();
            var peerTwo = new DummyPeer();
            var litePeerOne = new TestLitePeer(peerOne.Protocol, peerOne);
            var litePeerTwo = new TestLitePeer(peerTwo.Protocol, peerTwo);

            try
            {
                // peer 1: join
                OperationRequest request = GetJoinRequest();
                PeerHelper.InvokeOnOperationRequest(litePeerOne, request, new SendParameters());
                Assert.IsTrue(peerOne.WaitForNextResponse(WaitTimeout));

                List<OperationResponse> responseList = peerOne.GetResponseList();
                Assert.AreEqual(1, responseList.Count);
                OperationResponse response = responseList[0];
                Assert.AreEqual(1, response.Parameters[(byte)ParameterKey.ActorNr]);

                // peer 1: receive own join event
                Assert.IsTrue(peerOne.WaitForNextEvent(WaitTimeout));
                List<EventData> eventList = peerOne.GetEventList();
                Assert.AreEqual(1, eventList.Count);
                EventData eventData = eventList[0];
                Assert.AreEqual(LiteOpCode.Join, eventData.Code);
                Assert.AreEqual(1, eventData.Parameters[(byte)ParameterKey.ActorNr]);

                // peer 2: join 
                request = GetJoinRequest();
                PeerHelper.InvokeOnOperationRequest(litePeerTwo, request, new SendParameters());
                Assert.IsTrue(peerTwo.WaitForNextResponse(WaitTimeout));

                responseList = peerTwo.GetResponseList();
                Assert.AreEqual(1, responseList.Count);
                response = responseList[0];
                Assert.AreEqual(2, response.Parameters[(byte)ParameterKey.ActorNr]);

                // peer 2: receive own join event
                Assert.IsTrue(peerTwo.WaitForNextEvent(WaitTimeout));
                eventList = peerTwo.GetEventList();
                Assert.AreEqual(1, eventList.Count);
                eventData = eventList[0];
                Assert.AreEqual(LiteOpCode.Join, eventData.Code);
                Assert.AreEqual(2, eventData.Parameters[(byte)ParameterKey.ActorNr]);

                // peer 1: receive join event of peer 2
                Assert.IsTrue(peerOne.WaitForNextEvent(WaitTimeout));
                eventList = peerOne.GetEventList();
                Assert.AreEqual(1, eventList.Count);
                eventData = eventList[0];
                Assert.AreEqual(LiteOpCode.Join, eventData.Code);
                Assert.AreEqual(2, eventData.Parameters[(byte)ParameterKey.ActorNr]);

                // peer 2: join again
                request = GetJoinRequest();
                PeerHelper.InvokeOnOperationRequest(litePeerTwo, request, new SendParameters());
                Assert.IsTrue(peerTwo.WaitForNextResponse(WaitTimeout));
                responseList = peerTwo.GetResponseList();
                Assert.AreEqual(1, responseList.Count);

                // peer 2: do not receive own leave event, receive own join event
                Assert.IsTrue(peerTwo.WaitForNextEvent(WaitTimeout));
                eventList = peerTwo.GetEventList();
                Assert.AreEqual(1, eventList.Count);
                eventData = eventList[0];
                Assert.AreEqual(LiteOpCode.Join, eventData.Code);
                Assert.AreEqual(3, eventData.Parameters[(byte)ParameterKey.ActorNr]);

                // peer 1: receive leave and event of peer 2
                Assert.IsTrue(peerOne.WaitForNextEvent(WaitTimeout));
                eventList = peerOne.GetEventList();
                Assert.GreaterOrEqual(eventList.Count, 1);
                eventData = eventList[0];
                Assert.AreEqual(LiteOpCode.Leave, eventData.Code);
                Assert.AreEqual(2, eventData.Parameters[(byte)ParameterKey.ActorNr]);

                if (eventList.Count == 1)
                {
                    // waiting for join event
                    Assert.IsTrue(peerOne.WaitForNextEvent(WaitTimeout));
                    eventList = peerOne.GetEventList();
                    Assert.AreEqual(1, eventList.Count);
                    eventData = eventList[0];
                }
                else
                {
                    eventData = eventList[1];
                }

                Assert.AreEqual(LiteOpCode.Join, eventData.Code);
                Assert.AreEqual(3, eventData.Parameters[(byte)ParameterKey.ActorNr]);
            }
            finally
            {
                PeerHelper.SimulateDisconnect(litePeerOne);
                PeerHelper.SimulateDisconnect(litePeerTwo);
            }
        }

        /// <summary>
        ///   The join and leave.
        /// </summary>
        [Test]
        public void JoinAndLeave()
        {
            var peerOne = new DummyPeer();
            var peerTwo = new DummyPeer();
            var litePeerOne = new TestLitePeer(peerOne.Protocol, peerOne);
            var litePeerTwo = new TestLitePeer(peerTwo.Protocol, peerTwo);

            try
            {
                // peer 1: join
                OperationRequest request = GetJoinRequest();
                PeerHelper.InvokeOnOperationRequest(litePeerOne, request, new SendParameters());
                Assert.IsTrue(peerOne.WaitForNextResponse(WaitTimeout));

                List<OperationResponse> responseList = peerOne.GetResponseList();
                Assert.AreEqual(1, responseList.Count);
                OperationResponse response = responseList[0];
                Assert.AreEqual(1, response.Parameters[(byte)ParameterKey.ActorNr]);

                // peer 1: receive own join event
                Assert.IsTrue(peerOne.WaitForNextEvent(WaitTimeout));
                List<EventData> eventList = peerOne.GetEventList();
                Assert.AreEqual(1, eventList.Count);
                EventData eventData = eventList[0];
                Assert.AreEqual(LiteOpCode.Join, eventData.Code);
                Assert.AreEqual(1, eventData.Parameters[(byte)ParameterKey.ActorNr]);

                // peer 2: join 
                request = GetJoinRequest();
                PeerHelper.InvokeOnOperationRequest(litePeerTwo, request, new SendParameters());
                Assert.IsTrue(peerTwo.WaitForNextResponse(WaitTimeout));

                responseList = peerTwo.GetResponseList();
                Assert.AreEqual(1, responseList.Count);
                response = responseList[0];
                Assert.AreEqual(2, response.Parameters[(byte)ParameterKey.ActorNr]);

                // peer 2: receive own join event
                Assert.IsTrue(peerTwo.WaitForNextEvent(WaitTimeout));
                eventList = peerTwo.GetEventList();
                Assert.AreEqual(1, eventList.Count);
                eventData = eventList[0];
                Assert.AreEqual(LiteOpCode.Join, eventData.Code);
                Assert.AreEqual(2, eventData.Parameters[(byte)ParameterKey.ActorNr]);

                // peer 1: receive join event of peer 2
                Assert.IsTrue(peerOne.WaitForNextEvent(WaitTimeout));
                eventList = peerOne.GetEventList();
                Assert.AreEqual(1, eventList.Count);
                eventData = eventList[0];
                Assert.AreEqual(LiteOpCode.Join, eventData.Code);
                Assert.AreEqual(2, eventData.Parameters[(byte)ParameterKey.ActorNr]);

                // peer 2: leave
                request = GetLeaveRequest();
                PeerHelper.InvokeOnOperationRequest(litePeerTwo, request, new SendParameters());
                Assert.IsTrue(peerTwo.WaitForNextResponse(WaitTimeout));
                responseList = peerTwo.GetResponseList();
                Assert.AreEqual(1, responseList.Count);

                // peer 2: do not receive own leave event
                eventList = peerTwo.GetEventList();
                Assert.AreEqual(0, eventList.Count);

                // peer 1: receive leave event of peer 2
                Assert.IsTrue(peerOne.WaitForNextEvent(WaitTimeout));
                eventList = peerOne.GetEventList();
                Assert.AreEqual(1, eventList.Count);
                eventData = eventList[0];
                Assert.AreEqual(LiteOpCode.Leave, eventData.Code);
                Assert.AreEqual(2, eventData.Parameters[(byte)ParameterKey.ActorNr]);
            }
            finally
            {
                PeerHelper.SimulateDisconnect(litePeerOne);
                PeerHelper.SimulateDisconnect(litePeerTwo);
            }
        }

        /// <summary>
        ///   Test for sending event to target actor numbers.
        /// </summary>
        [Test]
        public void RaiseEventActors()
        {
            var peerOne = new DummyPeer();
            var peerTwo = new DummyPeer();
            var peerThree = new DummyPeer();
            var litePeerOne = new TestLitePeer(peerOne.Protocol, peerOne);
            var litePeerTwo = new TestLitePeer(peerTwo.Protocol, peerTwo);
            var litePeerThree = new TestLitePeer(peerThree.Protocol, peerThree);

            try
            {
                // peer 1: join
                JoinPeer(peerOne, litePeerOne, 1);

                // peer 2: join 
                JoinPeer(peerTwo, litePeerTwo, 2);

                // peer 1: receive join event of peer 2
                ReceiveJoinEvent(peerOne, 2);

                // peer 2: join 
                JoinPeer(peerThree, litePeerThree, 3);

                // peer 1: receive join event of peer 3
                ReceiveJoinEvent(peerOne, 3);

                // peer 2: receive join event of peer 3
                ReceiveJoinEvent(peerTwo, 3);

                // peer 1: send to peer 1 + 3 (ordered)
                byte code = 100;
                var data = new Hashtable { { 1, "value1" } };
                var request = GetRaiseEventRequest(code, data, null, null, new[] { 1, 2, 3 });
                PeerHelper.InvokeOnOperationRequest(litePeerOne, request, new SendParameters());

                // Peer 1 + 2 + 3: Receive data
                var @event = ReceiveEvent(peerOne, code);
                Assert.AreEqual(code, @event.Code);
                var @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                @event = ReceiveEvent(peerTwo, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                @event = ReceiveEvent(peerThree, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                // peer 3: send to peer 3 + 2 + 1 (unordered)
                code++;
                request = GetRaiseEventRequest(code, data, null, null, new[] { 3, 2, 1 });
                PeerHelper.InvokeOnOperationRequest(litePeerThree, request, new SendParameters());

                // Peer 1 + 2 + 3: Receive data
                @event = ReceiveEvent(peerOne, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                @event = ReceiveEvent(peerTwo, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                @event = ReceiveEvent(peerThree, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                // peer 3: send to peer 3 + 2 + 1 + 2 + 3 + 3 (unordered, duplicate)
                code++;
                request = GetRaiseEventRequest(code, data, null, null, new[] { 3, 2, 1, 2, 3, 3 });
                PeerHelper.InvokeOnOperationRequest(litePeerThree, request, new SendParameters());

                // Peer 1 + 2 + 3: Receive data
                @event = ReceiveEvent(peerOne, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                var events = ReceiveEvents(peerTwo, 2);
                foreach (var ev in events)
                {
                    @event = ev;
                    Assert.AreEqual(code, @event.Code);
                    @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                    Assert.AreEqual("value1", @eventData[1]);
                }

                events = ReceiveEvents(peerThree, 3);
                foreach (var ev in events)
                {
                    @event = ev;
                    Assert.AreEqual(code, @event.Code);
                    @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                    Assert.AreEqual("value1", @eventData[1]);
                }
            }
            finally
            {
                PeerHelper.SimulateDisconnect(litePeerOne);
                PeerHelper.SimulateDisconnect(litePeerTwo);
                PeerHelper.SimulateDisconnect(litePeerThree);
            }
        }

        /// <summary>
        ///   Tests if existing and new peer receive cached events.
        /// </summary>
        [Test]
        public void RaiseEventCacheMerge()
        {
            var peerOne = new DummyPeer();
            var peerTwo = new DummyPeer();
            var peerThree = new DummyPeer();
            var litePeerOne = new TestLitePeer(peerOne.Protocol, peerOne);
            var litePeerTwo = new TestLitePeer(peerTwo.Protocol, peerTwo);
            var litePeerThree = new TestLitePeer(peerThree.Protocol, peerThree);

            try
            {
                // peer 1: join
                JoinPeer(peerOne, litePeerOne, 1);

                // peer 2: join 
                JoinPeer(peerTwo, litePeerTwo, 2);

                // peer 1: receive join event of peer 2
                ReceiveJoinEvent(peerOne, 2);

                // peer 1: send to all others
                const byte Code = 100;
                var data = new Hashtable { { 1, "value1" } };
                var request = GetRaiseEventRequest(Code, data, (byte)CacheOperation.MergeCache, null, null);
                PeerHelper.InvokeOnOperationRequest(litePeerOne, request, new SendParameters());

                // Peer 2: Receive data
                var @event = ReceiveEvent(peerTwo, Code);
                Assert.AreEqual(Code, @event.Code);
                var @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                // peer 3: join 
                request = GetJoinRequest();
                PeerHelper.InvokeOnOperationRequest(litePeerThree, request, new SendParameters());
                Assert.IsTrue(peerThree.WaitForNextResponse(WaitTimeout));

                List<OperationResponse> responseList = peerThree.GetResponseList();
                Assert.AreEqual(1, responseList.Count);
                OperationResponse response = responseList[0];
                Assert.AreEqual(3, response.Parameters[(byte)ParameterKey.ActorNr]);

                // peer 3: receive own join event
                Assert.IsTrue(peerThree.WaitForNextEvent(WaitTimeout));
                List<EventData> eventList = peerThree.GetEventList();
                if (eventList.Count < 2)
                {
                    Assert.IsTrue(peerThree.WaitForNextEvent(WaitTimeout));
                    eventList.AddRange(peerThree.GetEventList());
                }

                Assert.AreEqual(2, eventList.Count);
                @event = eventList[0];
                Assert.AreEqual(LiteOpCode.Join, @event.Code);
                Assert.AreEqual(3, @event.Parameters[(byte)ParameterKey.ActorNr]);

                @event = eventList[1];
                Assert.AreEqual(Code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                // peer 1: receive join event of peer 3
                ReceiveJoinEvent(peerOne, 3);

                // peer 2: receive join event of peer 3
                ReceiveJoinEvent(peerTwo, 3);
            }
            finally
            {
                PeerHelper.SimulateDisconnect(litePeerOne);
                PeerHelper.SimulateDisconnect(litePeerTwo);
                PeerHelper.SimulateDisconnect(litePeerThree);
            }
        }

        /// <summary>
        ///   Tests if events are deleted when there is no content.
        /// </summary>
        [Test]
        public void RaiseEventCacheMerge2()
        {
            var peerOne = new DummyPeer();
            var peerTwo = new DummyPeer();
            var peerThree = new DummyPeer();
            var litePeerOne = new TestLitePeer(peerOne.Protocol, peerOne);
            var litePeerTwo = new TestLitePeer(peerTwo.Protocol, peerTwo);
            var litePeerThree = new TestLitePeer(peerThree.Protocol, peerThree);

            try
            {
                // peer 1: join
                JoinPeer(peerOne, litePeerOne, 1);

                // peer 2: join 
                JoinPeer(peerTwo, litePeerTwo, 2);

                // peer 1: receive join event of peer 2
                ReceiveJoinEvent(peerOne, 2);

                // peer 1: send to all others
                byte code = 100;
                var data = new Hashtable { { 1, "value1" } };
                var request = GetRaiseEventRequest(code, data, (byte)CacheOperation.MergeCache, null, null);
                PeerHelper.InvokeOnOperationRequest(litePeerOne, request, new SendParameters());

                // Peer 2: Receive data
                var @event = ReceiveEvent(peerTwo, code);
                Assert.AreEqual(code, @event.Code);
                var @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                code = 101;
                data = new Hashtable { { 1, "value2" } };
                request = GetRaiseEventRequest(code, data, (byte)CacheOperation.MergeCache, null, null);
                PeerHelper.InvokeOnOperationRequest(litePeerTwo, request, new SendParameters());

                // Peer 1: Receive data
                @event = ReceiveEvent(peerOne, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value2", @eventData[1]);

                // Peer 1: delete event
                code = 100;
                data = null;
                request = GetRaiseEventRequest(code, data, (byte)CacheOperation.MergeCache, null, null);
                PeerHelper.InvokeOnOperationRequest(litePeerOne, request, new SendParameters());

                // Peer 2: Receive data
                @event = ReceiveEvent(peerTwo, code);
                Assert.AreEqual(code, @event.Code);
                Assert.IsFalse(@event.Parameters.ContainsKey((byte)ParameterKey.Data));

                // Peer 2: add more to event
                code = 101;
                data = new Hashtable { { 2, "value3" }, { 3, "value4" } };
                request = GetRaiseEventRequest(code, data, (byte)CacheOperation.MergeCache, null, null);
                PeerHelper.InvokeOnOperationRequest(litePeerTwo, request, new SendParameters());

                // Peer 1: Receive data
                @event = ReceiveEvent(peerOne, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value3", @eventData[2]);
                Assert.AreEqual("value4", @eventData[3]);
                Assert.IsFalse(@eventData.ContainsKey(1));

                // Peer 2: remove from event
                code = 101;
                data = new Hashtable { { 2, null } };
                request = GetRaiseEventRequest(code, data, (byte)CacheOperation.MergeCache, null, null);
                PeerHelper.InvokeOnOperationRequest(litePeerTwo, request, new SendParameters());

                // Peer 1: Receive data
                @event = ReceiveEvent(peerOne, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.IsNull(@eventData[2]);
                Assert.IsFalse(@eventData.ContainsKey(1));
                Assert.IsTrue(@eventData.ContainsKey(2));
                Assert.IsFalse(@eventData.ContainsKey(3));

                // peer 3: join 
                request = GetJoinRequest();
                PeerHelper.InvokeOnOperationRequest(litePeerThree, request, new SendParameters());
                Assert.IsTrue(peerThree.WaitForNextResponse(WaitTimeout));

                List<OperationResponse> responseList = peerThree.GetResponseList();
                Assert.AreEqual(1, responseList.Count);
                OperationResponse response = responseList[0];
                Assert.AreEqual(3, response.Parameters[(byte)ParameterKey.ActorNr]);

                // peer 3: receive own join event and event 101 - event 100 was removed
                Assert.IsTrue(peerThree.WaitForNextEvent(WaitTimeout));
                List<EventData> eventList = peerThree.GetEventList();
                if (eventList.Count < 2)
                {
                    Assert.IsTrue(peerThree.WaitForNextEvent(WaitTimeout));
                    eventList.AddRange(peerThree.GetEventList());
                }

                Assert.AreEqual(2, eventList.Count);
                @event = eventList[0];
                Assert.AreEqual(LiteOpCode.Join, @event.Code);
                Assert.AreEqual(3, @event.Parameters[(byte)ParameterKey.ActorNr]);

                code = 101;
                @event = eventList[1];
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value2", @eventData[1]);
                Assert.AreEqual("value4", @eventData[3]);
                Assert.IsFalse(@eventData.ContainsKey(2));

                // peer 1: receive join event of peer 3
                ReceiveJoinEvent(peerOne, 3);

                // peer 2: receive join event of peer 3
                ReceiveJoinEvent(peerTwo, 3);
            }
            finally
            {
                PeerHelper.SimulateDisconnect(litePeerOne);
                PeerHelper.SimulateDisconnect(litePeerTwo);
                PeerHelper.SimulateDisconnect(litePeerThree);
            }
        }

        /// <summary>
        ///   Tests if events are removed.
        /// </summary>
        [Test]
        public void RaiseEventCacheRemove()
        {
            var peerOne = new DummyPeer();
            var peerTwo = new DummyPeer();
            var peerThree = new DummyPeer();
            var litePeerOne = new TestLitePeer(peerOne.Protocol, peerOne);
            var litePeerTwo = new TestLitePeer(peerTwo.Protocol, peerTwo);
            var litePeerThree = new TestLitePeer(peerThree.Protocol, peerThree);

            try
            {
                // peer 1: join
                JoinPeer(peerOne, litePeerOne, 1);

                // peer 2: join 
                JoinPeer(peerTwo, litePeerTwo, 2);

                // peer 1: receive join event of peer 2
                ReceiveJoinEvent(peerOne, 2);

                // peer 1: send to all others
                byte code = 100;
                var data = new Hashtable { { 1, "value1" } };
                var request = GetRaiseEventRequest(code, data, (byte)CacheOperation.ReplaceCache, null, null);
                PeerHelper.InvokeOnOperationRequest(litePeerOne, request, new SendParameters());

                // Peer 2: Receive data
                var @event = ReceiveEvent(peerTwo, code);
                Assert.AreEqual(code, @event.Code);
                var @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                data = new Hashtable { { 2, "value2" } };
                request = GetRaiseEventRequest(code, data, (byte)CacheOperation.RemoveCache, null, null);
                PeerHelper.InvokeOnOperationRequest(litePeerOne, request, new SendParameters());

                // Peer 2: Receive data
                @event = ReceiveEvent(peerTwo, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value2", @eventData[2]);
                Assert.IsFalse(@eventData.ContainsKey(1));

                // Peer 1: send event 101
                code++;
                data = new Hashtable { { 3, "value3" } };
                request = GetRaiseEventRequest(code, data, (byte)CacheOperation.MergeCache, null, null);
                PeerHelper.InvokeOnOperationRequest(litePeerOne, request, new SendParameters());

                // Peer 2: Receive data
                @event = ReceiveEvent(peerTwo, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value3", @eventData[3]);
                Assert.IsFalse(@eventData.ContainsKey(1));
                Assert.IsFalse(@eventData.ContainsKey(2));

                // peer 3: join 
                request = GetJoinRequest();
                PeerHelper.InvokeOnOperationRequest(litePeerThree, request, new SendParameters());
                Assert.IsTrue(peerThree.WaitForNextResponse(WaitTimeout));

                List<OperationResponse> responseList = peerThree.GetResponseList();
                Assert.AreEqual(1, responseList.Count);
                OperationResponse response = responseList[0];
                Assert.AreEqual(3, response.Parameters[(byte)ParameterKey.ActorNr]);

                // peer 3: receive own join event and event 101 - event 100 was removed
                Assert.IsTrue(peerThree.WaitForNextEvent(WaitTimeout));
                List<EventData> eventList = peerThree.GetEventList();
                if (eventList.Count < 2)
                {
                    Assert.IsTrue(peerThree.WaitForNextEvent(WaitTimeout));
                    eventList.AddRange(peerThree.GetEventList());
                }

                Assert.AreEqual(2, eventList.Count);
                @event = eventList[0];
                Assert.AreEqual(LiteOpCode.Join, @event.Code);
                Assert.AreEqual(3, @event.Parameters[(byte)ParameterKey.ActorNr]);

                @event = eventList[1];
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value3", @eventData[3]);
                Assert.IsFalse(@eventData.ContainsKey(2));
                Assert.IsFalse(@eventData.ContainsKey(1));

                // peer 1: receive join event of peer 3
                ReceiveJoinEvent(peerOne, 3);

                // peer 2: receive join event of peer 3
                ReceiveJoinEvent(peerTwo, 3);
            }
            finally
            {
                PeerHelper.SimulateDisconnect(litePeerOne);
                PeerHelper.SimulateDisconnect(litePeerTwo);
                PeerHelper.SimulateDisconnect(litePeerThree);
            }
        }

        /// <summary>
        ///   Tests if events are replaced.
        /// </summary>
        [Test]
        public void RaiseEventCacheReplace()
        {
            var peerOne = new DummyPeer();
            var peerTwo = new DummyPeer();
            var peerThree = new DummyPeer();
            var litePeerOne = new TestLitePeer(peerOne.Protocol, peerOne);
            var litePeerTwo = new TestLitePeer(peerTwo.Protocol, peerTwo);
            var litePeerThree = new TestLitePeer(peerThree.Protocol, peerThree);

            try
            {
                // peer 1: join
                JoinPeer(peerOne, litePeerOne, 1);

                // peer 2: join 
                JoinPeer(peerTwo, litePeerTwo, 2);

                // peer 1: receive join event of peer 2
                ReceiveJoinEvent(peerOne, 2);

                // peer 1: send to all others
                const byte Code = 100;
                var data = new Hashtable { { 1, "value1" } };
                var request = GetRaiseEventRequest(Code, data, (byte)CacheOperation.ReplaceCache, null, null);
                PeerHelper.InvokeOnOperationRequest(litePeerOne, request, new SendParameters());

                // Peer 2: Receive data
                var @event = ReceiveEvent(peerTwo, Code);
                Assert.AreEqual(Code, @event.Code);
                var @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                data = new Hashtable { { 2, "value2" } };
                request = GetRaiseEventRequest(Code, data, (byte)CacheOperation.ReplaceCache, null, null);
                PeerHelper.InvokeOnOperationRequest(litePeerOne, request, new SendParameters());

                // Peer 2: Receive data
                @event = ReceiveEvent(peerTwo, Code);
                Assert.AreEqual(Code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value2", @eventData[2]);
                Assert.IsFalse(@eventData.ContainsKey(1));

                // peer 3: join 
                request = GetJoinRequest();
                PeerHelper.InvokeOnOperationRequest(litePeerThree, request, new SendParameters());
                Assert.IsTrue(peerThree.WaitForNextResponse(WaitTimeout));

                List<OperationResponse> responseList = peerThree.GetResponseList();
                Assert.AreEqual(1, responseList.Count);
                OperationResponse response = responseList[0];
                Assert.AreEqual(3, response.Parameters[(byte)ParameterKey.ActorNr]);

                // peer 3: receive own join event and event 101 - event 100 was removed
                Assert.IsTrue(peerThree.WaitForNextEvent(WaitTimeout));
                List<EventData> eventList = peerThree.GetEventList();
                if (eventList.Count < 2)
                {
                    Assert.IsTrue(peerThree.WaitForNextEvent(WaitTimeout));
                    eventList.AddRange(peerThree.GetEventList());
                }

                Assert.AreEqual(2, eventList.Count);
                @event = eventList[0];
                Assert.AreEqual(LiteOpCode.Join, @event.Code);
                Assert.AreEqual(3, @event.Parameters[(byte)ParameterKey.ActorNr]);

                @event = eventList[1];
                Assert.AreEqual(Code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value2", @eventData[2]);
                Assert.IsFalse(@eventData.ContainsKey(1));

                // peer 1: receive join event of peer 3
                ReceiveJoinEvent(peerOne, 3);

                // peer 2: receive join event of peer 3
                ReceiveJoinEvent(peerTwo, 3);
            }
            finally
            {
                PeerHelper.SimulateDisconnect(litePeerOne);
                PeerHelper.SimulateDisconnect(litePeerTwo);
                PeerHelper.SimulateDisconnect(litePeerThree);
            }
        }
       
        /// <summary>
        ///   Test for sending events to receiver groups.
        /// </summary>
        [Test]
        public void RaiseEventReceiverGroups()
        {
            var peerOne = new DummyPeer();
            var peerTwo = new DummyPeer();
            var peerThree = new DummyPeer();
            var litePeerOne = new TestLitePeer(peerOne.Protocol, peerOne);
            var litePeerTwo = new TestLitePeer(peerTwo.Protocol, peerTwo);
            var litePeerThree = new TestLitePeer(peerThree.Protocol, peerThree);

            try
            {
                // peer 1: join
                JoinPeer(peerOne, litePeerOne, 1);

                // peer 2: join 
                JoinPeer(peerTwo, litePeerTwo, 2);

                // peer 1: receive join event of peer 2
                ReceiveJoinEvent(peerOne, 2);

                // peer 2: join 
                JoinPeer(peerThree, litePeerThree, 3);

                // peer 1: receive join event of peer 3
                ReceiveJoinEvent(peerOne, 3);

                // peer 2: receive join event of peer 3
                ReceiveJoinEvent(peerTwo, 3);

                // peer 1: send to all others
                byte code = 100;
                var data = new Hashtable { { 1, "value1" } };
                var request = GetRaiseEventRequest(code, data, null, null, null);
                PeerHelper.InvokeOnOperationRequest(litePeerOne, request, new SendParameters());

                // Peer 2 + 3: Receive data
                var @event = ReceiveEvent(peerTwo, code);
                Assert.AreEqual(code, @event.Code);
                var @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                @event = ReceiveEvent(peerThree, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                // peer 1: send to all others again
                code++;
                request = GetRaiseEventRequest(code, data, null, ReceiverGroup.Others, null);
                PeerHelper.InvokeOnOperationRequest(litePeerOne, request, new SendParameters());

                // Peer 2 + 3: Receive data
                @event = ReceiveEvent(peerTwo, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                @event = ReceiveEvent(peerThree, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                // peer 1: send to all 
                code++;
                request = GetRaiseEventRequest(code, data, null, ReceiverGroup.All, null);
                PeerHelper.InvokeOnOperationRequest(litePeerOne, request, new SendParameters());

                // Peer 1 + 2 + 3: Receive data
                @event = ReceiveEvent(peerOne, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                @event = ReceiveEvent(peerTwo, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                @event = ReceiveEvent(peerThree, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                // peer 1: send to master 
                code++;
                request = GetRaiseEventRequest(code, data, null, ReceiverGroup.MasterClient, null);
                PeerHelper.InvokeOnOperationRequest(litePeerOne, request, new SendParameters());

                // Peer 1: Receive data
                @event = ReceiveEvent(peerOne, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                // peer 2: send to master 
                code++;
                request = GetRaiseEventRequest(code, data, null, ReceiverGroup.MasterClient, null);
                PeerHelper.InvokeOnOperationRequest(litePeerTwo, request, new SendParameters());

                // Peer 1: Receive data
                @event = ReceiveEvent(peerOne, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                // peer 3: send to master 
                code++;
                request = GetRaiseEventRequest(code, data, null, ReceiverGroup.MasterClient, null);
                PeerHelper.InvokeOnOperationRequest(litePeerThree, request, new SendParameters());

                // Peer 1: Receive data
                @event = ReceiveEvent(peerOne, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                // peer 1: leave
                request = GetLeaveRequest();
                PeerHelper.InvokeOnOperationRequest(litePeerOne, request, new SendParameters());

                // peer 2+3: receive leave event
                ReceiveEvent(peerTwo, (byte)EventCode.Leave);
                ReceiveEvent(peerThree, (byte)EventCode.Leave);

                // peer 3: send to master 
                code++;
                request = GetRaiseEventRequest(code, data, null, ReceiverGroup.MasterClient, null);
                PeerHelper.InvokeOnOperationRequest(litePeerThree, request, new SendParameters());

                // Peer 2: Receive data
                @event = ReceiveEvent(peerTwo, code);
                Assert.AreEqual(code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);
            }
            finally
            {
                PeerHelper.SimulateDisconnect(litePeerOne);
                PeerHelper.SimulateDisconnect(litePeerTwo);
                PeerHelper.SimulateDisconnect(litePeerThree);
            }
        }

        /// <summary>
        ///   Tests if existing and new peer receive cached events.
        /// </summary>
        [Test]
        public void RaiseEventCacheRoom()
        {
            var peerOne = new DummyPeer();
            var peerTwo = new DummyPeer();
            var peerThree = new DummyPeer();
            var litePeerOne = new TestLitePeer(peerOne.Protocol, peerOne);
            var litePeerTwo = new TestLitePeer(peerTwo.Protocol, peerTwo);
            var litePeerThree = new TestLitePeer(peerThree.Protocol, peerThree);

            try
            {
                // peer 1: join
                var customJoinparams = new Dictionary<byte, object> { { (byte)ParameterKey.DeleteCacheOnLeave, true } };
                JoinPeer(peerOne, litePeerOne, 1, customJoinparams);

                // peer 2: join 
                JoinPeer(peerTwo, litePeerTwo, 2);

                // peer 1: receive join event of peer 2
                ReceiveJoinEvent(peerOne, 2);

                // peer 1: send to all others
                const byte Code = 100;
                var data = new Hashtable { { 1, "value1" } };
                var request = GetRaiseEventRequest(Code, data, (byte)CacheOperation.AddToRoomCache, null, null);
                PeerHelper.InvokeOnOperationRequest(litePeerOne, request, new SendParameters());

                // Peer 2: Receive data
                var @event = ReceiveEvent(peerTwo, Code);
                Assert.AreEqual(Code, @event.Code);
                var @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                // peer 3: join 
                request = GetJoinRequest();
                PeerHelper.InvokeOnOperationRequest(litePeerThree, request, new SendParameters());
                Assert.IsTrue(peerThree.WaitForNextResponse(WaitTimeout));

                List<OperationResponse> responseList = peerThree.GetResponseList();
                Assert.AreEqual(1, responseList.Count);
                OperationResponse response = responseList[0];
                Assert.AreEqual(3, response.Parameters[(byte)ParameterKey.ActorNr]);

                // peer 3: receive own join event
                Assert.IsTrue(peerThree.WaitForNextEvent(WaitTimeout));
                List<EventData> eventList = peerThree.GetEventList();
                if (eventList.Count < 2)
                {
                    Assert.IsTrue(peerThree.WaitForNextEvent(WaitTimeout));
                    eventList.AddRange(peerThree.GetEventList());
                }

                Assert.AreEqual(2, eventList.Count);
                @event = eventList[0];
                Assert.AreEqual(LiteOpCode.Join, @event.Code);
                Assert.AreEqual(3, @event.Parameters[(byte)ParameterKey.ActorNr]);

                @event = eventList[1];
                Assert.AreEqual(Code, @event.Code);
                @eventData = (Hashtable)@event.Parameters[(byte)ParameterKey.Data];
                Assert.AreEqual("value1", @eventData[1]);

                // receive join event of peer 3
                ReceiveJoinEvent(peerOne, 3);
                ReceiveJoinEvent(peerTwo, 3);
            }
            finally
            {
                PeerHelper.SimulateDisconnect(litePeerOne);
                PeerHelper.SimulateDisconnect(litePeerTwo);
                PeerHelper.SimulateDisconnect(litePeerThree);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   The get join request.
        /// </summary>
        /// <returns>
        ///   a join request
        /// </returns>
        private static OperationRequest GetJoinRequest()
        {
            return GetJoinRequest(null);
        }

        /// <summary>
        ///   The get join request.
        /// </summary>
        /// <returns>
        ///   a join request
        /// </returns>
        private static OperationRequest GetJoinRequest(Dictionary<byte, object> customParameter)
        {
            var request = new OperationRequest { OperationCode = LiteOpCode.Join, Parameters = new Dictionary<byte, object>() };
            request.Parameters.Add((byte)ParameterKey.GameId, "testGame");

            if (customParameter != null)
            {
                foreach (var keyValue in customParameter)
                {
                    request.Parameters.Add(keyValue.Key, keyValue.Value);
                }
            }
        
            return request;
        }

        /// <summary>
        ///   The get leave request.
        /// </summary>
        /// <returns>
        ///   a leave request
        /// </returns>
        private static OperationRequest GetLeaveRequest()
        {
            var request = new OperationRequest { OperationCode = LiteOpCode.Leave };
            return request;
        }

        /// <summary>
        ///   Creates a RaiseEvent request.
        /// </summary>
        /// <param name = "eventCode">
        ///   The event code.
        /// </param>
        /// <param name = "data">
        ///   The data.
        /// </param>
        /// <param name = "cache">
        ///   The cache.
        /// </param>
        /// <param name = "receiverGroup">
        ///   The receiver group.
        /// </param>
        /// <param name = "targetActors">
        ///   The target actors.
        /// </param>
        /// <returns>
        ///   An <see cref = "OperationRequest" />.
        /// </returns>
        private static OperationRequest GetRaiseEventRequest(byte eventCode, Hashtable data, byte? cache, ReceiverGroup? receiverGroup, int[] targetActors)
        {
            var @params = new Dictionary<byte, object> { { (byte)ParameterKey.Code, eventCode } };
            if (data != null)
            {
                @params.Add((byte)ParameterKey.Data, data);
            }

            if (cache.HasValue)
            {
                @params.Add((byte)ParameterKey.Cache, cache.Value);
            }

            if (receiverGroup.HasValue)
            {
                @params.Add((byte)ParameterKey.ReceiverGroup, (byte)receiverGroup.Value);
            }

            if (targetActors != null)
            {
                @params.Add((byte)ParameterKey.Actors, targetActors);
            }

            return new OperationRequest { OperationCode = LiteOpCode.RaiseEvent, Parameters = @params };
        }

        private static void JoinPeer(DummyPeer peer, TestLitePeer litePeer, int expectedNumber)
        {
            JoinPeer(peer, litePeer, expectedNumber, null);
        }

        /// <summary>
        ///   Sends a join operation and verifies the response.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "litePeer">
        ///   The lite peer.
        /// </param>
        /// <param name = "expectedNumber">
        ///   The expected number.
        /// </param>
        private static void JoinPeer(DummyPeer peer, TestLitePeer litePeer, int expectedNumber, Dictionary<byte, object> customParameters)
        {
            OperationRequest request = GetJoinRequest(customParameters);
            PeerHelper.InvokeOnOperationRequest(litePeer, request, new SendParameters());
            Assert.IsTrue(peer.WaitForNextResponse(WaitTimeout));

            List<OperationResponse> responseList = peer.GetResponseList();
            Assert.AreEqual(1, responseList.Count);
            OperationResponse response = responseList[0];
            Assert.AreEqual(expectedNumber, response.Parameters[(byte)ParameterKey.ActorNr]);

            // peer 1: receive own join event
            Assert.IsTrue(peer.WaitForNextEvent(WaitTimeout));
            List<EventData> eventList = peer.GetEventList();
            Assert.AreEqual(1, eventList.Count);
            EventData eventData = eventList[0];
            Assert.AreEqual(LiteOpCode.Join, eventData.Code);
            Assert.AreEqual(expectedNumber, eventData.Parameters[(byte)ParameterKey.ActorNr]);
        }

        /// <summary>
        ///   Waiting for an event with the given event code.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "expecteCode">
        ///   The expecte code.
        /// </param>
        /// <returns>
        ///   The received <see cref = "EventData" />.
        /// </returns>
        private static EventData ReceiveEvent(DummyPeer peer, byte expecteCode)
        {
            Assert.IsTrue(peer.WaitForNextEvent(WaitTimeout));
            var eventList = peer.GetEventList();
            Assert.AreEqual(1, eventList.Count);
            var eventData = eventList[0];
            Assert.AreEqual(expecteCode, eventData.Code);
            return eventData;
        }

        /// <summary>
        ///   Receive a certain amount of events.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "count">
        ///   The count.
        /// </param>
        /// <returns>
        ///   An array of <see cref = "EventData" />.
        /// </returns>
        private static EventData[] ReceiveEvents(DummyPeer peer, int count)
        {
            Assert.IsTrue(peer.WaitForNextEvent(WaitTimeout));
            var eventList = peer.GetEventList();
            while (eventList.Count < count)
            {
                Assert.IsTrue(peer.WaitForNextEvent(WaitTimeout));
                eventList.AddRange(peer.GetEventList());
            }

            Assert.AreEqual(count, eventList.Count);
            return eventList.ToArray();
        }

        /// <summary>
        ///   Waiting for a join event of a certain actor.
        /// </summary>
        /// <param name = "peerOne">
        ///   The peer one.
        /// </param>
        /// <param name = "expectedNumber">
        ///   The expected actor number.
        /// </param>
        private static void ReceiveJoinEvent(DummyPeer peerOne, int expectedNumber)
        {
            var eventData = ReceiveEvent(peerOne, LiteOpCode.Join);
            Assert.AreEqual(expectedNumber, eventData.Parameters[(byte)ParameterKey.ActorNr]);
        }

        #endregion
    }
}