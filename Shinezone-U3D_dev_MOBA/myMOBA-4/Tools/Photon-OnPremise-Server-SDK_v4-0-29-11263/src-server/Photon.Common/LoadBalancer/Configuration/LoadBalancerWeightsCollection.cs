// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadBalancerWeightsCollection.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Configuration;

namespace Photon.Common.LoadBalancer.Configuration
{
    internal class LoadBalancerWeightsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new LoadBalancerWeight(); 
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LoadBalancerWeight)element).Level; 
        }
    }
}
