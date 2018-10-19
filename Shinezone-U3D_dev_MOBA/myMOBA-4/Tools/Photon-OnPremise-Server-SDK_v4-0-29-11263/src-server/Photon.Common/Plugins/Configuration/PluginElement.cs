// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginElement.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Configuration;

namespace Photon.Common.Plugins.Configuration
{
    public class PluginElement : ConfigurationElement
    {
        #region Constants and Fields

        private readonly Dictionary<string, string> customAttributeDict = new Dictionary<string, string>();

        #endregion

        #region Properties

        public Dictionary<string, string> CustomAttributes
        {
            get
            {
                return this.customAttributeDict;
            }
        }

        [ConfigurationProperty("Name", IsRequired = true, DefaultValue = "", IsKey = true)]
        public string Name
        {
            get
            {
                return (string)base["Name"];
            }

            set
            {
                base["Name"] = value;
            }
        }

        [ConfigurationProperty("Version", IsRequired = false, DefaultValue = "", IsKey = false)]
        public string Version
        {
            get
            {
                return (string)base["Version"];
            }

            set
            {
                base["Version"] = value;
            }
        }

        [ConfigurationProperty("Type", IsRequired = true, DefaultValue = "", IsKey = false)]
        public string Type
        {
            get
            {
                return (string)base["Type"];
            }

            set
            {
                base["Type"] = value;
            }
        }

        [ConfigurationProperty("AssemblyName", IsRequired = true, DefaultValue = "", IsKey = false)]
        public string AssemblyName
        {
            get
            {
                return (string)base["AssemblyName"]; 
            }
            set
            {
                base["AssemblyName"] = value; 
            }
        }

        #endregion

        #region Methods

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            this.customAttributeDict[name] = value;
            return true;
        }

        #endregion
    }
}