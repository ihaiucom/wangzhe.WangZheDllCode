using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class BattleLogic : Singleton<BattleLogic>, IUpdateLogic
	{

		public const int SKILL_LEVEL_MAX = 6;

		public const int SKILL3_LEVEL_MAX = 3;

		public bool isRuning;

		public bool isFighting;

		public bool isGameOver;

		public bool isWaitMultiStart;

		public bool isWaitGameEnd;

		public CSPkg m_cachedSvrEndData;

		public SLevelContext m_LevelContext;

		public Vector2 m_battleSceneSize = new Vector2(133f, 22f);

		public MapWrapper mapLogic;

		public BattleStatistic battleStat;

		public IncomeControl incomeCtrl;

		public CBattleValueAdjust valAdjustCtrl = new CBattleValueAdjust();

		public AttackOrder attackOrder = new AttackOrder();

		public DynamicProperty dynamicProperty = new DynamicProperty();

		public ClashAddition clashAddition = new ClashAddition();

		public DynamicDifficulty dynamicDifficulty = new DynamicDifficulty();

		public GameTaskSys battleTaskSys = new GameTaskSys();

		public Horizon horizon = new Horizon();

		public HostPlayerLogic hostPlayerLogic = new HostPlayerLogic();

		public bool IsModifyingCamera;

		public SpawnGroup m_dragonSpawn;

		private Dictionary<uint, int> s_dragonBuffIds;

		public string m_countDownTips;

		public OrganControl organControl = new OrganControl();

		private static int m_DelayForceKillCrystalCamp = -1;

		public bool m_bIsPayStat;

		private int m_totalLowFPSTime;

		private int m_lowFPSTimeDeadline = 20000;

		private int m_qualitySetting;

		private int m_qualitySettingParticle;

		private bool m_isUserConfirmedQualityDegrade;

		private bool m_needAutoCheckQUality = true;

		public int m_iAutoLODState;

		private float m_FpsTimeBegin;

		public GlobalTrigger m_globalTrigger
		{
			get;
			private set;
		}

		public int DragonId
		{
			get
			{
				if (this.m_dragonSpawn != null)
				{
					return this.m_dragonSpawn.TheActorsMeta[0].ConfigId;
				}
				return 0;
			}
		}

		public static void OnFailureEvaluationChanged(IStarEvaluation InStarEvaluation, IStarCondition InStarCondition)
		{
			if (Singleton<StarSystem>.instance.failureEvaluation == InStarEvaluation && Singleton<StarSystem>.instance.isFailure)
			{
				PoolObjHandle<ActorRoot> src;
				PoolObjHandle<ActorRoot> atker;
				InStarCondition.GetActorRef(out src, out atker);
				Singleton<BattleLogic>.instance.OnFailure(src, atker);
			}
		}

		public static void OnStarSystemChanged(IStarEvaluation InStarEvaluation, IStarCondition InStarCondition)
		{
			if (Singleton<StarSystem>.instance.winEvaluation == InStarEvaluation && Singleton<StarSystem>.instance.isFirstStarCompleted)
			{
				PoolObjHandle<ActorRoot> src;
				PoolObjHandle<ActorRoot> atker;
				InStarCondition.GetActorRef(out src, out atker);
				Singleton<BattleLogic>.instance.OnWinning(src, atker);
			}
		}

		public static void OnWinStarSysChanged(IStarEvaluation InStarEvaluation, IStarCondition InStarCondition)
		{
			if (Singleton<WinLoseByStarSys>.instance.WinnerEvaluation == InStarEvaluation && Singleton<WinLoseByStarSys>.instance.isSuccess)
			{
				PoolObjHandle<ActorRoot> src;
				PoolObjHandle<ActorRoot> atker;
				InStarCondition.GetActorRef(out src, out atker);
				Singleton<BattleLogic>.instance.OnWinning(src, atker);
			}
		}

		public static void OnLoseStarSysChanged(IStarEvaluation InStarEvaluation, IStarCondition InStarCondition)
		{
			if (Singleton<WinLoseByStarSys>.instance.LoserEvaluation == InStarEvaluation && Singleton<WinLoseByStarSys>.instance.isFailure)
			{
				PoolObjHandle<ActorRoot> src;
				PoolObjHandle<ActorRoot> atker;
				InStarCondition.GetActorRef(out src, out atker);
				Singleton<BattleLogic>.instance.OnFailure(src, atker);
			}
		}

		public void OnWinning(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker)
		{
			if (this.m_LevelContext != null && this.m_LevelContext.IsFireHolePlayMode())
			{
				if (Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
				{
					BattleLogic.ForceKillCrystal(2);
				}
				else
				{
					BattleLogic.ForceKillCrystal(1);
				}
			}
			else
			{
				this.OnFinish(src, atker, this.m_LevelContext.m_passDialogId);
			}
		}

		public void OnFailure(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker)
		{
			if (this.m_LevelContext != null && this.m_LevelContext.IsFireHolePlayMode())
			{
				BattleLogic.ForceKillCrystal((int)Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp);
			}
			else
			{
				this.OnFinish(src, atker, this.m_LevelContext.m_failureDialogId);
			}
		}

		public void MakeAllHeroActorInvincible()
		{
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
			int count = heroActors.Count;
			for (int i = 0; i < count; i++)
			{
				PoolObjHandle<ActorRoot> ptr = heroActors[i];
				if (ptr && ptr.handle.ActorControl != null)
				{
					HeroWrapper heroWrapper = ptr.handle.ActorControl as HeroWrapper;
					if (heroWrapper != null)
					{
						heroWrapper.bGodMode = true;
					}
				}
			}
		}

		protected void OnFinish(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, int dialogID)
		{
			DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(src, atker);
			Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_FightOver, ref defaultGameEventParam);
			this.MakeAllHeroActorInvincible();
			if (dialogID > 0)
			{
				GameObject inSrc = (!src) ? null : src.handle.gameObject;
				GameObject inAtker = (!atker) ? null : atker.handle.gameObject;
				MonoSingleton<DialogueProcessor>.GetInstance().PlayDrama(dialogID, inSrc, inAtker, false);
			}
			else
			{
				DefaultGameEventParam defaultGameEventParam2 = new DefaultGameEventParam(src, atker);
				Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_GameEnd, ref defaultGameEventParam2);
			}
		}

		public void BindFightPrepareFinListener()
		{
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepareFin, new RefAction<DefaultGameEventParam>(this.OnFightPrepareFin));
		}

		private void OnFightPrepareFin(ref DefaultGameEventParam prm)
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepareFin, new RefAction<DefaultGameEventParam>(this.OnFightPrepareFin));
			this.DoBattleStart();
		}

		private void onActorDead(ref GameDeadEventParam prm)
		{
			if (prm.bImmediateRevive || prm.bSuicide)
			{
				return;
			}
			if (this.m_LevelContext != null && this.m_LevelContext.IsMobaMode() && prm.src && ActorHelper.IsHostCtrlActor(ref prm.src))
			{
				Singleton<CSoundManager>.instance.PostEvent("Set_Dead_Low_Pass", null);
				this.SendHostPlayerDieOrReLive(CS_MULTGAME_DIE_TYPE.CS_MULTGAME_DIE);
			}
			if (prm.src && prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				PoolObjHandle<ActorRoot> attker = default(PoolObjHandle<ActorRoot>);
				if (prm.orignalAtker && prm.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					attker = prm.orignalAtker;
				}
				else if (prm.src.handle.ActorControl.IsKilledByHero())
				{
					attker = prm.src.handle.ActorControl.LastHeroAtker;
				}
				HashSet<uint> assistSet = BattleLogic.GetAssistSet(prm.src, attker, true);
				HashSet<uint>.Enumerator enumerator = assistSet.GetEnumerator();
				while (enumerator.MoveNext())
				{
					PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(enumerator.Current);
					if (actor)
					{
						prm.src.handle.ActorControl.NotifyAssistActor(ref actor);
					}
				}
			}
		}

		private void onActorRevive(ref DefaultGameEventParam prm)
		{
			if (this.m_LevelContext != null && this.m_LevelContext.IsMobaMode() && prm.src && ActorHelper.IsHostCtrlActor(ref prm.src))
			{
				Singleton<CSoundManager>.instance.PostEvent("Reset_Dead_Low_Pass", null);
				this.SendHostPlayerDieOrReLive(CS_MULTGAME_DIE_TYPE.CS_MULTGAME_RELIVE);
			}
		}

		public override void Init()
		{
			int layer = LayerMask.NameToLayer("Ignore Raycast");
			int layer2 = LayerMask.NameToLayer("Actor");
			Physics.IgnoreLayerCollision(layer, layer2);
			this.battleStat = Singleton<BattleStatistic>.GetInstance();
			Singleton<SkillDetectionControl>.GetInstance();
			Singleton<SkillSelectControl>.GetInstance();
			this.IsModifyingCamera = false;
		}

		public void SetupMap(MapWrapper map)
		{
			this.mapLogic = map;
			if (this.mapLogic)
			{
				this.battleTaskSys.Initial("Assets.Scripts.GameLogic", GameDataMgr.gameTaskDatabin, GameDataMgr.gameTaskGroupDatabin, this.mapLogic.ActionHelper);
			}
		}

		public void UpdateLogic(int delta)
		{
			if (!this.isRuning)
			{
				return;
			}
			if (this.isFighting)
			{
				Singleton<DataReportSys>.GetInstance().ProcesFPSData(Singleton<FrameSynchr>.instance.LogicFrameTick);
			}
			if (!FogOfWar.enable)
			{
				this.horizon.UpdateLogic(delta);
			}
			if (this.mapLogic)
			{
				this.mapLogic.UpdateLogic(delta);
			}
			this.UpdateDragonSpawnUI(delta);
			this.hostPlayerLogic.UpdateLogic(delta);
			if (this.dynamicProperty != null)
			{
				this.dynamicProperty.UpdateLogic(delta);
			}
			if (this.m_globalTrigger != null)
			{
				this.m_globalTrigger.UpdateLogic(delta);
			}
			Singleton<CBattleSystem>.GetInstance().UpdateLogic(delta);
			Singleton<BattleStatistic>.instance.UpdateLogic(delta);
			this.DynamicCheckQualitySetting(delta);
			if (FogOfWar.enable)
			{
				FogOfWar.UpdateMain();
			}
		}

		public void Update()
		{
			if (FogOfWar.enable && Singleton<BattleLogic>.instance.isFighting)
			{
				FogOfWar.UpdateTextures();
			}
		}

		public void InitDynamicQualityCheck()
		{
			this.m_needAutoCheckQUality = true;
			this.m_isUserConfirmedQualityDegrade = false;
			this.m_totalLowFPSTime = 0;
			this.m_lowFPSTimeDeadline = 20000;
			this.m_qualitySetting = GameSettings.ModelLOD;
			this.m_qualitySettingParticle = GameSettings.ParticleLOD;
			int @int = PlayerPrefs.GetInt("autoCheckQualityCoolDown", 0);
			if (@int > 0)
			{
				this.m_needAutoCheckQUality = false;
				PlayerPrefs.SetInt("autoCheckQualityCoolDown", @int - 1);
				PlayerPrefs.Save();
			}
		}

		private bool IsAreadyLowestQuality()
		{
			return GameSettings.ModelLOD == 2 && GameSettings.ParticleLOD == 2 && !GameSettings.EnableOutline;
		}

		private void ApplyDynamicQualityCheck()
		{
			GameSettings.ModelLOD = this.m_qualitySetting;
			if (GameSettings.DynamicParticleLOD)
			{
				GameSettings.ParticleLOD = this.m_qualitySettingParticle;
			}
		}

		private void DynamicCheckQualitySetting(int delta)
		{
			if (MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover)
			{
				return;
			}
			if (Singleton<FrameSynchr>.GetInstance().tryCount > 1)
			{
				return;
			}
			if (!this.isFighting || !this.m_needAutoCheckQUality)
			{
				return;
			}
			if (this.IsAreadyLowestQuality())
			{
				return;
			}
			if (GameFramework.m_fFps < 15f)
			{
				this.m_totalLowFPSTime += delta;
			}
			if (this.m_totalLowFPSTime > this.m_lowFPSTimeDeadline)
			{
				this.LevelDownQuality();
				this.m_lowFPSTimeDeadline = Mathf.Max(11000, this.m_lowFPSTimeDeadline / 2);
			}
		}

		public void LevelDownShadowQuality()
		{
			GameSettings.EnableOutline = false;
			if (GameSettings.ShadowQuality == SGameRenderQuality.High)
			{
				GameSettings.ShadowQuality = SGameRenderQuality.Medium;
			}
		}

		private void LevelDownQuality()
		{
			if (!this.m_isUserConfirmedQualityDegrade)
			{
				string text = Singleton<CTextManager>.GetInstance().GetText("TIPS_DEGRADE_QUALITY");
				stUIEventParams par = default(stUIEventParams);
				Singleton<CUIManager>.GetInstance().OpenSmallMessageBox(text, true, enUIEventID.Degrade_Quality_Accept, enUIEventID.Degrade_Quality_Cancel, par, 10, enUIEventID.Degrade_Quality_Accept, string.Empty, string.Empty, false);
				this.m_needAutoCheckQUality = false;
				return;
			}
			if (GameSettings.ParticleLOD < 2)
			{
				string text2 = Singleton<CTextManager>.GetInstance().GetText("TIPS_AUTO_DEGRADE_QUALITY");
				Singleton<CUIManager>.GetInstance().OpenTips(text2, false, 1.5f, null, new object[0]);
			}
			this._LevelDownQuality();
		}

		private void _LevelDownQuality()
		{
			GameSettings.EnableOutline = false;
			if (GameSettings.DynamicParticleLOD)
			{
				GameSettings.ParticleLOD++;
				if (this.m_qualitySetting < GameSettings.ParticleLOD)
				{
					this.m_qualitySetting = GameSettings.ParticleLOD;
				}
			}
			else
			{
				this.m_qualitySettingParticle++;
				if (this.m_qualitySetting < this.m_qualitySettingParticle)
				{
					this.m_qualitySetting = this.m_qualitySettingParticle;
				}
			}
			this.LevelDownShadowQuality();
			this.m_totalLowFPSTime = 0;
			PlayerPrefs.SetInt("degrade", 1);
			PlayerPrefs.Save();
			this.m_needAutoCheckQUality = false;
		}

		public void LevelDownQualityAccept(CUIEvent uiEvent)
		{
			this.m_iAutoLODState = 1;
			this.m_isUserConfirmedQualityDegrade = true;
			this.m_needAutoCheckQUality = true;
			this._LevelDownQuality();
		}

		public void LevelDownQualityCancel(CUIEvent uiEvent)
		{
			this.m_iAutoLODState = 2;
			this.m_needAutoCheckQUality = false;
			PlayerPrefs.SetInt("autoCheckQualityCoolDown", 5);
			PlayerPrefs.Save();
		}

		public bool PrepareFight()
		{
			bool isCameraFlip = this.m_LevelContext.m_isCameraFlip;
			this.m_LevelContext.m_isCameraFlip = false;
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (this.m_LevelContext != null && this.m_LevelContext.IsMobaModeWithOutGuide() && hostPlayer != null && hostPlayer.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
			{
				this.m_LevelContext.m_isCameraFlip = (!Singleton<WatchController>.GetInstance().IsWatching && isCameraFlip);
			}
			Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Select_Hero, 0uL, 0u);
			this.battleStat.m_playerKDAStat.reset();
			Singleton<CBattleSystem>.GetInstance().OpenForm((!Singleton<WatchController>.GetInstance().IsWatching) ? CBattleSystem.FormType.Fight : CBattleSystem.FormType.Watch);
			if (this.valAdjustCtrl == null)
			{
				this.valAdjustCtrl = new CBattleValueAdjust();
			}
			this.valAdjustCtrl.Init();
			Singleton<GameObjMgr>.GetInstance().PrepareFight();
			MonoSingleton<CameraSystem>.GetInstance().PrepareFight();
			this.IsModifyingCamera = false;
			GameObject gameObject = GameObject.Find("Design");
			if (gameObject)
			{
				GlobalTrigger component = gameObject.GetComponent<GlobalTrigger>();
				if (component)
				{
					component.PrepareFight();
					this.m_globalTrigger = component;
				}
			}
			bool result = MonoSingleton<DialogueProcessor>.GetInstance().PrepareFight();
			DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain, Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain);
			Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, ref defaultGameEventParam);
			return result;
		}

		public void StartFightMultiGame()
		{
			this.isWaitMultiStart = false;
			Singleton<FrameSynchr>.instance.ResetSynchrSeed();
			Singleton<FrameSynchr>.instance.SetSynchrRunning(true);
			Singleton<GameReplayModule>.GetInstance().BattleStart();
			Singleton<BattleLogic>.GetInstance().DoBattleStart();
		}

		private void DoBattleStartEvent()
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			DebugHelper.Assert(hostPlayer != null, "Fatal Error when DoBattleStartEvent, HostPlayer is null!");
			DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(hostPlayer.Captain, hostPlayer.Captain);
			Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_FightStart, ref defaultGameEventParam);
		}

		private void DoBattleStartFightStart()
		{
			GameSettings.FightStart();
			DebugHelper.Assert(this.attackOrder != null, "attackOrder is null");
			this.attackOrder.FightStart();
			DebugHelper.Assert(this.dynamicProperty != null, "dynamicProperty is null");
			this.dynamicProperty.FightStart();
			DebugHelper.Assert(this.clashAddition != null, "clashAddition is null");
			this.clashAddition.FightStart();
			DebugHelper.Assert(this.dynamicDifficulty != null, "dynamicDifficulty is null");
			this.dynamicDifficulty.FightStart();
			DebugHelper.Assert(this.horizon != null, "horizon is null");
			this.horizon.FightStart();
			SLevelContext curLvelContext = this.GetCurLvelContext();
			DebugHelper.Assert(curLvelContext != null, "slc is null");
			if (curLvelContext != null)
			{
				DebugHelper.Assert(this.incomeCtrl != null, "incomeCtrl is null");
				this.incomeCtrl.Init(curLvelContext);
				DebugHelper.Assert(curLvelContext.m_battleTaskOfCamps != null, "slc.battleTaskOfCamps is null");
				DebugHelper.Assert(this.battleTaskSys != null, "battleTaskSys is null");
				for (int i = 0; i < curLvelContext.m_battleTaskOfCamps.Length; i++)
				{
					if (curLvelContext.m_battleTaskOfCamps[i] > 0u)
					{
						this.battleTaskSys.AddTask(curLvelContext.m_battleTaskOfCamps[i], true);
					}
				}
				if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
				{
					Singleton<CBattleSystem>.instance.FightForm.ShowTaskView(this.battleTaskSys.HasTask);
				}
				if (curLvelContext.IsGameTypeArena() && Singleton<CBattleSystem>.GetInstance().FightForm != null)
				{
					Singleton<CBattleSystem>.GetInstance().FightForm.ShowArenaTimer();
				}
			}
			if (Singleton<CBattleSystem>.GetInstance().FightForm != null && Singleton<CBattleSystem>.instance.FightForm.GetBattleMisc() != null)
			{
				Singleton<CBattleSystem>.instance.FightForm.GetBattleMisc().RebindBoss();
				Singleton<CBattleSystem>.instance.FightForm.GetBattleMisc().BindHP();
			}
			DebugHelper.Assert(this.hostPlayerLogic != null, "hostPlayerLogic is null");
			this.hostPlayerLogic.FightStart();
			this.organControl.FightStart();
		}

		public void DoBattleStart()
		{
			this.m_iAutoLODState = 0;
			Singleton<DataReportSys>.GetInstance().ClearFpsData();
			DebugHelper.Assert(!this.isFighting, "isFighting == false");
			if (this.isFighting)
			{
				return;
			}
			this.isFighting = true;
			this.isWaitGameEnd = false;
			this.m_cachedSvrEndData = null;
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Degrade_Quality_Accept, new CUIEventManager.OnUIEventHandler(this.LevelDownQualityAccept));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Degrade_Quality_Cancel, new CUIEventManager.OnUIEventHandler(this.LevelDownQualityCancel));
			if (this.battleStat != null)
			{
				this.battleStat.StartStatistic();
			}
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Battle_ActivateForm);
			this.RegistBattleEvent();
			Singleton<StarSystem>.GetInstance().StartFight();
			Singleton<WinLoseByStarSys>.GetInstance().StartFight();
			if (this.mapLogic)
			{
				this.mapLogic.Startup();
			}
			if (this.incomeCtrl == null)
			{
				this.incomeCtrl = new IncomeControl();
			}
			this.incomeCtrl.StartFight();
			Singleton<GameObjMgr>.GetInstance().StartFight();
			Singleton<GameStateCtrl>.GetInstance().GotoState("BattleState");
			Singleton<GameInput>.GetInstance().ChangeBattleMode(false);
			this.DoBattleStartEvent();
			this.DoBattleStartFightStart();
			this.SpawnMapBuffs();
			this.SendSecureStartInfoReq();
			Singleton<CSurrenderSystem>.instance.Reset();
			this.InitDynamicQualityCheck();
			if (FogOfWar.enable)
			{
				Singleton<GameFowManager>.instance.OnStartFight();
			}
			Singleton<CBattleSystem>.GetInstance().BattleStart();
			if (this.GetCurLvelContext().IsMultilModeWithWarmBattle())
			{
				MonoSingleton<VoiceSys>.instance.StartSyncVoiceStateTimer(4000);
			}
			Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
		}

		public bool FilterEnemyActor(ref PoolObjHandle<ActorRoot> actor)
		{
			return ActorHelper.IsHostEnemyActor(ref actor);
		}

		public bool FilterTeamActor(ref PoolObjHandle<ActorRoot> actor)
		{
			return ActorHelper.IsHostCampActor(ref actor);
		}

		public string GetLevelTypeDescription()
		{
			if (this.m_LevelContext == null)
			{
				return string.Empty;
			}
			if (!this.m_LevelContext.IsMobaMode())
			{
				return "PVE";
			}
			return this.m_LevelContext.m_pvpPlayerNum / 2 + "V" + this.m_LevelContext.m_pvpPlayerNum / 2;
		}

		private void SendSecureStartInfoReq()
		{
			Singleton<NetworkModule>.instance.RecvGameMsgCount = 0u;
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1062u);
			CSPKG_SECURE_INFO_START_REQ stSecureInfoStartReq = cSPkg.stPkgData.stSecureInfoStartReq;
			DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			TimeSpan timeSpan = DateTime.Now - d;
			cSPkg.stPkgData.stSecureInfoStartReq.dwClientStartTime = (uint)timeSpan.TotalSeconds;
			List<PoolObjHandle<ActorRoot>> list = ActorHelper.FilterActors(Singleton<GameObjMgr>.instance.HeroActors, new ActorFilterDelegate(this.FilterEnemyActor));
			stSecureInfoStartReq.iSvrBossCount = list.Count;
			for (int i = 0; i < list.Count; i++)
			{
				stSecureInfoStartReq.iSvrBossHPMax = ((stSecureInfoStartReq.iSvrBossHPMax <= list[i].handle.ValueComponent.actorHp) ? list[i].handle.ValueComponent.actorHp : stSecureInfoStartReq.iSvrBossHPMax);
				if (stSecureInfoStartReq.iSvrBossHPMin == 0)
				{
					stSecureInfoStartReq.iSvrBossHPMin = list[i].handle.ValueComponent.actorHp;
				}
				else
				{
					stSecureInfoStartReq.iSvrBossHPMin = ((stSecureInfoStartReq.iSvrBossHPMin >= list[i].handle.ValueComponent.actorHp) ? list[i].handle.ValueComponent.actorHp : stSecureInfoStartReq.iSvrBossHPMin);
				}
				stSecureInfoStartReq.iSvrBossHPTotal += list[i].handle.ValueComponent.actorHp;
				int totalValue = list[i].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
				stSecureInfoStartReq.iSvrBossAttackMax = ((stSecureInfoStartReq.iSvrBossAttackMax <= totalValue) ? totalValue : stSecureInfoStartReq.iSvrBossAttackMax);
				if (stSecureInfoStartReq.iSvrBossAttackMin == 0)
				{
					stSecureInfoStartReq.iSvrBossAttackMin = totalValue;
				}
				else
				{
					stSecureInfoStartReq.iSvrBossAttackMin = ((stSecureInfoStartReq.iSvrBossAttackMin >= totalValue) ? totalValue : stSecureInfoStartReq.iSvrBossAttackMin);
				}
				if (list.Count > 0)
				{
					stSecureInfoStartReq.iSvrEmenyCardID1 = list[0].handle.TheActorMeta.ConfigId;
					stSecureInfoStartReq.iSvrEmenyCardHP1 = list[0].handle.ValueComponent.actorHp;
					int num = list[0].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardATN1 = num;
					num = list[0].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardINT1 = num;
					num = list[0].handle.ValueComponent.actorMoveSpeed;
					stSecureInfoStartReq.iSvrEmenyCardSpeed1 = num;
					num = list[0].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardPhyDef1 = num;
					num = list[0].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardSpellDef1 = num;
				}
				if (list.Count > 1)
				{
					stSecureInfoStartReq.iSvrEmenyCardID2 = list[1].handle.TheActorMeta.ConfigId;
					stSecureInfoStartReq.iSvrEmenyCardHP2 = list[1].handle.ValueComponent.actorHp;
					int num2 = list[1].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardATN2 = num2;
					num2 = list[1].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardINT2 = num2;
					num2 = list[1].handle.ValueComponent.actorMoveSpeed;
					stSecureInfoStartReq.iSvrEmenyCardSpeed2 = num2;
					num2 = list[1].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardPhyDef2 = num2;
					num2 = list[1].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardSpellDef2 = num2;
				}
				if (list.Count > 2)
				{
					stSecureInfoStartReq.iSvrEmenyCardID3 = list[2].handle.TheActorMeta.ConfigId;
					stSecureInfoStartReq.iSvrEmenyCardHP3 = list[2].handle.ValueComponent.actorHp;
					int num3 = list[2].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardATN3 = num3;
					num3 = list[2].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardINT3 = num3;
					num3 = list[2].handle.ValueComponent.actorMoveSpeed;
					stSecureInfoStartReq.iSvrEmenyCardSpeed3 = num3;
					num3 = list[2].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardPhyDef3 = num3;
					num3 = list[2].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardSpellDef3 = num3;
				}
			}
			List<PoolObjHandle<ActorRoot>> organActors = Singleton<GameObjMgr>.instance.OrganActors;
			for (int j = 0; j < organActors.Count; j++)
			{
				PoolObjHandle<ActorRoot> poolObjHandle = organActors[j];
				int actorHp = poolObjHandle.handle.ValueComponent.actorHp;
				int totalValue2 = poolObjHandle.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
				if (ActorHelper.IsHostEnemyActor(ref poolObjHandle))
				{
					stSecureInfoStartReq.iSvrEmenyBuildingHPMax = ((stSecureInfoStartReq.iSvrEmenyBuildingHPMax <= actorHp) ? actorHp : stSecureInfoStartReq.iSvrEmenyBuildingHPMax);
					if (stSecureInfoStartReq.iSvrEmenyBuildingHPMin == 0)
					{
						stSecureInfoStartReq.iSvrEmenyBuildingHPMin = actorHp;
					}
					else
					{
						stSecureInfoStartReq.iSvrEmenyBuildingHPMin = ((stSecureInfoStartReq.iSvrEmenyBuildingHPMin >= actorHp) ? actorHp : stSecureInfoStartReq.iSvrEmenyBuildingHPMin);
					}
					stSecureInfoStartReq.iSvrEmenyHPTotal += actorHp;
					stSecureInfoStartReq.iSvrEmenyBuildingAttackMax = ((stSecureInfoStartReq.iSvrEmenyBuildingAttackMax <= totalValue2) ? totalValue2 : stSecureInfoStartReq.iSvrEmenyBuildingAttackMax);
					if (stSecureInfoStartReq.iSvrEmenyBuildingAttackMin == 0)
					{
						stSecureInfoStartReq.iSvrEmenyBuildingAttackMin = totalValue2;
					}
					else
					{
						stSecureInfoStartReq.iSvrEmenyBuildingAttackMin = ((stSecureInfoStartReq.iSvrEmenyBuildingAttackMin >= totalValue2) ? totalValue2 : stSecureInfoStartReq.iSvrEmenyBuildingAttackMin);
					}
				}
				else
				{
					stSecureInfoStartReq.iSvrBuildingHPMax = ((stSecureInfoStartReq.iSvrBuildingHPMax <= actorHp) ? actorHp : stSecureInfoStartReq.iSvrBuildingHPMax);
					if (stSecureInfoStartReq.iSvrBuildingHPMin == 0)
					{
						stSecureInfoStartReq.iSvrBuildingHPMin = actorHp;
					}
					else
					{
						stSecureInfoStartReq.iSvrBuildingHPMin = ((stSecureInfoStartReq.iSvrBuildingHPMin >= actorHp) ? actorHp : stSecureInfoStartReq.iSvrBuildingHPMin);
					}
					stSecureInfoStartReq.iSvrBuildingAttackMax = ((stSecureInfoStartReq.iSvrBuildingAttackMax <= totalValue2) ? totalValue2 : stSecureInfoStartReq.iSvrBuildingAttackMax);
					if (stSecureInfoStartReq.iSvrBuildingAttackMin == 0)
					{
						stSecureInfoStartReq.iSvrBuildingAttackMin = totalValue2;
					}
					else
					{
						stSecureInfoStartReq.iSvrBuildingAttackMin = ((stSecureInfoStartReq.iSvrBuildingAttackMin >= totalValue2) ? totalValue2 : stSecureInfoStartReq.iSvrBuildingAttackMin);
					}
				}
			}
			List<PoolObjHandle<ActorRoot>> list2 = ActorHelper.FilterActors(Singleton<GameObjMgr>.instance.SoldierActors, new ActorFilterDelegate(this.FilterEnemyActor));
			for (int k = 0; k < list2.Count; k++)
			{
				PoolObjHandle<ActorRoot> poolObjHandle2 = list2[k];
				int actorHp2 = poolObjHandle2.handle.ValueComponent.actorHp;
				int totalValue3 = poolObjHandle2.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
				stSecureInfoStartReq.iSvrEmenyHPMax = ((stSecureInfoStartReq.iSvrEmenyHPMax <= actorHp2) ? actorHp2 : stSecureInfoStartReq.iSvrEmenyHPMax);
				if (stSecureInfoStartReq.iSvrEmenyHPMin == 0)
				{
					stSecureInfoStartReq.iSvrEmenyHPMin = actorHp2;
				}
				else
				{
					stSecureInfoStartReq.iSvrEmenyHPMin = ((stSecureInfoStartReq.iSvrEmenyHPMin >= actorHp2) ? actorHp2 : stSecureInfoStartReq.iSvrEmenyHPMin);
				}
				stSecureInfoStartReq.iSvrEmenyAttackMax = ((stSecureInfoStartReq.iSvrEmenyAttackMax <= totalValue3) ? totalValue3 : stSecureInfoStartReq.iSvrEmenyAttackMax);
				if (stSecureInfoStartReq.iSvrEmenyAttackMin == 0)
				{
					stSecureInfoStartReq.iSvrEmenyAttackMin = totalValue3;
				}
				else
				{
					stSecureInfoStartReq.iSvrEmenyAttackMin = ((stSecureInfoStartReq.iSvrEmenyAttackMin >= totalValue3) ? totalValue3 : stSecureInfoStartReq.iSvrEmenyAttackMin);
				}
			}
			List<PoolObjHandle<ActorRoot>> list3 = ActorHelper.FilterActors(Singleton<GameObjMgr>.instance.HeroActors, new ActorFilterDelegate(this.FilterTeamActor));
			if (list3.Count > 0)
			{
				stSecureInfoStartReq.iSvrUserCardID1 = list3[0].handle.TheActorMeta.ConfigId;
				stSecureInfoStartReq.iSvrUserCardHP1 = list3[0].handle.ValueComponent.actorHp;
				int num4 = list3[0].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
				stSecureInfoStartReq.iSvrUserCardATN1 = num4;
				num4 = list3[0].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].totalValue;
				stSecureInfoStartReq.iSvrUserCardINT1 = num4;
				num4 = list3[0].handle.ValueComponent.actorMoveSpeed;
				stSecureInfoStartReq.iSvrUserCardSpeed1 = num4;
				num4 = list3[0].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].totalValue;
				stSecureInfoStartReq.iSvrUserCardPhyDef1 = num4;
				num4 = list3[0].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT].totalValue;
				stSecureInfoStartReq.iSvrUserCardSpellDef1 = num4;
			}
			if (list3.Count > 1)
			{
				stSecureInfoStartReq.iSvrUserCardID2 = list3[1].handle.TheActorMeta.ConfigId;
				stSecureInfoStartReq.iSvrUserCardHP2 = list3[1].handle.ValueComponent.actorHp;
				int num5 = list3[1].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
				stSecureInfoStartReq.iSvrUserCardATN2 = num5;
				num5 = list3[1].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].totalValue;
				stSecureInfoStartReq.iSvrUserCardINT2 = num5;
				num5 = list3[1].handle.ValueComponent.actorMoveSpeed;
				stSecureInfoStartReq.iSvrUserCardSpeed2 = num5;
				num5 = list3[1].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].totalValue;
				stSecureInfoStartReq.iSvrUserCardPhyDef2 = num5;
				num5 = list3[1].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT].totalValue;
				stSecureInfoStartReq.iSvrUserCardSpellDef2 = num5;
			}
			if (list3.Count > 2)
			{
				stSecureInfoStartReq.iSvrUserCardID3 = list3[2].handle.TheActorMeta.ConfigId;
				stSecureInfoStartReq.iSvrUserCardHP3 = list3[2].handle.ValueComponent.actorHp;
				int num6 = list3[2].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
				stSecureInfoStartReq.iSvrUserCardATN3 = num6;
				num6 = list3[2].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].totalValue;
				stSecureInfoStartReq.iSvrUserCardINT3 = num6;
				num6 = list3[2].handle.ValueComponent.actorMoveSpeed;
				stSecureInfoStartReq.iSvrUserCardSpeed3 = num6;
				num6 = list3[2].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].totalValue;
				stSecureInfoStartReq.iSvrUserCardPhyDef3 = num6;
				num6 = list3[2].handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT].totalValue;
				stSecureInfoStartReq.iSvrUserCardSpellDef3 = num6;
			}
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private static void OnDelayForceKillCrystalTimerComplete(int timerSequence)
		{
			if (BattleLogic.m_DelayForceKillCrystalCamp >= 0)
			{
				BattleLogic.ForceKillCrystal(BattleLogic.m_DelayForceKillCrystalCamp);
			}
		}

		public static void DelayForceKillCrystal(uint delay, int Camp)
		{
			BattleLogic.m_DelayForceKillCrystalCamp = Camp;
			Singleton<CTimerManager>.instance.AddTimer((int)delay, 1, new CTimer.OnTimeUpHandler(BattleLogic.OnDelayForceKillCrystalTimerComplete), true);
		}

		public static void ForceKillCrystal(int Camp)
		{
			List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.instance.GameActors;
			int count = gameActors.Count;
			for (int i = 0; i < count; i++)
			{
				PoolObjHandle<ActorRoot> ptr = gameActors[i];
				if (ptr && ptr.handle.ActorControl != null && ptr.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ && ptr.handle.TheActorMeta.ActorCamp == (COM_PLAYERCAMP)Camp && ptr.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 2)
				{
					ptr.handle.ValueComponent.actorHp = 0;
					break;
				}
			}
		}

		public void DealGameSurrender(byte bWinCamp)
		{
			if (this.isWaitGameEnd)
			{
				return;
			}
			COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
			this.battleStat.iBattleResult = (((byte)playerCamp != bWinCamp) ? 2 : 1);
			COM_PLAYERCAMP camp;
			if (this.battleStat.iBattleResult == 1)
			{
				camp = BattleLogic.GetOppositeCmp(playerCamp);
			}
			else
			{
				camp = playerCamp;
			}
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(182u).dwConfValue;
			KillNotify theKillNotify = Singleton<CBattleSystem>.GetInstance().TheKillNotify;
			if (theKillNotify != null)
			{
				theKillNotify.ClearKillNotifyList();
				if (this.battleStat.iBattleResult == 1)
				{
					theKillNotify.PlayAnimator("TouXiang_B");
				}
				else
				{
					theKillNotify.PlayAnimator("TouXiang_A");
				}
			}
			BattleLogic.DelayForceKillCrystal(dwConfValue, (int)camp);
		}

		public void onFightOver(ref DefaultGameEventParam prm)
		{
			if (!this.isFighting)
			{
				DebugHelper.Assert(false, "wtf, 重复触发onFightOver");
				return;
			}
			Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, ref prm);
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Degrade_Quality_Accept, new CUIEventManager.OnUIEventHandler(this.LevelDownQualityAccept));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Degrade_Quality_Cancel, new CUIEventManager.OnUIEventHandler(this.LevelDownQualityCancel));
			this.DoFightOver(true);
			if (Singleton<WatchController>.GetInstance().IsWatching)
			{
				Singleton<WatchController>.GetInstance().MarkOver(true);
			}
			else if (Singleton<LobbyLogic>.instance.inMultiGame)
			{
				Singleton<LobbyLogic>.GetInstance().StartSettleTimers();
				Singleton<LobbyLogic>.GetInstance().ReqMultiGameOver(false);
			}
			else
			{
				if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
				{
					Singleton<WinLose>.instance.LastSingleGameWin = (Singleton<BattleLogic>.instance.JudgeBattleResult(prm.src, prm.orignalAtker) == 1);
					if (this.m_LevelContext.IsMobaMode())
					{
						SettleEventParam settleEventParam = default(SettleEventParam);
						settleEventParam.isWin = Singleton<WinLose>.instance.LastSingleGameWin;
						Singleton<GameEventSys>.GetInstance().SendEvent<SettleEventParam>(GameEventDef.Event_SettleComplete, ref settleEventParam);
					}
				}
				Singleton<LobbyLogic>.GetInstance().ReqSingleGameFinish(false, false);
			}
		}

		public void DoFightOver(bool bNormalEnd)
		{
			if (!this.isFighting)
			{
				DebugHelper.Assert(false, "wtf, 重复调用DoFightOver");
				return;
			}
			Singleton<DataReportSys>.GetInstance().GameTime = (int)(Singleton<FrameSynchr>.GetInstance().LogicFrameTick * 0.001f);
			this.horizon.FightOver();
			Singleton<GameObjMgr>.GetInstance().FightOver();
			if (this.mapLogic != null)
			{
				this.mapLogic.Reset();
			}
			this.attackOrder.FightOver();
			this.dynamicProperty.FightOver();
			this.clashAddition.FightOver();
			this.battleTaskSys.Clear();
			if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				Singleton<CBattleSystem>.GetInstance().FightForm.DisableUIEvent();
			}
			this.hostPlayerLogic.FightOver();
			this.organControl.FightOver();
			Singleton<FrameSynchr>.instance.SwitchSynchrLocal();
			this.isFighting = false;
			this.isGameOver = true;
			this.isWaitGameEnd = bNormalEnd;
			this.m_cachedSvrEndData = null;
			BattleLogic.m_DelayForceKillCrystalCamp = -1;
		}

		public void onPreGameSettle()
		{
			if (this.mapLogic != null)
			{
				this.mapLogic.Reset();
			}
		}

		public void onGameEnd(ref DefaultGameEventParam prm)
		{
			MonoSingleton<VoiceSys>.instance.ClearVoiceStateData();
			Singleton<LobbyLogic>.GetInstance().StopGameEndTimer();
			if (!Singleton<WatchController>.GetInstance().IsWatching)
			{
				Singleton<LobbyLogic>.GetInstance().StartSettlePanelTimer();
				if (this.isWaitGameEnd && this.m_cachedSvrEndData != null)
				{
					CSPkg cachedSvrEndData = this.m_cachedSvrEndData;
					this.m_cachedSvrEndData = null;
					if (Singleton<LobbyLogic>.instance.inMultiGame)
					{
						if (cachedSvrEndData.stPkgData.stMultGameSettleGain.iErrCode == 0)
						{
							SLevelContext.SetMasterPvpDetailWhenGameSettle(cachedSvrEndData.stPkgData.stMultGameSettleGain.stDetail.stGameInfo);
						}
						LobbyMsgHandler.HandleGameSettle(cachedSvrEndData.stPkgData.stMultGameSettleGain.iErrCode == 0, true, cachedSvrEndData.stPkgData.stMultGameSettleGain.stDetail.stGameInfo.bGameResult, cachedSvrEndData.stPkgData.stMultGameSettleGain.stDetail.stHeroList, cachedSvrEndData.stPkgData.stMultGameSettleGain.stDetail.stRankInfo, cachedSvrEndData.stPkgData.stMultGameSettleGain.stDetail.stAcntInfo, cachedSvrEndData.stPkgData.stMultGameSettleGain.stDetail.stMultipleDetail, cachedSvrEndData.stPkgData.stMultGameSettleGain.stDetail.stSpecReward, cachedSvrEndData.stPkgData.stMultGameSettleGain.stDetail.stReward);
					}
					else
					{
						LobbyMsgHandler.HandleSingleGameSettle(cachedSvrEndData);
					}
				}
			}
			this.isWaitGameEnd = false;
			this.UnRegistBattleEvent();
		}

		public void ResetBattleSystem()
		{
			this.isWaitGameEnd = false;
			this.m_cachedSvrEndData = null;
			this.UnRegistBattleEvent();
			if (this.mapLogic != null)
			{
				this.mapLogic.Reset();
			}
			this.battleTaskSys.Clear();
			this.ApplyDynamicQualityCheck();
			if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				Singleton<CBattleSystem>.GetInstance().FightForm.DisableUIEvent();
			}
		}

		public void SingleReqLoseGame()
		{
			Singleton<WinLose>.GetInstance().LastSingleGameWin = false;
			bool clickGameOver = true;
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.IsGameTypeGuide() && curLvelContext.IsMobaMode() && curLvelContext.m_mapID == 7)
			{
				Singleton<WinLose>.GetInstance().LastSingleGameWin = true;
				clickGameOver = false;
			}
			Singleton<LobbyLogic>.GetInstance().ReqSingleGameFinish(clickGameOver, false);
		}

		private void RegistBattleEvent()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightOver, new RefAction<DefaultGameEventParam>(this.onFightOver));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_GameEnd, new RefAction<DefaultGameEventParam>(this.onGameEnd));
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.onActorRevive));
			Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
			Singleton<GameEventSys>.instance.RmvEventHandler<HemophagiaEventResultInfo>(GameEventDef.Event_Hemophagia, new RefAction<HemophagiaEventResultInfo>(this.OnActorHemophagia));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroSoulLvlChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this.OnHeroSkillLvlup));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightOver, new RefAction<DefaultGameEventParam>(this.onFightOver));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_GameEnd, new RefAction<DefaultGameEventParam>(this.onGameEnd));
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.onActorRevive));
			Singleton<GameEventSys>.instance.AddEventHandler<HemophagiaEventResultInfo>(GameEventDef.Event_Hemophagia, new RefAction<HemophagiaEventResultInfo>(this.OnActorHemophagia));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroSoulLvlChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this.OnHeroSkillLvlup));
		}

		private void UnRegistBattleEvent()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightOver, new RefAction<DefaultGameEventParam>(this.onFightOver));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_GameEnd, new RefAction<DefaultGameEventParam>(this.onGameEnd));
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepareFin, new RefAction<DefaultGameEventParam>(this.OnFightPrepareFin));
			Singleton<GameEventSys>.instance.RmvEventHandler<HemophagiaEventResultInfo>(GameEventDef.Event_Hemophagia, new RefAction<HemophagiaEventResultInfo>(this.OnActorHemophagia));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroSoulLvlChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this.OnHeroSkillLvlup));
			if (this.incomeCtrl != null)
			{
				this.incomeCtrl.uninit();
			}
			if (this.valAdjustCtrl != null)
			{
				this.valAdjustCtrl.UnInit();
			}
		}

		public void ShowWinLose(bool bWin)
		{
			if (Singleton<CBattleSystem>.GetInstance().FightForm != null && this.m_LevelContext.IsMobaMode())
			{
				Singleton<CBattleSystem>.GetInstance().FightForm.ShowWinLosePanel(bWin);
			}
		}

		private void OnActorHemophagia(ref HemophagiaEventResultInfo hri)
		{
			if (hri.src && (!hri.src.handle.Visible || !hri.src.handle.InCamera))
			{
				return;
			}
			if (ActorHelper.IsHostActor(ref hri.src))
			{
				CBattleSystem instance = Singleton<CBattleSystem>.GetInstance();
				if (hri.hpChanged != 0)
				{
					Vector3 position = hri.src.handle.myTransform.position;
					float num = UnityEngine.Random.Range(-0.5f, 0.5f);
					position = new Vector3(position.x, position.y + num, position.z);
					instance.CollectFloatDigitInSingleFrame(hri.src, hri.src, DIGIT_TYPE.ReviveHp, hri.hpChanged);
				}
			}
		}

		private void OnActorDamage(ref HurtEventResultInfo hri)
		{
			if (Singleton<CBattleSystem>.GetInstance().TheMinimapSys != null)
			{
				Singleton<CBattleSystem>.GetInstance().TheMinimapSys.OnActorDamage(ref hri);
			}
			if (hri.src && (!hri.src.handle.Visible || !hri.src.handle.InCamera))
			{
				return;
			}
			DIGIT_TYPE dIGIT_TYPE = DIGIT_TYPE.Invalid;
			PoolObjHandle<ActorRoot> ptr;
			if (hri.src && hri.src.handle.ActorControl != null)
			{
				ptr = hri.src.handle.ActorControl.GetOrignalActor();
			}
			else
			{
				ptr = hri.src;
			}
			PoolObjHandle<ActorRoot> ptr2;
			if (hri.atker && hri.atker.handle.ActorControl != null)
			{
				ptr2 = hri.atker.handle.ActorControl.GetOrignalActor();
			}
			else
			{
				ptr2 = hri.atker;
			}
			if ((ptr && ActorHelper.IsHostActor(ref ptr)) || (ptr2 && ActorHelper.IsHostActor(ref ptr2)))
			{
				if (hri.hurtInfo.hurtType == HurtTypeDef.Therapic)
				{
					dIGIT_TYPE = DIGIT_TYPE.ReviveHp;
				}
				else if (hri.hurtInfo.hurtType == HurtTypeDef.MagicHurt)
				{
					dIGIT_TYPE = ((hri.critValue <= 0) ? DIGIT_TYPE.MagicAttackNormal : DIGIT_TYPE.MagicAttackCrit);
				}
				else if (hri.hurtInfo.hurtType == HurtTypeDef.PhysHurt)
				{
					dIGIT_TYPE = ((hri.critValue <= 0) ? DIGIT_TYPE.PhysicalAttackNormal : DIGIT_TYPE.PhysicalAttackCrit);
				}
				else
				{
					dIGIT_TYPE = ((hri.critValue <= 0) ? DIGIT_TYPE.RealAttackNormal : DIGIT_TYPE.RealAttackCrit);
				}
			}
			if (dIGIT_TYPE != DIGIT_TYPE.Invalid)
			{
				CBattleSystem instance = Singleton<CBattleSystem>.GetInstance();
				if (hri.hpChanged != 0)
				{
					int value = (dIGIT_TYPE != DIGIT_TYPE.ReviveHp) ? hri.hurtTotal : hri.hpChanged;
					instance.CollectFloatDigitInSingleFrame(hri.atker, hri.src, dIGIT_TYPE, value);
				}
			}
		}

		public SLevelContext GetCurLvelContext()
		{
			return this.m_LevelContext;
		}

		public int JudgeBattleResult(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker)
		{
			SLevelContext curLvelContext = this.GetCurLvelContext();
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer == null)
			{
				return 0;
			}
			int playerCamp = (int)hostPlayer.PlayerCamp;
			if (curLvelContext != null && playerCamp >= 0 && playerCamp < curLvelContext.m_battleTaskOfCamps.Length)
			{
				GameTask task = this.battleTaskSys.GetTask(curLvelContext.m_battleTaskOfCamps[playerCamp], false);
				if (task != null)
				{
					return (!task.Achieving) ? 2 : 1;
				}
			}
			if (Singleton<WinLoseByStarSys>.instance.bStarted)
			{
				if (Singleton<WinLoseByStarSys>.instance.isSuccess)
				{
					return 1;
				}
				if (Singleton<WinLoseByStarSys>.instance.isFailure)
				{
					return 2;
				}
				if (src)
				{
					if (playerCamp == (int)src.handle.TheActorMeta.ActorCamp)
					{
					}
					return (playerCamp == (int)src.handle.TheActorMeta.ActorCamp) ? 2 : 1;
				}
				return 0;
			}
			else
			{
				if (src)
				{
					if (playerCamp == (int)src.handle.TheActorMeta.ActorCamp)
					{
					}
					return (playerCamp == (int)src.handle.TheActorMeta.ActorCamp) ? 2 : 1;
				}
				return 0;
			}
		}

		private void UpdateDragonSpawnUI(int delta)
		{
			if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				Singleton<CBattleSystem>.GetInstance().FightForm.OnUpdateDragonUI(delta);
			}
		}

		public static COM_PLAYERCAMP GetOppositeCmp(COM_PLAYERCAMP InCamp)
		{
			if (InCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
			{
				return COM_PLAYERCAMP.COM_PLAYERCAMP_2;
			}
			return COM_PLAYERCAMP.COM_PLAYERCAMP_1;
		}

		public static COM_PLAYERCAMP[] GetOthersCmp(COM_PLAYERCAMP InCamp)
		{
			int num = 3;
			COM_PLAYERCAMP[] array = new COM_PLAYERCAMP[num - 1];
			for (int i = 0; i < num; i++)
			{
				if (i != (int)InCamp)
				{
					array[((i <= (int)InCamp) ? num : 0) + (i - (int)InCamp) - 1] = (COM_PLAYERCAMP)i;
				}
			}
			return array;
		}

		public static int MapOtherCampIndex(COM_PLAYERCAMP myCamp, COM_PLAYERCAMP otherCamp)
		{
			return ((otherCamp <= myCamp) ? 3 : 0) + (otherCamp - myCamp) - 1;
		}

		public static COM_PLAYERCAMP MapOtherCampType(COM_PLAYERCAMP myCamp, int index)
		{
			return (COM_PLAYERCAMP)((int)(myCamp + index + 1) % (int)COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT);
		}

		public int GetDragonBuffId(RES_SKILL_SRC_TYPE type)
		{
			int result = 0;
			if (this.s_dragonBuffIds == null)
			{
				this.s_dragonBuffIds = new Dictionary<uint, int>();
				GameDataMgr.skillCombineDatabin.Accept(new Action<ResSkillCombineCfgInfo>(this.OnVisit));
			}
			this.s_dragonBuffIds.TryGetValue((uint)type, out result);
			return result;
		}

		private void OnVisit(ResSkillCombineCfgInfo InCfg)
		{
			byte b = 1;
			byte b2 = 5;
			if (InCfg.bSrcType >= b && InCfg.bSrcType < b2)
			{
				if (this.s_dragonBuffIds.ContainsKey((uint)InCfg.bSrcType))
				{
					this.s_dragonBuffIds[(uint)InCfg.bSrcType] = InCfg.iCfgID;
				}
				else
				{
					this.s_dragonBuffIds.Add((uint)InCfg.bSrcType, InCfg.iCfgID);
				}
			}
		}

		private void SpawnMapBuffs()
		{
			SLevelContext curLvelContext = this.GetCurLvelContext();
			if (curLvelContext == null || curLvelContext.m_mapBuffs == null)
			{
				return;
			}
			for (int i = 0; i < curLvelContext.m_mapBuffs.Length; i++)
			{
				ResDT_MapBuff resDT_MapBuff = curLvelContext.m_mapBuffs[i];
				if (resDT_MapBuff.dwID == 0u)
				{
					break;
				}
				List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
				for (int j = 0; j < heroActors.Count; j++)
				{
					PoolObjHandle<ActorRoot> inTargetActor = heroActors[j];
					if ((1 << (int)inTargetActor.handle.TheActorMeta.ActorCamp & (int)resDT_MapBuff.bCamp) > 0 && (resDT_MapBuff.bHeroType == 0 || inTargetActor.handle.TheStaticData.TheHeroOnlyInfo.HeroCapability == (int)resDT_MapBuff.bHeroType) && (resDT_MapBuff.bHeroDamageType == 0 || inTargetActor.handle.TheStaticData.TheHeroOnlyInfo.HeroDamageType == (int)resDT_MapBuff.bHeroDamageType) && (resDT_MapBuff.bHeroAttackType == 0 || inTargetActor.handle.TheStaticData.TheHeroOnlyInfo.HeroAttackType == (int)resDT_MapBuff.bHeroAttackType))
					{
						SkillUseParam skillUseParam = default(SkillUseParam);
						inTargetActor.handle.SkillControl.SpawnBuff(inTargetActor, ref skillUseParam, (int)resDT_MapBuff.dwID, false);
					}
				}
			}
		}

		public void SendHostPlayerDieOrReLive(CS_MULTGAME_DIE_TYPE type)
		{
			if (!Singleton<WatchController>.GetInstance().IsWatching)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1098u);
				cSPkg.stPkgData.stMultGameDieReq.bType = (byte)type;
				Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
			}
		}

		public static HashSet<uint> GetAssistSet(PoolObjHandle<ActorRoot> victim, PoolObjHandle<ActorRoot> attker, bool excludeAttker)
		{
			HashSet<uint> hashSet = new HashSet<uint>();
			uint num = (!attker) ? 0u : attker.handle.ObjID;
			if (victim)
			{
				List<KeyValuePair<uint, ulong>> hurtSelfActorList = victim.handle.ActorControl.hurtSelfActorList;
				for (int i = hurtSelfActorList.Count - 1; i >= 0; i--)
				{
					uint key = hurtSelfActorList[i].Key;
					if (!excludeAttker || num != key)
					{
						hashSet.Add(key);
					}
				}
			}
			if (attker)
			{
				List<KeyValuePair<uint, ulong>> helpSelfActorList = attker.handle.ActorControl.helpSelfActorList;
				for (int j = helpSelfActorList.Count - 1; j >= 0; j--)
				{
					uint key2 = helpSelfActorList[j].Key;
					if (!excludeAttker || num != key2)
					{
						hashSet.Add(key2);
					}
				}
			}
			return hashSet;
		}

		public int CalcCurrentTime()
		{
			int num = (int)Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
			SLevelContext curLvelContext = this.GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.m_isShowTrainingHelper && this.dynamicProperty != null)
			{
				num = (int)this.dynamicProperty.m_frameTimer;
			}
			return (int)((float)num * 0.001f);
		}

		public void AutoLearnSkill(PoolObjHandle<ActorRoot> hero)
		{
			if (!hero)
			{
				return;
			}
			if (hero.handle.ActorAgent.IsAutoAI())
			{
				for (int i = 3; i >= 1; i--)
				{
					if (this.IsMatchLearnSkillRule(hero, (SkillSlotType)i))
					{
						FrameCommand<LearnSkillCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<LearnSkillCommand>();
						frameCommand.cmdData.dwHeroID = hero.handle.ObjID;
						frameCommand.cmdData.bSlotType = (byte)i;
						byte bSkillLevel = 0;
						if (hero.handle.SkillControl != null && hero.handle.SkillControl.SkillSlotArray[i] != null)
						{
							bSkillLevel = (byte)hero.handle.SkillControl.SkillSlotArray[i].GetSkillLevel();
						}
						frameCommand.cmdData.bSkillLevel = bSkillLevel;
						hero.handle.ActorControl.CmdCommonLearnSkill(frameCommand);
					}
				}
			}
		}

		public bool IsMatchLearnSkillRule(PoolObjHandle<ActorRoot> hero, SkillSlotType slotType)
		{
			bool result = false;
			if (!hero || slotType < SkillSlotType.SLOT_SKILL_1 || slotType > SkillSlotType.SLOT_SKILL_3)
			{
				return result;
			}
			if (hero.handle.SkillControl != null && hero.handle.SkillControl.m_iSkillPoint > 0 && hero.handle.SkillControl.SkillSlotArray[(int)slotType] != null)
			{
				int allSkillLevel = hero.handle.SkillControl.GetAllSkillLevel();
				if (hero.handle.ValueComponent != null && allSkillLevel >= hero.handle.ValueComponent.actorSoulLevel)
				{
					return false;
				}
				int skillLevel = hero.handle.SkillControl.SkillSlotArray[(int)slotType].GetSkillLevel();
				int num = skillLevel + 1;
				int actorSoulLevel = hero.handle.ValueComponent.actorSoulLevel;
				if (skillLevel < 6)
				{
					if (slotType == SkillSlotType.SLOT_SKILL_3 && skillLevel < 3)
					{
						if (num * 4 - 1 < actorSoulLevel)
						{
							result = true;
						}
					}
					else if (slotType >= SkillSlotType.SLOT_SKILL_1 && slotType < SkillSlotType.SLOT_SKILL_3 && num * 2 - 1 <= actorSoulLevel)
					{
						result = true;
					}
				}
			}
			else if (hero.handle.SkillControl != null && hero.handle.SkillControl.m_iSkillPoint > 0 && hero.handle.SkillControl.SkillSlotArray[(int)slotType] == null)
			{
				if (slotType == SkillSlotType.SLOT_SKILL_3)
				{
					if (hero.handle.ValueComponent.actorSoulLevel >= 4)
					{
						result = true;
					}
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		private void TryAutoLearSkill(PoolObjHandle<ActorRoot> hero)
		{
			if (hero && hero.handle.SkillControl.m_iSkillPoint > 0 && hero.handle.ActorAgent != null && hero.handle.ActorAgent.IsAutoAI())
			{
				Singleton<BattleLogic>.GetInstance().AutoLearnSkill(hero);
			}
		}

		private void OnHeroSoulLvlChange(PoolObjHandle<ActorRoot> hero, int level)
		{
			this.TryAutoLearSkill(hero);
		}

		private void OnHeroSkillLvlup(PoolObjHandle<ActorRoot> hero, byte bSlotType, byte bSkillLevel)
		{
			this.TryAutoLearSkill(hero);
		}
	}
}
