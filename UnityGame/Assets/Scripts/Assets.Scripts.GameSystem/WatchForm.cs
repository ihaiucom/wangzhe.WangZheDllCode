using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class WatchForm : IBattleForm
	{
		private const float k_stepScale = 0.25f;

		private const float k_toleranceScale = 0.05f;

		private const float k_normalScaleVal = 1.2f;

		private const float k_hideScaleVal = 1.6f;

		public const int SAMPLE_FRAME_STEP = 150;

		private CUIFormScript _form;

		private PoolObjHandle<ActorRoot> _focusHero;

		private DictionaryView<uint, HeroInfoItem> _heroWrapDict;

		private WatchScoreHud _scoreHud;

		private GameObject _camp1List;

		private GameObject _camp2List;

		private CUIListScript _camp1BaseList;

		private CUIListScript _camp1EquipList;

		private CUIListScript _camp2BaseList;

		private CUIListScript _camp2EquipList;

		private CUIListScript _camp1BaseSideList;

		private CUIListScript _camp2BaseSideList;

		private GameObject _detailPanel;

		private DictionaryView<uint, HeroInfoSideItem> _heroWrapSideDict;

		private HeroInfoHud _heroInfoHud;

		private ReplayControl _replayControl;

		private GameObject _hideBtn;

		private GameObject _showBtn;

		private GameObject competitionBtn;

		private bool _isCampFold_1;

		private bool _isCampFold_2;

		private bool _isBottomFold;

		private bool _isBottomLong;

		private bool _isBottomInAnimDoing;

		private bool _kdaChanged;

		private bool _isViewHide;

		private Plane _pickPlane;

		private float m_targetScaleVal = 1.2f;

		private float _lastSampleTime;

		public ArrayList dragonKillInfos = new ArrayList();

		private float m_lastIRStoreTimeStamp;

		private float K_IR_STORE_GAP = 1f;

		private uint _lastUpdateFrame;

		private uint _clickPickHeroID;

		public uint TargetHeroId
		{
			get;
			private set;
		}

		public SampleData moneySample
		{
			get;
			private set;
		}

		public SampleData expSample
		{
			get;
			private set;
		}

		public CUIFormScript FormScript
		{
			get
			{
				return this._form;
			}
		}

		public bool IsFormShow
		{
			get
			{
				return null != this._form;
			}
		}

		public WatchForm()
		{
			this._pickPlane = new Plane(new Vector3(0f, 1f, 0f), 0f);
		}

		public static string GetWatchFormName()
		{
			if (WatchForm.IsNeedShowCampMidInterface())
			{
				return "UGUI/Form/Battle/Form_Watch_Competition.prefab";
			}
			return "UGUI/Form/Battle/Form_Watch.prefab";
		}

		public bool OpenForm()
		{
			this._form = Singleton<CUIManager>.GetInstance().OpenForm(WatchForm.GetWatchFormName(), false, true);
			if (null == this._form)
			{
				return false;
			}
			this._scoreHud = new WatchScoreHud(Utility.FindChild(this._form.gameObject, "ScoreBoard"));
			if (WatchForm.IsNeedShowCampMidInterface())
			{
				this._detailPanel = this._form.GetWidget(0);
				this._camp1BaseList = Utility.GetComponetInChild<CUIListScript>(this._detailPanel, "BaseInfoList1");
				this._camp1EquipList = Utility.GetComponetInChild<CUIListScript>(this._detailPanel, "EquipInfoList1");
				this._camp2BaseList = Utility.GetComponetInChild<CUIListScript>(this._detailPanel, "BaseInfoList2");
				this._camp2EquipList = Utility.GetComponetInChild<CUIListScript>(this._detailPanel, "EquipInfoList2");
				this._camp1List = Utility.FindChild(this._form.gameObject, "CampInfoList_1");
				this._camp2List = Utility.FindChild(this._form.gameObject, "CampInfoList_2");
				this._camp1BaseSideList = Utility.GetComponetInChild<CUIListScript>(this._camp1List, "BaseInfoList");
				this._camp2BaseSideList = Utility.GetComponetInChild<CUIListScript>(this._camp2List, "BaseInfoList");
			}
			else
			{
				this._camp1List = Utility.FindChild(this._form.gameObject, "CampInfoList_1");
				this._camp1BaseList = Utility.GetComponetInChild<CUIListScript>(this._camp1List, "BaseInfoList");
				this._camp1EquipList = Utility.GetComponetInChild<CUIListScript>(this._camp1List, "EquipInfoList");
				this._camp2List = Utility.FindChild(this._form.gameObject, "CampInfoList_2");
				this._camp2BaseList = Utility.GetComponetInChild<CUIListScript>(this._camp2List, "BaseInfoList");
				this._camp2EquipList = Utility.GetComponetInChild<CUIListScript>(this._camp2List, "EquipInfoList");
				this._camp1BaseSideList = null;
				this._camp2BaseSideList = null;
			}
			this._hideBtn = Utility.FindChild(this._form.gameObject, "PanelBtn/HideBtn");
			this._showBtn = Utility.FindChild(this._form.gameObject, "PanelBtn/ShowBtn");
			this.competitionBtn = Utility.FindChild(this._form.gameObject, "PanelBtn/competitionBtn");
			this.competitionBtn.CustomSetActive(Singleton<WatchController>.GetInstance().CanShowActorIRPosMap());
			if (!WatchForm.IsNeedShowCampMidInterface())
			{
				this._heroInfoHud = new HeroInfoHud(Utility.FindChild(this._form.gameObject, "HeroInfoHud"));
				this._replayControl = new ReplayControl(Utility.FindChild(this._form.gameObject, "ReplayControl"));
			}
			else
			{
				this._heroInfoHud = null;
				this._replayControl = new ReplayControl(Utility.FindChild(this._form.gameObject, "PanelBtn"));
			}
			return true;
		}

		public void CloseForm()
		{
			if (null == this._form)
			{
				return;
			}
			Singleton<CUIManager>.GetInstance().CloseForm(this._form);
			this._form = null;
		}

		private void OnFormClosed(CUIEvent uiEvt)
		{
			Singleton<CBattleSystem>.GetInstance().OnFormClosed();
			this.UnRegisterEvents();
			if (this._heroWrapDict != null)
			{
				this._heroWrapDict.Clear();
				this._heroWrapDict = null;
			}
			if (this._scoreHud != null)
			{
				this._scoreHud.Clear();
				this._scoreHud = null;
			}
			if (this._heroInfoHud != null)
			{
				this._heroInfoHud.Clear();
				this._heroInfoHud = null;
			}
			if (this._replayControl != null)
			{
				this._replayControl.Clear();
				this._replayControl = null;
			}
			this._camp1List = null;
			this._camp2List = null;
			this._camp1BaseList = null;
			this._camp1EquipList = null;
			this._camp2BaseList = null;
			this._camp2EquipList = null;
			this._hideBtn = null;
			this._showBtn = null;
			this._detailPanel = null;
			this._camp1BaseSideList = null;
			this._camp2BaseSideList = null;
			if (this._heroWrapDict != null)
			{
				this._heroWrapSideDict.Clear();
				this._heroWrapSideDict = null;
			}
			this.moneySample = null;
			this.expSample = null;
			this._form = null;
		}

		public static bool IsNeedShowCampMidInterface()
		{
			return Singleton<WatchController>.GetInstance().IsLiveCast;
		}

		private void RegisterEvents()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_CameraDraging, new CUIEventManager.OnUIEventHandler(this.OnCameraDraging));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_PickCampList_1, new CUIEventManager.OnUIEventHandler(this.OnPickCampList_1));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_PickCampList_2, new CUIEventManager.OnUIEventHandler(this.OnPickCampList_2));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_Quit, new CUIEventManager.OnUIEventHandler(this.OnQuitWatch));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickCampFold_1, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_1));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickCampFold_1_End, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_1_End));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickCampFold_2, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_2));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickCampFold_2_End, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_2_End));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickBottomFold, new CUIEventManager.OnUIEventHandler(this.OnClickBottomFold));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickBottomFold_End, new CUIEventManager.OnUIEventHandler(this.OnClickBottomFold_End));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickReplayTalk, new CUIEventManager.OnUIEventHandler(this.OnClickReplayTalk));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_HideView, new CUIEventManager.OnUIEventHandler(this.OnToggleHide));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_QuitConfirm, new CUIEventManager.OnUIEventHandler(this.OnQuitConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_FormClosed, new CUIEventManager.OnUIEventHandler(this.OnFormClosed));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_SelectHorizonBoth, new CUIEventManager.OnUIEventHandler(this.OnSelectHorizonBoth));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_SelectHorizonCamp_1, new CUIEventManager.OnUIEventHandler(this.OnSelectHorizonCamp_1));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_SelectHorizonCamp_2, new CUIEventManager.OnUIEventHandler(this.OnSelectHorizonCamp_2));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickBottomEquipFold, new CUIEventManager.OnUIEventHandler(this.OnClickBottomEquipFold));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickBottomEquipFold_Over, new CUIEventManager.OnUIEventHandler(this.OnClickBottomEquipFoldOver));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Click_Scene, new CUIEventManager.OnUIEventHandler(this.OnClickBattleScene));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroLevelChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this.OnBattleMoneyChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler<uint, stEquipInfo[], bool, int>("HeroEquipInBattleChange", new Action<uint, stEquipInfo[], bool, int>(this.OnBattleEquipChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.BATTLE_KDA_CHANGED_BY_ACTOR_DEAD, new Action(this.OnBattleKDAChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroEpChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, new GameSkillEvent<DefaultSkillEventParam>(this.OnSkillCDChanged));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UpdateSkillUI, new GameSkillEvent<DefaultSkillEventParam>(this.OnSkillChanged));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_EnableSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnSkillEnable));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this.OnSkillLevelUp));
		}

		private void UnRegisterEvents()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_CameraDraging, new CUIEventManager.OnUIEventHandler(this.OnCameraDraging));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_PickCampList_1, new CUIEventManager.OnUIEventHandler(this.OnPickCampList_1));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_PickCampList_2, new CUIEventManager.OnUIEventHandler(this.OnPickCampList_2));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_Quit, new CUIEventManager.OnUIEventHandler(this.OnQuitWatch));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickCampFold_1, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_1));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickCampFold_1_End, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_1_End));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickCampFold_2, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_2));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickCampFold_2_End, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_2_End));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickBottomFold, new CUIEventManager.OnUIEventHandler(this.OnClickBottomFold));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickBottomFold_End, new CUIEventManager.OnUIEventHandler(this.OnClickBottomFold_End));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickReplayTalk, new CUIEventManager.OnUIEventHandler(this.OnClickReplayTalk));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_HideView, new CUIEventManager.OnUIEventHandler(this.OnToggleHide));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_QuitConfirm, new CUIEventManager.OnUIEventHandler(this.OnQuitConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_FormClosed, new CUIEventManager.OnUIEventHandler(this.OnFormClosed));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_SelectHorizonBoth, new CUIEventManager.OnUIEventHandler(this.OnSelectHorizonBoth));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_SelectHorizonCamp_1, new CUIEventManager.OnUIEventHandler(this.OnSelectHorizonCamp_1));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_SelectHorizonCamp_2, new CUIEventManager.OnUIEventHandler(this.OnSelectHorizonCamp_2));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Click_Scene, new CUIEventManager.OnUIEventHandler(this.OnClickBattleScene));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroLevelChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this.OnBattleMoneyChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint, stEquipInfo[], bool, int>("HeroEquipInBattleChange", new Action<uint, stEquipInfo[], bool, int>(this.OnBattleEquipChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.BATTLE_KDA_CHANGED_BY_ACTOR_DEAD, new Action(this.OnBattleKDAChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroEpChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, new GameSkillEvent<DefaultSkillEventParam>(this.OnSkillCDChanged));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UpdateSkillUI, new GameSkillEvent<DefaultSkillEventParam>(this.OnSkillChanged));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this.OnSkillLevelUp));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_EnableSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnSkillEnable));
		}

		public void BattleStart()
		{
			if (this._heroWrapDict != null)
			{
				return;
			}
			this._lastUpdateFrame = 0u;
			this.TargetHeroId = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain.handle.ObjID;
			this._heroWrapDict = new DictionaryView<uint, HeroInfoItem>();
			if (WatchForm.IsNeedShowCampMidInterface())
			{
				this._heroWrapSideDict = new DictionaryView<uint, HeroInfoSideItem>();
			}
			List<HeroKDA> list = new List<HeroKDA>();
			List<HeroKDA> list2 = new List<HeroKDA>();
			CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				PlayerKDA value = current.get_Value();
				if (value.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
				{
					ListView<HeroKDA>.Enumerator enumerator2 = value.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						list.Add(enumerator2.Current);
					}
				}
				else if (value.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
				{
					ListView<HeroKDA>.Enumerator enumerator3 = value.GetEnumerator();
					while (enumerator3.MoveNext())
					{
						list2.Add(enumerator3.Current);
					}
				}
			}
			this.InitCampInfoUIList(COM_PLAYERCAMP.COM_PLAYERCAMP_1, list, this._camp1BaseList, this._camp1EquipList, this._camp1BaseSideList);
			this.InitCampInfoUIList(COM_PLAYERCAMP.COM_PLAYERCAMP_2, list2, this._camp2BaseList, this._camp2EquipList, this._camp2BaseSideList);
			Singleton<WatchController>.GetInstance().SwitchObserveCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT);
			this.FocusHero(this.TargetHeroId);
			this.ValidateCampMoney();
			if (this._heroInfoHud != null)
			{
				this._heroInfoHud.FightStart();
			}
			this.RegisterEvents();
			this._isBottomFold = false;
			this._isCampFold_1 = false;
			this.OnClickCampFold_1(null);
			this._isCampFold_2 = false;
			this.OnClickCampFold_2(null);
			this._isBottomLong = true;
			this.OnClickBottomEquipFold(null);
			this._isViewHide = false;
			float step = 150u * Singleton<WatchController>.GetInstance().FrameDelta * 0.001f;
			this.moneySample = new SampleData(step);
			this.expSample = new SampleData(step);
			this.dragonKillInfos.Clear();
			this._lastSampleTime = 0f;
			Moba_Camera mobaCamera = MonoSingleton<CameraSystem>.GetInstance().MobaCamera;
			this.m_targetScaleVal = 1.2f;
			if (null != mobaCamera)
			{
				mobaCamera.currentZoomRate = 1.2f;
				mobaCamera.CameraUpdate();
			}
		}

		private void InitCampInfoUIList(COM_PLAYERCAMP listCamp, List<HeroKDA> heroList, CUIListScript baseInfoUIList, CUIListScript equipInfoUIList, CUIListScript sideInfoList = null)
		{
			if (null == baseInfoUIList || heroList == null || heroList.get_Count() == 0)
			{
				return;
			}
			baseInfoUIList.SetElementAmount(5);
			equipInfoUIList.SetElementAmount(5);
			if (sideInfoList != null)
			{
				sideInfoList.SetElementAmount(5);
			}
			for (int i = 0; i < 5; i++)
			{
				GameObject gameObject = baseInfoUIList.GetElemenet(i).gameObject;
				GameObject gameObject2 = equipInfoUIList.GetElemenet(i).gameObject;
				GameObject gameObject3 = null;
				if (sideInfoList != null)
				{
					gameObject3 = sideInfoList.GetElemenet(i).gameObject;
				}
				if (i < heroList.get_Count())
				{
					HeroKDA heroKDA = heroList.get_Item(i);
					HeroInfoItem value = new HeroInfoItem(listCamp, i, heroKDA, gameObject, gameObject2);
					if (gameObject3 != null)
					{
						HeroInfoSideItem value2 = new HeroInfoSideItem(listCamp, heroKDA, i, gameObject3);
						this._heroWrapSideDict.Add(heroKDA.actorHero.handle.ObjID, value2);
					}
					this._heroWrapDict.Add(heroKDA.actorHero.handle.ObjID, value);
				}
				else
				{
					HeroInfoItem.MakeEmpty(gameObject, gameObject2);
					if (sideInfoList != null)
					{
						HeroInfoSideItem.MakeEmpty(gameObject3);
					}
				}
			}
		}

		public void UpdateLogic(int delta)
		{
			if (this.moneySample != null && this.expSample != null && Singleton<FrameSynchr>.GetInstance().CurFrameNum % 150u == 1u)
			{
				CampInfo campInfoByCamp = Singleton<BattleStatistic>.GetInstance().GetCampInfoByCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
				CampInfo campInfoByCamp2 = Singleton<BattleStatistic>.GetInstance().GetCampInfoByCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
				if (campInfoByCamp != null && campInfoByCamp2 != null)
				{
					this.moneySample.SetCurData(campInfoByCamp.coinTotal, campInfoByCamp2.coinTotal);
					this.expSample.SetCurData(campInfoByCamp.soulExpTotal, campInfoByCamp2.soulExpTotal);
					this._lastSampleTime = Time.time;
				}
			}
			if (this._heroInfoHud != null)
			{
				this._heroInfoHud.UpdateLogic(delta);
			}
			if (Singleton<WatchController>.GetInstance().CanShowActorIRPosMap() && Time.time - this.m_lastIRStoreTimeStamp > this.K_IR_STORE_GAP)
			{
				Singleton<CBattleStatCompetitionSystem>.GetInstance().StoreIRViewData();
				this.m_lastIRStoreTimeStamp = Time.time;
			}
		}

		public void Update()
		{
			Moba_Camera mobaCamera = MonoSingleton<CameraSystem>.GetInstance().MobaCamera;
			if (null != mobaCamera)
			{
				float num = this.m_targetScaleVal - mobaCamera.currentZoomRate;
				float num2 = Math.Abs(num);
				if (num2 >= 0.05f)
				{
					int arg_49_0 = (num > 0f) ? 1 : -1;
					mobaCamera.currentZoomRate += num * 0.25f;
					mobaCamera.CameraUpdate();
				}
				else
				{
					mobaCamera.currentZoomRate = this.m_targetScaleVal;
				}
			}
		}

		public void LateUpdate()
		{
			if (this._heroWrapDict == null)
			{
				return;
			}
			if (!Singleton<WatchController>.GetInstance().IsWatching)
			{
				this.CloseForm();
				bool flag = true;
				if (flag)
				{
					Singleton<SettlementSystem>.GetInstance().ShowSettlementPanel(true);
				}
				Singleton<GameBuilder>.GetInstance().EndGame();
				return;
			}
			uint curFrameNum = Singleton<FrameSynchr>.GetInstance().CurFrameNum;
			if (curFrameNum < this._lastUpdateFrame + 3u)
			{
				return;
			}
			this._lastUpdateFrame = curFrameNum;
			DictionaryView<uint, HeroInfoItem>.Enumerator enumerator = this._heroWrapDict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, HeroInfoItem> current = enumerator.Current;
				current.get_Value().LateUpdate();
				if (this._kdaChanged)
				{
					KeyValuePair<uint, HeroInfoItem> current2 = enumerator.Current;
					current2.get_Value().ValidateKDA();
				}
			}
			if (WatchForm.IsNeedShowCampMidInterface())
			{
				DictionaryView<uint, HeroInfoSideItem>.Enumerator enumerator2 = this._heroWrapSideDict.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					KeyValuePair<uint, HeroInfoSideItem> current3 = enumerator2.Current;
					current3.get_Value().LateUpdate();
				}
			}
			if (this._heroInfoHud != null)
			{
				this._heroInfoHud.LateUpdate();
				if (this._kdaChanged)
				{
					this._heroInfoHud.ValidateKDA();
				}
			}
			if (this._scoreHud != null)
			{
				this._scoreHud.LateUpdate();
			}
			if (this._replayControl != null)
			{
				this._replayControl.LateUpdate();
			}
			this._kdaChanged = false;
		}

		private void OnQuitWatch(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("confirmQuitWatch"), enUIEventID.Watch_QuitConfirm, enUIEventID.Watch_QuitCancel, false);
		}

		private void OnQuitConfirm(CUIEvent uiEvent)
		{
			Singleton<WatchController>.GetInstance().Quit();
		}

		private void OnToggleHide(CUIEvent uiEvent)
		{
			if (this._form && this._form.gameObject)
			{
				this._isViewHide = !this._isViewHide;
				this.SwitchCameraZoom();
				if (!WatchForm.IsNeedShowCampMidInterface())
				{
					this._hideBtn.CustomSetActive(!this._isViewHide);
					this._showBtn.CustomSetActive(this._isViewHide);
					this.DisableAnimator(Utility.FindChild(this._camp1List, "EquipInfoList"));
					this._camp1List.CustomSetActive(!this._isViewHide);
					this.DisableAnimator(Utility.FindChild(this._camp2List, "EquipInfoList"));
					this._camp2List.CustomSetActive(!this._isViewHide);
					if (!WatchForm.IsNeedShowCampMidInterface())
					{
						this.DisableAnimator(this._heroInfoHud.Root);
						this._heroInfoHud.Root.CustomSetActive(!this._isViewHide);
					}
					if (this._replayControl != null && !WatchForm.IsNeedShowCampMidInterface())
					{
						this.DisableAnimator(this._replayControl.Root);
						this._replayControl.Root.CustomSetActive(!this._isViewHide);
					}
				}
				else if (!this._isBottomInAnimDoing)
				{
					CUIAnimatorScript componetInChild = Utility.GetComponetInChild<CUIAnimatorScript>(this._form.gameObject, "CampInfoList_1");
					if (componetInChild)
					{
						bool flag = this._isCampFold_1 != this._isViewHide;
						this._isCampFold_1 = this._isViewHide;
						if (flag)
						{
							componetInChild.PlayAnimator(this._isCampFold_1 ? "Hide_Left_1" : "Hide_Left_2");
						}
					}
					CUIAnimatorScript componetInChild2 = Utility.GetComponetInChild<CUIAnimatorScript>(this._form.gameObject, "CampInfoList_2");
					if (componetInChild2)
					{
						bool flag2 = this._isCampFold_2 != this._isViewHide;
						this._isCampFold_2 = this._isViewHide;
						if (flag2)
						{
							componetInChild2.PlayAnimator(this._isCampFold_2 ? "Hide_Right_1" : "Hide_Right_2");
						}
					}
					CUIAnimatorScript componetInChild3 = Utility.GetComponetInChild<CUIAnimatorScript>(this._form.gameObject, "DetailControl");
					if (componetInChild3)
					{
						bool flag3 = this._isBottomFold != this._isViewHide;
						this._isBottomFold = this._isViewHide;
						if (flag3)
						{
							this._isBottomInAnimDoing = true;
							if (this._isBottomFold)
							{
								componetInChild3.PlayAnimator(this._isBottomLong ? "UpAndDown_1_1" : "UpAndDown_2_1");
							}
							else
							{
								componetInChild3.PlayAnimator(this._isBottomLong ? "UpAndDown_1_2" : "UpAndDown_2_2");
							}
						}
					}
				}
				this.m_targetScaleVal = (this._isViewHide ? 1.6f : 1.2f);
			}
		}

		private void DisableAnimator(GameObject node)
		{
			if (node)
			{
				Animator component = node.GetComponent<Animator>();
				if (component)
				{
					component.enabled = false;
				}
			}
		}

		public void OnCameraDraging(CUIEvent uiEvent)
		{
			if (MonoSingleton<CameraSystem>.GetInstance().enableLockedCamera)
			{
				if (this._focusHero)
				{
					MonoSingleton<CameraSystem>.instance.MobaCamera.SetAbsoluteLockLocation((Vector3)this._focusHero.handle.location);
				}
				this.FreeFocus();
			}
			MonoSingleton<CameraSystem>.GetInstance().MoveCamera(-uiEvent.m_pointerEventData.get_delta().x, -uiEvent.m_pointerEventData.get_delta().y);
		}

		public void FreeFocus()
		{
			this._camp1BaseList.SelectElement(-1, false);
			this._camp2BaseList.SelectElement(-1, false);
			if (this._focusHero)
			{
				this._focusHero.Release();
			}
			if (MonoSingleton<CameraSystem>.GetInstance().enableLockedCamera)
			{
				MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(true);
			}
		}

		public void OnPickCampList_1(CUIEvent uiEvent)
		{
			this.FocusHero(COM_PLAYERCAMP.COM_PLAYERCAMP_1, uiEvent.m_srcWidgetIndexInBelongedList);
		}

		public void OnPickCampList_2(CUIEvent uiEvent)
		{
			this.FocusHero(COM_PLAYERCAMP.COM_PLAYERCAMP_2, uiEvent.m_srcWidgetIndexInBelongedList);
		}

		private void FocusHero(uint heroObjId)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(heroObjId);
			if (actor)
			{
				this.FocusHero(actor.handle.TheActorMeta.ActorCamp, this._heroWrapDict[heroObjId].listIndex);
			}
		}

		private void FocusHero(COM_PLAYERCAMP listCamp, int listIndex)
		{
			if (this._heroWrapDict == null)
			{
				return;
			}
			HeroInfoItem heroInfoItem = null;
			DictionaryView<uint, HeroInfoItem>.Enumerator enumerator = this._heroWrapDict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, HeroInfoItem> current = enumerator.Current;
				HeroInfoItem value = current.get_Value();
				if (value.listCamp == listCamp && value.listIndex == listIndex)
				{
					heroInfoItem = value;
					break;
				}
			}
			if (heroInfoItem != null)
			{
				COM_PLAYERCAMP horizonCamp = Singleton<WatchController>.GetInstance().HorizonCamp;
				if (horizonCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT && horizonCamp != heroInfoItem.HeroInfo.actorHero.handle.TheActorMeta.ActorCamp)
				{
					MonoSingleton<CameraSystem>.instance.MobaCamera.SetAbsoluteLockLocation((Vector3)heroInfoItem.HeroInfo.actorHero.handle.location);
					this.FreeFocus();
				}
				else
				{
					MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(false);
					MonoSingleton<CameraSystem>.instance.SetFocusActor(heroInfoItem.HeroInfo.actorHero);
				}
				if (heroInfoItem != null && heroInfoItem.HeroInfo != null && this._camp1BaseList != null && this._camp2BaseList != null)
				{
					this._focusHero = heroInfoItem.HeroInfo.actorHero;
					if (this._heroInfoHud != null)
					{
						this._heroInfoHud.SetPickHero(heroInfoItem.HeroInfo);
					}
					if (listCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
					{
						this._camp1BaseList.SelectElement(listIndex, false);
						this._camp2BaseList.SelectElement(-1, false);
					}
					else if (listCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
					{
						this._camp2BaseList.SelectElement(listIndex, false);
						this._camp1BaseList.SelectElement(-1, false);
					}
					else
					{
						this._camp1BaseList.SelectElement(-1, false);
						this._camp2BaseList.SelectElement(-1, false);
					}
				}
			}
			else
			{
				this.FreeFocus();
			}
		}

		private void OnHeroLevelChange(PoolObjHandle<ActorRoot> hero, int level)
		{
			if (this._heroWrapDict.ContainsKey(hero.handle.ObjID))
			{
				this._heroWrapDict[hero.handle.ObjID].ValidateLevel();
			}
			if (WatchForm.IsNeedShowCampMidInterface())
			{
				if (this._heroWrapSideDict.ContainsKey(hero.handle.ObjID))
				{
					this._heroWrapSideDict[hero.handle.ObjID].ValidateLevel();
				}
			}
			else if (this._heroInfoHud != null && hero.handle.ObjID == this._heroInfoHud.PickedHeroID)
			{
				this._heroInfoHud.ValidateLevel();
			}
		}

		private void OnBattleKDAChange()
		{
			this._kdaChanged = true;
		}

		private void OnBattleMoneyChange(PoolObjHandle<ActorRoot> actor, int changeValue, bool isIncome, PoolObjHandle<ActorRoot> target)
		{
			this.ValidateCampMoney();
			if (this._heroWrapDict.ContainsKey(actor.handle.ObjID))
			{
				this._heroWrapDict[actor.handle.ObjID].ValidateMoney();
			}
			if (!WatchForm.IsNeedShowCampMidInterface() && this._heroInfoHud != null && actor.handle.ObjID == this._heroInfoHud.PickedHeroID)
			{
				this._heroInfoHud.ValidateMoney();
			}
		}

		private void OnBattleEquipChange(uint actorObjectID, stEquipInfo[] equips, bool bIsAdd, int iEquipSlotIndex)
		{
			if (this._heroWrapDict.ContainsKey(actorObjectID))
			{
				this._heroWrapDict[actorObjectID].ValidateEquip();
			}
			if (!WatchForm.IsNeedShowCampMidInterface() && this._heroInfoHud != null && actorObjectID == this._heroInfoHud.PickedHeroID)
			{
				this._heroInfoHud.ValidateEquip();
			}
		}

		private void OnHeroHpChange(PoolObjHandle<ActorRoot> hero, int iCurVal, int iMaxVal)
		{
			if (!WatchForm.IsNeedShowCampMidInterface() && this._heroInfoHud != null && hero.handle.ObjID == this._heroInfoHud.PickedHeroID)
			{
				this._heroInfoHud.ValidateHp();
			}
		}

		private void OnHeroEpChange(PoolObjHandle<ActorRoot> hero, int iCurVal, int iMaxVal)
		{
			if (!WatchForm.IsNeedShowCampMidInterface() && this._heroInfoHud != null && hero.handle.ObjID == this._heroInfoHud.PickedHeroID)
			{
				this._heroInfoHud.ValidateEp();
			}
		}

		private void ValidateCampMoney()
		{
			this._scoreHud.ValidateMoney(Singleton<BattleStatistic>.GetInstance().GetCampInfoByCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_1).coinTotal, Singleton<BattleStatistic>.GetInstance().GetCampInfoByCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_2).coinTotal);
		}

		private void OnSkillCDChanged(ref DefaultSkillEventParam _param)
		{
			if (this._heroInfoHud != null && _param.actor.handle.ObjID == this._heroInfoHud.PickedHeroID)
			{
				this._heroInfoHud.TheSkillHud.ValidateCD(_param.slot);
			}
			HeroInfoItem heroInfoItem = null;
			if (this._heroWrapDict.TryGetValue(_param.actor.handle.ObjID, out heroInfoItem))
			{
				heroInfoItem.ValidateCD(_param.slot, _param.actor);
			}
			if (WatchForm.IsNeedShowCampMidInterface())
			{
				HeroInfoSideItem heroInfoSideItem = null;
				if (this._heroWrapSideDict.TryGetValue(_param.actor.handle.ObjID, out heroInfoSideItem) && _param.slot == SkillSlotType.SLOT_SKILL_3)
				{
					heroInfoSideItem.ValidateSkill3();
				}
			}
		}

		private void OnSkillEnable(ref DefaultSkillEventParam _param)
		{
			if (WatchForm.IsNeedShowCampMidInterface())
			{
				HeroInfoSideItem heroInfoSideItem = null;
				if (this._heroWrapSideDict.TryGetValue(_param.actor.handle.ObjID, out heroInfoSideItem) && _param.slot == SkillSlotType.SLOT_SKILL_3)
				{
					heroInfoSideItem.ValidateSkill3();
				}
			}
		}

		private void OnSkillChanged(ref DefaultSkillEventParam _param)
		{
			if (this._heroInfoHud != null && _param.actor.handle.ObjID == this._heroInfoHud.PickedHeroID)
			{
				this._heroInfoHud.TheSkillHud.ValidateSkill(_param.slot);
				this._heroInfoHud.TheSkillHud.ValidateCD(_param.slot);
			}
			HeroInfoItem heroInfoItem = null;
			if (this._heroWrapDict.TryGetValue(_param.actor.handle.ObjID, out heroInfoItem))
			{
				heroInfoItem.ValidateSkill(_param.slot, _param.actor);
				heroInfoItem.ValidateCD(_param.slot, _param.actor);
			}
			if (WatchForm.IsNeedShowCampMidInterface())
			{
				HeroInfoSideItem heroInfoSideItem = null;
				if (this._heroWrapSideDict.TryGetValue(_param.actor.handle.ObjID, out heroInfoSideItem) && _param.slot == SkillSlotType.SLOT_SKILL_3)
				{
					heroInfoSideItem.ValidateSkill3();
				}
			}
		}

		private void OnSkillLevelUp(PoolObjHandle<ActorRoot> hero, byte bSlotType, byte bSkillLevel)
		{
			if (this._heroInfoHud != null && hero.handle.ObjID == this._heroInfoHud.PickedHeroID)
			{
				this._heroInfoHud.TheSkillHud.ValidateLevel((SkillSlotType)bSlotType);
			}
		}

		private void OnClickCampFold_1(CUIEvent evt)
		{
			CUIAnimatorScript componetInChild = Utility.GetComponetInChild<CUIAnimatorScript>(this._form.gameObject, "CampInfoList_1/EquipInfoList");
			if (componetInChild)
			{
				this._isCampFold_1 = !this._isCampFold_1;
				componetInChild.PlayAnimator(this._isCampFold_1 ? "Close" : "Open");
			}
		}

		private void OnClickCampFold_1_End(CUIEvent evt)
		{
			this._form.m_sgameGraphicRaycaster.RefreshGameObject(Utility.FindChild(this._form.gameObject, "CampInfoList_1/EquipInfoList/FoldButton"));
		}

		private void OnClickCampFold_2(CUIEvent evt)
		{
			CUIAnimatorScript componetInChild = Utility.GetComponetInChild<CUIAnimatorScript>(this._form.gameObject, "CampInfoList_2/EquipInfoList");
			if (componetInChild)
			{
				this._isCampFold_2 = !this._isCampFold_2;
				componetInChild.PlayAnimator(this._isCampFold_2 ? "Close" : "Open");
			}
		}

		private void OnClickCampFold_2_End(CUIEvent evt)
		{
			this._form.m_sgameGraphicRaycaster.RefreshGameObject(Utility.FindChild(this._form.gameObject, "CampInfoList_2/EquipInfoList/FoldButton"));
		}

		private void OnClickBottomFold(CUIEvent evt)
		{
			if (WatchForm.IsNeedShowCampMidInterface())
			{
				CUIAnimatorScript componetInChild = Utility.GetComponetInChild<CUIAnimatorScript>(this._form.gameObject, "DetailControl");
				if (componetInChild && !this._isBottomInAnimDoing)
				{
					this._isBottomFold = !this._isBottomFold;
					this._isBottomInAnimDoing = true;
					if (this._isBottomFold)
					{
						componetInChild.PlayAnimator(this._isBottomLong ? "UpAndDown_1_1" : "UpAndDown_2_1");
					}
					else
					{
						componetInChild.PlayAnimator(this._isBottomLong ? "UpAndDown_1_2" : "UpAndDown_2_2");
					}
				}
			}
			else
			{
				CUIAnimatorScript componetInChild2 = Utility.GetComponetInChild<CUIAnimatorScript>(this._form.gameObject, "HeroInfoHud");
				CUIAnimatorScript componetInChild3 = Utility.GetComponetInChild<CUIAnimatorScript>(this._form.gameObject, "ReplayControl");
				if (componetInChild2 && componetInChild3)
				{
					this._isBottomFold = !this._isBottomFold;
					componetInChild2.PlayAnimator(this._isBottomFold ? "Close" : "Open");
					componetInChild3.PlayAnimator(this._isBottomFold ? "Close" : "Open");
				}
			}
		}

		private void OnClickBottomEquipFold(CUIEvent evt)
		{
			CUIFormScript cUIFormScript = this._form;
			if (cUIFormScript == null && evt != null && evt.m_srcFormScript != null)
			{
				cUIFormScript = evt.m_srcFormScript;
			}
			CUIAnimatorScript componetInChild = Utility.GetComponetInChild<CUIAnimatorScript>(cUIFormScript.gameObject, "DetailControl");
			if (componetInChild && !this._isBottomInAnimDoing)
			{
				this._isBottomInAnimDoing = true;
				this._isBottomLong = !this._isBottomLong;
				componetInChild.PlayAnimator(this._isBottomLong ? "Expansion_2" : "Expansion_1");
			}
		}

		private void OnClickBottomEquipFoldOver(CUIEvent evt)
		{
			this._isBottomInAnimDoing = false;
		}

		private void OnClickBottomFold_End(CUIEvent evt)
		{
			if (WatchForm.IsNeedShowCampMidInterface())
			{
				this._form.m_sgameGraphicRaycaster.RefreshGameObject(Utility.FindChild(this._form.gameObject, "DetailControl"));
			}
			else
			{
				this._form.m_sgameGraphicRaycaster.RefreshGameObject(Utility.FindChild(this._form.gameObject, "HeroInfoHud"));
			}
			this._isBottomInAnimDoing = false;
		}

		private void OnClickReplayTalk(CUIEvent evt)
		{
			Singleton<CUIManager>.GetInstance().OpenTips("barrageNotReady", true, 1.5f, null, new object[0]);
		}

		private void OnClickBattleScene(CUIEvent uievent)
		{
			Ray ray = Camera.main.ScreenPointToRay(uievent.m_pointerEventData.get_position());
			this._clickPickHeroID = 0u;
			float distance;
			if (this._pickPlane.Raycast(ray, out distance))
			{
				Vector3 point = ray.GetPoint(distance);
				SceneManagement instance = Singleton<SceneManagement>.GetInstance();
				SceneManagement.Coordinate coord = default(SceneManagement.Coordinate);
				instance.GetCoord_Center(ref coord, ((VInt3)point).xz, 3000);
				instance.UpdateDirtyNodes();
				instance.ForeachActorsBreak(coord, new SceneManagement.Process_Bool(this.TrySearchHero));
				if (this._clickPickHeroID > 0u)
				{
					this.FocusHero(this._clickPickHeroID);
				}
			}
		}

		private bool TrySearchHero(ref PoolObjHandle<ActorRoot> actor)
		{
			if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				this._clickPickHeroID = actor.handle.ObjID;
				return false;
			}
			return true;
		}

		private void OnSelectHorizonBoth(CUIEvent evt)
		{
			if (evt.m_eventParams.togleIsOn)
			{
				this.SwitchObserveCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT);
			}
		}

		private void OnSelectHorizonCamp_1(CUIEvent evt)
		{
			if (evt.m_eventParams.togleIsOn)
			{
				this.SwitchObserveCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
			}
		}

		private void OnSelectHorizonCamp_2(CUIEvent evt)
		{
			if (evt.m_eventParams.togleIsOn)
			{
				this.SwitchObserveCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
			}
		}

		private void SwitchObserveCamp(COM_PLAYERCAMP camp)
		{
			Singleton<WatchController>.GetInstance().SwitchObserveCamp(camp);
			if (camp != COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT && (Singleton<WatchController>.GetInstance().HorizonCamp != camp || (this._focusHero && this._focusHero.handle.TheActorMeta.ActorCamp != camp)))
			{
				if (this._focusHero)
				{
					MonoSingleton<CameraSystem>.instance.MobaCamera.SetAbsoluteLockLocation((Vector3)this._focusHero.handle.location);
				}
				this.FreeFocus();
			}
		}

		private void SwitchCameraZoom()
		{
			if (Singleton<WatchController>.GetInstance().IsLiveCast)
			{
				Moba_Camera mobaCamera = MonoSingleton<CameraSystem>.GetInstance().MobaCamera;
				if (null != mobaCamera)
				{
					mobaCamera.currentZoomRate = ((mobaCamera.currentZoomRate > 1f) ? 1f : 2f);
					mobaCamera.CameraUpdate();
				}
			}
		}
	}
}
