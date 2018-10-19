// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueHistory.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ValueHistory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Photon.Common.LoadBalancer.LoadShedding
{
    internal class ValueHistory : Queue<int>
    {
        private readonly int capacity;

        public ValueHistory(int capacity)
            : base(capacity)
        {
            this.capacity = capacity;
        }

        public void Add(int value)
        {
            if (this.Count == this.capacity)
            {
                this.Dequeue();
            }

            this.Enqueue(value);
        }
    }
}