using System.Configuration;

namespace Photon.Common.Authentication.Configuration.Auth
{
    public class AuthSettings : ConfigurationSection
    {
        #region Constants and Fields

        private static readonly AuthSettings defaultInstance;

        #endregion

        #region Constructors and Destructors

        static AuthSettings()
        {
            defaultInstance = ConfigurationManager.GetSection("AuthSettings") as AuthSettings ?? new AuthSettings();
        }

        #endregion

        #region Properties

        public static AuthSettings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        [ConfigurationProperty("Enabled", IsRequired = false, DefaultValue = false)]
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

        [ConfigurationProperty("ClientAuthenticationAllowAnonymous", IsRequired = false, DefaultValue = false)]
        public bool ClientAuthenticationAllowAnonymous
        {
            get
            {
                return (bool)base["ClientAuthenticationAllowAnonymous"];
            }

            set
            {
                base["ClientAuthenticationAllowAnonymous"] = value;
            }
        }


        [ConfigurationProperty("AuthProviders", IsRequired = false)]
        [ConfigurationCollection(typeof(AuthProvider), CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public AuthProviderCollection AuthProviders
        {
            get
            {
                return (AuthProviderCollection)base["AuthProviders"];
            }
        }

        #endregion
    }
}
