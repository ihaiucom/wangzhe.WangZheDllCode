// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.StarDust.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    using ExitGames.Concurrency.Fibers;
    using ExitGames.Logging;
    using ExitGames.Threading;

    using log4net;
    using log4net.Config;

    using Photon.StarDust.Client.Connections;

    using LogManager = ExitGames.Logging.LogManager;

    /// <summary>
    /// The program.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The base game name.
        /// </summary>
        private static readonly string baseGameName = string.Format("{0}({1})", Environment.MachineName, Process.GetCurrentProcess().Id);

        /// <summary>
        /// The fiber.
        /// </summary>
        private static readonly PoolFiber fiber = new PoolFiber(new FailSafeBatchExecutor());

        private static readonly PoolFiber counterLoggingFiber = new PoolFiber(new FailSafeBatchExecutor());

        /// <summary>
        /// Used for logging.
        /// </summary>
        private static readonly string firstGameName = string.Format("{0}", Environment.MachineName);

        /// <summary>
        /// The games.
        /// </summary>
        private static readonly Dictionary<string, List<ClientConnection>> games = new Dictionary<string, List<ClientConnection>>();

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The random.
        /// </summary>
        private static readonly Random random = new Random();

        /// <summary>
        /// The reset event.
        /// </summary>
        private static readonly ManualResetEvent resetEvent = new ManualResetEvent(false);

        /// <summary>
        /// The game counter.
        /// </summary>
        private static int gameCounter;

        /// <summary>
        /// The stopped.
        /// </summary>
        private static bool stopped;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Main(params string[] args)
        {
            LogManager.SetLoggerFactory(ExitGames.Logging.Log4Net.Log4NetLoggerFactory.Instance);
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);

            // Set logfile name and application name variables
            FileInfo configFileInfo;
            try
            {
                // IOException if another process already writes to the file
                if (File.Exists(@"log\" + firstGameName + ".log"))
                {
                    File.Delete(@"log\" + firstGameName + ".log");
                }

                GlobalContext.Properties["LogName"] = firstGameName + ".log";

                configFileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config"));
            }
            catch (IOException)
            {
                GlobalContext.Properties["LogName"] = baseGameName + ".log";

                // just write to console for now
                configFileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4netConsole.config"));
            }

            XmlConfigurator.ConfigureAndWatch(configFileInfo);

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            WindowsCounters.Initialize();

            int index = 0;
            if (args.Length > index)
            {
                Settings.TestCase = args[index];
            }

            index++;
            if (args.Length > index)
            {
                Settings.NumGames = byte.Parse(args[index]);
            }

            index++;
            if (args.Length > index)
            {
                Settings.NumClientsPerGame = byte.Parse(args[index]);
            }

            index++;
            if (args.Length > index)
            {
                Settings.ServerAddress = args[index];
            }

            index++;
            if (args.Length > index)
            {
                Settings.FlushInterval = int.Parse(args[index]);
            }

            index++;
            if (args.Length > index)
            {
                Settings.SendReliableData = bool.Parse(args[index]);
            }

            index++;
            if (args.Length > index)
            {
                Settings.ReliableDataSendInterval = int.Parse(args[index]);
            }

            index++;
            if (args.Length > index)
            {
                Settings.SendUnreliableData = bool.Parse(args[index]);
            }

            index++;
            if (args.Length > index)
            {
                Settings.UnreliableDataSendInterval = int.Parse(args[index]);
            }

            Console.WriteLine("Test case: " + Settings.TestCase);
            Console.WriteLine("Settings: {0} games per process, {1} players per game, game server at {2}", 
                Settings.NumGames, 
                Settings.NumClientsPerGame, 
                Settings.ServerAddress);

            if (Settings.SendReliableData)
            {
                Console.WriteLine("Sending reliable operation every {0} ms", Settings.ReliableDataSendInterval);
            }

            if (Settings.SendUnreliableData)
            {
                Console.WriteLine("Sending unreliable operation every {0} ms", Settings.UnreliableDataSendInterval);
            }

            Console.WriteLine();

            stopped = false;
            fiber.Start();
            counterLoggingFiber.Start();
            
            counterLoggingFiber.ScheduleOnInterval(PrintCounter, Settings.LogCounterInterval, Settings.LogCounterInterval);

            try
            {
                log.InfoFormat("Starting {0} games with {1} players", Settings.NumGames, Settings.NumClientsPerGame);
                for (int g = 0; g < Settings.NumGames; g++)
                {
                    fiber.Enqueue(StartGame);
                }

                ////log.InfoFormat("{0} clients running", clients.Count);
                Console.WriteLine("[{0}] Press Return to End", Process.GetCurrentProcess().Id);
                Console.ReadLine();

                ////log.InfoFormat("Stopping {0} clients", clients.Count);
                StopGames();
                fiber.Stop(); 
                
                // wait for stop to complete
                resetEvent.WaitOne();
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            finally
            {
                log.InfoFormat("[{0}] Stopped", Process.GetCurrentProcess().Id);
            }
        }

        /// <summary>
        /// The current domain_ unhandled exception.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Error(e.ExceptionObject);
        }

        /// <summary>
        /// The print counter.
        /// </summary>
        private static void PrintCounter()
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat(
                    "peers: {0}, r-ev: {1}, ur-ev: {2}, r-op: {3}, ur-op: {4}, res: {5}, r-rtt: {6}, ur-rtt: {7}, rtt: {8}, var: {9}, flsh-rtt: {10}, flsh-ev {11}, flsh-op {12}", 
                    Counters.ConnectedClients.GetNextValue(), 
                    Counters.ReliableEventsReceived.GetNextValue(), 
                    Counters.UnreliableEventsReceived.GetNextValue(), 
                    Counters.ReliableOperationsSent.GetNextValue(), 
                    Counters.UnreliableOperationsSent.GetNextValue(), 
                    Counters.ReceivedOperationResponse.GetNextValue(), 
                    Counters.ReliableEventRoundTripTime.GetNextValue(), 
                    Counters.UnreliableEventRoundTripTime.GetNextValue(), 
                    Counters.RoundTripTime.GetNextValue(), 
                    Counters.RoundTripTimeVariance.GetNextValue(), 
                    Counters.FlushEventRoundTripTime.GetNextValue(), 
                    Counters.FlushEventsReceived.GetNextValue(), 
                    Counters.FlushOperationsSent.GetNextValue());
            }
            else
            {
                // reset average counters
                Counters.ConnectedClients.GetNextValue();
                Counters.ReliableEventsReceived.GetNextValue();
                Counters.UnreliableEventsReceived.GetNextValue();
                Counters.ReliableOperationsSent.GetNextValue();
                Counters.UnreliableOperationsSent.GetNextValue();
                Counters.ReceivedOperationResponse.GetNextValue();
                Counters.ReliableEventRoundTripTime.GetNextValue();
                Counters.UnreliableEventRoundTripTime.GetNextValue();
                Counters.RoundTripTime.GetNextValue();
                Counters.RoundTripTimeVariance.GetNextValue();
                Counters.FlushEventRoundTripTime.GetNextValue();
                Counters.FlushEventsReceived.GetNextValue();
                Counters.FlushOperationsSent.GetNextValue();
            }
        }

        /// <summary>
        /// The start game.
        /// </summary>
        private static void StartGame()
        {
            if (stopped)
            {
                return;
            }

            gameCounter++;
            string gameName = baseGameName + gameCounter;
            var clients = new List<ClientConnection>(Settings.NumClientsPerGame);
            games.Add(gameName, clients);
            for (int i = 0; i < Settings.NumClientsPerGame; i++)
            {
                var client = ClientConnectionFactory.GetClientConnection(Settings.PhotonApplication, gameName, i); 
                client.Start();
                clients.Add(client);

                // don't start all at once, that would not be realistic
                int sleep = random.Next((int)(Settings.StartupInterval * 0.5), (int)(Settings.StartupInterval * 1.5));
                Thread.Sleep(sleep);
            }

            log.InfoFormat("[{2}] Started game {1} with {0} clients", Settings.NumClientsPerGame, gameName, Process.GetCurrentProcess().Id);

            if (Settings.TimeInGame > 0)
            {
                fiber.Schedule(() => StopGame(gameName), (long)TimeSpan.FromSeconds(Settings.TimeInGame).TotalMilliseconds);
            }
        }

        /// <summary>
        /// The stop game.
        /// </summary>
        /// <param name="gameName">
        /// The game name.
        /// </param>
        private static void StopGame(string gameName)
        {
            List<ClientConnection> clients;
            if (games.TryGetValue(gameName, out clients))
            {
                clients.ForEach(c => c.Stop());
                clients.Clear();
                games.Remove(gameName);
                log.InfoFormat("[{1}] Stopped game {0}", gameName, Process.GetCurrentProcess().Id);
            }

            StartGame();
        }

        /// <summary>
        /// The stop games.
        /// </summary>
        private static void StopGames()
        {
            try
            {
                foreach (KeyValuePair<string, List<ClientConnection>> entry in games)
                {
                    entry.Value.ForEach(c => c.Stop());
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
    }
}