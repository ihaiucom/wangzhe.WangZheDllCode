// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginSettings.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Configuration;

namespace Photon.Common.Plugins.Configuration
{
    public class PluginSettings : ConfigurationSection
    {
        #region Constructors and Destructors
        /// <summary>
        /// internal constructor to prevent from deriving from this class. Use GenericPluginSettings instead
        /// </summary>
        internal PluginSettings()
        { }
        #endregion

        #region Properties

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

    /// <summary>
    /// Class helper, which helps automate plugin settings initialization
    /// </summary>
    /// <typeparam name="Derived"></typeparam>
    public class GenericPluginSettings<Derived> : PluginSettings where Derived : PluginSettings, new()
    {
        #region Constants and Fields

        protected static PluginSettings defaultInstance = ConfigurationManager.GetSection(typeof (Derived).Name) as Derived ?? new Derived();

        #endregion

        #region Properties

        public static PluginSettings Default
        {
            get { return defaultInstance; }
        }

        #endregion
    }
}