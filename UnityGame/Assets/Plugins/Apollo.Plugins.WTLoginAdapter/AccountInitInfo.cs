using System;

namespace Apollo.Plugins.WTLoginAdapter
{
	public class AccountInitInfo : ApolloBufferBase
	{
		public string AppId;

		public ulong Uin;

		public byte[] SKey;

		public byte[] SKeySignature;

		public string Password;

		public bool NeedLogin;

		public int Mode;

		public AccountInitInfo(string appId, ulong uin, byte[] skey, byte[] skeySignature)
		{
			this.AppId = appId;
			this.Uin = uin;
			this.SKey = skey;
			this.SKeySignature = skeySignature;
			this.NeedLogin = false;
		}

		public AccountInitInfo(string appid)
		{
			this.AppId = appid;
			this.NeedLogin = true;
			this.Mode = 2;
		}

		public AccountInitInfo(string appid, ulong uin, string password)
		{
			this.AppId = appid;
			this.Uin = uin;
			this.Password = password;
			this.Mode = 1;
			this.NeedLogin = true;
		}

		public override void WriteTo(ApolloBufferWriter writer)
		{
			writer.Write(this.AppId);
			writer.Write(this.Uin);
			writer.Write(this.SKey);
			writer.Write(this.SKeySignature);
			writer.Write(this.Uin.ToString());
			writer.Write(this.Password);
			writer.Write(this.NeedLogin);
			writer.Write(this.Mode);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			reader.Read(ref this.AppId);
			reader.Read(ref this.Uin);
			reader.Read(ref this.SKey);
			reader.Read(ref this.SKeySignature);
			string empty = string.Empty;
			reader.Read(ref empty);
			reader.Read(ref this.Password);
			reader.Read(ref this.NeedLogin);
			reader.Read(ref this.Mode);
		}
	}
}
