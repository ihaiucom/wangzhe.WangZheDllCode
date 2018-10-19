// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedbackLevelOrder.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FeedbackLevelOrder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Photon.Common.LoadBalancer.LoadShedding
{
    internal static class FeedbackLevelOrder
    {
        private static readonly Dictionary<FeedbackLevel, FeedbackLevel> ascending = new Dictionary<FeedbackLevel, FeedbackLevel> 
            {
                { FeedbackLevel.Lowest, FeedbackLevel.Low }, 
                { FeedbackLevel.Low, FeedbackLevel.Normal }, 
                { FeedbackLevel.Normal, FeedbackLevel.High }, 
                { FeedbackLevel.High, FeedbackLevel.Highest }, 
                { FeedbackLevel.Highest, FeedbackLevel.Highest },
            };

        private static readonly Dictionary<FeedbackLevel, FeedbackLevel> descending = new Dictionary<FeedbackLevel, FeedbackLevel> 
            {
                { FeedbackLevel.Lowest, FeedbackLevel.Lowest }, 
                { FeedbackLevel.Low, FeedbackLevel.Lowest }, 
                { FeedbackLevel.Normal, FeedbackLevel.Low }, 
                { FeedbackLevel.High, FeedbackLevel.Normal }, 
                { FeedbackLevel.Highest, FeedbackLevel.Normal },
            };

        public static FeedbackLevel GetNextHigher(FeedbackLevel level)
        {
            return ascending[level];
        }

        public static FeedbackLevel GetNextLower(FeedbackLevel level)
        {
            return descending[level];
        }
    }
}