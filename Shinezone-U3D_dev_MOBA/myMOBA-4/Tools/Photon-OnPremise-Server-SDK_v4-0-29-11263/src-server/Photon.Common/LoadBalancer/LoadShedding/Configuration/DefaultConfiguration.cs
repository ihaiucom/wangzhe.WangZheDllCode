// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultConfiguration.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Common.LoadBalancer.LoadShedding.Configuration
{
    using System.Collections.Generic;

    internal class DefaultConfiguration
    {
        internal static List<FeedbackController> GetDefaultControllers()
        {
            var cpuController = new FeedbackController(
            FeedbackName.CpuUsage,
            new Dictionary<FeedbackLevel, int>
                    {
                        { FeedbackLevel.Lowest, 20 },
                        { FeedbackLevel.Low, 35 },
                        { FeedbackLevel.Normal, 50 },
                        { FeedbackLevel.High, 70 },
                        { FeedbackLevel.Highest, 90 }
                    },
            0,
            FeedbackLevel.Lowest);
      
        const int megaByte = 1024 * 1024;
        var thresholdValues = new Dictionary<FeedbackLevel, int> 
                {
                    { FeedbackLevel.Lowest, megaByte }, 
                    { FeedbackLevel.Normal, 4 * megaByte }, 
                    { FeedbackLevel.High, 8 * megaByte }, 
                    { FeedbackLevel.Highest, 10 * megaByte }
                };
        var bandwidthController = new FeedbackController(FeedbackName.Bandwidth, thresholdValues, 0, FeedbackLevel.Lowest);

            return new List<FeedbackController> { cpuController, bandwidthController }; 
        }
    }
}
