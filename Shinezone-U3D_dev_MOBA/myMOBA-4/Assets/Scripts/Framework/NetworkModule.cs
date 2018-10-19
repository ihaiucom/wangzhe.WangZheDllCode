using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	public class NetworkModule : Singleton<NetworkModule>
	{

		public GameConnector gameSvr = new GameConnector();

		public LobbyConnector lobbySvr = new LobbyConnector();

		private Dictionary<uint, NetMsgDelegate> mNetMsgHandlers = new Dictionary<uint, NetMsgDelegate>();

		private bool _bOnlineMode = true;

		private float _preLobbyHeartTime;

		private float _preGameHeartTime;

		private uint m_uiRecvGameMsgCount;

		public bool isOnlineMode
		{
			get
			{
				return this._bOnlineMode;
			}
			set
			{
				this._bOnlineMode = value;
			}
		}

		public uint lobbyPing
		{
			get;
			private set;
		}

		public uint RecvGameMsgCount
		{
			get
			{
				return this.m_uiRecvGameMsgCount;
			}
			set
			{
				this.m_uiRecvGameMsgCount = value;
			}
		}

		public override void Init()
		{
			this.isOnlineMode = true;
			ClassEnumerator classEnumerator = new ClassEnumerator(typeof(MessageHandlerClassAttribute), null, typeof(NetworkModule).Assembly, true, false, false);
			ListView<Type>.Enumerator enumerator = classEnumerator.results.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Type current = enumerator.Current;
				MethodInfo[] methods = current.GetMethods();
				int num = 0;
				while (methods != null && num < methods.Length)
				{
					MethodInfo methodInfo = methods[num];
					if (methodInfo.IsStatic)
					{
						object[] customAttributes = methodInfo.GetCustomAttributes(typeof(MessageHandlerAttribute), true);
						for (int i = 0; i < customAttributes.Length; i++)
						{
							MessageHandlerAttribute messageHandlerAttribute = customAttributes[i] as MessageHandlerAttribute;
							if (messageHandlerAttribute != null)
							{
								this.RegisterMsgHandler(messageHandlerAttribute.ID, (NetMsgDelegate)Delegate.CreateDelegate(typeof(NetMsgDelegate), methodInfo));
								if (messageHandlerAttribute.AdditionalIdList != null)
								{
									int num2 = messageHandlerAttribute.AdditionalIdList.Length;
									for (int j = 0; j < num2; j++)
									{
										this.RegisterMsgHandler(messageHandlerAttribute.AdditionalIdList[j], (NetMsgDelegate)Delegate.CreateDelegate(typeof(NetMsgDelegate), methodInfo));
									}
								}
							}
						}
					}
					num++;
				}
			}
		}

		public void RegisterMsgHandler(uint cmdID, NetMsgDelegate handler)
		{
			if (this.mNetMsgHandlers.ContainsKey(cmdID))
			{
				return;
			}
			this.mNetMsgHandlers.Add(cmdID, handler);
		}

		public void RemoveMsgHandler(uint cmdID)
		{
			if (!this.mNetMsgHandlers.ContainsKey(cmdID))
			{
				return;
			}
			this.mNetMsgHandlers.Remove(cmdID);
		}

		public static void InitRelayConnnecting(COMDT_TGWINFO inRelayTgw)
		{
			if (inRelayTgw.dwVipCnt > 0u)
			{
				string text;
				if (MonoSingleton<CTongCaiSys>.GetInstance().IsCanUseTongCai())
				{
					text = ApolloConfig.loginOnlyIpOrUrl;
				}
				else if (inRelayTgw.szRelayUrl.Length > 0 && inRelayTgw.szRelayUrl[0] != 0)
				{
					text = StringHelper.UTF8BytesToString(ref inRelayTgw.szRelayUrl);
				}
				else
				{
					text = ApolloConfig.loginOnlyIpOrUrl;
				}
				MonoSingleton<GSDKsys>.GetInstance().DetermineWhichSpeed();
				Singleton<ReconnectIpSelect>.instance.SetRelayTgw(inRelayTgw);
				NetworkModule.LookUpDNSOfServerAndConfigNetAcc(text, (int)inRelayTgw.wEchoPort);
				ConnectorParam connectorParam = new ConnectorParam();
				connectorParam.bIsUDP = (inRelayTgw.bIsUDP > 0);
				connectorParam.ip = text;
				connectorParam.SetVPort(inRelayTgw.wVPort);
				NetworkAccelerator.ClearUDPCache();
				NetworkAccelerator.SetEchoPort((int)inRelayTgw.wEchoPort);
				NetworkAccelerator.SetConnectIP(text, inRelayTgw.wVPort);
				ApolloConfig.echoPort = inRelayTgw.wEchoPort;
				Singleton<NetworkModule>.GetInstance().InitGameServerConnect(connectorParam);
				MonoSingleton<GSDKsys>.GetInstance().StartSpeed(text, (int)inRelayTgw.wVPort);
			}
		}

		private static void LookUpDNSOfServerAndConfigNetAcc(string host, int port)
		{
			ApolloConfig.loginOnlyIp = string.Empty;
			try
			{
				Dns.BeginGetHostAddresses(host, new AsyncCallback(NetworkModule.DNSAsyncCallback), port);
			}
			catch (Exception var_0_28)
			{
			}
		}

		private static void DNSAsyncCallback(IAsyncResult ar)
		{
			int num = (int)ar.AsyncState;
			IPAddress[] array = Dns.EndGetHostAddresses(ar);
			if (array == null || array.Length == 0)
			{
				return;
			}
			ApolloConfig.loginOnlyIp = array[0].ToString();
		}

		public bool InitLobbyServerConnect(ConnectorParam para)
		{
			this.isOnlineMode = true;
			return this.lobbySvr.Init(para);
		}

		public bool InitGameServerConnect(ConnectorParam para)
		{
			MonoSingleton<Reconnection>.GetInstance().ResetRelaySyncCache();
			Singleton<FrameWindow>.GetInstance().Reset();
			return this.gameSvr.Init(para);
		}

		public void ResetLobbySending()
		{
			this.lobbySvr.ResetSending(true);
		}

		public void CloseAllServerConnect()
		{
			this.CloseLobbyServerConnect();
			this.CloseGameServerConnect(true);
		}

		public void CloseLobbyServerConnect()
		{
			this.lobbySvr.CleanUp();
			this.lobbySvr.Disconnect();
			this.lobbyPing = 0u;
		}

		public void CloseGameServerConnect(bool switchLocal = true)
		{
			if (switchLocal)
			{
				Singleton<FrameSynchr>.instance.SwitchSynchrLocal();
			}
			MonoSingleton<Reconnection>.instance.ResetRelaySyncCache();
			Singleton<FrameWindow>.GetInstance().Reset();
			this.gameSvr.CleanUp();
			this.gameSvr.Disconnect();
		}

		public void UpdateFrame()
		{
			if (this.isOnlineMode)
			{
				this.UpdateLobbyConnection();
				this.UpdateGameConnection();
				try
				{
					this.HandleLobbyMsgSend();
				}
				catch (Exception ex)
				{
					DebugHelper.Assert(false, "Error In HandleLobbyMsgSend: {0}, Call stack : {1}", new object[]
					{
						ex.Message,
						ex.StackTrace
					});
				}
				try
				{
					this.HandleGameMsgSend();
				}
				catch (Exception ex2)
				{
					DebugHelper.Assert(false, "Error In HandleGameMsgSend: {0}, Call stack : {1}", new object[]
					{
						ex2.Message,
						ex2.StackTrace
					});
				}
				try
				{
					this.HandleLobbyMsgRecv();
				}
				catch (Exception ex3)
				{
					DebugHelper.Assert(false, "Error In HandleLobbyMsgRecv: {0}, Call stack : {1}", new object[]
					{
						ex3.Message,
						ex3.StackTrace
					});
				}
				try
				{
					this.HandleGameMsgRecv();
				}
				catch (Exception ex4)
				{
					DebugHelper.Assert(false, "Error In HandleGameMsgRecv: {0}, Call stack : {1}", new object[]
					{
						ex4.Message,
						ex4.StackTrace
					});
				}
			}
		}

		private void HandleLobbyMsgSend()
		{
			if (this.isOnlineMode)
			{
				this.lobbySvr.HandleSending();
			}
		}

		private void HandleLobbyMsgRecv()
		{
			if (this.lobbySvr != null)
			{
				for (CSPkg cSPkg = this.lobbySvr.RecvPackage(); cSPkg != null; cSPkg = this.lobbySvr.RecvPackage())
				{
					NetMsgDelegate netMsgDelegate = null;
					if (this.mNetMsgHandlers.TryGetValue(cSPkg.stPkgHead.dwMsgID, out netMsgDelegate))
					{
						netMsgDelegate(cSPkg);
					}
					else if (cSPkg.stPkgHead.dwMsgID == 1261u)
					{
						uint num = (uint)(Time.realtimeSinceStartup * 1000f) - cSPkg.stPkgData.stGameSvrPing.dwTime;
						this.lobbyPing = (num + this.lobbyPing) / 2u;
					}
					this.lobbySvr.PostRecvPackage(cSPkg);
				}
			}
		}

		public void HandleGameMsgSend()
		{
			if (this.isOnlineMode)
			{
				this.gameSvr.HandleSending();
			}
		}

		private void HandleGameMsgRecv()
		{
			if (this.gameSvr != null)
			{
				CSPkg msg;
				while ((msg = this.gameSvr.RecvPackage()) != null)
				{
					this.m_uiRecvGameMsgCount += 1u;
					if (!MonoSingleton<Reconnection>.GetInstance().FilterRelaySvrPackage(msg))
					{
						this.gameSvr.HandleMsg(msg);
					}
				}
			}
		}

		private void UpdateLobbyConnection()
		{
			if (this.lobbySvr.CanSendPing() && Time.realtimeSinceStartup - this._preLobbyHeartTime > 5f)
			{
				this._preLobbyHeartTime = Time.realtimeSinceStartup;
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1261u);
				cSPkg.stPkgData.stGameSvrPing.dwTime = (uint)(Time.realtimeSinceStartup * 1000f);
				this.lobbySvr.PushSendMsg(cSPkg);
			}
		}

		public bool IsNeedSendGamePing()
		{
			return this.gameSvr.connected && Time.realtimeSinceStartup - this._preGameHeartTime > 2f;
		}

		private void UpdateGameConnection()
		{
			MonoSingleton<Reconnection>.instance.UpdateFrame();
			if (this.IsNeedSendGamePing())
			{
				this._preGameHeartTime = Time.realtimeSinceStartup;
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1260u);
				cSPkg.stPkgData.stRelaySvrPing.dwTime = (uint)(Time.realtimeSinceStartup * 1000f);
				cSPkg.stPkgData.stRelaySvrPing.dwSeqNo = (uint)Singleton<DataReportSys>.GetInstance().SendHeartAdd();
				this.gameSvr.PushSendMsg(cSPkg);
			}
		}

		public bool SendLobbyMsg(ref CSPkg msg, bool isShowAlert = false)
		{
			if (this.isOnlineMode)
			{
				if (isShowAlert && !Singleton<BattleLogic>.instance.isRuning)
				{
					Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
				}
				this.lobbySvr.PushSendMsg(msg);
				return true;
			}
			return false;
		}

		public bool SendGameMsg(ref CSPkg msg, uint confirmMsgID = 0u)
		{
			if (this.isOnlineMode && this.gameSvr.connected)
			{
				msg.stPkgHead.dwReserve = confirmMsgID;
				this.gameSvr.PushSendMsg(msg);
				return true;
			}
			return false;
		}

		public NetMsgDelegate GetMsgHandler(uint msgId)
		{
			NetMsgDelegate result;
			this.mNetMsgHandlers.TryGetValue(msgId, out result);
			return result;
		}

		public static CSPkg CreateDefaultCSPKG(uint msgID)
		{
			CSPkg cSPkg = CSPkg.New();
			cSPkg.stPkgHead.dwMsgID = msgID;
			cSPkg.stPkgHead.iVersion = MetaLib.getVersion();
			cSPkg.stPkgData.construct((long)((ulong)msgID));
			return cSPkg;
		}
	}
}
