// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpTests.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The test base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;

    using ExitGames.Logging;

    using Lite.Operations;
    using Lite.Tests.Client;

    using NUnit.Framework;

    using Photon.SocketServer;

    /// <summary>
    ///   The test base.
    /// </summary>
    [TestFixture]
    public class TcpTests
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TcpTests" /> class.
        /// </summary>
        public TcpTests()
        {
            this.WaitTime = 5000;
        }

        #endregion

        #region Enums

        /// <summary>
        ///   The property keys.
        /// </summary>
        protected enum PropertyKeys
        {
            /// <summary>
            ///   The actor name.
            /// </summary>
            ActorName,

            /// <summary>
            ///   The actor age.
            /// </summary>
            ActorAge,

            /// <summary>
            ///   The room color.
            /// </summary>
            RoomColor,

            /// <summary>
            ///   The room temperature.
            /// </summary>
            RoomTemperature
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets WaitTime.
        /// </summary>
        protected int WaitTime { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///   The join.
        /// </summary>
        [Test]
        public void Join()
        {
            string roomName = CreateRandomRoomName();

            using (TestClient client = this.InitClient())
            {
                var request = new OperationRequest { OperationCode = (byte)OperationCode.Join, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.GameId, roomName);

                client.SendOperationRequest(request);
                OperationResponse response = client.WaitForOperationResponse(this.WaitTime);
                EventData eventArgs = client.WaitForEvent(this.WaitTime);

                // check operation params
                CheckDefaultOperationParameters(response, OperationCode.Join);
                CheckParam(response, ParameterKey.ActorNr, 1);

                // check event params
                CheckDefaultEventParameters(eventArgs, OperationCode.Join, 1);
                CheckEventParamExists(eventArgs, ParameterKey.Actors);
            }
        }

        /// <summary>
        ///   The join with channel.
        /// </summary>
        [Test]
        public void JoinWithChannel()
        {
            string roomName = CreateRandomRoomName();

            using (TestClient client = this.InitClient())
            {
                var request = new OperationRequest { OperationCode = (byte)OperationCode.Join, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.GameId, roomName);
                client.SendOperationRequest(request);

                OperationResponse response = client.WaitForOperationResponse(this.WaitTime);
                EventData eventArgs = client.WaitForEvent(this.WaitTime);

                // check operation params
                CheckDefaultOperationParameters(response, OperationCode.Join);
                CheckParam(response, ParameterKey.ActorNr, 1);

                // check event params
                CheckDefaultEventParameters(eventArgs, OperationCode.Join, 1);
                CheckEventParamExists(eventArgs, ParameterKey.Actors);
            }
        }

        /// <summary>
        ///   The join with properties int.
        /// </summary>
        [Test]
        public virtual void JoinWithPropertiesInt()
        {
            string roomName = CreateRandomRoomName();

            using (TestClient client = this.InitClient())
            {
                var actorProperties = new Hashtable { { (int)PropertyKeys.ActorName, "Bert" } };

                var request = new OperationRequest { OperationCode = (byte)OperationCode.Join, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.GameId, roomName);
                request.Parameters.Add((byte)ParameterKey.ActorProperties, actorProperties);
                client.SendOperationRequest(request);

                OperationResponse response = client.WaitForOperationResponse(this.WaitTime);
                EventData eventArgs = client.WaitForEvent(this.WaitTime);

                // check operation params
                CheckDefaultOperationParameters(response, OperationCode.Join);
                CheckParam(response, ParameterKey.ActorNr, 1);

                // check event params
                CheckDefaultEventParameters(eventArgs, OperationCode.Join, 1);
                CheckEventParamExists(eventArgs, ParameterKey.Actors);

                // ----------------------------------------------------------------
                // set some additional actor properties 
                // ----------------------------------------------------------------
                actorProperties = new Hashtable { { (int)PropertyKeys.ActorAge, 28 } };

                request = new OperationRequest { OperationCode = (byte)OperationCode.SetProperties, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.ActorNr, 1);
                request.Parameters.Add((byte)ParameterKey.Properties, actorProperties);

                client.SendOperationRequest(request);
                response = client.WaitForOperationResponse(this.WaitTime);

                CheckDefaultOperationParameters(response, OperationCode.SetProperties);

                // ----------------------------------------------------------------
                // get properties from actor  
                // ----------------------------------------------------------------
                var propertyKeys = new[] { (int)PropertyKeys.ActorName, (int)PropertyKeys.ActorAge };

                request = new OperationRequest { OperationCode = (byte)OperationCode.GetProperties, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.Properties, (byte)PropertyType.Actor);
                request.Parameters.Add((byte)ParameterKey.ActorProperties, propertyKeys);
                request.Parameters.Add((byte)ParameterKey.Actors, new[] { 1 });
                client.SendOperationRequest(request);
                response = client.WaitForOperationResponse(this.WaitTime);

                CheckDefaultOperationParameters(response, OperationCode.GetProperties);
                CheckParamExists(response, ParameterKey.ActorProperties);
                var properties = (Hashtable)response.Parameters[(byte)ParameterKey.ActorProperties];
                actorProperties = (Hashtable)properties[1];
                Assert.AreEqual("Bert", actorProperties[(int)PropertyKeys.ActorName]);
                Assert.AreEqual(28, actorProperties[(int)PropertyKeys.ActorAge]);

                // ----------------------------------------------------------------
                // set room properties
                // ----------------------------------------------------------------
                var roomProperties = new Hashtable { { (int)PropertyKeys.RoomColor, "Red" }, { (int)PropertyKeys.RoomTemperature, 12 } };

                request = new OperationRequest { OperationCode = (byte)OperationCode.SetProperties, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.Properties, roomProperties);

                client.SendOperationRequest(request);
                response = client.WaitForOperationResponse(this.WaitTime);

                CheckDefaultOperationParameters(response, OperationCode.SetProperties);

                // ----------------------------------------------------------------
                // get room properties
                // ----------------------------------------------------------------
                propertyKeys = new[] { (int)PropertyKeys.RoomColor, (int)PropertyKeys.RoomTemperature };

                request = new OperationRequest { OperationCode = (byte)OperationCode.GetProperties, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.Properties, (byte)PropertyType.Game);
                request.Parameters.Add((byte)ParameterKey.GameProperties, propertyKeys);

                client.SendOperationRequest(request);
                response = client.WaitForOperationResponse(this.WaitTime);

                CheckDefaultOperationParameters(response, OperationCode.GetProperties);
                CheckParamExists(response, ParameterKey.GameProperties);
                properties = (Hashtable)response.Parameters[(byte)ParameterKey.GameProperties];
                Assert.AreEqual("Red", properties[(int)PropertyKeys.RoomColor]);
                Assert.AreEqual(12, properties[(int)PropertyKeys.RoomTemperature]);
            }
        }

        /// <summary>
        ///   The join with properties string.
        /// </summary>
        [Test]
        public void JoinWithPropertiesString()
        {
            string roomName = CreateRandomRoomName();

            using (TestClient client = this.InitClient())
            {
                var actorProperties = new Hashtable { { "Name", "Bert" } };

                var request = new OperationRequest { OperationCode = (byte)OperationCode.Join, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.GameId, roomName);
                request.Parameters.Add((byte)ParameterKey.ActorProperties, actorProperties);

                client.SendOperationRequest(request);
                OperationResponse response = client.WaitForOperationResponse(this.WaitTime);
                EventData eventArgs = client.WaitForEvent(this.WaitTime);

                // check operation params
                CheckDefaultOperationParameters(response, OperationCode.Join);
                CheckParam(response, ParameterKey.ActorNr, 1);

                // check event params
                CheckDefaultEventParameters(eventArgs, OperationCode.Join, 1);
                CheckEventParamExists(eventArgs, ParameterKey.Actors);

                // ----------------------------------------------------------------
                // set some additional actor properties 
                // ----------------------------------------------------------------
                actorProperties = new Hashtable { { "Age", 28 } };

                request = new OperationRequest { OperationCode = (byte)OperationCode.SetProperties, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.ActorNr, 1);
                request.Parameters.Add((byte)ParameterKey.Properties, actorProperties);

                client.SendOperationRequest(request);
                response = client.WaitForOperationResponse(this.WaitTime);

                CheckDefaultOperationParameters(response, OperationCode.SetProperties);

                // ----------------------------------------------------------------
                // get properties from actor  
                // ----------------------------------------------------------------
                var propertyKeys = new[] { "Name", "Age" };
                request = new OperationRequest { OperationCode = (byte)OperationCode.GetProperties, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.Properties, (byte)PropertyType.Actor);
                request.Parameters.Add((byte)ParameterKey.GameProperties, propertyKeys);

                client.SendOperationRequest(request);
                response = client.WaitForOperationResponse(this.WaitTime);

                CheckDefaultOperationParameters(response, OperationCode.GetProperties);
                CheckParamExists(response, ParameterKey.ActorProperties);

                var properties = (Hashtable)response.Parameters[(byte)ParameterKey.ActorProperties];
                object actorKey = this.GetActorNumberKey(1);
                actorProperties = (Hashtable)properties[actorKey];
                Assert.AreEqual("Bert", actorProperties["Name"]);
                Assert.AreEqual(28, actorProperties["Age"]);

                // ----------------------------------------------------------------
                // set room properties
                // ----------------------------------------------------------------
                var roomProperties = new Hashtable { { "Color", "Red" }, { "Temperature", 12 } };

                request = new OperationRequest { OperationCode = (byte)OperationCode.SetProperties, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.Properties, roomProperties);

                client.SendOperationRequest(request);
                response = client.WaitForOperationResponse(this.WaitTime);

                CheckDefaultOperationParameters(response, OperationCode.SetProperties);

                // ----------------------------------------------------------------
                // get room properties
                // ----------------------------------------------------------------
                propertyKeys = new[] { "Color", "Temperature" };

                request = new OperationRequest { OperationCode = (byte)OperationCode.GetProperties, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.Properties, (byte)PropertyType.Game);
                request.Parameters.Add((byte)ParameterKey.GameProperties, propertyKeys);

                client.SendOperationRequest(request);
                response = client.WaitForOperationResponse(this.WaitTime);

                CheckDefaultOperationParameters(response, OperationCode.GetProperties);
                CheckParamExists(response, ParameterKey.GameProperties);
                properties = (Hashtable)response.Parameters[(byte)ParameterKey.GameProperties];
                Assert.AreEqual("Red", properties["Color"]);
                Assert.AreEqual(12, properties["Temperature"]);
            }
        }

        /// <summary>
        ///   The join with property broadcast.
        /// </summary>
        [Test]
        public void JoinWithPropertyBroadcast()
        {
            string roomName = CreateRandomRoomName();

            using (TestClient client1 = this.InitClient())
            using (TestClient client2 = this.InitClient())
            {
                var actorProperties1 = new Hashtable { { "Name", "Bert" } };
                var actorProperties2 = new Hashtable { { "Name", "Ernie" } };
                this.JoinClientsToRoom(roomName, client1, client2, actorProperties1, actorProperties2, true);
            }
        }

        /// <summary>
        ///   The send custom event.
        /// </summary>
        [Test]
        public void SendCustomEvent()
        {
            string roomName = CreateRandomRoomName();

            // cretae clients
            using (TestClient client1 = this.InitClient())
            using (TestClient client2 = this.InitClient())
            {
                // join clients to room
                this.JoinClientsToRoom(roomName, client1, client2);

                var array = new int[1][];
                array[0] = new[] { 1, 2, 3, };
                var eventData = new Hashtable { { 1, "TestData" }, { 2, array } };

                var request = new OperationRequest { OperationCode = (byte)OperationCode.RaiseEvent, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.Code, (byte)OperationCode.RaiseEvent);
                request.Parameters.Add((byte)ParameterKey.Data, eventData);
                client1.SendOperationRequest(request);

                // no response anymore for reliable raise event
                ////OperationResponse response = client1.WaitForOperationResponse(this.WaitTime);
                ////CheckDefaultOperationParameters(response, OperationCodes.RaiseEvent);
                EventData eventArgs2 = client2.WaitForEvent(this.WaitTime);
                CheckDefaultEventParameters(eventArgs2, OperationCode.RaiseEvent, 1);

                // leave from room
                this.LeaveClientsFromRoom(client1, client2);
            }
        }

        /// <summary>
        /// Sends a custom event to a list of actors.
        /// </summary>
        [Test]
        public void SendCustomEventToActorList()
        {
            string roomName = CreateRandomRoomName();

            // cretae clients
            using (TestClient client1 = this.InitClient())
            using (TestClient client2 = this.InitClient())
            {
                // join clients to room
                this.JoinClientsToRoom(roomName, client1, client2);

                var array = new int[1][];
                array[0] = new[] { 1, 2, 3, };
                var eventData = new Hashtable { { 1, "TestData" }, { 2, array } };

                var request = new OperationRequest { OperationCode = (byte)OperationCode.RaiseEvent, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.Code, (byte)OperationCode.RaiseEvent);
                request.Parameters.Add((byte)ParameterKey.Data, eventData);
                request.Parameters.Add((byte)ParameterKey.Actors, new[] { 2 });
                client1.SendOperationRequest(request);

                EventData eventArgs2 = client2.WaitForEvent(this.WaitTime);
                CheckDefaultEventParameters(eventArgs2, OperationCode.RaiseEvent, 1);

                // leave from room
                this.LeaveClientsFromRoom(client1, client2);
            }
        }

        /// <summary>
        ///   The send custom event with large data.
        /// </summary>
        [Test]
        public virtual void SendCustomEventWithLargeData()
        {
            string roomName = CreateRandomRoomName();

            // create clients
            using (TestClient client1 = this.InitClient())
            using (TestClient client2 = this.InitClient())
            {
                // join clients to room
                this.JoinClientsToRoom(roomName, client1, client2);

                var testData = new byte[40 * 1024];
                var eventData = new Hashtable { { 1, testData } };

                var request = new OperationRequest { OperationCode = (byte)OperationCode.RaiseEvent, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.Code, (byte)OperationCode.RaiseEvent);
                request.Parameters.Add((byte)ParameterKey.Data, eventData);
                client1.SendOperationRequest(request);

                ////OperationResponse response = client1.WaitForOperationResponse(this.WaitTime);
                ////CheckDefaultOperationParameters(response, OperationCodes.RaiseEvent);
                EventData eventArgs2 = client2.WaitForEvent(this.WaitTime);
                CheckDefaultEventParameters(eventArgs2, OperationCode.RaiseEvent, 1);

                // leave from room
                this.LeaveClientsFromRoom(client1, client2);
            }
        }

        /// <summary>
        ///   The send ping.
        /// </summary>
        [Test]
        public void SendPing()
        {
            using (TestClient client = this.InitClient())
            {

                var operationRequest = new OperationRequest { OperationCode = (byte)OperationCode.Ping, Parameters = new Dictionary<byte, object>() };
                client.SendOperationRequest(operationRequest);
                OperationResponse response = client.WaitForOperationResponse(this.WaitTime);

                // check operation params
                CheckDefaultOperationParameters(response, OperationCode.Ping);
            }
        }

        /// <summary>
        ///   The set properties with broadcast.
        /// </summary>
        [Test]
        public virtual void SetPropertiesWithBroadcast()
        {
            string roomName = CreateRandomRoomName();

            // init clients
            using (TestClient client1 = this.InitClient())
            using (TestClient client2 = this.InitClient())
            {
                // join clients to room
                this.JoinClientsToRoom(roomName, client1, client2);

                var roomProperties = new Hashtable { { "TestKey", "TestData" } };

                // set room properties
                var request = new OperationRequest { OperationCode = (byte)OperationCode.SetProperties, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.Properties, roomProperties);
                request.Parameters.Add((byte)ParameterKey.Broadcast, true);

                client1.SendOperationRequest(request);
                OperationResponse response = client1.WaitForOperationResponse(this.WaitTime);
                CheckDefaultOperationParameters(response, OperationCode.SetProperties);

                EventData eventArgs2 = client2.WaitForEvent(this.WaitTime);
                CheckDefaultEventParameters(eventArgs2, EventCode.PropertiesChanged, 1);
                CheckEventParam(eventArgs2, ParameterKey.TargetActorNr, 0);

                // set actor properties
                request = new OperationRequest { OperationCode = (byte)OperationCode.SetProperties, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.ActorNr, 1);
                request.Parameters.Add((byte)ParameterKey.Properties, roomProperties);
                request.Parameters.Add((byte)ParameterKey.Broadcast, true);

                client1.SendOperationRequest(request);
                response = client1.WaitForOperationResponse(this.WaitTime);
                CheckDefaultOperationParameters(response, OperationCode.SetProperties);

                eventArgs2 = client2.WaitForEvent(this.WaitTime);
                CheckDefaultEventParameters(eventArgs2, EventCode.PropertiesChanged, 1);
                CheckEventParam(eventArgs2, ParameterKey.TargetActorNr, 1);

                // leave clients
                this.LeaveClientsFromRoom(client1, client2);
            }
        }

        /// <summary>
        ///   The set properties with broadcast v 15.
        /// </summary>
        [Test]
        public virtual void SetPropertiesWithBroadcastV15()
        {
            string roomName = CreateRandomRoomName();

            using (TestClient client1 = this.InitClient())
            using (TestClient client2 = this.InitClient())
            {
                // join clients to room
                this.JoinClientsToRoom(roomName, client1, client2);

                var roomProperties = new Hashtable { { "TestKey", "TestData" }, { "FloatKey", new[] { 1.0f, 2.0f } }, { "DoubleKey", new[] { 10.0d, 20.0d } } };

                // set room properties
                var request = new OperationRequest { OperationCode = (byte)OperationCode.SetProperties, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.Properties, roomProperties);
                request.Parameters.Add((byte)ParameterKey.Broadcast, true);

                client1.SendOperationRequest(request);
                OperationResponse response = client1.WaitForOperationResponse(this.WaitTime);
                CheckDefaultOperationParameters(response, OperationCode.SetProperties);

                EventData eventArgs2 = client2.WaitForEvent(this.WaitTime);
                CheckDefaultEventParameters(eventArgs2, EventCode.PropertiesChanged, 1);
                CheckEventParam(eventArgs2, ParameterKey.TargetActorNr, 0);

                // set actor properties
                request = new OperationRequest { OperationCode = (byte)OperationCode.SetProperties, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.ActorNr, 1);
                request.Parameters.Add((byte)ParameterKey.Properties, roomProperties);
                request.Parameters.Add((byte)ParameterKey.Broadcast, true);

                client1.SendOperationRequest(request);
                response = client1.WaitForOperationResponse(this.WaitTime);
                CheckDefaultOperationParameters(response, OperationCode.SetProperties);

                eventArgs2 = client2.WaitForEvent(this.WaitTime);
                CheckDefaultEventParameters(eventArgs2, EventCode.PropertiesChanged, 1);
                CheckEventParam(eventArgs2, ParameterKey.TargetActorNr, 1);

                // leave clients
                this.LeaveClientsFromRoom(client1, client2);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   The check default event params.
        /// </summary>
        /// <param name = "eventArgs">
        ///   The event args.
        /// </param>
        /// <param name = "operationCode">
        ///   The operation code.
        /// </param>
        /// <param name = "actorNumber">
        ///   The actor number.
        /// </param>
        protected static void CheckDefaultEventParameters(EventData eventArgs, OperationCode operationCode, int actorNumber)
        {
            CheckEventParam(eventArgs, ParameterKey.ActorNr, actorNumber);
        }

        /// <summary>
        ///   The check default event params.
        /// </summary>
        /// <param name = "eventArgs">
        ///   The event args.
        /// </param>
        /// <param name = "eventEventCode">
        ///   The event code.
        /// </param>
        /// <param name = "actorNumber">
        ///   The actor number.
        /// </param>
        protected static void CheckDefaultEventParameters(EventData eventArgs, EventCode eventEventCode, int actorNumber)
        {
            CheckEventParam(eventArgs, ParameterKey.ActorNr, actorNumber);
        }

        /// <summary>
        ///   The check default operation params.
        /// </summary>
        /// <param name = "response">
        ///   The response.
        /// </param>
        /// <param name = "operationCode">
        ///   The operation code.
        /// </param>
        protected static void CheckDefaultOperationParameters(OperationResponse response, OperationCode operationCode)
        {
            Assert.AreEqual(0, response.ReturnCode, string.Format("Response has Error. ERR={0}, DBG={1}", response.ReturnCode, response.DebugMessage));
        }

        /// <summary>
        ///   The check event param.
        /// </summary>
        /// <param name = "eventArgs">
        ///   The event args.
        /// </param>
        /// <param name = "paramKey">
        ///   The param key.
        /// </param>
        /// <param name = "expectedValue">
        ///   The expected value.
        /// </param>
        protected static void CheckEventParam(EventData eventArgs, ParameterKey paramKey, object expectedValue)
        {
            CheckEventParamExists(eventArgs, paramKey);
            object value = eventArgs.Parameters[(byte)paramKey];
            if (value is Hashtable)
            {
                // does not like synchronized hashtables for some reason
                value = new Hashtable((Hashtable)value);
            }

            Assert.AreEqual(expectedValue, value, "Event param '{0}' has unexpected value", paramKey);
        }

        /// <summary>
        ///   The check event param.
        /// </summary>
        /// <param name = "eventArgs">
        ///   The event args.
        /// </param>
        /// <param name = "key">
        ///   The key.
        /// </param>
        /// <param name = "expectedValue">
        ///   The expected value.
        /// </param>
        protected static void CheckEventParam(EventData eventArgs, object key, object expectedValue)
        {
            CheckEventParamExists(eventArgs, key);
            Assert.AreEqual(expectedValue, eventArgs.Parameters[(byte)key], "Event param '{0}' has unexpected value", key);
        }

        /// <summary>
        ///   The check event param exists.
        /// </summary>
        /// <param name = "eventArgs">
        ///   The event args.
        /// </param>
        /// <param name = "paramKey">
        ///   The param key.
        /// </param>
        protected static void CheckEventParamExists(EventData eventArgs, ParameterKey paramKey)
        {
            Assert.Contains((short)paramKey, eventArgs.Parameters.Keys, "Parameter '{0}' is missing in event.", paramKey);
        }

        /// <summary>
        ///   The check event param exists.
        /// </summary>
        /// <param name = "eventArgs">
        ///   The event args.
        /// </param>
        /// <param name = "key">
        ///   The key.
        /// </param>
        protected static void CheckEventParamExists(EventData eventArgs, object key)
        {
            Assert.Contains((short)key, eventArgs.Parameters.Keys, "Parameter '{0}' is missing in event.", key);
        }

        /// <summary>
        ///   The check join event.
        /// </summary>
        /// <param name = "eventArgs">
        ///   The event args.
        /// </param>
        /// <param name = "actorNumber">
        ///   The actor number.
        /// </param>
        protected static void CheckJoinEvent(EventData eventArgs, int actorNumber)
        {
            CheckJoinEvent(eventArgs, actorNumber, null);
        }

        /// <summary>
        ///   The check join event.
        /// </summary>
        /// <param name = "eventArgs">
        ///   The event args.
        /// </param>
        /// <param name = "actorNumber">
        ///   The actor number.
        /// </param>
        /// <param name = "expectedActorProperties">
        ///   The expected actor properties.
        /// </param>
        protected static void CheckJoinEvent(EventData eventArgs, int actorNumber, Hashtable expectedActorProperties)
        {
            CheckDefaultEventParameters(eventArgs, OperationCode.Join, actorNumber);
            CheckEventParamExists(eventArgs, ParameterKey.Actors);

            if (expectedActorProperties != null)
            {
                CheckEventParamExists(eventArgs, ParameterKey.ActorProperties);
                CheckEventParam(eventArgs, ParameterKey.ActorProperties, expectedActorProperties);
            }
        }

        /// <summary>
        ///   The check join response.
        /// </summary>
        /// <param name = "operationResponse">
        ///   The operation response.
        /// </param>
        /// <param name = "expectedActorNumber">
        ///   The expected actor number.
        /// </param>
        protected static void CheckJoinResponse(OperationResponse operationResponse, int expectedActorNumber)
        {
            CheckDefaultOperationParameters(operationResponse, OperationCode.Join);
            CheckParam(operationResponse, ParameterKey.ActorNr, expectedActorNumber);
        }

        /// <summary>
        ///   The check param.
        /// </summary>
        /// <param name = "response">
        ///   The response.
        /// </param>
        /// <param name = "paramKey">
        ///   The param key.
        /// </param>
        /// <param name = "expectedValue">
        ///   The expected value.
        /// </param>
        protected static void CheckParam(OperationResponse response, ParameterKey paramKey, object expectedValue)
        {
            CheckParamExists(response, paramKey);
            object value = response.Parameters[(byte)paramKey];
            Assert.AreEqual(expectedValue, value, "Parameter '{0} has an unexpected value", paramKey);
        }

        /// <summary>
        ///   The check param exists.
        /// </summary>
        /// <param name = "response">
        ///   The response.
        /// </param>
        /// <param name = "paramKey">
        ///   The param key.
        /// </param>
        protected static void CheckParamExists(OperationResponse response, ParameterKey paramKey)
        {
            Assert.Contains((short)paramKey, response.Parameters.Keys, "Parameter '{0}' is missing in operation response.", paramKey);
        }

        /// <summary>
        ///   Converts a hex string to an byte array.
        /// </summary>
        /// <param name = "hexString">
        ///   The hex string.
        /// </param>
        /// <param name = "delimiter">
        ///   The delimiter between the hex values.
        /// </param>
        /// <returns>
        ///   A byte array.
        /// </returns>
        protected static byte[] CreateByteArrayFromHexString(string hexString, char delimiter)
        {
            string[] split = hexString.Split(delimiter);
            var buffer = new byte[split.Length];
            for (int i = 0; i < split.Length; i++)
            {
                buffer[i] = byte.Parse(split[i], NumberStyles.AllowHexSpecifier);
            }

            return buffer;
        }

        /// <summary>
        ///   The create join request.
        /// </summary>
        /// <param name = "roomName">
        ///   The room name.
        /// </param>
        /// <param name = "properties">
        ///   The properties.
        /// </param>
        /// <param name = "broadcastProperties">
        ///   The broadcast properties.
        /// </param>
        /// <returns>
        ///   the join request
        /// </returns>
        protected static OperationRequest CreateJoinRequest(string roomName, Hashtable properties, bool broadcastProperties)
        {
            var request = new OperationRequest { OperationCode = (byte)OperationCode.Join, Parameters = new Dictionary<byte, object>() };
            request.Parameters.Add((byte)ParameterKey.GameId, roomName);
            if (properties != null)
            {
                request.Parameters.Add((byte)ParameterKey.ActorProperties, properties);
            }

            if (broadcastProperties)
            {
                request.Parameters.Add((byte)ParameterKey.Broadcast, true);
            }

            return request;
        }

        /// <summary>
        ///   The create random room name.
        /// </summary>
        /// <returns>
        ///   The random room name.
        /// </returns>
        protected static string CreateRandomRoomName()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        ///   The get actor number key.
        /// </summary>
        /// <param name = "actorNumber">
        ///   The actor number.
        /// </param>
        /// <returns>
        ///   The actor number key.
        /// </returns>
        protected virtual object GetActorNumberKey(int actorNumber)
        {
            return actorNumber;
        }

        /// <summary>
        ///   The init client.
        /// </summary>
        /// <returns>
        ///   the test client
        /// </returns>
        protected virtual TestClient InitClient()
        {
            var client = new TestClient(Settings.UseTcp);
            client.Connect(Settings.ServerAddress, Settings.Port, "Lite");

            if (client.WaitForConnect(this.WaitTime) == false)
            {
                Assert.Fail("Didn't received init response in expected time.");
            }

            return client;
        }

        /// <summary>
        ///   The join clients to room.
        /// </summary>
        /// <param name = "roomName">
        ///   The room name.
        /// </param>
        /// <param name = "client1">
        ///   The client 1.
        /// </param>
        /// <param name = "client2">
        ///   The client 2.
        /// </param>
        protected void JoinClientsToRoom(string roomName, TestClient client1, TestClient client2)
        {
            this.JoinClientsToRoom(roomName, client1, client2, null, null, false);
        }

        /// <summary>
        ///   The join clients to room.
        /// </summary>
        /// <param name = "roomName">
        ///   The room name.
        /// </param>
        /// <param name = "client1">
        ///   The client 1.
        /// </param>
        /// <param name = "client2">
        ///   The client 2.
        /// </param>
        /// <param name = "clientProperties1">
        ///   The client properties 1.
        /// </param>
        /// <param name = "clientProperties2">
        ///   The client properties 2.
        /// </param>
        /// <param name = "broadcastClientProperties">
        ///   The broadcast client properties.
        /// </param>
        protected void JoinClientsToRoom(
            string roomName, TestClient client1, TestClient client2, Hashtable clientProperties1, Hashtable clientProperties2, bool broadcastClientProperties)
        {
            // send join operation for client one
            OperationRequest request = CreateJoinRequest(roomName, clientProperties1, broadcastClientProperties);
            client1.SendOperationRequest(request);

            // wait for operation response and join event
            OperationResponse response = client1.WaitForOperationResponse(this.WaitTime);
            EventData eventArgs1 = client1.WaitForEvent(this.WaitTime);

            // check operation response 
            CheckJoinResponse(response, 1);

            // check join event
            Hashtable expectedActorProperties = broadcastClientProperties ? clientProperties1 : null;
            CheckJoinEvent(eventArgs1, 1, expectedActorProperties);

            if (client2 != null)
            {
                // send join operation for client two
                request = CreateJoinRequest(roomName, clientProperties2, broadcastClientProperties);
                client2.SendOperationRequest(request);

                // wait for operation response and join events
                response = client2.WaitForOperationResponse(this.WaitTime);
                eventArgs1 = client1.WaitForEvent(this.WaitTime);
                EventData eventArgs2 = client2.WaitForEvent(this.WaitTime);

                // check operation response 
                CheckJoinResponse(response, 2);

                // check join events
                expectedActorProperties = broadcastClientProperties ? clientProperties2 : null;
                CheckJoinEvent(eventArgs1, 2, expectedActorProperties);
                CheckJoinEvent(eventArgs2, 2, expectedActorProperties);
            }
        }

        /// <summary>
        ///   The leave clients from room.
        /// </summary>
        /// <param name = "client1">
        ///   The client 1.
        /// </param>
        /// <param name = "client2">
        ///   The client 2.
        /// </param>
        protected void LeaveClientsFromRoom(TestClient client1, TestClient client2)
        {
            var request = new OperationRequest { OperationCode = (byte)OperationCode.Leave, Parameters = new Dictionary<byte, object>() };
            client1.SendOperationRequest(request);
            OperationResponse response = client1.WaitForOperationResponse(this.WaitTime);
            CheckDefaultOperationParameters(response, OperationCode.Leave);

            if (client2 != null)
            {
                EventData eventArgs2 = client2.WaitForEvent(this.WaitTime);
                CheckDefaultEventParameters(eventArgs2, OperationCode.Leave, 1);

                client2.SendOperationRequest(request);
                response = client2.WaitForOperationResponse(this.WaitTime);
                CheckDefaultOperationParameters(response, OperationCode.Leave);
            }
        }

        /// <summary>
        ///   The log elapsed time.
        /// </summary>
        /// <param name = "logger">
        ///   The logger.
        /// </param>
        /// <param name = "prefix">
        ///   The prefix.
        /// </param>
        /// <param name = "elapsedTime">
        ///   The elapsed time.
        /// </param>
        /// <param name = "numItems">
        ///   The num items.
        /// </param>
        protected void LogElapsedTime(ILogger logger, string prefix, TimeSpan elapsedTime, long numItems)
        {
            if (logger.IsInfoEnabled)
            {
                logger.InfoFormat(
                    "{0}{1,10:N2} ms = {2,10:N5} ms/item = {3,10:N0} items/s",
                    prefix,
                    elapsedTime.TotalMilliseconds,
                    elapsedTime.TotalMilliseconds / numItems,
                    1000.0 / elapsedTime.TotalMilliseconds * numItems);
            }
        }

        #endregion
    }
}