using Apollo;
using System;

namespace Assets.Scripts.Framework
{
	public class ConnectorParam
	{
		public bool bIsUDP;

		public string url = string.Empty;

		public string ip = string.Empty;

		public ushort vPort = 6629;

		public string httpDns = string.Empty;

		public ApolloEncryptMethod enc = ApolloEncryptMethod.Aes;

		public ApolloKeyMaking keyMaking = ApolloKeyMaking.Server;

		public static string DH = "C0FC17D2ADC0007C512E9B6187823F559595D953C82D3D4F281D5198E86C79DF14FAB1F2A901F909FECB71B147DBD265837A254B204D1B5BC5FD64BF804DCD03";

		public void SetVPort(ushort nPort)
		{
			this.vPort = nPort;
			if (this.bIsUDP)
			{
				this.url = string.Format("lwip://{0}:{1}", this.ip, this.vPort);
			}
			else
			{
				this.url = string.Format("tcp://{0}:{1}", this.ip, this.vPort);
			}
		}

		public void SetVip(string Vip)
		{
			this.ip = Vip;
			if (this.bIsUDP)
			{
				this.url = string.Format("lwip://{0}:{1}", this.ip, this.vPort);
			}
			else
			{
				this.url = string.Format("tcp://{0}:{1}", this.ip, this.vPort);
			}
		}

		public void DealWithHttpDNS()
		{
			this.httpDns = HttpDnsPolicy.GetHostByName(this.ip);
			if (this.bIsUDP)
			{
				this.url = string.Format("lwip://{0}:{1}", this.httpDns, this.vPort);
			}
			else
			{
				this.url = string.Format("tcp://{0}:{1}", this.httpDns, this.vPort);
			}
		}
	}
}
