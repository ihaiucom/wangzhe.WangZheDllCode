using Apollo;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using tsf4g_tdr_csharp;

namespace Assets.Scripts.Framework
{
	public class LobbyConnector : BaseConnector
	{
		public delegate uint DelegateGetTryReconnect(uint curConnectTime, uint maxCount);

		private static int nBuffSize = 204800;

		private byte[] szSendBuffer = new byte[204800];

		public LobbyConnector.DelegateGetTryReconnect GetTryReconnect;

		public uint curSvrPkgSeq;

		public uint curCltPkgSeq;

		public ApolloResult lastResult;

		private ReconnectPolicy reconPolicy = new ReconnectPolicy();

		private List<CSPkg> lobbySendQueue = new List<CSPkg>();

		private List<CSPkg> confirmSendQueue = new List<CSPkg>();

		public event NetConnectedEvent ConnectedEvent
		{
			[MethodImpl(32)]
			add
			{
				this.ConnectedEvent = (NetConnectedEvent)Delegate.Combine(this.ConnectedEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.ConnectedEvent = (NetConnectedEvent)Delegate.Remove(this.ConnectedEvent, value);
			}
		}

		public event NetDisconnectEvent DisconnectEvent
		{
			[MethodImpl(32)]
			add
			{
				this.DisconnectEvent = (NetDisconnectEvent)Delegate.Combine(this.DisconnectEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.DisconnectEvent = (NetDisconnectEvent)Delegate.Remove(this.DisconnectEvent, value);
			}
		}

		~LobbyConnector()
		{
			base.DestroyConnector();
			this.reconPolicy = null;
		}

		public void Disconnect()
		{
			base.DestroyConnector();
			this.reconPolicy.StopPolicy();
			this.reconPolicy.SetConnector(null, null, 0u);
		}

		public bool RedirectNewPort(ushort nPort)
		{
			this.initParam.SetVPort(nPort);
			this.reconPolicy.SetConnector(this, new tryReconnectDelegate(this.onTryReconnect), 8u);
			return base.CreateConnector(this.initParam);
		}

		public bool CanSendPing()
		{
			return this.connected && this.lobbySendQueue.get_Count() == 0 && this.curSvrPkgSeq > 0u;
		}

		public bool Init(ConnectorParam para)
		{
			this.reconPolicy.SetConnector(this, new tryReconnectDelegate(this.onTryReconnect), 8u);
			return base.CreateConnector(para);
		}

		public void CleanUp()
		{
			this.lobbySendQueue.Clear();
			this.confirmSendQueue.Clear();
			this.reconPolicy.StopPolicy();
			this.szSendBuffer.Initialize();
			this.curSvrPkgSeq = 0u;
			this.curCltPkgSeq = 0u;
		}

		public void ResetSending(bool bResetSeq)
		{
			this.lobbySendQueue.Clear();
			this.confirmSendQueue.Clear();
			this.szSendBuffer.Initialize();
			if (bResetSeq)
			{
				this.curCltPkgSeq = 0u;
			}
		}

		private bool CanPushMsg(CSPkg msg)
		{
			return this.connected || (msg.stPkgHead.dwMsgID != 1305u && msg.stPkgHead.dwMsgID != 1261u && msg.stPkgHead.dwMsgID != 1301u && msg.stPkgHead.dwMsgID != 2289u);
		}

		public void PushSendMsg(CSPkg msg)
		{
			if (this.CanPushMsg(msg))
			{
				this.curCltPkgSeq += 1u;
				msg.stPkgHead.dwReserve = this.curCltPkgSeq;
				this.lobbySendQueue.Add(msg);
			}
		}

		public void HandleSending()
		{
			if (this.connected)
			{
				int num = 0;
				while (this.connected && num < this.lobbySendQueue.get_Count())
				{
					CSPkg cSPkg = this.lobbySendQueue.get_Item(num);
					if (this.SendPackage(cSPkg))
					{
						if (cSPkg.stPkgHead.dwMsgID != 1261u)
						{
							this.confirmSendQueue.Add(cSPkg);
						}
						this.lobbySendQueue.RemoveAt(num);
					}
					else
					{
						num++;
					}
				}
			}
			else
			{
				this.reconPolicy.UpdatePolicy(false);
			}
		}

		private uint onTryReconnect(uint nCount, uint nMax)
		{
			ListView<CSPkg> listView = new ListView<CSPkg>();
			for (int i = 0; i < this.lobbySendQueue.get_Count(); i++)
			{
				listView.Add(this.lobbySendQueue.get_Item(i));
			}
			this.lobbySendQueue.Clear();
			for (int j = 0; j < this.confirmSendQueue.get_Count(); j++)
			{
				this.lobbySendQueue.Add(this.confirmSendQueue.get_Item(j));
			}
			this.confirmSendQueue.Clear();
			for (int k = 0; k < listView.Count; k++)
			{
				this.lobbySendQueue.Add(listView[k]);
			}
			Singleton<DataReportSys>.GetInstance().AddLobbyReconnectCount();
			if (this.GetTryReconnect != null)
			{
				return this.GetTryReconnect(nCount, nMax);
			}
			return 0u;
		}

		protected override void DealConnectSucc()
		{
			if (MonoSingleton<CTongCaiSys>.instance.IsCanUseTongCai())
			{
				LobbyConnector.LookUpDNSOfServerTongcai(ApolloConfig.loginOnlyIpOrUrl, 0);
			}
			else
			{
				ApolloConfig.loginOnlyIpTongCai = string.Empty;
			}
			this.reconPolicy.StopPolicy();
			Singleton<ReconnectIpSelect>.instance.SetLobbySuccessUrl(this.initParam.ip);
			if (this.ConnectedEvent != null)
			{
				this.ConnectedEvent(this);
			}
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			MonoSingleton<TssdkSys>.GetInstance().OnAccountLogin();
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
			list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
			list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
			list.Add(new KeyValuePair<string, string>("openid", "NULL"));
			list.Add(new KeyValuePair<string, string>("status", "0"));
			list.Add(new KeyValuePair<string, string>("type", "platform"));
			list.Add(new KeyValuePair<string, string>("errorCode", "SUCC"));
			Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_SvrConnectFail", list, true);
		}

		protected override void DealConnectError(ApolloResult result)
		{
			this.lastResult = result;
			this.reconPolicy.StartPolicy(result, 10);
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
			list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
			list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
			list.Add(new KeyValuePair<string, string>("openid", "NULL"));
			list.Add(new KeyValuePair<string, string>("status", "0"));
			list.Add(new KeyValuePair<string, string>("type", "platform"));
			list.Add(new KeyValuePair<string, string>("errorCode", result.ToString()));
			Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_SvrConnectFail", list, true);
		}

		protected override void DealConnectFail(ApolloResult result, ApolloLoginInfo loginInfo)
		{
			this.lastResult = result;
			if (result == ApolloResult.StayInQueue || result == ApolloResult.SvrIsFull)
			{
				MonoSingleton<TdirMgr>.GetInstance().ConnectLobby();
			}
			else
			{
				this.reconPolicy.StartPolicy(result, 10);
			}
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
			list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
			list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
			list.Add(new KeyValuePair<string, string>("openid", "NULL"));
			list.Add(new KeyValuePair<string, string>("status", "0"));
			list.Add(new KeyValuePair<string, string>("type", "platform"));
			list.Add(new KeyValuePair<string, string>("errorCode", result.ToString()));
			Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_SvrConnectFail", list, true);
		}

		protected override void DealConnectClose(ApolloResult result)
		{
			if (this.DisconnectEvent != null)
			{
				this.DisconnectEvent(this);
			}
		}

		private bool SendPackage(CSPkg msg)
		{
			if (!this.connected || this.connector == null)
			{
				return false;
			}
			msg.stPkgHead.dwSvrPkgSeq = this.curSvrPkgSeq;
			int num = 0;
			if (msg.pack(ref this.szSendBuffer, LobbyConnector.nBuffSize, ref num, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
			{
				byte[] array = new byte[num];
				Array.Copy(this.szSendBuffer, array, num);
				return this.connector.WriteData(array, -1) == ApolloResult.Success;
			}
			return false;
		}

		public void PostRecvPackage(CSPkg msg)
		{
			if (msg != null && msg.stPkgHead.dwReserve <= this.curCltPkgSeq)
			{
				int i = 0;
				while (i < this.confirmSendQueue.get_Count())
				{
					CSPkg cSPkg = this.confirmSendQueue.get_Item(i);
					if (cSPkg.stPkgHead.dwReserve > 0u && cSPkg.stPkgHead.dwReserve <= msg.stPkgHead.dwReserve)
					{
						this.confirmSendQueue.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
			}
		}

		public CSPkg RecvPackage()
		{
			byte[] array;
			int size;
			if (this.connected && this.connector != null && this.connector.ReadData(out array, out size) == ApolloResult.Success)
			{
				int num = 0;
				CSPkg cSPkg = CSPkg.New();
				TdrError.ErrorType errorType = cSPkg.unpack(ref array, size, ref num, 0u);
				if (errorType == TdrError.ErrorType.TDR_NO_ERROR && num > 0)
				{
					if (cSPkg.stPkgHead.dwMsgID == 1014u)
					{
						this.curSvrPkgSeq = 0u;
					}
					if (cSPkg.stPkgHead.dwSvrPkgSeq > this.curSvrPkgSeq || cSPkg.stPkgHead.dwSvrPkgSeq == 0u)
					{
						if (cSPkg.stPkgHead.dwSvrPkgSeq > this.curSvrPkgSeq && cSPkg.stPkgHead.dwMsgID != 1040u)
						{
							this.curSvrPkgSeq = cSPkg.stPkgHead.dwSvrPkgSeq;
						}
						return cSPkg;
					}
				}
				else
				{
					DebugHelper.Assert(false, "TDR Unpack lobbyMsg Error -- {0}", new object[]
					{
						errorType
					});
				}
			}
			return null;
		}

		private static void LookUpDNSOfServerTongcai(string host, int port)
		{
			ApolloConfig.loginOnlyIpTongCai = string.Empty;
			try
			{
				Dns.BeginGetHostAddresses(host, new AsyncCallback(LobbyConnector.DNSAsyncCallbackTongcai), port);
			}
			catch (Exception var_0_28)
			{
			}
		}

		private static void DNSAsyncCallbackTongcai(IAsyncResult ar)
		{
			int num = (int)ar.get_AsyncState();
			IPAddress[] array = Dns.EndGetHostAddresses(ar);
			if (array == null || array.Length == 0)
			{
				return;
			}
			string loginOnlyIpTongCai = array[0].ToString();
			ApolloConfig.loginOnlyIpTongCai = loginOnlyIpTongCai;
		}
	}
}
