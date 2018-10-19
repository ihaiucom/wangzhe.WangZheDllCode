// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.TestClient
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    using ExitGames.Concurrency.Fibers;
    using ExitGames.Logging;
    using ExitGames.Threading;

    using log4net.Config;

    using Photon.LoadBalancing.TestClient.ConnectionStates;

    using LogManager = ExitGames.Logging.LogManager;

    #endregion

    public class Program
    {
        #region Constants and Fields
        
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        
        private static readonly PoolFiber fiber = new PoolFiber(new FailSafeBatchExecutor());
        
        private static Dictionary<string, List<Master>> games = new Dictionary<string, List<Master>>(); 

        private static bool stopped;

        private static readonly Random random = new Random();

        private static readonly ManualResetEvent resetEvent = new ManualResetEvent(false);

        private static int gameCounter;

        private static readonly string baseGameName = string.Format("{0}({1})", Environment.MachineName, Process.GetCurrentProcess().Id);
        #endregion

        #region Public Methods

        public static void Main(string[] args)
        {
            Setup();

            fiber.Start();

            //fiber.ScheduleOnInterval()

            for (int i = 0; i < Settings.NumGames; i++)
            {
                fiber.Enqueue(StartGame);
            }

            Console.WriteLine("[{0}] Press Return to End", Process.GetCurrentProcess().Id);
            Console.ReadLine();

            ///log.InfoFormat("Stopping {0} clients", clients.Count);
            fiber.Enqueue(StopGames);

            // wait for stop to complete
            resetEvent.WaitOne();

            Console.ReadLine();
        }
        #endregion

        #region Methods

        private static void Setup()
        {
            LogManager.SetLoggerFactory(ExitGames.Logging.Log4Net.Log4NetLoggerFactory.Instance);
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            
            var configFileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config"));
            XmlConfigurator.ConfigureAndWatch(configFileInfo);
            
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            //WindowsCounters.Initialize();
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Error(e.ExceptionObject);
        }

        private static void StartGame()
        {
            gameCounter++;
            string gameName = baseGameName + gameCounter;
            var masters = new List<Master>(Settings.NumClientsPerGame);
            games.Add(gameName, masters);
            for (int i = 0; i < Settings.NumClientsPerGame; i++)
            {
                var master = new Master();
                master.Start(gameName, i);
                masters.Add(master);

                // don't start all at once, that would not be realistic
                int sleep = random.Next(10, 300);
                Thread.Sleep(sleep);
            }

            log.InfoFormat("[{2}] Started game {1} with {0} clients", Settings.NumClientsPerGame, gameName, Process.GetCurrentProcess().Id);

            //if (Settings.TimeInGame > 0)
            //{
            //    fiber.Schedule(() => StopGame(gameName), Settings.TimeInGame);
            //}
        }

        private static void StopGame(string gameName)
        {
            List<Master> masters;
            if (games.TryGetValue(gameName, out masters))
            {
                masters.ForEach(m => m.Stop());
                masters.Clear();
                games.Remove(gameName);
                log.InfoFormat("[{1}] Stopped game {0}", gameName, Process.GetCurrentProcess().Id);
            }

            StartGame();
        }

        private static void StopGames()
        {
            try
            {
                foreach (KeyValuePair<string, List<Master>> entry in games)
                {
                    entry.Value.ForEach(m => m.Stop());
                    entry.Value.Clear();
                    log.InfoFormat("[{1}] Shutdown: Stopped game {0}", entry.Key, Process.GetCurrentProcess().Id);
                }

                games.Clear();
            }
            finally
            {
                stopped = true;
                resetEvent.Set();
            }
        }

        #endregion
    }
}