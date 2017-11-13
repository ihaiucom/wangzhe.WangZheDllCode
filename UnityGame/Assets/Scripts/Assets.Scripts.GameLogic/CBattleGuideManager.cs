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
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic
{
	internal class CBattleGuideManager : Singleton<CBattleGuideManager>
	{
		public enum EBattleGuideFormType
		{
			Invalid,
			Intro,
			Gesture,
			JoyStick,
			Intro_2,
			BaojunAlert,
			BaojunTips,
			BaojunTips2,
			SkillGesture2,
			SkillGesture3,
			SkillGesture2Cancel,
			BigSkill,
			BlueBuff,
			Duke,
			Grass,
			Heal,
			Intro_5V5,
			Intro_Jungle,
			Intro_LunPan,
			Map_01,
			Map_02,
			Map_03,
			RedBuff,
			Tower,
			XiaoLong,
			Map_BlueBuff,
			Map_RedBuff,
			BigMapGuide,
			SelectModeGuide,
			CustomRecomEquipIntro,
			FingerMovement,
			ADBigSkill,
			APBigSkill,
			BloodReturn,
			Count
		}

		protected enum enNewbieProfitWidgets
		{
			None = -1,
			WinLoseTitle,
			ExpInfo,
			CoinInfo,
			SymbolCoinInfo,
			GuildInfo,
			LadderInfo,
			PvpMapInfo,
			GuildPointMaxTip
		}

		public const string SETTLE_FORM_PATH = "UGUI/Form/System/Newbie/Form_NewbieSettle.prefab";

		public const string HALL_INTRO_FORM_PATH = "UGUI/Form/System/Newbie/Form_IntroHall.prefab";

		private const string INTRO_33_FORM_PATH = "UGUI/Form/System/Newbie/Form_Intro_3V3.prefab";

		public const string OLD_PLAYER_FIRST_FORM_PATH = "UGUI/Form/System/Newbie/Form_OldTip.prefab";

		private const float ExpBarWidth = 220.3f;

		private const float TweenTime = 2f;

		public static string TUTORIAL_END_FORM_PATH = "UGUI/Form/System/Newbie/Form_End_Tutorial.prefab";

		private static string PROFIT_FORM_NAME = "UGUI/Form/System/Newbie/Form_NewbieProfit.prefab";

		private static string NEWBIE_GOLDAWARD_FORM_PATH = "UGUI/Form/System/Newbie/Form_AwardNewbie";

		public static bool ms_bOldPlayerFormOpened = false;

		public static readonly uint AdvanceGuideLevelID = GameDataMgr.globalInfoDatabin.GetDataByKey(55u).dwConfValue;

		public static readonly uint GuideLevelID3v3 = GameDataMgr.globalInfoDatabin.GetDataByKey(117u).dwConfValue;

		public static readonly uint GuideLevelIDCasting = GameDataMgr.globalInfoDatabin.GetDataByKey(119u).dwConfValue;

		public static readonly uint GuideLevelIDJungle = GameDataMgr.globalInfoDatabin.GetDataByKey(121u).dwConfValue;

		public static readonly uint Warm1v1SpecialLevelId = GameDataMgr.globalInfoDatabin.GetDataByKey(147u).dwConfValue;

		private static readonly uint GuideLevel1v1Tank = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_LEVEL);

		private static readonly uint GuideLevel1v1AD = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_LEVEL_AD);

		private static readonly uint GuideLevel1v1AP = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_LEVEL_AP);

		private static readonly uint GuideLevel5v5Tank = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_LEVEL);

		private static readonly uint GuideLevel5v5AD = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_LEVEL_AD);

		private static readonly uint GuideLevel5v5AP = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_LEVEL_AP);

		public static int TIME_OUT = 30000;

		public static bool CanSelectHeroOnTrainLevelEntry = false;

		private string[] GuideFormPathList = new string[34];

		public bool bReEntry;

		public bool bTrainingAdv;

		public CBattleGuideManager.EBattleGuideFormType m_lastFormType;

		private CUIFormScript curOpenForm;

		private COMDT_REWARD_DETAIL TrainLevelReward = new COMDT_REWARD_DETAIL();

		private bool m_bWin;

		private NewbieBannerIntroDialog m_BannerIntroDialog;

		private bool _PauseGame;

		private Dictionary<object, int> m_pauseGameStack = new Dictionary<object, int>();

		private LTDescr coinLtd;

		private LTDescr expLtd;

		private LTDescr symbolCoinfLtd;

		private float expFrom;

		private float expTo;

		private float coinFrom;

		private float coinTo;

		private float symbolCoinFrom;

		private float symbolCoinTo;

		private Text coinTweenText;

		private Text symbolCoinTweenText;

		private RectTransform expTweenRect;

		private static uint lvUpGrade = 0u;

		private GuideInfo m_guideInfo = default(GuideInfo);

		public bool bPauseGame
		{
			get
			{
				return this._PauseGame;
			}
			private set
			{
				if (value)
				{
					Singleton<GameInput>.instance.StopInput();
				}
				this._PauseGame = value;
			}
		}

		private void UpdateGamePause(bool bEffectTimeScale)
		{
			this.bPauseGame = (this.m_pauseGameStack.get_Count() != 0);
			Singleton<FrameSynchr>.instance.SetSynchrRunning(!this.bPauseGame);
			if (bEffectTimeScale)
			{
				DebugHelper.Assert(!Singleton<FrameSynchr>.instance.bActive, "frame synchr active forbid set timeScale");
				Time.timeScale = (float)(this.bPauseGame ? 0 : 1);
			}
		}

		public void PauseGame(object sender, bool bEffectTimeScale = true)
		{
			if (sender == null)
			{
				return;
			}
			if (this.m_pauseGameStack.ContainsKey(sender))
			{
				Dictionary<object, int> pauseGameStack;
				int num = (pauseGameStack = this.m_pauseGameStack).get_Item(sender);
				pauseGameStack.set_Item(sender, num + 1);
			}
			else
			{
				this.m_pauseGameStack.Add(sender, 1);
			}
			this.UpdateGamePause(bEffectTimeScale);
		}

		public void ResumeGame(object sender)
		{
			if (sender == null)
			{
				return;
			}
			if (!this.m_pauseGameStack.ContainsKey(sender))
			{
				return;
			}
			Dictionary<object, int> pauseGameStack;
			int num = (pauseGameStack = this.m_pauseGameStack).get_Item(sender);
			int num2 = num - 1;
			pauseGameStack.set_Item(sender, num2);
			if (num2 == 0)
			{
				this.m_pauseGameStack.Remove(sender);
			}
			this.UpdateGamePause(true);
		}

		public void resetPause()
		{
			this.m_pauseGameStack.Clear();
			this.bPauseGame = false;
			Singleton<FrameSynchr>.instance.SetSynchrRunning(!this.bPauseGame);
			Time.timeScale = 1f;
		}

		public string QueryFormPath(CBattleGuideManager.EBattleGuideFormType inFormType)
		{
			return this.GuideFormPathList[(int)inFormType];
		}

		public override void Init()
		{
			base.Init();
			for (int i = 0; i < this.GuideFormPathList.Length; i++)
			{
				string text = string.Format("Newbie/Form_{0}.prefab", ((CBattleGuideManager.EBattleGuideFormType)i).ToString("G"));
				text = "UGUI/Form/System/" + text;
				this.GuideFormPathList[i] = text;
			}
			this.m_BannerIntroDialog = new NewbieBannerIntroDialog();
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseIntroForm, new CUIEventManager.OnUIEventHandler(this.onCloseIntro));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseIntroForm2, new CUIEventManager.OnUIEventHandler(this.onCloseIntro2));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseGestureGuide, new CUIEventManager.OnUIEventHandler(this.onCloseGesture));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseJoyStickGuide, new CUIEventManager.OnUIEventHandler(this.onCloseJoyStick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseSettle, new CUIEventManager.OnUIEventHandler(this.onCloseSettle));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_ConfirmAdvanceGuide, new CUIEventManager.OnUIEventHandler(this.EnterAdvanceGuide));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_RejectAdvanceGuide, new CUIEventManager.OnUIEventHandler(this.RejectAdvanceGuide));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseTyrantAlert, new CUIEventManager.OnUIEventHandler(this.onCloseTyrantAlert));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseTyrantTip, new CUIEventManager.OnUIEventHandler(this.onCloseTyrantTip));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseTyrantTip2, new CUIEventManager.OnUIEventHandler(this.onCloseTyrantTip2));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseSkillGesture, new CUIEventManager.OnUIEventHandler(this.onCloseSkillGesture));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_OldPlayerFirstFormClose, new CUIEventManager.OnUIEventHandler(this.onOldPlayerFirstFormClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonUp, new CUIEventManager.OnUIEventHandler(this.OnSkillButtonUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_BannerIntroDlg_ClickPrePage, new CUIEventManager.OnUIEventHandler(this.OnClickToPrePage));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_BannerIntroDlg_ClickNextPage, new CUIEventManager.OnUIEventHandler(this.OnClickToNextPage));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_BannerIntroDlg_DragStart, new CUIEventManager.OnUIEventHandler(this.OnDragStart));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_BannerIntroDlg_DragEnd, new CUIEventManager.OnUIEventHandler(this.OnDragEnd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_BannerIntroDlg_ClickConfirm, new CUIEventManager.OnUIEventHandler(this.OnDialogClickConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_BannerIntroDlg_Close, new CUIEventManager.OnUIEventHandler(this.OnDialogClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_BannerIntroDlg_OnMoveTimeUp, new CUIEventManager.OnUIEventHandler(this.onMoveTimeUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_ClickVictoryTips, new CUIEventManager.OnUIEventHandler(this.onClickVictoryTips));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_ClickProfitContinue, new CUIEventManager.OnUIEventHandler(this.onClickProfitContinue));
		}

		public override void UnInit()
		{
			this.m_BannerIntroDialog = null;
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseIntroForm, new CUIEventManager.OnUIEventHandler(this.onCloseIntro));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseIntroForm2, new CUIEventManager.OnUIEventHandler(this.onCloseIntro2));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseGestureGuide, new CUIEventManager.OnUIEventHandler(this.onCloseGesture));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseJoyStickGuide, new CUIEventManager.OnUIEventHandler(this.onCloseJoyStick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseSettle, new CUIEventManager.OnUIEventHandler(this.onCloseSettle));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_ConfirmAdvanceGuide, new CUIEventManager.OnUIEventHandler(this.EnterAdvanceGuide));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_RejectAdvanceGuide, new CUIEventManager.OnUIEventHandler(this.RejectAdvanceGuide));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseTyrantAlert, new CUIEventManager.OnUIEventHandler(this.onCloseTyrantAlert));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseTyrantTip, new CUIEventManager.OnUIEventHandler(this.onCloseTyrantTip));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseTyrantTip2, new CUIEventManager.OnUIEventHandler(this.onCloseTyrantTip2));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseSkillGesture, new CUIEventManager.OnUIEventHandler(this.onCloseSkillGesture));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_OldPlayerFirstFormClose, new CUIEventManager.OnUIEventHandler(this.onOldPlayerFirstFormClose));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonUp, new CUIEventManager.OnUIEventHandler(this.OnSkillButtonUp));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_BannerIntroDlg_ClickPrePage, new CUIEventManager.OnUIEventHandler(this.OnClickToPrePage));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_BannerIntroDlg_ClickNextPage, new CUIEventManager.OnUIEventHandler(this.OnClickToNextPage));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_BannerIntroDlg_DragStart, new CUIEventManager.OnUIEventHandler(this.OnDragStart));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_BannerIntroDlg_DragEnd, new CUIEventManager.OnUIEventHandler(this.OnDragEnd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_BannerIntroDlg_ClickConfirm, new CUIEventManager.OnUIEventHandler(this.OnDialogClickConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_BannerIntroDlg_Close, new CUIEventManager.OnUIEventHandler(this.OnDialogClose));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_BannerIntroDlg_OnMoveTimeUp, new CUIEventManager.OnUIEventHandler(this.onMoveTimeUp));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_ClickVictoryTips, new CUIEventManager.OnUIEventHandler(this.onClickVictoryTips));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_ClickProfitContinue, new CUIEventManager.OnUIEventHandler(this.onClickProfitContinue));
			base.UnInit();
		}

		private void onCloseIntro(CUIEvent uiEvent)
		{
			this.CloseFormShared(CBattleGuideManager.EBattleGuideFormType.Intro);
		}

		private void onCloseIntro2(CUIEvent uiEvent)
		{
			this.CloseFormShared(CBattleGuideManager.EBattleGuideFormType.Intro_2);
		}

		private void onCloseGesture(CUIEvent uiEvent)
		{
			this.CloseFormShared(CBattleGuideManager.EBattleGuideFormType.Gesture);
		}

		private void onCloseJoyStick(CUIEvent uiEvent)
		{
			if (Singleton<CUIManager>.GetInstance().GetForm(this.QueryFormPath(CBattleGuideManager.EBattleGuideFormType.JoyStick)) != null)
			{
				this.CloseFormShared(CBattleGuideManager.EBattleGuideFormType.JoyStick);
			}
		}

		private void onCloseTyrantAlert(CUIEvent uiEvent)
		{
			this.CloseFormShared(CBattleGuideManager.EBattleGuideFormType.BaojunAlert);
		}

		private void onCloseTyrantTip(CUIEvent uiEvent)
		{
			this.CloseFormShared(CBattleGuideManager.EBattleGuideFormType.BaojunTips);
		}

		private void onCloseTyrantTip2(CUIEvent uiEvent)
		{
			this.CloseFormShared(CBattleGuideManager.EBattleGuideFormType.BaojunTips2);
		}

		private void onOldPlayerFirstFormClose(CUIEvent uiEvent)
		{
			if (uiEvent != null)
			{
				Singleton<CUIManager>.instance.CloseForm(uiEvent.m_srcFormScript);
			}
		}

		private void OnSkillButtonUp(CUIEvent uiEvent)
		{
			if (this.m_lastFormType == CBattleGuideManager.EBattleGuideFormType.SkillGesture2Cancel)
			{
				if (Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager.IsIndicatorInCancelArea())
				{
					this.CloseFormShared(this.m_lastFormType);
				}
				else
				{
					string formPath = this.GuideFormPathList[(int)this.m_lastFormType];
					CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(formPath);
					if (form != null)
					{
						form.Appear(enFormHideFlag.HideByCustom, true);
						List<GameObject> list = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(SkillSlotType.SLOT_SKILL_2, false);
						if (list.get_Count() > 0)
						{
							GameObject gameObject = list.get_Item(0);
							this.showSkillButton(gameObject.transform.FindChild("Present").gameObject, false);
						}
					}
				}
			}
		}

		private void onCloseSkillGesture(CUIEvent uiEvent)
		{
			if (uiEvent == null)
			{
				return;
			}
			SkillSlotType skillSlotType = uiEvent.m_eventParams.m_skillSlotType;
			if (this.m_lastFormType == CBattleGuideManager.EBattleGuideFormType.SkillGesture2Cancel)
			{
				string formPath = this.GuideFormPathList[(int)this.m_lastFormType];
				CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(formPath);
				if (form != null)
				{
					form.Hide(enFormHideFlag.HideByCustom, true);
					List<GameObject> list = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(SkillSlotType.SLOT_SKILL_2, false);
					if (list.get_Count() > 0)
					{
						GameObject gameObject = list.get_Item(0);
						this.showSkillButton(gameObject.transform.FindChild("Present").gameObject, true);
					}
				}
			}
			else
			{
				this.CloseFormShared(this.m_lastFormType);
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer != null)
			{
				PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
				if (captain && captain.handle.EffectControl != null)
				{
					captain.handle.EffectControl.EndSkillGestureEffect();
				}
			}
			List<GameObject> list2 = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(skillSlotType, false);
			if (list2 != null && list2.get_Count() > 0)
			{
				GameObject gameObject2 = list2.get_Item(0);
				if (gameObject2 != null)
				{
					Transform transform = gameObject2.transform.FindChild("Present");
					DebugHelper.Assert(transform != null);
					if (transform != null)
					{
						transform.gameObject.CustomSetActive(true);
					}
				}
			}
		}

		private void onCloseSettle(CUIEvent uiEvent)
		{
			DynamicShadow.DisableAllDynamicShadows();
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(54u).dwConfValue;
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			DebugHelper.Assert(curLvelContext != null, "Battle Level Context is NULL!!");
			if (dwConfValue != 0u && (long)curLvelContext.m_mapID != (long)((ulong)CBattleGuideManager.AdvanceGuideLevelID))
			{
				Singleton<CUIManager>.instance.OpenForm("UGUI/Form/System/Newbie/Form_Intro_3V3.prefab", false, true);
			}
			else
			{
				this.CloseSettle();
			}
		}

		private void EnterAdvanceGuide(CUIEvent uiEvent)
		{
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(144u).dwConfValue;
			Singleton<LobbyLogic>.GetInstance().ReqStartGuideLevel((int)CBattleGuideManager.AdvanceGuideLevelID, dwConfValue);
		}

		private void RejectAdvanceGuide(CUIEvent uiEvent)
		{
			this.CloseSettle();
		}

		private void onCloseAwardTips(CUIEvent evt)
		{
			MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEntryTrainLevelEntry, new uint[0]);
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Guide_CloseTrainLevel_Award, new CUIEventManager.OnUIEventHandler(this.onCloseAwardTips));
		}

		private void UpdateGamePausing(bool bPauseGame)
		{
			if (bPauseGame)
			{
				this.PauseGame(this, false);
				Singleton<CUIParticleSystem>.GetInstance().ClearAll(true);
			}
			else
			{
				this.ResumeGame(this);
			}
		}

		public void OpenFormShared(CBattleGuideManager.EBattleGuideFormType inFormType, int delayTime = 0, bool bPauseGame = true)
		{
			if (inFormType <= CBattleGuideManager.EBattleGuideFormType.Invalid || inFormType >= CBattleGuideManager.EBattleGuideFormType.Count)
			{
				return;
			}
			Singleton<CUILoadingSystem>.instance.HideLoading();
			this.curOpenForm = Singleton<CUIManager>.GetInstance().OpenForm(this.GuideFormPathList[(int)inFormType], false, true);
			if (delayTime != 0 && this.curOpenForm != null)
			{
				Transform transform = this.curOpenForm.transform.FindChild("Panel_Interactable");
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(false);
					Singleton<CTimerManager>.GetInstance().AddTimer(delayTime, 1, new CTimer.OnTimeUpHandler(this.ShowInteractable));
				}
			}
			Singleton<CTimerManager>.GetInstance().AddTimer(CBattleGuideManager.TIME_OUT, 1, new CTimer.OnTimeUpHandler(CBattleGuideManager.TimeOutDelegate));
			if (inFormType != CBattleGuideManager.EBattleGuideFormType.BigMapGuide && inFormType != CBattleGuideManager.EBattleGuideFormType.SelectModeGuide && inFormType != CBattleGuideManager.EBattleGuideFormType.BaojunAlert && inFormType != CBattleGuideManager.EBattleGuideFormType.SkillGesture2Cancel)
			{
				this.UpdateGamePausing(bPauseGame);
			}
			if (Singleton<GameStateCtrl>.instance.isBattleState)
			{
				if (inFormType == CBattleGuideManager.EBattleGuideFormType.SkillGesture2)
				{
					List<GameObject> list = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(SkillSlotType.SLOT_SKILL_2, false);
					if (list.get_Count() > 0)
					{
						GameObject gameObject = list.get_Item(0);
						this.showSkillButton(gameObject.transform.FindChild("Present").gameObject, false);
						Singleton<CTimerManager>.instance.AddTimer(500, 1, new CTimer.OnTimeUpHandler(this.OnSkillGestEffTimer), false);
					}
				}
				else if (inFormType == CBattleGuideManager.EBattleGuideFormType.SkillGesture2Cancel)
				{
					List<GameObject> list2 = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(SkillSlotType.SLOT_SKILL_2, false);
					if (list2.get_Count() > 0)
					{
						GameObject gameObject2 = list2.get_Item(0);
						this.showSkillButton(gameObject2.transform.FindChild("Present").gameObject, false);
						Singleton<CTimerManager>.instance.AddTimer(500, 1, new CTimer.OnTimeUpHandler(this.OnSkillGestEffTimer), false);
						GameObject gameObject3 = GameObject.Find("Design");
						if (gameObject3 != null)
						{
							GlobalTrigger component = gameObject3.GetComponent<GlobalTrigger>();
							if (component != null)
							{
								component.BindSkillCancelListener();
							}
						}
					}
				}
				else if (inFormType == CBattleGuideManager.EBattleGuideFormType.SkillGesture3)
				{
					List<GameObject> list3 = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(SkillSlotType.SLOT_SKILL_3, false);
					if (list3.get_Count() > 0)
					{
						GameObject gameObject4 = list3.get_Item(0);
						this.showSkillButton(gameObject4.transform.FindChild("Present").gameObject, false);
						Singleton<CTimerManager>.instance.AddTimer(500, 1, new CTimer.OnTimeUpHandler(this.OnSkillGestEffTimer), false);
						GameObject gameObject5 = GameObject.Find("Design");
						if (gameObject5 != null)
						{
							GlobalTrigger component2 = gameObject5.GetComponent<GlobalTrigger>();
							if (component2 != null)
							{
								component2.UnbindSkillCancelListener();
							}
						}
					}
				}
			}
			this.m_lastFormType = inFormType;
		}

		private void OnSkillGestEffTimer(int inTimeSeq)
		{
			PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain;
			SkillSlotType curSkillSlotType = Singleton<CBattleSystem>.GetInstance().FightForm.GetCurSkillSlotType();
			if (this.m_lastFormType == CBattleGuideManager.EBattleGuideFormType.SkillGesture2)
			{
				if (curSkillSlotType == SkillSlotType.SLOT_SKILL_2)
				{
					return;
				}
				if (captain && captain.handle.EffectControl != null)
				{
					captain.handle.EffectControl.StartSkillGestureEffect();
				}
			}
			else if (this.m_lastFormType == CBattleGuideManager.EBattleGuideFormType.SkillGesture2Cancel)
			{
				if (curSkillSlotType == SkillSlotType.SLOT_SKILL_2)
				{
					return;
				}
				if (captain && captain.handle.EffectControl != null)
				{
					captain.handle.EffectControl.StartSkillGestureEffectCancel();
				}
			}
			else if (this.m_lastFormType == CBattleGuideManager.EBattleGuideFormType.SkillGesture3)
			{
				if (curSkillSlotType == SkillSlotType.SLOT_SKILL_3)
				{
					return;
				}
				if (captain && captain.handle.EffectControl != null)
				{
					captain.handle.EffectControl.StartSkillGestureEffect3();
				}
			}
			Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
		}

		private void showSkillButton(GameObject skillPresent, bool bShow)
		{
			if (skillPresent == null)
			{
				return;
			}
			CanvasGroup canvasGroup = skillPresent.GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = skillPresent.AddComponent<CanvasGroup>();
			}
			if (bShow)
			{
				canvasGroup.alpha = 1f;
			}
			else
			{
				canvasGroup.alpha = 0f;
			}
		}

		public void CloseFormShared(CBattleGuideManager.EBattleGuideFormType inFormType)
		{
			Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CBattleGuideManager.TimeOutDelegate));
			this.UpdateGamePausing(false);
			string formPath = this.GuideFormPathList[(int)inFormType];
			Singleton<CUIManager>.GetInstance().CloseForm(formPath);
			if (Singleton<GameStateCtrl>.instance.isBattleState)
			{
				if (inFormType == CBattleGuideManager.EBattleGuideFormType.SkillGesture2)
				{
					List<GameObject> list = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(SkillSlotType.SLOT_SKILL_2, false);
					if (list.get_Count() > 0)
					{
						GameObject gameObject = list.get_Item(0);
						this.showSkillButton(gameObject.transform.FindChild("Present").gameObject, true);
					}
				}
				else if (inFormType == CBattleGuideManager.EBattleGuideFormType.SkillGesture2Cancel)
				{
					List<GameObject> list2 = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(SkillSlotType.SLOT_SKILL_2, false);
					if (list2.get_Count() > 0)
					{
						GameObject gameObject2 = list2.get_Item(0);
						this.showSkillButton(gameObject2.transform.FindChild("Present").gameObject, true);
					}
				}
				else if (inFormType == CBattleGuideManager.EBattleGuideFormType.SkillGesture3)
				{
					List<GameObject> list3 = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(SkillSlotType.SLOT_SKILL_3, false);
					if (list3.get_Count() > 0)
					{
						GameObject gameObject3 = list3.get_Item(0);
						this.showSkillButton(gameObject3.transform.FindChild("Present").gameObject, true);
					}
				}
			}
		}

		public static void TimeOutDelegate(int timerSequence)
		{
			Singleton<CBattleGuideManager>.GetInstance().CloseFormShared(Singleton<CBattleGuideManager>.GetInstance().m_lastFormType);
		}

		private void ShowInteractable(int timerSequence)
		{
			Singleton<CTimerManager>.instance.RemoveTimer(new CTimer.OnTimeUpHandler(this.ShowInteractable));
			if (this.curOpenForm != null)
			{
				Transform transform = this.curOpenForm.transform.FindChild("Panel_Interactable");
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(true);
					CUIAnimatorScript component = transform.GetComponent<CUIAnimatorScript>();
					if (component != null)
					{
						component.PlayAnimator("Interactable_Enabled");
					}
				}
			}
		}

		private void ShowNewbiePassedHero(uint heroId, bool bImage1)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Newbie/Form_NewbieSettle.prefab", false, true);
			if (cUIFormScript != null)
			{
				DynamicShadow.EnableDynamicShow(cUIFormScript.gameObject, true);
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null && cUIFormScript != null)
			{
				int heroWearSkinId = (int)masterRoleInfo.GetHeroWearSkinId(heroId);
				string objectName = CUICommonSystem.GetHeroPrefabPath(heroId, heroWearSkinId, true).ObjectName;
				CUI3DImageScript component = cUIFormScript.transform.Find("3DImage").gameObject.GetComponent<CUI3DImageScript>();
				GameObject model = component.AddGameObject(objectName, false, false);
				CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
				instance.Set3DModel(model);
				instance.InitAnimatList();
				instance.InitAnimatSoundList(heroId, (uint)heroWearSkinId);
				instance.OnModePlayAnima("Cheer");
				Transform transform = cUIFormScript.transform.FindChild("Panel_NewHero/MaskBlack/Text");
				if (transform != null)
				{
					Text component2 = transform.GetComponent<Text>();
					if (component2 != null)
					{
						ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
						if (dataByKey != null)
						{
							component2.set_text(StringHelper.UTF8BytesToString(ref dataByKey.szName));
						}
					}
				}
				if (bImage1)
				{
					Transform transform2 = cUIFormScript.transform.FindChild("Panel_NewHero/Image1");
					if (transform2 != null)
					{
						transform2.gameObject.CustomSetActive(true);
					}
				}
				else
				{
					Transform transform3 = cUIFormScript.transform.FindChild("Panel_NewHero/Image2");
					if (transform3 != null)
					{
						transform3.gameObject.CustomSetActive(true);
					}
				}
			}
		}

		public void StartSettle(COMDT_SETTLE_RESULT_DETAIL detail)
		{
			this.TrainLevelReward = detail.stReward;
			this.m_bWin = (detail.stGameInfo.bGameResult == 1);
			Singleton<CBattleSystem>.GetInstance().FightForm.ShowWinLosePanel(this.m_bWin);
		}

		public void OpenSettle()
		{
			if (this.bReEntry)
			{
				this.UpdateGamePausing(false);
				Singleton<CMatchingSystem>.GetInstance().OpenPvPEntry(enPvPEntryFormWidget.GuideBtnGroup);
			}
			this.ShowPersonalProfit(true);
		}

		private void OpenNewbieSettle()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			if (CBattleGuideManager.Is1v1GuideLevel(curLvelContext.m_mapID))
			{
				if (!this.bReEntry)
				{
					uint firstHeroId = masterRoleInfo.GetFirstHeroId();
					this.ShowNewbiePassedHero(firstHeroId, true);
					Singleton<CTimerManager>.GetInstance().AddTimer(1000, 1, delegate(int seq)
					{
						Singleton<CNewbieAchieveSys>.GetInstance().ShowAchieve(enNewbieAchieve.COM_ACNT_CLIENT_BITS_TYPE_GET_HERO);
					});
				}
			}
			else if (CBattleGuideManager.Is5v5GuideLevel(curLvelContext.m_mapID))
			{
				uint guideLevel2FadeHeroId = masterRoleInfo.GetGuideLevel2FadeHeroId();
				this.ShowNewbiePassedHero(guideLevel2FadeHeroId, false);
			}
			else if (CBattleGuideManager.IsCastingGuideLevel(curLvelContext.m_mapID))
			{
				CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CBattleGuideManager.NEWBIE_GOLDAWARD_FORM_PATH, false, true);
				if (cUIFormScript != null)
				{
					Transform transform = cUIFormScript.transform.FindChild("itemCell");
					if (transform != null)
					{
						CUseable itemUseable = CUseableManager.CreateCoinUseable(RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN, (int)GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_CORONA_COIN));
						CUICommonSystem.SetItemCell(cUIFormScript, transform.gameObject, itemUseable, true, false, false, false);
					}
				}
			}
		}

		private void OpenTrainLevelSettle()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			DebugHelper.Assert(curLvelContext != null, "Battle Level Context is NULL!!");
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				Singleton<CMatchingSystem>.GetInstance().OpenPvPEntry(enPvPEntryFormWidget.GuideBtnGroup);
				ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(this.TrainLevelReward);
				ListView<CUseable> listView = new ListView<CUseable>();
				int count = useableListFromReward.Count;
				for (int i = 0; i < count; i++)
				{
					if (useableListFromReward[i].MapRewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_HONOUR && useableListFromReward[i].MapRewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOLCOIN)
					{
						listView.Add(useableListFromReward[i]);
					}
				}
				if (listView.Count != 0)
				{
					Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(listView), Singleton<CTextManager>.GetInstance().GetText("TrainLevel_Settel_Tile0"), true, enUIEventID.Battle_Guide_CloseTrainLevel_Award, false, false, "Form_Award");
					Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Guide_CloseTrainLevel_Award, new CUIEventManager.OnUIEventHandler(this.onCloseAwardTips));
				}
			}
		}

		private void CloseSettle()
		{
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Newbie/Form_NewbieSettle.prefab");
			Singleton<EventRouter>.instance.BroadCastEvent("CheckNewbieIntro");
		}

		public void ShowPersonalProfit(bool win)
		{
			if (Singleton<CUIManager>.GetInstance().GetForm(CBattleGuideManager.PROFIT_FORM_NAME) != null)
			{
				return;
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CBattleGuideManager.PROFIT_FORM_NAME, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			GameObject gameObject = cUIFormScript.m_formWidgets[0];
			gameObject.transform.FindChild("Win").gameObject.CustomSetActive(win);
			gameObject.transform.FindChild("Lose").gameObject.CustomSetActive(!win);
			this.SetExpProfit(cUIFormScript);
			this.SetGoldCoinProfit(cUIFormScript);
			this.SetSymbolCoinProfit(cUIFormScript);
			this.SetMapInfo(cUIFormScript);
			this.DoCoinAndExpTween();
		}

		private void SetExpProfit(CUIFormScript profitForm)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint)((byte)masterRoleInfo.PvpLevel));
			if (dataByKey == null)
			{
				return;
			}
			COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
			if (acntInfo == null)
			{
				return;
			}
			GameObject gameObject = profitForm.m_formWidgets[1];
			gameObject.transform.FindChild("PlayerName").gameObject.GetComponent<Text>().set_text(masterRoleInfo.Name);
			gameObject.transform.FindChild("PlayerLv").gameObject.GetComponent<Text>().set_text(string.Format("Lv.{0}", masterRoleInfo.PvpLevel));
			Text component = gameObject.transform.FindChild("PvpExpTxt").gameObject.GetComponent<Text>();
			component.set_text(string.Format("{0}/{1}", acntInfo.dwPvpExp, dataByKey.dwNeedExp));
			Text component2 = gameObject.transform.FindChild("AddPvpExpTxt").gameObject.GetComponent<Text>();
			component2.set_text(string.Format("+{0}", acntInfo.dwPvpSettleExp));
			gameObject.transform.FindChild("Bar").gameObject.CustomSetActive(acntInfo.dwPvpSettleExp != 0u);
			if (acntInfo.dwPvpSettleExp <= 0u)
			{
				CUICommonSystem.SetObjActive(gameObject.transform.FindChild("text"), false);
			}
			RectTransform component3 = gameObject.transform.FindChild("PvpExpSliderBg/BasePvpExpSlider").gameObject.GetComponent<RectTransform>();
			RectTransform component4 = gameObject.transform.FindChild("PvpExpSliderBg/AddPvpExpSlider").gameObject.GetComponent<RectTransform>();
			if (acntInfo.dwPvpSettleExp > 0u)
			{
				Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_jingyan", null);
			}
			int num = (int)(acntInfo.dwPvpExp - acntInfo.dwPvpSettleExp);
			CBattleGuideManager.lvUpGrade = ((num < 0) ? acntInfo.dwPvpLv : 0u);
			float num2 = Mathf.Max(0f, (float)num / dataByKey.dwNeedExp);
			float num3 = Mathf.Max(0f, ((num < 0) ? acntInfo.dwPvpExp : acntInfo.dwPvpSettleExp) / dataByKey.dwNeedExp);
			component3.sizeDelta = new Vector2(num2 * 220.3f, component3.sizeDelta.y);
			component4.sizeDelta = new Vector2(num2 * 220.3f, component4.sizeDelta.y);
			this.expFrom = num2;
			this.expTo = num2 + num3;
			this.expTweenRect = component4;
			component3.gameObject.CustomSetActive(num >= 0);
			CUIHttpImageScript component5 = gameObject.transform.FindChild("HeadImage").GetComponent<CUIHttpImageScript>();
			Image component6 = gameObject.transform.FindChild("NobeIcon").GetComponent<Image>();
			Image component7 = gameObject.transform.FindChild("HeadFrame").GetComponent<Image>();
			if (!CSysDynamicBlock.bSocialBlocked)
			{
				string headUrl = masterRoleInfo.HeadUrl;
				component5.SetImageUrl(headUrl);
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component6, (int)masterRoleInfo.GetNobeInfo().stGameVipClient.dwCurLevel, false, true, 0uL);
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component7, (int)masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId);
			}
			else
			{
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component6, 0, false, true, 0uL);
			}
		}

		private void SetGoldCoinProfit(CUIFormScript profitForm)
		{
			if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() == null)
			{
				return;
			}
			COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
			if (acntInfo == null)
			{
				return;
			}
			GameObject gameObject = profitForm.m_formWidgets[2];
			Text component = gameObject.transform.FindChild("GoldNum").GetComponent<Text>();
			component.set_text("+0");
			this.coinFrom = 0f;
			uint num = 0u;
			for (int i = 0; i < (int)this.TrainLevelReward.bNum; i++)
			{
				if (this.TrainLevelReward.astRewardDetail[i].bType == 11)
				{
					num += this.TrainLevelReward.astRewardDetail[i].stRewardInfo.dwPvpCoin;
				}
			}
			num += acntInfo.dwPvpSettleCoin;
			if (num <= 0u)
			{
				CUICommonSystem.SetObjActive(gameObject.transform.FindChild("text"), false);
			}
			this.coinTo = num;
			this.coinTweenText = component;
		}

		private void SetSymbolCoinProfit(CUIFormScript profitForm)
		{
			GameObject gameObject = profitForm.m_formWidgets[3];
			Text component = gameObject.transform.FindChild("CoinNum").GetComponent<Text>();
			component.set_text("+0");
			this.symbolCoinFrom = 0f;
			uint num = 0u;
			for (int i = 0; i < (int)this.TrainLevelReward.bNum; i++)
			{
				if (this.TrainLevelReward.astRewardDetail[i].bType == 14)
				{
					num += this.TrainLevelReward.astRewardDetail[i].stRewardInfo.dwSymbolCoin;
				}
			}
			this.symbolCoinTo = num;
			this.symbolCoinTweenText = component;
		}

		private void SetMapInfo(CUIFormScript profitForm)
		{
			GameObject gameObject = profitForm.m_formWidgets[6];
			gameObject.CustomSetActive(true);
			Text component = gameObject.transform.FindChild("GameType").GetComponent<Text>();
			Text component2 = gameObject.transform.FindChild("MapName").GetComponent<Text>();
			string text = Singleton<CTextManager>.instance.GetText("Battle_Settle_Game_Type_Train");
			string text2 = string.Empty;
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext == null)
			{
				return;
			}
			uint mapID = (uint)curLvelContext.m_mapID;
			ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey(mapID);
			if (dataByKey != null)
			{
				text2 = dataByKey.szName;
			}
			component.set_text(text);
			component2.set_text(text2);
		}

		private void DoCoinAndExpTween()
		{
			try
			{
				if (this.coinTweenText != null && this.coinTweenText.gameObject != null)
				{
					this.coinLtd = LeanTween.value(this.coinTweenText.gameObject, delegate(float value)
					{
						if (this.coinTweenText == null || this.coinTweenText.gameObject == null)
						{
							return;
						}
						this.coinTweenText.set_text(string.Format("+{0}", value.ToString("N0")));
						if (value >= this.coinTo)
						{
							this.DoCoinTweenEnd();
						}
					}, this.coinFrom, this.coinTo, 2f);
				}
				if (this.symbolCoinTweenText != null && this.symbolCoinTweenText.gameObject != null)
				{
					this.symbolCoinfLtd = LeanTween.value(this.coinTweenText.gameObject, delegate(float value)
					{
						if (this.symbolCoinTweenText == null || this.symbolCoinTweenText.gameObject == null)
						{
							return;
						}
						this.symbolCoinTweenText.set_text(string.Format("+{0}", value.ToString("N0")));
						if (value >= this.symbolCoinTo)
						{
							this.DoSymbolCoinTweenEnd();
						}
					}, this.symbolCoinFrom, this.symbolCoinTo, 2f);
				}
				if (this.expTweenRect != null && this.expTweenRect.gameObject != null)
				{
					this.expLtd = LeanTween.value(this.expTweenRect.gameObject, delegate(float value)
					{
						if (this.expTweenRect == null || this.expTweenRect.gameObject == null)
						{
							return;
						}
						this.expTweenRect.sizeDelta = new Vector2(value * 220.3f, this.expTweenRect.sizeDelta.y);
						if (value >= this.expTo)
						{
							this.DoExpTweenEnd();
						}
					}, this.expFrom, this.expTo, 2f);
				}
			}
			catch (Exception ex)
			{
				DebugHelper.Assert(false, "Exceptin in DoCoinAndExpTween, {0}", new object[]
				{
					ex.get_Message()
				});
			}
		}

		public void DoCoinTweenEnd()
		{
			if (this.coinLtd == null || this.coinTweenText == null)
			{
				return;
			}
			this.coinTweenText.set_text(string.Format("+{0}", this.coinTo.ToString("N0")));
			COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
			if (Singleton<BattleStatistic>.GetInstance().multiDetail != null)
			{
				CUICommonSystem.AppendMultipleText(this.coinTweenText, CUseable.GetMultiple(acntInfo.dwPvpSettleBaseCoin, ref Singleton<BattleStatistic>.GetInstance().multiDetail, 0, -1));
			}
			this.coinLtd.cancel();
			this.coinLtd = null;
			this.coinTweenText = null;
		}

		public void DoSymbolCoinTweenEnd()
		{
			if (this.symbolCoinfLtd == null || this.symbolCoinTweenText == null)
			{
				return;
			}
			this.symbolCoinTweenText.set_text(string.Format("+{0}", this.symbolCoinTo.ToString("N0")));
			this.symbolCoinfLtd.cancel();
			this.symbolCoinfLtd = null;
			this.symbolCoinTweenText = null;
		}

		private void DoExpTweenEnd()
		{
			if (this.expTweenRect != null && this.expLtd != null)
			{
				this.expTweenRect.sizeDelta = new Vector2(this.expTo * 220.3f, this.expTweenRect.sizeDelta.y);
				this.expLtd.cancel();
				this.expLtd = null;
				this.expTweenRect = null;
			}
			if (CBattleGuideManager.lvUpGrade > 1u)
			{
				CUIEvent cUIEvent = new CUIEvent();
				cUIEvent.m_eventID = enUIEventID.Settle_OpenLvlUp;
				cUIEvent.m_eventParams.tag = (int)(CBattleGuideManager.lvUpGrade - 1u);
				cUIEvent.m_eventParams.tag2 = (int)CBattleGuideManager.lvUpGrade;
				CUIEvent uiEvent = cUIEvent;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
			}
			CBattleGuideManager.lvUpGrade = 0u;
		}

		private void onClickProfitContinue(CUIEvent uiEvent)
		{
			if (!this.bReEntry)
			{
				this.OpenNewbieSettle();
			}
			else
			{
				this.OpenTrainLevelSettle();
			}
		}

		public void SetPlayerGuideInfo(ref GuideInfo inGuideInfo)
		{
			this.m_guideInfo.m_PlayerMobaLevel = inGuideInfo.m_PlayerMobaLevel;
		}

		public GuideInfo GetPlayerGuideInfo()
		{
			return this.m_guideInfo;
		}

		public void OnEndGame()
		{
			this.m_guideInfo = default(GuideInfo);
		}

		public void ShowBuyEuipPanel(bool bShow)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
			GameObject widget = form.GetWidget(45);
			widget.CustomSetActive(bShow);
		}

		public void OpenOldPlayerFirstForm()
		{
			if (CBattleGuideManager.ms_bOldPlayerFormOpened || !MonoSingleton<NewbieGuideManager>.GetInstance().newbieGuideEnable)
			{
				return;
			}
			if (Singleton<CUIManager>.instance.GetForm("UGUI/Form/System/Newbie/Form_OldTip.prefab") == null)
			{
				Singleton<CUIManager>.instance.OpenForm("UGUI/Form/System/Newbie/Form_OldTip.prefab", false, true);
				CBattleGuideManager.ms_bOldPlayerFormOpened = true;
			}
		}

		public bool OpenBannerDlgByBannerGuideId(uint Id, CUIEvent uiEventParam = null, bool bShowChooseGuideNextTime = false)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			NewbieGuideBannerGuideConf dataByKey = GameDataMgr.newbieBannerGuideDatabin.GetDataByKey(Id);
			if (masterRoleInfo == null || dataByKey == null)
			{
				return false;
			}
			int num = NewbieGuideManager.ConvertNewbieBitToClientBit(dataByKey.dwGuideBit);
			if (num != 0 && (masterRoleInfo.IsClientBitsSet(num) || CSysDynamicBlock.bNewbieBlocked))
			{
				return false;
			}
			int num2 = 0;
			for (int i = 0; i < dataByKey.astPicPath.Length; i++)
			{
				if (dataByKey.astPicPath[i].dwID == 0u)
				{
					break;
				}
				num2++;
			}
			string[] array = new string[num2];
			for (int j = 0; j < num2; j++)
			{
				array[j] = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Newbie_Dir, dataByKey.astPicPath[j].dwID.ToString());
			}
			this.OpenBannerIntroDialog(num, array, num2, uiEventParam, dataByKey.szTitleName, dataByKey.szBtnName, true, bShowChooseGuideNextTime);
			return true;
		}

		public void OpenBannerIntroDialog(int clientBit, string[] imgPath, int imgNum, CUIEvent uiEventParam = null, string title = null, string btnName = null, bool bAutoMove = true, bool bShowChooseGuideNextTime = false)
		{
			this.m_BannerIntroDialog.OpenForm(clientBit, imgPath, imgNum, uiEventParam, title, btnName, bAutoMove, bShowChooseGuideNextTime);
		}

		private void OnClickToPrePage(CUIEvent uiEvent)
		{
			this.m_BannerIntroDialog.MoveToPrePage();
		}

		private void OnClickToNextPage(CUIEvent uiEvent)
		{
			this.m_BannerIntroDialog.MoveToNextPage();
		}

		private void OnDragStart(CUIEvent uiEvent)
		{
			this.m_BannerIntroDialog.DragStart(uiEvent.m_pointerEventData.get_position());
		}

		private void OnDragEnd(CUIEvent uiEvent)
		{
			this.m_BannerIntroDialog.DragEnd(uiEvent.m_pointerEventData.get_position());
		}

		private void OnDialogClickConfirm(CUIEvent uiEvent)
		{
			int tag = uiEvent.m_eventParams.tag;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			if (tag != 0 && this.m_BannerIntroDialog.CanSetClientBit())
			{
				masterRoleInfo.SetClientBits(tag, true, true);
			}
			this.m_BannerIntroDialog.Confirm();
		}

		private void OnDialogClose(CUIEvent uiEvent)
		{
			this.m_BannerIntroDialog.Clear();
		}

		private void onMoveTimeUp(CUIEvent uiEvent)
		{
			this.m_BannerIntroDialog.AutoMove();
		}

		private void onClickVictoryTips(CUIEvent uiEvt)
		{
			uint tagUInt = uiEvt.m_eventParams.tagUInt;
			CUICommonSystem.OpenUrl(uiEvt.m_eventParams.tagStr, true, 0);
			uiEvt.m_srcWidget.transform.parent.FindChild("Panel_Guide").gameObject.CustomSetActive(false);
		}

		public static bool EnableHeroVictoryTips()
		{
			bool result = false;
			ResGlobalInfo resGlobalInfo = null;
			GameDataMgr.svr2CltCfgDict.TryGetValue(9u, out resGlobalInfo);
			if (resGlobalInfo != null)
			{
				result = Convert.ToBoolean(resGlobalInfo.dwConfValue);
			}
			return result;
		}

		public static bool GetGuide1v1HeroIDAndLevelID(uint heroType, out uint heroId, out int levelId)
		{
			if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1))
			{
				heroId = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_HERO);
				levelId = (int)GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_LEVEL);
				return true;
			}
			if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE2))
			{
				heroId = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_HERO_AD);
				levelId = (int)GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_LEVEL_AD);
				return true;
			}
			if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE3))
			{
				heroId = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_HERO_AP);
				levelId = (int)GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_LEVEL_AP);
				return true;
			}
			heroId = 0u;
			levelId = 0;
			return false;
		}

		public static bool GetGuide5v5HeroIDAndLevelID(uint heroType, out uint heroId, out int levelId)
		{
			if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1))
			{
				heroId = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_HERO);
				levelId = (int)GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_LEVEL);
				return true;
			}
			if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE2))
			{
				heroId = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_HERO_AD);
				levelId = (int)GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_LEVEL_AD);
				return true;
			}
			if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE3))
			{
				heroId = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_HERO_AP);
				levelId = (int)GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_LEVEL_AP);
				return true;
			}
			heroId = 0u;
			levelId = 0;
			return false;
		}

		public static bool Is1v1GuideLevel(int levelId)
		{
			return (long)levelId == (long)((ulong)CBattleGuideManager.GuideLevel1v1Tank) || (long)levelId == (long)((ulong)CBattleGuideManager.GuideLevel1v1AD) || (long)levelId == (long)((ulong)CBattleGuideManager.GuideLevel1v1AP);
		}

		public static bool Is5v5GuideLevel(int levelId)
		{
			return (long)levelId == (long)((ulong)CBattleGuideManager.GuideLevel5v5Tank) || (long)levelId == (long)((ulong)CBattleGuideManager.GuideLevel5v5AD) || (long)levelId == (long)((ulong)CBattleGuideManager.GuideLevel5v5AP);
		}

		public static bool IsCastingGuideLevel(int levelId)
		{
			return (long)levelId == (long)((ulong)CBattleGuideManager.GuideLevelIDCasting);
		}
	}
}
