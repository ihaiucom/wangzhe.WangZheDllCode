// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadBalancerTests.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LoadBalancerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Common.LoadBalancer;
using Photon.Common.LoadBalancer.LoadShedding;

namespace Photon.LoadBalancing.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    using ExitGames.Logging;
    using ExitGames.Logging.Log4Net;

    using log4net.Config;
    using NUnit.Framework;

    [TestFixture]
    public class LoadBalancerTests
    {
        private LoadBalancer<Server> balancer;
        private List<Server> servers;

        [TestFixtureSetUp]
        public void Setup()
        {
            LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config")); 

            this.balancer = new LoadBalancer<Server>();
            
            this.servers = new List<Server>();
            for (int i = 0; i < 5; i++)
            {
                this.servers.Add(new Server { Name = "Server" + i });
            }
        }

        [Test]
        public void Basics()
        {
            Server server;

            this.balancer = new LoadBalancer<Server>();
            this.TryGetServer(out server, false);
            this.TryUpdateServer(this.servers[0], FeedbackLevel.Low, false);

            this.TryAddServer(this.servers[0], FeedbackLevel.Highest);
            this.TryGetServer(out server, false);

            this.TryAddServer(this.servers[1], FeedbackLevel.Highest);
            this.TryGetServer(out server, false);

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Normal);
            this.TryGetServer(out server);
            Assert.AreSame(this.servers[0], server);

            this.TryRemoveServer(this.servers[0]);
            this.TryRemoveServer(this.servers[0], false);
        }

        [Test]
        public void Properties()
        {
            // FeedbackLevel.Lowest  value = 0, weight = 40
            // FeedbackLevel.Low     value = 1, weight = 30
            // FeedbackLevel.Normal  value = 2, weight = 20
            // FeedbackLevel.High    value = 3, weight = 10
            // FeedbackLevel.Highest value = 4, weight = 0

            this.balancer = new LoadBalancer<Server>();
            this.CheckLoadBalancerProperties(0, 0, 0);

            this.TryAddServer(this.servers[0], FeedbackLevel.Lowest);
            this.CheckLoadBalancerProperties(0, 40, 0);

            this.TryAddServer(this.servers[1], FeedbackLevel.Lowest);
            this.CheckLoadBalancerProperties(0, 80, 0);

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Normal);
            this.CheckLoadBalancerProperties(2, 60, 25);

            this.TryUpdateServer(this.servers[1], FeedbackLevel.Normal);
            this.CheckLoadBalancerProperties(4, 40, 50);

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Highest);
            this.CheckLoadBalancerProperties(6, 20, 75);

            this.TryUpdateServer(this.servers[1], FeedbackLevel.Highest);
            this.CheckLoadBalancerProperties(8, 0, 100);

            this.TryRemoveServer(this.servers[1]);
            this.CheckLoadBalancerProperties(4, 0, 100);

            this.TryRemoveServer(this.servers[0]);
            this.CheckLoadBalancerProperties(0, 0, 0);
        }

        [Test]
        public void LoadSpreadByDefault()
        {
            const int count = 100000;

            // default, as per DefaultConfiguration.GetDefaultWeights: 
            /*
            var loadLevelWeights = new int[]
            {
                40, // FeedbackLevel.Lowest
                30, // FeedbackLevel.Low
                20, // FeedbackLevel.Normal
                10, // FeedbackLevel.High
                0 // FeedbackLevel.Highest
            };
             */

            this.balancer = new LoadBalancer<Server>();

            for (int i = 0; i < this.servers.Count; i++)
            {
                bool result = this.balancer.TryAddServer(this.servers[i], FeedbackLevel.Lowest);
                Assert.IsTrue(result);
            }

            // 5 servers with a load level of lowest
            // every server should get about 20 percent of the assignments
            this.AssignServerLoop(count);
            for (int i = 0; i < this.servers.Count; i++)
            {
                this.CheckServer(this.servers[i], count, 20, 5);
            }

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Lowest);
            this.TryUpdateServer(this.servers[1], FeedbackLevel.Low);
            this.TryUpdateServer(this.servers[2], FeedbackLevel.Normal);
            this.TryUpdateServer(this.servers[3], FeedbackLevel.High);
            this.TryUpdateServer(this.servers[4], FeedbackLevel.Highest);
            
            this.AssignServerLoop(count);
            this.CheckServer(this.servers[0], count, 40, 5);
            this.CheckServer(this.servers[1], count, 30, 5);
            this.CheckServer(this.servers[2], count, 20, 5);
            this.CheckServer(this.servers[3], count, 10, 5);
            this.CheckServer(this.servers[4], count, 0, 0);

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Lowest);
            this.TryUpdateServer(this.servers[1], FeedbackLevel.Low);
            this.TryUpdateServer(this.servers[2], FeedbackLevel.Low);
            this.TryUpdateServer(this.servers[3], FeedbackLevel.Normal);
            this.TryUpdateServer(this.servers[4], FeedbackLevel.Normal);
            this.AssignServerLoop(count);
            this.CheckServer(this.servers[0], count, 28, 5);
            this.CheckServer(this.servers[1], count, 21, 5);
            this.CheckServer(this.servers[2], count, 21, 5);
            this.CheckServer(this.servers[3], count, 14, 5);
            this.CheckServer(this.servers[4], count, 14, 5);

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Low);
            this.TryUpdateServer(this.servers[1], FeedbackLevel.Normal);
            this.TryUpdateServer(this.servers[2], FeedbackLevel.Normal);
            this.TryUpdateServer(this.servers[3], FeedbackLevel.Normal);
            this.TryUpdateServer(this.servers[4], FeedbackLevel.Normal);
            this.AssignServerLoop(count);
            this.CheckServer(this.servers[0], count, 27, 5);
            this.CheckServer(this.servers[1], count, 18, 5);
            this.CheckServer(this.servers[2], count, 18, 5);
            this.CheckServer(this.servers[3], count, 18, 5);
            this.CheckServer(this.servers[4], count, 18, 5);

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Low);
            this.TryUpdateServer(this.servers[1], FeedbackLevel.Highest);
            this.TryUpdateServer(this.servers[2], FeedbackLevel.Highest);
            this.TryUpdateServer(this.servers[3], FeedbackLevel.Highest);
            this.TryUpdateServer(this.servers[4], FeedbackLevel.Highest);
            this.AssignServerLoop(count);
            this.CheckServer(this.servers[0], count, 100, 0);
            this.CheckServer(this.servers[1], count, 0, 0);
            this.CheckServer(this.servers[2], count, 0, 0);
            this.CheckServer(this.servers[3], count, 0, 0);
            this.CheckServer(this.servers[4], count, 0, 0);

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Highest);
            Server server;
            Assert.IsFalse(this.balancer.TryGetServer(out server));
        }

        [Test]
        public void LoadSpread()
        {
            const int count = 100000;

            var loadLevelWeights = new int[] 
            { 
                50, // FeedbackLevel.Lowest
                30, // FeedbackLevel.Low
                15, // FeedbackLevel.Normal
                5, // FeedbackLevel.High
                0 // FeedbackLevel.Highest
            };

            this.balancer = new LoadBalancer<Server>(loadLevelWeights, 42);

            for (int i = 0; i < this.servers.Count; i++)
            {
                bool result = this.balancer.TryAddServer(this.servers[i], FeedbackLevel.Lowest);
                Assert.IsTrue(result);
            }

            // 5 servers with a load level of lowest
            // every server should get about 20 percent of the assignments
            this.AssignServerLoop(count);
            for (int i = 0; i < this.servers.Count; i++)
            {
                this.CheckServer(this.servers[i], count, 20, 5);
            }

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Lowest);
            this.TryUpdateServer(this.servers[1], FeedbackLevel.Low);
            this.TryUpdateServer(this.servers[2], FeedbackLevel.Normal);
            this.TryUpdateServer(this.servers[3], FeedbackLevel.High);
            this.TryUpdateServer(this.servers[4], FeedbackLevel.Highest);
            this.AssignServerLoop(count);
            this.CheckServer(this.servers[0], count, 50, 5);
            this.CheckServer(this.servers[1], count, 30, 5);
            this.CheckServer(this.servers[2], count, 15, 5);
            this.CheckServer(this.servers[3], count, 5, 5);
            this.CheckServer(this.servers[4], count, 0, 0);

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Lowest);
            this.TryUpdateServer(this.servers[1], FeedbackLevel.Low);
            this.TryUpdateServer(this.servers[2], FeedbackLevel.Low);
            this.TryUpdateServer(this.servers[3], FeedbackLevel.Normal);
            this.TryUpdateServer(this.servers[4], FeedbackLevel.Normal);
            this.AssignServerLoop(count);
            this.CheckServer(this.servers[0], count, 36, 5);
            this.CheckServer(this.servers[1], count, 21, 5);
            this.CheckServer(this.servers[2], count, 21, 5);
            this.CheckServer(this.servers[3], count, 11, 5);
            this.CheckServer(this.servers[4], count, 11, 5);

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Low);
            this.TryUpdateServer(this.servers[1], FeedbackLevel.Normal);
            this.TryUpdateServer(this.servers[2], FeedbackLevel.Normal);
            this.TryUpdateServer(this.servers[3], FeedbackLevel.Normal);
            this.TryUpdateServer(this.servers[4], FeedbackLevel.Normal);
            this.AssignServerLoop(count);
            this.CheckServer(this.servers[0], count, 33, 5);
            this.CheckServer(this.servers[1], count, 17, 5);
            this.CheckServer(this.servers[2], count, 17, 5);
            this.CheckServer(this.servers[3], count, 17, 5);
            this.CheckServer(this.servers[4], count, 17, 5);

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Low);
            this.TryUpdateServer(this.servers[1], FeedbackLevel.Highest);
            this.TryUpdateServer(this.servers[2], FeedbackLevel.Highest);
            this.TryUpdateServer(this.servers[3], FeedbackLevel.Highest);
            this.TryUpdateServer(this.servers[4], FeedbackLevel.Highest);
            this.AssignServerLoop(count);
            this.CheckServer(this.servers[0], count, 100, 0);
            this.CheckServer(this.servers[1], count, 0, 0);
            this.CheckServer(this.servers[2], count, 0, 0);
            this.CheckServer(this.servers[3], count, 0, 0);
            this.CheckServer(this.servers[4], count, 0, 0);

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Highest);
            Server server;
            Assert.IsFalse(this.balancer.TryGetServer(out server));
        }

        [Test]
        public void LoadSpreadFromConfig()
        {
            const int count = 100000;

            const string configPath = "LoadBalancer.config";
            this.balancer = new LoadBalancer<Server>(configPath); 

            for (int i = 0; i < this.servers.Count; i++)
            {
                bool result = this.balancer.TryAddServer(this.servers[i], FeedbackLevel.Lowest);
                Assert.IsTrue(result);
            }

            // 5 servers with a load level of lowest
            // every server should get about 20 percent of the assignments
            this.AssignServerLoop(count);
            for (int i = 0; i < this.servers.Count; i++)
            {
                this.CheckServer(this.servers[i], count, 20, 5);
            }

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Lowest);
            this.TryUpdateServer(this.servers[1], FeedbackLevel.Low);
            this.TryUpdateServer(this.servers[2], FeedbackLevel.Normal);
            this.TryUpdateServer(this.servers[3], FeedbackLevel.High);
            this.TryUpdateServer(this.servers[4], FeedbackLevel.Highest);
            this.AssignServerLoop(count);
            this.CheckServer(this.servers[0], count, 25, 5);
            this.CheckServer(this.servers[1], count, 25, 5);
            this.CheckServer(this.servers[2], count, 25, 5);
            this.CheckServer(this.servers[3], count, 25, 5);
            this.CheckServer(this.servers[4], count, 0, 0);

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Lowest);
            this.TryUpdateServer(this.servers[1], FeedbackLevel.Low);
            this.TryUpdateServer(this.servers[2], FeedbackLevel.Low);
            this.TryUpdateServer(this.servers[3], FeedbackLevel.Normal);
            this.TryUpdateServer(this.servers[4], FeedbackLevel.Normal);
            this.AssignServerLoop(count);
            this.CheckServer(this.servers[0], count, 20, 5);
            this.CheckServer(this.servers[1], count, 20, 5);
            this.CheckServer(this.servers[2], count, 20, 5);
            this.CheckServer(this.servers[3], count, 20, 5);
            this.CheckServer(this.servers[4], count, 20, 5);

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Low);
            this.TryUpdateServer(this.servers[1], FeedbackLevel.Normal);
            this.TryUpdateServer(this.servers[2], FeedbackLevel.Normal);
            this.TryUpdateServer(this.servers[3], FeedbackLevel.Normal);
            this.TryUpdateServer(this.servers[4], FeedbackLevel.Normal);
            this.AssignServerLoop(count);
            this.CheckServer(this.servers[0], count, 20, 5);
            this.CheckServer(this.servers[1], count, 20, 5);
            this.CheckServer(this.servers[2], count, 20, 5);
            this.CheckServer(this.servers[3], count, 20, 5);
            this.CheckServer(this.servers[4], count, 20, 5);

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Low);
            this.TryUpdateServer(this.servers[1], FeedbackLevel.Highest);
            this.TryUpdateServer(this.servers[2], FeedbackLevel.Highest);
            this.TryUpdateServer(this.servers[3], FeedbackLevel.Highest);
            this.TryUpdateServer(this.servers[4], FeedbackLevel.Highest);
            this.AssignServerLoop(count);
            this.CheckServer(this.servers[0], count, 100, 0);
            this.CheckServer(this.servers[1], count, 0, 0);
            this.CheckServer(this.servers[2], count, 0, 0);
            this.CheckServer(this.servers[3], count, 0, 0);
            this.CheckServer(this.servers[4], count, 0, 0);

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Highest);
            Server server;
            Assert.IsFalse(this.balancer.TryGetServer(out server));
        }

        [Test]
        public void LoadSpreadAfterConfigChange()
        {
            const int count = 100000;

            const string configPath = "LoadBalancer.config";
            this.balancer = new LoadBalancer<Server>(configPath);

            for (int i = 0; i < this.servers.Count; i++)
            {
                bool result = this.balancer.TryAddServer(this.servers[i], FeedbackLevel.Lowest);
                Assert.IsTrue(result);
            }

            // 5 servers with a load level of lowest
            // every server should get about 20 percent of the assignments
            this.AssignServerLoop(count);
            for (int i = 0; i < this.servers.Count; i++)
            {
                this.CheckServer(this.servers[i], count, 20, 5);
            }

            this.TryUpdateServer(this.servers[0], FeedbackLevel.Lowest);
            this.TryUpdateServer(this.servers[1], FeedbackLevel.Low);
            this.TryUpdateServer(this.servers[2], FeedbackLevel.Normal);
            this.TryUpdateServer(this.servers[3], FeedbackLevel.High);
            this.TryUpdateServer(this.servers[4], FeedbackLevel.Highest);
           
            this.AssignServerLoop(count);
            this.CheckServer(this.servers[0], count, 25, 5);
            this.CheckServer(this.servers[1], count, 25, 5);
            this.CheckServer(this.servers[2], count, 25, 5);
            this.CheckServer(this.servers[3], count, 25, 5);
            this.CheckServer(this.servers[4], count, 0, 0);

            File.Copy("LoadBalancer.config", "LoadBalancer.config.bak", true);
            File.Delete("LoadBalancer.config");
            
            // wait a bit until the update is done: 
            Thread.Sleep(1000);

            this.AssignServerLoop(count);

            this.CheckServer(this.servers[0], count, 40, 5);
            this.CheckServer(this.servers[1], count, 30, 5);
            this.CheckServer(this.servers[2], count, 20, 5);
            this.CheckServer(this.servers[3], count, 10, 5);
            this.CheckServer(this.servers[4], count, 0, 0);

            File.Copy("LoadBalancer.config.bak", "LoadBalancer.config", true);

            // wait a bit until the update is done: 
            Thread.Sleep(1000);

            this.AssignServerLoop(count);
            this.CheckServer(this.servers[0], count, 25, 5);
            this.CheckServer(this.servers[1], count, 25, 5);
            this.CheckServer(this.servers[2], count, 25, 5);
            this.CheckServer(this.servers[3], count, 25, 5);
            this.CheckServer(this.servers[4], count, 0, 0);
        }

        private void AssignServerLoop(int count)
        {
            this.ResetServerCount();

            for (int i = 0; i < count; i++)
            {
                Server server;
                bool result = this.balancer.TryGetServer(out server);
                Assert.IsTrue(result);
                server.Count++;
            }
        }

        private void ResetServerCount()
        {
            for (int i = 0; i < this.servers.Count; i++)
            {
                this.servers[i].Count = 0;
            }
        }

        private void CheckServer(Server server, int count, int expectedPercent, int toleranceInPercent)
        {
            int expectedCount = count * expectedPercent / 100;
            int tolerance = Math.Abs(expectedCount * toleranceInPercent / 100);

            int difference = Math.Abs(expectedCount - server.Count);
            if (difference > tolerance)
            {
                Assert.Fail(
                    "{0} has an unexpected count of assignments. Expected a value between {1} and {2} but is {3}", 
                    server.Name, 
                    expectedCount - tolerance, 
                    expectedCount + tolerance, 
                    server.Count);
            }
        }

        private void TryAddServer(Server server, FeedbackLevel loadLevel, bool expectedResult = true)
        {
            var result = this.balancer.TryAddServer(server, loadLevel);
            Assert.AreEqual(result, expectedResult);
        }

        private void TryUpdateServer(Server server, FeedbackLevel newLoadLevel, bool expectedResult = true)
        {
            var result = this.balancer.TryUpdateServer(server, newLoadLevel);
            Assert.AreEqual(expectedResult, result, "Unexpected update server result.");
        }

        private void TryGetServer(out Server server, bool expectedResult = true)
        {
            var result = this.balancer.TryGetServer(out server);
            Assert.AreEqual(expectedResult, result);
        }

        private void TryRemoveServer(Server server, bool expectedResult = true)
        {
            bool result = this.balancer.TryRemoveServer(server);
            Assert.AreEqual(expectedResult, result);
        }

        private void CheckLoadBalancerProperties(int totalWorkload, int totalWeight, int averageWorkloadPercent)
        {
            Assert.AreEqual(totalWorkload, this.balancer.TotalWorkload);
            Assert.AreEqual(totalWeight, this.balancer.TotalWeight);
            Assert.AreEqual(averageWorkloadPercent, 25 * (int)this.balancer.AverageWorkload);
        }

        private class Server
        {
            public string Name { get; set; }

            public int Count { get; set; }

            public override string ToString()
            {
                return string.Format("{0}: Count={1}", this.Name, this.Count);
            }
        }
    }
}
