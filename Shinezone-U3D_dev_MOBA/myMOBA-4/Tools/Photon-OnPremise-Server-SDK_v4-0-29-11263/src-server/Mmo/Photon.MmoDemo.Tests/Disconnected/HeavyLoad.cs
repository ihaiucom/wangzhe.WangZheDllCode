// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeavyLoad.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The heavy load.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Tests.Disconnected
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    using ExitGames.Concurrency.Fibers;
    using ExitGames.Logging;
    using ExitGames.Threading;

    using NUnit.Framework;

    using Photon.MmoDemo.Common;
    using Photon.MmoDemo.Server;

    using Settings = Tests.Settings;
    using Photon.SocketServer;
    using Photon.SocketServer.Diagnostics;

    [TestFixture]
    [Explicit]
    public class HeavyLoad
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static int clientCount;

        [Test]
        public void SetupClients()
        {
            World world;
            WorldCache.Instance.Clear();
            WorldCache.Instance.TryCreate("TestWorld", new BoundingBox(new Vector(0f, 0f), new Vector(400f, 400f)), new Vector(40f, 40f), out world);


            for (int i = 0; i < 10; i++)
            {
                List<Client> clients = SetupClients(world);
                DisconnectClients(clients);
            }
        }

        [Test]
        public void Run()
        {
            World world;
            WorldCache.Instance.Clear();
            WorldCache.Instance.TryCreate("TestWorld", new BoundingBox(new Vector(0f, 0f), new Vector(400f, 400f)), new Vector(40f, 40f), out world);

            List<Client> clients = SetupClients(world);

            Stopwatch t = Stopwatch.StartNew();
            using (var fiber = new PoolFiber(new FailSafeBatchExecutor()))
            {
                fiber.Start();
                fiber.ScheduleOnInterval(() => PrintStatsPerSecond(t), 1000, 1000);

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
                log.Info("move completed");
                Assert.AreEqual(0, Client.Exceptions, "Exceptions occured at exit");

                DisconnectClients(clients);
            }
        }

        [Test]
        public void RunForTime()
        {
            WorldCache.Instance.Clear();
            World world;
            WorldCache.Instance.TryCreate("TestWorld", new BoundingBox(new Vector(0f, 0f), new Vector(400f, 400f)), new Vector(40f, 40f), out world);


            List<Client> clients = SetupClients(world);

            Stopwatch t = Stopwatch.StartNew();
            using (var fiber = new PoolFiber(new FailSafeBatchExecutor()))
            {
                fiber.Start();
                fiber.ScheduleOnInterval(() => PrintStatsPerSecond(t), 1000, 1000);

                while (t.ElapsedMilliseconds < 10000)
                {
                    MoveClients(clients);
                    PrintStats(t);
                }
            }

            DisconnectClients(clients);
        }

        private static void ResetEvent(Client client)
        {
            client.ResetEvent();
        }

        private static void BeginReceiveEvent(Client client, EventCode eventCode, Func<EventData, bool> checkAction)
        {
            client.BeginReceiveEvent(eventCode, checkAction);
        }

        private static void BeginReceiveEvent(Client client, EventCode eventCode)
        {
            client.BeginReceiveEvent(eventCode, d => true);
        }

        private static void DisconnectClients(List<Client> clients)
        {
            Stopwatch t = Stopwatch.StartNew();
            
            clients.ForEach(ResetEvent);
            clients.ForEach(ExitWorldBegin);
            clients.ForEach(ExitWorldEnd); // blocking

            log.Info("exit completed");
            PrintStats(t);

            Assert.AreEqual(0, Client.Exceptions, "Exceptions occured at exit");

            t = Stopwatch.StartNew();
            clients.ForEach(c => c.Disconnect());
            Thread.Sleep(100);
            log.Info("disconnect completed");
            PrintStats(t);

            Assert.AreEqual(0, Client.Exceptions, "Exceptions occured at exit");
        }

        static int counterEndReceiveEvent;
        private static void EndReceiveEvent(Client client, EventCode eventCode)
        {
            EventData data;
            Assert.IsTrue(client.EndReceiveEvent(Settings.WaitTimeMultiOp, out data), "Event " + eventCode + " not received");
            Assert.AreEqual(eventCode, (EventCode)data.Code);
            Console.WriteLine("EndReceiveEvent " + client.Username + " " + eventCode + " " + Interlocked.Increment(ref counterEndReceiveEvent));
        }

        private static void EnterWorldBegin(Client client, World world, Vector position)
        {
            var viewDistanceEnter = new Vector( 1f, 1f);
            var viewDistanceExit = new Vector( 2f, 2f);

            client.Position = position;

            ThreadPoolEnqueue(
                client,
                () =>
                {
                    client.SendOperation(Operations.EnterWorld(world.Name, client.Username, null, position, viewDistanceEnter, viewDistanceExit));
                    client.BeginReceiveResponse();
                });
        }

        static int counterEnterWorldEnd;
        private static void EnterWorldEnd(Client client)
        {
            OperationResponse data;
            Assert.IsTrue(client.EndReceiveResponse(Settings.WaitTimeMultiOp, out data));
            Assert.AreEqual(0, data.ReturnCode);
            Console.WriteLine("EnterWorldEnd " + client.Username + " " + data + " " + Interlocked.Increment(ref counterEnterWorldEnd));
        }

        static int counterExitWorldBegin;
        private static void ExitWorldBegin(Client client)
        {
            ThreadPoolEnqueue(
                client,
                () =>
                {
                    client.SendOperation(Operations.ExitWorld());
                    Console.WriteLine("ExitWorldBegin " + client.Username + " " + Interlocked.Increment(ref counterExitWorldBegin));
                    BeginReceiveEvent(client, EventCode.WorldExited);
                });
        }

        private static void ExitWorldEnd(Client client)
        {
            EndReceiveEvent(client, EventCode.WorldExited);
        }

        private static void Move(Client client)
        {
            client.Peer.RequestFiber.Enqueue(() => MoveAction(client, 1));
        }

        private static void MoveAction(Client client, int number)
        {
            if (number < 5)
            {
                Vector pos = client.Position;
                client.SendOperation(Operations.Move(null, pos));
                client.Peer.RequestFiber.Schedule(() => MoveAction(client, number + 1), 100);
            }
        }

        private static void MoveClients(List<Client> clients)
        {
            clients.ForEach(ResetEvent);
            clients.ForEach(
                c =>
                {
                    Move(c);
                });

            clients.ForEach(ResetEvent);
            clients.ForEach(c => BeginReceiveEvent(c, EventCode.ItemMoved));
            clients.ForEach(c => EndReceiveEvent(c, EventCode.ItemMoved)); // blocking

            clients.ForEach(ResetEvent);
            clients.ForEach(c => BeginReceiveEvent(c, EventCode.ItemMoved));
            clients.ForEach(c => EndReceiveEvent(c, EventCode.ItemMoved)); // blocking

            clients.ForEach(ResetEvent);
            clients.ForEach(c => BeginReceiveEvent(c, EventCode.ItemMoved));
            clients.ForEach(c => EndReceiveEvent(c, EventCode.ItemMoved)); // blocking

            clients.ForEach(ResetEvent);
            clients.ForEach(c => BeginReceiveEvent(c, EventCode.ItemMoved));
            clients.ForEach(c => EndReceiveEvent(c, EventCode.ItemMoved)); // blocking

            clients.ForEach(ResetEvent);
            clients.ForEach(c => BeginReceiveEvent(c, EventCode.ItemMoved));
            clients.ForEach(c => EndReceiveEvent(c, EventCode.ItemMoved)); // blocking
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

            Client.ResetStats();
        }

        private static void PrintStatsPerSecond(Stopwatch t)
        {
            log.InfoFormat(
                "{2:00000} Itemessages: {0:0.00} sent, {1:0.00} received",
                MessageCounters.CounterSend.GetNextValue(),
                MessageCounters.CounterReceive.GetNextValue(),
                t.ElapsedMilliseconds);
        }

        private static List<Client> SetupClients(World world)
        {
            Stopwatch t = Stopwatch.StartNew();

            var clients = new List<Client>();
            for (int xx = 0; xx < world.TileX; xx++)
            {
                
                for (int yy = 0; yy < world.TileY; yy++)
                {
                    for (var i = 0; i < 2; i++)
                    {
                        // 2 clients in single tile offset by x?
                        int x = (int)(world.Area.Min.X + world.TileDimensions.X * xx + world.TileDimensions.X*(i + 1) / 3);
                        int y = (int)(world.Area.Min.Y + world.TileDimensions.Y * yy + world.TileDimensions.Y / 2 );
                        string name = string.Format("MyUsername{0}/{1}", x, y);
                        var client = new Client(name);

                        client.ResetEvent();                        
                        EnterWorldBegin(client, world, new Vector(x / 100f, y / 100f, 0f));
                        // ...

                        clients.Add(client);
                        clientCount++;
                        Console.WriteLine(string.Format("Client {0} added, total: {1}", name, clientCount));
                    }
                }

            }

            // ...            
            clients.ForEach(EnterWorldEnd); // blocking

            clients.ForEach(ResetEvent);
            clients.ForEach(c => BeginReceiveEvent(c, EventCode.ItemSubscribed, d => true));
            clients.ForEach(c => EndReceiveEvent(c, EventCode.ItemSubscribed));// blocking

            PrintStats(t);

            Assert.AreEqual(0, Client.Exceptions, "Exceptions occured at start");
            log.Info("enter completed");

            Assert.AreEqual(0, Client.Exceptions, "Exceptions occured at exit");

            return clients;
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
    }
}