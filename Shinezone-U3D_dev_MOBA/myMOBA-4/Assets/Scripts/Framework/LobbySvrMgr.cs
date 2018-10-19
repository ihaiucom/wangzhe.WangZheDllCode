using Apollo;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	public class LobbySvrMgr : Singleton<LobbySvrMgr>
	{
		public delegate void ConnectFailHandler(ApolloResult result);

		public bool isFirstLogin;

		public bool isLogin;

		private bool isLoginingByDefault = true;

		public ChooseSvrPolicy chooseSvrPol;

		public bool canReconnect = true;

		public event LobbySvrMgr.ConnectFailHandler connectFailHandler;

		public bool ConnectServer()
		{
			this.canReconnect = true;
			if (Singleton<NetworkModule>.GetInstance().isOnlineMode && !this.isLogin)
			{
				Singleton<NetworkModule>.GetInstance().lobbySvr.ConnectedEvent -= new NetConnectedEvent(this.onLobbyConnected);
				Singleton<NetworkModule>.GetInstance().lobbySvr.DisconnectEvent -= new NetDisconnectEvent(this.onLobbyDisconnected);
				Singleton<NetworkModule>.GetInstance().lobbySvr.ConnectedEvent += new NetConnectedEvent(this.onLobbyConnected);
				Singleton<NetworkModule>.GetInstance().lobbySvr.DisconnectEvent += new NetDisconnectEvent(this.onLobbyDisconnected);
				Singleton<NetworkModule>.GetInstance().lobbySvr.GetTryReconnect -= new LobbyConnector.DelegateGetTryReconnect(this.OnTryReconnect);
				Singleton<NetworkModule>.GetInstance().lobbySvr.GetTryReconnect += new LobbyConnector.DelegateGetTryReconnect(this.OnTryReconnect);

				ConnectorParam connectorParam = new ConnectorParam();
				connectorParam.url = ApolloConfig.loginUrl;
				connectorParam.ip = ApolloConfig.loginOnlyIpOrUrl;
				connectorParam.vPort = ApolloConfig.loginOnlyVPort;
				bool flag = Singleton<NetworkModule>.GetInstance().InitLobbyServerConnect(connectorParam);
				if (flag)
				{
					Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(null, 10, enUIEventID.None);
				}
				return flag;
			}
			return false;
		}

		public uint OnTryReconnect(uint curConnectTime, uint maxcount)
		{
			if (this.canReconnect)
			{
				if (this.isLogin)
				{
					string connectUrl = Singleton<ReconnectIpSelect>.instance.GetConnectUrl(ConnectorType.Lobby, curConnectTime);
					ApolloConfig.loginOnlyIpOrUrl = connectUrl;
					ApolloConfig.loginUrl = string.Format("tcp://{0}:{1}", ApolloConfig.loginOnlyIpOrUrl, ApolloConfig.loginOnlyVPort);
					Singleton<NetworkModule>.instance.lobbySvr.initParam.SetVip(ApolloConfig.loginOnlyIpOrUrl);
					if (!Singleton<LobbyLogic>.instance.inMultiGame && (Singleton<BattleLogic>.instance.isGameOver || Singleton<BattleLogic>.instance.m_bIsPayStat))
					{
						if (curConnectTime > maxcount)
						{
							if (curConnectTime == maxcount + 1u)
							{
								Singleton<LobbyLogic>.instance.OnSendSingleGameFinishFail();
							}
						}
						else
						{
							Singleton<CUIManager>.GetInstance().OpenSendMsgAlert("尝试重新连接服务器...", 5, enUIEventID.None);
						}
						return curConnectTime;
					}
					if (!Singleton<BattleLogic>.instance.isRuning)
					{
						Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
					}
					if (curConnectTime >= maxcount)
					{
						return 0u;
					}
					return curConnectTime;
				}
				else if (this.isLoginingByDefault)
				{
					this.ConnectServerWithTdirCandidate(MonoSingleton<TdirMgr>.GetInstance().m_connectIndex);
				}
				else
				{
					this.ConnectFailed();
				}
			}
			return curConnectTime + maxcount;
		}

		public void ConnectServerWithTdirDefault(int index, ChooseSvrPolicy policy)
		{
			this.chooseSvrPol = policy;
			this.isLoginingByDefault = true;
			ApolloConfig.loginOnlyVPort = ushort.Parse(MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs[index].port);
			ApolloConfig.ISPType = (int)MonoSingleton<TdirMgr>.GetInstance().GetISP();
			if (MonoSingleton<CTongCaiSys>.instance.IsCanUseTongCai())
			{
				ApolloConfig.loginUrl = string.Format("tcp://{0}:{1}", MonoSingleton<CTongCaiSys>.instance.TongcaiUrl, MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs[index].port);
				ApolloConfig.loginOnlyIpOrUrl = MonoSingleton<CTongCaiSys>.instance.TongcaiUrl;
			}
			else
			{
				ApolloConfig.loginUrl = string.Format("tcp://{0}:{1}", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs[index].ip, MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs[index].port);
				ApolloConfig.loginOnlyIpOrUrl = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs[index].ip;
			}
			this.ConnectServer();
			Singleton<BeaconHelper>.GetInstance().Event_CommonReport("Event_LoginMsgSend");
		}

		private void ConnectServerWithTdirCandidate(int index)
		{
			bool flag = false;
			this.isLoginingByDefault = false;
			ApolloConfig.loginOnlyVPort = ushort.Parse(MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs[index].port);
			if (MonoSingleton<CTongCaiSys>.instance.IsCanUseTongCai())
			{
				flag = true;
				ApolloConfig.ISPType = (int)MonoSingleton<TdirMgr>.GetInstance().GetISP();
				ApolloConfig.loginOnlyIpOrUrl = MonoSingleton<CTongCaiSys>.instance.TongcaiIps[ApolloConfig.ISPType];
				ApolloConfig.loginUrl = string.Format("tcp://{0}:{1}", ApolloConfig.loginOnlyIpOrUrl, MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs[index].port);
			}
			else
			{
				object obj = 0;
				if (MonoSingleton<TdirMgr>.GetInstance().ParseNodeAppAttrWithBackup(MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.attr, MonoSingleton<TdirMgr>.GetInstance().mRootNodeAppAttr, TdirNodeAttrType.ISPChoose, ref obj))
				{
					Dictionary<string, int> dictionary = (Dictionary<string, int>)obj;
					if (dictionary != null)
					{
						foreach (KeyValuePair<string, int> current in dictionary)
						{
							if (current.Value == (int)MonoSingleton<TdirMgr>.GetInstance().GetISP())
							{
								ApolloConfig.loginUrl = string.Format("tcp://{0}:{1}", current.Key, MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs[index].port);
								ApolloConfig.loginOnlyIpOrUrl = current.Key;
								ApolloConfig.ISPType = (int)MonoSingleton<TdirMgr>.GetInstance().GetISP();
								flag = true;
								break;
							}
						}
					}
				}
			}
			if (flag)
			{
				this.ConnectServer();
			}
			else
			{
				this.ConnectFailed();
			}
		}

		private void ConnectFailed()
		{
			if (this.chooseSvrPol == ChooseSvrPolicy.DeviceID)
			{
				MonoSingleton<TdirMgr>.GetInstance().ConnectLobbyWithSnsNameChooseSvr();
			}
			else if (this.chooseSvrPol == ChooseSvrPolicy.NickName)
			{
				MonoSingleton<TdirMgr>.GetInstance().ConnectLobbyRandomChooseSvr(ChooseSvrPolicy.Random1);
			}
			else if (this.chooseSvrPol == ChooseSvrPolicy.Random1)
			{
				MonoSingleton<TdirMgr>.GetInstance().ConnectLobbyRandomChooseSvr(ChooseSvrPolicy.Random2);
			}
			else
			{
				Debug.Log("LobbyConnector ConnectFailed called!");
				if (this.connectFailHandler != null)
				{
					this.connectFailHandler(Singleton<NetworkModule>.GetInstance().lobbySvr.lastResult);
				}
			}
		}

		private void onLobbyConnected(object sender)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
		}

		private void onLobbyDisconnected(object sender)
		{
		}
	}
}
