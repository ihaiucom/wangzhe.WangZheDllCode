// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.TestConsole
{
    using System;
    using System.Threading;

    using Photon.Hive.Tests;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        private static void Main()
        {
            var setupFixture = new SetupFixture();
            setupFixture.Setup();

            while (true)
            {
                Console.WriteLine("1 - Tcp");
                Console.WriteLine("2 - Tcp performance tests #2");
                Console.WriteLine("3 - Tcp performance tests");
                Console.WriteLine("4 - Tcp s2s performance tests");

                Console.WriteLine();
                Console.WriteLine("Q - Quit");

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1:
                        RunTcpClientTests();
                        break;

                    case ConsoleKey.D2:
                        RunTcpPerformanceTests(2);
                        break;

                    case ConsoleKey.D3:
                        RunTcpPerformanceTests(1);
                        break;

                    case ConsoleKey.D4:
                        RunS2sPerformanceTests();
                        break;

                    case ConsoleKey.Q:
                        return;
                }
            }
        }

        private static void RunS2sPerformanceTests()
        {
            Console.Clear();
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("Tcp performance");
            Console.WriteLine("---------------------------------------");

            var tests = new ServerToServerTests();

            tests.Ping();

            Thread.Sleep(1000);

            Console.WriteLine("Tests finished.");
            Console.WriteLine("Press key to continue");
            Console.ReadKey(true);
            Console.Clear();
        }

        /// <summary>
        /// The run tcp client tests.
        /// </summary>
        private static void RunTcpClientTests()
        {
            Console.Clear();

            Console.WriteLine("---------------------------------------");
            Console.WriteLine("Tcp protocol version 1.5 (byte codes)");
            Console.WriteLine("---------------------------------------");
            var unitTests = new TcpTests();
            TestTcpClient(unitTests);
            Thread.Sleep(1000);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Tests finished.");
            Console.WriteLine("Press key to continue");
            Console.ReadKey(true);
            Console.Clear();
        }

        ////private static void RunAmf3ClientTests()
        ////{
        ////    Console.Clear();
        ////    Console.WriteLine("---------------------------------------");
        ////    Console.WriteLine("Amf3 protocol");
        ////    Console.WriteLine("---------------------------------------");

        ////    TestBase unitTests = new TcpTests.Amf();
        ////    TestTcpClient(unitTests);
        ////    System.Threading.Thread.Sleep(1000);

        ////    Console.WriteLine();
        ////    Console.WriteLine();
        ////    Console.WriteLine("Tests finished.");
        ////    Console.WriteLine("Press key to continue");
        ////    Console.ReadKey(true);
        ////    Console.Clear();
        ////}

        /// <summary>
        /// The run tcp performance tests.
        /// </summary>
        private static void RunTcpPerformanceTests(int test)
        {
            Console.Clear();
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("Tcp performance");
            Console.WriteLine("---------------------------------------");

            var tests = new TcpPerformanceTests();

            tests.Setup();

            if (test == 1) tests.Ping();
            else if (test == 2) tests.PingPing();

            Thread.Sleep(1000);

            tests.TearDown();

            Console.WriteLine("Tests finished.");
            Console.WriteLine("Press key to continue");
            Console.ReadKey(true);
            Console.Clear();
        }

        /// <summary>
        /// The test tcp client.
        /// </summary>
        /// <param name="unitTests">
        /// The unit tests.
        /// </param>
        private static void TestTcpClient(TcpTests unitTests)
        {
            Console.WriteLine("Send ping");
            unitTests.SendPing();

            Console.WriteLine("Join");
            unitTests.Join();

            Console.WriteLine("Join with channel");
            unitTests.JoinWithChannel();

            Console.WriteLine("Join with int properties");
            unitTests.JoinWithPropertiesInt();

            Console.WriteLine("Join with string properties");
            unitTests.JoinWithPropertiesString();

            Console.WriteLine("Send custom event");
            unitTests.SendCustomEvent();

            Console.WriteLine("Set properties with broadcast");
            unitTests.SetPropertiesWithBroadcast();

            Console.WriteLine("Set properties with boradcast (Version 1.5)");
            unitTests.SetPropertiesWithBroadcastV15();

            Console.WriteLine();
        }
    }
}