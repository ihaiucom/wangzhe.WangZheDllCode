// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILatencyMonitor.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ILatencyMonitor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.LoadShedding
{
    public interface ILatencyMonitor
    {
        int AverageLatencyMs { get; }
    }
}