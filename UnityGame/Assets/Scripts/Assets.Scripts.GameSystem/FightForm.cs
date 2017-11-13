using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class FightForm : IBattleForm
	{
		public static string s_battleUIForm = "UGUI/Form/Battle/Form_Battle.prefab";

		public static string s_battleJoystick = "UGUI/Form/Battle/Part/Form_Battle_Part_Joystick.prefab";

		public static string s_battleSkillCursor = "UGUI/Form/Battle/Part/Form_Battle_Part_SkillCursor.prefab";

		public static string s_battleScene = "UGUI/Form/Battle/Part/Form_Battle_Part_Scene.prefab";

		public static string s_battleCameraMove = "UGUI/Form/Battle/Part/Form_Battle_Part_CameraMove.prefab";

		public static string s_skillBtnFormPath = "UGUI/Form/Battle/Part/Form_Battle_Part_SkillBtn.prefab";

		public static string s_buffSkillFormPath = "UGUI/Form/Battle/Part/Form_Battle_Part_BuffSkill.prefab";

		public static string s_enemyHeroAtkFormPath = "UGUI/Form/Battle/Part/Form_Battle_Part_EnemyHeroAtk.prefab";

		public static string s_cameraDragPanelPath = "UGUI/Form/Battle/Part/Form_Battle_Part_CameraDragPanel";

		public static string s_battleDragonTipForm = "UGUI/Form/Battle/Form_Battle_Dragon_Tips.prefab";

		public CSkillButtonManager m_skillButtonManager;

		public ScoreBoard scoreBoard;

		private ScoreboardPvE scoreboardPvE;

		private CStarEvalPanel starEvalPanel;

		private BattleTaskView battleTaskView;

		private SoldierWave soldierWaveView;

		private CTreasureHud treasureHud;

		private HeroHeadHud heroHeadHud;

		private BattleDragonView m_dragonView;

		private SignalPanel m_signalPanel;

		private BattleMisc m_battleMisc;

		private HeroInfoPanel m_heroInfoPanel;

		private CBattleShowBuffDesc m_showBuffDesc;

		public CEnemyHeroAtkBtn m_enemyHeroAtkBtn;

		private BackToCityProcessBar m_goBackProcessBar;

		private CHostHeroDeadInfo m_hostHeroDeadInfo;

		private GameObject DominateVanguardWaves;

		private Text DominateVanguardWavesText;

		private COM_PLAYERCAMP DominateVanguardCamp;

		private int DominateVanguardRepeatCount;

		public bool m_isInBattle;

		public CUIFormScript _formScript;

		private CUIFormScript _formJoystickScript;

		private CUIFormScript _formSkillCursorScript;

		private CUIFormScript _formSceneScript;

		private CUIFormScript _formCameraMoveScript;

		private CUIFormScript _formSkillBtnScript;

		private CUIFormScript _formBuffSkillScript;

		private CUIFormScript _formEnemyHeroAtkScript;

		private CUIFormScript _formCameraDragPanel;

		private float timeSkillCooldown;

		private float timeEnergyShortage;

		private float timeNoSkillTarget;

		private float timeSkillBeanShortage;

		private uint _lastFps;

		private CUIJoystickScript _joystick;

		public ListView<SLOTINFO> m_SkillSlotList = new ListView<SLOTINFO>();

		public Vector2 world_UI_Factor_Small;

		public Vector2 UI_world_Factor_Small;

		public Vector2 world_UI_Factor_Big;

		public Vector2 UI_world_Factor_Big;

		private bool m_isSkillDecShow;

		private bool m_bOpenSpeak;

		private bool m_bOpenMic;

		private int m_VocetimerFirst;

		private int m_Vocetimer;

		private int m_VoiceMictime;

		private int m_VoiceTipsShowTime = 2000;

		private Transform m_OpenSpeakerObj;

		private Transform m_OpenSpeakerTipObj;

		private Text m_OpenSpeakerTipText;

		private Transform m_OpeankSpeakAnim;

		private Transform m_OpeankBigMap;

		private Transform m_OpenMicObj;

		private Transform m_OpenMicTipObj;

		private Text m_OpenMicTipText;

		private int m_displayPing;

		private bool m_bShowRealPing;

		public int m_wifiIconCheckTicks = 3;

		public int m_wifiIconCheckMaxTicks = 3;

		public int m_wifiTimerInterval = 2;

		public Image m_wifiIcon;

		public Text m_wifiText;

		private bool a = true;

		private GameObject BuffDesc;

		private GameObject skillTipDesc;

		private string microphone_path = "UGUI/Sprite/Battle/Battle_btn_Microphone.prefab";

		private string no_microphone_path = "UGUI/Sprite/Battle/Battle_btn_No_Microphone.prefab";

		private string voiceIcon_path = "UGUI/Sprite/Battle/Battle_btn_voice.prefab";

		private string no_voiceIcon_path = "UGUI/Sprite/Battle/Battle_btn_No_voice.prefab";

		private GameObject m_panelHeroInfo;

		private CanvasGroup m_panelHeroCanvas;

		private GameObject m_objHeroHead;

		private Image m_HpImg;

		private Text m_HpTxt;

		private Image m_EpImg;

		private Text m_AdTxt;

		private Text m_ApTxt;

		private Text m_PhyDefTxt;

		private Text m_MgcDefTxt;

		private PoolObjHandle<ActorRoot> m_selectedHero;

		private PoolObjHandle<ActorRoot> m_HideSelectedHero;

		public bool IsMicUIOpen
		{
			get
			{
				return this.m_bOpenMic;
			}
		}

		public CUIFormScript FormScript
		{
			get
			{
				return this._formScript;
			}
		}

		public CUIContainerScript TextHudContainer
		{
			get
			{
				if (this._formScript)
				{
					GameObject widget = this._formScript.GetWidget(24);
					if (widget)
					{
						return widget.GetComponent<CUIContainerScript>();
					}
				}
				return null;
			}
		}

		public bool OpenSpeakInBattle
		{
			get
			{
				return this.m_bOpenSpeak;
			}
		}

		public FightForm()
		{
			this._formScript = null;
			this._formJoystickScript = null;
			this._formSkillCursorScript = null;
			this._formSceneScript = null;
			this._formCameraMoveScript = null;
			this._formSkillBtnScript = null;
			this._formBuffSkillScript = null;
			this._formEnemyHeroAtkScript = null;
		}

		private void RegisterEvents()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_MultiHashInvalid, new CUIEventManager.OnUIEventHandler(this.onMultiHashNotSync));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ActivateForm, new CUIEventManager.OnUIEventHandler(this.Battle_ActivateForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnCloseForm, new CUIEventManager.OnUIEventHandler(this.OnFormClosed));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnHideForm, new CUIEventManager.OnUIEventHandler(this.OnFormHide));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnAppearForm, new CUIEventManager.OnUIEventHandler(this.OnFormAppear));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OpenSysMenu, new CUIEventManager.OnUIEventHandler(this.ShowSysMenu));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_SysReturnLobby, new CUIEventManager.OnUIEventHandler(this.onReturnLobby));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ConfirmSysReturnLobby, new CUIEventManager.OnUIEventHandler(this.onConfirmReturnLobby));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_SysQuitGame, new CUIEventManager.OnUIEventHandler(this.onQuitGame));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_SysReturnGame, new CUIEventManager.OnUIEventHandler(this.onReturnGame));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnOpMode, new CUIEventManager.OnUIEventHandler(this.onChangeOperateMode));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_SwitchAutoAI, new CUIEventManager.OnUIEventHandler(this.OnToggleAutoAI));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ChgFreeCamera, new CUIEventManager.OnUIEventHandler(this.OnToggleFreeCamera));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_HeroInfoSwitch, new CUIEventManager.OnUIEventHandler(this.OnSwitchHeroInfoPanel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_HeroInfoPanelOpen, new CUIEventManager.OnUIEventHandler(this.OnOpenHeorInfoPanel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_HeroInfoPanelClose, new CUIEventManager.OnUIEventHandler(this.OnCloseHeorInfoPanel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_FPSAndLagUpdate, new CUIEventManager.OnUIEventHandler(this.UpdateFpsAndLag));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnOpenDragonTip, new CUIEventManager.OnUIEventHandler(this.OnDragonTipFormOpen));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnCloseDragonTip, new CUIEventManager.OnUIEventHandler(this.OnDragonTipFormClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonDown, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonDown));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonUp, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Disable_Alert, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableAlert));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonDragged, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonDragged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Disable_Down, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableBtnDown));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Disable_Up, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableBtnUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonHold, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillBtnHold));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonHoldEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonHoldEnd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnAtkSelectHeroDown, new CUIEventManager.OnUIEventHandler(this.OnBattleAtkSelectHeroBtnDown));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnAtkSelectSoldierDown, new CUIEventManager.OnUIEventHandler(this.OnBattleAtkSelectSoldierBtnDown));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnLastHitButtonDown, new CUIEventManager.OnUIEventHandler(this.OnBattleLastHitBtnDown));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnLastHitButtonUp, new CUIEventManager.OnUIEventHandler(this.OnBattleLastHitBtnUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnAttackOrganButtonDown, new CUIEventManager.OnUIEventHandler(this.OnBattleAttackOrganBtnDown));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnAttackOrganButtonUp, new CUIEventManager.OnUIEventHandler(this.OnBattleAttackOrganBtnUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_LearnSkillBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleLearnSkillButtonClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_SkillLevelUpEffectPlayEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillLevelUpEffectPlayEnd));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.onHeroEnergyChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyMax", new Action<PoolObjHandle<ActorRoot>, int, int>(this.onHeroEnergyMax));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_BattleWifiCheckTimer, new CUIEventManager.OnUIEventHandler(this.OnCommon_BattleWifiCheckTimer));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_BattleShowOrHideWifiInfo, new CUIEventManager.OnUIEventHandler(this.OnCommon_BattleShowOrHideWifiInfo));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnUITypeChangeComplete, new CUIEventManager.OnUIEventHandler(this.OnUITypeChangeComplete));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_CameraDraging, new CUIEventManager.OnUIEventHandler(this.OnDropCamera));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_RevivieTimer, new CUIEventManager.OnUIEventHandler(this.OnReviveTimerChange));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ChangeSkillEventParam>(GameSkillEventDef.Event_ChangeSkill, new GameSkillEvent<ChangeSkillEventParam>(this.OnPlayerSkillChanged));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_RecoverSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillRecovered));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDChanged));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDStart, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDStart));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDEnd, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDEnd));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_ChangeSkillBean, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillBeanChanged));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_EnableSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillEnable));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_DisableSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillDisable));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_ProtectDisappear, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerProtectDisappear));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_SkillCooldown, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerSkillCooldown));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.Enent_EnergyShortage, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerEnergyShortage));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_SkillBeanShortage, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerSkillBeanShortage));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_NoSkillTarget, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerNoSkillTarget));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_Blindess, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerBlindess));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_LimitSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerLimitSkill));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_CancelLimitSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerCancelLimitSkill));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UpdateSkillUI, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerUpdateSkill));
			Singleton<GameEventSys>.GetInstance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorImmune, new RefAction<DefaultGameEventParam>(this.OnActorImmune));
			Singleton<GameEventSys>.GetInstance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorHurtAbsorb, new RefAction<DefaultGameEventParam>(this.OnActorHurtAbsorb));
			Singleton<GameEventSys>.GetInstance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_AutoAISwitch, new RefAction<DefaultGameEventParam>(this.OnSwitchAutoAI));
			Singleton<GameEventSys>.GetInstance().AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_UseSkill, new GameSkillEvent<ActorSkillEventParam>(this.onUseSkill));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Pause_Game, new CUIEventManager.OnUIEventHandler(this.OnBattlePauseGame));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Resume_Game, new CUIEventManager.OnUIEventHandler(this.OnBattleResumeGame));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_RecommendEquip_Buy, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipQuicklyBuy));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_EquipBoughtEffectPlayEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBoughtEffectPlayEnd));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this.OnActorGoldCoinInBattleChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Trusteeship_Accept, new CUIEventManager.OnUIEventHandler(this.OnAcceptTrusteeship));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Trusteeship_Cancel, new CUIEventManager.OnUIEventHandler(this.OnCancelTrusteeship));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroSoulLvlChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this.OnHeroSkillLvlup));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnCaptainSwitched));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VOICE_OpenSpeaker, new CUIEventManager.OnUIEventHandler(this.OnBattleOpenSpeaker));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VOICE_OpenMic, new CUIEventManager.OnUIEventHandler(this.OnBattleOpenMic));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CommonAttactType>(EventID.GAME_SETTING_COMMONATTACK_TYPE_CHANGE, new Action<CommonAttactType>(this.OnGameSettingCommonAttackTypeChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler<LastHitMode>(EventID.GAME_SETTING_LASTHIT_MODE_CHANGE, new Action<LastHitMode>(this.OnGameSettingLastHitModeChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler<AttackOrganMode>(EventID.GAME_SETTING_ATTACKORGAN_MODE_CHANGE, new Action<AttackOrganMode>(this.OnGameSettingAttackOrganModeChange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Click_Scene, new CUIEventManager.OnUIEventHandler(this.OnClickBattleScene));
			Singleton<GameEventSys>.instance.AddEventHandler<SoldierWaveParam>(GameEventDef.Event_SoldierWaveNext, new RefAction<SoldierWaveParam>(this.OnWaveNext));
			Singleton<GameEventSys>.instance.AddEventHandler<SoldierWaveParam>(GameEventDef.Event_SoldierWaveNextRepeat, new RefAction<SoldierWaveParam>(this.OnWaveNextRepeat));
			this.SetCommonAttackTargetEvent();
		}

		private void UnregisterEvents()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_MultiHashInvalid, new CUIEventManager.OnUIEventHandler(this.onMultiHashNotSync));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ActivateForm, new CUIEventManager.OnUIEventHandler(this.Battle_ActivateForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnCloseForm, new CUIEventManager.OnUIEventHandler(this.OnFormClosed));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnHideForm, new CUIEventManager.OnUIEventHandler(this.OnFormHide));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnAppearForm, new CUIEventManager.OnUIEventHandler(this.OnFormAppear));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OpenSysMenu, new CUIEventManager.OnUIEventHandler(this.ShowSysMenu));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_SysReturnLobby, new CUIEventManager.OnUIEventHandler(this.onReturnLobby));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ConfirmSysReturnLobby, new CUIEventManager.OnUIEventHandler(this.onConfirmReturnLobby));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_SysQuitGame, new CUIEventManager.OnUIEventHandler(this.onQuitGame));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_SysReturnGame, new CUIEventManager.OnUIEventHandler(this.onReturnGame));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnOpMode, new CUIEventManager.OnUIEventHandler(this.onChangeOperateMode));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_SwitchAutoAI, new CUIEventManager.OnUIEventHandler(this.OnToggleAutoAI));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ChgFreeCamera, new CUIEventManager.OnUIEventHandler(this.OnToggleFreeCamera));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_HeroInfoSwitch, new CUIEventManager.OnUIEventHandler(this.OnSwitchHeroInfoPanel));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_HeroInfoPanelOpen, new CUIEventManager.OnUIEventHandler(this.OnOpenHeorInfoPanel));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_HeroInfoPanelClose, new CUIEventManager.OnUIEventHandler(this.OnCloseHeorInfoPanel));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_FPSAndLagUpdate, new CUIEventManager.OnUIEventHandler(this.UpdateFpsAndLag));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnOpenDragonTip, new CUIEventManager.OnUIEventHandler(this.OnDragonTipFormOpen));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnCloseDragonTip, new CUIEventManager.OnUIEventHandler(this.OnDragonTipFormClose));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonDown, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonDown));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonUp, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonUp));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Disable_Alert, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableAlert));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonDragged, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonDragged));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Disable_Down, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableBtnDown));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Disable_Up, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableBtnUp));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonHold, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillBtnHold));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonHoldEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonHoldEnd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnAtkSelectHeroDown, new CUIEventManager.OnUIEventHandler(this.OnBattleAtkSelectHeroBtnDown));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnAtkSelectSoldierDown, new CUIEventManager.OnUIEventHandler(this.OnBattleAtkSelectSoldierBtnDown));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnLastHitButtonDown, new CUIEventManager.OnUIEventHandler(this.OnBattleLastHitBtnDown));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnLastHitButtonUp, new CUIEventManager.OnUIEventHandler(this.OnBattleLastHitBtnUp));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnAttackOrganButtonDown, new CUIEventManager.OnUIEventHandler(this.OnBattleAttackOrganBtnDown));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnAttackOrganButtonUp, new CUIEventManager.OnUIEventHandler(this.OnBattleAttackOrganBtnUp));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_LearnSkillBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleLearnSkillButtonClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_SkillLevelUpEffectPlayEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillLevelUpEffectPlayEnd));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroSoulLvlChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this.OnHeroSkillLvlup));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnCaptainSwitched));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.onHeroEnergyChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyMax", new Action<PoolObjHandle<ActorRoot>, int, int>(this.onHeroEnergyMax));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Common_BattleWifiCheckTimer, new CUIEventManager.OnUIEventHandler(this.OnCommon_BattleWifiCheckTimer));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Common_BattleShowOrHideWifiInfo, new CUIEventManager.OnUIEventHandler(this.OnCommon_BattleShowOrHideWifiInfo));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnUITypeChangeComplete, new CUIEventManager.OnUIEventHandler(this.OnUITypeChangeComplete));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_CameraDraging, new CUIEventManager.OnUIEventHandler(this.OnDropCamera));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_RevivieTimer, new CUIEventManager.OnUIEventHandler(this.OnReviveTimerChange));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ChangeSkillEventParam>(GameSkillEventDef.Event_ChangeSkill, new GameSkillEvent<ChangeSkillEventParam>(this.OnPlayerSkillChanged));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_RecoverSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillRecovered));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDChanged));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDStart, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDStart));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDEnd, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDEnd));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_ChangeSkillBean, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillBeanChanged));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_EnableSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillEnable));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_DisableSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillDisable));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_ProtectDisappear, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerProtectDisappear));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_SkillCooldown, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerSkillCooldown));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.Enent_EnergyShortage, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerEnergyShortage));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_SkillBeanShortage, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerSkillBeanShortage));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_NoSkillTarget, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerNoSkillTarget));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_Blindess, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerBlindess));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_LimitSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerLimitSkill));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_CancelLimitSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerCancelLimitSkill));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UpdateSkillUI, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerUpdateSkill));
			Singleton<GameEventSys>.GetInstance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorImmune, new RefAction<DefaultGameEventParam>(this.OnActorImmune));
			Singleton<GameEventSys>.GetInstance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorHurtAbsorb, new RefAction<DefaultGameEventParam>(this.OnActorHurtAbsorb));
			Singleton<GameEventSys>.GetInstance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_AutoAISwitch, new RefAction<DefaultGameEventParam>(this.OnSwitchAutoAI));
			Singleton<GameEventSys>.GetInstance().RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_UseSkill, new GameSkillEvent<ActorSkillEventParam>(this.onUseSkill));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Pause_Game, new CUIEventManager.OnUIEventHandler(this.OnBattlePauseGame));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Resume_Game, new CUIEventManager.OnUIEventHandler(this.OnBattleResumeGame));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_RecommendEquip_Buy, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipQuicklyBuy));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_EquipBoughtEffectPlayEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBoughtEffectPlayEnd));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this.OnActorGoldCoinInBattleChanged));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Trusteeship_Accept, new CUIEventManager.OnUIEventHandler(this.OnAcceptTrusteeship));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Trusteeship_Cancel, new CUIEventManager.OnUIEventHandler(this.OnCancelTrusteeship));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.VOICE_OpenSpeaker, new CUIEventManager.OnUIEventHandler(this.OnBattleOpenSpeaker));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.VOICE_OpenMic, new CUIEventManager.OnUIEventHandler(this.OnBattleOpenMic));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<CommonAttactType>(EventID.GAME_SETTING_COMMONATTACK_TYPE_CHANGE, new Action<CommonAttactType>(this.OnGameSettingCommonAttackTypeChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<LastHitMode>(EventID.GAME_SETTING_LASTHIT_MODE_CHANGE, new Action<LastHitMode>(this.OnGameSettingLastHitModeChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<AttackOrganMode>(EventID.GAME_SETTING_ATTACKORGAN_MODE_CHANGE, new Action<AttackOrganMode>(this.OnGameSettingAttackOrganModeChange));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Click_Scene, new CUIEventManager.OnUIEventHandler(this.OnClickBattleScene));
			Singleton<GameEventSys>.instance.RmvEventHandler<SoldierWaveParam>(GameEventDef.Event_SoldierWaveNext, new RefAction<SoldierWaveParam>(this.OnWaveNext));
			Singleton<GameEventSys>.instance.RmvEventHandler<SoldierWaveParam>(GameEventDef.Event_SoldierWaveNextRepeat, new RefAction<SoldierWaveParam>(this.OnWaveNextRepeat));
			this.UnRegisteredCommonAttackTargetEvent();
		}

		public bool OpenForm()
		{
			this._formCameraDragPanel = Singleton<CUIManager>.GetInstance().OpenForm(FightForm.s_cameraDragPanelPath, false, true);
			this._formEnemyHeroAtkScript = Singleton<CUIManager>.GetInstance().OpenForm(FightForm.s_enemyHeroAtkFormPath, false, true);
			this._formBuffSkillScript = Singleton<CUIManager>.GetInstance().OpenForm(FightForm.s_buffSkillFormPath, false, true);
			this._formSkillBtnScript = Singleton<CUIManager>.GetInstance().OpenForm(FightForm.s_skillBtnFormPath, false, true);
			this._formScript = Singleton<CUIManager>.GetInstance().OpenForm(FightForm.s_battleUIForm, false, true);
			this._formSkillCursorScript = Singleton<CUIManager>.GetInstance().OpenForm(FightForm.s_battleSkillCursor, false, true);
			this._formSceneScript = Singleton<CUIManager>.GetInstance().OpenForm(FightForm.s_battleScene, false, true);
			this._formCameraMoveScript = Singleton<CUIManager>.GetInstance().OpenForm(FightForm.s_battleCameraMove, false, true);
			this._formJoystickScript = Singleton<CUIManager>.GetInstance().OpenForm(FightForm.s_battleJoystick, false, true);
			if (null == this._formScript || null == this._formJoystickScript || null == this._formSkillCursorScript || this._formSceneScript == null || this._formCameraMoveScript == null || null == this._formSkillBtnScript || this._formBuffSkillScript == null || this._formEnemyHeroAtkScript == null || this._formCameraDragPanel == null)
			{
				return false;
			}
			this.m_isInBattle = true;
			this.m_SkillSlotList.Clear();
			this.InitWifiInfo();
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			this.m_skillButtonManager = new CSkillButtonManager();
			this.m_skillButtonManager.Init();
			SGameGraphicRaycaster component = this._formScript.GetComponent<SGameGraphicRaycaster>();
			if (component != null)
			{
				DebugHelper.Assert(component.raycastMode == SGameGraphicRaycaster.RaycastMode.Sgame_tile, "Form_Battle RayCast Mode 应该设置为Sgame_tile,请检查...");
			}
			this.m_Vocetimer = Singleton<CTimerManager>.instance.AddTimer(this.m_VoiceTipsShowTime, -1, new CTimer.OnTimeUpHandler(this.OnVoiceTimeEnd));
			Singleton<CTimerManager>.instance.PauseTimer(this.m_Vocetimer);
			Singleton<CTimerManager>.instance.ResetTimer(this.m_Vocetimer);
			this.m_VoiceMictime = Singleton<CTimerManager>.instance.AddTimer(this.m_VoiceTipsShowTime, -1, new CTimer.OnTimeUpHandler(this.OnVoiceMicTimeEnd));
			Singleton<CTimerManager>.instance.PauseTimer(this.m_VoiceMictime);
			Singleton<CTimerManager>.instance.ResetTimer(this.m_VoiceMictime);
			this.m_OpenSpeakerObj = this._formScript.transform.Find("MapPanel/Mini/Voice_OpenSpeaker");
			this.m_OpenSpeakerTipObj = this._formScript.transform.Find("MapPanel/Mini/Voice_OpenSpeaker/info");
			this.m_OpeankSpeakAnim = this._formScript.transform.Find("MapPanel/Mini/Voice_OpenSpeaker/voice_anim");
			this.m_OpeankBigMap = this._formScript.transform.Find("MapPanel/Mini/Button_OpenBigMap");
			this.m_OpenMicObj = this._formScript.transform.Find("MapPanel/Mini/Voice_OpenMic");
			this.m_OpenMicTipObj = this._formScript.transform.Find("MapPanel/Mini/Voice_OpenMic/info");
			this.m_OpenSpeakerTipText = this.m_OpenSpeakerTipObj.Find("Text").GetComponent<Text>();
			if (this.m_OpeankBigMap)
			{
				this.m_OpeankBigMap.gameObject.CustomSetActive(true);
			}
			if (this.m_OpenMicTipObj)
			{
				this.m_OpenMicTipObj.gameObject.CustomSetActive(false);
				this.m_OpenMicTipText = this.m_OpenMicTipObj.Find("Text").GetComponent<Text>();
			}
			if (this.m_OpenSpeakerObj)
			{
				this.m_OpenSpeakerObj.gameObject.CustomSetActive(true);
			}
			if (this.m_OpenMicObj)
			{
				this.m_OpenMicObj.gameObject.CustomSetActive(true);
			}
			try
			{
				MonoSingleton<VoiceSys>.GetInstance().UpdateMyVoiceIcon(0);
				if (MonoSingleton<VoiceSys>.GetInstance().IsUseVoiceSysSetting)
				{
					this.BattleOpenSpeak(null, true);
				}
				else
				{
					MonoSingleton<VoiceSys>.GetInstance().ClosenSpeakers();
					MonoSingleton<VoiceSys>.GetInstance().CloseMic();
				}
			}
			catch (Exception ex)
			{
				DebugHelper.Assert(false, "exception for closen speakers... {0} {1}", new object[]
				{
					ex.get_Message(),
					ex.get_StackTrace()
				});
			}
			this.SetJoyStickMoveType(GameSettings.JoyStickMoveType);
			this.SetJoyStickShowType(GameSettings.JoyStickShowType);
			this.SetFpsShowType(GameSettings.FpsShowType);
			this.SetCameraMoveMode(GameSettings.TheCameraMoveType);
			this.m_goBackProcessBar = new BackToCityProcessBar();
			this.m_goBackProcessBar.Init(Utility.FindChild(this._formScript.gameObject, "GoBackProcessBar"));
			this.treasureHud = new CTreasureHud();
			this.treasureHud.Init(Utility.FindChild(this._formScript.gameObject, "TreasurePanel"));
			this.treasureHud.Hide();
			this.starEvalPanel = new CStarEvalPanel();
			this.starEvalPanel.Init(Utility.FindChild(this._formScript.gameObject, "PVPTopRightPanel/StarEvalPanel"));
			this.starEvalPanel.reset();
			this.m_battleMisc = new BattleMisc();
			this.m_battleMisc.Init(Utility.FindChild(this._formScript.gameObject, "mis"), this._formScript);
			this.battleTaskView = new BattleTaskView();
			this.battleTaskView.Init(Utility.FindChild(this._formScript.gameObject, "TaskView"));
			GameObject obj = Utility.FindChild(this._formScript.gameObject, "PVPTopRightPanel/PanelBtn/ToggleAutoBtn");
			obj.CustomSetActive(!curLvelContext.IsMobaMode());
			if (Singleton<BattleLogic>.GetInstance().GetCurLvelContext().m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_GUIDE || !Singleton<BattleLogic>.GetInstance().GetCurLvelContext().canAutoAI)
			{
				obj.CustomSetActive(false);
			}
			GameObject widget = this._formScript.GetWidget(75);
			if (widget != null)
			{
				Singleton<CReplayKitSys>.GetInstance().InitReplayKit(widget.transform, false, true);
			}
			GameObject obj2 = Utility.FindChild(this._formScript.gameObject, "PVPTopRightPanel/PanelBtn/btnViewBattleInfo");
			obj2.CustomSetActive(curLvelContext.IsMobaMode());
			GameObject obj3 = Utility.FindChild(this._formScript.gameObject, "PVPTopRightPanel/panelTopRight/SignalPanel");
			obj3.CustomSetActive(curLvelContext.IsMobaMode());
			this.m_signalPanel = new SignalPanel();
			this.m_signalPanel.Init(this._formScript, this._formScript.GetWidget(6), null, curLvelContext.IsMobaMode());
			Singleton<InBattleMsgMgr>.instance.InitView(Utility.FindChild(this._formScript.gameObject, "PVPTopRightPanel/panelTopRight/SignalPanel/Button_Chat"), this._formScript);
			if (!curLvelContext.IsMobaMode())
			{
				if (this.m_OpenSpeakerObj)
				{
					this.m_OpenSpeakerObj.gameObject.CustomSetActive(false);
				}
				if (this.m_OpenMicObj)
				{
					this.m_OpenMicObj.gameObject.CustomSetActive(false);
				}
				if (this.m_OpeankSpeakAnim)
				{
					this.m_OpeankSpeakAnim.gameObject.CustomSetActive(false);
				}
				if (this.m_OpeankBigMap)
				{
					this.m_OpeankBigMap.gameObject.CustomSetActive(false);
				}
			}
			if (curLvelContext.m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_GUIDE)
			{
				if (this.m_OpenSpeakerObj)
				{
					this.m_OpenSpeakerObj.gameObject.CustomSetActive(false);
				}
				if (this.m_OpenMicObj)
				{
					this.m_OpenMicObj.gameObject.CustomSetActive(false);
				}
			}
			if (curLvelContext.m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_DEFEND)
			{
				this.soldierWaveView = new SoldierWave();
				this.soldierWaveView.Init(Utility.FindChild(this._formScript.gameObject, "PVPTopRightPanel/WaveStatistics"));
				this.soldierWaveView.Show();
			}
			GameObject obj4 = Utility.FindChild(this._formScript.gameObject, "PVPTopRightPanel/ScoreBoard");
			GameObject gameObject = Utility.FindChild(this._formScript.gameObject, "PVPTopRightPanel/ScoreBoardTime");
			GameObject obj5 = Utility.FindChild(this._formScript.gameObject, "PVPTopRightPanel/ScoreBoardPvE");
			GameObject gameObject2 = Utility.FindChild(this._formScript.gameObject, "MapPanel/DragonInfo");
			if (curLvelContext.IsMobaMode())
			{
				this.scoreBoard = new ScoreBoard();
				this.scoreBoard.Init(obj4, gameObject);
				this.scoreBoard.RegiseterEvent();
				this.scoreBoard.Show();
			}
			else
			{
				obj4.CustomSetActive(false);
				gameObject.CustomSetActive(false);
				if (curLvelContext.IsGameTypeAdventure())
				{
					this.scoreboardPvE = new ScoreboardPvE();
					this.scoreboardPvE.Init(obj5);
					this.scoreboardPvE.Show();
				}
			}
			if (Singleton<BattleLogic>.instance.m_dragonSpawn != null)
			{
				if (curLvelContext.IsMobaModeWithOutGuide() && curLvelContext.m_pvpPlayerNum == 10)
				{
					gameObject2.CustomSetActive(false);
				}
				else
				{
					gameObject2.CustomSetActive(true);
					this.m_dragonView = new BattleDragonView();
					this.m_dragonView.Init(gameObject2, Singleton<BattleLogic>.instance.m_dragonSpawn);
				}
			}
			else
			{
				gameObject2.CustomSetActive(false);
			}
			GameObject widget2 = this._formScript.GetWidget(39);
			GameObject widget3 = this._formScript.GetWidget(40);
			GameObject widget4 = this._formScript.GetWidget(41);
			if (curLvelContext.IsMultilModeWithWarmBattle() || curLvelContext.IsGameTypeArena())
			{
				widget2.CustomSetActive(false);
			}
			else
			{
				widget2.CustomSetActive(true);
			}
			widget3.CustomSetActive(false);
			widget4.CustomSetActive(false);
			if (this._formScript != null)
			{
				this._formScript.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (this._formJoystickScript != null)
			{
				this._formJoystickScript.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (this._formSkillCursorScript != null)
			{
				this._formSkillCursorScript.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (this._formSceneScript != null)
			{
				this._formSceneScript.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (this._formCameraMoveScript != null)
			{
				this._formCameraMoveScript.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (this._formSkillBtnScript != null)
			{
				this._formSkillBtnScript.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (this._formBuffSkillScript != null)
			{
				this._formBuffSkillScript.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (this._formEnemyHeroAtkScript != null)
			{
				this._formEnemyHeroAtkScript.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (this._formCameraDragPanel != null)
			{
				this._formCameraDragPanel.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (hostPlayer != null)
			{
				this.SetDeadMaskState(hostPlayer.Captain);
			}
			Singleton<InBattleMsgMgr>.instance.RegInBattleEvent();
			if (CSysDynamicBlock.bUnfinishBlock)
			{
				GameObject obj6 = Utility.FindChild(this._formScript.gameObject, "PVPTopRightPanel/panelTopRight/SignalPanel/Button_Chat");
				obj6.CustomSetActive(false);
			}
			this.m_showBuffDesc = new CBattleShowBuffDesc();
			this.m_showBuffDesc.Init(Utility.FindChild(this._formBuffSkillScript.gameObject, "BuffSkill"));
			this.m_enemyHeroAtkBtn = new CEnemyHeroAtkBtn();
			this.m_enemyHeroAtkBtn.Init(Utility.FindChild(this._formEnemyHeroAtkScript.gameObject, "EnemyHeroAtk"));
			GameObject widget5 = this._formScript.GetWidget(45);
			if (widget5 != null)
			{
				widget5.CustomSetActive(curLvelContext.IsMobaMode());
			}
			GameObject widget6 = this._formScript.GetWidget(46);
			if (widget6 != null)
			{
				Text component2 = widget6.GetComponent<Text>();
				if (component2 != null)
				{
					component2.set_text(0.ToString());
				}
			}
			this._joystick = this._formJoystickScript.GetWidget(0).transform.GetComponent<CUIJoystickScript>();
			this.m_hostHeroDeadInfo = new CHostHeroDeadInfo();
			if (this.m_hostHeroDeadInfo != null)
			{
				this.m_hostHeroDeadInfo.Init();
			}
			this.DominateVanguardWaves = Utility.FindChild(this._formScript.gameObject, "PVPTopRightPanel/ScoreBoard/DominateVanguardWaves");
			if (this.DominateVanguardWaves != null)
			{
				this.DominateVanguardWaves.CustomSetActive(false);
				this.DominateVanguardWavesText = this.DominateVanguardWaves.transform.GetChild(1).GetComponent<Text>();
				this.ClearDominateVanguardWavesText();
			}
			if (curLvelContext != null && curLvelContext.m_isShowTrainingHelper)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Training_HelperInit);
			}
			this.RegisterEvents();
			this.ResetFightFormUIShowType();
			return true;
		}

		public void SetBigMapBtnVisible(bool bShow)
		{
			if (this.m_OpeankBigMap != null)
			{
				this.m_OpeankBigMap.gameObject.CustomSetActive(bShow);
			}
		}

		private void InitWifiInfo()
		{
			GameObject widget = this._formScript.GetWidget(36);
			GameObject widget2 = this._formScript.GetWidget(37);
			ResGlobalInfo resGlobalInfo = new ResGlobalInfo();
			if (GameDataMgr.svr2CltCfgDict.TryGetValue(30u, out resGlobalInfo))
			{
				this.m_bShowRealPing = (resGlobalInfo.dwConfValue != 0u);
			}
			else
			{
				this.m_bShowRealPing = false;
			}
			if (widget2 != null)
			{
				this.m_wifiIcon = widget2.GetComponent<Image>();
			}
			if (widget != null)
			{
				this.m_wifiText = widget.transform.GetComponent<Text>();
			}
			GameObject widget3 = this._formScript.GetWidget(35);
			if (widget3 != null)
			{
				Transform transform = widget3.transform.Find("WifiTimer");
				if (transform != null)
				{
					CUITimerScript component = transform.GetComponent<CUITimerScript>();
					if (component != null)
					{
						component.SetOnChangedIntervalTime((float)this.m_wifiTimerInterval);
					}
				}
			}
		}

		private void SetDeadMaskState(PoolObjHandle<ActorRoot> captain)
		{
			if (captain && captain.handle.TheStaticData.TheBaseAttribute.DeadControl)
			{
				GameObject gameObject = Utility.FindChild(this._formCameraDragPanel.gameObject, "CameraDragPanel");
				if (gameObject != null)
				{
					Image component = gameObject.GetComponent<Image>();
					if (component != null)
					{
						component.enabled = false;
					}
				}
			}
		}

		private void RefreshDeadMaskState(PoolObjHandle<ActorRoot> captain, bool bDead)
		{
			if (captain && captain.handle.TheStaticData.TheBaseAttribute.DeadControl)
			{
				GameObject widget = this._formJoystickScript.GetWidget(1);
				if (widget != null)
				{
					widget.CustomSetActive(bDead);
				}
			}
		}

		public void BattleStart()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (this.scoreBoard != null && this.scoreBoard.IsShown())
			{
				this.scoreBoard.RegiseterEvent();
				this.scoreBoard.Show();
			}
			if (!curLvelContext.IsMobaMode())
			{
				Utility.FindChild(this._formScript.gameObject, "HeroHeadHud").CustomSetActive(true);
				this.heroHeadHud = Utility.FindChild(this._formScript.gameObject, "HeroHeadHud").GetComponent<HeroHeadHud>();
				this.heroHeadHud.Init();
				this._formScript.GetWidget(72).CustomSetActive(false);
			}
			else
			{
				Utility.FindChild(this._formScript.gameObject, "HeroHeadHud").CustomSetActive(false);
				GameObject widget = this._formScript.GetWidget(72);
				widget.CustomSetActive(true);
				this.m_heroInfoPanel = new HeroInfoPanel();
				this.m_heroInfoPanel.Init(widget);
			}
			this.ResetSkillButtonManager(Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain, false, SkillSlotType.SLOT_SKILL_COUNT);
			if (this.m_skillButtonManager != null)
			{
				this.m_skillButtonManager.InitializeCampHeroInfo(this._formScript, false);
				this.m_skillButtonManager.InitializeCampHeroInfo(this._formScript, true);
			}
			SLevelContext curLvelContext2 = Singleton<BattleLogic>.instance.GetCurLvelContext();
			SkillButton button = this.GetButton(SkillSlotType.SLOT_SKILL_4);
			SkillButton button2 = this.GetButton(SkillSlotType.SLOT_SKILL_5);
			SkillButton button3 = this.GetButton(SkillSlotType.SLOT_SKILL_7);
			SkillButton button4 = this.GetButton(SkillSlotType.SLOT_SKILL_6);
			if (curLvelContext2 != null && button2 != null && button2.m_button != null && button4 != null && button4.m_button != null && button3 != null && button3.m_button != null && button != null && button.m_button != null)
			{
				bool flag = false;
				if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL) && curLvelContext2.IsMobaModeWithOutGuide())
				{
					button2.m_button.CustomSetActive(true);
				}
				else
				{
					button2.m_button.CustomSetActive(false);
					bool flag2 = false;
					if (curLvelContext.IsGameTypeGuide())
					{
						if (CBattleGuideManager.Is5v5GuideLevel(curLvelContext.m_mapID))
						{
							flag2 = true;
						}
					}
					else if (curLvelContext.IsMobaModeWithOutGuide() && curLvelContext.m_pvpPlayerNum == 10)
					{
						flag2 = true;
					}
					if (flag2)
					{
						button4.m_button.transform.position = button.m_button.transform.position;
						button4.m_skillIndicatorFixedPosition = button.m_skillIndicatorFixedPosition;
					}
					button.m_button.transform.position = button3.m_button.transform.position;
					button.m_skillIndicatorFixedPosition = button3.m_skillIndicatorFixedPosition;
					button3.m_button.transform.position = button2.m_button.transform.position;
					button3.m_skillIndicatorFixedPosition = button2.m_skillIndicatorFixedPosition;
					flag = true;
				}
				button3.m_button.CustomSetActive(curLvelContext2.m_bEnableOrnamentSlot);
				if (!curLvelContext2.m_bEnableOrnamentSlot)
				{
					flag = true;
					button4.m_button.transform.position = button.m_button.transform.position;
					button4.m_skillIndicatorFixedPosition = button.m_skillIndicatorFixedPosition;
					button.m_button.transform.position = button3.m_button.transform.position;
					button.m_skillIndicatorFixedPosition = button3.m_skillIndicatorFixedPosition;
				}
				if (flag && this._formScript.m_sgameGraphicRaycaster)
				{
					this._formScript.m_sgameGraphicRaycaster.UpdateTiles();
				}
			}
			if (this.m_OpeankBigMap != null)
			{
				MiniMapSysUT.RefreshMapPointerBig(this.m_OpeankBigMap.gameObject);
			}
			if (this.m_OpenMicObj != null)
			{
				MiniMapSysUT.RefreshMapPointerBig(this.m_OpenMicObj.gameObject);
			}
			if (this.m_OpenSpeakerObj != null)
			{
				MiniMapSysUT.RefreshMapPointerBig(this.m_OpenSpeakerObj.gameObject);
			}
		}

		public void UpdateLogic(int delta)
		{
			if (this.m_isInBattle)
			{
				this.m_skillButtonManager.UpdateLogic(delta);
				if (this.m_showBuffDesc != null)
				{
					this.m_showBuffDesc.UpdateBuffCD(delta);
				}
				Singleton<CBattleSelectTarget>.GetInstance().Update(this.m_selectedHero);
			}
		}

		public void Update()
		{
			if (this.m_isInBattle)
			{
				if (this.scoreBoard != null)
				{
					this.scoreBoard.Update();
				}
				if (this.m_heroInfoPanel != null)
				{
					this.m_heroInfoPanel.Update();
				}
				if (this.scoreboardPvE != null)
				{
					this.scoreboardPvE.Update();
				}
				if (this.soldierWaveView != null)
				{
					this.soldierWaveView.Update();
				}
				if (this.m_signalPanel != null)
				{
					this.m_signalPanel.Update();
				}
				if (Singleton<InBattleMsgMgr>.instance != null)
				{
					Singleton<InBattleMsgMgr>.instance.Update();
				}
				if (Singleton<CBattleSystem>.instance.TheMinimapSys != null)
				{
					Singleton<CBattleSystem>.instance.TheMinimapSys.Update();
				}
				if (Singleton<TeleportTargetSelector>.GetInstance() != null)
				{
					Singleton<TeleportTargetSelector>.GetInstance().Update();
				}
			}
		}

		public void LateUpdate()
		{
			if (this.m_isInBattle)
			{
				if (this.scoreBoard != null)
				{
					this.scoreBoard.LateUpdate();
				}
				if (this.m_enemyHeroAtkBtn != null)
				{
					this.m_enemyHeroAtkBtn.LateUpdate();
				}
				if (this.m_skillButtonManager != null)
				{
					this.m_skillButtonManager.LateUpdate();
				}
			}
		}

		public void SetTopRightUIActive(bool bIsActive)
		{
			if (!bIsActive && Singleton<InBattleMsgMgr>.GetInstance().m_shortcutChat != null)
			{
				Singleton<InBattleMsgMgr>.GetInstance().m_shortcutChat.Show(false);
			}
		}

		public int GetDisplayPing()
		{
			return this.m_displayPing;
		}

		private void Battle_ActivateForm(CUIEvent uiEvent)
		{
			if (this._formScript != null)
			{
				this._formScript.Appear(enFormHideFlag.HideByCustom, true);
			}
			if (this._formSkillCursorScript != null)
			{
				this._formSkillCursorScript.Appear(enFormHideFlag.HideByCustom, true);
			}
			if (this._formJoystickScript != null)
			{
				this._formJoystickScript.Appear(enFormHideFlag.HideByCustom, true);
			}
			if (this._formSceneScript != null)
			{
				this._formSceneScript.Appear(enFormHideFlag.HideByCustom, true);
			}
			if (this._formCameraMoveScript != null)
			{
				this._formCameraMoveScript.Appear(enFormHideFlag.HideByCustom, true);
			}
			if (this._formSkillBtnScript != null)
			{
				this._formSkillBtnScript.Appear(enFormHideFlag.HideByCustom, true);
			}
			if (this._formBuffSkillScript != null)
			{
				this._formBuffSkillScript.Appear(enFormHideFlag.HideByCustom, true);
			}
			if (this._formEnemyHeroAtkScript)
			{
				this._formEnemyHeroAtkScript.Appear(enFormHideFlag.HideByCustom, true);
			}
			if (this._formCameraDragPanel)
			{
				this._formCameraDragPanel.Appear(enFormHideFlag.HideByCustom, true);
			}
		}

		public void ShowVoiceTips()
		{
			if (this.m_OpenSpeakerTipObj)
			{
				if (!MonoSingleton<VoiceSys>.GetInstance().IsUseVoiceSysSetting)
				{
					this.m_OpenSpeakerTipObj.gameObject.CustomSetActive(true);
					if (this.m_OpenSpeakerTipText)
					{
						this.m_OpenSpeakerTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_FirstTips);
					}
					if (this.m_OpeankSpeakAnim)
					{
						this.m_OpeankSpeakAnim.gameObject.CustomSetActive(true);
					}
					this.m_VocetimerFirst = Singleton<CTimerManager>.instance.AddTimer(10000, 1, new CTimer.OnTimeUpHandler(this.OnVoiceTimeEndFirst));
				}
				else
				{
					this.m_OpenSpeakerTipObj.gameObject.CustomSetActive(false);
					if (this.m_OpeankSpeakAnim)
					{
						this.m_OpeankSpeakAnim.gameObject.CustomSetActive(false);
					}
				}
			}
		}

		public void CloseForm()
		{
			Singleton<CUIManager>.GetInstance().CloseForm(FightForm.s_battleUIForm);
		}

		public SignalPanel GetSignalPanel()
		{
			return this.m_signalPanel;
		}

		public BattleMisc GetBattleMisc()
		{
			return this.m_battleMisc;
		}

		private void OnFormHide(CUIEvent uiEvent)
		{
			if (this._formJoystickScript != null)
			{
				this._formJoystickScript.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (this._formSkillCursorScript != null)
			{
				this._formSkillCursorScript.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (this._formSceneScript != null)
			{
				this._formSceneScript.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (this._formCameraMoveScript != null)
			{
				this._formCameraMoveScript.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (this._formSkillBtnScript != null)
			{
				this._formSkillBtnScript.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (this._formBuffSkillScript != null)
			{
				this._formBuffSkillScript.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (this._formEnemyHeroAtkScript != null)
			{
				this._formEnemyHeroAtkScript.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (this._formCameraDragPanel != null)
			{
				this._formCameraDragPanel.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (this.m_hostHeroDeadInfo != null)
			{
				this.m_hostHeroDeadInfo.OnDeadInfoFormClose(null);
			}
		}

		private void OnFormAppear(CUIEvent uiEvent)
		{
			if (this._formJoystickScript != null)
			{
				this._formJoystickScript.Appear(enFormHideFlag.HideByCustom, true);
			}
			if (this._formSkillCursorScript != null)
			{
				this._formSkillCursorScript.Appear(enFormHideFlag.HideByCustom, true);
			}
			if (this._formSceneScript != null)
			{
				this._formSceneScript.Appear(enFormHideFlag.HideByCustom, true);
			}
			if (this._formCameraMoveScript != null)
			{
				this._formCameraMoveScript.Appear(enFormHideFlag.HideByCustom, true);
			}
			if (this._formSkillBtnScript != null)
			{
				this._formSkillBtnScript.Appear(enFormHideFlag.HideByCustom, true);
			}
			if (this._formBuffSkillScript != null)
			{
				this._formBuffSkillScript.Appear(enFormHideFlag.HideByCustom, true);
			}
			if (this._formEnemyHeroAtkScript != null)
			{
				this._formEnemyHeroAtkScript.Appear(enFormHideFlag.HideByCustom, true);
			}
			if (this._formCameraDragPanel != null)
			{
				this._formCameraDragPanel.Appear(enFormHideFlag.HideByCustom, true);
			}
		}

		private void OnFormClosed(CUIEvent uiEvent)
		{
			Singleton<CBattleSystem>.GetInstance().OnFormClosed();
			this.UnregisterEvents();
			Singleton<InBattleMsgMgr>.instance.Clear();
			this.m_isInBattle = false;
			this.m_bOpenSpeak = false;
			this.m_bOpenMic = false;
			MonoSingleton<VoiceSys>.GetInstance().LeaveRoom();
			Singleton<CTimerManager>.instance.RemoveTimer(this.m_Vocetimer);
			Singleton<CTimerManager>.instance.RemoveTimer(this.m_VoiceMictime);
			Singleton<CTimerManager>.instance.RemoveTimer(this.m_VocetimerFirst);
			this.m_SkillSlotList.Clear();
			if (this.scoreBoard != null)
			{
				this.scoreBoard.Clear();
				this.scoreBoard = null;
			}
			if (this.m_heroInfoPanel != null)
			{
				this.m_heroInfoPanel.Clear();
				this.m_heroInfoPanel = null;
			}
			if (this.scoreboardPvE != null)
			{
				this.scoreboardPvE.Clear();
				this.scoreboardPvE = null;
			}
			if (this.treasureHud != null)
			{
				this.treasureHud.Clear();
				this.treasureHud = null;
			}
			if (this.starEvalPanel != null)
			{
				this.starEvalPanel.Clear();
				this.starEvalPanel = null;
			}
			if (this.battleTaskView != null)
			{
				this.battleTaskView.Clear();
				this.battleTaskView = null;
			}
			if (this.soldierWaveView != null)
			{
				this.soldierWaveView.Clear();
				this.soldierWaveView = null;
			}
			if (this.heroHeadHud != null)
			{
				this.heroHeadHud.Clear();
				this.heroHeadHud = null;
			}
			if (this.m_dragonView != null)
			{
				this.m_dragonView.Clear();
				this.m_dragonView = null;
			}
			if (this.m_signalPanel != null)
			{
				this.m_signalPanel.Clear();
				this.m_signalPanel = null;
			}
			if (this.m_battleMisc != null)
			{
				this.m_battleMisc.Uninit();
				this.m_battleMisc.Clear();
				this.m_battleMisc = null;
			}
			if (this.m_goBackProcessBar != null)
			{
				this.m_goBackProcessBar.Uninit();
				this.m_goBackProcessBar = null;
			}
			if (this.m_skillButtonManager != null)
			{
				this.m_skillButtonManager.Clear();
				this.m_skillButtonManager.Uninit();
				this.m_skillButtonManager = null;
			}
			this._joystick = null;
			this.m_bOpenSpeak = false;
			this.m_bOpenMic = false;
			this.m_VocetimerFirst = 0;
			this.m_Vocetimer = 0;
			this.m_VoiceMictime = 0;
			this.m_OpenSpeakerObj = null;
			this.m_OpenSpeakerTipObj = null;
			this.m_OpenSpeakerTipText = null;
			this.m_OpeankSpeakAnim = null;
			this.m_OpeankBigMap = null;
			this.m_OpenMicObj = null;
			this.m_OpenMicTipObj = null;
			this.m_OpenMicTipText = null;
			this.m_wifiIcon = null;
			this.m_wifiText = null;
			this.ClearDominateVanguardWavesText();
			this.DominateVanguardWaves = null;
			this.DominateVanguardWavesText = null;
			Singleton<CUIManager>.GetInstance().CloseForm(CSettingsSys.SETTING_FORM);
			Singleton<CUIManager>.GetInstance().CloseForm(FightForm.s_battleJoystick);
			Singleton<CUIManager>.GetInstance().CloseForm(FightForm.s_battleSkillCursor);
			Singleton<CUIManager>.GetInstance().CloseForm(FightForm.s_battleScene);
			Singleton<CUIManager>.GetInstance().CloseForm(FightForm.s_battleCameraMove);
			Singleton<CUIManager>.GetInstance().CloseForm(string.Format("{0}{1}", "UGUI/Form/Common/", "Form_SmallMessageBox.prefab"));
			Singleton<CBattleHeroInfoPanel>.GetInstance().Hide();
			this._formScript = null;
			if (this.m_showBuffDesc != null)
			{
				this.m_showBuffDesc.UnInit();
			}
			if (this.m_enemyHeroAtkBtn != null)
			{
				this.m_enemyHeroAtkBtn.UnInit();
			}
			if (this.m_hostHeroDeadInfo != null)
			{
				this.m_hostHeroDeadInfo.UnInit();
			}
		}

		public void StartGoBackProcessBar(uint startTime, uint totalTime, string text)
		{
			if (this.m_goBackProcessBar != null)
			{
				this.m_goBackProcessBar.Start(startTime, totalTime, text);
			}
		}

		public void UpdateGoBackProcessBar(uint curTime)
		{
			if (this.m_goBackProcessBar != null)
			{
				this.m_goBackProcessBar.Update(curTime);
			}
		}

		public void EndGoBackProcessBar()
		{
			if (this.m_goBackProcessBar != null)
			{
				this.m_goBackProcessBar.End();
			}
		}

		public void DisableUIEvent()
		{
			if (this._formScript != null && this._formScript.gameObject != null)
			{
				GraphicRaycaster component = this._formScript.gameObject.GetComponent<GraphicRaycaster>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
		}

		private void onActorDead(ref GameDeadEventParam prm)
		{
			if (ActorHelper.IsHostCtrlActor(ref prm.src) && this.m_skillButtonManager != null)
			{
				this.m_skillButtonManager.SkillButtonUp(this._formScript);
			}
		}

		private void UpdateDominateVanguardWavesText(bool bFriendly)
		{
			DebugHelper.Assert(this.DominateVanguardCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID);
			if (this.DominateVanguardWavesText != null)
			{
				this.DominateVanguardWavesText.set_text(string.Format("强化兵线波次： <color=#f457e5>{0}/{1}</color>", this.DominateVanguardRepeatCount, MonoSingleton<GlobalConfig>.instance.DominateVanguardRepeatTotal));
			}
		}

		private void ClearDominateVanguardWavesText()
		{
			if (this.DominateVanguardWaves != null)
			{
				this.DominateVanguardWaves.CustomSetActive(false);
			}
			this.DominateVanguardRepeatCount = 0;
			this.DominateVanguardCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
		}

		private void OnWaveNext(ref SoldierWaveParam inParam)
		{
			if (inParam.Wave == null || inParam.Wave.Region == null || inParam.Wave.Selector == null)
			{
				return;
			}
			uint configID = inParam.Wave.Selector.PeekNextSoldierId();
			ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff((int)configID);
			if (dataCfgInfoByCurLevelDiff != null && dataCfgInfoByCurLevelDiff.bSoldierType == 16)
			{
				if (this.DominateVanguardCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
				{
					this.DominateVanguardCamp = inParam.Wave.Region.CampType;
				}
				this.DominateVanguardRepeatCount = 1;
				this.UpdateDominateVanguardWavesText(this.DominateVanguardCamp == Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp);
				this.DominateVanguardWaves.CustomSetActive(true);
			}
		}

		private void OnWaveNextRepeat(ref SoldierWaveParam inParam)
		{
			if (inParam.Wave == null || inParam.Wave.Region == null || inParam.Wave.Selector == null)
			{
				return;
			}
			uint lastSoldierId = inParam.Wave.Selector.LastSoldierId;
			ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff((int)lastSoldierId);
			if (dataCfgInfoByCurLevelDiff != null)
			{
				COM_PLAYERCAMP campType = inParam.Wave.Region.CampType;
				if (dataCfgInfoByCurLevelDiff.bSoldierType == 16)
				{
					if (this.DominateVanguardCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
					{
						this.DominateVanguardCamp = campType;
					}
					this.DominateVanguardRepeatCount = inParam.RepeatCount;
					this.UpdateDominateVanguardWavesText(this.DominateVanguardCamp == Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp);
				}
				else if (this.DominateVanguardCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID && this.DominateVanguardCamp == campType)
				{
					this.ClearDominateVanguardWavesText();
				}
			}
		}

		public SkillButton GetButton(SkillSlotType skillSlotType)
		{
			return (this.m_skillButtonManager != null) ? this.m_skillButtonManager.GetButton(skillSlotType) : null;
		}

		public void SetButtonHighLight(GameObject button, bool highLight)
		{
			this.m_skillButtonManager.SetButtonHighLight(button, highLight);
		}

		public void SetLearnBtnHighLight(GameObject button, bool highLight)
		{
			this.m_skillButtonManager.SetlearnBtnHighLight(button, highLight);
		}

		public SkillSlotType GetCurSkillSlotType()
		{
			return this.m_skillButtonManager.GetCurSkillSlotType();
		}

		public void ResetSkillButtonManager(PoolObjHandle<ActorRoot> actor, bool bInitSpecifiedButton = false, SkillSlotType specifiedType = SkillSlotType.SLOT_SKILL_COUNT)
		{
			if (actor.handle == null || this.m_skillButtonManager == null)
			{
				return;
			}
			this.m_skillButtonManager.Initialise(actor, bInitSpecifiedButton, specifiedType);
			Singleton<EventRouter>.GetInstance().BroadCastEvent("ResetSkillButtonManager");
		}

		public void ResetHostPlayerSkillIndicatorSensitivity()
		{
			if (!Singleton<BattleLogic>.GetInstance().isFighting)
			{
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null)
			{
				return;
			}
			PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
			if (!captain || captain.handle.SkillControl == null)
			{
				return;
			}
			float moveSpeed = 0f;
			float rotateSpeed = 0f;
			GameSettings.GetLunPanSensitivity(out moveSpeed, out rotateSpeed);
			for (int i = 0; i < captain.handle.SkillControl.SkillSlotArray.Length; i++)
			{
				SkillSlot skillSlot = captain.handle.SkillControl.SkillSlotArray[i];
				if (skillSlot != null && skillSlot.skillIndicator != null)
				{
					skillSlot.skillIndicator.SetIndicatorSpeed(moveSpeed, rotateSpeed);
				}
			}
		}

		public void OnToggleFreeCamera(CUIEvent uiEvent)
		{
			MonoSingleton<CameraSystem>.instance.ToggleFreeCamera();
		}

		public void OnToggleAutoAI(CUIEvent uiEvent)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer != null && hostPlayer.Captain && hostPlayer.Captain.handle.ActorControl != null)
			{
				FrameCommand<SwitchActorAutoAICommand> frameCommand = FrameCommandFactory.CreateFrameCommand<SwitchActorAutoAICommand>();
				frameCommand.cmdData.IsAutoAI = (hostPlayer.Captain.handle.ActorControl.m_isAutoAI ? 0 : 1);
				frameCommand.Send();
				Transform transform = uiEvent.m_srcWidget.gameObject.transform.Find("imgAuto");
				transform.gameObject.CustomSetActive(!hostPlayer.Captain.handle.ActorControl.m_isAutoAI);
				MonoSingleton<DialogueProcessor>.GetInstance().bAutoNextPage = ((int)frameCommand.cmdData.IsAutoAI != 0);
			}
		}

		public void ShowSysMenu(CUIEvent uiEvent)
		{
			Singleton<CUIParticleSystem>.instance.Hide(null);
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Settings_OpenForm);
		}

		public void ShowTaskView(bool show)
		{
			if (this.battleTaskView != null)
			{
				this.battleTaskView.Visible = show;
			}
		}

		private void OnBigMap_Open_BigMap(CUIEvent uievent)
		{
		}

		public void OnSwitchAutoAI(ref DefaultGameEventParam param)
		{
			if (Singleton<GamePlayerCenter>.instance != null && Singleton<GamePlayerCenter>.instance.GetHostPlayer() != null && Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain && param.src == Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain)
			{
				GameObject gameObject = (this._formScript != null) ? Utility.FindChild(this._formScript.gameObject, "PVPTopRightPanel/PanelBtn/ToggleAutoBtn") : null;
				if (gameObject == null)
				{
					return;
				}
				Transform transform = gameObject.transform.Find("imgAuto");
				if (transform == null)
				{
					return;
				}
				transform.gameObject.CustomSetActive(Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain.handle.ActorControl.m_isAutoAI);
				MonoSingleton<DialogueProcessor>.GetInstance().bAutoNextPage = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain.handle.ActorControl.m_isAutoAI;
			}
		}

		private void onReturnLobby(CUIEvent uiEvent)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext == null)
			{
				return;
			}
			if (curLvelContext.IsMultilModeWithWarmBattle())
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("多人对战不能退出游戏。"), false, 1.5f, null, new object[0]);
			}
			else
			{
				this.onConfirmReturnLobby(null);
			}
		}

		private void onConfirmReturnLobby(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CSettingsSys.SETTING_FORM);
			if (Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsGameTypeGuide() && !Singleton<CBattleGuideManager>.instance.bTrainingAdv)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Tutorial_Level_Qiut_Tip"), false, 1.5f, null, new object[0]);
				return;
			}
			if (Singleton<CBattleGuideManager>.instance.bPauseGame)
			{
				Singleton<CBattleGuideManager>.instance.ResumeGame(this);
			}
			if (Singleton<LobbyLogic>.instance.inMultiGame)
			{
				Singleton<LobbyLogic>.instance.ReqMultiGameRunaway();
			}
			else
			{
				Singleton<BattleLogic>.instance.DoFightOver(false);
				Singleton<BattleLogic>.instance.SingleReqLoseGame();
			}
		}

		private void onChangeOperateMode(CUIEvent uiEvent)
		{
		}

		private void onQuitGame(CUIEvent uiEvent)
		{
			GameObject obj = Utility.FindChild(this._formScript.gameObject, "SysMenu");
			obj.CustomSetActive(false);
			SGameApplication.Quit();
		}

		private void onReturnGame(CUIEvent uiEvent)
		{
			GameObject obj = Utility.FindChild(this._formScript.gameObject, "SysMenu");
			obj.CustomSetActive(false);
		}

		public void OnSwitchHeroInfoPanel(CUIEvent uiEvent)
		{
		}

		public void OnOpenHeorInfoPanel(CUIEvent uiEvent)
		{
			CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_ButtonViewSkillInfo);
			Singleton<CBattleHeroInfoPanel>.GetInstance().Show();
		}

		public void OnCloseHeorInfoPanel(CUIEvent uiEvent)
		{
			Singleton<CBattleHeroInfoPanel>.GetInstance().Hide();
		}

		private void OnPlayerProtectDisappear(ref ActorSkillEventParam _param)
		{
			if (_param.src)
			{
				Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.ShieldDisappear, (Vector3)_param.src.handle.location, new string[0]);
			}
		}

		private void OnActorImmune(ref DefaultGameEventParam _param)
		{
			if (_param.src && ActorHelper.IsHostActor(ref _param.src))
			{
				Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.Immunity, (Vector3)_param.src.handle.location, new string[0]);
			}
			else if (_param.atker && ActorHelper.IsHostActor(ref _param.atker))
			{
				Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.Immunity, (Vector3)_param.src.handle.location, new string[0]);
			}
		}

		private void OnActorHurtAbsorb(ref DefaultGameEventParam _param)
		{
			if (_param.src && ActorHelper.IsHostActor(ref _param.src))
			{
				Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.Absorb, (Vector3)_param.src.handle.location, new string[0]);
			}
			else if (_param.atker && ActorHelper.IsHostActor(ref _param.atker))
			{
				Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.Absorb, (Vector3)_param.src.handle.location, new string[0]);
			}
		}

		private void OnPlayerBlindess(ref ActorSkillEventParam _param)
		{
			if (_param.src)
			{
				Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.Blindess, (Vector3)_param.src.handle.location, new string[0]);
			}
		}

		private void OnPlayerSkillCooldown(ref ActorSkillEventParam _param)
		{
			if (_param.src)
			{
				float time = Time.time;
				if (time - this.timeSkillCooldown > 2f)
				{
					Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.InCooldown, (Vector3)_param.src.handle.location, new string[0]);
					this.timeSkillCooldown = time;
				}
			}
		}

		private void OnPlayerEnergyShortage(ref ActorSkillEventParam _param)
		{
			if (_param.src)
			{
				float time = Time.time;
				if (time - this.timeEnergyShortage > 2f)
				{
					if (_param.src.handle.ValueComponent == null)
					{
						return;
					}
					int energyType = (int)_param.src.handle.ValueComponent.mActorValue.EnergyType;
					Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(EnergyCommon.GetShortageText(energyType), (Vector3)_param.src.handle.location, new string[0]);
					this.timeEnergyShortage = time;
				}
			}
		}

		private void OnPlayerSkillBeanShortage(ref ActorSkillEventParam _param)
		{
			if (_param.src)
			{
				float time = Time.time;
				if (time - this.timeSkillBeanShortage > 2f)
				{
					Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.BeanShortage, (Vector3)_param.src.handle.location, new string[0]);
					this.timeSkillBeanShortage = time;
				}
			}
		}

		private void OnPlayerNoSkillTarget(ref ActorSkillEventParam _param)
		{
			if (_param.src)
			{
				float time = Time.time;
				if (time - this.timeNoSkillTarget > 2f)
				{
					Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.NoTarget, (Vector3)_param.src.handle.location, new string[0]);
					this.timeNoSkillTarget = time;
				}
			}
		}

		private void OnPlayerSkillChanged(ref ChangeSkillEventParam _param)
		{
			if (this._formScript == null)
			{
				return;
			}
			this.m_skillButtonManager.ChangeSkill(_param.slot, ref _param);
		}

		private void OnPlayerSkillRecovered(ref DefaultSkillEventParam _param)
		{
			if (this._formScript == null)
			{
				return;
			}
			this.m_skillButtonManager.RecoverSkill(_param.slot, ref _param);
		}

		private void OnPlayerSkillBeanChanged(ref DefaultSkillEventParam _param)
		{
			if (this._formScript == null)
			{
				return;
			}
			if (_param.actor && ActorHelper.IsHostCtrlActor(ref _param.actor))
			{
				this.m_skillButtonManager.UpdateButtonBeanNum(_param.slot, _param.param);
			}
		}

		private void OnPlayerSkillCDChanged(ref DefaultSkillEventParam _param)
		{
			if (this._formScript == null)
			{
				return;
			}
			if (_param.actor && ActorHelper.IsHostCtrlActor(ref _param.actor))
			{
				this.m_skillButtonManager.UpdateButtonCD(_param.slot, _param.param);
			}
		}

		private void OnPlayerSkillCDStart(ref DefaultSkillEventParam _param)
		{
			if (this._formScript == null)
			{
				return;
			}
			this.m_skillButtonManager.SetButtonCDStart(_param.slot);
		}

		private void OnPlayerSkillCDEnd(ref DefaultSkillEventParam _param)
		{
			if (this._formScript == null)
			{
				return;
			}
			this.m_skillButtonManager.SetButtonCDOver(_param.slot, true);
		}

		private void OnPlayerSkillEnable(ref DefaultSkillEventParam _param)
		{
			if (this._formScript == null)
			{
				return;
			}
			this.m_skillButtonManager.SetEnableButton(_param.slot);
		}

		private void OnPlayerSkillDisable(ref DefaultSkillEventParam _param)
		{
			if (this._formScript == null)
			{
				return;
			}
			this.m_skillButtonManager.SetDisableButton(_param.slot);
		}

		private void OnPlayerLimitSkill(ref DefaultSkillEventParam _param)
		{
			if (this._formScript == null)
			{
				return;
			}
			this.m_skillButtonManager.SetLimitButton(_param.slot);
		}

		private void OnPlayerUpdateSkill(ref DefaultSkillEventParam _param)
		{
			if (this._formScript == null)
			{
				return;
			}
			if (_param.slot != SkillSlotType.SLOT_SKILL_0)
			{
				PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
				this.m_skillButtonManager.Initialise(captain, true, _param.slot);
			}
		}

		private void OnPlayerCancelLimitSkill(ref DefaultSkillEventParam _param)
		{
			if (this._formScript == null)
			{
				return;
			}
			this.m_skillButtonManager.CancelLimitButton(_param.slot);
		}

		private void OnBattleSkillButtonDown(CUIEvent uiEvent)
		{
			if (this.m_signalPanel != null)
			{
				this.m_signalPanel.CancelSelectedSignalButton();
			}
			stUIEventParams stUIEventParams = default(stUIEventParams);
			stUIEventParams eventParams = uiEvent.m_eventParams;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Newbie_CloseSkillGesture, eventParams);
			if (this.IsSkillTipsActive())
			{
				this.HideSkillDescInfo();
			}
			this.m_skillButtonManager.SkillButtonDown(uiEvent.m_srcFormScript, uiEvent.m_eventParams.m_skillSlotType, uiEvent.m_pointerEventData.get_position());
		}

		private void OnBattleSkillButtonUp(CUIEvent uiEvent)
		{
			this.m_skillButtonManager.SkillButtonUp(uiEvent.m_srcFormScript, uiEvent.m_eventParams.m_skillSlotType, true, uiEvent.m_pointerEventData.get_position());
		}

		private void OnBattleLastHitBtnDown(CUIEvent uiEvent)
		{
			if (this.m_skillButtonManager != null)
			{
				this.m_skillButtonManager.LastHitButtonDown(uiEvent.m_srcFormScript);
			}
		}

		private void OnBattleLastHitBtnUp(CUIEvent uiEvent)
		{
			if (this.m_skillButtonManager != null)
			{
				this.m_skillButtonManager.LastHitButtonUp(uiEvent.m_srcFormScript);
			}
		}

		private void OnBattleAttackOrganBtnDown(CUIEvent uiEvent)
		{
			if (this.m_skillButtonManager != null)
			{
				this.m_skillButtonManager.AttackOrganButtonDown(uiEvent.m_srcFormScript);
			}
		}

		private void OnBattleAttackOrganBtnUp(CUIEvent uiEvent)
		{
			if (this.m_skillButtonManager != null)
			{
				this.m_skillButtonManager.AttackOrganButtonUp(uiEvent.m_srcFormScript);
			}
		}

		private void OnGameSettingLastHitModeChange(LastHitMode mode)
		{
			if (!Singleton<BattleLogic>.instance.isRuning || this.m_skillButtonManager == null)
			{
				return;
			}
			this.m_skillButtonManager.SetLastHitBtnState(mode == LastHitMode.LastHit);
		}

		private void OnGameSettingAttackOrganModeChange(AttackOrganMode mode)
		{
			if (!Singleton<BattleLogic>.instance.isRuning || this.m_skillButtonManager == null)
			{
				return;
			}
			this.m_skillButtonManager.SetAttackOrganBtnState(mode == AttackOrganMode.AttackOrgan);
		}

		private void OnBattleSkillButtonHoldEnd(CUIEvent uiEvent)
		{
			if (this.m_isSkillDecShow)
			{
				this.HideSkillDescInfo();
				this.m_skillButtonManager.SkillButtonUp(uiEvent.m_srcFormScript, uiEvent.m_eventParams.m_skillSlotType, false, default(Vector2));
			}
		}

		private void OnBattleSkillButtonDragged(CUIEvent uiEvent)
		{
			this.m_skillButtonManager.DragSkillButton(uiEvent.m_srcFormScript, uiEvent.m_eventParams.m_skillSlotType, uiEvent.m_pointerEventData.get_position());
		}

		private void OnBattleSkillDisableAlert(CUIEvent uiEvent)
		{
			this.m_skillButtonManager.OnBattleSkillDisableAlert(uiEvent.m_eventParams.m_skillSlotType);
		}

		public void onQuitAppClick()
		{
			SGameApplication.Quit();
		}

		public void OnBattlePauseGame(CUIEvent uiEvent)
		{
			Singleton<CBattleGuideManager>.instance.PauseGame(this, true);
			if (this._formScript != null)
			{
				this._formScript.GetWidget(39).CustomSetActive(false);
				this._formScript.GetWidget(40).CustomSetActive(true);
				this._formScript.GetWidget(41).CustomSetActive(true);
			}
		}

		public void OnBattleResumeGame(CUIEvent uiEvent)
		{
			Singleton<CBattleGuideManager>.instance.ResumeGame(this);
			if (this._formScript != null)
			{
				this._formScript.GetWidget(40).CustomSetActive(false);
				this._formScript.GetWidget(39).CustomSetActive(true);
				this._formScript.GetWidget(41).CustomSetActive(false);
			}
		}

		public void ShowWinLosePanel(bool bWin)
		{
			Singleton<WinLose>.GetInstance().ShowPanel(bWin);
			Singleton<CRecordUseSDK>.instance.DoFightOver();
		}

		public void SetDragonUINum(COM_PLAYERCAMP camp, byte num)
		{
			if (this.m_dragonView != null)
			{
				this.m_dragonView.SetDrgonNum(camp, num);
			}
		}

		private void OnActorGoldCoinInBattleChanged(PoolObjHandle<ActorRoot> actor, int changeValue, bool isIncome, PoolObjHandle<ActorRoot> target)
		{
			if (!Singleton<BattleLogic>.GetInstance().m_LevelContext.IsMobaMode())
			{
				return;
			}
			if (!actor || (!ActorHelper.IsHostCtrlActor(ref actor) && !ActorHelper.IsHostCtrlActorCaller(ref actor)))
			{
				return;
			}
			if (this._formScript != null)
			{
				GameObject widget = this._formScript.GetWidget(46);
				if (widget != null)
				{
					Text component = widget.GetComponent<Text>();
					int num = 0;
					if (actor && actor.handle.ValueComponent != null)
					{
						num = actor.handle.ValueComponent.GetGoldCoinInBattle();
					}
					if (component != null)
					{
						component.set_text(num.ToString());
					}
				}
			}
		}

		public void OnDragonTipFormOpen(CUIEvent cuiEvent)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.instance.OpenForm(FightForm.s_battleDragonTipForm, false, true);
			Text component = Utility.FindChild(cUIFormScript.gameObject, "DragonBuffx1Text").GetComponent<Text>();
			Text component2 = Utility.FindChild(cUIFormScript.gameObject, "DragonBuffx2Text").GetComponent<Text>();
			Text component3 = Utility.FindChild(cUIFormScript.gameObject, "DragonBuffx3Text").GetComponent<Text>();
			ResSkillCombineCfgInfo dataByKey = GameDataMgr.skillCombineDatabin.GetDataByKey((long)Singleton<BattleLogic>.instance.GetDragonBuffId(RES_SKILL_SRC_TYPE.RES_SKILL_SRC_DRAGON_ONE));
			ResSkillCombineCfgInfo dataByKey2 = GameDataMgr.skillCombineDatabin.GetDataByKey((long)Singleton<BattleLogic>.instance.GetDragonBuffId(RES_SKILL_SRC_TYPE.RES_SKILL_SRC_DRAGON_TWO));
			ResSkillCombineCfgInfo dataByKey3 = GameDataMgr.skillCombineDatabin.GetDataByKey((long)Singleton<BattleLogic>.instance.GetDragonBuffId(RES_SKILL_SRC_TYPE.RES_SKILL_SRC_DRAGON_THREE));
			if (dataByKey != null)
			{
				component.set_text(Utility.UTF8Convert(dataByKey.szSkillCombineDesc));
			}
			if (dataByKey2 != null)
			{
				component2.set_text(Utility.UTF8Convert(dataByKey2.szSkillCombineDesc));
			}
			if (dataByKey3 != null)
			{
				component3.set_text(Utility.UTF8Convert(dataByKey3.szSkillCombineDesc));
			}
		}

		public void OnDragonTipFormClose(CUIEvent cuiEvent)
		{
			Singleton<CUIManager>.instance.CloseForm(FightForm.s_battleDragonTipForm);
		}

		public void OnUpdateDragonUI(int delta)
		{
			if (this.m_dragonView != null)
			{
				this.m_dragonView.UpdateDragon(delta);
			}
		}

		private void OnCommon_BattleWifiCheckTimer(CUIEvent uiEvent)
		{
			if (this._formScript == null)
			{
				return;
			}
			this.m_displayPing = (this.m_bShowRealPing ? Singleton<FrameSynchr>.instance.RealSvrPing : Singleton<FrameSynchr>.GetInstance().GameSvrPing);
			this.m_displayPing = ((this.m_displayPing > 100) ? ((this.m_displayPing - 100) * 7 / 10 + 100) : this.m_displayPing);
			this.m_displayPing = Mathf.Clamp(this.m_displayPing, 0, 460);
			if (this.m_displayPing == 0)
			{
				this.m_displayPing = 10;
				SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
				if (curLvelContext != null && curLvelContext.m_isWarmBattle && curLvelContext.IsGameTypeComBat())
				{
					int num = Random.Range(0, 10);
					if (num == 0)
					{
						this.m_displayPing = 50 + Random.Range(0, 100);
					}
					else if (num < 3)
					{
						this.m_displayPing = 50 + Random.Range(0, 50);
					}
					else if (num < 6)
					{
						this.m_displayPing = 50 + Random.Range(0, 30);
					}
					else
					{
						this.m_displayPing = 50 + Random.Range(0, 15);
					}
				}
			}
			uint num2;
			if (this.m_displayPing < 100)
			{
				num2 = 2u;
			}
			else if (this.m_displayPing < 200)
			{
				num2 = 1u;
			}
			else
			{
				num2 = 0u;
			}
			if (this.m_wifiIcon != null && this.m_wifiIconCheckTicks >= this.m_wifiIconCheckMaxTicks)
			{
				enNetWorkType netWorkType = CUICommonSystem.GetNetWorkType();
				if (netWorkType == enNetWorkType.enNone)
				{
					this.m_wifiIcon.SetSprite(CUIUtility.s_Sprite_System_Wifi_Dir + CLobbySystem.s_noNetStateName, this._formScript, true, true, false, false);
				}
				else if (netWorkType == enNetWorkType.enWifi)
				{
					this.m_wifiIcon.SetSprite(CUIUtility.s_Sprite_System_Wifi_Dir + CLobbySystem.s_wifiStateName[(int)((uint)((UIntPtr)num2))], this._formScript, true, true, false, false);
				}
				else if (netWorkType == enNetWorkType.enNet)
				{
					this.m_wifiIcon.SetSprite(CUIUtility.s_Sprite_System_Wifi_Dir + CLobbySystem.s_netStateName[(int)((uint)((UIntPtr)num2))], this._formScript, true, true, false, false);
				}
				this.m_wifiIconCheckTicks = 0;
			}
			else
			{
				this.m_wifiIconCheckTicks++;
			}
			if (this.m_wifiText != null)
			{
				this.m_wifiText.set_text(string.Format("{0}ms", this.m_displayPing));
				this.m_wifiText.set_color(CLobbySystem.s_WifiStateColor[(int)((uint)((UIntPtr)num2))]);
			}
		}

		private void OnCommon_BattleShowOrHideWifiInfo(CUIEvent uiEvent)
		{
			GameObject widget = this._formScript.GetWidget(37);
			DebugHelper.Assert(widget != null);
			if (widget != null)
			{
				widget.CustomSetActive(!widget.activeInHierarchy);
			}
		}

		public void EnableCameraDragPanelForDead()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (this._formScript != null && curLvelContext != null)
			{
				GameObject gameObject = Utility.FindChild(this._formCameraDragPanel.gameObject, "CameraDragPanel");
				if (curLvelContext.IsMobaMode() && gameObject != null)
				{
					gameObject.CustomSetActive(true);
				}
				if (curLvelContext.IsMobaMode())
				{
					PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
					if (captain && this._formScript != null)
					{
						this.RefreshDeadMaskState(captain, true);
						if (gameObject != null)
						{
							Transform transform = gameObject.transform.Find("panelDeadInfo");
							if (transform != null)
							{
								Transform transform2 = transform.Find("Timer");
								if (transform2 != null)
								{
									CUITimerScript component = transform2.GetComponent<CUITimerScript>();
									if (component != null)
									{
										component.SetTotalTime(36000f);
										component.StartTimer();
										this.OnReviveTimerChange(null);
									}
								}
								transform.gameObject.CustomSetActive(true);
							}
						}
					}
				}
			}
		}

		private void OnReviveTimerChange(CUIEvent uiEvent)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (this._formScript != null && curLvelContext != null)
			{
				GameObject gameObject = Utility.FindChild(this._formCameraDragPanel.gameObject, "CameraDragPanel");
				Text component = gameObject.transform.Find("panelDeadInfo/lblReviveTime").GetComponent<Text>();
				Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
				if (hostPlayer != null)
				{
					PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
					if (captain)
					{
						float num = (float)captain.handle.ActorControl.ReviveCooldown * 0.001f;
						component.set_text(string.Format("{0}", Mathf.FloorToInt(num + 0.2f)));
					}
				}
			}
		}

		public void DisableCameraDragPanelForRevive()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (this._formScript != null && curLvelContext != null)
			{
				GameObject gameObject = Utility.FindChild(this._formCameraDragPanel.gameObject, "CameraDragPanel");
				if (curLvelContext.IsMobaMode())
				{
					gameObject.CustomSetActive(false);
				}
				if (curLvelContext.IsMobaMode())
				{
					PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
					if (captain && this._formScript != null)
					{
						Transform transform = (gameObject != null) ? gameObject.transform.Find("panelDeadInfo") : null;
						this.RefreshDeadMaskState(captain, false);
						if (transform != null)
						{
							Transform transform2 = transform.Find("Timer");
							if (transform2 != null)
							{
								CUITimerScript component = transform2.GetComponent<CUITimerScript>();
								if (component != null)
								{
									component.EndTimer();
								}
							}
							transform.gameObject.CustomSetActive(false);
						}
					}
				}
			}
		}

		private void OnDropCamera(CUIEvent uiEvent)
		{
			MonoSingleton<CameraSystem>.GetInstance().MoveCamera(-uiEvent.m_pointerEventData.get_delta().x, -uiEvent.m_pointerEventData.get_delta().y);
		}

		public void ShowArenaTimer()
		{
			GameObject gameObject = Utility.FindChild(this._formScript.gameObject, "PVPTopRightPanel/ArenaTimer63s");
			if (gameObject == null)
			{
				return;
			}
			Transform transform = gameObject.transform.Find("Timer");
			if (transform)
			{
				CUITimerScript component = transform.GetComponent<CUITimerScript>();
				if (component)
				{
					component.ReStartTimer();
				}
			}
			gameObject.CustomSetActive(true);
		}

		public CUIJoystickScript GetJoystick()
		{
			return this._joystick;
		}

		public void SetJoyStickMoveType(int moveType)
		{
			if (this._formJoystickScript == null)
			{
				return;
			}
			CUIJoystickScript component = this._formJoystickScript.GetWidget(0).transform.GetComponent<CUIJoystickScript>();
			if (component == null)
			{
				return;
			}
			if (moveType == 0 || CCheatSystem.IsJoystickForceMoveable())
			{
				component.m_isAxisMoveable = true;
			}
			else
			{
				component.m_isAxisMoveable = false;
			}
		}

		public void SetJoyStickShowType(int showType)
		{
			if (this._formScript == null)
			{
				return;
			}
			if (showType == 0)
			{
				this.m_skillButtonManager.SetSkillIndicatorMode(enSkillIndicatorMode.FixedPosition);
			}
			else
			{
				this.m_skillButtonManager.SetSkillIndicatorMode(enSkillIndicatorMode.General);
			}
		}

		public void SetFpsShowType(int showType)
		{
			if (this._formScript == null)
			{
				return;
			}
			bool bActive = showType == 1;
			this._formScript.GetWidget(32).CustomSetActive(bActive);
		}

		public void SetCameraMoveMode(CameraMoveType cameraMoveType)
		{
			if (this._formCameraMoveScript == null)
			{
				return;
			}
			bool bActive = false;
			bool bActive2 = false;
			switch (cameraMoveType)
			{
			case CameraMoveType.Close:
				bActive = false;
				bActive2 = false;
				break;
			case CameraMoveType.JoyStick:
				bActive = true;
				bActive2 = false;
				break;
			case CameraMoveType.Slide:
				bActive = false;
				bActive2 = true;
				break;
			}
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null && !curLvelContext.m_isCanRightJoyStickCameraDrag)
			{
				bActive = false;
				bActive2 = false;
			}
			GameObject widget = this._formCameraMoveScript.GetWidget(0);
			GameObject widget2 = this._formCameraMoveScript.GetWidget(1);
			if (widget != null)
			{
				widget.CustomSetActive(bActive);
			}
			if (widget2 != null)
			{
				widget2.CustomSetActive(bActive2);
			}
		}

		protected void UpdateFpsAndLag(CUIEvent uiEvent)
		{
			if (this._formScript == null)
			{
				return;
			}
			GameObject widget = this._formScript.GetWidget(32);
			uint num = (uint)GameFramework.m_fFps;
			if (widget != null)
			{
				Text component = widget.transform.FindChild("FPSText").gameObject.GetComponent<Text>();
				component.set_text(string.Format("FPS {0}", num));
				if (CheatCommandCommonEntry.CPU_CLOCK_ENABLE)
				{
					component.set_text(string.Format("FPS {0}\n{1}Mhz-{2}Mhz", num, Utility.GetCpuCurrentClock(), Utility.GetCpuMinClock()));
				}
			}
			this._lastFps = num;
		}

		private void onUseSkill(ref ActorSkillEventParam prm)
		{
			if (!ActorHelper.IsHostCtrlActor(ref prm.src))
			{
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			int configId = hostPlayer.Captain.handle.TheActorMeta.ConfigId;
			SLOTINFO sLOTINFO = null;
			for (int i = 0; i < this.m_SkillSlotList.Count; i++)
			{
				if (this.m_SkillSlotList[i].id == configId)
				{
					sLOTINFO = this.m_SkillSlotList[i];
					break;
				}
			}
			if (sLOTINFO == null)
			{
				sLOTINFO = new SLOTINFO();
				sLOTINFO.id = configId;
				this.m_SkillSlotList.Add(sLOTINFO);
			}
			sLOTINFO.m_SKillSlot[(int)prm.slot] += 1u;
		}

		private void OnBattleEquipQuicklyBuy(CUIEvent uiEvent)
		{
			bool flag = GameSettings.IsOpenFightFormUiTypeSwitchFunc();
			EquipPosType equipPosMode = GameSettings.EquipPosMode;
			int num = 50;
			if (flag && equipPosMode == EquipPosType.EquipPosRightMode)
			{
				num = 55;
			}
			if (this._formScript != null)
			{
				GameObject widget = this._formScript.GetWidget(num + uiEvent.m_eventParams.battleEquipPar.m_indexInQuicklyBuyList);
				if (widget != null)
				{
					widget.CustomSetActive(true);
					CUIAnimationScript component = widget.GetComponent<CUIAnimationScript>();
					if (component != null)
					{
						component.PlayAnimation("Battle_UI_ZhuangBei_01", true);
					}
				}
				GameObject widget2 = this._formScript.GetWidget(52 + uiEvent.m_eventParams.battleEquipPar.m_indexInQuicklyBuyList);
				if (widget2 != null)
				{
					widget2.CustomSetActive(true);
					CUIAnimationScript component2 = widget2.GetComponent<CUIAnimationScript>();
					if (component2 != null)
					{
						component2.PlayAnimation("Battle_UI_ZhuangBei_01", true);
					}
				}
			}
		}

		private void OnBattleEquipBoughtEffectPlayEnd(CUIEvent uiEvent)
		{
			uiEvent.m_srcWidget.CustomSetActive(false);
		}

		private void OnBattleLearnSkillButtonClick(CUIEvent uiEvent)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer == null)
			{
				return;
			}
			if (!hostPlayer.Captain || hostPlayer.Captain.handle.ActorControl == null)
			{
				return;
			}
			PoolObjHandle<ActorRoot> orignalActor = hostPlayer.Captain.handle.ActorControl.GetOrignalActor();
			if (!orignalActor)
			{
				return;
			}
			if (uiEvent == null || uiEvent.m_srcWidget == null || uiEvent.m_srcWidget.transform == null || uiEvent.m_srcWidget.transform.parent == null)
			{
				return;
			}
			string name = uiEvent.m_srcWidget.transform.parent.name;
			int num = int.Parse(name.Substring(name.get_Length() - 1));
			if (num < 1 || num > 3)
			{
				return;
			}
			byte bSkillLvl = 0;
			if (orignalActor.handle.SkillControl != null && orignalActor.handle.SkillControl.SkillSlotArray[num] != null)
			{
				bSkillLvl = (byte)orignalActor.handle.SkillControl.SkillSlotArray[num].GetSkillLevel();
			}
			this.SendLearnSkillCommand(orignalActor, (SkillSlotType)num, bSkillLvl);
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_junei_ani_jinengxuexi", null);
			Transform transform = uiEvent.m_srcWidget.transform.parent.Find("LearnEffect");
			if (transform != null)
			{
				transform.gameObject.CustomSetActive(true);
				CUIAnimationScript component = transform.gameObject.GetComponent<CUIAnimationScript>();
				if (component != null)
				{
					component.PlayAnimation("Battle_UI_Skill_01", true);
				}
			}
		}

		private void OnBattleSkillLevelUpEffectPlayEnd(CUIEvent uiEvent)
		{
			uiEvent.m_srcWidget.CustomSetActive(false);
		}

		private void SendLearnSkillCommand(PoolObjHandle<ActorRoot> actor, SkillSlotType enmSkillSlotType, byte bSkillLvl)
		{
			FrameCommand<LearnSkillCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<LearnSkillCommand>();
			frameCommand.cmdData.dwHeroID = actor.handle.ObjID;
			frameCommand.cmdData.bSkillLevel = bSkillLvl;
			frameCommand.cmdData.bSlotType = (byte)enmSkillSlotType;
			frameCommand.Send();
		}

		private void UpdateLearnSkillBtnState(int iSkillSlotType, bool bIsShow)
		{
			SkillButton button = this.GetButton((SkillSlotType)iSkillSlotType);
			if (button != null)
			{
				GameObject learnSkillButton = button.GetLearnSkillButton();
				if (learnSkillButton != null)
				{
					learnSkillButton.CustomSetActive(bIsShow);
					Singleton<EventRouter>.GetInstance().BroadCastEvent<int, bool>("HeroSkillLearnButtonStateChange", iSkillSlotType, bIsShow);
				}
			}
		}

		private void CheckAndUpdateLearnSkill(PoolObjHandle<ActorRoot> hero)
		{
			if (!hero)
			{
				return;
			}
			for (int i = 1; i <= 3; i++)
			{
				if (Singleton<BattleLogic>.GetInstance().IsMatchLearnSkillRule(hero, (SkillSlotType)i))
				{
					this.UpdateLearnSkillBtnState(i, true);
				}
				else
				{
					this.UpdateLearnSkillBtnState(i, false);
				}
			}
		}

		private void onHeroEnergyChange(PoolObjHandle<ActorRoot> actor, int curVal, int maxVal)
		{
			if (this._formScript == null)
			{
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer == null)
			{
				return;
			}
			PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
			if (actor && actor == captain)
			{
				for (int i = 1; i <= 3; i++)
				{
					SkillSlot skillSlot = actor.handle.SkillControl.SkillSlotArray[i];
					SkillButton button = this.GetButton((SkillSlotType)i);
					if (skillSlot != null && skillSlot.CanEnableSkillSlotByEnergy())
					{
						if (!skillSlot.IsEnergyEnough)
						{
							if (!button.bDisableFlag)
							{
								this.m_skillButtonManager.SetEnergyDisableButton((SkillSlotType)i);
							}
						}
						else if (button.bDisableFlag)
						{
							this.m_skillButtonManager.SetEnableButton((SkillSlotType)i);
						}
					}
				}
			}
			this.OnHeroEpChange(actor, curVal, maxVal);
		}

		private void onHeroEnergyMax(PoolObjHandle<ActorRoot> actor, int curVal, int maxVal)
		{
			if (this._formScript == null)
			{
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer == null)
			{
				return;
			}
			PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
			if (actor && actor == captain)
			{
				for (int i = 1; i <= 3; i++)
				{
					SkillSlot skillSlot = actor.handle.SkillControl.SkillSlotArray[i];
					SkillButton button = this.GetButton((SkillSlotType)i);
					if (skillSlot != null && skillSlot.CanEnableSkillSlotByEnergy() && button.bDisableFlag)
					{
						this.m_skillButtonManager.SetEnableButton((SkillSlotType)i);
					}
				}
			}
		}

		private void OnHeroSoulLvlChange(PoolObjHandle<ActorRoot> hero, int level)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer == null)
			{
				return;
			}
			PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
			if (!captain || !hero)
			{
				return;
			}
			if (hero.handle.ActorAgent != null && !hero.handle.ActorAgent.IsAutoAI() && captain.handle.ActorControl != null && captain.handle.ActorControl.GetOrignalActor() == hero)
			{
				this.CheckAndUpdateLearnSkill(hero);
			}
		}

		private void OnHeroSkillLvlup(PoolObjHandle<ActorRoot> hero, byte bSlotType, byte bSkillLevel)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer == null)
			{
				return;
			}
			PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
			if (captain && captain.handle.ActorControl != null && captain.handle.ActorControl.GetOrignalActor() == hero)
			{
				this.UpdateSkillLvlState((int)bSlotType, (int)bSkillLevel);
				this.CheckAndUpdateLearnSkill(hero);
				if (bSkillLevel == 1)
				{
					this.ResetSkillButtonManager(captain, true, (SkillSlotType)bSlotType);
				}
			}
		}

		private void OnCaptainSwitched(ref DefaultGameEventParam prm)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer == null)
			{
				return;
			}
			PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
			if (!captain || !prm.src)
			{
				return;
			}
			if (prm.src.handle.ActorAgent != null && !prm.src.handle.ActorAgent.IsAutoAI() && captain == prm.src)
			{
				this.CheckAndUpdateLearnSkill(prm.src);
			}
		}

		public void ClearSkillLvlStates(int iSkillSlotType)
		{
			SkillButton button = this.GetButton((SkillSlotType)iSkillSlotType);
			if (button != null)
			{
				GameObject skillLvlImg = button.GetSkillLvlImg(1);
				if (skillLvlImg != null)
				{
					ListView<GameObject> listView = new ListView<GameObject>();
					Transform parent = skillLvlImg.transform.parent;
					int num = parent.childCount;
					for (int i = 0; i < num; i++)
					{
						GameObject gameObject = parent.GetChild(i).gameObject;
						if (gameObject.name.Contains("SkillLvlImg") && gameObject.activeSelf)
						{
							listView.Add(gameObject);
						}
					}
					num = listView.Count;
					for (int j = 0; j < num; j++)
					{
						GameObject obj = listView[j];
						obj.CustomSetActive(false);
					}
					listView.Clear();
				}
			}
		}

		private void UpdateSkillLvlState(int iSkillSlotType, int iSkillLvl)
		{
			SkillButton button = this.GetButton((SkillSlotType)iSkillSlotType);
			if (button != null)
			{
				GameObject skillLvlImg = button.GetSkillLvlImg(iSkillLvl);
				if (skillLvlImg != null)
				{
					skillLvlImg.CustomSetActive(true);
				}
			}
		}

		public void OnAcceptTrusteeship(CUIEvent uiEvent)
		{
			this.SendTrusteeshipResult(uiEvent.m_eventParams.commonUInt32Param1, ACCEPT_AIPLAYER_RSP.ACCEPT_AIPLAEYR_YES);
		}

		public void OnCancelTrusteeship(CUIEvent uiEvent)
		{
			this.SendTrusteeshipResult(uiEvent.m_eventParams.commonUInt32Param1, ACCEPT_AIPLAYER_RSP.ACCEPT_AIPLAYER_NO);
		}

		public void SendTrusteeshipResult(uint objID, ACCEPT_AIPLAYER_RSP trusteeshipRsp)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1045u);
			cSPkg.stPkgData.stIsAcceptAiPlayerRsp.dwAiPlayerObjID = objID;
			cSPkg.stPkgData.stIsAcceptAiPlayerRsp.bResult = (byte)trusteeshipRsp;
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
		}

		private void OnBattleSkillDisableBtnDown(CUIEvent uiEvent)
		{
			this.OnBattleSkillDecShow(uiEvent);
		}

		private void OnBattleSkillDecShow(CUIEvent uiEvent)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain)
			{
				return;
			}
			string name = uiEvent.m_srcWidget.transform.parent.name;
			Vector3 position = uiEvent.m_srcWidget.transform.parent.transform.position;
			int num;
			if (!int.TryParse(name.Substring(name.get_Length() - 1), ref num))
			{
				name = uiEvent.m_srcWidget.transform.name;
				position = uiEvent.m_srcWidget.transform.position;
				if (!int.TryParse(name.Substring(name.get_Length() - 1), ref num))
				{
					return;
				}
			}
			SkillSlotType type = (SkillSlotType)num;
			SkillSlot skillSlot;
			if (hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(type, out skillSlot))
			{
				string text = Utility.UTF8Convert(skillSlot.SkillObj.cfgData.szSkillDesc);
				this.OnBattleHeroSkillTipOpen(skillSlot, position, (uint)hostPlayer.Captain.handle.TheActorMeta.ConfigId);
			}
		}

		private void OnBattleHeroSkillTipOpen(SkillSlot skillSlot, Vector3 Pos, uint heroId)
		{
			if (null == this._formScript)
			{
				return;
			}
			if (this.skillTipDesc == null)
			{
				this.skillTipDesc = Utility.FindChild(this._formScript.gameObject, "Panel_SkillTip");
				if (this.skillTipDesc == null)
				{
					return;
				}
			}
			if (skillSlot == null)
			{
				return;
			}
			PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
			if (!captain)
			{
				return;
			}
			IHeroData heroData = CHeroDataFactory.CreateHeroData((uint)captain.handle.TheActorMeta.ConfigId);
			Text component = this.skillTipDesc.transform.Find("skillNameText").GetComponent<Text>();
			component.set_text(StringHelper.UTF8BytesToString(ref skillSlot.SkillObj.cfgData.szSkillName));
			Text component2 = this.skillTipDesc.transform.Find("SkillDescribeText").GetComponent<Text>();
			ValueDataInfo[] actorValue = captain.handle.ValueComponent.mActorValue.GetActorValue();
			if (component2 != null && skillSlot.SkillObj.cfgData.szSkillDesc.get_Length() > 0)
			{
				component2.set_text(CUICommonSystem.GetSkillDesc(skillSlot.SkillObj.cfgData.szSkillDesc, actorValue, skillSlot.GetSkillLevel(), captain.handle.ValueComponent.actorSoulLevel, heroId));
			}
			Text component3 = this.skillTipDesc.transform.Find("SkillCDText").GetComponent<Text>();
			component3.set_text(Singleton<CTextManager>.instance.GetText("Skill_Cool_Down_Tips", new string[]
			{
				CUICommonSystem.ConvertMillisecondToSecondWithOneDecimal(skillSlot.GetSkillCDMax())
			}));
			Text component4 = component3.transform.Find("SkillEnergyCostText").GetComponent<Text>();
			component4.set_text((captain.handle.ValueComponent.mActorValue.EnergyType == EnergyType.BloodResource) ? string.Empty : Singleton<CTextManager>.instance.GetText(EnergyCommon.GetEnergyShowText((uint)captain.handle.ValueComponent.mActorValue.EnergyType, EnergyShowType.CostValue), new string[]
			{
				skillSlot.NextSkillEnergyCostTotal().ToString()
			}));
			ushort[] skillEffectType = skillSlot.SkillObj.cfgData.SkillEffectType;
			for (int i = 1; i <= 2; i++)
			{
				GameObject gameObject = component.transform.Find(string.Format("EffectNode{0}", i)).gameObject;
				if (i <= skillEffectType.Length && skillEffectType[i - 1] != 0)
				{
					gameObject.CustomSetActive(true);
					gameObject.GetComponent<Image>().SetSprite(CSkillData.GetEffectSlotBg((SkillEffectType)skillEffectType[i - 1]), this._formScript, true, false, false, false);
					gameObject.transform.Find("Text").GetComponent<Text>().set_text(CSkillData.GetEffectDesc((SkillEffectType)skillEffectType[i - 1]));
				}
				else
				{
					gameObject.CustomSetActive(false);
				}
			}
			Vector3 position = Pos;
			position.x -= 4f;
			position.y += 4f;
			this.skillTipDesc.transform.position = position;
			this.skillTipDesc.CustomSetActive(true);
			this.m_isSkillDecShow = true;
		}

		private void ShowSkillDescInfo(string strSkillDesc, Vector3 Pos)
		{
			if (this.BuffDesc == null)
			{
				this.BuffDesc = Utility.FindChild(this._formScript.gameObject, "SkillDesc");
				if (this.BuffDesc == null)
				{
					return;
				}
			}
			GameObject gameObject = Utility.FindChild(this.BuffDesc, "Text");
			if (gameObject == null)
			{
				return;
			}
			Text component = gameObject.GetComponent<Text>();
			component.set_text(strSkillDesc);
			float num = component.get_preferredHeight();
			Image componetInChild = Utility.GetComponetInChild<Image>(this.BuffDesc, "bg");
			if (componetInChild == null)
			{
				return;
			}
			Vector2 sizeDelta = componetInChild.get_rectTransform().sizeDelta;
			num += (componetInChild.gameObject.transform.localPosition.y - component.gameObject.transform.localPosition.y) * 2f + 10f;
			componetInChild.get_rectTransform().sizeDelta = new Vector2(sizeDelta.x, num);
			RectTransform component2 = this.BuffDesc.GetComponent<RectTransform>();
			if (component2 != null)
			{
				component2.sizeDelta = componetInChild.get_rectTransform().sizeDelta;
			}
			Vector3 position = Pos;
			position.x -= 4f;
			position.y += 4f;
			this.BuffDesc.transform.position = position;
			this.BuffDesc.CustomSetActive(true);
			this.m_isSkillDecShow = true;
		}

		public void HideSkillDescInfo()
		{
			if (this.skillTipDesc == null)
			{
				this.skillTipDesc = this._formSkillBtnScript.GetWidget(26);
			}
			if (this.skillTipDesc != null)
			{
				this.skillTipDesc.CustomSetActive(false);
				this.m_isSkillDecShow = false;
			}
		}

		private bool IsSkillTipsActive()
		{
			if (this.skillTipDesc == null && this._formScript != null)
			{
				this.skillTipDesc = this._formSkillBtnScript.GetWidget(26);
			}
			return this.skillTipDesc != null && this.skillTipDesc.activeSelf;
		}

		private void OnBattleSkillDisableBtnUp(CUIEvent uiEvent)
		{
			this.HideSkillDescInfo();
		}

		private void OnBattleSkillBtnHold(CUIEvent uiEvent)
		{
			if (!this.m_isSkillDecShow && !this.m_skillButtonManager.CurrentSkillTipsResponed)
			{
				this.OnBattleSkillDecShow(uiEvent);
			}
			else if (this.m_isSkillDecShow && this.m_skillButtonManager.CurrentSkillTipsResponed)
			{
				this.HideSkillDescInfo();
			}
		}

		public void ChangeSpeakerBtnState()
		{
			if (this.m_isInBattle && this.m_bOpenSpeak != GameSettings.EnableVoice)
			{
				this.OnBattleOpenSpeaker(null);
			}
		}

		private void BattleOpenSpeak(CUIEvent uiEvent, bool bInit = false)
		{
			if (uiEvent != null)
			{
				CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_Voice_OpenSpeak);
			}
			if (this.m_OpenSpeakerTipObj)
			{
				if (!CFakePvPHelper.bInFakeSelect && MonoSingleton<VoiceSys>.GetInstance().IsBattleSupportVoice())
				{
					if (!MonoSingleton<VoiceSys>.GetInstance().GlobalVoiceSetting)
					{
						if (!bInit)
						{
							if (this.m_OpenSpeakerTipText)
							{
								this.m_OpenSpeakerTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Server_Not_Open_Tips);
							}
							this.m_OpenSpeakerTipObj.gameObject.CustomSetActive(true);
							Singleton<CTimerManager>.instance.ResumeTimer(this.m_Vocetimer);
						}
						return;
					}
					if (!MonoSingleton<VoiceSys>.GetInstance().IsInVoiceRoom())
					{
						if (this.m_OpenSpeakerTipText)
						{
							this.m_OpenSpeakerTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Cannot_JoinVoiceRoom);
						}
						this.m_OpenSpeakerTipObj.gameObject.CustomSetActive(true);
						Singleton<CTimerManager>.instance.ResumeTimer(this.m_Vocetimer);
						return;
					}
				}
				if (this.m_bOpenSpeak)
				{
					if (this.m_OpenSpeakerTipText)
					{
						this.m_OpenSpeakerTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_CloseSpeaker);
					}
					MonoSingleton<VoiceSys>.GetInstance().ClosenSpeakers();
					MonoSingleton<VoiceSys>.GetInstance().CloseMic();
					this.m_bOpenMic = false;
					if (this.m_OpenSpeakerObj)
					{
						CUIUtility.SetImageSprite(this.m_OpenSpeakerObj.GetComponent<Image>(), this.no_voiceIcon_path, null, true, false, false, false);
					}
					if (this.m_OpenMicObj)
					{
						CUIUtility.SetImageSprite(this.m_OpenMicObj.GetComponent<Image>(), this.no_microphone_path, null, true, false, false, false);
					}
				}
				else
				{
					if (this.m_OpenSpeakerTipText)
					{
						this.m_OpenSpeakerTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_OpenSpeaker);
					}
					MonoSingleton<VoiceSys>.GetInstance().OpenSpeakers();
					if (this.m_OpenSpeakerObj)
					{
						CUIUtility.SetImageSprite(this.m_OpenSpeakerObj.GetComponent<Image>(), this.voiceIcon_path, null, true, false, false, false);
					}
				}
				this.m_bOpenSpeak = !this.m_bOpenSpeak;
				if (this.m_bOpenSpeak)
				{
					if (!GameSettings.EnableVoice)
					{
						GameSettings.EnableVoice = true;
					}
					if (bInit)
					{
						if (MonoSingleton<VoiceSys>.GetInstance().UseMicOnUser)
						{
							this.OnBattleOpenMic(null);
						}
					}
					else
					{
						this.OnBattleOpenMic(null);
					}
				}
				else if (GameSettings.EnableVoice)
				{
					GameSettings.EnableVoice = false;
				}
				this.m_OpenSpeakerTipObj.gameObject.CustomSetActive(true);
				Singleton<CTimerManager>.instance.ResumeTimer(this.m_Vocetimer);
			}
			if (this.m_OpeankSpeakAnim)
			{
				this.m_OpeankSpeakAnim.gameObject.CustomSetActive(false);
			}
		}

		private void OnBattleOpenSpeaker(CUIEvent uiEvent)
		{
			this.BattleOpenSpeak(uiEvent, false);
		}

		private void OnBattleOpenMic(CUIEvent uiEvent)
		{
			bool flag = true;
			if (uiEvent == null)
			{
				flag = false;
			}
			if (flag)
			{
				CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_Voice_OpenMic);
			}
			if (this.m_OpenMicTipObj)
			{
				if (!this.m_bOpenSpeak)
				{
					if (this.m_OpenMicTipText)
					{
						this.m_OpenMicTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_FIrstOPenSpeak);
					}
					if (flag)
					{
						this.m_OpenMicTipObj.gameObject.CustomSetActive(true);
					}
					Singleton<CTimerManager>.instance.ResumeTimer(this.m_VoiceMictime);
					return;
				}
				if (!CFakePvPHelper.bInFakeSelect && MonoSingleton<VoiceSys>.GetInstance().IsBattleSupportVoice())
				{
					if (!MonoSingleton<VoiceSys>.GetInstance().GlobalVoiceSetting)
					{
						if (this.m_OpenMicTipText)
						{
							this.m_OpenMicTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Server_Not_Open_Tips);
						}
						if (flag)
						{
							this.m_OpenMicTipObj.gameObject.CustomSetActive(true);
						}
						Singleton<CTimerManager>.instance.ResumeTimer(this.m_VoiceMictime);
						return;
					}
					if (!MonoSingleton<VoiceSys>.GetInstance().IsInVoiceRoom())
					{
						if (this.m_OpenMicTipText)
						{
							this.m_OpenMicTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Cannot_JoinVoiceRoom);
						}
						if (flag)
						{
							this.m_OpenMicTipObj.gameObject.CustomSetActive(true);
						}
						Singleton<CTimerManager>.instance.ResumeTimer(this.m_VoiceMictime);
						return;
					}
				}
				if (this.m_bOpenMic)
				{
					if (this.m_OpenMicTipText)
					{
						this.m_OpenMicTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_CloseMic);
					}
					MonoSingleton<VoiceSys>.GetInstance().CloseMic();
					if (this.m_OpenMicObj)
					{
						CUIUtility.SetImageSprite(this.m_OpenMicObj.GetComponent<Image>(), this.no_microphone_path, null, true, false, false, false);
					}
				}
				else
				{
					if (this.m_OpenMicTipText)
					{
						this.m_OpenMicTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_OpenMic);
					}
					MonoSingleton<VoiceSys>.GetInstance().OpenMic();
					if (this.m_OpenMicObj)
					{
						CUIUtility.SetImageSprite(this.m_OpenMicObj.GetComponent<Image>(), this.microphone_path, null, true, false, false, false);
					}
				}
				this.m_bOpenMic = !this.m_bOpenMic;
				if (flag)
				{
					this.m_OpenMicTipObj.gameObject.CustomSetActive(true);
				}
				Singleton<CTimerManager>.instance.ResumeTimer(this.m_VoiceMictime);
			}
		}

		private void onMultiHashNotSync(CUIEvent uiEvent)
		{
			ConnectorParam connectionParam = Singleton<NetworkModule>.instance.gameSvr.GetConnectionParam();
			Singleton<GameBuilder>.instance.EndGame();
			if (connectionParam != null)
			{
				Singleton<NetworkModule>.instance.InitGameServerConnect(connectionParam);
			}
		}

		private void OnVoiceTimeEndFirst(int timersequence)
		{
			if (this.m_OpenSpeakerTipObj)
			{
				Singleton<CTimerManager>.instance.PauseTimer(this.m_Vocetimer);
				Singleton<CTimerManager>.instance.ResetTimer(this.m_Vocetimer);
				this.m_OpenSpeakerTipObj.gameObject.CustomSetActive(false);
			}
			if (this.m_OpeankSpeakAnim)
			{
				this.m_OpeankSpeakAnim.gameObject.CustomSetActive(false);
			}
		}

		private void OnVoiceTimeEnd(int timersequence)
		{
			if (this.m_OpenSpeakerTipObj)
			{
				Singleton<CTimerManager>.instance.PauseTimer(this.m_Vocetimer);
				Singleton<CTimerManager>.instance.ResetTimer(this.m_Vocetimer);
				this.m_OpenSpeakerTipObj.gameObject.CustomSetActive(false);
			}
		}

		private void OnVoiceMicTimeEnd(int timersequence)
		{
			if (this.m_OpenMicTipObj)
			{
				Singleton<CTimerManager>.instance.PauseTimer(this.m_VoiceMictime);
				Singleton<CTimerManager>.instance.ResetTimer(this.m_VoiceMictime);
				this.m_OpenMicTipObj.gameObject.CustomSetActive(false);
			}
		}

		public static void Preload(ref ActorPreloadTab preloadTab)
		{
			preloadTab.AddMesh(CUIParticleSystem.s_particleSkillBtnEffect_Path);
		}

		private void OnBattleAtkSelectSoldierBtnDown(CUIEvent uiEvent)
		{
			SkillButton button = this.GetButton(SkillSlotType.SLOT_SKILL_0);
			if (button == null || button.bDisableFlag)
			{
				return;
			}
			Singleton<LockModeKeySelector>.GetInstance().OnHandleClickSelectTargetBtn(AttackTargetType.ATTACK_TARGET_SOLDIER);
			Singleton<CUIParticleSystem>.GetInstance().AddParticle(CUIParticleSystem.s_particleSkillBtnEffect_Path, 0.5f, uiEvent.m_srcWidget, uiEvent.m_srcFormScript, default(Quaternion?));
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_signal_Change_xiaobing", null);
		}

		private void OnBattleAtkSelectHeroBtnDown(CUIEvent uiEvent)
		{
			SkillButton button = this.GetButton(SkillSlotType.SLOT_SKILL_0);
			if (button == null || button.bDisableFlag)
			{
				return;
			}
			Singleton<LockModeKeySelector>.GetInstance().OnHandleClickSelectTargetBtn(AttackTargetType.ATTACK_TARGET_HERO);
			Singleton<CUIParticleSystem>.GetInstance().AddParticle(CUIParticleSystem.s_particleSkillBtnEffect_Path, 0.5f, uiEvent.m_srcWidget, uiEvent.m_srcFormScript, default(Quaternion?));
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_signal_Change_hero", null);
		}

		public void SetCommonAttackTargetEvent()
		{
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_SelectTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnSelectTarget));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_ClearTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnClearTarget));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<LockTargetEventParam>(GameSkillEventDef.Event_LockTarget, new GameSkillEvent<LockTargetEventParam>(this.OnLockTarget));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<LockTargetEventParam>(GameSkillEventDef.Event_ClearLockTarget, new GameSkillEvent<LockTargetEventParam>(this.OnClearLockTarget));
		}

		private void UnRegisteredCommonAttackTargetEvent()
		{
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_SelectTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnSelectTarget));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_ClearTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnClearTarget));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<LockTargetEventParam>(GameSkillEventDef.Event_LockTarget, new GameSkillEvent<LockTargetEventParam>(this.OnLockTarget));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<LockTargetEventParam>(GameSkillEventDef.Event_ClearLockTarget, new GameSkillEvent<LockTargetEventParam>(this.OnClearLockTarget));
		}

		private void OnGameSettingCommonAttackTypeChange(CommonAttactType byAtkType)
		{
			if (!Singleton<BattleLogic>.instance.isRuning)
			{
				return;
			}
			this.m_skillButtonManager.SetCommonAtkBtnState(byAtkType);
			this.m_skillButtonManager.SetAdvancedAttackBtnState(byAtkType);
		}

		private void OnClickBattleScene(CUIEvent uievent)
		{
			Singleton<LockModeScreenSelector>.GetInstance().OnClickBattleScene(uievent.m_pointerEventData.get_position());
			Singleton<TeleportTargetSelector>.GetInstance().OnClickBattleScene(uievent.m_pointerEventData.get_position());
		}

		public void ShowHeroInfoPanel(PoolObjHandle<ActorRoot> hero)
		{
			if (this.m_panelHeroInfo == null)
			{
				this.m_panelHeroInfo = Utility.FindChild(this._formScript.gameObject, "PanelHeroInfo");
				if (this.m_panelHeroInfo != null)
				{
					this.m_panelHeroInfo.CustomSetActive(true);
				}
			}
			if (this.m_panelHeroCanvas == null)
			{
				this.m_panelHeroCanvas = this.m_panelHeroInfo.GetComponent<CanvasGroup>();
			}
			if (this.m_panelHeroInfo == null || this.m_panelHeroCanvas == null || !hero || hero.handle.ValueComponent == null)
			{
				return;
			}
			this.m_selectedHero = hero;
			this.SetSelectedHeroInfo(hero);
			this.m_panelHeroCanvas.alpha = 1f;
		}

		private void OnHeroHpChange(PoolObjHandle<ActorRoot> hero, int iCurVal, int iMaxVal)
		{
			if (iCurVal <= 0 && Singleton<GamePlayerCenter>.GetInstance().HostPlayerId != 0u && hero == Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain)
			{
				this.HideHeroInfoPanel(false);
			}
			if (!this.m_selectedHero || hero != this.m_selectedHero)
			{
				return;
			}
			this.UpdateHpInfo();
		}

		public void UpdateHpInfo()
		{
			if (!this.m_selectedHero)
			{
				return;
			}
			int actorHp = this.m_selectedHero.handle.ValueComponent.actorHp;
			int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
			if (this.m_HpImg == null)
			{
				GameObject gameObject = Utility.FindChild(this.m_panelHeroInfo, "HpImg");
				if (gameObject != null)
				{
					this.m_HpImg = gameObject.GetComponent<Image>();
				}
			}
			if (this.m_HpImg != null)
			{
				this.m_HpImg.CustomFillAmount((float)actorHp / (float)totalValue);
			}
			if (this.m_HpTxt == null)
			{
				GameObject gameObject2 = Utility.FindChild(this.m_panelHeroInfo, "HpTxt");
				if (gameObject2 != null)
				{
					this.m_HpTxt = gameObject2.GetComponent<Text>();
				}
			}
			if (this.m_HpTxt != null)
			{
				string text = string.Format("{0}/{1}", actorHp, totalValue);
				this.m_HpTxt.set_text(text);
			}
		}

		private void OnHeroEpChange(PoolObjHandle<ActorRoot> hero, int iCurVal, int iMaxVal)
		{
			if (!this.m_selectedHero || hero != this.m_selectedHero)
			{
				return;
			}
			this.UpdateEpInfo();
		}

		public void UpdateEpInfo()
		{
			if (!this.m_selectedHero)
			{
				return;
			}
			int actorEp = this.m_selectedHero.handle.ValueComponent.actorEp;
			int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue;
			if (this.m_EpImg == null)
			{
				GameObject gameObject = Utility.FindChild(this.m_panelHeroInfo, "EpImg");
				if (gameObject != null)
				{
					this.m_EpImg = gameObject.GetComponent<Image>();
				}
			}
			if (this.m_EpImg != null)
			{
				float value = 0f;
				if (totalValue > 0)
				{
					value = (float)actorEp / (float)totalValue;
				}
				this.m_EpImg.CustomFillAmount(value);
			}
		}

		public void UpdateAdValueInfo()
		{
			if (!this.m_selectedHero)
			{
				return;
			}
			int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
			if (this.m_AdTxt == null)
			{
				GameObject gameObject = Utility.FindChild(this.m_panelHeroInfo, "InfoPanel/TxtPanel/AdTxt");
				if (gameObject != null)
				{
					this.m_AdTxt = gameObject.GetComponent<Text>();
				}
			}
			if (this.m_AdTxt != null)
			{
				if (totalValue >= 1000)
				{
					this.m_AdTxt.set_text(Math.Round((double)totalValue / 1000.0, 2) + "k");
				}
				else
				{
					this.m_AdTxt.set_text(totalValue.ToString());
				}
			}
		}

		public void UpdateApValueInfo()
		{
			if (!this.m_selectedHero)
			{
				return;
			}
			int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].totalValue;
			if (this.m_ApTxt == null)
			{
				GameObject gameObject = Utility.FindChild(this.m_panelHeroInfo, "InfoPanel/TxtPanel/ApTxt");
				if (gameObject != null)
				{
					this.m_ApTxt = gameObject.GetComponent<Text>();
				}
			}
			if (this.m_ApTxt != null)
			{
				if (totalValue >= 1000)
				{
					this.m_ApTxt.set_text(Math.Round((double)totalValue / 1000.0, 2) + "k");
				}
				else
				{
					this.m_ApTxt.set_text(totalValue.ToString());
				}
			}
		}

		public void UpdatePhyDefValueInfo()
		{
			if (!this.m_selectedHero)
			{
				return;
			}
			int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].totalValue;
			if (this.m_PhyDefTxt == null)
			{
				GameObject gameObject = Utility.FindChild(this.m_panelHeroInfo, "InfoPanel/TxtPanel/PhyDefTxt");
				if (gameObject != null)
				{
					this.m_PhyDefTxt = gameObject.GetComponent<Text>();
				}
			}
			if (this.m_PhyDefTxt != null)
			{
				if (totalValue >= 1000)
				{
					this.m_PhyDefTxt.set_text(Math.Round((double)totalValue / 1000.0, 2) + "k");
				}
				else
				{
					this.m_PhyDefTxt.set_text(totalValue.ToString());
				}
			}
		}

		public void UpdateMgcDefValueInfo()
		{
			if (!this.m_selectedHero)
			{
				return;
			}
			int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT].totalValue;
			GameObject gameObject = Utility.FindChild(this.m_panelHeroInfo, "InfoPanel/TxtPanel/MgcDefTxt");
			if (gameObject != null)
			{
				this.m_MgcDefTxt = gameObject.GetComponent<Text>();
			}
			if (this.m_MgcDefTxt != null)
			{
				if (totalValue >= 1000)
				{
					this.m_MgcDefTxt.set_text(Math.Round((double)totalValue / 1000.0, 2) + "k");
				}
				else
				{
					this.m_MgcDefTxt.set_text(totalValue.ToString());
				}
			}
		}

		private void SetSelectedHeroInfo(PoolObjHandle<ActorRoot> hero)
		{
			if (this.m_panelHeroInfo == null)
			{
				this.m_panelHeroInfo = Utility.FindChild(this._formScript.gameObject, "PanelHeroInfo");
			}
			if (this.m_panelHeroInfo == null || !hero)
			{
				return;
			}
			if (this.m_objHeroHead == null)
			{
				this.m_objHeroHead = Utility.FindChild(this.m_panelHeroInfo, "HeroHead");
			}
			if (this.m_objHeroHead != null)
			{
				uint configId = (uint)hero.handle.TheActorMeta.ConfigId;
				KillInfo killInfo = KillNotifyUT.Convert_DetailInfo_KillInfo(new KillDetailInfo
				{
					Killer = hero
				});
				Image component = this.m_objHeroHead.transform.Find("imageIcon").GetComponent<Image>();
				component.SetSprite(killInfo.KillerImgSrc, this._formScript, true, false, false, false);
			}
			Singleton<CBattleSelectTarget>.GetInstance().OpenForm(hero);
		}

		public void HideHeroInfoPanel(bool bIsFromSetting = false)
		{
			if (this.m_panelHeroInfo == null)
			{
				if (this._formScript == null)
				{
					return;
				}
				this.m_panelHeroInfo = Utility.FindChild(this._formScript.gameObject, "PanelHeroInfo");
				if (this.m_panelHeroInfo == null)
				{
					return;
				}
			}
			if (this.m_panelHeroCanvas == null)
			{
				this.m_panelHeroCanvas = this.m_panelHeroInfo.GetComponent<CanvasGroup>();
			}
			if (this.m_panelHeroCanvas != null)
			{
				this.m_panelHeroCanvas.alpha = 0f;
			}
			if (Singleton<CBattleSelectTarget>.GetInstance() != null)
			{
				Singleton<CBattleSelectTarget>.GetInstance().CloseForm();
			}
			if (this.m_selectedHero)
			{
				if (bIsFromSetting)
				{
					this.m_HideSelectedHero = this.m_selectedHero;
				}
				this.m_selectedHero.Release();
			}
		}

		private void ShowTargetInfoByTargetId(uint uilockTargetID)
		{
			if (Singleton<GameObjMgr>.instance == null)
			{
				return;
			}
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.instance.GetActor(uilockTargetID);
			if (!actor)
			{
				return;
			}
			this.ShowHeroInfoPanel(actor);
		}

		private void OnLockTarget(ref LockTargetEventParam prm)
		{
			if (!GameSettings.EnableHeroInfo)
			{
				this.m_HideSelectedHero = Singleton<GameObjMgr>.instance.GetActor(prm.lockTargetID);
				return;
			}
			if (Singleton<GamePlayerCenter>.GetInstance().HostPlayerId == 0u)
			{
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null)
			{
				return;
			}
			OperateMode operateMode = hostPlayer.GetOperateMode();
			if (operateMode == OperateMode.LockMode)
			{
				this.ShowTargetInfoByTargetId(prm.lockTargetID);
			}
		}

		private void OnClearLockTarget(ref LockTargetEventParam prm)
		{
			this.HideHeroInfoPanel(false);
		}

		private void OnSelectTarget(ref SelectTargetEventParam prm)
		{
			if (!GameSettings.EnableHeroInfo)
			{
				this.m_HideSelectedHero = Singleton<GameObjMgr>.instance.GetActor(prm.commonAttackTargetID);
				return;
			}
			if (Singleton<GamePlayerCenter>.GetInstance().HostPlayerId == 0u)
			{
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null)
			{
				return;
			}
			if (hostPlayer.GetOperateMode() == OperateMode.DefaultMode)
			{
				this.ShowTargetInfoByTargetId(prm.commonAttackTargetID);
			}
		}

		private void OnClearTarget(ref SelectTargetEventParam prm)
		{
			this.HideHeroInfoPanel(false);
		}

		public void OpenHeroInfoPanel()
		{
			if (this.m_HideSelectedHero)
			{
				this.ShowHeroInfoPanel(this.m_HideSelectedHero);
			}
		}

		public GameObject GetSkillCursor(enSkillJoystickMode mode)
		{
			if (this._formSkillCursorScript == null)
			{
				return null;
			}
			if (mode == enSkillJoystickMode.General)
			{
				return this._formSkillCursorScript.GetWidget(0);
			}
			if (mode == enSkillJoystickMode.SelectTarget5)
			{
				return this._formSkillCursorScript.GetWidget(4);
			}
			return this._formSkillCursorScript.GetWidget(1);
		}

		public GameObject GetSkillCancleArea()
		{
			if (this._formSkillCursorScript == null)
			{
				return null;
			}
			return this._formSkillCursorScript.GetWidget(2);
		}

		public GameObject GetEquipSkillCancleArea()
		{
			if (this._formSkillCursorScript == null)
			{
				return null;
			}
			return this._formSkillCursorScript.GetWidget(3);
		}

		public GameObject GetMoveJoystick()
		{
			if (this._formJoystickScript == null)
			{
				return null;
			}
			return this._formJoystickScript.GetWidget(0);
		}

		public CUIFormScript GetJoystickFormScript()
		{
			return this._formJoystickScript;
		}

		public CUIFormScript GetSkillCursorFormScript()
		{
			return this._formSkillCursorScript;
		}

		public CUIFormScript GetSkillBtnFormScript()
		{
			return this._formSkillBtnScript;
		}

		public GameObject GetBigMapDragonContainer()
		{
			return this._formScript.GetWidget(60);
		}

		public void ResetFightFormUIShowType()
		{
			if (!GameSettings.IsOpenFightFormUiTypeSwitchFunc())
			{
				return;
			}
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			CUIFormScript formScript = this._formScript;
			if (curLvelContext == null || !curLvelContext.IsMobaMode() || formScript == null)
			{
				return;
			}
			CUIAnimatorScript component = formScript.GetComponent<CUIAnimatorScript>();
			Animator component2 = formScript.GetComponent<Animator>();
			if (component != null)
			{
				component.enabled = true;
			}
			if (component2 != null)
			{
				component2.enabled = true;
			}
			EquipPosType equipPosMode = GameSettings.EquipPosMode;
			string stateName = "type1";
			if (equipPosMode == EquipPosType.EquipPosLeftMode)
			{
				stateName = "type1";
			}
			else if (equipPosMode == EquipPosType.EquipPosRightMode)
			{
				stateName = "type2";
			}
			component.PlayAnimator(stateName);
			TextAnchor alignment = TextAnchor.MiddleLeft;
			if (equipPosMode == EquipPosType.EquipPosRightMode)
			{
				alignment = TextAnchor.MiddleRight;
			}
			for (int i = 0; i < 2; i++)
			{
				GameObject widget = this._formScript.GetWidget(47 + i);
				if (widget == null)
				{
					return;
				}
				Transform transform = widget.transform.Find("Panel_Info/descText");
				if (transform == null)
				{
					return;
				}
				Text component3 = transform.GetComponent<Text>();
				if (component3 == null)
				{
					return;
				}
				component3.set_alignment(alignment);
			}
		}

		private void OnUITypeChangeComplete(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			CUIAnimatorScript component = srcFormScript.GetComponent<CUIAnimatorScript>();
			Animator component2 = srcFormScript.GetComponent<Animator>();
			if (component != null)
			{
				component.enabled = false;
			}
			if (component2 != null)
			{
				component2.enabled = false;
			}
			srcFormScript.m_sgameGraphicRaycaster.RefreshGameObject(srcFormScript.gameObject);
		}
	}
}
