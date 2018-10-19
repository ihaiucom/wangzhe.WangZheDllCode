using System.Configuration;

namespace Photon.Common.Authentication.Configuration.Auth
{
    public class AuthProviderCollection : ConfigurationElementCollection
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
                return "AuthProvider";
            }
        }

        #endregion

        #region Indexers

        public AuthProvider this[int index]
        {
            get
            {
                return (AuthProvider)BaseGet(index);
            }
        }

        #endregion

        #region Methods

        protected override ConfigurationElement CreateNewElement()
        {
            return new AuthProvider();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AuthProvider)element).Name;
        }

        #endregion
    }
}
