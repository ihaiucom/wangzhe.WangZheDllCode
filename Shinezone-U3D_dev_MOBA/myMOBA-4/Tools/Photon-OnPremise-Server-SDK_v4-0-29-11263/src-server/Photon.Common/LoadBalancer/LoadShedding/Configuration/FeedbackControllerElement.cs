// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedbackControllerElement.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Configuration;

namespace Photon.Common.LoadBalancer.LoadShedding.Configuration
{
    internal class FeedbackControllerElement : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired = true)]
        public FeedbackName Name
        {
            get { return (FeedbackName)this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("InitialInput", IsRequired = true)]
        public int InitialInput
        {
            get { return (int)this["InitialInput"]; }
            set { this["InitialInput"] = value; }
        }

        [ConfigurationProperty("InitialLevel", IsRequired = true)]
        public FeedbackLevel InitialLevel
        {
            get { return (FeedbackLevel)this["InitialLevel"]; }
            set { this["InitialLevel"] = value; }
        }

        [ConfigurationProperty("FeedbackLevels", IsDefaultCollection = false, IsRequired = true)]
        public FeedbackLevelElementCollection Levels
        {
            get
            {
                return (FeedbackLevelElementCollection)base["FeedbackLevels"];
            }
        }
    }
}
