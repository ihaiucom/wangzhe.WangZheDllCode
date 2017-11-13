using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using tsf4g_tdr_csharp;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class WatchController : Singleton<WatchController>
	{
		public enum WorkMode
		{
			None,
			Observe,
			Replay,
			Judge
		}

		public const float OVER_WAIT_TIME = 10f;

		public const int RECV_BUFF_SIZE = 409600;

		public const int MAX_PUSH_FRAME_LEN = 60;

		public const int STOP_SPEED_BUFFERLIMIT = 120;

		private WatchController.WorkMode _workMode;

		private bool _isRunning;

		private byte _speedRate = 1;

		private float _overTime;

		private COMDT_TGWINFO _tgw;

		private uint _observeDelayFrames = 900u;

		private uint _maxFrameNo;

		private bool _isStreamEnd;

		private byte[] _recvBuff;

		private uint _buffRecvSize;

		private uint _totalRecvSize;

		private Queue<object> _pkgQueue;

		private uint _thisTick;

		private uint _lastRequestTick;

		private uint _requestTickSpan = 80u;

		public ulong TargetUID;

		private COM_PLAYERCAMP observeCamp_ = COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT;

		public bool FightOverJust
		{
			get;
			private set;
		}

		public WatchController.WorkMode workMode
		{
			get
			{
				return this._workMode;
			}
		}

		public COM_PLAYERCAMP HorizonCamp
		{
			get
			{
				return this.observeCamp_;
			}
		}

		public bool IsWatching
		{
			get
			{
				return this._workMode != WatchController.WorkMode.None;
			}
		}

		public bool IsObserving
		{
			get
			{
				return this._workMode == WatchController.WorkMode.Observe;
			}
		}

		public bool IsReplaying
		{
			get
			{
				return this._workMode == WatchController.WorkMode.Replay;
			}
		}

		public bool IsLiveCast
		{
			get
			{
				return this._workMode == WatchController.WorkMode.Judge;
			}
		}

		public bool IsRelayCast
		{
			get
			{
				return this._workMode == WatchController.WorkMode.Observe || this._workMode == WatchController.WorkMode.Replay;
			}
		}

		public bool IsRunning
		{
			get
			{
				return this._isRunning;
			}
			set
			{
				if (this.IsWatching)
				{
					this._isRunning = value;
					Singleton<FrameSynchr>.GetInstance().SetSynchrRunning(this._isRunning);
					Singleton<FrameSynchr>.GetInstance().FrameSpeed = this._speedRate;
				}
			}
		}

		public byte SpeedRateMin
		{
			get
			{
				return 1;
			}
		}

		public byte SpeedRateMax
		{
			get
			{
				return 8;
			}
		}

		public byte SpeedRate
		{
			get
			{
				return this._speedRate;
			}
			set
			{
				this._speedRate = (byte)Mathf.Clamp((int)value, (int)this.SpeedRateMin, (int)this.SpeedRateMax);
				if (this.IsWatching && this._isRunning)
				{
					Singleton<FrameSynchr>.GetInstance().FrameSpeed = this._speedRate;
					Time.timeScale = (float)this._speedRate;
				}
			}
		}

		public uint CurFrameNo
		{
			get
			{
				uint curFrameNum = Singleton<FrameSynchr>.GetInstance().CurFrameNum;
				uint endFrameNo = this.EndFrameNo;
				return (curFrameNum < endFrameNo) ? curFrameNum : endFrameNo;
			}
		}

		public uint EndFrameNo
		{
			get
			{
				if (this._workMode == WatchController.WorkMode.Observe)
				{
					return this._isStreamEnd ? this._maxFrameNo : ((this._maxFrameNo > this._observeDelayFrames) ? (this._maxFrameNo - this._observeDelayFrames) : 0u);
				}
				return Singleton<FrameSynchr>.GetInstance().EndFrameNum;
			}
		}

		public uint FrameDelta
		{
			get
			{
				return Singleton<FrameSynchr>.GetInstance().FrameDelta;
			}
		}

		public bool StartObserve(COMDT_TGWINFO inTGW)
		{
			if (this.IsWatching || Singleton<BattleLogic>.instance.isRuning)
			{
				return false;
			}
			this._workMode = WatchController.WorkMode.Observe;
			this._overTime = 0f;
			this.FightOverJust = false;
			this.IsRunning = true;
			this.SpeedRate = 1;
			this._recvBuff = new byte[409600];
			this._buffRecvSize = 0u;
			this._totalRecvSize = 0u;
			this._pkgQueue = new Queue<object>(1000);
			this._maxFrameNo = 0u;
			this._isStreamEnd = false;
			this._thisTick = 0u;
			this._lastRequestTick = 0u;
			ResGlobalInfo resGlobalInfo = new ResGlobalInfo();
			if (GameDataMgr.svr2CltCfgDict.TryGetValue(215u, out resGlobalInfo))
			{
				this._observeDelayFrames = (uint)Mathf.Clamp((int)(resGlobalInfo.dwConfValue * 1000u / Singleton<FrameSynchr>.GetInstance().SvrFrameDelta), 0, 4500);
			}
			else
			{
				this._observeDelayFrames = 900u;
			}
			this._tgw = inTGW;
			if (this._tgw != null)
			{
				NetworkModule.InitRelayConnnecting(this._tgw);
			}
			this.RequestVideoPiece(true);
			return true;
		}

		public void StartReplay(string replayPath)
		{
			if (this.IsWatching || Singleton<BattleLogic>.instance.isRuning)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Last_match_isnot_clean"), false, 1.5f, null, new object[0]);
				return;
			}
			if (Singleton<GameReplayModule>.GetInstance().BeginReplay(replayPath))
			{
				this._workMode = WatchController.WorkMode.Replay;
				this._overTime = 0f;
				this.FightOverJust = false;
				this.IsRunning = true;
				this.SpeedRate = 1;
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Can not play file: " + replayPath, false, 2f, null, new object[0]);
			}
			this._tgw = null;
		}

		public void StartJudge(COMDT_TGWINFO inTGW)
		{
			if (this.IsWatching || Singleton<BattleLogic>.instance.isRuning)
			{
				return;
			}
			this._workMode = WatchController.WorkMode.Judge;
			this.IsRunning = true;
			this.FightOverJust = false;
			this._tgw = inTGW;
			if (this._tgw != null)
			{
				NetworkModule.InitRelayConnnecting(this._tgw);
			}
		}

		public void Stop()
		{
			if (this._workMode == WatchController.WorkMode.Replay)
			{
				Singleton<GameReplayModule>.GetInstance().StopReplay();
			}
			this.SpeedRate = 1;
			this._isRunning = false;
			this._recvBuff = null;
			this._pkgQueue = null;
			this._overTime = 0f;
			this.TargetUID = 0uL;
			this._workMode = WatchController.WorkMode.None;
			this.CloseConnect();
		}

		public bool CoversCamp(COM_PLAYERCAMP inCamp)
		{
			if (this.IsWatching)
			{
				return this.observeCamp_ == inCamp || this.observeCamp_ == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT;
			}
			return this.observeCamp_ == inCamp;
		}

		public void SwitchObserveCamp(COM_PLAYERCAMP inObserveCamp)
		{
			if (this.IsWatching)
			{
				this.observeCamp_ = inObserveCamp;
				if (FogOfWar.enable)
				{
					GameFowManager.ResetHostCamp(inObserveCamp);
					Singleton<GameFowManager>.instance.ResetBaseMapData(true);
				}
			}
		}

		public void InitHostCamp()
		{
			this.observeCamp_ = Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
		}

		public bool CanShowActorIRPosMap()
		{
			return false;
		}

		private void SendPackage(CSPkg pkg)
		{
			if (this._tgw != null)
			{
				Singleton<NetworkModule>.GetInstance().SendGameMsg(ref pkg, 0u);
			}
			else
			{
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref pkg, false);
			}
		}

		private void CloseConnect()
		{
			if (this._tgw != null)
			{
				Singleton<NetworkModule>.GetInstance().CloseGameServerConnect(false);
				this._tgw = null;
			}
		}

		private void RequestVideoPiece(bool isNewPiece)
		{
			if (this._workMode != WatchController.WorkMode.Observe || this._isStreamEnd)
			{
				return;
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5228u);
			if (isNewPiece)
			{
				cSPkg.stPkgData.stGetVideoFrapsReq.dwOffset = this._totalRecvSize;
				cSPkg.stPkgData.stGetVideoFrapsReq.bNew = 1;
			}
			else
			{
				cSPkg.stPkgData.stGetVideoFrapsReq.bNew = 0;
			}
			this.SendPackage(cSPkg);
			this._lastRequestTick = this._thisTick;
		}

		private void HandleVideoPiece(SCPKG_GET_VIDEOFRAPS_RSP pkg)
		{
			if (this._workMode != WatchController.WorkMode.Observe)
			{
				return;
			}
			if (pkg.dwOffset != this._totalRecvSize)
			{
				return;
			}
			if (pkg.bAllOver == 1)
			{
				this._isStreamEnd = true;
				this.CloseConnect();
			}
			if (pkg.dwTotalNum > 0u)
			{
				uint dwBufLen = pkg.dwBufLen;
				if (dwBufLen > 0u)
				{
					int num = (int)(this._buffRecvSize + dwBufLen);
					if (num > this._recvBuff.Length)
					{
						Array.Resize<byte>(ref this._recvBuff, num);
					}
					Buffer.BlockCopy(pkg.szBuf, 0, this._recvBuff, (int)this._buffRecvSize, (int)dwBufLen);
					this._buffRecvSize = (uint)num;
					this._totalRecvSize += dwBufLen;
					if (pkg.dwThisPos + 1u < pkg.dwTotalNum)
					{
						this.RequestVideoPiece(false);
					}
					else
					{
						this.ParseVideoPackage();
					}
				}
			}
		}

		private void ParseVideoPackage()
		{
			while (this._buffRecvSize > 0u)
			{
				int num = 0;
				CSPkg cSPkg = CSPkg.New();
				if (cSPkg.unpack(ref this._recvBuff, (int)this._buffRecvSize, ref num, 0u) != TdrError.ErrorType.TDR_NO_ERROR || num <= 0 || num > (int)this._buffRecvSize)
				{
					break;
				}
				if (cSPkg.stPkgHead.dwMsgID == 1035u)
				{
					SCPKG_FRAPBOOTINFO stFrapBootInfo = cSPkg.stPkgData.stFrapBootInfo;
					for (int i = 0; i < (int)stFrapBootInfo.bSpareNum; i++)
					{
						SCPKG_FRAPBOOT_SINGLE sCPKG_FRAPBOOT_SINGLE = stFrapBootInfo.astSpareFrap[i];
						CSDT_FRAPBOOT_INFO cSDT_FRAPBOOT_INFO = CSDT_FRAPBOOT_INFO.New();
						int num2 = 0;
						if (cSDT_FRAPBOOT_INFO.unpack(ref sCPKG_FRAPBOOT_SINGLE.szInfoBuff, (int)sCPKG_FRAPBOOT_SINGLE.wLen, ref num2, 0u) == TdrError.ErrorType.TDR_NO_ERROR && num2 > 0)
						{
							this._maxFrameNo = cSDT_FRAPBOOT_INFO.dwKFrapsNo;
							this._pkgQueue.Enqueue(cSDT_FRAPBOOT_INFO);
						}
					}
				}
				else
				{
					this._pkgQueue.Enqueue(cSPkg);
				}
				this._buffRecvSize -= (uint)num;
				Buffer.BlockCopy(this._recvBuff, num, this._recvBuff, 0, (int)this._buffRecvSize);
			}
		}

		public void UpdateFrame()
		{
			if (!this.IsWatching)
			{
				return;
			}
			this._thisTick += 1u;
			if (this._overTime > 0f)
			{
				if (Time.time > this._overTime + 10f)
				{
					this.Stop();
					return;
				}
			}
			else
			{
				bool flag = false;
				if (this._workMode == WatchController.WorkMode.Observe)
				{
					flag = (this._isStreamEnd && this._pkgQueue.get_Count() == 0);
				}
				else if (this._workMode == WatchController.WorkMode.Replay)
				{
					flag = Singleton<GameReplayModule>.GetInstance().IsStreamEnd;
				}
				if (flag && Singleton<FrameSynchr>.GetInstance().CurFrameNum >= Singleton<FrameSynchr>.GetInstance().EndFrameNum)
				{
					this.MarkOver(false);
					return;
				}
			}
			if (this._workMode == WatchController.WorkMode.Observe)
			{
				if (!this._isStreamEnd && this._thisTick > this._lastRequestTick + this._requestTickSpan)
				{
					this.RequestVideoPiece(true);
				}
				if (this._isRunning && this._pkgQueue.get_Count() > 0)
				{
					this.HandlePackage();
				}
			}
			else if (this._workMode == WatchController.WorkMode.Replay && this.SpeedRate > 1 && Singleton<FrameSynchr>.GetInstance().CurFrameNum >= Singleton<FrameSynchr>.GetInstance().EndFrameNum && Singleton<GameReplayModule>.GetInstance().IsStreamEnd)
			{
				this.SpeedRate = 1;
			}
		}

		public void Quit()
		{
			if (this._workMode == WatchController.WorkMode.Observe)
			{
				if (this._isStreamEnd)
				{
					this.Stop();
				}
				else
				{
					CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5230u);
					cSPkg.stPkgData.stQuitObserveReq.bReserve = 0;
					Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
				}
			}
			else if (this._workMode == WatchController.WorkMode.Judge)
			{
				CSPkg cSPkg2 = NetworkModule.CreateDefaultCSPKG(5230u);
				cSPkg2.stPkgData.stQuitObserveReq.bReserve = 0;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg2, false);
			}
			else
			{
				this.Stop();
			}
		}

		private void HandleQuitResponse(SCPKG_QUITOBSERVE_RSP pkg)
		{
			if (!this.IsWatching)
			{
				return;
			}
			this.Stop();
		}

		private void HandlePackage()
		{
			uint curFrameNum = Singleton<FrameSynchr>.GetInstance().CurFrameNum;
			while (!MonoSingleton<GameLoader>.GetInstance().isLoadStart && this._pkgQueue.get_Count() > 0)
			{
				object obj = this._pkgQueue.Peek();
				CSDT_FRAPBOOT_INFO cSDT_FRAPBOOT_INFO = obj as CSDT_FRAPBOOT_INFO;
				if (cSDT_FRAPBOOT_INFO != null)
				{
					if (cSDT_FRAPBOOT_INFO.dwKFrapsNo > this.EndFrameNo || cSDT_FRAPBOOT_INFO.dwKFrapsNo <= curFrameNum || cSDT_FRAPBOOT_INFO.dwKFrapsNo > curFrameNum + 60u)
					{
						break;
					}
					this._pkgQueue.Dequeue();
					FrameWindow.HandleFraqBootInfo(cSDT_FRAPBOOT_INFO);
				}
				else
				{
					this._pkgQueue.Dequeue();
					CSPkg cSPkg = obj as CSPkg;
					NetMsgDelegate msgHandler = Singleton<NetworkModule>.GetInstance().GetMsgHandler(cSPkg.stPkgHead.dwMsgID);
					if (msgHandler != null)
					{
						try
						{
							Singleton<GameReplayModule>.instance.CacheRecord(cSPkg);
							msgHandler(cSPkg);
						}
						catch (Exception var_5_BA)
						{
						}
					}
				}
			}
			if (this.SpeedRate > 1 && this.EndFrameNo < curFrameNum + 120u)
			{
				this.SpeedRate = 1;
			}
		}

		public void MarkOver(bool fightOver)
		{
			if (this.IsWatching)
			{
				this._overTime = Time.time;
				this.FightOverJust = fightOver;
			}
		}

		[MessageHandler(5229)]
		public static void OnGetVideoFraqRsp(CSPkg msg)
		{
			Singleton<WatchController>.GetInstance().HandleVideoPiece(msg.stPkgData.stGetVideoFrapsRsp);
		}

		[MessageHandler(5231)]
		public static void OnQuitObserveRsp(CSPkg msg)
		{
			Singleton<WatchController>.GetInstance().HandleQuitResponse(msg.stPkgData.stQuitObserveRsp);
		}
	}
}
