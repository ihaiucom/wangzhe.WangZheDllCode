// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginSettings.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Configuration
{
    using System.Configuration;

    public class PluginSettings : ConfigurationSection
    {
        #region Constants and Fields

        private static readonly PluginSettings defaultInstance;

        #endregion

        #region Constructors and Destructors

        static PluginSettings()
        {
            defaultInstance = ConfigurationManager.GetSection("PluginSettings") as PluginSettings ?? new PluginSettings();
        }

        #endregion

        #region Properties

        public static PluginSettings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        [ConfigurationProperty("Enabled", IsRequired = false, DefaultValue = "False")]
        public bool Enabled
        {
            get
            {
                return (bool)base["Enabled"];
            }

            set
            {
                base["Enabled"] = value;
            }
        }

        [ConfigurationProperty("Plugins", IsRequired = false)]
        [ConfigurationCollection(typeof(PluginElement), CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public PluginElementCollection Plugins
        {
            get
            {
                return (PluginElementCollection)base["Plugins"];
            }
        }

        #endregion
    }
}