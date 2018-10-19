// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AverageCounterReader.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AverageCounterReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using ExitGames.Diagnostics.Counter;

namespace Photon.Common.LoadBalancer.LoadShedding
{
    public sealed class AverageCounterReader : PerformanceCounterReader
    {
        private readonly ValueHistory values;

        public AverageCounterReader(int capacity, string categoryName, string counterName)
            : base(categoryName, counterName)
        {
            this.values = new ValueHistory(capacity);
        }

        public AverageCounterReader(int capacity, string categoryName, string counterName, string instanceName)
            : base(categoryName, counterName, instanceName)
        {
            this.values = new ValueHistory(capacity);
        }

        public double GetNextAverage()
        {
            float value = this.GetNextValue();
            this.values.Add((int)value);
            return this.values.Average();
        }
    }
}