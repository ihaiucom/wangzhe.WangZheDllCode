// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadBalancerWeightElement.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Configuration;
using Photon.Common.LoadBalancer.LoadShedding;

namespace Photon.Common.LoadBalancer.Configuration
{
    internal class LoadBalancerWeight : ConfigurationElement
    {
        [ConfigurationProperty("Level", IsRequired = true)]
        public FeedbackLevel Level
        {
            get { return (FeedbackLevel)this["Level"]; }
            set { this["Level"] = value; }
        }


        [ConfigurationProperty("Value", IsRequired = true)]
        public int Value
        {
            get { return (int)this["Value"]; }
            set { this["Value"] = value; }
        }
    }
}
