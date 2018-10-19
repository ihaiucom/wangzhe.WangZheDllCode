// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationTests.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The operation tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Tests.Disconnected
{
    using System;
    using System.Collections;
    using System.Threading;

    using NUnit.Framework;

    using Photon.MmoDemo.Common;
    using Photon.MmoDemo.Server;
    using Photon.SocketServer;

    using Settings = Photon.MmoDemo.Tests.Settings;

    public class OperationTests
    {
        public OperationTests()
        {
            World world;
            WorldCache.Instance.Clear();
            WorldCache.Instance.TryCreate(
                "TestWorld",
                new BoundingBox(
                    new Vector(1f, 1f),
                    new Vector(10f, 10f)
                ),
                new Vector(1f, 1f, 0f),
                out world);
        }

        [Test]
        public void AttachCamera()
        {
            using (var client = new Client("Test"))
            {
                SpawnItem(client);

                client.ResetEvent();
                client.SendOperation(Operations.AttachCamera("MyItem"));
                client.BeginReceiveResponse();
                OperationResponse data;
                Assert.IsTrue(client.EndReceiveResponse(Settings.WaitTime, out data), "Event not received"); // blocking

                Assert.AreEqual(data.ReturnCode, (int)ReturnCode.Ok);
                Func<OperationResponse, bool> checkAction =
                    e => (string)e.Parameters[(byte)ParameterCode.ItemId] == "MyItem";
                Assert.IsTrue(checkAction(data), "check action failed");
            }
        }

        [Test]
        public void CreateWorld()
        {
            using (var client = new Client("Test"))
            {
                client.ResetEvent();
                client.SendOperation(Operations.CreateWorld("CreateWorld", new BoundingBox(new Vector(0f, 0f, 0f), new Vector(10f, 10f, 0f)), new Vector(1f, 1f, 0f)));
                client.BeginReceiveResponse();
                OperationResponse data;
                Assert.IsTrue(client.EndReceiveResponse(Settings.WaitTime, out data), "Response not received"); // blocking
                Assert.AreEqual(data.ReturnCode, (int)ReturnCode.Ok);

                // "Test" defined in setup
                client.ResetEvent();
                client.SendOperation(Operations.CreateWorld("TestWorld", new BoundingBox(new Vector(0f, 0f, 0f), new Vector(10f, 10f, 0f)), new Vector(1f, 1f, 0f)));
                client.BeginReceiveResponse();
                Assert.IsTrue(client.EndReceiveResponse(Settings.WaitTime, out data), "Response not received"); // blocking
                Assert.AreEqual(data.ReturnCode, (byte)ReturnCode.WorldAlreadyExists);
            }
        }

        [Test]
        public void DestroyItem()
        {
            using (var client = new Client("Test"))
            {
                SpawnItem(client);

                client.ResetEvent();
                client.SendOperation(Operations.DestroyItem("MyItem"));
                Func<EventData, bool> checkAction =
                    e => (string)e.Parameters[(byte)ParameterCode.ItemId] == "MyItem";
                client.BeginReceiveEvent(EventCode.ItemDestroyed, checkAction); 
                EventData data;
                Assert.IsTrue(client.EndReceiveEvent(Settings.WaitTime, out data), "Event not received"); // blocking

                Assert.AreEqual(data.Code, (byte)EventCode.ItemDestroyed);
                Assert.IsTrue(checkAction(data), "check action failed");
            }
        }

        [Test]
        public void DetachCamera()
        {
            using (var client = new Client("Test"))
            {
                EnterWorld(client, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);

                client.ResetEvent();
                client.SendOperation(Operations.DetachCamera());
                client.BeginReceiveResponse(); 

                OperationResponse data;
                Assert.IsTrue(client.EndReceiveResponse(Settings.WaitTime, out data), "Response not received"); // blocking
                Assert.AreEqual(data.ReturnCode, (int)ReturnCode.Ok);
            }
        }

        [Test]
        public void EnterWorld()
        {
            using (var client = new Client("Test"))
            {
                EnterWorld(client, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);
                
                client.ResetEvent();
                client.SendOperation(Operations.EnterWorld("TestWorld", client.Username, null, new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f)));
                ReceiveOperationResponse(client, ReturnCode.InvalidOperation); // blocking

                using (var client2 = new Client("Test"))
                {
                    EnterWorld(client2, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);

                    EventData @event;
                    client.ResetEvent();
                    client.BeginReceiveEvent(EventCode.WorldExited, d => true);
                    Assert.IsTrue(client.EndReceiveEvent(Settings.WaitTime, out @event), "Event not received"); // blocking
                    Assert.AreEqual((byte)EventCode.WorldExited, @event.Code);

                    Assert.IsFalse(client.Peer.Connected);
                }
            }
        }

        [Test]
        public void ExitWorld()
        {
            using (var client = new Client("Test"))
            {
                client.ResetEvent();
                client.SendOperation(Operations.ExitWorld());
                ReceiveOperationResponse(client, ReturnCode.InvalidOperation); // blocking

                EnterWorld(client, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);
                ExitWorld(client);
            }
        }

        [Test]
        public void Move()
        {
            using (var client = new Client("Test"))
            {
                EnterWorld(client, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);

                client.ResetEvent();
                client.SendOperation(Operations.Move(null, new Vector(1f, 2f, 0f)));
                NotReceiveOperationResponse(client); // blocking
            }
        }

        [Test]
        public void SetProperties()
        {
            using (var client = new Client("Test"))
            {
                EnterWorld(client, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);

                client.ResetEvent();
                client.SendOperation(Operations.SetProperties(null, new Hashtable { { "Key", "Value" } }, null));
                NotReceiveOperationResponse(client);// blocking
            }
        }

        [Test]
        public void SetViewDistance()
        {
            using (var client = new Client("Test"))
            {
                EnterWorld(client, "TestWorld", new Vector( 1f, 1f, 0f), new Vector( 1f, 1f, 0f), new Vector( 2f, 2f, 0f), null);

                client.ResetEvent();
                client.SendOperation(Operations.SetViewDistance(new Vector(2f, 2f, 0f), new Vector( 3f, 3f, 0f)));
                NotReceiveOperationResponse(client); // blocking
            }
        }

        [Test]
        public void SpawnItem()
        {
            using (var client = new Client("Test"))
            {
                SpawnItem(client);
            }
        }

        [Test]
        public void SubscribeUnsubscribeItem()
        {
            using (var client = new Client("Test"))
            {              
                EnterWorld(client, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);

                var myItemId = "MyItem";

                // spawn item out of interest area
                client.ResetEvent();
                client.SendOperation(Operations.SpawnItem(myItemId, ItemType.Bot, new Vector(3f, 3f, 0f), null, true));
                OperationResponse spawnData = ReceiveOperationResponse(client); // blocking
                Assert.AreEqual(spawnData.ReturnCode, (int)ReturnCode.Ok, "SpawnItem op error:" + spawnData.ReturnCode + " " + spawnData.DebugMessage);

                Console.WriteLine("Subscribing...");
                client.ResetEvent();
                client.SendOperation(Operations.SubscribeItem(myItemId, null));
                client.BeginReceiveEvent(EventCode.ItemSubscribed, d => (string)d.Parameters[(byte)ParameterCode.ItemId] == myItemId);
                EventData data;
                Assert.IsTrue(client.EndReceiveEvent(Settings.WaitTime, out data), "Event not received"); // blocking

                Console.WriteLine("Unsubscribing...");
                
                client.ResetEvent();
                client.SendOperation(Operations.Move(myItemId, new Vector(3f, 3.3f, 0f)));
                Func<EventData, bool> checkMoveAction = d => (string)d.Parameters[(byte)ParameterCode.ItemId] == myItemId;
                client.BeginReceiveEvent(EventCode.ItemMoved, checkMoveAction);
                Assert.IsTrue(client.EndReceiveEvent(Settings.WaitTime, out data), "Event not received");// blocking

                Assert.AreEqual(data.Code, (byte)EventCode.ItemMoved);
                Assert.IsTrue(checkMoveAction(data), "check action failed");

                // unsubscribe test
                client.ResetEvent();
                client.SendOperation(Operations.UnsubscribeItem(myItemId));
                client.BeginReceiveEvent(EventCode.ItemUnsubscribed, d => (string)d.Parameters[(byte)ParameterCode.ItemId] == myItemId);
                client.EndReceiveEvent(Settings.WaitTime, out data); // blocking
                Assert.AreEqual(data.Code, (byte)EventCode.ItemUnsubscribed);

                // check if unsubscription worked
                client.ResetEvent();
                client.SendOperation(Operations.Move(null, new Vector(1f, 2f, 0f)));
                client.BeginReceiveEvent(EventCode.ItemMoved, checkMoveAction);
                Assert.IsFalse(client.EndReceiveEvent(Settings.WaitTime, out data), "Event received"); // blocking
            }
        }

        [TearDown]
        public void TearDown()
        {
            // wait for client disconnect
            Thread.Sleep(100);
        }

        private static void EnterWorld(
            Client client, string worldName, Vector position, Vector viewDistanceEnter, Vector viewDistanceExit, Hashtable properties)
        
        {
            client.ResetEvent();
            client.SendOperation(Operations.EnterWorld(worldName, client.Username, properties, position, viewDistanceEnter, viewDistanceExit));
            ReceiveOperationResponse(client, ReturnCode.Ok); // blocking
        }

        private static void ExitWorld(Client client)
        {
            EventData data;
            client.ResetEvent();
            client.SendOperation(Operations.ExitWorld());
            client.BeginReceiveEvent(EventCode.WorldExited, d => true);// blocking
            Assert.IsTrue(client.EndReceiveEvent(Settings.WaitTime, out data), "Event not received"); // blocking
            Assert.AreEqual(data.Code, (byte)EventCode.WorldExited);
        }

        private static void NotReceiveOperationResponse(Client client)
        {
            client.BeginReceiveResponse();
            OperationResponse data;
            Assert.IsFalse(client.EndReceiveResponse(Settings.WaitTime, out data), "Response not received"); 
        }

        private static void ReceiveOperationResponse(Client client, ReturnCode expectedReturn)
        {
            client.BeginReceiveResponse();

            OperationResponse data;
            Assert.IsTrue(client.EndReceiveResponse(Settings.WaitTime, out data), "Response not received");
            var errorCode = (ReturnCode)data.ReturnCode;
            Assert.AreEqual(errorCode, expectedReturn);
        }

        private static OperationResponse ReceiveOperationResponse(Client client)
        {
            client.BeginReceiveResponse();

            OperationResponse data;
            Assert.IsTrue(client.EndReceiveResponse(Settings.WaitTime, out data), "Response not received");
            return data;
        }

        private static void SpawnItem(Client client)
        {
            EnterWorld(client, "TestWorld", new Vector(1f, 1f, 0f), new Vector(1f, 1f, 0f), new Vector(2f, 2f, 0f), null);

            client.ResetEvent();
            client.SendOperation(Operations.SpawnItem("MyItem", ItemType.Bot, new Vector(1f, 1f, 0f), null, true));
            OperationResponse data = ReceiveOperationResponse(client); // blocking

            Assert.AreEqual(data.ReturnCode, (int)ReturnCode.Ok, "SpawnItem op error:" + data.ReturnCode + " " + data.DebugMessage);
            
            // move item to view area            
            client.ResetEvent();
            client.SendOperation(Operations.Move("MyItem", new Vector(1f, 1f, 0f)));
            NotReceiveOperationResponse(client); // blocking

            // test not existing item move 
            client.ResetEvent();
            client.SendOperation(Operations.Move("NotExistsing", new Vector(1f, 1f, 0f)));
            ReceiveOperationResponse(client, ReturnCode.ItemNotFound); // blocking

        }
    }
}