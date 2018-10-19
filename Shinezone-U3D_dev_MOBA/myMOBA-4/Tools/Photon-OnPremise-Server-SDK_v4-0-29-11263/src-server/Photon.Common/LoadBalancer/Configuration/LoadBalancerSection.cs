// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadBalancerSection.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Configuration;

namespace Photon.Common.LoadBalancer.Configuration
{
    internal class LoadBalancerSection : ConfigurationSection
    {
        [ConfigurationProperty("LoadBalancerWeights", IsDefaultCollection = true, IsRequired = true)]
        public LoadBalancerWeightsCollection LoadBalancerWeights
        {
            get
            {
                return (LoadBalancerWeightsCollection)base["LoadBalancerWeights"];
            }
        }

        public void Deserialize(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            this.DeserializeElement(reader, serializeCollectionKey);
        }
    }
}
