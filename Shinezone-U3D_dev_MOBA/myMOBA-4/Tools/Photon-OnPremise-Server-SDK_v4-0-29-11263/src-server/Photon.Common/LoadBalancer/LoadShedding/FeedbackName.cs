// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedbackName.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FeedbackName type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Common.LoadBalancer.LoadShedding
{
    internal enum FeedbackName
    {
        CpuUsage, 
        PeerCount, 
        Bandwidth,
        OutOfRotation
    }
}