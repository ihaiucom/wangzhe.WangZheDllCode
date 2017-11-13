using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;
using Tests;
using tsf4g_tdr_csharp;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	[MessageHandlerClass]
	public class FrameWindow : Singleton<FrameWindow>, IGameModule
	{
		public const uint FRQ_WIN_LEN = 900u;

		public const int MAX_TIMEOUT_TIMES = 5;

		private const int ALIVE_THRESHOLD_FRAME_OF32 = 10;

		private uint _sendCmdSeq;

		private byte _svrSession = 3;

		private CSDT_FRAPBOOT_INFO[] _receiveWindow;

		private uint _basFrqNo;

		private uint _begFrqNo;

		private uint _maxFrqNo;

		private int _repairCounter;

		private uint _repairBegNo;

		private int _repairTimes;

		private int _timeoutCounter;

		private int _timeoutTimes;

		private uint _aliveFlags;

		private int _aliveFrameCount;

		private int _timeoutFrameStep;

		private int _timeoutFrameOffset;

		private static byte _frameExceptionCounter;

		private bool _showChart;

		private uint NewSendCmdSeq
		{
			get
			{
				return this._sendCmdSeq += 1u;
			}
		}

		public FrameWindow()
		{
			this.Reset();
		}

		[MessageHandler(1035)]
		public static void onFrapBootInfoMultipleNtf(CSPkg msg)
		{
			SCPKG_FRAPBOOTINFO stFrapBootInfo = msg.stPkgData.stFrapBootInfo;
			Singleton<FrameWindow>.GetInstance()._svrSession = stFrapBootInfo.bSession;
			for (int i = 0; i < (int)stFrapBootInfo.bSpareNum; i++)
			{
				if (FrameWindow.HandleFraqBootSingle(stFrapBootInfo.astSpareFrap[i]))
				{
					MonoSingleton<Reconnection>.GetInstance().UpdateCachedLen(msg);
					break;
				}
			}
			Singleton<FrameWindow>.GetInstance().SampleFrameSpare((int)stFrapBootInfo.bSpareNum);
			msg.Release();
		}

		[MessageHandler(1034)]
		public static void onFrapBootInfoSingleNtf(CSPkg msg)
		{
			if (FrameWindow.HandleFraqBootSingle(msg.stPkgData.stFrapBootSingle))
			{
				MonoSingleton<Reconnection>.GetInstance().UpdateCachedLen(msg);
			}
			msg.Release();
		}

		private static bool HandleFraqBootSingle(SCPKG_FRAPBOOT_SINGLE fbi)
		{
			CSDT_FRAPBOOT_INFO cSDT_FRAPBOOT_INFO = CSDT_FRAPBOOT_INFO.New();
			int num = 0;
			return cSDT_FRAPBOOT_INFO.unpack(ref fbi.szInfoBuff, (int)fbi.wLen, ref num, 0u) == TdrError.ErrorType.TDR_NO_ERROR && num > 0 && Singleton<FrameWindow>.GetInstance().SetFrqWin(cSDT_FRAPBOOT_INFO);
		}

		private uint _FrqNoToWinIdx_(uint theFrqNo)
		{
			return (theFrqNo - this._basFrqNo) % 900u;
		}

		public void ResetSendCmdSeq()
		{
			this._sendCmdSeq = 0u;
		}

		public void Reset()
		{
			this._receiveWindow = new CSDT_FRAPBOOT_INFO[900];
			this._basFrqNo = 0u;
			this._begFrqNo = 1u;
			this._maxFrqNo = 0u;
			this._repairCounter = 0;
			this._repairBegNo = 0u;
			this._repairTimes = 0;
			this._timeoutCounter = 0;
			this._timeoutTimes = 0;
			this._aliveFlags = 0u;
			this._aliveFrameCount = 0;
			this._timeoutFrameStep = 3;
			this._timeoutFrameOffset = 1;
			FrameWindow._frameExceptionCounter = 0;
			this._svrSession = 3;
		}

		public void UpdateFrame()
		{
			if (Singleton<WatchController>.GetInstance().IsRelayCast)
			{
				return;
			}
			this._aliveFlags &= ~(1u << Time.frameCount % 32);
			if (Singleton<FrameSynchr>.GetInstance().bActive && !MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover)
			{
				if (this._maxFrqNo > this._begFrqNo && this._receiveWindow[(int)((uint)((UIntPtr)this._FrqNoToWinIdx_(this._begFrqNo)))] == null)
				{
					if (this._repairBegNo != this._begFrqNo)
					{
						this._repairBegNo = this._begFrqNo;
						this.RequestRepairFraqBootInfo();
						this._repairTimes = 0;
						this._repairCounter = 0;
					}
					else if (++this._repairCounter > (2 ^ this._repairTimes) * this._timeoutFrameStep)
					{
						this.RequestRepairFraqBootInfo();
						this._repairCounter = 0;
						this._repairTimes++;
					}
				}
				if (this._timeoutTimes < 5 && this._begFrqNo > 1u && ++this._timeoutCounter > (2 ^ this._timeoutTimes) * this._timeoutFrameStep + this._timeoutFrameOffset)
				{
					this.RequestTimeoutFraqBootInfo();
					this._timeoutCounter = 0;
					this._timeoutTimes++;
				}
			}
		}

		private bool SetFrqWin(CSDT_FRAPBOOT_INFO fbid)
		{
			if (this._aliveFrameCount != Time.frameCount)
			{
				this.RefreshTimeout();
			}
			bool result = false;
			uint dwKFrapsNo = fbid.dwKFrapsNo;
			if (dwKFrapsNo > this._maxFrqNo)
			{
				this._maxFrqNo = dwKFrapsNo;
			}
			if (this._begFrqNo <= dwKFrapsNo && dwKFrapsNo < this._begFrqNo + 900u)
			{
				this._receiveWindow[(int)((uint)((UIntPtr)this._FrqNoToWinIdx_(dwKFrapsNo)))] = fbid;
				if (Singleton<FrameSynchr>.GetInstance().bActive)
				{
					CSDT_FRAPBOOT_INFO fbid2;
					while ((fbid2 = this._FetchFBI_(this._begFrqNo)) != null)
					{
						if ((this._begFrqNo += 1u) % 900u == 0u)
						{
							this._basFrqNo = this._begFrqNo;
						}
						FrameWindow.HandleFraqBootInfo(fbid2);
						result = true;
					}
				}
			}
			else if (dwKFrapsNo > this._begFrqNo)
			{
				MonoSingleton<Reconnection>.GetInstance().RequestRelaySyncCacheFrames(false);
			}
			return result;
		}

		private CSDT_FRAPBOOT_INFO _FetchFBI_(uint frqNo)
		{
			uint num = this._FrqNoToWinIdx_(frqNo);
			CSDT_FRAPBOOT_INFO result = this._receiveWindow[(int)((uint)((UIntPtr)num))];
			this._receiveWindow[(int)((uint)((UIntPtr)num))] = null;
			return result;
		}

		private void RefreshTimeout()
		{
			this._aliveFrameCount = Time.frameCount;
			this._timeoutCounter = 0;
			this._aliveFlags |= 1u << Time.frameCount % 32;
			if (this._timeoutTimes > 0)
			{
				int num = 0;
				for (int i = 0; i < 32; i++)
				{
					if ((this._aliveFlags & 1u << i) > 0u)
					{
						num++;
					}
				}
				if (num > 10)
				{
					this._timeoutTimes = 0;
				}
			}
		}

		private void RequestRepairFraqBootInfo()
		{
			if (this._maxFrqNo <= this._begFrqNo)
			{
				return;
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1036u);
			CSPKG_REQUESTFRAPBOOTSINGLE stReqFrapBootSingle = cSPkg.stPkgData.stReqFrapBootSingle;
			stReqFrapBootSingle.bNum = 0;
			byte b = 0;
			byte b2 = 0;
			for (uint num = this._begFrqNo; num < this._maxFrqNo; num += 1u)
			{
				if (this._receiveWindow[(int)((uint)((UIntPtr)this._FrqNoToWinIdx_(num)))] == null)
				{
					b += 1;
					if ((int)stReqFrapBootSingle.bNum >= stReqFrapBootSingle.KFrapsNo.Length)
					{
						break;
					}
					uint[] kFrapsNo = stReqFrapBootSingle.KFrapsNo;
					CSPKG_REQUESTFRAPBOOTSINGLE cSPKG_REQUESTFRAPBOOTSINGLE = stReqFrapBootSingle;
					byte bNum;
					cSPKG_REQUESTFRAPBOOTSINGLE.bNum = (bNum = cSPKG_REQUESTFRAPBOOTSINGLE.bNum) + 1;
					kFrapsNo[(int)bNum] = num;
				}
				else
				{
					if (b > b2)
					{
						b2 = b;
					}
					b = 0;
				}
			}
			if (stReqFrapBootSingle.bNum > 0)
			{
				stReqFrapBootSingle.bFrapDiff = b2;
				stReqFrapBootSingle.bSession = this._svrSession;
				Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
			}
		}

		private void RequestTimeoutFraqBootInfo()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1037u);
			cSPkg.stPkgData.stReqFrapBootTimeout.dwCurKFrapsNo = this._begFrqNo;
			cSPkg.stPkgData.stReqFrapBootTimeout.bSession = this._svrSession;
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
		}

		public static void HandleFraqBootInfo(CSDT_FRAPBOOT_INFO fbid)
		{
			if (Singleton<FrameSynchr>.GetInstance().SetKeyFrameIndex(fbid.dwKFrapsNo))
			{
				Singleton<GameReplayModule>.instance.SetKFraqNo(fbid.dwKFrapsNo);
				if (fbid.bNum > 0)
				{
					Singleton<GameReplayModule>.instance.CacheRecord(fbid);
				}
				for (int i = 0; i < (int)fbid.bNum; i++)
				{
					CSDT_FRAPBOOT_DETAIL cSDT_FRAPBOOT_DETAIL = fbid.astBootInfo[i];
					switch (cSDT_FRAPBOOT_DETAIL.bType)
					{
					case 1:
						FrameWindow.HandleClientClientSyncCommand(fbid.dwKFrapsNo, cSDT_FRAPBOOT_DETAIL.stDetail.stCCBoot);
						break;
					case 2:
						FrameWindow.HandleClientServerSyncCommand(fbid.dwKFrapsNo, cSDT_FRAPBOOT_DETAIL.stDetail.stCSBoot);
						break;
					case 3:
						FrameWindow.HandleClientStateSyncCommand(fbid.dwKFrapsNo, cSDT_FRAPBOOT_DETAIL.stDetail.stAcntState);
						break;
					case 4:
						FrameWindow.HandleAssistChgSyncCommand(fbid.dwKFrapsNo, cSDT_FRAPBOOT_DETAIL.stDetail.stAssistState);
						break;
					case 5:
						FrameWindow.HandleAIChgSyncCommand(fbid.dwKFrapsNo, cSDT_FRAPBOOT_DETAIL.stDetail.stAiState);
						break;
					case 6:
						FrameWindow.HandleGameOverCommand(fbid.dwKFrapsNo, cSDT_FRAPBOOT_DETAIL.stDetail.stGameOverNtf);
						break;
					case 7:
						FrameWindow.HandleGamePauseCommand(fbid.dwKFrapsNo, cSDT_FRAPBOOT_DETAIL.stDetail.stPause);
						break;
					}
				}
			}
			fbid.Release();
		}

		private static void HandleClientClientSyncCommand(uint dwFrqNo, CSDT_FRAPBOOT_CC ccSynDt)
		{
			int num = 0;
			FRAME_CMD_PKG fRAME_CMD_PKG = FRAME_CMD_PKG.New();
			TdrError.ErrorType errorType = fRAME_CMD_PKG.unpack(ref ccSynDt.stSyncInfo.szBuff, (int)ccSynDt.stSyncInfo.wLen, ref num, 0u);
			DebugHelper.Assert(errorType == TdrError.ErrorType.TDR_NO_ERROR);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				IFrameCommand frameCommand = FrameCommandFactory.CreateFrameCommand(ref fRAME_CMD_PKG);
				if (frameCommand != null)
				{
					frameCommand.playerID = ccSynDt.dwObjID;
					frameCommand.frameNum = dwFrqNo;
					Singleton<FrameSynchr>.GetInstance().PushFrameCommand(frameCommand);
				}
				else if ((FrameWindow._frameExceptionCounter += 1) <= 30)
				{
					BuglyAgent.ReportException(new Exception("CreateFrameCommandException"), "create ccSync frame command error!");
				}
			}
			else if ((FrameWindow._frameExceptionCounter += 1) <= 30)
			{
				BuglyAgent.ReportException(new Exception("TdrUnpackException"), "CCSync unpack error!");
			}
			fRAME_CMD_PKG.Release();
		}

		private static void HandleClientServerSyncCommand(uint dwFrqNo, CSDT_FRAPBOOT_CS csSynDt)
		{
			IFrameCommand frameCommand = FrameCommandFactory.CreateFrameCommandByCSSyncInfo(ref csSynDt.stSyncInfo);
			if (frameCommand != null)
			{
				frameCommand.playerID = csSynDt.dwObjID;
				frameCommand.frameNum = dwFrqNo;
				Singleton<FrameSynchr>.GetInstance().PushFrameCommand(frameCommand);
			}
			else if ((FrameWindow._frameExceptionCounter += 1) <= 30)
			{
				BuglyAgent.ReportException(new Exception("CreateFrameCommandException"), "create csSync frame command error!");
			}
		}

		private static void HandleClientStateSyncCommand(uint dwFrqNo, CSDT_FRAPBOOT_ACNTSTATE stateSyncDt)
		{
			IFrameCommand frameCommand = null;
			switch (stateSyncDt.bStateChgType)
			{
			case 1:
				frameCommand = FrameCommandFactory.CreateSCSyncFrameCommand<SvrDisconnectCommand>();
				break;
			case 2:
				frameCommand = FrameCommandFactory.CreateSCSyncFrameCommand<SvrReconnectCommand>();
				break;
			case 3:
				frameCommand = FrameCommandFactory.CreateSCSyncFrameCommand<SvrRunawayCommand>();
				break;
			}
			if (frameCommand != null)
			{
				frameCommand.playerID = stateSyncDt.dwObjID;
				frameCommand.frameNum = dwFrqNo;
				Singleton<FrameSynchr>.GetInstance().PushFrameCommand(frameCommand);
			}
			else if ((FrameWindow._frameExceptionCounter += 1) <= 30)
			{
				BuglyAgent.ReportException(new Exception("CreateFrameCommandException"), "create stateChange frame command error!");
			}
		}

		private static void HandleAssistChgSyncCommand(uint dwFrqNo, CSDT_FRAPBOOT_ASSISTSTATE assistChgState)
		{
			FrameCommand<AssistStateChgCommand> frameCommand = FrameCommandFactory.CreateSCSyncFrameCommand<AssistStateChgCommand>();
			frameCommand.cmdData.m_chgType = assistChgState.bType;
			frameCommand.cmdData.m_aiPlayerID = assistChgState.dwAiPlayerObjID;
			frameCommand.cmdData.m_masterPlayerID = assistChgState.dwMasterObjID;
			IFrameCommand frameCommand2 = frameCommand;
			if (frameCommand2 != null)
			{
				frameCommand2.playerID = assistChgState.dwAiPlayerObjID;
				frameCommand2.frameNum = dwFrqNo;
				Singleton<FrameSynchr>.GetInstance().PushFrameCommand(frameCommand2);
			}
			else if ((FrameWindow._frameExceptionCounter += 1) <= 30)
			{
				BuglyAgent.ReportException(new Exception("CreateFrameCommandException"), "create assistChange frame command error!");
			}
		}

		private static void HandleAIChgSyncCommand(uint dwFrqNo, CSDT_FRAPBOOT_AISTATE AIState)
		{
			FrameCommand<AutoAIChgCommand> frameCommand = FrameCommandFactory.CreateSCSyncFrameCommand<AutoAIChgCommand>();
			frameCommand.cmdData.m_autoType = AIState.bType;
			frameCommand.cmdData.m_playerID = AIState.dwPlayerObjID;
			IFrameCommand frameCommand2 = frameCommand;
			if (frameCommand2 != null)
			{
				frameCommand2.playerID = AIState.dwPlayerObjID;
				frameCommand2.frameNum = dwFrqNo;
				Singleton<FrameSynchr>.GetInstance().PushFrameCommand(frameCommand2);
			}
			else if ((FrameWindow._frameExceptionCounter += 1) <= 30)
			{
				BuglyAgent.ReportException(new Exception("CreateFrameCommandException"), "create aiChange frame command error!");
			}
		}

		private static void HandleGameOverCommand(uint dwFrqNo, CSDT_FRAPBOOT_GAMEOVERNTF OverNtf)
		{
			FrameCommand<SvrNtfGameOverCommand> frameCommand = FrameCommandFactory.CreateSCSyncFrameCommand<SvrNtfGameOverCommand>();
			frameCommand.cmdData.m_bWinCamp = OverNtf.bWinCamp;
			IFrameCommand frameCommand2 = frameCommand;
			if (frameCommand2 != null)
			{
				frameCommand2.frameNum = dwFrqNo;
				Singleton<FrameSynchr>.GetInstance().PushFrameCommand(frameCommand2);
			}
		}

		private static void HandleGamePauseCommand(uint dwFrqNo, CSDT_FRAPBOOT_PAUSE pauseNtf)
		{
			FrameCommand<PauseResumeGameCommand> frameCommand = FrameCommandFactory.CreateSCSyncFrameCommand<PauseResumeGameCommand>();
			frameCommand.cmdData.PauseCommand = pauseNtf.bType;
			frameCommand.cmdData.PauseByCamp = pauseNtf.bCamp;
			frameCommand.cmdData.CampPauseTimes = new byte[pauseNtf.szCampPauseNum.Length];
			Buffer.BlockCopy(pauseNtf.szCampPauseNum, 0, frameCommand.cmdData.CampPauseTimes, 0, pauseNtf.szCampPauseNum.Length);
			IFrameCommand frameCommand2 = frameCommand;
			if (frameCommand2 != null)
			{
				frameCommand2.frameNum = dwFrqNo;
				Singleton<FrameSynchr>.GetInstance().PushFrameCommand(frameCommand2);
			}
		}

		public bool SendGameCmd(IFrameCommand cmd, bool bMultiGame)
		{
			if (Singleton<WatchController>.GetInstance().IsWatching)
			{
				return false;
			}
			if (Singleton<NetworkModule>.GetInstance().isOnlineMode)
			{
				if (bMultiGame)
				{
					if (Singleton<NetworkModule>.GetInstance().gameSvr.connected)
					{
						cmd.cmdId = this.NewSendCmdSeq;
						cmd.frameNum = (uint)Time.frameCount;
						cmd.sendCnt = 0;
						Singleton<NetworkModule>.GetInstance().gameSvr.ImmeSendCmd(ref cmd);
					}
				}
				else
				{
					Singleton<FrameSynchr>.GetInstance().PushFrameCommand(cmd);
				}
			}
			return true;
		}

		public void ToggleShowFrameSpareChart()
		{
			this._showChart = !this._showChart;
			if (this._showChart)
			{
				MonoSingleton<RealTimeChart>.instance.isVisible = true;
				Track track = MonoSingleton<RealTimeChart>.instance.AddTrack("FramePackageSpare", Color.yellow, true, 0f, 5f);
				track.isVisiable = true;
			}
			else
			{
				MonoSingleton<RealTimeChart>.instance.isVisible = false;
				MonoSingleton<RealTimeChart>.instance.RemoveTrack("FramePackageSpare");
			}
		}

		private void SampleFrameSpare(int frameSpare)
		{
			if (this._showChart)
			{
				MonoSingleton<RealTimeChart>.instance.AddSample("FramePackageSpare", (float)frameSpare);
			}
		}
	}
}
