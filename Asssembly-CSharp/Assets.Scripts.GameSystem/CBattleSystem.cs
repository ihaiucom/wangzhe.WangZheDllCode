using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CBattleSystem : Singleton<CBattleSystem>
	{
		public enum FormType
		{
			None,
			Fight,
			Watch
		}

		private IBattleForm _battleForm;

		private CBattleFloatDigitManager m_battleFloatDigitManager;

		public CBattleEquipSystem m_battleEquipSystem;

		private TowerHitMgr m_towerHitMgr;

		private BattleStatView _battleStatView;

		private MinimapSys _miniMapSys;

		private KillNotify _killNotify;

		private InBattle3DTouch _battle3DTouch;

		public Vector2 world_UI_Factor_Small;

		public Vector2 UI_world_Factor_Small;

		public Vector2 world_UI_Factor_Big;

		public Vector2 UI_world_Factor_Big;

		public PauseControl pauseControl
		{
			get;
			private set;
		}

		public bool IsFormOpen
		{
			get
			{
				return null != this._battleForm;
			}
		}

		public CUIFormScript FormScript
		{
			get
			{
				return (this._battleForm != null) ? this._battleForm.FormScript : null;
			}
		}

		public FightForm FightForm
		{
			get
			{
				return this._battleForm as FightForm;
			}
		}

		public CUIFormScript FightFormScript
		{
			get
			{
				return (this.FightForm != null) ? this.FightForm.FormScript : null;
			}
		}

		public WatchForm WatchForm
		{
			get
			{
				return this._battleForm as WatchForm;
			}
		}

		public CUIFormScript WatchFormScript
		{
			get
			{
				return (this.WatchForm != null) ? this.WatchForm.FormScript : null;
			}
		}

		public MinimapSys TheMinimapSys
		{
			get
			{
				return this._miniMapSys;
			}
		}

		public KillNotify TheKillNotify
		{
			get
			{
				return this._killNotify;
			}
		}

		public BattleStatView BattleStatView
		{
			get
			{
				return this._battleStatView;
			}
		}

		public TowerHitMgr TowerHitMgr
		{
			get
			{
				return this.m_towerHitMgr;
			}
		}

		public SignalPanel TheSignalPanel
		{
			get
			{
				return (this.FightForm != null) ? this.FightForm.GetSignalPanel() : null;
			}
		}

		public CUIContainerScript TextHudContainer
		{
			get
			{
				return (this.FightForm != null) ? this.FightForm.TextHudContainer : null;
			}
		}

		public override void Init()
		{
			this.m_battleFloatDigitManager = new CBattleFloatDigitManager();
			this.m_battleEquipSystem = new CBattleEquipSystem();
		}

		public override void UnInit()
		{
		}

		private void RegisterEvents()
		{
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<SpawnBuffEventParam>(GameSkillEventDef.Event_SpawnBuff, new GameSkillEvent<SpawnBuffEventParam>(this.OnPlayerSpawnBuff));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnFloatTextAnimEnd, new CUIEventManager.OnUIEventHandler(this.OnFloatTextAnimEnd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_Form_Open, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipFormOpen));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_Form_Close, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipFormClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_TypeList_Select, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipTypeListSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_Item_Select, new CUIEventManager.OnUIEventHandler(this.OnBattleEuipItemSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_BagItem_Select, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBagItemSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_BuyBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBuy));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_SaleBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipSale));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_RecommendEquip_Buy, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipQuicklyBuy));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_OpenEquipTree, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipOpenEquipTree));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_CloseEquipTree, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipCloseEquipTree));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_SelectItemInEquipTree, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipSelectItemInEquipTree));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_BackEquipListSelectedChanged, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBackEquipListSelectedChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_BackEquipListScrollChanged, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBackEquipListScrollChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_BackEquipListMoveLeft, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBackEquipListMoreLeftClicked));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_BackEquipListMoveRight, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBackEquipListMoreRightClicked));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_ChangeSelfRcmEuipPlan, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipChangeSelfRcmEuipPlan));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_OpenSelfRcmEuipPlanForm, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipOpenSelfRcmEuipPlanForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ToggleStatView, new CUIEventManager.OnUIEventHandler(this.OnToggleStatView));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.GAME_SETTING_SHOWEQUIPINFO_CHANGE, new Action(this.OnSettingShowEquipInfoChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this.OnActorGoldCoinInBattleChanged));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>(EventID.BATTLE_DATAANL_DRAGON_KILLED, new Action<PoolObjHandle<ActorRoot>, int>(this.OnDragonKilled));
		}

		private void UnregisterEvents()
		{
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<SpawnBuffEventParam>(GameSkillEventDef.Event_SpawnBuff, new GameSkillEvent<SpawnBuffEventParam>(this.OnPlayerSpawnBuff));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnFloatTextAnimEnd, new CUIEventManager.OnUIEventHandler(this.OnFloatTextAnimEnd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_Form_Open, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipFormOpen));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_Form_Close, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipFormClose));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_TypeList_Select, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipTypeListSelect));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_Item_Select, new CUIEventManager.OnUIEventHandler(this.OnBattleEuipItemSelect));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_BagItem_Select, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBagItemSelect));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_BuyBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBuy));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_SaleBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipSale));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_RecommendEquip_Buy, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipQuicklyBuy));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_OpenEquipTree, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipOpenEquipTree));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_CloseEquipTree, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipCloseEquipTree));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_SelectItemInEquipTree, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipSelectItemInEquipTree));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_BackEquipListSelectedChanged, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBackEquipListSelectedChanged));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_BackEquipListScrollChanged, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBackEquipListScrollChanged));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_BackEquipListMoveLeft, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBackEquipListMoreLeftClicked));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_BackEquipListMoveRight, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBackEquipListMoreRightClicked));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ToggleStatView, new CUIEventManager.OnUIEventHandler(this.OnToggleStatView));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_ChangeSelfRcmEuipPlan, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipChangeSelfRcmEuipPlan));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_OpenSelfRcmEuipPlanForm, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipOpenSelfRcmEuipPlanForm));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.GAME_SETTING_SHOWEQUIPINFO_CHANGE, new Action(this.OnSettingShowEquipInfoChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this.OnActorGoldCoinInBattleChanged));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>(EventID.BATTLE_DATAANL_DRAGON_KILLED, new Action<PoolObjHandle<ActorRoot>, int>(this.OnDragonKilled));
		}

		public CUIFormScript LoadForm(CBattleSystem.FormType formType)
		{
			if (formType == CBattleSystem.FormType.Fight)
			{
				return Singleton<CUIManager>.GetInstance().OpenForm(FightForm.s_battleUIForm, false, true);
			}
			if (formType == CBattleSystem.FormType.Watch)
			{
				if (Singleton<WatchController>.GetInstance().CanShowActorIRPosMap())
				{
					Singleton<CBattleStatCompetitionSystem>.GetInstance().PreLoadForm();
				}
				return Singleton<CUIManager>.GetInstance().OpenForm(WatchForm.GetWatchFormName(), false, true);
			}
			return null;
		}

		public void OpenForm(CBattleSystem.FormType formType)
		{
			if (formType == CBattleSystem.FormType.Fight)
			{
				this._battle3DTouch = new InBattle3DTouch();
				this._battle3DTouch.Init();
				this._battleForm = new FightForm();
			}
			else if (formType == CBattleSystem.FormType.Watch)
			{
				this._battleForm = new WatchForm();
			}
			if (this._battleForm == null || !this._battleForm.OpenForm())
			{
				this._battleForm = null;
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			this.m_battleEquipSystem.Initialize(this.FightFormScript, hostPlayer.Captain, curLvelContext.IsMobaMode(), curLvelContext.m_isBattleEquipLimit);
			this._miniMapSys = new MinimapSys();
			this._miniMapSys.Init(this.FormScript, curLvelContext);
			this._killNotify = new KillNotify();
			this._killNotify.Init();
			this._killNotify.Hide();
			this.m_towerHitMgr = new TowerHitMgr();
			this.m_towerHitMgr.Init();
			if (curLvelContext.IsMobaMode())
			{
				this._battleStatView = new BattleStatView();
				this._battleStatView.Init();
			}
			this.pauseControl = new PauseControl(this.FormScript);
			this.RegisterEvents();
		}

		public void BattleStart()
		{
			if (this._battleForm != null)
			{
				this._battleForm.BattleStart();
			}
		}

		public void CloseForm()
		{
			if (this._battleForm != null)
			{
				this._battleForm.CloseForm();
			}
		}

		public void OnFormClosed()
		{
			this.UnregisterEvents();
			this.m_battleFloatDigitManager.ClearAllBattleFloatText();
			this.m_battleEquipSystem.Clear();
			if (this._miniMapSys != null)
			{
				this._miniMapSys.Clear();
				this._miniMapSys = null;
			}
			if (this._killNotify != null)
			{
				this._killNotify.Clear();
				this._killNotify = null;
			}
			if (this.m_towerHitMgr != null)
			{
				this.m_towerHitMgr.Clear();
				this.m_towerHitMgr = null;
			}
			if (this._battleStatView != null)
			{
				this._battleStatView.Clear();
				this._battleStatView = null;
			}
			if (this.pauseControl != null)
			{
				this.pauseControl.Clear();
				this.pauseControl = null;
			}
			if (this._battle3DTouch != null)
			{
				this._battle3DTouch.Clear();
				this._battle3DTouch = null;
			}
			this._battleForm = null;
		}

		public void UpdateLogic(int delta)
		{
			if (this.IsFormOpen && this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.UpdateLogic(delta);
			}
			if (this._battleForm != null)
			{
				this._battleForm.UpdateLogic(delta);
			}
		}

		public void Update()
		{
			if (this.IsFormOpen && this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.Update();
			}
			if (this._battleForm != null)
			{
				this._battleForm.Update();
			}
		}

		public void LateUpdate()
		{
			if (this.IsFormOpen)
			{
				this.m_battleFloatDigitManager.LateUpdate();
				if (this._battleStatView != null)
				{
					this._battleStatView.LateUpdate();
				}
			}
			if (this._battleForm != null)
			{
				this._battleForm.LateUpdate();
			}
		}

		private void OnPlayerSpawnBuff(ref SpawnBuffEventParam _param)
		{
			if (_param.src)
			{
				if (_param.showType != 0u)
				{
					this.CreateRestrictFloatText((RESTRICT_TYPE)_param.showType, (Vector3)_param.src.handle.location);
				}
				else if (_param.floatTextID > 0u)
				{
					this.CreateSpecifiedFloatText(_param.floatTextID, (Vector3)_param.src.handle.location);
				}
			}
		}

		public void CreateBattleFloatDigit(int digitValue, DIGIT_TYPE digitType, Vector3 worldPosition)
		{
			if (this.IsFormOpen)
			{
				this.m_battleFloatDigitManager.CreateBattleFloatDigit(digitValue, digitType, ref worldPosition);
			}
		}

		public void CreateBattleFloatDigit(int digitValue, DIGIT_TYPE digitType, Vector3 worldPosition, int animatIndex)
		{
			if (this.IsFormOpen)
			{
				this.m_battleFloatDigitManager.CreateBattleFloatDigit(digitValue, digitType, ref worldPosition, animatIndex);
			}
		}

		public void CollectFloatDigitInSingleFrame(PoolObjHandle<ActorRoot> attacker, PoolObjHandle<ActorRoot> target, DIGIT_TYPE digitType, int value)
		{
			if (this.IsFormOpen)
			{
				this.m_battleFloatDigitManager.CollectFloatDigitInSingleFrame(attacker, target, digitType, value);
			}
		}

		public void CreateRestrictFloatText(RESTRICT_TYPE restrictType, Vector3 worldPosition)
		{
			if (this.IsFormOpen)
			{
				this.m_battleFloatDigitManager.CreateRestrictFloatText(restrictType, ref worldPosition);
			}
		}

		public void CreateSpecifiedFloatText(uint floatTextID, Vector3 worldPosition)
		{
			if (this.IsFormOpen)
			{
				this.m_battleFloatDigitManager.CreateSpecifiedFloatText(floatTextID, ref worldPosition);
			}
		}

		public void CreateOtherFloatText(enOtherFloatTextContent otherFloatTextContent, Vector3 worldPosition, params string[] args)
		{
			if (this.IsFormOpen)
			{
				this.m_battleFloatDigitManager.CreateOtherFloatText(otherFloatTextContent, ref worldPosition, args);
			}
		}

		public CUIContainerScript GetInOutEquipShopHudContainer()
		{
			CUIFormScript cUIFormScript = Singleton<WatchController>.GetInstance().IsWatching ? this.WatchFormScript : this.FightFormScript;
			if (cUIFormScript != null)
			{
				GameObject widget = cUIFormScript.GetWidget(2);
				if (widget != null)
				{
					return widget.GetComponent<CUIContainerScript>();
				}
			}
			return null;
		}

		private void OnFloatTextAnimEnd(CUIEvent uiEvent)
		{
			this.m_battleFloatDigitManager.ClearBattleFloatText(uiEvent.m_srcWidgetScript as CUIAnimatorScript);
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
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnActorGoldChange(changeValue, actor.handle.ValueComponent.GetGoldCoinInBattle());
			}
		}

		private void OnDragonKilled(PoolObjHandle<ActorRoot> actor, int dragonType)
		{
			DragonKillInfo dragonKillInfo = new DragonKillInfo();
			dragonKillInfo.time = Singleton<BattleLogic>.GetInstance().CalcCurrentTime();
			dragonKillInfo.killer = actor;
			dragonKillInfo.dragonType = dragonType;
			if (this.WatchForm != null)
			{
				this.WatchForm.dragonKillInfos.Add(dragonKillInfo);
			}
		}

		private void OnBattleEquipFormOpen(CUIEvent uiEvent)
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnEquipFormOpen(uiEvent);
			}
		}

		private void OnBattleEquipFormClose(CUIEvent uiEvent)
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnEquipFormClose(uiEvent);
			}
		}

		private void OnBattleEquipTypeListSelect(CUIEvent uiEvent)
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnEquipTypeListSelect(uiEvent);
			}
		}

		private void OnBattleEuipItemSelect(CUIEvent uiEvent)
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnEquipItemSelect(uiEvent);
			}
		}

		private void OnBattleEquipBagItemSelect(CUIEvent uiEvent)
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnEquipBagItemSelect(uiEvent);
			}
		}

		private void OnBattleEquipBuy(CUIEvent uiEvent)
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnEquipBuyBtnClick(uiEvent);
			}
		}

		private void OnBattleEquipSale(CUIEvent uiEvent)
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnEquipSaleBtnClick(uiEvent);
			}
		}

		private void OnBattleEquipQuicklyBuy(CUIEvent uiEvent)
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnBattleEquipQuicklyBuy(uiEvent);
			}
		}

		private void OnBattleEquipOpenEquipTree(CUIEvent uiEvent)
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnBattleEquipOpenEquipTree(uiEvent);
			}
		}

		private void OnBattleEquipCloseEquipTree(CUIEvent uiEvent)
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnBattleEquipCloseEquipTree(uiEvent);
			}
		}

		private void OnBattleEquipSelectItemInEquipTree(CUIEvent uiEvent)
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnBattleEquipSelectItemInEquipTree(uiEvent);
			}
		}

		private void OnBattleEquipBackEquipListSelectedChanged(CUIEvent uiEvent)
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnBattleEquipBackEquipListSelectedChanged(uiEvent);
			}
		}

		private void OnBattleEquipBackEquipListScrollChanged(CUIEvent uiEvent)
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnBattleEquipBackEquipListScrollChanged(uiEvent);
			}
		}

		private void OnBattleEquipBackEquipListMoreLeftClicked(CUIEvent uiEvent)
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnBattleEquipBackEquipListMoreLeftClicked(uiEvent);
			}
		}

		private void OnBattleEquipBackEquipListMoreRightClicked(CUIEvent uiEvent)
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnBattleEquipBackEquipListMoreRightClicked(uiEvent);
			}
		}

		private void OnBattleEquipChangeSelfRcmEuipPlan(CUIEvent uiEvent)
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnBattleEquipChangeSelfRcmEuipPlan(uiEvent);
			}
		}

		private void OnBattleEquipOpenSelfRcmEuipPlanForm(CUIEvent uiEvent)
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.OnBattleEquipOpenSelfRcmEuipPlanForm(uiEvent);
			}
		}

		private void OnSettingShowEquipInfoChange()
		{
			if (this.m_battleEquipSystem != null)
			{
				this.m_battleEquipSystem.RefreshQuicklyBuyPanel(true);
			}
		}

		public static void Preload(ref ActorPreloadTab preloadTab)
		{
			preloadTab.AddMesh(CUIParticleSystem.s_particleSkillBtnEffect_Path);
		}

		private void OnToggleStatView(CUIEvent uiEvent)
		{
			if (this._battleStatView != null)
			{
				this._battleStatView.Toggle();
			}
		}

		public void ClosePopupForms()
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CSettingsSys.SETTING_FORM);
			Singleton<CUIManager>.GetInstance().CloseForm(CBattleEquipSystem.s_equipFormPath);
			Singleton<CUIManager>.GetInstance().CloseForm(CBattleHeroInfoPanel.s_battleHeroInfoForm);
			if (this._miniMapSys != null)
			{
				this._miniMapSys.Switch(MinimapSys.EMapType.Mini);
			}
		}
	}
}
