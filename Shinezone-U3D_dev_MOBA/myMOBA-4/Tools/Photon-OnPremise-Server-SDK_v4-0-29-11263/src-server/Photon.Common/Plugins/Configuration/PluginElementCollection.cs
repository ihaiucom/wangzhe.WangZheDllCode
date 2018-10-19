// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginElementCollection.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Configuration;

namespace Photon.Common.Plugins.Configuration
{
    public class PluginElementCollection : ConfigurationElementCollection
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
                return "Plugin";
            }
        }

        #endregion

        #region Indexers

        public PluginElement this[int index]
        {
            get
            {
                return (PluginElement)this.BaseGet(index);
            }
        }

        #endregion

        #region Methods

        protected override ConfigurationElement CreateNewElement()
        {
            return new PluginElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PluginElement)element).Name;
        }

        #endregion
    }
}