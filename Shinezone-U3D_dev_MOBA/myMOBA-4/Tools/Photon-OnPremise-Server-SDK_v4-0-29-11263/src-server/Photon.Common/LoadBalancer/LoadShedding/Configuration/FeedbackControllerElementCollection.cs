// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedbackControllerElementCollection.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Configuration;

namespace Photon.Common.LoadBalancer.LoadShedding.Configuration
{
    internal class FeedbackControllerElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FeedbackControllerElement(); 
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FeedbackControllerElement)element).Name; 
        }
    }
}
