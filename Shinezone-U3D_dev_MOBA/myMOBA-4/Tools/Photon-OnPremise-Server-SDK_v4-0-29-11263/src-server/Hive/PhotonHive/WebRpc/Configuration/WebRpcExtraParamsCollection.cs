// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RpcExtraParamsCollection.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Configuration;

namespace Photon.Hive.WebRpc.Configuration
{
    public class WebRpcExtraParamsCollection : KeyValueConfigurationCollection
    {
        #region Properties

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        protected override string ElementName
        {
            get
            {
                return "Param";
            }
        }

        #endregion

        #region Indexers

        public KeyValueConfigurationElement this[int index]
        {
            get
            {
                return (KeyValueConfigurationElement)BaseGet(index);
            }
        }

        #endregion

        #region Methods

        public Dictionary<string, string> AsDictionary()
        {
            var d = new Dictionary<string, string>();
            for (int i = 0; i < this.Count; ++i)
            {
                var element = this[i];
                d[element.Key] = element.Value;
            }

            return d;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new KeyValueConfigurationElement(string.Empty, string.Empty);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((KeyValueConfigurationElement)element).Key;
        }

        #endregion
    }
}