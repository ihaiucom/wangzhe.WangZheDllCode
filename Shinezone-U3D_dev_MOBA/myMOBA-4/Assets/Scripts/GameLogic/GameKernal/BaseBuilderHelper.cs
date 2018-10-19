using AGE;
using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic.GameKernal
{
	internal class BaseBuilderHelper
	{
		protected PlayerBuilder PlayerBuilder = new PlayerBuilder();

		protected GameReportor GameReportor = new GameReportor();

		public float LastLoadingTime
		{
			get;
			private set;
		}

		public virtual void BuildGameContext(ProtocolObject svrInfo)
		{
		}

		public virtual void BuildGamePlayer(ProtocolObject svrInfo)
		{
		}

		public virtual void PreLoad()
		{
			if (Singleton<BattleLogic>.instance.isRuning)
			{
				return;
			}
			GameSettings.DecideDynamicParticleLOD();
			Singleton<CHeroSelectBaseSystem>.instance.m_fOpenHeroSelectForm = Time.time - Singleton<CHeroSelectBaseSystem>.instance.m_fOpenHeroSelectForm;
			this.GameReportor.PrepareReport();
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
			List<Player>.Enumerator enumerator = Singleton<GamePlayerCenter>.instance.GetAllPlayers().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current != null)
				{
					ReadonlyContext<uint> allHeroIds = enumerator.Current.GetAllHeroIds();
					for (int i = 0; i < allHeroIds.Count; i++)
					{
						ActorMeta actorMeta = default(ActorMeta);
						ActorMeta actorMeta2 = actorMeta;
						actorMeta2.ActorCamp = enumerator.Current.PlayerCamp;
						actorMeta2.PlayerId = enumerator.Current.PlayerId;
						actorMeta2.ConfigId = (int)allHeroIds[i];
						actorMeta = actorMeta2;
						MonoSingleton<GameLoader>.instance.AddActor(ref actorMeta);
					}
				}
			}
			if (!MonoSingleton<GameFramework>.instance.EditorPreviewMode)
			{
				if (string.IsNullOrEmpty(Singleton<GameContextEx>.GetInstance().GameContextCommonInfo.MapArtistFileName))
				{
					MonoSingleton<GameLoader>.instance.AddLevel(Singleton<GameContextEx>.GetInstance().GameContextCommonInfo.MapDesignFileName);
				}
				else
				{
					MonoSingleton<GameLoader>.instance.AddDesignSerializedLevel(Singleton<GameContextEx>.GetInstance().GameContextCommonInfo.MapDesignFileName);
					MonoSingleton<GameLoader>.instance.AddArtistSerializedLevel(Singleton<GameContextEx>.GetInstance().GameContextCommonInfo.MapArtistFileName);
				}
				MonoSingleton<GameLoader>.instance.AddSoundBank("Effect_Common");
				MonoSingleton<GameLoader>.instance.AddSoundBank("System_Voice");
			}
			try
			{
				DebugHelper.CustomLog("GameBuilder Start Game: ispvplevel={0} ispvpmode={4} levelid={1} leveltype={6} levelname={3} Gametype={2} pick={5}", new object[]
				{
					Singleton<GameContextEx>.GetInstance().IsMobaMode(),
					Singleton<GameContextEx>.GetInstance().GameContextCommonInfo.MapId,
					Singleton<GameContextEx>.GetInstance().GetGameType(),
					Singleton<GameContextEx>.GetInstance().GameContextCommonInfo.MapName,
					Singleton<GameContextEx>.GetInstance().IsMobaMode(),
					Singleton<GameContextEx>.GetInstance().GetSelectHeroType(),
					Singleton<GameContextEx>.GetInstance().GameContextSoloInfo.PveLevelType
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
		}

		public virtual void Load()
		{
			if (MonoSingleton<GameFramework>.instance.EditorPreviewMode)
			{
				return;
			}
			this.LastLoadingTime = Time.time;
			MonoSingleton<GameLoader>.GetInstance().Load(new GameLoader.LoadProgressDelegate(this._OnGameLoadProgress), new GameLoader.LoadCompleteDelegate(this._OnGameLoadComplete));
			MonoSingleton<VoiceSys>.GetInstance().HeroSelectTobattle();
			Singleton<GameStateCtrl>.GetInstance().GotoState("LoadingState");
		}

		public virtual void BeginPlay()
		{
		}

		public virtual void StartFight()
		{
		}

		public virtual void EndGame()
		{
			try
			{
				DebugHelper.CustomLog("Prepare GameBuilder EndGame");
			}
			catch (Exception)
			{
			}
			Singleton<LobbyLogic>.GetInstance().StopGameEndTimer();
			Singleton<LobbyLogic>.GetInstance().StopSettleMsgTimer();
			Singleton<LobbyLogic>.GetInstance().StopSettlePanelTimer();
			MonoSingleton<GameLoader>.instance.AdvanceStopLoad();
			Singleton<WatchController>.GetInstance().Stop();
			Singleton<CBattleGuideManager>.GetInstance().resetPause();
			MonoSingleton<ShareSys>.instance.SendQQGameTeamStateChgMsg(ShareSys.QQGameTeamEventType.end, COM_ROOM_TYPE.COM_ROOM_TYPE_NULL, 0, 0u, string.Empty, 0u, 0u);
			Singleton<StarSystem>.GetInstance().EndGame();
			Singleton<WinLoseByStarSys>.GetInstance().EndGame();
			this.GameReportor.DoApolloReport();
			MonoSingleton<DialogueProcessor>.GetInstance().Uninit();
			Singleton<TipProcessor>.GetInstance().Uninit();
			Singleton<LobbyLogic>.instance.inMultiRoom = false;
			Singleton<LobbyLogic>.instance.inMultiGame = false;
			Singleton<BattleLogic>.GetInstance().isRuning = false;
			Singleton<BattleLogic>.GetInstance().isFighting = false;
			Singleton<BattleLogic>.GetInstance().isGameOver = false;
			Singleton<BattleLogic>.GetInstance().isWaitMultiStart = false;
			Singleton<NetworkModule>.GetInstance().CloseGameServerConnect(true);
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
			if (!Singleton<GameStateCtrl>.instance.isLobbyState)
			{
				DebugHelper.CustomLog("GotoLobbyState by EndGame");
				Singleton<GameStateCtrl>.GetInstance().GotoState("LobbyState");
			}
			Singleton<BattleSkillHudControl>.DestroyInstance();
			try
			{
				FogOfWar.EndLevel();
			}
			catch (DllNotFoundException ex)
			{
				DebugHelper.Assert(false, "FOW Exception {0} {1}", new object[]
				{
					ex.Message,
					ex.StackTrace
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

		public virtual void OnGameLoadProgress(float progress)
		{
		}

		public virtual void OnGameLoadComplete()
		{
			this.LastLoadingTime = Time.time - this.LastLoadingTime;
			if (MonoSingleton<Reconnection>.GetInstance().g_fBeginReconnectTime > 0f)
			{
				List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>>
				{
					new KeyValuePair<string, string>("ReconnectTime", (Time.time - MonoSingleton<Reconnection>.GetInstance().g_fBeginReconnectTime).ToString())
				};
				MonoSingleton<Reconnection>.GetInstance().g_fBeginReconnectTime = -1f;
				Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Reconnet_IntoGame", events, true);
			}
			Singleton<BattleLogic>.GetInstance().PrepareFight();
			if (!Singleton<GameContextEx>.GetInstance().IsMultiPlayerGame())
			{
				Singleton<FrameSynchr>.GetInstance().ResetSynchr();
				bool flag = false;
				Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
				int preDialogId = Singleton<GameContextEx>.GetInstance().GameContextSoloInfo.PreDialogId;
				if (preDialogId > 0 && hostPlayer != null && hostPlayer.Captain)
				{
					flag = true;
					MonoSingleton<DialogueProcessor>.instance.PlayDrama(preDialogId, hostPlayer.Captain.handle.gameObject, hostPlayer.Captain.handle.gameObject, true);
				}
				if (!flag)
				{
					Singleton<BattleLogic>.GetInstance().DoBattleStart();
				}
				else
				{
					Singleton<BattleLogic>.GetInstance().BindFightPrepareFinListener();
				}
			}
			else if (!Singleton<WatchController>.GetInstance().IsWatching)
			{
				Singleton<LobbyLogic>.GetInstance().ReqStartMultiGame();
			}
			SoldierRegion.bFirstSpawnEvent = true;
		}

		protected void _OnGameLoadProgress(float progress)
		{
			this.OnGameLoadProgress(progress);
		}

		protected void _OnGameLoadComplete()
		{
			this.OnGameLoadComplete();
		}
	}
}
