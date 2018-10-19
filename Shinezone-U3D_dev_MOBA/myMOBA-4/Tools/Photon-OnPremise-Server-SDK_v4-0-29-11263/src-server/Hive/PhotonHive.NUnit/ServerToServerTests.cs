// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerToServerTests.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The s2s tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;

    using ExitGames.Logging;

    using Photon.Hive.Operations;

    using NUnit.Framework;

    using Photon.SocketServer;
    using Photon.SocketServer.ServerToServer;

    using OperationRequest = Photon.SocketServer.OperationRequest;

    /// <summary>
    ///   The s2s tests.
    /// </summary>
    [TestFixture]
    public class ServerToServerTests
    {
        #region Constants and Fields

        /// <summary>
        ///   The logger.
        /// </summary>
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///   The reset event to signal that something was received.
        /// </summary>
        private readonly AutoResetEvent autoReset = new AutoResetEvent(false);

        /// <summary>
        ///   A counter used for signaling.
        /// </summary>
        private int counter;

        /// <summary>
        ///   A value that shows if the client disconnected.
        /// </summary>
        private int disconnect;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the client count.
        /// </summary>
        private static int ClientCount
        {
            get
            {
                return Settings.Clients;
            }
        }

        /// <summary>
        ///   Gets the loop count.
        /// </summary>
        private static int LoopCount
        {
            get
            {
                return Settings.Loops;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Sends many pings..
        /// </summary>
        [Test]
        public void Ping()
        {
            var requests = new OperationRequest[LoopCount];
            var @params = new Dictionary<byte, object> { { 10, new byte[4096] } };
            var operation = new OperationRequest { OperationCode = (byte)OperationCode.Ping, Parameters = @params };

            for (int l = 0; l < LoopCount; l++)
            {
                requests[l] = operation;
            }

            var clients = new TcpClient[ClientCount];
            for (int i = 0; i < ClientCount; i++)
            {
                var endPoint = new IPEndPoint(IPAddress.Parse(Settings.ServerAddress), 4530);
                clients[i] = new TcpClient();
                clients[i].ConnectCompleted += this.TcpClient_OnConnectCompleted;
                clients[i].ConnectError += TcpClient_OnConnectError;
                clients[i].Disconnected += this.TcpClient_OnDisconnected;
                clients[i].Event += TcpClient_OnEvent;
                clients[i].OperationResponse += this.TcpClient_OnOperationResponse;
                clients[i].Connect(endPoint, "Lite");
            }

            Assert.IsTrue(this.autoReset.WaitOne(1000), "Connect timeout");
            Assert.AreEqual(0, this.disconnect);
            Interlocked.Exchange(ref this.counter, 0);

            Stopwatch stopWatch = Stopwatch.StartNew();

            // join all clients to a game (ping only works if peer has joined a game)
            for (int i = 0; i < clients.Length; i++)
            {
                var request = new OperationRequest { OperationCode = (byte)OperationCode.Join, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.GameId, "TestGame" + i);
                Assert.AreEqual(SendResult.Ok, clients[i].SendOperationRequest(request, new SendParameters()));
            }

            Assert.IsTrue(this.autoReset.WaitOne(10000), "wait for join timeout");
            Assert.AreEqual(0, this.disconnect);
            Interlocked.Exchange(ref this.counter, 0);

            Console.WriteLine("sending...");
            foreach (OperationRequest operationRequest in requests)
            {
                foreach (TcpClient t in clients)
                {
                    t.SendOperationRequest(operationRequest, new SendParameters());
                }
            }

            Console.WriteLine("send completed.");

            Assert.IsTrue(this.autoReset.WaitOne(10000), "wait for response timeout");
            Assert.AreEqual(0, this.disconnect);
            LogElapsedTime(log, "Receive took: ", stopWatch.Elapsed, ClientCount * LoopCount);

            foreach (TcpClient t in clients)
            {
                t.Disconnect();
            }

            Assert.IsTrue(this.autoReset.WaitOne(10000), "wait for disconnect timeout");
            Assert.AreEqual(ClientCount, this.disconnect);
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Logs the elapsed time.
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
        private static void LogElapsedTime(ILogger logger, string prefix, TimeSpan elapsedTime, long numItems)
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

        /// <summary>
        ///   Logs an error.
        /// </summary>
        /// <param name = "sender">
        ///   The sender.
        /// </param>
        /// <param name = "e">
        ///   The event args.
        /// </param>
        private static void TcpClient_OnConnectError(object sender, SocketErrorEventArgs e)
        {
            log.Error(e.SocketError);
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// <param name = "sender">
        ///   The sender.
        /// </param>
        /// <param name = "e">
        ///   The event args.
        /// </param>
        private static void TcpClient_OnEvent(object sender, EventDataEventArgs e)
        {
            ////log.Info("Event" + (LiteCode)e.EventData.Code);
            ////if (e.EventData.Code == (int)LiteCode.Join)
            ////{
            ////    if (Interlocked.Increment(ref this.counter) == ClientCount)
            ////    {
            ////        this.autoReset.Set();
            ////    }
            ////}
        }

        /// <summary>
        ///   Callback for connects.
        /// </summary>
        /// <param name = "sender">
        ///   The sender.
        /// </param>
        /// <param name = "e">
        ///   The event args.
        /// </param>
        private void TcpClient_OnConnectCompleted(object sender, EventArgs e)
        {
            if (Interlocked.Increment(ref this.counter) == ClientCount)
            {
                this.autoReset.Set();
            }
        }

        /// <summary>
        ///   Callback for disconnects.
        /// </summary>
        /// <param name = "sender">
        ///   The sender.
        /// </param>
        /// <param name = "e">
        ///   The event args.
        /// </param>
        private void TcpClient_OnDisconnected(object sender, SocketErrorEventArgs e)
        {
            if (Interlocked.Increment(ref this.disconnect) == ClientCount)
            {
                this.autoReset.Set();
            }
        }

        /// <summary>
        ///   Callback for operation response.
        /// </summary>
        /// <param name = "sender">
        ///   The sender.
        /// </param>
        /// <param name = "e">
        ///   The event args.
        /// </param>
        private void TcpClient_OnOperationResponse(object sender, OperationResponseEventArgs e)
        {
            ////log.Info("Response" + e.OperationResponse.OperationCode);
            if (e.OperationResponse.OperationCode == (byte)OperationCode.Join)
            {
                if (Interlocked.Increment(ref this.counter) == ClientCount)
                {
                    this.autoReset.Set();
                }
            }
            else if (e.OperationResponse.OperationCode == (byte)OperationCode.Ping)
            {
                if (Interlocked.Increment(ref this.counter) == ClientCount * LoopCount)
                {
                    this.autoReset.Set();
                }
            }
        }

        #endregion
    }
}