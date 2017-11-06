using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExitGames.Client.Photon.Chat
{
	public class ChatPeer : PhotonPeer
	{
		public const string NameServerHost = "ns.exitgames.com";

		public const string NameServerHttp = "http://ns.exitgamescloud.com:80/photon/n";

		private static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort;

		public string NameServerAddress
		{
			get
			{
				return this.GetNameServerAddress();
			}
		}

		internal virtual bool IsProtocolSecure
		{
			get
			{
				return base.get_UsedProtocol() == 5;
			}
		}

		public ChatPeer(IPhotonPeerListener listener, ConnectionProtocol protocol) : base(listener, protocol)
		{
			this.ConfigUnitySockets();
		}

		static ChatPeer()
		{
			// Note: this type is marked as 'beforefieldinit'.
			Dictionary<ConnectionProtocol, int> dictionary = new Dictionary<ConnectionProtocol, int>();
			dictionary.Add(0, 5058);
			dictionary.Add(1, 4533);
			dictionary.Add(4, 9093);
			dictionary.Add(5, 19093);
			ChatPeer.ProtocolToNameServerPort = dictionary;
		}

		[Conditional("UNITY")]
		private void ConfigUnitySockets()
		{
			Type type = Type.GetType("ExitGames.Client.Photon.SocketWebTcp, Assembly-CSharp", false);
			if (type == null)
			{
				type = Type.GetType("ExitGames.Client.Photon.SocketWebTcp, Assembly-CSharp-firstpass", false);
			}
			if (type != null)
			{
				this.SocketImplementationConfig.set_Item(4, type);
				this.SocketImplementationConfig.set_Item(5, type);
			}
		}

		private string GetNameServerAddress()
		{
			int num = 0;
			ChatPeer.ProtocolToNameServerPort.TryGetValue(base.get_TransportProtocol(), ref num);
			switch (base.get_TransportProtocol())
			{
			case 0:
			case 1:
				return string.Format("{0}:{1}", "ns.exitgames.com", num);
			case 4:
				return string.Format("ws://{0}:{1}", "ns.exitgames.com", num);
			case 5:
				return string.Format("wss://{0}:{1}", "ns.exitgames.com", num);
			}
			throw new ArgumentOutOfRangeException();
		}

		public bool Connect()
		{
			if (this.DebugOut >= 3)
			{
				base.get_Listener().DebugReturn(3, "Connecting to nameserver " + this.NameServerAddress);
			}
			return this.Connect(this.NameServerAddress, "NameServer");
		}

		public bool AuthenticateOnNameServer(string appId, string appVersion, string region, AuthenticationValues authValues)
		{
			if (this.DebugOut >= 3)
			{
				base.get_Listener().DebugReturn(3, "OpAuthenticate()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.set_Item(220, appVersion);
			dictionary.set_Item(224, appId);
			dictionary.set_Item(210, region);
			if (authValues != null)
			{
				if (!string.IsNullOrEmpty(authValues.UserId))
				{
					dictionary.set_Item(225, authValues.UserId);
				}
				if (authValues != null && authValues.AuthType != CustomAuthenticationType.None)
				{
					dictionary.set_Item(217, (byte)authValues.AuthType);
					if (!string.IsNullOrEmpty(authValues.Token))
					{
						dictionary.set_Item(221, authValues.Token);
					}
					else
					{
						if (!string.IsNullOrEmpty(authValues.AuthGetParameters))
						{
							dictionary.set_Item(216, authValues.AuthGetParameters);
						}
						if (authValues.AuthPostData != null)
						{
							dictionary.set_Item(214, authValues.AuthPostData);
						}
					}
				}
			}
			return this.OpCustom(230, dictionary, true, 0, base.get_IsEncryptionAvailable());
		}
	}
}
