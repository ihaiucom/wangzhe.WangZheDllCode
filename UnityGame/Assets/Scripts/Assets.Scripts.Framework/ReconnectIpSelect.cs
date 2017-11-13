using CSProtocol;
using System;
using System.Net;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	public class ReconnectIpSelect : Singleton<ReconnectIpSelect>
	{
		private string m_successLobbyUrlForTongcai;

		private string m_successLobbyUrlForNormal;

		private string m_successRelayUrlForTongcai;

		private string m_successRelayUrlForNormal;

		private string m_curLobbyUrlForTongcai;

		private string m_curLobbyUrlForNormal;

		private string m_curRelayUrlForTongcai;

		private string m_curRelayUrlForNormal;

		private COMDT_TGWINFO m_tgwInfo;

		private ListView<string> tempIpList = new ListView<string>();

		private NetworkReachability lobbylastSucNetworkReachability;

		private NetworkReachability relaylastSucNetworkReachability;

		public void Reset()
		{
			this.m_successLobbyUrlForTongcai = null;
			this.m_successLobbyUrlForNormal = null;
			this.m_successRelayUrlForTongcai = null;
			this.m_successRelayUrlForNormal = null;
			this.lobbylastSucNetworkReachability = NetworkReachability.NotReachable;
			this.relaylastSucNetworkReachability = NetworkReachability.NotReachable;
		}

		public void SetRelayTgw(COMDT_TGWINFO tgw)
		{
			this.m_tgwInfo = tgw;
			this.m_successRelayUrlForNormal = null;
		}

		public void SetLobbySuccessUrl(string url)
		{
			this.lobbylastSucNetworkReachability = Application.internetReachability;
			if (MonoSingleton<CTongCaiSys>.instance.IsCanUseTongCai())
			{
				if (this.m_successLobbyUrlForTongcai == null)
				{
					this.m_successLobbyUrlForTongcai = url;
				}
			}
			else if (this.m_successLobbyUrlForNormal == null)
			{
				this.m_successLobbyUrlForNormal = url;
			}
			this.m_curLobbyUrlForTongcai = null;
			this.m_curLobbyUrlForNormal = null;
		}

		public void SetRelaySuccessUrl(string url)
		{
			this.relaylastSucNetworkReachability = Application.internetReachability;
			if (MonoSingleton<CTongCaiSys>.instance.IsCanUseTongCai())
			{
				if (this.m_successRelayUrlForTongcai == null)
				{
					this.m_successRelayUrlForTongcai = url;
				}
			}
			else if (this.m_successRelayUrlForNormal == null)
			{
				this.m_successRelayUrlForNormal = url;
			}
			this.m_curRelayUrlForTongcai = null;
			this.m_curRelayUrlForNormal = null;
		}

		public string GetConnectUrl(ConnectorType connectType, uint curConnectTime)
		{
			if (MonoSingleton<CTongCaiSys>.instance.IsCanUseTongCai())
			{
				if (connectType == ConnectorType.Lobby)
				{
					if (!string.IsNullOrEmpty(this.m_successLobbyUrlForTongcai) && this.lobbylastSucNetworkReachability == Application.internetReachability)
					{
						return this.m_successLobbyUrlForTongcai;
					}
					if (curConnectTime == 1u)
					{
						MonoSingleton<TdirMgr>.GetInstance().TdirAsyncISP();
					}
					this.m_curLobbyUrlForTongcai = this.GetLobbyTongcaiConnectUrl(curConnectTime);
					return this.m_curLobbyUrlForTongcai;
				}
				else
				{
					if (!string.IsNullOrEmpty(this.m_successRelayUrlForTongcai) && this.relaylastSucNetworkReachability == Application.internetReachability)
					{
						return this.m_successRelayUrlForTongcai;
					}
					if (curConnectTime == 1u)
					{
						MonoSingleton<TdirMgr>.GetInstance().TdirAsyncISP();
					}
					this.m_curRelayUrlForTongcai = this.GetRelayTongCaiConnectUrl(curConnectTime);
					return this.m_curRelayUrlForTongcai;
				}
			}
			else if (connectType == ConnectorType.Lobby)
			{
				if (!string.IsNullOrEmpty(this.m_successLobbyUrlForNormal) && this.lobbylastSucNetworkReachability == Application.internetReachability)
				{
					return this.m_successLobbyUrlForNormal;
				}
				if (curConnectTime == 1u)
				{
					MonoSingleton<TdirMgr>.GetInstance().TdirAsyncISP();
				}
				this.m_curLobbyUrlForNormal = this.GetLobbyNormalConnectUrl(curConnectTime);
				return this.m_curLobbyUrlForNormal;
			}
			else
			{
				if (!string.IsNullOrEmpty(this.m_successRelayUrlForNormal) && this.relaylastSucNetworkReachability == Application.internetReachability)
				{
					return this.m_successRelayUrlForNormal;
				}
				if (curConnectTime == 1u)
				{
					MonoSingleton<TdirMgr>.GetInstance().TdirAsyncISP();
				}
				this.m_curRelayUrlForNormal = this.GetRelayNormalConnectUrl(curConnectTime);
				return this.m_curRelayUrlForNormal;
			}
		}

		private string GetLobbyTongcaiConnectUrl(uint curConnectTime)
		{
			string text = null;
			if (MonoSingleton<TdirMgr>.instance.GetISP() <= IspType.Null)
			{
				if (curConnectTime == 1u || curConnectTime == 2u)
				{
					text = MonoSingleton<CTongCaiSys>.instance.TongcaiUrl;
				}
				else if (curConnectTime == 3u || curConnectTime == 4u)
				{
					text = MonoSingleton<CTongCaiSys>.instance.TongcaiIps[2];
				}
				else if (curConnectTime == 5u || curConnectTime == 6u)
				{
					text = MonoSingleton<CTongCaiSys>.instance.TongcaiIps[3];
				}
				else if (curConnectTime == 7u || curConnectTime == 8u)
				{
					text = MonoSingleton<CTongCaiSys>.instance.TongcaiIps[1];
				}
			}
			else if (curConnectTime <= 2u)
			{
				text = MonoSingleton<CTongCaiSys>.instance.TongcaiUrl;
			}
			else if (curConnectTime <= 5u)
			{
				text = MonoSingleton<CTongCaiSys>.instance.TongcaiIps[(int)MonoSingleton<TdirMgr>.instance.GetISP()];
			}
			else if (curConnectTime == 6u)
			{
				text = MonoSingleton<CTongCaiSys>.instance.TongcaiIps[2];
			}
			else if (curConnectTime == 7u)
			{
				text = MonoSingleton<CTongCaiSys>.instance.TongcaiIps[3];
			}
			else if (curConnectTime == 8u)
			{
				text = MonoSingleton<CTongCaiSys>.instance.TongcaiIps[1];
			}
			if (string.IsNullOrEmpty(text))
			{
				text = MonoSingleton<CTongCaiSys>.instance.TongcaiUrl;
			}
			return text;
		}

		private string GetLobbyNormalConnectUrl(uint curConnectTime)
		{
			string text = null;
			int connectIndex = MonoSingleton<TdirMgr>.instance.m_connectIndex;
			if (MonoSingleton<TdirMgr>.instance.GetISP() <= IspType.Null)
			{
				if (curConnectTime == 1u || curConnectTime == 2u)
				{
					text = MonoSingleton<TdirMgr>.instance.SelectedTdir.addrs.get_Item(connectIndex).ip;
				}
				else if (curConnectTime == 3u || curConnectTime == 4u)
				{
					text = MonoSingleton<TdirMgr>.instance.GetLianTongIP();
				}
				else if (curConnectTime == 5u || curConnectTime == 6u)
				{
					text = MonoSingleton<TdirMgr>.instance.GetYiDongIP();
				}
				else if (curConnectTime == 7u || curConnectTime == 8u)
				{
					text = MonoSingleton<TdirMgr>.instance.GetDianXingIP();
				}
			}
			else if (curConnectTime <= 2u)
			{
				text = MonoSingleton<TdirMgr>.instance.SelectedTdir.addrs.get_Item(connectIndex).ip;
			}
			else if (curConnectTime <= 5u)
			{
				text = MonoSingleton<TdirMgr>.instance.GetIpByIspCode((int)MonoSingleton<TdirMgr>.instance.GetISP());
			}
			else if (curConnectTime == 6u)
			{
				text = MonoSingleton<TdirMgr>.instance.GetLianTongIP();
			}
			else if (curConnectTime == 7u)
			{
				text = MonoSingleton<TdirMgr>.instance.GetYiDongIP();
			}
			else if (curConnectTime == 8u)
			{
				text = MonoSingleton<TdirMgr>.instance.GetDianXingIP();
			}
			if (string.IsNullOrEmpty(text))
			{
				text = MonoSingleton<TdirMgr>.instance.SelectedTdir.addrs.get_Item(connectIndex).ip;
			}
			return text;
		}

		private string GetRelayTongCaiConnectUrl(uint curConnectTime)
		{
			return this.GetLobbyTongcaiConnectUrl(curConnectTime);
		}

		private string GetRelayNormalConnectUrl(uint curConnectTime)
		{
			string text = null;
			if (MonoSingleton<TdirMgr>.instance.GetISP() <= IspType.Null)
			{
				if (curConnectTime == 1u || curConnectTime == 2u)
				{
					if (this.m_tgwInfo.szRelayUrl.Length > 0 && this.m_tgwInfo.szRelayUrl[0] != 0)
					{
						text = StringHelper.UTF8BytesToString(ref this.m_tgwInfo.szRelayUrl);
					}
					else
					{
						text = ApolloConfig.loginOnlyIpOrUrl;
					}
				}
				else if (curConnectTime == 3u || curConnectTime == 4u)
				{
					text = this.GetIPFromTgw(this.m_tgwInfo, IspType.Liantong);
				}
				else if (curConnectTime == 5u || curConnectTime == 6u)
				{
					text = this.GetIPFromTgw(this.m_tgwInfo, IspType.Yidong);
				}
				else if (curConnectTime == 7u || curConnectTime == 8u)
				{
					text = this.GetIPFromTgw(this.m_tgwInfo, IspType.Dianxing);
				}
			}
			else if (curConnectTime <= 2u)
			{
				if (this.m_tgwInfo.szRelayUrl.Length > 0 && this.m_tgwInfo.szRelayUrl[0] != 0)
				{
					text = StringHelper.UTF8BytesToString(ref this.m_tgwInfo.szRelayUrl);
				}
				else
				{
					text = ApolloConfig.loginOnlyIpOrUrl;
				}
			}
			else if (curConnectTime <= 5u)
			{
				text = this.GetIPFromTgw(this.m_tgwInfo, MonoSingleton<TdirMgr>.instance.GetISP());
			}
			else if (curConnectTime == 6u)
			{
				text = this.GetIPFromTgw(this.m_tgwInfo, IspType.Liantong);
			}
			else if (curConnectTime == 7u)
			{
				text = this.GetIPFromTgw(this.m_tgwInfo, IspType.Yidong);
			}
			else if (curConnectTime == 8u)
			{
				text = this.GetIPFromTgw(this.m_tgwInfo, IspType.Dianxing);
			}
			if (string.IsNullOrEmpty(text))
			{
				if (this.m_tgwInfo.szRelayUrl.Length > 0 && this.m_tgwInfo.szRelayUrl[0] != 0)
				{
					text = StringHelper.UTF8BytesToString(ref this.m_tgwInfo.szRelayUrl);
				}
				else
				{
					text = ApolloConfig.loginOnlyIpOrUrl;
				}
			}
			return text;
		}

		private string GetIPFromTgw(COMDT_TGWINFO tgwInfo, IspType ispType)
		{
			this.tempIpList.Clear();
			int num = 0;
			while ((long)num < (long)((ulong)tgwInfo.dwVipCnt))
			{
				if (tgwInfo.astVipInfo[num].iISPType == (int)ispType)
				{
					IPAddress iPAddress = new IPAddress((long)((ulong)tgwInfo.astVipInfo[num].dwVIP));
					string item = iPAddress.ToString();
					this.tempIpList.Add(item);
				}
				num++;
			}
			if (this.tempIpList.Count == 0)
			{
				return null;
			}
			int index = Random.Range(0, this.tempIpList.Count);
			return this.tempIpList[index];
		}
	}
}
