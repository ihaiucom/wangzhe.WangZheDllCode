using Apollo;
using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using tsf4g_tdr_csharp;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	[MessageHandlerClass]
	public class Reconnection : MonoSingleton<Reconnection>, IGameModule
	{
		private const float WAIT_VIDEO_TIME_OUT = 15f;

		private ListView<CSPkg> ms_laterPkgList = new ListView<CSPkg>();

		private ListView<CSPkg> ms_cachePkgList = new ListView<CSPkg>();

		private int nRecvBuffSize = 409600;

		private int nRecvByteSize;

		private byte[] szRecvBuffer = new byte[409600];

		private int ms_cachedPkgLen;

		private uint ms_recvVideoPieceIdx;

		private uint ms_nVideoPieceSeq;

		private bool ms_bWaitingRelaySync;

		private float ms_fWaitVideoTimeout;

		private bool ms_bDealRelayCachePkg;

		private uint ms_nRelayCacheEndFrameNum;

		private bool ms_bProcessingRelaySync;

		private bool ms_bChaseupGameFrames;

		private bool m_bShouldSpin = true;

		private bool m_bUpdateReconnect;

		public float g_fBeginReconnectTime = -1f;

		public bool shouldReconnect
		{
			get
			{
				return this.m_bShouldSpin;
			}
		}

		public bool isProcessingRelaySync
		{
			get
			{
				return this.ms_bWaitingRelaySync || this.ms_bProcessingRelaySync;
			}
		}

		public bool isProcessingRelayRecover
		{
			get
			{
				return this.ms_bWaitingRelaySync || this.ms_bProcessingRelaySync || this.ms_bChaseupGameFrames;
			}
		}

		public bool isExcuteCacheMsgData
		{
			get
			{
				return this.ms_bDealRelayCachePkg;
			}
		}

		protected override void Init()
		{
			Singleton<CUICommonSystem>.GetInstance();
		}

		public void ResetRelaySyncCache()
		{
			this.nRecvByteSize = 0;
			this.ms_cachedPkgLen = 0;
			this.ms_recvVideoPieceIdx = 0u;
			this.ms_nVideoPieceSeq = 0u;
			this.ms_bWaitingRelaySync = false;
			this.ms_fWaitVideoTimeout = 0f;
			this.ms_bDealRelayCachePkg = false;
			this.ms_bProcessingRelaySync = false;
			this.ms_bChaseupGameFrames = false;
			this.m_bShouldSpin = true;
			this.m_bUpdateReconnect = true;
			this.ms_nRelayCacheEndFrameNum = 0u;
			base.StopCoroutine("ProcessRelaySyncCache");
			this.ms_cachePkgList.Clear();
			this.ms_laterPkgList.Clear();
		}

		public bool FilterRelaySvrPackage(CSPkg msg)
		{
			if (msg.stPkgHead.dwMsgID == 1082u)
			{
				this.m_bShouldSpin = false;
			}
			if (msg.stPkgHead.dwMsgID == 1092u)
			{
				this.onReconnectGame(msg);
				return true;
			}
			if (this.isProcessingRelaySync)
			{
				if (msg.stPkgHead.dwMsgID == 1091u)
				{
					this.onRelaySyncCacheFrames(msg);
				}
				else
				{
					this.ms_laterPkgList.Add(msg);
				}
				return true;
			}
			return false;
		}

		private void AddCachePkg(CSPkg msg)
		{
			DebugHelper.Assert(msg != null);
			if (!this.ms_bDealRelayCachePkg)
			{
				this.ms_cachePkgList.Add(msg);
			}
			else
			{
				msg.Release();
			}
		}

		public void UpdateCachedLen(CSPkg msg)
		{
			DebugHelper.Assert(msg != null);
			if ((ulong)msg.stPkgHead.dwReserve > (ulong)((long)this.ms_cachedPkgLen))
			{
				this.ms_cachedPkgLen = (int)msg.stPkgHead.dwReserve;
			}
		}

		private void onReconnectGame(CSPkg msg)
		{
			switch (msg.stPkgData.stReconnGameNtf.bState)
			{
			case 1:
				this.HeroSelectReconectBanStep(msg.stPkgData.stReconnGameNtf.stStateData.stBanInfo);
				Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
				break;
			case 2:
				this.HeroSelectReconectPickStep(msg.stPkgData.stReconnGameNtf.stStateData.stPickInfo);
				Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
				break;
			case 3:
				this.HeroSelectReconectSwapStep(msg.stPkgData.stReconnGameNtf.stStateData.stAdjustInfo);
				Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
				break;
			case 4:
			{
				if (Singleton<LobbyLogic>.instance.inMultiGame)
				{
					return;
				}
				Singleton<LobbyLogic>.instance.inMultiGame = true;
				Singleton<LobbyLogic>.instance.inMultiRoom = true;
				SCPKG_MULTGAME_BEGINLOAD stBeginLoad = msg.stPkgData.stReconnGameNtf.stStateData.stLoadingInfo.stBeginLoad;
				Singleton<GameBuilder>.instance.StartGame(new MultiGameContext(stBeginLoad));
				Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
				break;
			}
			case 5:
				this.g_fBeginReconnectTime = Time.time;
				Singleton<LobbyLogic>.GetInstance().inMultiRoom = true;
				Singleton<LobbyLogic>.GetInstance().reconnGameInfo = msg.stPkgData.stReconnGameNtf.stStateData.stGamingInfo;
				MonoSingleton<VoiceSys>.instance.SyncReconnectData(Singleton<LobbyLogic>.GetInstance().reconnGameInfo);
				MonoSingleton<Reconnection>.GetInstance().RequestRelaySyncCacheFrames(false);
				break;
			case 6:
				MonoSingleton<Reconnection>.GetInstance().ExitMultiGame();
				break;
			default:
				DebugHelper.Assert(false);
				break;
			}
		}

		private void HeroSelectReconectBanStep(CSDT_RECONN_BANINFO banInfo)
		{
			Singleton<GameStateCtrl>.GetInstance().GotoState("LobbyState");
			Singleton<CRoomSystem>.instance.SetRoomType(1);
			CSDT_CAMPINFO[] array = new CSDT_CAMPINFO[banInfo.astCampInfo.Length];
			for (int i = 0; i < banInfo.astCampInfo.Length; i++)
			{
				array[i] = new CSDT_CAMPINFO();
				array[i].dwPlayerNum = banInfo.astCampInfo[i].dwPlayerNum;
				array[i].astCampPlayerInfo = new CSDT_CAMPPLAYERINFO[banInfo.astCampInfo[i].astCampPlayerInfo.Length];
				for (int j = 0; j < banInfo.astCampInfo[i].astCampPlayerInfo.Length; j++)
				{
					array[i].astCampPlayerInfo[j] = banInfo.astCampInfo[i].astCampPlayerInfo[j];
				}
			}
			CHeroSelectBaseSystem.StartPvpHeroSelectSystem(banInfo.stDeskInfo, array, banInfo.stFreeHero, banInfo.stFreeHeroSymbol);
			Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep = enBanPickStep.enBan;
			if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enBanPick)
			{
				MemberInfo masterMemberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMasterMemberInfo();
				if (masterMemberInfo == null)
				{
					return;
				}
				Singleton<CHeroSelectBaseSystem>.instance.AddBanHero(COM_PLAYERCAMP.COM_PLAYERCAMP_1, banInfo.stStateInfo.Camp1BanList);
				Singleton<CHeroSelectBaseSystem>.instance.AddBanHero(COM_PLAYERCAMP.COM_PLAYERCAMP_2, banInfo.stStateInfo.Camp2BanList);
				Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState = banInfo.stStateInfo.stCurState;
				Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stNextState = banInfo.stStateInfo.stNextState;
				Singleton<CHeroSelectBaseSystem>.instance.m_banHeroTeamMaxCount = (int)banInfo.stStateInfo.bBanPosNum;
				Singleton<CHeroSelectBanPickSystem>.instance.InitMenu(false);
				Singleton<CHeroSelectBanPickSystem>.instance.RefreshAll();
				Singleton<CHeroSelectBanPickSystem>.instance.PlayStepTitleAnimation();
				Singleton<CHeroSelectBanPickSystem>.instance.PlayCurrentBgAnimation();
				if (Singleton<CHeroSelectBaseSystem>.instance.IsCurBanOrPickMember(masterMemberInfo))
				{
					Utility.VibrateHelper();
					Singleton<CSoundManager>.GetInstance().PostEvent("UI_MyTurn", null);
					Singleton<CSoundManager>.GetInstance().PostEvent("Play_sys_ban_3", null);
				}
				else if (Singleton<CHeroSelectBaseSystem>.instance.IsCurOpByCamp(masterMemberInfo))
				{
					Singleton<CSoundManager>.GetInstance().PostEvent("Play_sys_ban_2", null);
				}
				else if (masterMemberInfo.camp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
				{
					Singleton<CSoundManager>.GetInstance().PostEvent("Play_sys_ban_1", null);
				}
				Singleton<CSoundManager>.GetInstance().PostEvent("Play_Music_BanPick", null);
			}
		}

		private void HeroSelectReconectPickStep(CSDT_RECONN_PICKINFO pickInfo)
		{
			Singleton<GameStateCtrl>.GetInstance().GotoState("LobbyState");
			Singleton<CRoomSystem>.instance.SetRoomType(1);
			CSDT_CAMPINFO[] array = new CSDT_CAMPINFO[pickInfo.astCampInfo.Length];
			for (int i = 0; i < pickInfo.astCampInfo.Length; i++)
			{
				array[i] = new CSDT_CAMPINFO();
				array[i].dwPlayerNum = pickInfo.astCampInfo[i].dwPlayerNum;
				array[i].astCampPlayerInfo = new CSDT_CAMPPLAYERINFO[pickInfo.astCampInfo[i].astPlayerInfo.Length];
				for (int j = 0; j < pickInfo.astCampInfo[i].astPlayerInfo.Length; j++)
				{
					array[i].astCampPlayerInfo[j] = pickInfo.astCampInfo[i].astPlayerInfo[j].stPickHeroInfo;
				}
			}
			CHeroSelectBaseSystem.StartPvpHeroSelectSystem(pickInfo.stDeskInfo, array, pickInfo.stFreeHero, pickInfo.stFreeHeroSymbol);
			Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep = enBanPickStep.enPick;
			for (int k = 0; k < pickInfo.astCampInfo.Length; k++)
			{
				for (int l = 0; l < pickInfo.astCampInfo[k].astPlayerInfo.Length; l++)
				{
					uint dwObjId = pickInfo.astCampInfo[k].astPlayerInfo[l].stPickHeroInfo.stPlayerInfo.dwObjId;
					byte bIsPickOK = pickInfo.astCampInfo[k].astPlayerInfo[l].bIsPickOK;
					MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo(dwObjId);
					if (memberInfo != null)
					{
						memberInfo.isPrepare = (bIsPickOK == 1);
					}
				}
			}
			MemberInfo masterMemberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMasterMemberInfo();
			if (masterMemberInfo != null)
			{
				Singleton<CHeroSelectBaseSystem>.instance.SetPvpHeroSelect(masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
				if (masterMemberInfo.isPrepare)
				{
					Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm = true;
				}
				if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
				{
					Singleton<CHeroSelectNormalSystem>.GetInstance().m_showHeroID = masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
					if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enRandom)
					{
						Singleton<CHeroSelectNormalSystem>.instance.SwitchSkinMenuSelect(1);
					}
					Singleton<CHeroSelectNormalSystem>.GetInstance().RefreshHeroPanel(true, true);
				}
				else if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enBanPick)
				{
					CSDT_RECONN_BAN_PICK_STATE_INFO stBanPickInfo = pickInfo.stPickStateExtra.stPickDetail.stBanPickInfo;
					if (stBanPickInfo == null)
					{
						return;
					}
					Singleton<CHeroSelectBaseSystem>.instance.AddBanHero(COM_PLAYERCAMP.COM_PLAYERCAMP_1, stBanPickInfo.Camp1BanList);
					Singleton<CHeroSelectBaseSystem>.instance.AddBanHero(COM_PLAYERCAMP.COM_PLAYERCAMP_2, stBanPickInfo.Camp2BanList);
					Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState = stBanPickInfo.stCurState;
					Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stNextState = stBanPickInfo.stNextState;
					Singleton<CHeroSelectBaseSystem>.instance.m_banHeroTeamMaxCount = (int)stBanPickInfo.bBanPosNum;
					Singleton<CHeroSelectBanPickSystem>.instance.InitMenu(false);
					Singleton<CHeroSelectBanPickSystem>.instance.RefreshAll();
					Singleton<CHeroSelectBanPickSystem>.instance.PlayStepTitleAnimation();
					Singleton<CHeroSelectBanPickSystem>.instance.PlayCurrentBgAnimation();
					if (Singleton<CHeroSelectBaseSystem>.instance.IsCurBanOrPickMember(masterMemberInfo))
					{
						Utility.VibrateHelper();
						Singleton<CSoundManager>.GetInstance().PostEvent("UI_MyTurn", null);
						Singleton<CSoundManager>.GetInstance().PostEvent("Play_sys_ban_4", null);
						Singleton<CSoundManager>.GetInstance().PostEvent("Set_Segment2", null);
					}
					else
					{
						if (Singleton<CHeroSelectBaseSystem>.instance.IsCurOpByCamp(masterMemberInfo))
						{
							Singleton<CSoundManager>.GetInstance().PostEvent("Play_sys_ban_7", null);
						}
						else if (masterMemberInfo.camp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
						{
							Singleton<CSoundManager>.GetInstance().PostEvent("Play_sys_ban_6", null);
						}
						Singleton<CSoundManager>.GetInstance().PostEvent("Set_Segment1", null);
					}
				}
				return;
			}
		}

		private void HeroSelectReconectSwapStep(CSDT_RECONN_ADJUSTINFO swapInfo)
		{
			Singleton<GameStateCtrl>.GetInstance().GotoState("LobbyState");
			Singleton<CRoomSystem>.instance.SetRoomType(1);
			CSDT_CAMPINFO[] array = new CSDT_CAMPINFO[swapInfo.astCampInfo.Length];
			for (int i = 0; i < swapInfo.astCampInfo.Length; i++)
			{
				array[i] = new CSDT_CAMPINFO();
				array[i].dwPlayerNum = swapInfo.astCampInfo[i].dwPlayerNum;
				array[i].astCampPlayerInfo = new CSDT_CAMPPLAYERINFO[swapInfo.astCampInfo[i].astPlayerInfo.Length];
				for (int j = 0; j < swapInfo.astCampInfo[i].astPlayerInfo.Length; j++)
				{
					array[i].astCampPlayerInfo[j] = swapInfo.astCampInfo[i].astPlayerInfo[j].stPickHeroInfo;
				}
			}
			CHeroSelectBaseSystem.StartPvpHeroSelectSystem(swapInfo.stDeskInfo, array, swapInfo.stFreeHero, swapInfo.stFreeHeroSymbol);
			Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep = enBanPickStep.enSwap;
			for (int k = 0; k < swapInfo.astCampInfo.Length; k++)
			{
				for (int l = 0; l < swapInfo.astCampInfo[k].astPlayerInfo.Length; l++)
				{
					uint dwObjId = swapInfo.astCampInfo[k].astPlayerInfo[l].stPickHeroInfo.stPlayerInfo.dwObjId;
					byte bIsPickOK = swapInfo.astCampInfo[k].astPlayerInfo[l].bIsPickOK;
					MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo(dwObjId);
					if (memberInfo != null)
					{
						memberInfo.isPrepare = (bIsPickOK == 1);
					}
				}
			}
			MemberInfo masterMemberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMasterMemberInfo();
			if (masterMemberInfo != null)
			{
				Singleton<CHeroSelectBaseSystem>.instance.SetPvpHeroSelect(masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
				if (masterMemberInfo.isPrepare)
				{
					Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm = true;
				}
				if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
				{
					Singleton<CHeroSelectNormalSystem>.GetInstance().m_showHeroID = masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
					if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enRandom)
					{
						Singleton<CHeroSelectNormalSystem>.instance.SwitchSkinMenuSelect(1);
					}
					Singleton<CHeroSelectNormalSystem>.instance.RefreshHeroPanel(false, true);
					Singleton<CHeroSelectNormalSystem>.instance.StartEndTimer((int)(swapInfo.dwLeftMs / 1000u));
				}
				else if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enBanPick)
				{
					Singleton<CHeroSelectBaseSystem>.instance.AddBanHero(COM_PLAYERCAMP.COM_PLAYERCAMP_1, swapInfo.stHeroSwapInfo.stPickDetail.stHeroSwapInfo.Camp1BanList);
					Singleton<CHeroSelectBaseSystem>.instance.AddBanHero(COM_PLAYERCAMP.COM_PLAYERCAMP_2, swapInfo.stHeroSwapInfo.stPickDetail.stHeroSwapInfo.Camp2BanList);
					Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo.dwActiveObjID = swapInfo.stHeroSwapInfo.stPickDetail.stHeroSwapInfo.dwActiveObjID;
					Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo.dwPassiveObjID = swapInfo.stHeroSwapInfo.stPickDetail.stHeroSwapInfo.dwPassiveObjID;
					Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo.iErrCode = 0;
					Singleton<CHeroSelectBaseSystem>.instance.m_banHeroTeamMaxCount = (int)swapInfo.stHeroSwapInfo.stPickDetail.stHeroSwapInfo.bBanPosNum;
					if (masterMemberInfo.dwObjId == Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo.dwActiveObjID)
					{
						Singleton<CHeroSelectBaseSystem>.instance.m_swapState = enSwapHeroState.enReqing;
					}
					else if (masterMemberInfo.dwObjId == Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo.dwPassiveObjID)
					{
						Singleton<CHeroSelectBaseSystem>.instance.m_swapState = enSwapHeroState.enSwapAllow;
					}
					Singleton<CHeroSelectBanPickSystem>.instance.InitMenu(false);
					Singleton<CHeroSelectBanPickSystem>.instance.RefreshAll();
					Singleton<CHeroSelectBanPickSystem>.instance.StartEndTimer((int)(swapInfo.dwLeftMs / 1000u));
					Singleton<CSoundManager>.GetInstance().PostEvent("Set_BanPickEnd", null);
				}
				return;
			}
		}

		private void onRelaySyncCacheFrames(CSPkg inFrapMsg)
		{
			DebugHelper.Assert(this.ms_bWaitingRelaySync);
			if (!this.ms_bWaitingRelaySync || inFrapMsg.stPkgData.stRecoverFrapRsp.dwCltSeq != this.ms_nVideoPieceSeq)
			{
				return;
			}
			if (inFrapMsg.stPkgData.stRecoverFrapRsp.dwThisPos != this.ms_recvVideoPieceIdx)
			{
				this.RequestRelaySyncCacheFrames(true);
				return;
			}
			this.ms_recvVideoPieceIdx += 1u;
			if (inFrapMsg.stPkgData.stRecoverFrapRsp.dwBufLen > 0u)
			{
				this.CacheFramesData(inFrapMsg);
			}
			if (inFrapMsg.stPkgData.stRecoverFrapRsp.dwTotalNum == this.ms_recvVideoPieceIdx)
			{
				this.ParseFramesData();
				DebugHelper.Assert(this.nRecvByteSize == 0);
				this.ms_bWaitingRelaySync = false;
				this.ms_bProcessingRelaySync = true;
				this.ms_nRelayCacheEndFrameNum = inFrapMsg.stPkgData.stRecoverFrapRsp.dwCurKFrapsNo;
				base.StartCoroutine("ProcessRelaySyncCache");
			}
			else
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1090u);
				cSPkg.stPkgData.stRecoverFrapReq.bIsNew = 0;
				Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
				this.ms_fWaitVideoTimeout = 15f;
			}
		}

		private void ExitMultiGame()
		{
			this.m_bShouldSpin = false;
			this.m_bUpdateReconnect = false;
			this.ms_bChaseupGameFrames = false;
			Singleton<GameBuilder>.instance.EndGame();
			Singleton<CUIManager>.instance.CloseAllFormExceptLobby(true);
		}

        private IEnumerator ProcessRelaySyncCache()
        {
            ms_bDealRelayCachePkg = true;
            var i = 0;
            while (i < ms_cachePkgList.Count)
            {
                var msg = ms_cachePkgList[i];
                while (MonoSingleton<GameLoader>.instance.isLoadStart)
                {
                    yield return new WaitForEndOfFrame();
                }

                Singleton<NetworkModule>.instance.gameSvr.HandleMsg(msg);
                if (Singleton<NetworkModule>.instance.IsNeedSendGamePing())
                {
                    Singleton<NetworkModule>.instance.UpdateFrame();
                }
                i++;
            }
            ms_bDealRelayCachePkg = false;
            ms_cachePkgList.Clear();
            var j = 0;
            while (j < ms_laterPkgList.Count)
            {
                var msg = ms_laterPkgList[j];
                Singleton<NetworkModule>.instance.gameSvr.HandleMsg(msg);
                if (Singleton<NetworkModule>.instance.IsNeedSendGamePing())
                {
                    Singleton<NetworkModule>.instance.UpdateFrame();
                }
                j++;
            }
            ms_laterPkgList.Clear();
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            ms_bChaseupGameFrames = true;
            ms_bProcessingRelaySync = false;
        }

		private void CacheFramesData(CSPkg inFrapMsg)
		{
			int dwBufLen = (int)inFrapMsg.stPkgData.stRecoverFrapRsp.dwBufLen;
			byte[] array = new byte[dwBufLen];
			Buffer.BlockCopy(inFrapMsg.stPkgData.stRecoverFrapRsp.szBuf, 0, array, 0, dwBufLen);
			if (this.nRecvByteSize + array.Length > this.nRecvBuffSize)
			{
				Array.Resize<byte>(ref this.szRecvBuffer, this.nRecvByteSize + array.Length);
				Buffer.BlockCopy(array, 0, this.szRecvBuffer, this.nRecvByteSize, array.Length);
				this.nRecvBuffSize = this.nRecvByteSize + array.Length;
				this.nRecvByteSize = this.nRecvBuffSize;
			}
			else
			{
				Buffer.BlockCopy(array, 0, this.szRecvBuffer, this.nRecvByteSize, array.Length);
				this.nRecvByteSize += array.Length;
			}
		}

		private void ParseFramesData()
		{
			try
			{
				while (this.nRecvByteSize > 0)
				{
					int num = 0;
					CSPkg cSPkg = CSPkg.New();
					if (cSPkg.unpack(ref this.szRecvBuffer, this.nRecvByteSize, ref num, 0u) != TdrError.ErrorType.TDR_NO_ERROR || num <= 0)
					{
						break;
					}
					Buffer.BlockCopy(this.szRecvBuffer, num, this.szRecvBuffer, 0, this.nRecvByteSize - num);
					this.nRecvByteSize -= num;
					this.AddCachePkg(cSPkg);
				}
			}
			catch (Exception ex)
			{
				BugLocateLogSys.Log("ParseFramesCacheData " + ex.Message);
			}
		}

		public bool RequestRelaySyncCacheFrames(bool force = false)
		{
			if (Singleton<WatchController>.GetInstance().IsRelayCast)
			{
				return false;
			}
			if (this.isProcessingRelayRecover && !force)
			{
				return false;
			}
			base.StopCoroutine("ProcessRelaySyncCache");
			this.ms_cachePkgList.Clear();
			this.ms_laterPkgList.Clear();
			this.ms_nRelayCacheEndFrameNum = 0u;
			this.ms_bDealRelayCachePkg = false;
			this.ms_bProcessingRelaySync = false;
			this.ms_bChaseupGameFrames = false;
			this.m_bShouldSpin = true;
			this.m_bUpdateReconnect = true;
			this.ms_recvVideoPieceIdx = 0u;
			this.ms_nVideoPieceSeq = (uint)Time.frameCount;
			this.nRecvByteSize = 0;
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1090u);
			cSPkg.stPkgData.stRecoverFrapReq.dwCurLen = (uint)this.ms_cachedPkgLen;
			cSPkg.stPkgData.stRecoverFrapReq.bIsNew = 1;
			cSPkg.stPkgData.stRecoverFrapReq.dwCltSeq = this.ms_nVideoPieceSeq;
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
			this.ms_bWaitingRelaySync = true;
			this.ms_fWaitVideoTimeout = 15f;
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert("断线重连数据恢复中，嗷了个嗷...", 15, enUIEventID.None);
			return true;
		}

		public bool SendReconnectSucceeded()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1093u);
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
			return true;
		}

		public void UpdateReconnect()
		{
			if (Singleton<NetworkModule>.GetInstance().isOnlineMode && this.m_bShouldSpin && this.m_bUpdateReconnect)
			{
				Singleton<NetworkModule>.instance.gameSvr.Update();
			}
		}

		public void UpdateFrame()
		{
			if (this.ms_bChaseupGameFrames)
			{
				if (Singleton<FrameSynchr>.instance.EndFrameNum > 0u && Singleton<FrameSynchr>.instance.CurFrameNum > 0u)
				{
					CUILoadingSystem.OnSelfLoadProcess(0.99f * Singleton<FrameSynchr>.instance.CurFrameNum / Singleton<FrameSynchr>.instance.EndFrameNum);
				}
				this.ms_bChaseupGameFrames = (Singleton<FrameSynchr>.instance.CurFrameNum < Singleton<FrameSynchr>.instance.EndFrameNum - 15u);
				if (!this.ms_bChaseupGameFrames)
				{
					this.SendReconnectSucceeded();
					ProtocolObjectPool.Clear(50);
					Singleton<GameEventSys>.instance.SendEvent(GameEventDef.Event_MultiRecoverFin);
				}
			}
			if (this.ms_bWaitingRelaySync && this.ms_fWaitVideoTimeout > 0f)
			{
				this.ms_fWaitVideoTimeout -= Time.unscaledDeltaTime;
				if (this.ms_fWaitVideoTimeout <= 0f)
				{
					this.RequestRelaySyncCacheFrames(true);
				}
			}
		}

		private void PopConfirmingReconnecting()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/Common/Form_MessageBox.prefab");
			if (form == null)
			{
				this.m_bUpdateReconnect = false;
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel("重连服务器失败，请检测手机网络环境。", enUIEventID.Net_ReconnectConfirm, enUIEventID.Net_ReconnectCancel, false);
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Net_ReconnectConfirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmReconnecting));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Net_ReconnectCancel, new CUIEventManager.OnUIEventHandler(this.OnCancelReconnecting));
			}
		}

		private void OnConfirmReconnecting(CUIEvent uiEvent)
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Net_ReconnectConfirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmReconnecting));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Net_ReconnectCancel, new CUIEventManager.OnUIEventHandler(this.OnCancelReconnecting));
			this.m_bUpdateReconnect = true;
			Singleton<NetworkModule>.instance.gameSvr.ForceReconnect();
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert("手动重连游戏尝试...", 10, enUIEventID.None);
		}

		private void OnCancelReconnecting(CUIEvent uiEvent)
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Net_ReconnectConfirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmReconnecting));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Net_ReconnectCancel, new CUIEventManager.OnUIEventHandler(this.OnCancelReconnecting));
			this.ExitMultiGame();
		}

		public void PopMsgBoxConnectionClosed()
		{
			Singleton<CUIManager>.GetInstance().OpenMessageBox("游戏已经结束，点击确定返回游戏大厅。", enUIEventID.Net_ReconnectClosed, false);
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Net_ReconnectClosed, new CUIEventManager.OnUIEventHandler(this.OnConnectionClosedExitGame));
		}

		private void OnConnectionClosedExitGame(CUIEvent uiEvent)
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Net_ReconnectClosed, new CUIEventManager.OnUIEventHandler(this.OnConnectionClosedExitGame));
			this.ExitMultiGame();
		}

		public void QueryIsRelayGaming(ApolloResult result)
		{
			if (!this.m_bShouldSpin)
			{
				return;
			}
			if (result == ApolloResult.PeerStopSession || result == ApolloResult.PeerCloseConnection)
			{
				if (!Singleton<WatchController>.GetInstance().IsWatching || Singleton<WatchController>.GetInstance().IsLiveCast)
				{
					CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1060u);
					Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
				}
			}
			else if (result == ApolloResult.TokenInvalid)
			{
				Singleton<ApolloHelper>.GetInstance().Login(ApolloConfig.platform, true, 0uL, null);
				Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("AccessTokenExpired", null, true);
			}
			else
			{
				this.m_bUpdateReconnect = true;
			}
		}

		[MessageHandler(1061)]
		public static void onQueryIsRelayGaming(CSPkg msg)
		{
			if (!MonoSingleton<Reconnection>.instance.shouldReconnect || Singleton<WatchController>.GetInstance().IsRelayCast)
			{
				return;
			}
			if (msg.stPkgData.stAskInMultGameRsp.bYes != 0)
			{
				MonoSingleton<Reconnection>.instance.m_bUpdateReconnect = true;
			}
			else
			{
				MonoSingleton<Reconnection>.instance.PopMsgBoxConnectionClosed();
			}
		}

		public void OnConnectSuccess()
		{
		}

		public void ShowReconnectMsgAlert(int nCount, int nMax)
		{
			if (nCount > nMax)
			{
				if (nCount == nMax + 1)
				{
					MonoSingleton<Reconnection>.instance.PopConfirmingReconnecting();
				}
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(string.Format("自动重连 第[{0}/{1}]尝试...", nCount, nMax), 10, enUIEventID.None);
			}
		}
	}
}
