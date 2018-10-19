// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RpcSettings.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Configuration;

namespace Photon.Hive.WebRpc.Configuration
{
    public class WebRpcSettings : ConfigurationSection
    {
        #region Constants and Fields

        private static readonly WebRpcSettings defaultInstance;

        #endregion

        #region Constructors and Destructors

        static WebRpcSettings()
        {
            defaultInstance = ConfigurationManager.GetSection("WebRpcSettings") as WebRpcSettings ?? new WebRpcSettings();
        }

        #endregion

        #region Properties

        public static WebRpcSettings Default
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

        [ConfigurationProperty("ReconnectInterval", IsRequired = false, DefaultValue = "60")]
        public int ReconnectInterval
        {
            get
            {
                return (int)base["ReconnectInterval"];
            }

            set
            {
                base["ReconnectInterval"] = value;
            }
        }

        [ConfigurationProperty("ExtraParams", IsRequired = false)]
        [ConfigurationCollection(typeof(KeyValueConfigurationElement), CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public WebRpcExtraParamsCollection ExtraParams
        {
            get
            {
                return (WebRpcExtraParamsCollection)base["ExtraParams"];
            }
        }

        [ConfigurationProperty("BaseUrl", IsRequired = true)]
        public WebRpcBaseUrlElement BaseUrl
        {
            get
            {
                return (WebRpcBaseUrlElement)base["BaseUrl"];
            }
        }

        #endregion
    }
}