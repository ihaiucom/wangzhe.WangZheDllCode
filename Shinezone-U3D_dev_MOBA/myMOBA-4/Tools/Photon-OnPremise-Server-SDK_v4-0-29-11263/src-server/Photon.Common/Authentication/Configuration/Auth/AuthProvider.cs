using System.Collections.Generic;
using System.Configuration;
using Photon.Common.Authentication.Data;

namespace Photon.Common.Authentication.Configuration.Auth
{
    public class AuthProvider : ConfigurationElement
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

        [ConfigurationProperty("AuthUrl", IsRequired = true, DefaultValue = "", IsKey = false)]
        public string AuthUrl
        {
            get
            {
                return (string)base["AuthUrl"];
            }

            set
            {
                base["AuthUrl"] = value;
            }
        }

        [ConfigurationProperty("AuthenticationType", IsRequired = true, DefaultValue = "0", IsKey = false)]
        public int AuthenticationType
        {
            get
            {
                return (int)base["AuthenticationType"];
            }

            set
            {
                base["AuthenticationType"] = value;
            }
        }

        [ConfigurationProperty("RejectIfUnavailable", IsRequired = false, DefaultValue = "true", IsKey = false)]
        public bool RejectIfUnavailable
        {
            get
            {
                return (bool)base["RejectIfUnavailable"];
            }

            set
            {
                base["RejectIfUnavailable"] = value;
            }
        }

        [ConfigurationProperty("Secret", IsRequired = false, DefaultValue = "", IsKey = false)]
        public string Secret
        {
            get
            {
                return (string)base["Secret"];
            }

            set
            {
                base["Secret"] = value;
            }
        }

        [ConfigurationProperty("AppId", IsRequired = false, DefaultValue = "", IsKey = false)]
        public string AppId
        {
            get
            {
                return (string)base["AppId"];
            }

            set
            {
                base["AppId"] = value;
            }
        }

        public bool IsFacebook
        {
            get
            {
                return this.AuthenticationType == 2;
            }
        }

        public string NameValuePairAsQueryString { get; private set; }


        #endregion

        public override string ToString()
        {
            return string.Format("[Name={0},type={4},url={1},RejectIfUnavailable={2},keys={3}]",
                this.Name,
                this.AuthUrl,
                this.RejectIfUnavailable,
                this.NameValuePairAsQueryString,
                (ClientAuthenticationType)this.AuthenticationType);
        }

        #region Methods

        protected override void PostDeserialize()
        {
            base.PostDeserialize();
            if (this.IsFacebook)
            {
                this.NameValuePairAsQueryString = string.Format("secret={0}&appid={1}", this.Secret, this.AppId);
            }
            else
            {
                this.NameValuePairAsQueryString = GetNameValuePairsAsQueryString(this.customAttributeDict);
                if (!string.IsNullOrEmpty(this.Secret))
                {
                    this.NameValuePairAsQueryString += string.Format("secret={0}", this.Secret);
                }

                if (!string.IsNullOrEmpty(this.AppId))
                {
                    this.NameValuePairAsQueryString += string.Format("appid={0}", this.AppId);
                }
            }
        }

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            this.customAttributeDict[name] = value;
            return true;
        }

        private static string GetNameValuePairsAsQueryString(Dictionary<string, string> nameValuePairs)
        {
            if (nameValuePairs == null || nameValuePairs.Count == 0) return null;

            var httpValueCollection = System.Web.HttpUtility.ParseQueryString(string.Empty);
            foreach (var entry in nameValuePairs)
            {
                httpValueCollection.Add(entry.Key, entry.Value);
            }
            return httpValueCollection.ToString();
        }


        #endregion
    }
}
