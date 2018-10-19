// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RpcBaseUrlElement.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Configuration;

namespace Photon.Hive.WebRpc.Configuration
{
    public class WebRpcBaseUrlElement : ConfigurationElement
    {
        [ConfigurationProperty("Value", IsRequired = true, DefaultValue = "", IsKey = true)]
        public string Value
        {
            get
            {
                return (string)base["Value"];
            }

            set
            {
                base["Value"] = value;
            }
        }
    }
}