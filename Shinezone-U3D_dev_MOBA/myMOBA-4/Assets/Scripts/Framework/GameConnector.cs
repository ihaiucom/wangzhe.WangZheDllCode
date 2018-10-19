using Apollo;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using tsf4g_tdr_csharp;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	public class GameConnector : BaseConnector
	{
		public const int nCmdRedQueueCount = 3;

		private int nBuffSize = 204800;

		private byte[] szSendBuffer = new byte[204800];

		private ReconnectPolicy reconPolicy = new ReconnectPolicy();

		private List<CSPkg> gameMsgSendQueue = new List<CSPkg>();

		private List<CSPkg> confirmSendQueue = new List<CSPkg>();

		private int nCmdRedQueueIndex;

		private IFrameCommand[] cmdRedundancyQueue = new IFrameCommand[3];

		private bool netStateChanged;

		private NetworkState changedNetState;

		public NetworkReachability curNetworkReachability;

		~GameConnector()
		{
			base.DestroyConnector();
			this.reconPolicy = null;
		}

		public void Disconnect()
		{
			ApolloNetworkService.Intance.NetworkChangedEvent -= new NetworkStateChanged(this.NetworkStateChanged);
			base.DestroyConnector();
			this.reconPolicy.StopPolicy();
			this.reconPolicy.SetConnector(null, null, 0u);
			this.initParam = null;
		}

		public void Update()
		{
			this.reconPolicy.UpdatePolicy(false);
			if (this.netStateChanged)
			{
				if (this.changedNetState == NetworkState.NotReachable)
				{
					Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(Singleton<CTextManager>.GetInstance().GetText("NetworkConnecting"), 10, enUIEventID.None);
				}
				else
				{
					Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
				}
				this.netStateChanged = false;
			}
		}

		public void ForceReconnect()
		{
			this.connected = false;
			this.reconPolicy.UpdatePolicy(true);
		}

		public bool Init(ConnectorParam para)
		{
			this.reconPolicy.SetConnector(this, new tryReconnectDelegate(this.onTryReconnect), 8u);
			ApolloNetworkService.Intance.NetworkChangedEvent -= new NetworkStateChanged(this.NetworkStateChanged);
			ApolloNetworkService.Intance.NetworkChangedEvent += new NetworkStateChanged(this.NetworkStateChanged);
			this.curNetworkReachability = Application.internetReachability;
			return base.CreateConnector(para);
		}

		public void CleanUp()
		{
			this.gameMsgSendQueue.Clear();
			this.confirmSendQueue.Clear();
			this.reconPolicy.StopPolicy();
			this.ClearBuffer();
			this.nCmdRedQueueIndex = 0;
			Array.Clear(this.cmdRedundancyQueue, 0, this.cmdRedundancyQueue.Length);
		}

		private void ClearBuffer()
		{
			this.szSendBuffer.Initialize();
		}

		private uint onTryReconnect(uint nCount, uint nMax)
		{
			string connectUrl = Singleton<ReconnectIpSelect>.instance.GetConnectUrl(ConnectorType.Relay, nCount);
			this.initParam.SetVip(connectUrl);
			if (nCount < 2u)
			{
				if (!CCheatSystem.IsInhanceReconnect())
				{
					goto IL_68;
				}
			}
			try
			{
				MonoSingleton<Reconnection>.GetInstance().ShowReconnectMsgAlert((int)(nCount - 1u), (int)(nMax - 1u));
			}
			catch (Exception ex)
			{
				DebugHelper.Assert(false, "Exception In GameConnector Try Reconnect, {0} {1}", new object[]
				{
					ex.Message,
					ex.StackTrace
				});
			}
			IL_68:
			if (nCount == 2u)
			{
				List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
				list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
				list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
				list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
				list.Add(new KeyValuePair<string, string>("ping", Singleton<FrameSynchr>.instance.GameSvrPing.ToString()));
				if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
				{
					list.Add(new KeyValuePair<string, string>("Network", "3G or 4G"));
				}
				else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
				{
					list.Add(new KeyValuePair<string, string>("Network", "WIFI"));
				}
				else
				{
					list.Add(new KeyValuePair<string, string>("Network", "NoSignal"));
				}
				list.Add(new KeyValuePair<string, string>("FrameNum", Singleton<FrameSynchr>.instance.CurFrameNum.ToString()));
				list.Add(new KeyValuePair<string, string>("IsFighting", Singleton<BattleLogic>.instance.isFighting.ToString()));
				Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("GameConnector.onTryReconnect", list, true);
			}
			Singleton<DataReportSys>.GetInstance().AddBattleReconnectCount();
			return nCount;
		}

		private void NetworkStateChanged(NetworkState state)
		{
			this.changedNetState = state;
			this.netStateChanged = true;
		}

		protected override void DealConnectSucc()
		{
			this.reconPolicy.StopPolicy();
			Singleton<ReconnectIpSelect>.instance.SetRelaySuccessUrl(this.initParam.ip);
			MonoSingleton<Reconnection>.GetInstance().OnConnectSuccess();
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
			list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
			list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
			list.Add(new KeyValuePair<string, string>("openid", "NULL"));
			list.Add(new KeyValuePair<string, string>("status", "0"));
			list.Add(new KeyValuePair<string, string>("type", "challenge"));
			list.Add(new KeyValuePair<string, string>("errorCode", "SUCC"));
			Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_SvrConnectFail", list, true);
		}

		protected override void DealConnectError(ApolloResult result)
		{
			this.reconPolicy.StartPolicy(result, 6);
			MonoSingleton<Reconnection>.instance.QueryIsRelayGaming(result);
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
			list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
			list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
			list.Add(new KeyValuePair<string, string>("openid", "NULL"));
			list.Add(new KeyValuePair<string, string>("status", "0"));
			list.Add(new KeyValuePair<string, string>("type", "challenge"));
			list.Add(new KeyValuePair<string, string>("errorCode", result.ToString()));
			Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_SvrConnectFail", list, true);
		}

		protected override void DealConnectFail(ApolloResult result, ApolloLoginInfo loginInfo)
		{
			this.reconPolicy.StartPolicy(result, 6);
			MonoSingleton<Reconnection>.instance.QueryIsRelayGaming(result);
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
			list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
			list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
			list.Add(new KeyValuePair<string, string>("openid", "NULL"));
			list.Add(new KeyValuePair<string, string>("status", "0"));
			list.Add(new KeyValuePair<string, string>("type", "challenge"));
			list.Add(new KeyValuePair<string, string>("errorCode", result.ToString()));
			Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_SvrConnectFail", list, true);
		}

		protected override void DealConnectClose(ApolloResult result)
		{
		}

		public void PushSendMsg(CSPkg msg)
		{
			this.gameMsgSendQueue.Add(msg);
		}

		public void ImmeSendCmd(ref IFrameCommand cmd)
		{
			this.FlushSendCmd(cmd);
			if (cmd.cmdType == 128 || cmd.cmdType == 129 || cmd.cmdType == 130 || cmd.cmdType == 132)
			{
				this.nCmdRedQueueIndex = ++this.nCmdRedQueueIndex % 3;
				this.cmdRedundancyQueue[this.nCmdRedQueueIndex] = cmd;
			}
		}

		private void FlushSendCmd(IFrameCommand inCmd)
		{
			IFrameCommand frameCommand = null;
			IFrameCommand frameCommand2 = null;
			IFrameCommand frameCommand3 = null;
			if (inCmd != null)
			{
				frameCommand = inCmd;
				frameCommand2 = this.cmdRedundancyQueue[this.nCmdRedQueueIndex];
				frameCommand3 = this.cmdRedundancyQueue[(this.nCmdRedQueueIndex - 1 + 3) % 3];
			}
			else
			{
				frameCommand = this.cmdRedundancyQueue[this.nCmdRedQueueIndex];
				frameCommand2 = this.cmdRedundancyQueue[(this.nCmdRedQueueIndex - 1 + 3) % 3];
				frameCommand3 = this.cmdRedundancyQueue[(this.nCmdRedQueueIndex - 2 + 3) % 3];
			}

			if (frameCommand != null && (inCmd != null || frameCommand.sendCnt < 3))
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1004u);
				cSPkg.stPkgData.stGamingUperMsg.bNum = 0;
				this.PackCmd2Msg(ref frameCommand, cSPkg.stPkgData.stGamingUperMsg.astUperInfo[(int)cSPkg.stPkgData.stGamingUperMsg.bNum]);
                cSPkg.stPkgData.stGamingUperMsg.bNum += 1;
                frameCommand.sendCnt += 1;

				if (frameCommand2 != null && frameCommand2.sendCnt < 3 && (ulong)(frameCommand2.frameNum + 10u) > (ulong)((long)Time.frameCount))
				{
					this.PackCmd2Msg(ref frameCommand2, cSPkg.stPkgData.stGamingUperMsg.astUperInfo[(int)cSPkg.stPkgData.stGamingUperMsg.bNum]);
                    cSPkg.stPkgData.stGamingUperMsg.bNum += 1;
                    frameCommand2.sendCnt += 1;

					if (frameCommand3 != null && frameCommand3.sendCnt < 3 && (ulong)(frameCommand3.frameNum + 10u) > (ulong)((long)Time.frameCount))
					{
						this.PackCmd2Msg(ref frameCommand3, cSPkg.stPkgData.stGamingUperMsg.astUperInfo[(int)cSPkg.stPkgData.stGamingUperMsg.bNum]);
                        cSPkg.stPkgData.stGamingUperMsg.bNum += 1;
                        frameCommand3.sendCnt += 1;
					}
				}
				this.SendPackage(cSPkg);
				cSPkg.Release();
			}
		}

		private void PackCmd2Msg(ref IFrameCommand cmd, CSDT_GAMING_UPER_INFO msg)
		{
			if (cmd.isCSSync)
			{
				msg.bType = 2;
				msg.dwCmdSeq = cmd.cmdId;
				msg.stUperDt.construct((long)msg.bType);
				msg.stUperDt.stCSInfo.stCSSyncDt.construct((long)cmd.cmdType);
				cmd.TransProtocol(msg.stUperDt.stCSInfo);
			}
			else
			{
				msg.bType = 1;
				msg.dwCmdSeq = cmd.cmdId;
				msg.stUperDt.construct((long)msg.bType);
				msg.stUperDt.stCCInfo.construct();
				FRAME_CMD_PKG fRAME_CMD_PKG = FrameCommandFactory.CreateCommandPKG(cmd);
				cmd.TransProtocol(fRAME_CMD_PKG);
				int num = 0;
				TdrError.ErrorType errorType = fRAME_CMD_PKG.pack(ref msg.stUperDt.stCCInfo.szBuff, 64, ref num, 0u);
				msg.stUperDt.stCCInfo.wLen = (ushort)num;
				DebugHelper.Assert(errorType == TdrError.ErrorType.TDR_NO_ERROR);
				fRAME_CMD_PKG.Release();
			}
		}

		public void HandleSending()
		{
			if (this.connected)
			{
				this.FlushSendCmd(null);
				for (int i = 0; i < this.confirmSendQueue.Count; i++)
				{
					CSPkg cSPkg = this.confirmSendQueue[i];
					if (Singleton<GameLogic>.instance.GameRunningTick - cSPkg.stPkgHead.dwSvrPkgSeq > 5000u)
					{
						this.SendPackage(cSPkg);
						cSPkg.stPkgHead.dwSvrPkgSeq = Singleton<GameLogic>.instance.GameRunningTick;
					}
				}
				while (this.connected && this.gameMsgSendQueue.Count > 0)
				{
					CSPkg cSPkg2 = this.gameMsgSendQueue[0];
					if (!this.SendPackage(cSPkg2))
					{
						break;
					}
					if (cSPkg2.stPkgHead.dwReserve > 0u)
					{
						cSPkg2.stPkgHead.dwSvrPkgSeq = Singleton<GameLogic>.instance.GameRunningTick;
						this.confirmSendQueue.Add(cSPkg2);
					}
					else
					{
						cSPkg2.Release();
					}
					this.gameMsgSendQueue.RemoveAt(0);
				}
			}
			else
			{
				MonoSingleton<Reconnection>.instance.UpdateReconnect();
			}
		}

		public bool SendPackage(CSPkg msg)
		{
			if (!this.connected || this.connector == null)
			{
				return false;
			}
			int len = 0;
			if (msg.pack(ref this.szSendBuffer, this.nBuffSize, ref len, 0u) != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return false;
			}
			if (this.initParam.bIsUDP && (msg.stPkgHead.dwMsgID == 1004u || msg.stPkgHead.dwMsgID == 1260u || msg.stPkgHead.dwMsgID == 1036u || msg.stPkgHead.dwMsgID == 1037u))
			{
				return this.connector.WriteUdpData(this.szSendBuffer, len) == ApolloResult.Success;
			}
			return this.connector.WriteData(this.szSendBuffer, len) == ApolloResult.Success;
		}

		public CSPkg RecvPackage()
		{
			if (this.connected && this.connector != null)
			{
				byte[] array;
				int size;
				if (this.connector.ReadUdpData(out array, out size) == ApolloResult.Success)
				{
					int num = 0;
					CSPkg cSPkg = CSPkg.New();
					if (cSPkg.unpack(ref array, size, ref num, 0u) == TdrError.ErrorType.TDR_NO_ERROR && num > 0)
					{
						return cSPkg;
					}
				}
				if (this.connector.ReadData(out array, out size) == ApolloResult.Success)
				{
					int num2 = 0;
					CSPkg cSPkg2 = CSPkg.New();
					if (cSPkg2.unpack(ref array, size, ref num2, 0u) == TdrError.ErrorType.TDR_NO_ERROR && num2 > 0)
					{
						int i = 0;
						while (i < this.confirmSendQueue.Count)
						{
							CSPkg cSPkg3 = this.confirmSendQueue[i];
							if (cSPkg3.stPkgHead.dwReserve > 0u && cSPkg3.stPkgHead.dwReserve == cSPkg2.stPkgHead.dwMsgID)
							{
								cSPkg3.Release();
								this.confirmSendQueue.RemoveAt(i);
							}
							else
							{
								i++;
							}
						}
						return cSPkg2;
					}
				}
			}
			return null;
		}

		public void HandleMsg(CSPkg msg)
		{
			if (msg.stPkgHead.dwMsgID == 1075u || msg.stPkgHead.dwMsgID == 1077u)
			{
				Singleton<GameReplayModule>.instance.CacheRecord(msg);
			}
			NetMsgDelegate msgHandler = Singleton<NetworkModule>.instance.GetMsgHandler(msg.stPkgHead.dwMsgID);
			if (msgHandler != null)
			{
				msgHandler(msg);
			}
		}
	}
}
