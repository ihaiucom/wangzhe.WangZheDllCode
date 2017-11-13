using System;

namespace ExitGames.Client.Photon.Chat
{
	public class AuthenticationValues
	{
		private CustomAuthenticationType authType = CustomAuthenticationType.None;

		public CustomAuthenticationType AuthType
		{
			get
			{
				return this.authType;
			}
			set
			{
				this.authType = value;
			}
		}

		public string AuthGetParameters
		{
			get;
			set;
		}

		public object AuthPostData
		{
			get;
			private set;
		}

		public string Token
		{
			get;
			set;
		}

		public string UserId
		{
			get;
			set;
		}

		public AuthenticationValues()
		{
		}

		public AuthenticationValues(string userId)
		{
			this.UserId = userId;
		}

		public virtual void SetAuthPostData(string stringData)
		{
			this.AuthPostData = ((!string.IsNullOrEmpty(stringData)) ? stringData : null);
		}

		public virtual void SetAuthPostData(byte[] byteData)
		{
			this.AuthPostData = byteData;
		}

		public virtual void AddAuthParameter(string key, string value)
		{
			string text = (!string.IsNullOrEmpty(this.AuthGetParameters)) ? "&" : string.Empty;
			this.AuthGetParameters = string.Format("{0}{1}{2}={3}", new object[]
			{
				this.AuthGetParameters,
				text,
				Uri.EscapeDataString(key),
				Uri.EscapeDataString(value)
			});
		}

		public override string ToString()
		{
			return string.Format("AuthenticationValues UserId: {0}, GetParameters: {1} Token available: {2}", this.UserId, this.AuthGetParameters, this.Token != null);
		}
	}
}
