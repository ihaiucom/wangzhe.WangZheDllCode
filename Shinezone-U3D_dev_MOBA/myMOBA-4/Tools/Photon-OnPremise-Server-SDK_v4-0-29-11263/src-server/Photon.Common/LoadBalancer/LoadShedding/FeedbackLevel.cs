// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedbackLevel.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FeedbackLevel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Common.LoadBalancer.LoadShedding
{
    public enum FeedbackLevel
    {
        Highest = 4, 
        High = 3, 
        Normal = 2, 
        Low = 1, 
        Lowest = 0
    }
}