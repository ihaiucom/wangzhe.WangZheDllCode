using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExitGames.Logging;
using Photon.SocketServer.Diagnostics;

namespace Photon.Common.Authentication.Diagnostic
{
    public class CustomAuthResultCounters : PerfCounterManagerBase
    {
        private const string CategoryName = "Photon: Custom Authentication Results";

        private const string CustomAuthResult0Name = "Custom Auth 'Data' Results";
        private const string CustomAuthResult0PerSecondName = "Custom Auth 0 Results/sec";

        private const string CustomAuthResult1Name = "Custom Auth 'Success' Results";
        private const string CustomAuthResult1PerSecondName = "Custom Auth 1 Results/sec";

        private const string CustomAuthResultOtherName = "Custom Auth 'Failed' Results";
        private const string CustomAuthResultOtherPerSecondName = "Custom Auth 'Other' Results/sec";

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public static PerformanceCounterCategory PerformanceCounterCategory;

        private static readonly object SyncRoot = new object();

        private static readonly Dictionary<string, Instance> Instances = new Dictionary<string, Instance>(); 

        private static readonly NullInstance nullInstance = new NullInstance();

        private static CounterCreationDataCollection GetCounterCreationData()
        {
            return new CounterCreationDataCollection
            {
                new CounterCreationData(CustomAuthResult0Name, string.Empty, PerformanceCounterType.NumberOfItems32),
                new CounterCreationData(CustomAuthResult0PerSecondName, string.Empty, PerformanceCounterType.RateOfCountsPerSecond32),

                new CounterCreationData(CustomAuthResult1Name, string.Empty, PerformanceCounterType.NumberOfItems32),
                new CounterCreationData(CustomAuthResult1PerSecondName, string.Empty, PerformanceCounterType.RateOfCountsPerSecond32),

                new CounterCreationData(CustomAuthResultOtherName, string.Empty, PerformanceCounterType.NumberOfItems32),
                new CounterCreationData(CustomAuthResultOtherPerSecondName, string.Empty, PerformanceCounterType.RateOfCountsPerSecond32)
            };
        }

        public static void Initialize()
        {
            if (Log.IsInfoEnabled)
            {
                Log.InfoFormat("Start to initialize {0} performance counters", CategoryName);
            }

            if (!IsUserAdmin())
            {
                Log.Info("User has no Admin rights. CustomAuthResultCounters initialization skipped");
                return;
            }

            try
            {
                PerformanceCounterCategory = GetOrCreateCategory(CategoryName, GetCounterCreationData());
            }
            catch (Exception e)
            {
                Log.WarnFormat("Exception happened during counters initialization. Excpetion {0}", e);
                return;
            }

            if (Log.IsInfoEnabled)
            {
                Log.InfoFormat("{0} performance counters successfully initialized", CategoryName);
            }
        }

        public static Instance GetInstance(string instanceName)
        {
            lock (SyncRoot)
            {
                if (PerformanceCounterCategory == null)
                {
                    return nullInstance;
                }

            }
            lock (Instances)
            {
                Instance result;
                if (Instances.TryGetValue(instanceName, out result))
                {
                    return result;
                }
                result = new Instance(instanceName);
                Instances.Add(instanceName, result);
                return result;
            }
        }

        public class Instance
        {
            public readonly PerformanceCounter CustomAuthResultData;
            public readonly PerformanceCounter CustomAuthResultDataPerSecond;
            public readonly PerformanceCounter CustomAuthResultSuccess;
            public readonly PerformanceCounter CustomAuthResultSuccessPerSecond;
            public readonly PerformanceCounter CustomAuthResultFailures;
            public readonly PerformanceCounter CustomAuthResultFailuresPerSecond;

            private static PerformanceCounter CreateCounter(string name, string instanceName)
            {
                return new PerformanceCounter
                {
                    CategoryName = CategoryName,
                    CounterName = name,
                    InstanceName = instanceName,
                    ReadOnly = false,
                    InstanceLifetime = PerformanceCounterInstanceLifetime.Process
                };
            }

            protected Instance()
            {
            }

            public Instance(string instanceName)
            {
                this.CustomAuthResultData = CreateCounter(CustomAuthResult0Name, instanceName);
                this.CustomAuthResultDataPerSecond = CreateCounter(CustomAuthResult0PerSecondName, instanceName);

                this.CustomAuthResultSuccess = CreateCounter(CustomAuthResult1Name, instanceName);
                this.CustomAuthResultSuccessPerSecond = CreateCounter(CustomAuthResult1PerSecondName, instanceName);

                this.CustomAuthResultFailures = CreateCounter(CustomAuthResultOtherName, instanceName);
                this.CustomAuthResultFailuresPerSecond = CreateCounter(CustomAuthResultOtherPerSecondName, instanceName);
            }

            virtual public void IncrementCustomAuthResultData()
            {
                this.CustomAuthResultData.Increment();
                this.CustomAuthResultDataPerSecond.Increment();
            }

            virtual public void IncrementCustomAuthResultSuccess()
            {
                this.CustomAuthResultSuccess.Increment();
                this.CustomAuthResultSuccessPerSecond.Increment();
            }

            virtual public void IncrementCustomAuthResultFailed()
            {
                this.CustomAuthResultFailures.Increment();
                this.CustomAuthResultFailuresPerSecond.Increment();
            }
        }

        public class NullInstance : Instance
        {
            public override void IncrementCustomAuthResultData()
            {
            }

            public override void IncrementCustomAuthResultFailed()
            {
            }

            public override void IncrementCustomAuthResultSuccess()
            {
            }
        }
    }
}
