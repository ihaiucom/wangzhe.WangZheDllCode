// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationTests.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The operation tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Tests.Connected
{
    using System;
    using System.Collections;
    using System.Threading;

    using NUnit.Framework;

    using Photon.MmoDemo.Common;
    using Photon.SocketServer;

    using EventData = ExitGames.Client.Photon.EventData;
    using OperationResponse = ExitGames.Client.Photon.OperationResponse;

    [TestFixture]
    public class OperationTests
    {
        #region Public Methods

        [Test]
        public void AttachCamera()
        {
            using (var client = new Client("Test"))
            {
                Assert.IsTrue(client.Connect());
                CreateWorld("TestWorld", client);
                EnterWorld(client, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);

                SpawnItem(client);

                Func<OperationResponse, bool> checkAction =
                    e => (string)e[(byte)ParameterCode.ItemId] == "MyItem";
                OperationResponse data;
                Operations.AttachCamera(client, "MyItem");
                client.BeginReceiveResponse(0);

                Assert.IsTrue(client.EndReceiveResponse(Settings.WaitTime, out data), "Response not received");
                Assert.AreEqual(data.ReturnCode, (int)ReturnCode.Ok);
                Assert.IsTrue(checkAction(data), "check action failed");
            }
        }

        [Test]
        public void Connect()
        {
            using (var client = new Client("Test"))
            {
                Assert.IsTrue(client.Connect());
                Thread.Sleep(1000);
                Assert.IsTrue(client.Disconnect());
            }
        }

        [Test]
        public void CreateWorld()
        {
            using (var client = new Client("Test"))
            {
                Assert.IsTrue(client.Connect());
                CreateWorld("CreateWorld", client);

                // "Test" defined in setup
                Operations.CreateWorld(client, "CreateWorld", new BoundingBox(new Vector(0f, 0f, 0f), new Vector(10f, 10f, 0f)), new Vector(1f, 1f, 0f));

                Func<OperationResponse, bool> checkAction = d => d.OperationCode == (byte)OperationCode.CreateWorld;
                client.BeginReceiveResponse(10);

                OperationResponse data;
                Assert.IsTrue(client.EndReceiveResponse(Settings.WaitTime, out data), "Response not received");
                Assert.AreEqual(data.ReturnCode, (int)ReturnCode.WorldAlreadyExists);
                Assert.IsTrue(checkAction(data), "check action failed");
            }
        }

        [Test]
        public void DestroyItem()
        {
            using (var client = new Client("Test"))
            {
                Assert.IsTrue(client.Connect());
                CreateWorld("TestWorld", client);
                EnterWorld(client, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);
                SpawnItem(client);
                Operations.DestroyItem(client, "MyItem");

                Func<EventData, bool> checkAction =
                    d => (string)d.Parameters[(byte)ParameterCode.ItemId] == "MyItem";
                client.BeginReceiveEvent(EventCode.ItemDestroyed, checkAction, 10);
                EventData data;
                Assert.IsTrue(client.EndReceiveEvent(Settings.WaitTime, out data), "Event not received");
                Assert.AreEqual(data.Code, (byte)EventCode.ItemDestroyed);
                Assert.IsTrue(checkAction(data), "check action failed");
            }
        }

        [Test]
        public void DetachCamera()
        {
            using (var client = new Client("Test"))
            {
                Assert.IsTrue(client.Connect());
                CreateWorld("TestWorld", client);
                EnterWorld(client, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);

                Operations.DetachCamera(client);
                client.BeginReceiveResponse(0);

                OperationResponse data;
                Assert.IsTrue(client.EndReceiveResponse(Settings.WaitTime, out data), "Response not received");
                Assert.AreEqual(data.ReturnCode, (byte)ReturnCode.Ok);
            }
        }

        [Test]
        public void EnterWorld()
        {
            using (var client = new Client("Test"))
            {
                Assert.IsTrue(client.Connect());
                CreateWorld("TestWorld", client);
                EnterWorld(client, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);
            }
        }

        [Test]
        public void ExitWorld()
        {
            using (var client = new Client("Test"))
            {
                Assert.IsTrue(client.Connect());
                CreateWorld("TestWorld", client);
                Operations.ExitWorld(client);

                client.BeginReceiveResponse(0);

                OperationResponse data;
                Assert.IsTrue(client.EndReceiveResponse(Settings.WaitTime, out data), "Response not received");
                Assert.AreEqual(data.ReturnCode, (byte)ReturnCode.InvalidOperation);
                EnterWorld(client, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);
                ExitWorld(client);
            }
        }

        [Test]
        public void Move()
        {
            using (var client = new Client("Test"))
            {
                Assert.IsTrue(client.Connect());
                CreateWorld("TestWorld", client);
                EnterWorld(client, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);
                Operations.Move(client, null, new Vector(1f, 2f, 0f));

                client.BeginReceiveResponse(0);
                OperationResponse data;
                Assert.IsFalse(client.EndReceiveResponse(Settings.WaitTime, out data), "Response received");
            }
        }

        [Test]
        public void SetProperties()
        {
            using (var client = new Client("Test"))
            {
                Assert.IsTrue(client.Connect());
                CreateWorld("TestWorld", client);
                EnterWorld(client, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);

                Operations.SetProperties(client, null, new Hashtable { { "Key", "Value" } }, null);

                client.BeginReceiveResponse(0);
                EventData data;
                Assert.IsFalse(client.EndReceiveEvent(Settings.WaitTime, out data), "Response received");
            }
        }

        [Test]
        public void SetViewDistance()
        {
            using (var client = new Client("Test"))
            {
                Assert.IsTrue(client.Connect());
                CreateWorld("TestWorld", client);
                EnterWorld(client, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);

                Operations.SetViewDistance(client, new Vector(2f, 2f, 0f), new Vector(3f, 3f, 0f));

                client.BeginReceiveResponse(0);
                OperationResponse data;
                Assert.IsFalse(client.EndReceiveResponse(Settings.WaitTime, out data), "Response received");
            }
        }

        [Test]
        public void SpawnItem()
        {
            using (var client = new Client("Test"))
            {
                Assert.IsTrue(client.Connect());
                CreateWorld("TestWorld", client);
                EnterWorld(client, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);
                SpawnItem(client);
                // check error on existing
                SpawnItem(client, ReturnCode.ItemAlreadyExists);
            }
        }

        [Test]
        public void SubscribeUnsubscribeItem()
        {
            using (var client = new Client("Test"))
            {
                var myItemId = "MyItem";

                Assert.IsTrue(client.Connect());
                CreateWorld("TestWorld", client);
                EnterWorld(client, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);

                SpawnItem(client);

                // subscribing...
                Operations.SubscribeItem(client, myItemId, null);
                client.BeginReceiveEvent(EventCode.ItemSubscribed, d => (string)d[(byte)ParameterCode.ItemId] == myItemId, 0);
                EventData data;
                client.EndReceiveEvent(Settings.WaitTime, out data);
                Assert.AreEqual(data.Code, (byte)EventCode.ItemSubscribed);

                // check if subscription works
                Operations.Move(client, myItemId, new Vector(3f, 3.3f, 0f));

                Func<EventData, bool> checkAction =
                    d => (string)d[(byte)ParameterCode.ItemId] == myItemId;
                client.BeginReceiveEvent(EventCode.ItemMoved, checkAction, 0);
                Assert.IsTrue(client.EndReceiveEvent(Settings.WaitTime, out data), "Event not received");
                Assert.AreEqual(data.Code, (byte)EventCode.ItemMoved);
                Assert.IsTrue(checkAction(data), "check action failed");

                // unsubscribing...
                Operations.UnsubscribeItem(client, myItemId);
                client.BeginReceiveEvent(EventCode.ItemUnsubscribed, d => (string)d[(byte)ParameterCode.ItemId] == myItemId, 0);
                client.EndReceiveEvent(Settings.WaitTime, out data);
                Assert.AreEqual(data.Code, (byte)EventCode.ItemUnsubscribed);

                // check if unsubscription works
                Operations.Move(client, null, new Vector(1f, 2f, 0f));

                client.BeginReceiveEvent(EventCode.ItemMoved, checkAction, 0);
                Assert.IsFalse(client.EndReceiveEvent(Settings.WaitTime, out data), "Event received");
            }
        }
        
        #endregion

        #region Methods

        private static void CreateWorld(string world, Client client)
        {
            Operations.CreateWorld(client, world, new BoundingBox(new Vector(0f, 0f, 0f), new Vector(10f, 10f, 0f)), new Vector(1f, 1f, 0f));

            client.BeginReceiveResponse(0);

            OperationResponse data;
            Assert.IsTrue(client.EndReceiveResponse(Settings.WaitTime, out data), "Response not received");
            Assert.IsTrue(data.ReturnCode == (int)ReturnCode.Ok || data.ReturnCode == (int)ReturnCode.WorldAlreadyExists);
        }

        private static void EnterWorld(
            Client client, string worldName, Vector position, Vector viewDistanceEnter, Vector viewDistanceExit, Hashtable properties)
        {
            Operations.EnterWorld(client, worldName, client.Username, properties, position, viewDistanceEnter, viewDistanceExit);

            client.BeginReceiveResponse(0);

            OperationResponse data;
            Assert.IsTrue(client.EndReceiveResponse(Settings.WaitTime, out data), "Event not received");
            Assert.AreEqual(data.ReturnCode, (int)ReturnCode.Ok);
        }

        private static void ExitWorld(Client client)
        {
            EventData data;
            Operations.ExitWorld(client);
            client.BeginReceiveEvent(EventCode.WorldExited, d => true, 0);
            Assert.IsTrue(client.EndReceiveEvent(Settings.WaitTime, out data), "Event not received");
            Assert.AreEqual(data.Code, (byte)EventCode.WorldExited);
        }

        private static void SpawnItem(Client client, ReturnCode result = ReturnCode.Ok)
        {
            Operations.SpawnItem(client, "MyItem", byte.MaxValue, new Vector(1f, 1f, 0f), null, true);

            Func<OperationResponse, bool> checkAction =
                e => (string)e[(byte)ParameterCode.ItemId] == "MyItem";
            client.BeginReceiveResponse(0);

            OperationResponse data;
            // may exist if other client is online?
            Assert.IsTrue(client.EndReceiveResponse(Settings.WaitTime, out data), "Response not received");
            Assert.AreEqual(result, (ReturnCode)data.ReturnCode);
            if (data.ReturnCode != (int)ReturnCode.Ok)
            {
                Assert.AreEqual(ReturnCode.ItemAlreadyExists, (ReturnCode)data.ReturnCode);
                Operations.Move(client, "MyItem", new Vector(1f, 1f, 0f));
            }
            else
            {
                Assert.IsTrue(checkAction(data), "check action failed");
            }
        }
        #endregion
    }
}