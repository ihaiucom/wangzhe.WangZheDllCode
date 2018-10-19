// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpPerformanceTests.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The tcp performance tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    using ExitGames.Logging;

    using Lite.Operations;
    using Lite.Tests.Client;

    using NUnit.Framework;

    using Photon.SocketServer;

    /// <summary>
    ///   The tcp performance tests.
    /// </summary>
    [Explicit]
    [TestFixture]
    public class TcpPerformanceTests : TcpTestsBase
    {
        #region Constants and Fields

        /// <summary>
        ///   Gets the number of requests that are sent until we wait for the response to arrive before continueing.
        /// </summary>
        private const int WaitSteps = 30;

        /// <summary>
        ///   The log.
        /// </summary>
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///   The clients.
        /// </summary>
        private readonly TestClient[] clients;

        /// <summary>
        ///   The init response count.
        /// </summary>
        private int initResponseCount;

        /// <summary>
        ///   The operation response count.
        /// </summary>
        private int operationResponseCount;

        /// <summary>
        ///   Gets the current test nr.
        /// </summary>
        private int testNr;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TcpPerformanceTests" /> class.
        /// </summary>
        public TcpPerformanceTests()
        {
            TestClient.Connected += this.OnInitResponseReceived;
            TestClient.ResponseReceived += this.OnOperationResponseReceived;
            TestClient.EventReceived += this.OnEventReceived;

            this.WaitTime = 30000;

            this.clients = new TestClient[ClientCount];
        }

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
        ///   Sends Pings.
        /// </summary>
        [Test]
        public void Ping()
        {
            this.testNr = 1;

            // join all clients to a game (ping only works if peer has joined a game)
            for (int i = 0; i < this.clients.Length; i++)
            {
                var request = new OperationRequest { OperationCode = (byte)OperationCode.Join, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.GameId, "TestGame" + i);
                this.clients[i].SendOperationRequest(request);
                this.WaitForEvent();
            }

            Console.WriteLine("Join sent for {0} clients...", ClientCount);

            Thread.Sleep(1000);
            Interlocked.Exchange(ref this.operationResponseCount, 0);

            Stopwatch stopWatch = Stopwatch.StartNew();
            this.AutoResetEventOperation.Reset();

            for (int l = 0; l < LoopCount; l++)
            {
                foreach (TestClient t in this.clients)
                {
                    ThreadPool.QueueUserWorkItem(PingCallback, t);
                }

                Console.WriteLine("Ping sent for {0} clients {1} times...", ClientCount, l + 1);

                if ((l + 1) % WaitSteps == 0)
                {
                    Console.WriteLine("wait: " + (l + 1));
                    this.WaitForOperationResponse();
                }
            }

            if (LoopCount % WaitSteps != 0)
            {
                Console.WriteLine("wait final");
                this.WaitForOperationResponse();
            }

            stopWatch.Stop();

            this.LogElapsedTime(log, "Receive took: ", stopWatch.Elapsed, ClientCount * LoopCount);
        }

        /// <summary>
        ///   The ping.
        /// </summary>
        [Test]
        public void PingPing()
        {
            this.testNr = 2;
            Console.WriteLine("Sending ping for {0} clients {1} times ...", ClientCount, LoopCount);

            // join all clients to a game (ping only works if peer has joined a game)
            for (int i = 0; i < this.clients.Length; i++)
            {
                var request = new OperationRequest { OperationCode = (byte)OperationCode.Join, Parameters = new Dictionary<byte, object>() };
                request.Parameters.Add((byte)ParameterKey.GameId, "TestGame" + i);
                this.clients[i].SendOperationRequest(request);
                this.WaitForEvent();
            }

            Console.WriteLine("Join sent for {0} clients...", ClientCount);

            Thread.Sleep(1000);
            Interlocked.Exchange(ref this.operationResponseCount, 0);

            Stopwatch stopWatch = Stopwatch.StartNew();
            this.AutoResetEventOperation.Reset();
            var requests = new OperationRequest[LoopCount];
            var @params = new Dictionary<byte, object> { { 10, new byte[4096] } };
            var operation = new OperationRequest { OperationCode = (byte)OperationCode.Ping, Parameters = @params };

            for (int l = 0; l < LoopCount; l++)
            {
                requests[l] = operation;
            }

            foreach (TestClient t in this.clients)
            {
                // var client = this.clients[0];
                TestClient client = t;
                client.SendOperationRequests(requests);
            }

            ////    Console.WriteLine("Ping sent for {0} clients {1} times...", ClientCount, l + 1);

            ////    if ((l + 1) % this.waitSteps == 0)
            ////    {
            ////        Console.WriteLine("wait: " + (l + 1));
            ////        this.WaitForOperationResponse();
            ////    }
            ////}
            if (LoopCount % WaitSteps != 0)
            {
                Console.WriteLine("wait final");
                this.WaitForOperationResponse();
            }

            stopWatch.Stop();

            this.LogElapsedTime(log, "Receive took: ", stopWatch.Elapsed, ClientCount * LoopCount);
        }

        /// <summary>
        ///   The setup.
        /// </summary>
        [TestFixtureSetUp]
        public void Setup()
        {
            for (int i = 0; i < ClientCount; i++)
            {
                this.clients[i] = new TestClient(Settings.UseTcp);

                this.clients[i].Connect(Settings.ServerAddress, Settings.Port, "Lite");
                this.clients[i].WaitForConnect(100);
            }

            ////this.clients[0].PhotonClient.DebugOut = DebugLevel.ALL;
            this.WaitForInitResponse();
        }

        /// <summary>
        ///   The tear down.
        /// </summary>
        [TestFixtureTearDown]
        public void TearDown()
        {
            foreach (TestClient t in this.clients)
            {
                t.Close();
                t.Dispose();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   The on event received.
        /// </summary>
        /// <param name = "arg1">
        ///   The arg 1.
        /// </param>
        /// <param name = "arg2">
        ///   The arg 2.
        /// </param>
        protected void OnEventReceived(TestClient arg1, EventData arg2)
        {
            this.AutoResetEventEvent.Set();
        }

        /// <summary>
        ///   The on init response received.
        /// </summary>
        /// <param name = "obj">
        ///   The obj.
        /// </param>
        protected void OnInitResponseReceived(TestClient obj)
        {
            int count = Interlocked.Increment(ref this.initResponseCount);
            if (count == ClientCount)
            {
                this.AutoResetEventInit.Set();
            }
        }

        /// <summary>
        ///   The on operation response received.
        /// </summary>
        /// <param name = "arg1">
        ///   The arg 1.
        /// </param>
        /// <param name = "arg2">
        ///   The arg 2.
        /// </param>
        protected void OnOperationResponseReceived(TestClient arg1, OperationResponse arg2)
        {
            int count = Interlocked.Increment(ref this.operationResponseCount);

            if (this.testNr == 1)
            {
                if (count % ClientCount == 0 && (count / ClientCount) % WaitSteps == 0)
                {
                    Console.WriteLine("set: " + count);
                    this.AutoResetEventOperation.Set();
                }
                else if (count == ClientCount * LoopCount)
                {
                    Console.WriteLine("final set: " + count);
                    this.AutoResetEventOperation.Set();
                }

                return;
            }

            if (this.testNr == 2)
            {
                if (count == ClientCount * LoopCount)
                {
                    Console.WriteLine("final set: " + count);
                    this.AutoResetEventOperation.Set();
                }
            }
        }

        /// <summary>
        ///   The ping callback.
        /// </summary>
        /// <param name = "state">
        ///   The state.
        /// </param>
        private static void PingCallback(object state)
        {
            try
            {
                var client = (TestClient)state;
                var operationRequest = new OperationRequest { OperationCode = (byte)OperationCode.Ping };
                client.SendOperationRequest(operationRequest);
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }
            }
        }

        #endregion
    }
}