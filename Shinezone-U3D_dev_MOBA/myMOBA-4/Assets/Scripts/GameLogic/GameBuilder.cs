using AGE;
using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public sealed class GameBuilder : Singleton<GameBuilder>
	{
		public int m_iMapId;

		public COM_GAME_TYPE m_kGameType = COM_GAME_TYPE.COM_GAME_TYPE_MAX;

		private float m_fLoadingTime;

		private float m_fLoadProgress;

		private List<KeyValuePair<string, string>> m_eventsLoadingTime = new List<KeyValuePair<string, string>>();

		public GameInfoBase gameInfo
		{
			get;
			private set;
		}

		public GameInfoBase StartGame(GameContextBase InGameContext)
		{
			DebugHelper.Assert(InGameContext != null);
			if (InGameContext == null)
			{
				return null;
			}
			if (Singleton<BattleLogic>.instance.isRuning)
			{
				return null;
			}
			MultiGameContext multiGameContext = InGameContext as MultiGameContext;
			Singleton<GameInput>.GetInstance().isSlowUpMoveCmd = (multiGameContext != null && multiGameContext.MessageRef != null && multiGameContext.MessageRef.bIsSlowUP != 0);
			SynchrReport.Reset();
			GameSettings.DecideDynamicParticleLOD();
			MonoSingleton<TGPSDKSys>.GetInstance().EnablePhone(true);
			Singleton<CHeroSelectBaseSystem>.instance.m_fOpenHeroSelectForm = Time.time - Singleton<CHeroSelectBaseSystem>.instance.m_fOpenHeroSelectForm;
			this.m_fLoadingTime = Time.time;
			this.m_eventsLoadingTime.Clear();
			ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
			DebugHelper.Assert(accountInfo != null, "account info is null");
			this.m_iMapId = InGameContext.levelContext.m_mapID;
			this.m_kGameType = InGameContext.levelContext.GetGameType();
			this.m_eventsLoadingTime.Add(new KeyValuePair<string, string>("OpenID", (accountInfo == null) ? "0" : accountInfo.OpenId));
			this.m_eventsLoadingTime.Add(new KeyValuePair<string, string>("LevelID", InGameContext.levelContext.m_mapID.ToString()));
			this.m_eventsLoadingTime.Add(new KeyValuePair<string, string>("isPVPLevel", InGameContext.levelContext.IsMobaMode().ToString()));
			this.m_eventsLoadingTime.Add(new KeyValuePair<string, string>("isPVPMode", InGameContext.levelContext.IsMobaMode().ToString()));
			this.m_eventsLoadingTime.Add(new KeyValuePair<string, string>("bLevelNo", InGameContext.levelContext.m_levelNo.ToString()));
			Singleton<BattleLogic>.GetInstance().isRuning = true;
			Singleton<BattleLogic>.GetInstance().isFighting = false;
			Singleton<BattleLogic>.GetInstance().isGameOver = false;
			Singleton<BattleLogic>.GetInstance().isWaitMultiStart = false;
			ActionManager.Instance.frameMode = true;
			MonoSingleton<ActionManager>.GetInstance().ForceStop();
			Singleton<GameObjMgr>.GetInstance().ClearActor();
			Singleton<SceneManagement>.GetInstance().Clear();
			MonoSingleton<SceneMgr>.GetInstance().ClearAll();
			MonoSingleton<GameLoader>.GetInstance().ResetLoader();
			InGameContext.PrepareStartup();
			if (!MonoSingleton<GameFramework>.instance.EditorPreviewMode)
			{
				DebugHelper.Assert(InGameContext.levelContext != null);
				DebugHelper.Assert(!string.IsNullOrEmpty(InGameContext.levelContext.m_levelDesignFileName));
				if (string.IsNullOrEmpty(InGameContext.levelContext.m_levelArtistFileName))
				{
					MonoSingleton<GameLoader>.instance.AddLevel(InGameContext.levelContext.m_levelDesignFileName);
				}
				else
				{
					MonoSingleton<GameLoader>.instance.AddDesignSerializedLevel(InGameContext.levelContext.m_levelDesignFileName);
					MonoSingleton<GameLoader>.instance.AddArtistSerializedLevel(InGameContext.levelContext.m_levelArtistFileName);
				}
				MonoSingleton<GameLoader>.instance.AddSoundBank("Effect_Common");
				MonoSingleton<GameLoader>.instance.AddSoundBank("System_Voice");
			}
			GameInfoBase gameInfoBase = InGameContext.CreateGameInfo();
			DebugHelper.Assert(gameInfoBase != null, "can't create game logic object!");
			this.gameInfo = gameInfoBase;
			gameInfoBase.PreBeginPlay();
			Singleton<BattleLogic>.instance.m_LevelContext = this.gameInfo.gameContext.levelContext;
			try
			{
				DebugHelper.CustomLog("GameBuilder Start Game: ispvplevel={0} ispvpmode={4} levelid={1} leveltype={6} levelname={3} Gametype={2} pick={5}", new object[]
				{
					InGameContext.levelContext.IsMobaMode(),
					InGameContext.levelContext.m_mapID,
					InGameContext.levelContext.GetGameType(),
					InGameContext.levelContext.m_levelName,
					InGameContext.levelContext.IsMobaMode(),
					InGameContext.levelContext.GetSelectHeroType(),
					InGameContext.levelContext.m_pveLevelType
				});
				Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
				if (hostPlayer != null)
				{
					DebugHelper.CustomLog("HostPlayer player id={1} name={2} ", new object[]
					{
						hostPlayer.PlayerId,
						hostPlayer.Name
					});
				}
			}
			catch (Exception)
			{
			}
			if (!MonoSingleton<GameFramework>.instance.EditorPreviewMode)
			{
				this.m_fLoadProgress = 0f;
				MonoSingleton<GameLoader>.GetInstance().Load(new GameLoader.LoadProgressDelegate(this.onGameLoadProgress), new GameLoader.LoadCompleteDelegate(this.OnGameLoadComplete));
				MonoSingleton<VoiceSys>.GetInstance().HeroSelectTobattle();
				Singleton<GameStateCtrl>.GetInstance().GotoState("LoadingState");
			}
			return gameInfoBase;
		}

		public void EndGame()
		{
			if (!Singleton<BattleLogic>.instance.isRuning)
			{
				return;
			}
			try
			{
				DebugHelper.CustomLog("Prepare GameBuilder EndGame");
			}
			catch (Exception)
			{
			}

			MonoSingleton<TGPSDKSys>.GetInstance().EnablePhone(false);
			MonoSingleton<GSDKsys>.GetInstance().EndSpeed();
			Singleton<Assets.Scripts.Framework.GameLogic>.GetInstance().HashCheckFreq = 500u;
			Singleton<Assets.Scripts.Framework.GameLogic>.GetInstance().SnakeTraceMasks = 0u;
			Singleton<Assets.Scripts.Framework.GameLogic>.GetInstance().SnakeTraceSize = 1024000u;
			Singleton<LobbyLogic>.GetInstance().StopGameEndTimer();
			Singleton<LobbyLogic>.GetInstance().StopSettleMsgTimer();
			Singleton<LobbyLogic>.GetInstance().StopSettlePanelTimer();
			MonoSingleton<GameLoader>.instance.AdvanceStopLoad();
			Singleton<WatchController>.GetInstance().Stop();
			Singleton<FrameWindow>.GetInstance().ResetSendCmdSeq();
			Singleton<CBattleGuideManager>.GetInstance().resetPause();
			MonoSingleton<ShareSys>.instance.SendQQGameTeamStateChgMsg(ShareSys.QQGameTeamEventType.end, COM_ROOM_TYPE.COM_ROOM_TYPE_NULL, 0, 0u, string.Empty, 0u, 0u);
			Singleton<StarSystem>.GetInstance().EndGame();
			Singleton<WinLoseByStarSys>.GetInstance().EndGame();
			Singleton<CMatchingSystem>.GetInstance().EndGame();
			string openID = Singleton<ApolloHelper>.GetInstance().GetOpenID();
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
			list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
			list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
			list.Add(new KeyValuePair<string, string>("openid", openID));
			list.Add(new KeyValuePair<string, string>("GameType", this.m_kGameType.ToString()));
			list.Add(new KeyValuePair<string, string>("MapID", this.m_iMapId.ToString()));
			list.Add(new KeyValuePair<string, string>("LoadingTime", this.m_fLoadingTime.ToString()));
			Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_LoadingBattle", list, true);
			List<KeyValuePair<string, string>> list2 = new List<KeyValuePair<string, string>>();
			list2.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
			list2.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
			list2.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
			list2.Add(new KeyValuePair<string, string>("openid", openID));
			list2.Add(new KeyValuePair<string, string>("totaltime", Singleton<CHeroSelectBaseSystem>.instance.m_fOpenHeroSelectForm.ToString()));
			list2.Add(new KeyValuePair<string, string>("gameType", this.m_kGameType.ToString()));
			list2.Add(new KeyValuePair<string, string>("role_list", string.Empty));
			list2.Add(new KeyValuePair<string, string>("errorCode", string.Empty));
			list2.Add(new KeyValuePair<string, string>("error_msg", string.Empty));
			Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Login_EnterGame", list2, true);
			float num = (float)Singleton<DataReportSys>.GetInstance().GameTime;
			List<KeyValuePair<string, string>> list3 = new List<KeyValuePair<string, string>>();
			list3.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
			list3.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
			list3.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().GetPlatformStr()));
			list3.Add(new KeyValuePair<string, string>("openid", openID));
			list3.Add(new KeyValuePair<string, string>("GameType", this.m_kGameType.ToString()));
			list3.Add(new KeyValuePair<string, string>("MapID", this.m_iMapId.ToString()));
			list3.Add(new KeyValuePair<string, string>("Battle_Time", num.ToString()));
			list3.Add(new KeyValuePair<string, string>("music", GameSettings.EnableMusic.ToString()));
			list3.Add(new KeyValuePair<string, string>("quality", GameSettings.RenderQuality.ToString()));
			list3.Add(new KeyValuePair<string, string>("status", "1"));
			list3.Add(new KeyValuePair<string, string>("Quality_Mode", GameSettings.ModelLOD.ToString()));
			list3.Add(new KeyValuePair<string, string>("Quality_Particle", GameSettings.ParticleLOD.ToString()));
			list3.Add(new KeyValuePair<string, string>("receiveMoveCmdAverage", Singleton<FrameSynchr>.instance.m_receiveMoveCmdAverage.ToString()));
			list3.Add(new KeyValuePair<string, string>("receiveMoveCmdMax", Singleton<FrameSynchr>.instance.m_receiveMoveCmdMax.ToString()));
			list3.Add(new KeyValuePair<string, string>("execMoveCmdAverage", Singleton<FrameSynchr>.instance.m_execMoveCmdAverage.ToString()));
			list3.Add(new KeyValuePair<string, string>("execMoveCmdMax", Singleton<FrameSynchr>.instance.m_execMoveCmdMax.ToString()));
			list3.Add(new KeyValuePair<string, string>("LOD_Down", Singleton<BattleLogic>.GetInstance().m_iAutoLODState.ToString()));
			if (NetworkAccelerator.started)
			{
				if (NetworkAccelerator.isAccerating())
				{
					list3.Add(new KeyValuePair<string, string>("AccState", "Acc"));
				}
				else
				{
					list3.Add(new KeyValuePair<string, string>("AccState", "Direct"));
				}
			}
			else
			{
				list3.Add(new KeyValuePair<string, string>("AccState", "Off"));
			}
			list3.Add(new KeyValuePair<string, string>("MnaState", MonoSingleton<GSDKsys>.GetInstance().m_GsdkSpeedFlag.ToString()));
			int num2 = 0;
			if (MonoSingleton<VoiceSys>.GetInstance().UseSpeak && MonoSingleton<VoiceSys>.GetInstance().UseMic)
			{
				num2 = 2;
			}
			else if (MonoSingleton<VoiceSys>.GetInstance().UseSpeak)
			{
				num2 = 1;
			}
			list3.Add(new KeyValuePair<string, string>("Mic", num2.ToString()));
			list3.Add(new KeyValuePair<string, string>("NetWorkType", CVersionUpdateSystem.Android_GetNetworkType().ToString()));
			list3.Add(new KeyValuePair<string, string>("vport", NetworkAccelerator.GetConnectIPstr()));
			List<KeyValuePair<string, string>> list4 = Singleton<DataReportSys>.GetInstance().ReportPingToBeacon();
			for (int i = 0; i < list4.Count; i++)
			{
				list3.Add(list4[i]);
			}
			list4 = Singleton<DataReportSys>.GetInstance().ReportFPSToBeacon();
			for (int j = 0; j < list4.Count; j++)
			{
				list3.Add(list4[j]);
			}
			Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_PVPBattle_Summary", list3, true);
			this.m_eventsLoadingTime.Clear();
			try
			{
				float num3 = (float)Singleton<DataReportSys>.GetInstance().FPS10Count / (float)Singleton<DataReportSys>.GetInstance().FPSCount;
				int iFps10PercentNum = Mathf.CeilToInt(num3 * 100f / 10f) * 10;
				float num4 = (float)(Singleton<DataReportSys>.GetInstance().FPS10Count + Singleton<DataReportSys>.GetInstance().FPS18Count) / (float)Singleton<DataReportSys>.GetInstance().FPSCount;
				int iFps18PercentNum = Mathf.CeilToInt(num4 * 100f / 10f) * 10;
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5000u);
				cSPkg.stPkgData.stCltPerformance.iMapID = this.m_iMapId;
				cSPkg.stPkgData.stCltPerformance.iPlayerCnt = Singleton<GamePlayerCenter>.instance.GetAllPlayers().Count;
				cSPkg.stPkgData.stCltPerformance.chModelLOD = (sbyte)GameSettings.ModelLOD;
				cSPkg.stPkgData.stCltPerformance.chParticleLOD = (sbyte)GameSettings.ParticleLOD;
				cSPkg.stPkgData.stCltPerformance.chCameraHeight = (sbyte)GameSettings.CameraHeight;
				cSPkg.stPkgData.stCltPerformance.chEnableOutline = ((!GameSettings.EnableOutline) ? (sbyte)0 : (sbyte)1);
				cSPkg.stPkgData.stCltPerformance.iFps10PercentNum = iFps10PercentNum;
				cSPkg.stPkgData.stCltPerformance.iFps18PercentNum = iFps18PercentNum;
				cSPkg.stPkgData.stCltPerformance.iAveFps = Singleton<DataReportSys>.GetInstance().FPSAVE;
				cSPkg.stPkgData.stCltPerformance.iPingAverage = Singleton<DataReportSys>.GetInstance().HeartPingAve;
				cSPkg.stPkgData.stCltPerformance.iPingVariance = Singleton<DataReportSys>.GetInstance().HeartPingVar;
				Utility.StringToByteArray(SystemInfo.deviceModel, ref cSPkg.stPkgData.stCltPerformance.szDeviceModel);
				Utility.StringToByteArray(SystemInfo.graphicsDeviceName, ref cSPkg.stPkgData.stCltPerformance.szGPUName);
				cSPkg.stPkgData.stCltPerformance.iCpuCoreNum = SystemInfo.processorCount;
				cSPkg.stPkgData.stCltPerformance.iSysMemorySize = SystemInfo.systemMemorySize;
				cSPkg.stPkgData.stCltPerformance.iAvailMemory = DeviceCheckSys.GetAvailMemory();
				cSPkg.stPkgData.stCltPerformance.iIsTongCai = ((!MonoSingleton<CTongCaiSys>.GetInstance().IsCanUseTongCai()) ? 0 : 1);
				int num5;
				if (NetworkAccelerator.started)
				{
					if (NetworkAccelerator.isAccerating())
					{
						num5 = 1;
					}
					else
					{
						num5 = 2;
					}
				}
				else
				{
					num5 = 0;
				}
				if (MonoSingleton<GSDKsys>.GetInstance().UseGSdkSpeed)
				{
					num5 = 4 + num5;
				}
				cSPkg.stPkgData.stCltPerformance.iIsSpeedUp = num5;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
			MonoSingleton<DialogueProcessor>.GetInstance().Uninit();
			Singleton<TipProcessor>.GetInstance().Uninit();
			Singleton<LobbyLogic>.instance.inMultiRoom = false;
			Singleton<LobbyLogic>.instance.inMultiGame = false;
			Singleton<LobbyLogic>.GetInstance().reconnGameInfo = null;
			Singleton<BattleLogic>.GetInstance().isRuning = false;
			Singleton<BattleLogic>.GetInstance().isFighting = false;
			Singleton<BattleLogic>.GetInstance().isGameOver = false;
			Singleton<BattleLogic>.GetInstance().isWaitMultiStart = false;
			Singleton<NetworkModule>.GetInstance().CloseGameServerConnect(true);
			NetworkAccelerator.ClearConnectIP();
			Singleton<ShenFuSystem>.instance.ClearAll();
			MonoSingleton<ActionManager>.GetInstance().ForceStop();
			Singleton<GameObjMgr>.GetInstance().ClearActor();
			Singleton<SceneManagement>.GetInstance().Clear();
			MonoSingleton<SceneMgr>.GetInstance().ClearAll();
			Singleton<GamePlayerCenter>.GetInstance().ClearAllPlayers();
			Singleton<ActorDataCenter>.instance.ClearHeroServerData();
			Singleton<FrameSynchr>.GetInstance().ResetSynchr();
			Singleton<GameReplayModule>.GetInstance().OnGameEnd();
			Singleton<BattleLogic>.GetInstance().ResetBattleSystem();
			ActionManager.Instance.frameMode = false;
			MonoSingleton<VoiceInteractionSys>.instance.OnEndGame();
			Singleton<CBattleGuideManager>.instance.OnEndGame();
			Singleton<DataReportSys>.GetInstance().ClearAllData();
			if (!Singleton<GameStateCtrl>.instance.isLobbyState)
			{
				DebugHelper.CustomLog("GotoLobbyState by EndGame");
				Singleton<GameStateCtrl>.GetInstance().GotoState("LobbyState");
			}
			Singleton<BattleSkillHudControl>.DestroyInstance();
			this.m_kGameType = COM_GAME_TYPE.COM_GAME_TYPE_MAX;
			this.m_iMapId = 0;
			try
			{
				FogOfWar.EndLevel();
			}
			catch (DllNotFoundException ex2)
			{
				DebugHelper.Assert(false, "FOW Exception {0} {1}", new object[]
				{
					ex2.Message,
					ex2.StackTrace
				});
			}
			Singleton<BattleStatistic>.instance.PostEndGame();
			try
			{
				DebugHelper.CustomLog("Finish GameBuilder EndGame");
			}
			catch (Exception)
			{
			}
		}

		private void onGameLoadProgress(float progress)
		{
			if (this.gameInfo != null && progress >= this.m_fLoadProgress + 0.01f)
			{
				this.m_fLoadProgress = progress;
				this.gameInfo.OnLoadingProgress(progress);
			}
		}

		private void OnGameLoadComplete()
		{
			if (!Singleton<BattleLogic>.instance.isRuning)
			{
				DebugHelper.Assert(false, "都没有在游戏局内，何来的游戏加载完成");
				return;
			}
			if (Singleton<WatchController>.instance.workMode != WatchController.WorkMode.None)
			{
				DebugHelper.CustomLog("观战模式:{0}", new object[]
				{
					Singleton<WatchController>.instance.workMode.ToString()
				});
			}
			try
			{
				this.gameInfo.PostBeginPlay();
			}
			catch (Exception ex)
			{
				DebugHelper.Assert(false, "Exception In PostBeginPlay {0} {1}", new object[]
				{
					ex.Message,
					ex.StackTrace
				});
				throw ex;
			}
			this.m_fLoadingTime = Time.time - this.m_fLoadingTime;
			if (MonoSingleton<Reconnection>.GetInstance().g_fBeginReconnectTime > 0f)
			{
				List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
				list.Add(new KeyValuePair<string, string>("ReconnectTime", (Time.time - MonoSingleton<Reconnection>.GetInstance().g_fBeginReconnectTime).ToString()));
				MonoSingleton<Reconnection>.GetInstance().g_fBeginReconnectTime = -1f;
				Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Reconnet_IntoGame", list, true);
			}
		}

		public void StoreGame()
		{
		}

		public void RestoreGame()
		{
		}
	}
}
