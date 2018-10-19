// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowsCounters.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the WindowsCounters type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.StarDust.Client
{
    using System;
    using System.Data;
    using System.Diagnostics;

    using ExitGames.Diagnostics.Counter;
    using ExitGames.Logging;
    using System.Threading;
    using System.Security.Permissions;

    public static class WindowsCounters
    {
        private static Mutex mut = new Mutex(false, "Photon.StarDust.Client");
        private static bool boss = false;
        private static bool useDummyCounters = false; 

        const string connectedClients = "Connected Clients";
        const string receivedOperationResponsesSec = "Received Operation Responses / sec";
        const string reliableEventRoundtripTime = "Reliable Event RoundTrip Time";
        const string flushoperationsSentSec = "FlushOperations Sent / sec";
        const string flusheventRoundtripTime = "FlushEvent RoundTrip Time";
        const string flusheventsReceivedSec = "FlushEvents Received / sec";



        const string reliableEventsReceivedSec = "Reliable Events Received / sec";
        const string reliableOperationsSentSec = "Reliable Operations Sent / sec";
        const string roundtripTime = "RoundTrip Time";
        const string roundtripTimeVariance = "RoundTrip Time Variance";
        const string unreliableEventRoundtripTime = "Unreliable Event RoundTrip Time";
        const string unreliableEventsReceivedSec = "Unreliable Events Received / sec";
        const string unreliableOperationsSentSec = "Unreliable Operations Sent / sec";

        const string flushEventRoundtripTimeBase = "FlushEvent RoundTrip Time Base";
        const string reliableEventRoundtripTimeBase = "Reliable Event RoundTrip Time Base";
        const string roundtripTimeBase = "RoundTrip Time Base";
        const string roundtripTimeVarianceBase = "RoundTrip Time Variance Base";
        const string unreliableEventRoundtripTimeBase = "Unreliable Event RoundTrip Time Base";

        public static readonly string CategoryName = "Photon: Stardust Client";

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static Timer TotalTimer = null;

        private static PerformanceCounterCategory Category = null;

        static WindowsCounters()
        {
            //try
            //{
            //    SetupPerformanceCounters();
            //}
            //catch (Exception e)
            //{
            //    log.Warn("Could not create PerformanceCounterCategory " + CategoryName, e);

            //    SetupDummyCounters();
            //}

            SetupDummyCounters();
        }

        private static void SetupPerformanceCounters()
        {
            if (!PerformanceCounterCategory.Exists(CategoryName))
            {
                Category = PerformanceCounterCategory.Create(
                    CategoryName,
                    string.Empty,
                    PerformanceCounterCategoryType.MultiInstance,
                    new CounterCreationDataCollection(
                        new[]
                            {
                                new CounterCreationData(connectedClients, string.Empty, PerformanceCounterType.NumberOfItems64),
                                new CounterCreationData(flusheventRoundtripTime, string.Empty, PerformanceCounterType.AverageCount64),
                                new CounterCreationData(flushEventRoundtripTimeBase, string.Empty, PerformanceCounterType.AverageBase),
                                new CounterCreationData(flusheventsReceivedSec, string.Empty, PerformanceCounterType.RateOfCountsPerSecond64),
                                new CounterCreationData(flushoperationsSentSec, string.Empty, PerformanceCounterType.RateOfCountsPerSecond64),
                                new CounterCreationData(receivedOperationResponsesSec, string.Empty, PerformanceCounterType.RateOfCountsPerSecond64),
                                new CounterCreationData(reliableEventRoundtripTime, string.Empty, PerformanceCounterType.AverageCount64),
                                new CounterCreationData(reliableEventRoundtripTimeBase, string.Empty, PerformanceCounterType.AverageBase),
                                new CounterCreationData(reliableEventsReceivedSec, string.Empty, PerformanceCounterType.RateOfCountsPerSecond64),
                                new CounterCreationData(reliableOperationsSentSec, string.Empty, PerformanceCounterType.RateOfCountsPerSecond64),
                                new CounterCreationData(roundtripTime, string.Empty, PerformanceCounterType.AverageCount64),
                                new CounterCreationData(roundtripTimeBase, string.Empty, PerformanceCounterType.AverageBase),
                                new CounterCreationData(roundtripTimeVariance, string.Empty, PerformanceCounterType.AverageCount64),
                                new CounterCreationData(roundtripTimeVarianceBase, string.Empty, PerformanceCounterType.AverageBase),
                                new CounterCreationData(unreliableEventRoundtripTime, string.Empty, PerformanceCounterType.AverageCount64),
                                new CounterCreationData(unreliableEventRoundtripTimeBase, string.Empty, PerformanceCounterType.AverageBase),
                                new CounterCreationData(unreliableEventsReceivedSec, string.Empty, PerformanceCounterType.RateOfCountsPerSecond64),
                                new CounterCreationData(unreliableOperationsSentSec, string.Empty, PerformanceCounterType.RateOfCountsPerSecond64)
                            }));
            }
            else
            {
                var list = PerformanceCounterCategory.GetCategories();
                foreach (var c in list) if (c.CategoryName == CategoryName) Category = c;
            }

            var procId = Process.GetCurrentProcess().Id.ToString();
            ConnectedClientsTotal = GetPerformanceCounter(connectedClients, "_Total", false);
            ConnectedClients = new WindowsPerformanceCounter(GetPerformanceCounter(connectedClients, procId, true));
            FlushEventRoundTripTime = new WindowsPerformanceCounter(GetPerformanceCounter(flusheventRoundtripTime, "_Total", false));
            //                    new WindowsPerformanceCounter(
            //                        new PerformanceCounter(CategoryName, flusheventRoundtripTime, AppDomain.CurrentDomain.FriendlyName, false));
            FlushEventsReceived = new WindowsPerformanceCounter(GetPerformanceCounter(flusheventsReceivedSec, "_Total", false));
            //                    new WindowsPerformanceCounter(
            //                        new PerformanceCounter(CategoryName, flusheventsReceivedSec, AppDomain.CurrentDomain.FriendlyName, false));
            FlushOperationsSent = new WindowsPerformanceCounter(GetPerformanceCounter(flushoperationsSentSec, "_Total", false));
            //                    new WindowsPerformanceCounter(
            //                        new PerformanceCounter(CategoryName, flushoperationsSentSec, AppDomain.CurrentDomain.FriendlyName, false));
            ReceivedOperationResponse = new WindowsPerformanceCounter(GetPerformanceCounter(receivedOperationResponsesSec, "_Total", false));
            //                    new WindowsPerformanceCounter(
            //                        new PerformanceCounter(CategoryName, receivedOperationResponsesSec, AppDomain.CurrentDomain.FriendlyName, false));
            ReliableEventsReceived = new WindowsPerformanceCounter(GetPerformanceCounter(reliableEventsReceivedSec, "_Total", false));
            //                    new WindowsPerformanceCounter(
            //                        new PerformanceCounter(CategoryName, reliableEventsReceivedSec, AppDomain.CurrentDomain.FriendlyName, false));
            ReliableEventRoundTripTime = new WindowsPerformanceCounter(GetPerformanceCounter(reliableEventRoundtripTime, "_Total", false));
            //                    new WindowsPerformanceCounter(
            //                        new PerformanceCounter(CategoryName, reliableEventRoundtripTime, AppDomain.CurrentDomain.FriendlyName, false));
            ReliableOperationsSent = new WindowsPerformanceCounter(GetPerformanceCounter(reliableOperationsSentSec, "_Total", false));
            //                    new WindowsPerformanceCounter(
            //                        new PerformanceCounter(CategoryName, reliableOperationsSentSec, AppDomain.CurrentDomain.FriendlyName, false));
            RoundTripTime = new WindowsPerformanceCounter(GetPerformanceCounter(roundtripTime, "_Total", false));
            //                    new WindowsPerformanceCounter(
            //                        new PerformanceCounter(CategoryName, roundtripTime, AppDomain.CurrentDomain.FriendlyName, false));
            RoundTripTimeVariance = new WindowsPerformanceCounter(GetPerformanceCounter(roundtripTimeVariance, "_Total", false));
            //                    new WindowsPerformanceCounter(
            //                        new PerformanceCounter(CategoryName, roundtripTimeVariance, AppDomain.CurrentDomain.FriendlyName, false));
            UnreliableEventRoundTripTime = new WindowsPerformanceCounter(GetPerformanceCounter(unreliableEventRoundtripTime, "_Total", false));
            //                    new WindowsPerformanceCounter(
            //                        new PerformanceCounter(CategoryName, unreliableEventRoundtripTime, AppDomain.CurrentDomain.FriendlyName, false));
            UnreliableEventsReceived = new WindowsPerformanceCounter(GetPerformanceCounter(unreliableEventsReceivedSec, "_Total", false));
            //                    new WindowsPerformanceCounter(
            //                        new PerformanceCounter(CategoryName, unreliableEventsReceivedSec, AppDomain.CurrentDomain.FriendlyName, false));
            UnreliableOperationsSent = new WindowsPerformanceCounter(GetPerformanceCounter(unreliableOperationsSentSec, "_Total", false));
            //                    new WindowsPerformanceCounter(
            //                        new PerformanceCounter(CategoryName, unreliableOperationsSentSec, AppDomain.CurrentDomain.FriendlyName, false));

            // base counters: 
            FlusheventRoundtripTimeBase = new WindowsPerformanceCounter(GetPerformanceCounter(flushEventRoundtripTimeBase, "_Total", false));
            //                    new WindowsPerformanceCounter(
            //                        new PerformanceCounter(CategoryName, flushEventRoundtripTimeBase, AppDomain.CurrentDomain.FriendlyName, false));
            ReliableEventRoundtripTimeBase = new WindowsPerformanceCounter(GetPerformanceCounter(reliableEventRoundtripTimeBase, "_Total", false));
            //                new WindowsPerformanceCounter(
            //                        new PerformanceCounter(CategoryName, reliableEventRoundtripTimeBase, AppDomain.CurrentDomain.FriendlyName, false));
            RoundtripTimeBase = new WindowsPerformanceCounter(GetPerformanceCounter(roundtripTimeBase, "_Total", false));
            //                new WindowsPerformanceCounter(
            //                        new PerformanceCounter(CategoryName, roundtripTimeBase, AppDomain.CurrentDomain.FriendlyName, false));
            RoundtripTimeVarianceBase = new WindowsPerformanceCounter(GetPerformanceCounter(roundtripTimeVarianceBase, "_Total", false));
            //                new WindowsPerformanceCounter(
            //                        new PerformanceCounter(CategoryName, roundtripTimeVarianceBase, AppDomain.CurrentDomain.FriendlyName, false));
            UnreliableEventRoundtripTimeBase = new WindowsPerformanceCounter(GetPerformanceCounter(unreliableEventRoundtripTimeBase, "_Total", false));
            //                new WindowsPerformanceCounter(
            //                        new PerformanceCounter(CategoryName, unreliableEventRoundtripTimeBase, AppDomain.CurrentDomain.FriendlyName, false));

            TotalTimer = new Timer(new TimerCallback(UpdateTotalCounters), null, 1000, 0);
        }

        private static void SetupDummyCounters()
        {
            useDummyCounters = true; 

            ConnectedClients = new DummyCounter(connectedClients);
            FlushEventRoundTripTime = new DummyCounter(flusheventRoundtripTime);
            FlushEventsReceived = new DummyCounter(flusheventsReceivedSec);
            FlushOperationsSent = new DummyCounter("Flush Operations Sent / sec");
            ReceivedOperationResponse = new DummyCounter(receivedOperationResponsesSec);
            ReliableEventsReceived = new DummyCounter(reliableEventsReceivedSec);
            ReliableEventRoundTripTime = new DummyCounter(reliableEventRoundtripTime);
            ReliableOperationsSent = new DummyCounter(reliableOperationsSentSec);
            RoundTripTime = new DummyCounter(roundtripTime);
            RoundTripTimeVariance = new DummyCounter(roundtripTimeVariance);
            UnreliableEventRoundTripTime = new DummyCounter(unreliableEventRoundtripTime);
            UnreliableEventsReceived = new DummyCounter(unreliableEventsReceivedSec);
            UnreliableOperationsSent = new DummyCounter(unreliableOperationsSentSec);

            // Base counters: 
            FlusheventRoundtripTimeBase = new DummyCounter(flushEventRoundtripTimeBase);
            ReliableEventRoundtripTimeBase = new DummyCounter(reliableEventRoundtripTimeBase);
            RoundtripTimeBase = new DummyCounter(roundtripTimeBase);
            RoundtripTimeVarianceBase = new DummyCounter(roundtripTimeVarianceBase);
            UnreliableEventRoundtripTimeBase = new DummyCounter(unreliableEventRoundtripTimeBase); 
        }

        [HostProtectionAttribute(Synchronization = true, ExternalThreading = true)]
        private static void UpdateTotalCounters(Object stateInfo)
        {
            if (useDummyCounters) return; 
            
            if (!boss)
            {
            label:
                try
                {
                    mut.WaitOne();
                }
                catch (AbandonedMutexException)
                {
                    mut.Close();
                    mut = new Mutex(false, "Photon.StarDust.Client");
                    goto label;
                }
            boss = true;
            }
            try
            {
                long cnt = 0;

                if (null != Category)
                {
                    //Console.WriteLine("UpdateTotalCounters");
                    var list = Category.GetInstanceNames();
                    foreach (var i in list)
                        if ("_Total" != i && "_total" != i)
                        {
                            var counters = Category.GetCounters(i);
                            foreach (var c in counters)
                                if (connectedClients == c.CounterName)
                                    cnt += c.RawValue;
                        }
                }

                TotalTimer = new Timer(new TimerCallback(UpdateTotalCounters), null, 1000, 0);
                Console.WriteLine("Connections on all Test Client instances: " + cnt);
                ConnectedClientsTotal.RawValue = cnt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static PerformanceCounter GetPerformanceCounter(string counterName, string instanceName, bool isProcessLifeTime)
        {
            var tmpCounter = new PerformanceCounter();
            if (isProcessLifeTime)
                tmpCounter.InstanceLifetime = PerformanceCounterInstanceLifetime.Process;
            tmpCounter.CategoryName = CategoryName;
            tmpCounter.CounterName = counterName;
            tmpCounter.InstanceName = instanceName;
            tmpCounter.ReadOnly = false;
            return tmpCounter;
        }

        public static void Initialize()
        {
            // all done by constructor...    
        }

        /// <summary>
        /// The connected clients.
        /// </summary>
        public static PerformanceCounter ConnectedClientsTotal;

        /// <summary>
        /// The connected clients.
        /// </summary>
        public static ICounter ConnectedClients;

        /// <summary>
        /// Events that are flushed have a lower round trip time
        /// </summary>
        public static ICounter FlushEventRoundTripTime;

        /// <summary>
        /// The number of flushed events received per second.
        /// </summary>
        public static ICounter FlushEventsReceived;

        /// <summary>
        /// The number of flush invoking operations sent per second.
        /// </summary>
        public static ICounter FlushOperationsSent;

        /// <summary>
        /// The received operation response.
        /// </summary>
        public static ICounter ReceivedOperationResponse;

        /// <summary>
        /// The reliable event round trip time.
        /// </summary>
        public static ICounter ReliableEventRoundTripTime; 

        /// <summary>
        /// The received events.
        /// </summary>
        public static ICounter ReliableEventsReceived; 

        /// <summary>
        /// The send operations.
        /// </summary>
        public static ICounter ReliableOperationsSent; 

        /// <summary>
        /// The round trip time.
        /// </summary>
        public static ICounter RoundTripTime; 

        /// <summary>
        /// The round trip variance.
        /// </summary>
        public static ICounter RoundTripTimeVariance; 

        /// <summary>
        /// The unreliable event round trip time.
        /// </summary>
        public static ICounter UnreliableEventRoundTripTime; 
        
        /// <summary>
        /// The unreliable events received.
        /// </summary>
        public static ICounter UnreliableEventsReceived; 

        /// <summary>
        /// The unreliable operations sent.
        /// </summary>
        public static ICounter UnreliableOperationsSent;

        //     Base Counters: 
        public static ICounter FlusheventRoundtripTimeBase;

        public static ICounter ReliableEventRoundtripTimeBase;

        public static ICounter RoundtripTimeBase;

        public static ICounter RoundtripTimeVarianceBase;

        public static ICounter UnreliableEventRoundtripTimeBase; 
    }
}