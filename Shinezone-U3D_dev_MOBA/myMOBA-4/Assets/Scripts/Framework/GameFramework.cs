using AGE;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	[AutoSingleton(false)]
	public class GameFramework : MonoSingleton<GameFramework>
	{
        // 预处理完成回调代理
		public delegate void DelegateOnBaseSystemPrepareComplete();

        // 应用版本
		public static string AppVersion;

        // 渲染帧频
		public static int c_renderFPS = 30;

        // 一帧时间 (毫秒)
		public static float c_targetFrameTime = 1000f / (float)GameFramework.c_renderFPS;

        // 最后一次计算 FPS时间
		public double lastRealTime;

		public string tongCaiKey;

		public string tongCaiKey1;

		private bool lockFPS_SGame = true;

		public bool EditorPreviewMode;

		public bool CreateReplayFile = true;

		private bool m_isBaseSystemPrepared;

		private bool m_isAllSystemPrepared;

		private float frequency = 0.1f;

		private float accum;

		private int frames;

		public static float m_fFps = 0f;

		private float accumTime;

		private float lastUpdateTime;

		private Vector3 _DebugPreCamPos;

		private float _DebugPreCamTime;

		public bool LockFPS_SGame
		{
			get
			{
				return this.lockFPS_SGame;
			}
			set
			{
				if (value == this.lockFPS_SGame)
				{
					return;
				}
				this.lockFPS_SGame = value;
				this.setTargetFrameRate();
			}
		}

		public static int unityTargetFrameRate
		{
			get
			{
				int num = GameFramework.c_renderFPS;
				return 60;
			}
		}

		public void setTargetFrameRate()
		{
			if (this.lockFPS_SGame)
			{
				Application.targetFrameRate = GameFramework.unityTargetFrameRate; // 60
				base.StartCoroutine("SGame_WaitForTargetFrameRate");
			}
			else
			{
				base.StopCoroutine("SGame_WaitForTargetFrameRate");
				Application.targetFrameRate = GameFramework.c_renderFPS; // 30
			}
		}

		public void StartPrepareBaseSystem(GameFramework.DelegateOnBaseSystemPrepareComplete delegateOnBaseSystemPrepareComplete)
		{
			base.StartCoroutine(this.PrepareBaseSystem(delegateOnBaseSystemPrepareComplete));
		}

		// bsh: load ServerSettings asset bundle
		private IEnumerator InitPUN()
		{
			string fullPath = Application.streamingAssetsPath + "/PhotonServerSettings.assetbundle";
			WWW www = new WWW (fullPath);
			yield return www;
			if (!String.IsNullOrEmpty (www.error)) {
				UnityEngine.Debug.LogWarning ("InitPUN, error: " + www.error);
				yield break;
			}

			UnityEngine.Object o = null;
			if (www.assetBundle != null) {
				UnityEngine.Object[] allObj = www.assetBundle.LoadAll();
				if (allObj.Length > 0) {
					o = allObj[0];
				}
			}

			if (o == null) {
				UnityEngine.Debug.LogWarning ("InitPUN, can not read settings!");
				yield break;
			}
			myHackClass.mySettings = o as ServerSettings;
		}

		private IEnumerator PrepareBaseSystem(GameFramework.DelegateOnBaseSystemPrepareComplete delegateOnBaseSystemPrepareComplete)
		{
			Singleton<EventRouter>.CreateInstance();
			Singleton<CTextManager>.CreateInstance();
			Singleton<CTextManager>.GetInstance().LoadLocalText();
			yield return null;

			// bsh: init Photon
			yield return StartCoroutine(InitPUN());
			Singleton<NetworkModule>.CreateInstance();
			MonoSingleton<TssdkSys>.GetInstance();
			yield return null;

			Singleton<FrameSynchr>.CreateInstance();
			yield return null;

			m_isBaseSystemPrepared = true;
			if (delegateOnBaseSystemPrepareComplete != null)
			{
				delegateOnBaseSystemPrepareComplete();
			}
		}

		public void PrepareSoundSystem(bool applySoundSettings = false)
		{
			Singleton<CSoundManager>.GetInstance().Prepare();
			Singleton<CSoundManager>.GetInstance().LoadBank("Music_Login", CSoundManager.BankType.Global);
			Singleton<CSoundManager>.GetInstance().LoadBank("Music", CSoundManager.BankType.Global);
			Singleton<CSoundManager>.GetInstance().LoadBank("Ambience", CSoundManager.BankType.Global);
			Singleton<CUIManager>.GetInstance().LoadSoundBank();
			Singleton<CSoundManager>.GetInstance().LoadBank("Audio_Control", CSoundManager.BankType.Global);
			if (applySoundSettings)
			{
				GameSettings.ApplySettings_Music();
				GameSettings.ApplySettings_Sound();
			}
		}

		protected override void Init()
		{
            // 设置不黑屏
			Screen.sleepTimeout = -1;
            // 获取应用版本号
			GameFramework.AppVersion = CVersion.GetAppVersion();
			this.setTargetFrameRate();
		}

		public IEnumerator PrepareGameSystem()
		{
			Singleton<GameDataMgr>.CreateInstance();
			yield return StartCoroutine(Singleton<GameDataMgr>.GetInstance().LoadDataBin());

			Singleton<CTextManager>.GetInstance().LoadText(GameDataMgr.s_textDatabin);
			Singleton<CUIUtility>.CreateInstance();
			Singleton<CFriendContoller>.instance.model.LoadPreconfigVerifyContentList();
			Singleton<CFriendContoller>.instance.model.LoadIntimacyConfig();
			Singleton<CFriendContoller>.instance.model.FRData.LoadConfig();
			Singleton<CFriendContoller>.instance.model.friendRecruit.LoadConfig();
			Singleton<CPlayerSocialInfoController>.CreateInstance();
			Singleton<CChatController>.CreateInstance();
			Singleton<CChatController>.instance.model.Load_HeroSelect_ChatTemplate();
			Singleton<CTaskSys>.instance.model.Load_Share_task();
			Singleton<CTaskSys>.instance.model.Load_Task_Tab_String();
			Singleton<CInviteSystem>.instance.Load_Config_Text();
			Singleton<CMailSys>.instance.LoadTxtStr();
			Singleton<InBattleMsgMgr>.instance.ParseCfgData();
			Singleton<CTaskSys>.instance.model.ParseLevelRewardData();
			KillNotify.LoadConfig();
			PrepareSoundSystem(false);
			yield return null;

			InitCoreSys();
			yield return null;

			if (!EditorPreviewMode)
			{
				InitPeripherySys();
				yield return null;
			}

			InitBattleSys();
			yield return null;

			InitMiscSys();
			yield return null;

			Singleton<GameReplayModule>.GetInstance().Reset();
			m_isAllSystemPrepared = true;
		}

		private IEnumerator SGame_WaitForTargetFrameRate()
		{
			yield return new WaitForEndOfFrame();

			double dt = (Time.realtimeSinceStartup - lastRealTime) * 1000.0;
			double sleepTime = GameFramework.c_targetFrameTime - dt;
			int sleepTimeInt = Mathf.Clamp((int) sleepTime, 0, GameFramework.c_renderFPS);
			if (sleepTimeInt > 0)
			{
				Thread.Sleep(sleepTimeInt);
			}
			lastRealTime = Time.realtimeSinceStartup;
			float fdelta = Time.realtimeSinceStartup - lastUpdateTime;
			accumTime += fdelta;
			accum += 1f / fdelta;
			lastUpdateTime = Time.realtimeSinceStartup;
			frames++;
			if (accumTime >= frequency)
			{
				GameFramework.m_fFps = accum / ((float) frames);
				accumTime = 0f;
				accum = 0f;
				frames = 0;
			}

			yield return new WaitForEndOfFrame();
		}

		protected void InitBaseSys()
		{
			Singleton<CTimerManager>.CreateInstance();
			Singleton<CResourceManager>.CreateInstance();
			Singleton<ResourceLoader>.GetInstance();
			Singleton<CGameObjectPool>.CreateInstance();
			Singleton<CUIEventManager>.CreateInstance();
			Singleton<CUIManager>.CreateInstance();
			MonoSingleton<CVersionUpdateSystem>.GetInstance();
			Singleton<CCheatSystem>.CreateInstance();
			Singleton<GameStateCtrl>.CreateInstance();
			OutlineFilter.EnableSurfaceShaderOutline(false);
			DynamicShadow.InitDefaultGlobalVariables();
		}

		protected void InitCoreSys()
		{
			Singleton<GameEventSys>.CreateInstance();
			MonoSingleton<GameLoader>.GetInstance();
			MonoSingleton<ActionManager>.GetInstance();
			Singleton<InputModule>.CreateInstance();
			Singleton<GameInput>.GetInstance();
			Singleton<GameLogic>.CreateInstance();
			Singleton<LobbyLogic>.CreateInstance();
			Singleton<BattleLogic>.CreateInstance();
			Singleton<GameReplayModule>.CreateInstance();
			Singleton<CUICommonSystem>.CreateInstance();
			Singleton<GameBuilderEx>.CreateInstance();
			Singleton<GameContextEx>.CreateInstance();
		}

		protected void InitPeripherySys()
		{
			GameSettings.Init();
			Singleton<CRoleInfoManager>.CreateInstance();
			Singleton<CLoginSystem>.CreateInstance();
			Singleton<CBagSystem>.CreateInstance();
			Singleton<RankingSystem>.CreateInstance();
			Singleton<SevenDayCheckSystem>.CreateInstance();
			Singleton<SettlementSystem>.CreateInstance();
			Singleton<QQVipWidget>.CreateInstance();
			Singleton<CMallSystem>.CreateInstance();
			Singleton<CMallSymbolGiftController>.CreateInstance();
			Singleton<CAdventureSys>.CreateInstance();
			Singleton<CMatchingSystem>.CreateInstance();
			Singleton<CRoomSystem>.CreateInstance();
			Singleton<CInviteSystem>.CreateInstance();
			Singleton<CLadderSystem>.CreateInstance();
			Singleton<CHeroChooseSys>.CreateInstance();
			Singleton<CHeroOverviewSystem>.CreateInstance();
			Singleton<CHeroInfoSystem2>.CreateInstance();
			Singleton<CHeroSkinBuyManager>.CreateInstance();
			Singleton<CFriendContoller>.CreateInstance();
			Singleton<CChatController>.CreateInstance();
			Singleton<BurnExpeditionController>.CreateInstance();
			Singleton<CShopSystem>.CreateInstance();
			Singleton<CPvPHeroShop>.CreateInstance();
			Singleton<CQualifyingSystem>.CreateInstance();
			Singleton<CLobbySystem>.CreateInstance();
			Singleton<CSettingsSys>.CreateInstance();
			Singleton<CHeroSelectBaseSystem>.CreateInstance();
			Singleton<CHeroSelectNormalSystem>.CreateInstance();
			Singleton<CSymbolSystem>.CreateInstance();
			Singleton<TreasureChestMgr>.CreateInstance();
			Singleton<CSettleSystem>.CreateInstance();
			Singleton<PVESettleSys>.CreateInstance();
			Singleton<NewbieGuideDataManager>.CreateInstance();
			Singleton<CRoleRegisterSys>.CreateInstance();
			Singleton<CUILoadingSystem>.CreateInstance();
			Singleton<CMailSys>.CreateInstance();
			Singleton<CPurchaseSys>.CreateInstance();
			Singleton<CGuildSystem>.GetInstance();
			Singleton<CGuildMatchSystem>.GetInstance();
			Singleton<CAchievementSystem>.GetInstance();
			Singleton<CExperienceCardSystem>.GetInstance();
			Singleton<CTaskSys>.CreateInstance();
			Singleton<HeroChooseLogic>.CreateInstance();
			MonoSingleton<NewbieGuideManager>.GetInstance();
			Singleton<SingleGameSettleMgr>.CreateInstance();
			Singleton<CPlayerInfoSystem>.CreateInstance();
			Singleton<CMiniPlayerInfoSystem>.CreateInstance();
			Singleton<CArenaSystem>.CreateInstance();
			Singleton<CNewbieAchieveSys>.CreateInstance();
			Singleton<CHeroAnimaSystem>.CreateInstance();
			Singleton<CUIParticleSystem>.CreateInstance();
			Singleton<CPaySystem>.CreateInstance();
			Singleton<CPartnerSystem>.CreateInstance();
			MonoSingleton<IDIPSys>.GetInstance();
			MonoSingleton<PandroaSys>.GetInstance();
			Singleton<HeadIconSys>.GetInstance();
			MonoSingleton<BannerImageSys>.GetInstance();
			MonoSingleton<ShareSys>.GetInstance();
			MonoSingleton<TGPSDKSys>.GetInstance();
			MonoSingleton<TGASys>.GetInstance();
			MonoSingleton<NoticeSys>.GetInstance();
			Singleton<CAddSkillSys>.GetInstance();
			MonoSingleton<NobeSys>.GetInstance();
			MonoSingleton<VoiceSys>.GetInstance();
			Singleton<CMiShuSystem>.GetInstance();
			Singleton<CUnionBattleEntrySystem>.GetInstance();
			Singleton<CUnionBattleRankSystem>.GetInstance();
			Singleton<CEquipSystem>.CreateInstance();
			Singleton<CUnionBattleEntrySystem>.CreateInstance();
			Singleton<CLoudSpeakerSys>.CreateInstance();
			Singleton<COBSystem>.CreateInstance();
			Singleton<CReplayKitSys>.CreateInstance();
			Singleton<CRecordUseSDK>.CreateInstance();
			Singleton<CSecurePwdSystem>.CreateInstance();
			Singleton<Day14CheckSystem>.CreateInstance();
			Singleton<CBattleStatCompetitionSystem>.CreateInstance();
		}

		protected void InitBattleSys()
		{
			BTConfig.SetBTConfig();
			Singleton<GamePlayerCenter>.CreateInstance();
			Singleton<ActorDataCenter>.CreateInstance();
			Singleton<ShenFuSystem>.CreateInstance();
			MonoSingleton<CameraSystem>.GetInstance();
			Singleton<StarSystem>.CreateInstance();
			Singleton<BattleStatistic>.CreateInstance();
			Singleton<CBattleSystem>.CreateInstance();
			Singleton<DropItemMgr>.CreateInstance();
			Singleton<PassiveCreater<PassiveEvent, PassiveEventAttribute>>.CreateInstance();
			Singleton<PassiveCreater<PassiveCondition, PassiveConditionAttribute>>.CreateInstance();
			Singleton<EnergyCreater<BaseEnergyLogic, EnergyAttribute>>.CreateInstance();
			Singleton<TipProcessor>.CreateInstance();
			Singleton<GameBuilder>.CreateInstance();
			Singleton<GameObjMgr>.CreateInstance();
			Singleton<SceneManagement>.CreateInstance();
			Singleton<SkillDetectionControl>.CreateInstance();
			MonoSingleton<SceneMgr>.GetInstance();
			Singleton<SkillSelectControl>.CreateInstance();
			Singleton<SkillFuncDelegator>.CreateInstance();
			Singleton<SkillIndicateSystem>.CreateInstance();
			Singleton<CBattleGuideManager>.CreateInstance();
			Singleton<CTrainingHelper>.CreateInstance();
			Singleton<CSurrenderSystem>.CreateInstance();
			Singleton<SoundCookieSys>.CreateInstance();
			MonoSingleton<VoiceInteractionSys>.GetInstance();
			Singleton<TeleportTargetSelector>.GetInstance();
			Singleton<SpeedAdjuster>.CreateInstance();
		}

		protected void InitMiscSys()
		{
		}

		public virtual void Start()
		{
			Application.runInBackground = true;
			try
			{
				DebugHelper.CustomLog("GameFramework Start, Version:{0}.R({1}), Unity:{2}", new object[]
				{
					CVersion.GetAppVersion(),
					CVersion.GetRevisonNumber(),
					Application.unityVersion
				});
			}
			catch (Exception)
			{
			}
            // 日志系统
			Singleton<BugLocateLogSys>.CreateInstance();
            // SDK： 登录、支付、分享
			Singleton<ApolloHelper>.GetInstance().EnableBugly();
            // 设置横屏和朝向
			Screen.autorotateToLandscapeLeft = true;
			Screen.autorotateToLandscapeRight = true;
			Screen.autorotateToPortrait = false;
			Screen.autorotateToPortraitUpsideDown = false;
			Screen.orientation = ScreenOrientation.AutoRotation;
            // 设置分辨率
			GameSettings.RefreshResolution();
			AndroidJavaClass androidJavaClass = new AndroidJavaClass(ApolloConfig.GetGameUtilityString());
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("EnableInput", new object[]
				{
					true
				});
				androidJavaClass.Dispose();
			}
			UnityEngine.Debug.Log("android EnableInput");
			CVersionUpdateSystem.SetIIPSServerTypeFromFile();
			this.InitBaseSys();
			Singleton<GameStateCtrl>.GetInstance().Initialize();
			Singleton<GameStateCtrl>.GetInstance().GotoState("LaunchState");
		}

		private void Update()
		{
			if (Singleton<BattleLogic>.HasInstance() && Singleton<BattleLogic>.GetInstance().isFighting)
			{
				FPSStatistic.Update();
			}
			try
			{
				if (this.m_isBaseSystemPrepared)
				{
					Singleton<CGameObjectPool>.GetInstance().Update();
					Singleton<CResourceManager>.GetInstance().CustomUpdate();
				}
				if (this.m_isAllSystemPrepared)
				{
					if (!Singleton<WatchController>.GetInstance().IsWatching)
					{
						Singleton<InputModule>.GetInstance().UpdateFrame();
						Singleton<GameInput>.GetInstance().UpdateFrame();
					}
					Singleton<NetworkModule>.GetInstance().UpdateFrame();
					Singleton<WatchController>.GetInstance().UpdateFrame();
					if (Singleton<GameReplayModule>.GetInstance().IsReplaying)
					{
						Singleton<GameReplayModule>.GetInstance().UpdateFrame();
					}
					Singleton<FrameWindow>.GetInstance().UpdateFrame();
					Singleton<FrameSynchr>.GetInstance().UpdateFrame();
				}
			}
			catch (Exception ex)
			{
				Singleton<CCheatSystem>.GetInstance().RecordErrorLog(string.Format("Exception Occur when GameFramework.FixedUpdate, Message:{0}", ex.Message));
				throw ex;
			}
			this.UpdateElse();
		}

		private void UpdateElse()
		{
			try
			{
				if (this.m_isAllSystemPrepared)
				{
					Singleton<CBattleSystem>.GetInstance().Update();
					Singleton<NewbieWeakGuideControl>.GetInstance().Update();
					Singleton<CChatController>.GetInstance().Update();
					Singleton<CFriendContoller>.GetInstance().Update();
				}
				if (Singleton<BattleLogic>.HasInstance())
				{
					Singleton<BattleLogic>.GetInstance().Update();
				}
				Singleton<CTimerManager>.GetInstance().Update();
				Singleton<CUIManager>.GetInstance().Update();
				MonoSingleton<TssdkSys>.GetInstance().Update();
				if (!this.lockFPS_SGame)
				{
					float num = Time.realtimeSinceStartup - this.lastUpdateTime;
					this.accumTime += num;
					this.accum += 1f / num;
					this.lastUpdateTime = Time.realtimeSinceStartup;
					this.frames++;
					if (this.accumTime >= this.frequency)
					{
						GameFramework.m_fFps = this.accum / (float)this.frames;
						this.accumTime = 0f;
						this.accum = 0f;
						this.frames = 0;
					}
				}
			}
			catch (Exception ex)
			{
				Singleton<CCheatSystem>.GetInstance().RecordErrorLog(string.Format("Exception Occur when GameFramework.UpdateElse, Message:{0}", ex.Message));
				throw ex;
			}
		}

		private void LateUpdate()
		{
			try
			{
				if (this.m_isAllSystemPrepared)
				{
					Singleton<GameLogic>.GetInstance().LateUpdate();
					Singleton<CBattleSystem>.GetInstance().LateUpdate();
				}
				try
				{
					Singleton<CUIManager>.GetInstance().LateUpdate();
				}
				catch (Exception ex)
				{
					DebugHelper.Assert(false, "Exception Occur when CUIManager.LateUpdate, Message:{0}, Stack:{1}", new object[]
					{
						ex.Message,
						ex.StackTrace
					});
				}
				if (this.m_isAllSystemPrepared)
				{
					Singleton<LobbyLogic>.GetInstance().LateUpdate();
				}
			}
			catch (Exception ex2)
			{
				Singleton<CCheatSystem>.GetInstance().RecordErrorLog(string.Format("Exception Occur when GameFramework.LateUpdate, Message:{0}", ex2.Message));
				throw ex2;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			Singleton<BugLocateLogSys>.DestroyInstance();
		}

		protected void DestroyBaseSys()
		{
			Singleton<GameStateCtrl>.DestroyInstance();
			Singleton<CCheatSystem>.DestroyInstance();
			Singleton<CTextManager>.DestroyInstance();
			Singleton<CUIEventManager>.DestroyInstance();
			Singleton<CUIManager>.DestroyInstance();
			Singleton<CSoundManager>.DestroyInstance();
			Singleton<CGameObjectPool>.DestroyInstance();
			Singleton<CResourceManager>.DestroyInstance();
			Singleton<CTimerManager>.DestroyInstance();
			Singleton<EventRouter>.DestroyInstance();
			MonoSingleton<TssdkSys>.DestroyInstance();
		}

		protected void DestroyCoreSys()
		{
			Singleton<BattleLogic>.DestroyInstance();
			Singleton<LobbyLogic>.DestroyInstance();
			Singleton<GameLogic>.DestroyInstance();
			Singleton<GameInput>.DestroyInstance();
			Singleton<InputModule>.DestroyInstance();
			MonoSingleton<ActionManager>.DestroyInstance();
			MonoSingleton<GameLoader>.DestroyInstance();
			Singleton<ResourceLoader>.DestroyInstance();
			Singleton<GameEventSys>.DestroyInstance();
			Singleton<GameDataMgr>.DestroyInstance();
		}

		protected void DestoryPeripherySys()
		{
			Singleton<Day14CheckSystem>.DestroyInstance();
			Singleton<CEquipSystem>.DestroyInstance();
			Singleton<CUIParticleSystem>.DestroyInstance();
			MonoSingleton<NewbieGuideManager>.DestroyInstance();
			Singleton<HeroChooseLogic>.DestroyInstance();
			Singleton<CTaskSys>.DestroyInstance();
			Singleton<CGuildSystem>.DestroyInstance();
			Singleton<CGuildMatchSystem>.DestroyInstance();
			Singleton<CAchievementSystem>.DestroyInstance();
			Singleton<CExperienceCardSystem>.DestroyInstance();
			Singleton<CPurchaseSys>.DestroyInstance();
			Singleton<CMailSys>.DestroyInstance();
			Singleton<CUILoadingSystem>.DestroyInstance();
			Singleton<CRoleRegisterSys>.DestroyInstance();
			Singleton<NewbieGuideDataManager>.DestroyInstance();
			Singleton<PVESettleSys>.DestroyInstance();
			Singleton<CSettleSystem>.DestroyInstance();
			Singleton<TreasureChestMgr>.DestroyInstance();
			Singleton<CSymbolSystem>.DestroyInstance();
			Singleton<CHeroSelectNormalSystem>.DestroyInstance();
			Singleton<CSettingsSys>.DestroyInstance();
			Singleton<CLobbySystem>.DestroyInstance();
			Singleton<CQualifyingSystem>.DestroyInstance();
			Singleton<CPvPHeroShop>.DestroyInstance();
			Singleton<CShopSystem>.DestroyInstance();
			Singleton<BurnExpeditionController>.DestroyInstance();
			Singleton<CChatController>.DestroyInstance();
			Singleton<CFriendContoller>.DestroyInstance();
			Singleton<CHeroOverviewSystem>.DestroyInstance();
			Singleton<CHeroInfoSystem2>.DestroyInstance();
			Singleton<CHeroSkinBuyManager>.DestroyInstance();
			Singleton<CHeroChooseSys>.DestroyInstance();
			Singleton<CLadderSystem>.DestroyInstance();
			Singleton<CInviteSystem>.DestroyInstance();
			Singleton<CRoomSystem>.DestroyInstance();
			Singleton<CMatchingSystem>.DestroyInstance();
			Singleton<CAdventureSys>.DestroyInstance();
			Singleton<CMallSystem>.DestroyInstance();
			Singleton<CMallSymbolGiftController>.DestroyInstance();
			Singleton<CBagSystem>.DestroyInstance();
			Singleton<CRoleInfoManager>.DestroyInstance();
			Singleton<CUICommonSystem>.DestroyInstance();
			Singleton<SingleGameSettleMgr>.DestroyInstance();
			Singleton<CHeroAnimaSystem>.DestroyInstance();
			Singleton<CPaySystem>.DestroyInstance();
			Singleton<CPartnerSystem>.DestroyInstance();
			MonoSingleton<IDIPSys>.DestroyInstance();
			MonoSingleton<PandroaSys>.DestroyInstance();
			Singleton<HeadIconSys>.DestroyInstance();
			MonoSingleton<BannerImageSys>.DestroyInstance();
			MonoSingleton<ShareSys>.DestroyInstance();
			MonoSingleton<TGPSDKSys>.DestroyInstance();
			MonoSingleton<TGASys>.DestroyInstance();
			MonoSingleton<NoticeSys>.DestroyInstance();
			Singleton<CAddSkillSys>.DestroyInstance();
			MonoSingleton<NobeSys>.DestroyInstance();
			MonoSingleton<VoiceSys>.DestroyInstance();
			Singleton<CPlayerInfoSystem>.DestroyInstance();
			Singleton<CMiniPlayerInfoSystem>.DestroyInstance();
			Singleton<CLoudSpeakerSys>.DestroyInstance();
			Singleton<COBSystem>.DestroyInstance();
			Singleton<CBattleStatCompetitionSystem>.DestroyInstance();
		}

		protected void DestoryBattleSys()
		{
			Singleton<SpeedAdjuster>.DestroyInstance();
			Singleton<SkillIndicateSystem>.DestroyInstance();
			Singleton<SkillFuncDelegator>.DestroyInstance();
			Singleton<SkillSelectControl>.DestroyInstance();
			MonoSingleton<SceneMgr>.DestroyInstance();
			Singleton<SkillDetectionControl>.DestroyInstance();
			Singleton<GameObjMgr>.DestroyInstance();
			Singleton<SceneManagement>.DestroyInstance();
			Singleton<GameBuilder>.DestroyInstance();
			Singleton<TipProcessor>.DestroyInstance();
			Singleton<PassiveCreater<PassiveCondition, PassiveConditionAttribute>>.DestroyInstance();
			Singleton<PassiveCreater<PassiveEvent, PassiveEventAttribute>>.DestroyInstance();
			Singleton<EnergyCreater<BaseEnergyLogic, EnergyAttribute>>.DestroyInstance();
			Singleton<DropItemMgr>.DestroyInstance();
			Singleton<CBattleSystem>.DestroyInstance();
			Singleton<BattleStatistic>.DestroyInstance();
			Singleton<StarSystem>.DestroyInstance();
			MonoSingleton<CameraSystem>.DestroyInstance();
			Singleton<ActorDataCenter>.DestroyInstance();
			Singleton<GamePlayerCenter>.DestroyInstance();
			Singleton<CTrainingHelper>.DestroyInstance();
			Singleton<CSurrenderSystem>.DestroyInstance();
			Singleton<SoundCookieSys>.DestroyInstance();
			MonoSingleton<VoiceInteractionSys>.DestroyInstance();
			Singleton<TeleportTargetSelector>.DestroyInstance();
		}

		protected void DestoryMiscSys()
		{
			Singleton<CheatCommandRegister>.DestroyInstance();
			Singleton<CheatCommandsRepository>.DestroyInstance();
			if (MonoSingleton<ConsoleWindow>.HasInstance())
			{
				MonoSingleton<ConsoleWindow>.DestroyInstance();
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (this.m_isAllSystemPrepared && !pauseStatus && !Singleton<BattleLogic>.GetInstance().isRuning && !Singleton<CMatchingSystem>.GetInstance().IsInMatching && !Singleton<CMatchingSystem>.GetInstance().IsInMatchingTeam && Singleton<CLobbySystem>.GetInstance().IsInLobbyForm())
			{
				CPaySystem.SendReqQueryAcntDianQuan(CS_COUPONS_PAYTYPE.CS_COUPONS_PAYTYPE_QUERY, true);
			}
			DebugHelper.CustomLog("OnApplicationPause {0}", new object[]
			{
				pauseStatus
			});
		}

		private void OnApplicationFocus(bool focus)
		{
			if (focus)
			{
				Singleton<CSoundManager>.instance.PostEvent("All_Resume", null);
			}
			else
			{
				Singleton<CSoundManager>.instance.PostEvent("All_Pause", null);
			}
		}
	}
}
