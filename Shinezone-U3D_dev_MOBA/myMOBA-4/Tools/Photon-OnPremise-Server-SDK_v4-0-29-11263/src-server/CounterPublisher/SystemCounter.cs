// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCounter.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The system counter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;

namespace Photon.CounterPublisher
{
    using System;

    using ExitGames.Diagnostics.Counter;
    using ExitGames.Diagnostics.Monitoring;

    using Photon.SocketServer.Diagnostics;

    /// <summary>
    /// The system counter.
    /// </summary>
    public class SystemCounter
    {
        /// <summary>
        /// The cpu.
        /// </summary>
        [PublishCounter(Name = "Cpu")]
        public static readonly CpuUsageCounterReader Cpu = new CpuUsageCounterReader();

        /// <summary>
        /// The cpu total.
        /// </summary>
        [PublishCounter(Name = "CpuTotal")]
        public static readonly PerformanceCounterReader CpuTotal = new PerformanceCounterReader("Processor", "% Processor Time", "_Total");

        /// <summary>
        /// The memory.
        /// </summary>
        [PublishCounter(Name = "Memory")]
        public static readonly PerformanceCounterReader Memory = new PerformanceCounterReader("Memory", "Available MBytes");

        [PublishCounter(Name = "BytesTotalPerSecond")]
        public static readonly NetworkInterfaceCounter BytesTotalPerSecond = new NetworkInterfaceCounter("BytesTotalPerSecond", "Bytes Total/sec");

        [PublishCounter(Name = "BytesSentPerSecond")]
        public static readonly NetworkInterfaceCounter BytesSentPerSecond = new NetworkInterfaceCounter("BytesSentPerSecond", "Bytes Sent/sec");

        [PublishCounter(Name = "BytesReceivedPerSecond")]
        public static readonly NetworkInterfaceCounter BytesReceivedPerSecond = new NetworkInterfaceCounter("BytesReceivedPerSecond", "Bytes Received/sec");

        /// <summary>
        /// Helper method to retrieve all instances for a certain performance counter category. 
        /// </summary>
        /// <param name="categoryName"></param>
        private static string[] GetInstanceNames(string categoryName)
        {
            foreach (var category in PerformanceCounterCategory.GetCategories())
            {
                if (category.CategoryName == categoryName)
                {
                    if (category.CategoryType == PerformanceCounterCategoryType.SingleInstance)
                    {
                        return new[] {string.Empty}; 
                    }
                    
                    return category.GetInstanceNames();
                }
            }

            return new[] { string.Empty }; 
        }

        public class NetworkInterfaceCounter : ICounter
        {
            private readonly List<PerformanceCounterReader> counterList = new List<PerformanceCounterReader>();

            public NetworkInterfaceCounter(string name, string counter)
            {
                this.Name = name;
                this.CounterType = CounterType.CountPerSecound;

                var instanceNameList = GetInstanceNames("Network Interface");
                for (int i = 0; i < instanceNameList.Length; i++)
                {
                    string instanceName = instanceNameList[i];

                    // don't include loopback interfaces and isatap interfaces for now 
                    if (instanceName.Contains("Loopback") || instanceName.StartsWith("isatap"))
                    {
                        continue;
                    }

                    var counterReader = new PerformanceCounterReader("Network Interface", counter, instanceName);
                    this.counterList.Add(counterReader);
                }
            }

            public string Name { get; private set; }

            public CounterType CounterType { get; private set; }

            public long Decrement()
            {
                throw new NotImplementedException();
            }

            public float GetNextValue()
            {
                float result = 0;
                for (int i = 0; i < this.counterList.Count; i++)
                {
                    result += this.counterList[i].GetNextValue();
                }

                return result;
            }

            public long Increment()
            {
                throw new NotImplementedException();
            }

            public long IncrementBy(long value)
            {
                throw new NotImplementedException();
            }
        }
    }
}