// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeavyLoad.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The heavy load.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Tests.Connected
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    using ExitGames.Client.Photon;
    using ExitGames.Logging;

    using NUnit.Framework;

    using Photon.MmoDemo.Common;
    using Photon.MmoDemo.Server;

    using Settings = Photon.MmoDemo.Tests.Settings;

    [TestFixture]
    [Explicit]
    public class HeavyLoad
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static int clientCount;

        #endregion

        #region Public Methods

        [Test]
        public void Run()
        {
            WorldCache.Instance.Clear();
            World world;
            WorldCache.Instance.TryCreate(
                "HeavyLoad2", new BoundingBox( new Vector(0f, 0f), new Vector(100f, 100f)), new Vector(20f, 20f, 0f), out world);

            using (var client = new Client(string.Empty))
            {
                Assert.IsTrue(client.Connect());
                CreateWorld(world, client);
            }

            Stopwatch t = Stopwatch.StartNew();

            var clients = new List<Client>();
            try
            {
                SetupClients(world, clients, t);

                Client.ResetStats();
                log.Info("ItemPositionUpdate wait completed");
                Assert.AreEqual(0, Client.Exceptions, "Exceptions occured at exit");

                t = Stopwatch.StartNew();
                MoveClients(clients);
                PrintStats(t);
                MoveClients(clients);
                PrintStats(t);
                MoveClients(clients);
                PrintStats(t);
                MoveClients(clients);
                PrintStats(t);
                MoveClients(clients);
                PrintStats(t);
                Client.ResetStats();
                log.Info("move completed");
                Assert.AreEqual(0, Client.Exceptions, "Exceptions occured at exit");

                DisconnectClients(clients);
            }
            finally
            {
                clients.ForEach(c => c.Dispose());
            }
        }

        [Test]
        public void RunForTime()
        {
            WorldCache.Instance.Clear();
            World world;
            WorldCache.Instance.TryCreate(
                "HeavyLoad3", new BoundingBox(new Vector(0f, 0f), new Vector(100f, 100f)), new Vector(20f, 20f, 0f), out world);

            using (var client = new Client(string.Empty))
            {
                Assert.IsTrue(client.Connect());
                CreateWorld(world, client);
            }

            Stopwatch t = Stopwatch.StartNew();

            var clients = new List<Client>();
            try
            {
                SetupClients(world, clients, t);

                Client.ResetStats();
                t = Stopwatch.StartNew();
                while (t.ElapsedMilliseconds < 10000)
                {
                    MoveClients(clients);
                    PrintStats(t);
                    Client.ResetStats();
                }

                log.Info("move completed");
                Assert.AreEqual(0, Client.Exceptions, "Exceptions occured at exit");

                DisconnectClients(clients);
            }
            finally
            {
                clients.ForEach(c => c.Dispose());
            }
        }

        #endregion

        #region Methods

        private static void BeginReceiveEvent(Client client, EventCode eventCode, Func<EventData, bool> checkAction, int delay)
        {
            client.BeginReceiveEvent(eventCode, checkAction, delay);
        }

        private static void BeginReceiveEvent(Client client, EventCode eventCode, int delay)
        {
            client.BeginReceiveEvent(eventCode, d => true, delay);
        }

        private static void CreateWorld(World world, Client client)
        {
            Operations.CreateWorld(client, world.Name, world.Area, world.TileDimensions);

            client.BeginReceiveResponse(0);

            OperationResponse data;
            Assert.IsTrue(client.EndReceiveResponse(Settings.WaitTimeMultiOp, out data), "Response not received");
            Assert.IsTrue(data.ReturnCode == (int)ReturnCode.Ok || data.ReturnCode == (int)ReturnCode.WorldAlreadyExists);
        }

        private static void DisconnectClients(List<Client> clients)
        {
            Stopwatch t = Stopwatch.StartNew();
            clients.ForEach(ExitWorldBegin);
            clients.ForEach(ExitWorldEnd);
            log.Info("exit completed");
            PrintStats(t);
            Client.ResetStats();
            Assert.AreEqual(0, Client.Exceptions, "Exceptions occured at exit");

            t = Stopwatch.StartNew();
            clients.ForEach(c => c.BeginDisconnect());
            clients.ForEach(c => c.EndDisconnect());
            log.Info("disconnect completed");
            PrintStats(t);
            Client.ResetStats();
            Assert.AreEqual(0, Client.Exceptions, "Exceptions occured at exit");
        }

        private static EventData EndReceiveEvent(Client client, EventCode eventCode)
        {
            EventData data;
            Assert.IsTrue(client.EndReceiveEvent(Settings.WaitTimeMultiOp, out data), "Event not received");
            Assert.AreEqual(eventCode, (EventCode)data.Code);
            return data;
        }

        private static void EnterWorldBegin(Client client, World world)
        {
            var viewDistanceEnter = new Vector(1f, 1f, 0f);
            var viewDistanceExit = new Vector( 2f, 2f, 0f);

            ThreadPoolEnqueue(
                client, 
                () =>
                    {
                        Operations.EnterWorld(client, world.Name, client.Username, null, client.Position, viewDistanceEnter, viewDistanceExit);
                        client.BeginReceiveResponse(10);
                    });
        }

        private static void EnterWorldEnd(Client client)
        {
            OperationResponse data;
            Assert.IsTrue(client.EndReceiveResponse(Settings.WaitTimeMultiOp, out data));
        }

        private static void ExitWorldBegin(Client client)
        {
            ThreadPoolEnqueue(client, () => Operations.ExitWorld(client));
        }

        private static void ExitWorldEnd(Client client)
        {
            BeginReceiveEvent(client, EventCode.WorldExited, 0);
            EndReceiveEvent(client, EventCode.WorldExited);
        }

        private static void Move(Client client)
        {
            client.OperationFiber.Enqueue(() => MoveAction(client, 1));
        }

        private static void MoveAction(Client client, int number)
        {
            Vector pos = client.Position;
            Operations.Move(client, null, pos);
            number++;
            if (number < 6)
            {
                client.OperationFiber.Schedule(() => MoveAction(client, number), 100);
            }
        }

        private static void MoveClients(List<Client> clients)
        {
            clients.ForEach(
                c =>
                    {
                        Move(c);
                        BeginReceiveEvent(c, EventCode.ItemMoved, 10);
                    });

            clients.ForEach(c => EndReceiveEvent(c, EventCode.ItemMoved));
            Thread.Sleep(100);
            clients.ForEach(c => BeginReceiveEvent(c, EventCode.ItemMoved, 0));
            clients.ForEach(c => EndReceiveEvent(c, EventCode.ItemMoved));
            Thread.Sleep(100);
            clients.ForEach(c => BeginReceiveEvent(c, EventCode.ItemMoved, 0));
            clients.ForEach(c => EndReceiveEvent(c, EventCode.ItemMoved));
            Thread.Sleep(100);
            clients.ForEach(c => BeginReceiveEvent(c, EventCode.ItemMoved, 0));
            clients.ForEach(c => EndReceiveEvent(c, EventCode.ItemMoved));
            Thread.Sleep(100);
            clients.ForEach(c => BeginReceiveEvent(c, EventCode.ItemMoved, 0));
            clients.ForEach(c => EndReceiveEvent(c, EventCode.ItemMoved));
        }

        private static void PrintStats(Stopwatch t)
        {
            log.InfoFormat(
                "{7:00000} Client({8}): {9} operations, {0} fast events in {1:0.00}ms avg, {2} middle events in {3:0.00}ms avg, {4} slow events in {5:0.00}ms avg, {6:0.00}ms max - {10} exceptions", 
                Client.EventsReceivedFast, 
                Client.EventsReceivedTimeTotalFast / (double)Client.EventsReceivedFast, 
                Client.EventsReceivedMiddle, 
                Client.EventsReceivedTimeTotalMiddle / (double)Client.EventsReceivedMiddle, 
                Client.EventsReceivedSlow, 
                Client.EventsReceivedTimeTotalSlow / (double)Client.EventsReceivedSlow, 
                Client.EventsReceivedTimeMax, 
                t.ElapsedMilliseconds, 
                clientCount, 
                Client.OperationsSent, 
                Client.Exceptions);
        }

        private static void SetupClients(World world, List<Client> clients, Stopwatch t)
        {
            for (int x = (int)(world.Area.Min.X + (world.TileDimensions.X / 2)); x < world.Area.Max.X; x += (int)world.TileDimensions.X)
            {
                for (int y = (int)(world.Area.Min.Y + (world.TileDimensions.Y / 2)); y < world.Area.Max.Y; y += (int)world.TileDimensions.Y)
                {
                    string name = string.Format("MyUsername{0}/{1}", x, y);
                    var client = new Client(name, new Vector( x / 100f, y / 100f, 0f));
                    client.BeginConnect();
                    clients.Add(client);
                    clientCount++;
                }

                for (int y = (int)(world.Area.Min.Y + (world.TileDimensions.Y / 2)) + 1; y < world.Area.Max.Y; y += (int)world.TileDimensions.Y)
                {
                    string name = string.Format("MyUsername{0}/{1}", x + 1, y);
                    var client = new Client(name, new Vector((x + 1) / 100f, y / 100f, 0f));
                    client.BeginConnect();
                    clients.Add(client);
                    clientCount++;
                }
            }

            clients.ForEach(c => Assert.IsTrue(c.EndConnect()));
            log.InfoFormat("connect completed, {0} clients", clientCount);

            clients.ForEach(c => EnterWorldBegin(c, world));
            clients.ForEach(EnterWorldEnd);
            clients.ForEach(c => BeginReceiveEvent(c, EventCode.ItemSubscribed, d => (string)d[(byte)ParameterCode.ItemId] != c.Username, 500));
            clients.ForEach(c => EndReceiveEvent(c, EventCode.ItemSubscribed));
            PrintStats(t);
            Client.ResetStats();
            Assert.AreEqual(0, Client.Exceptions, "Exceptions occured at start");
            log.Info("enter completed");
        }

        private static void ThreadPoolEnqueue(Client client, Action action)
        {
            ThreadPool.QueueUserWorkItem(
                o =>
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception e)
                        {
                            client.HandleException(e);
                        }
                    });
        }

        #endregion
    }
}