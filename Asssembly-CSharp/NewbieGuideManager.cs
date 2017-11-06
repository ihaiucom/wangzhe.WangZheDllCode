using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

[MessageHandlerClass]
public class NewbieGuideManager : MonoSingleton<NewbieGuideManager>
{
	public static string FORM_5v5GUIDE_CONFIRM = "UGUI/Form/System/Newbie/Form_5v5Guide_Confirm.prefab";

	public static string FORM_3v3GUIDE_CONFIRM = "UGUI/Form/System/Newbie/Form_3v3Guide_Confirm.prefab";

	public static short WEAKGUIDE_BIT_OFFSET = 110;

	private Dictionary<uint, bool> mCompleteCacheDic;

	private Dictionary<uint, bool> mWeakCompleteCacheDic;

	private bool mIsInit;

	private bool m_IsCheckSkip;

	public bool bTimeOutSkip;

	public bool IsComplteNewbieGuide;

	private List<uint> mSingleBattleCompleteCacheDic;

	public bool newbieGuideEnable;

	private uint mCurrentNewbieGuideId;

	private NewbieGuideScriptControl mCurrentScriptControl;

	public static NewbieGuideScriptControl.AddScriptDelegate addScriptDelegate;

	private int hostDeadNum;

	private int lastChangeTime;

	private int lastRate = 100;

	private string logTitle
	{
		get
		{
			return "[<color=cyan>NewbieGuide</color>][<color=green>test</color>]";
		}
	}

	public uint currentNewbieGuideId
	{
		get
		{
			return this.mCurrentNewbieGuideId;
		}
	}

	public bool isNewbieGuiding
	{
		get
		{
			return this.currentNewbieGuideId != 0u;
		}
	}

	public void SetNewbieGuideState(uint id, bool bOpen)
	{
		if ((ulong)id > (ulong)((long)NewbieGuideManager.WEAKGUIDE_BIT_OFFSET) && id <= 128u)
		{
			this.mWeakCompleteCacheDic.set_Item((uint)((ulong)id - (ulong)((long)NewbieGuideManager.WEAKGUIDE_BIT_OFFSET)), bOpen);
		}
		else
		{
			this.mCompleteCacheDic.set_Item(id, bOpen);
		}
	}

	public static void CloseGuideForm()
	{
		Singleton<CUIManager>.GetInstance().CloseForm(NewbieGuideScriptControl.FormGuideMaskPath);
	}

	public void StopCurrentGuide()
	{
		if (null != this.mCurrentScriptControl)
		{
			this.mCurrentScriptControl.Stop();
			this.DestroyCurrentScriptControl();
		}
	}

	public bool CheckTriggerTime(NewbieGuideTriggerTimeType type, params uint[] param)
	{
		if (!this.newbieGuideEnable)
		{
			return false;
		}
		if (!this.m_IsCheckSkip)
		{
			return false;
		}
		if (this.currentNewbieGuideId == 0u)
		{
			ListView<NewbieGuideMainLineConf> newbieGuideMainLineConfListByTriggerTimeType = Singleton<NewbieGuideDataManager>.GetInstance().GetNewbieGuideMainLineConfListByTriggerTimeType(type, param);
			int count = newbieGuideMainLineConfListByTriggerTimeType.Count;
			for (int i = 0; i < count; i++)
			{
				NewbieGuideMainLineConf conf = newbieGuideMainLineConfListByTriggerTimeType[i];
				if (this.CheckTrigger(type, conf))
				{
					if (Singleton<NewbieWeakGuideControl>.instance.isGuiding)
					{
						Singleton<NewbieWeakGuideControl>.instance.RemoveAllEffect();
					}
					return true;
				}
			}
			ListView<NewbieWeakGuideMainLineConf> newBieGuideWeakMainLineConfListByTiggerTimeType = Singleton<NewbieGuideDataManager>.GetInstance().GetNewBieGuideWeakMainLineConfListByTiggerTimeType(type, param);
			count = newBieGuideWeakMainLineConfListByTiggerTimeType.Count;
			for (int j = 0; j < count; j++)
			{
				NewbieWeakGuideMainLineConf conf2 = newBieGuideWeakMainLineConfListByTiggerTimeType[j];
				if (this.TriggerWeakNewbieGuide(conf2, type, true))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static int ConvertNewbieBitToClientBit(uint newGuideBit)
	{
		if (newGuideBit >= 300u || newGuideBit <= 200u)
		{
			return 0;
		}
		return (int)(newGuideBit - 100u);
	}

	public static void CompleteAllNewbieGuide()
	{
		MonoSingleton<NewbieGuideManager>.GetInstance().ForceCompleteNewbieGuideAll(true, true, false);
		MonoSingleton<NewbieGuideManager>.GetInstance().ForceSetWeakGuideCompleteAll(true, true, false);
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (masterRoleInfo != null)
		{
			masterRoleInfo.SetGuidedStateSet(0, true);
			masterRoleInfo.GameDifficult = COM_ACNT_NEWBIE_TYPE.COM_ACNT_NEWBIE_TYPE_MASTER;
		}
		masterRoleInfo.SetNewbieAchieve(17, true, false);
		masterRoleInfo.SyncClientBitsToSvr();
		masterRoleInfo.SyncNewbieAchieveToSvr(false);
	}

	public static void ShowCompleteNewbieGuidePanel()
	{
		Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Old_Acnt_Choose_Complete_Text"), enUIEventID.Newbie_ClickCompleteNewbieGuide, enUIEventID.Newbie_ClickNotCompleteNewbieGuide, false);
	}

	protected override void Awake()
	{
		base.Awake();
		NewbieGuideManager.addScriptDelegate = new NewbieGuideScriptControl.AddScriptDelegate(NewbieGuideSctiptFactory.AddScript);
		DebugHelper.Assert(NewbieGuideManager.addScriptDelegate != null);
		this.mCompleteCacheDic = new Dictionary<uint, bool>();
		this.mWeakCompleteCacheDic = new Dictionary<uint, bool>();
		this.mSingleBattleCompleteCacheDic = new List<uint>();
		List<uint> mainLineIDList = Singleton<NewbieGuideDataManager>.GetInstance().GetMainLineIDList();
		int count = mainLineIDList.get_Count();
		for (int i = 0; i < count; i++)
		{
			this.mCompleteCacheDic.Add(mainLineIDList.get_Item(i), false);
		}
		List<uint> weakMianLineIDList = Singleton<NewbieGuideDataManager>.GetInstance().GetWeakMianLineIDList();
		count = weakMianLineIDList.get_Count();
		for (int j = 0; j < count; j++)
		{
			this.mWeakCompleteCacheDic.Add(weakMianLineIDList.get_Item(j), false);
		}
		this.newbieGuideEnable = true;
		this.bTimeOutSkip = true;
		this.Initialize();
	}

	public void Initialize()
	{
		Singleton<NewbieWeakGuideControl>.CreateInstance();
		this.mIsInit = true;
	}

	protected override void Init()
	{
		base.Init();
		Singleton<GameEventSys>.instance.AddEventHandler<TalentLevelChangeParam>(GameEventDef.Event_TalentLevelChange, new RefAction<TalentLevelChangeParam>(this.onTalentLevelChange));
		Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.OnFightStart));
		if (!this.IsBigMapSignGuideCompelte())
		{
			Singleton<GameEventSys>.GetInstance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.OnFightPrepare));
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
		}
		if (!this.IsCameraMoveGuideComplete())
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnCameraAxisPushed, new CUIEventManager.OnUIEventHandler(this.onMoveCamera));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnPanelCameraStartDrag, new CUIEventManager.OnUIEventHandler(this.onMoveCamera));
		}
		if (!this.IsBattleEquipGuideComplete())
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_Form_Close, new CUIEventManager.OnUIEventHandler(this.onBattleEuipFormClose));
		}
		if (!this.IsHurtByTowerGuideComplete())
		{
			Singleton<EventRouter>.GetInstance().AddEventHandler("NewbieHostPlayerInAndHitByTower", new Action(this.OnHostPlayerHitByTower));
		}
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_ClickCompleteNewbieGuide, new CUIEventManager.OnUIEventHandler(this.OnChooseCompletenewbieGuide));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_ClickNotCompleteNewbieGuide, new CUIEventManager.OnUIEventHandler(this.OnChooseNotCompleteNewbieGuide));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_BubbleTimeout, new CUIEventManager.OnUIEventHandler(this.OnWeakGuideBubbleTimeOut));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CommomBubbleTimeout, new CUIEventManager.OnUIEventHandler(this.OnBattleBubbleTimeout));
	}

	protected override void OnDestroy()
	{
		Singleton<GameEventSys>.instance.RmvEventHandler<TalentLevelChangeParam>(GameEventDef.Event_TalentLevelChange, new RefAction<TalentLevelChangeParam>(this.onTalentLevelChange));
		Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.OnFightPrepare));
		Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.OnFightStart));
		Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnCameraAxisPushed, new CUIEventManager.OnUIEventHandler(this.onMoveCamera));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnPanelCameraStartDrag, new CUIEventManager.OnUIEventHandler(this.onMoveCamera));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_ClickCompleteNewbieGuide, new CUIEventManager.OnUIEventHandler(this.OnChooseCompletenewbieGuide));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_ClickNotCompleteNewbieGuide, new CUIEventManager.OnUIEventHandler(this.OnChooseNotCompleteNewbieGuide));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_BubbleTimeout, new CUIEventManager.OnUIEventHandler(this.OnWeakGuideBubbleTimeOut));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_Form_Close, new CUIEventManager.OnUIEventHandler(this.onBattleEuipFormClose));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CommomBubbleTimeout, new CUIEventManager.OnUIEventHandler(this.OnBattleBubbleTimeout));
		Singleton<EventRouter>.GetInstance().RemoveEventHandler("NewbieHostPlayerInAndHitByTower", new Action(this.OnHostPlayerHitByTower));
		Singleton<NewbieWeakGuideControl>.DestroyInstance();
		base.OnDestroy();
	}

	private void onTalentLevelChange(ref TalentLevelChangeParam inParam)
	{
		if (Singleton<GamePlayerCenter>.instance.GetHostPlayer() == null)
		{
			return;
		}
		PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
		if (!captain)
		{
			return;
		}
		if (!inParam.src || inParam.src != captain)
		{
			return;
		}
		MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onTalentLevelChange, new uint[]
		{
			(uint)inParam.SoulLevel,
			(uint)inParam.TalentLevel
		});
	}

	private void OnFightPrepare(ref DefaultGameEventParam param)
	{
		this.hostDeadNum = 0;
	}

	private void OnFightStart(ref DefaultGameEventParam param)
	{
		if (!MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onBattleStart, new uint[0]) && Singleton<CBattleSystem>.GetInstance().FightForm != null)
		{
			Singleton<CBattleSystem>.GetInstance().FightForm.ShowVoiceTips();
		}
		SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
		if (curLvelContext != null && !this.IsLowHPGuideComplete() && curLvelContext.IsMobaMode())
		{
			ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = Singleton<GamePlayerCenter>.instance.GetHostPlayer().GetAllHeroes();
			DebugHelper.Assert(allHeroes.isValidReference, "newbie guide manager invalid all heros list.");
			if (allHeroes.isValidReference)
			{
				for (int i = 0; i < allHeroes.Count; i++)
				{
					if (allHeroes[i] && allHeroes[i].handle.ValueComponent != null)
					{
						allHeroes[i].handle.ValueComponent.HpChgEvent -= new ValueChangeDelegate(this.OnHostPlayerHPChange);
					}
				}
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer != null && hostPlayer.Captain && hostPlayer.Captain.handle.ValueComponent != null)
			{
				hostPlayer.Captain.handle.ValueComponent.HpChgEvent += new ValueChangeDelegate(this.OnHostPlayerHPChange);
			}
		}
	}

	private void onActorDead(ref GameDeadEventParam param)
	{
		if (param.src && ActorHelper.IsHostCtrlActor(ref param.src))
		{
			this.hostDeadNum++;
			if (this.hostDeadNum == 1)
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
				if (curLvelContext.m_mapID == 20011)
				{
					this.CheckTriggerTime(NewbieGuideTriggerTimeType.onHostFirstDead, new uint[0]);
				}
			}
		}
	}

	private void onMoveCamera(CUIEvent uiEvt)
	{
		Singleton<CBattleGuideManager>.GetInstance().CloseFormShared(CBattleGuideManager.EBattleGuideFormType.FingerMovement);
	}

	private void OnHostPlayerHPChange()
	{
		int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
		if (currentUTCTime - this.lastChangeTime < 2)
		{
			return;
		}
		this.lastChangeTime = currentUTCTime;
		Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
		if (hostPlayer == null || !hostPlayer.Captain || hostPlayer.Captain.handle.ActorControl == null || hostPlayer.Captain.handle.ValueComponent == null)
		{
			return;
		}
		int roundInt = (hostPlayer.Captain.handle.ValueComponent.GetHpRate() * 100L).roundInt;
		if (this.lastRate < roundInt)
		{
			this.lastRate = roundInt;
			return;
		}
		this.lastRate = roundInt;
		if (roundInt < 40)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.IsMobaModeWithOutGuide())
			{
				this.CheckTriggerTime(NewbieGuideTriggerTimeType.onHostPlayerLowHp, new uint[0]);
			}
		}
	}

	private void OnHostPlayerHitByTower()
	{
		SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
		if (curLvelContext != null && curLvelContext.IsMobaModeWithOutGuide())
		{
			this.CheckTriggerTime(NewbieGuideTriggerTimeType.onHostPlayerHitByTower, new uint[0]);
		}
	}

	public void CheckSkipIntoLobby()
	{
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasComplete33Guide, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasRewardTaskPvp, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasRewardTaskPve, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteHeroAdv, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteHeroStar, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasBoughtHero, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasGotChapterReward, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasAdvancedEquip, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasMopup, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasEnteredPvP, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasEnteredTrial, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasEnteredZhuangzi, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasEnteredBurning, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasEnteredElitePvE, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasEnteredGuild, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasUsedSymbol, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasEnteredMysteryShop, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteNewbieArchive, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteBaseGuide, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteHumanAi33Match, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteHuman33Match, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteHumanAi33, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasManufacuredSymbol, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasEnoughSymbolOf, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteLottery, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasIncreaseEquip, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteHeroUp, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasEnteredTournament, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasOverThreeHeroes, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteWeakNewbieArchive, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteDungeon, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteNewbieGuide, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteTrainLevel55, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasComplete11Match, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteTrainLevel33, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasDiamondDraw, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteFirst55Warm, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteCoronaGuide, new uint[0]);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCoinDrawFive, new uint[0]);
		this.m_IsCheckSkip = true;
	}

	public void CheckSkipCondition(NewbieGuideSkipConditionType type, params uint[] param)
	{
		this.CheckForceSkipCondition(type, param);
		this.CheckWeakSkipCondition(type, param);
	}

	private void CheckForceSkipCondition(NewbieGuideSkipConditionType type, params uint[] param)
	{
		ListView<NewbieGuideMainLineConf> newbieGuideMainLineConfListBySkipType = Singleton<NewbieGuideDataManager>.GetInstance().GetNewbieGuideMainLineConfListBySkipType(type);
		int count = newbieGuideMainLineConfListBySkipType.Count;
		for (int i = 0; i < count; i++)
		{
			NewbieGuideMainLineConf newbieGuideMainLineConf = newbieGuideMainLineConfListBySkipType[i];
			if (!this.IsMianLineComplete(newbieGuideMainLineConf.dwID))
			{
				for (int j = 0; j < newbieGuideMainLineConf.astSkipCondition.Length; j++)
				{
					if ((NewbieGuideSkipConditionType)newbieGuideMainLineConf.astSkipCondition[j].wType == type && NewbieGuideCheckSkipConditionUtil.CheckSkipCondition(newbieGuideMainLineConf.astSkipCondition[j], param))
					{
						if (!(null != this.mCurrentScriptControl))
						{
							this.SetNewbieGuideComplete(newbieGuideMainLineConf.dwID, false, false, true);
							break;
						}
						if (this.mCurrentScriptControl.currentMainLineId != newbieGuideMainLineConf.dwID)
						{
							this.SetNewbieGuideComplete(newbieGuideMainLineConf.dwID, false, false, true);
							break;
						}
					}
				}
			}
		}
	}

	private void CheckWeakSkipCondition(NewbieGuideSkipConditionType type, params uint[] param)
	{
		ListView<NewbieWeakGuideMainLineConf> newbieGuideWeakMianLineConfListBySkipType = Singleton<NewbieGuideDataManager>.GetInstance().GetNewbieGuideWeakMianLineConfListBySkipType(type);
		int count = newbieGuideWeakMianLineConfListBySkipType.Count;
		for (int i = 0; i < count; i++)
		{
			NewbieWeakGuideMainLineConf newbieWeakGuideMainLineConf = newbieGuideWeakMianLineConfListBySkipType[i];
			if (!this.IsWeakLineComplete(newbieWeakGuideMainLineConf.dwID))
			{
				for (int j = 0; j < newbieWeakGuideMainLineConf.astSkipCondition.Length; j++)
				{
					NewbieGuideSkipConditionItem newbieGuideSkipConditionItem = newbieWeakGuideMainLineConf.astSkipCondition[j];
					if ((NewbieGuideSkipConditionType)newbieGuideSkipConditionItem.wType == type && NewbieGuideCheckSkipConditionUtil.CheckSkipCondition(newbieGuideSkipConditionItem, param))
					{
						this.SetWeakGuideComplete(newbieWeakGuideMainLineConf.dwID, true, true);
						break;
					}
				}
			}
		}
	}

	public void CheckWeakGuideTrigger(NewbieGuideWeakGuideType type, params uint[] param)
	{
	}

	private bool CheckTrigger(NewbieGuideTriggerTimeType type, NewbieGuideMainLineConf conf)
	{
		if (this.CheckTriggerTime(conf) && this.CheckTriggerCondition(conf.dwID, conf.astTriggerCondition))
		{
			int startIndexByTriggerTime = this.GetStartIndexByTriggerTime(type, conf);
			this.TriggerNewbieGuide(conf, startIndexByTriggerTime);
			return true;
		}
		return false;
	}

	private int GetStartIndexByTriggerTime(NewbieGuideTriggerTimeType type, NewbieGuideMainLineConf conf)
	{
		int num = conf.astTriggerTime.Length;
		for (int i = 0; i < num; i++)
		{
			NewbieGuideTriggerTimeItem newbieGuideTriggerTimeItem = conf.astTriggerTime[i];
			if (type == (NewbieGuideTriggerTimeType)newbieGuideTriggerTimeItem.wType)
			{
				return (int)newbieGuideTriggerTimeItem.dwStartIndex;
			}
		}
		return 0;
	}

	private bool CheckTriggerTime(NewbieGuideMainLineConf conf)
	{
		if ((!this.IsMianLineComplete(conf.dwID) || conf.iSavePoint == -1) && !Singleton<WatchController>.instance.IsWatching)
		{
			bool flag = true;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				if (conf.wTriggerLevelUpperLimit > 0)
				{
					flag &= (masterRoleInfo.PvpLevel <= (uint)conf.wTriggerLevelUpperLimit);
				}
				if (conf.wTriggerLevelLowerLimit > 0)
				{
					flag &= (masterRoleInfo.PvpLevel >= (uint)conf.wTriggerLevelLowerLimit);
				}
			}
			return flag;
		}
		return false;
	}

	private bool CheckTriggerCondition(uint id, NewbieGuideTriggerConditionItem[] conditions)
	{
		int num = conditions.Length;
		for (int i = 0; i < num; i++)
		{
			NewbieGuideTriggerConditionItem newbieGuideTriggerConditionItem = conditions[i];
			if (newbieGuideTriggerConditionItem.wType != 0 && !NewbieGuideCheckTriggerConditionUtil.CheckTriggerCondition(id, newbieGuideTriggerConditionItem))
			{
				return false;
			}
		}
		return true;
	}

	private bool IsMianLineComplete(uint id)
	{
		bool flag;
		return this.mCompleteCacheDic.TryGetValue(id, ref flag) && flag;
	}

	private void TriggerNewbieGuide(NewbieGuideMainLineConf conf, int startIndex)
	{
		if (!Singleton<NetworkModule>.GetInstance().lobbySvr.connected && conf.bIndependentNet != 1)
		{
			return;
		}
		this.SetCurrentNewbieGuideId(conf.dwID);
		this.mCurrentScriptControl = base.gameObject.AddComponent<NewbieGuideScriptControl>();
		this.mCurrentScriptControl.CompleteEvent += new NewbieGuideScriptControl.NewbieGuideScriptControlDelegate(this.NewbieGuideCompleteHandler);
		this.mCurrentScriptControl.SaveEvent += new NewbieGuideScriptControl.NewbieGuideScriptControlDelegate(this.NewbieGuideSaveHandler);
		this.mCurrentScriptControl.SetData(conf.dwID, startIndex);
		this.mCurrentScriptControl.addScriptDelegate = NewbieGuideManager.addScriptDelegate;
	}

	private void NewbieGuideSaveHandler()
	{
		this.Save();
	}

	private void NewbieGuideCompleteHandler()
	{
		if (null != this.mCurrentScriptControl)
		{
			uint currentNewbieGuideId = this.currentNewbieGuideId;
			this.SetNewbieGuideComplete(this.currentNewbieGuideId, true, true, true);
			NewbieGuideMainLineConf newbieGuideMainLineConf = Singleton<NewbieGuideDataManager>.GetInstance().GetNewbieGuideMainLineConf(currentNewbieGuideId);
			if (newbieGuideMainLineConf != null)
			{
				int num = newbieGuideMainLineConf.astSetCompleteId.Length;
				for (int i = 0; i < num; i++)
				{
					if (newbieGuideMainLineConf.astSetCompleteId[i].dwID != 0u)
					{
						uint dwID = newbieGuideMainLineConf.astSetCompleteId[i].dwID;
						if ((ulong)dwID <= (ulong)((long)NewbieGuideManager.WEAKGUIDE_BIT_OFFSET))
						{
							this.SetNewbieGuideComplete(dwID, false, true, true);
						}
						else
						{
							this.SetWeakGuideComplete((uint)((ulong)dwID - (ulong)((long)NewbieGuideManager.WEAKGUIDE_BIT_OFFSET)), true, true);
						}
					}
				}
			}
			this.AddSingleBattleCompleteID(currentNewbieGuideId);
			this.DestroyCurrentScriptControl();
			this.CheckTriggerTime(NewbieGuideTriggerTimeType.preNewbieGuideComplete, new uint[]
			{
				currentNewbieGuideId
			});
		}
	}

	public void SetNewbieGuideComplete(uint id, bool reset, bool bNormalComplete = false, bool bSync = true)
	{
		bool flag = this.mCompleteCacheDic.get_Item(id);
		if (!bNormalComplete)
		{
			this.mCompleteCacheDic.set_Item(id, true);
		}
		else if (!(this.mCurrentScriptControl != null) || this.mCurrentScriptControl.savePoint >= 0)
		{
			this.mCompleteCacheDic.set_Item(id, true);
		}
		if (reset)
		{
			this.SetCurrentNewbieGuideId(0u);
		}
		if (!flag && this.mCompleteCacheDic.get_Item(id))
		{
			this.SetNewbieBit((int)id, true, bSync);
			this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteNewbieGuide, new uint[]
			{
				id
			});
		}
	}

	public bool IsNewbieGuideComplete(uint id)
	{
		bool result;
		if (this.mCompleteCacheDic.TryGetValue(id, ref result))
		{
			return result;
		}
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (masterRoleInfo == null)
		{
			return false;
		}
		if (id >= 290u && id < 300u)
		{
			return masterRoleInfo.IsClientBitsSet(NewbieGuideManager.ConvertNewbieBitToClientBit(id));
		}
		return id >= 64u && id < 128u && masterRoleInfo.IsNewbieAchieveSet((int)id);
	}

	public void ForceCompleteNewbieGuide()
	{
		if (this.mCompleteCacheDic.ContainsKey(this.mCurrentNewbieGuideId) && !this.mCompleteCacheDic.get_Item(this.mCurrentNewbieGuideId))
		{
			NewbieGuideMainLineConf newbieGuideMainLineConf = Singleton<NewbieGuideDataManager>.GetInstance().GetNewbieGuideMainLineConf(this.mCurrentNewbieGuideId);
			if (newbieGuideMainLineConf != null)
			{
				int num = newbieGuideMainLineConf.astSetCompleteId.Length;
				for (int i = 0; i < num; i++)
				{
					if (newbieGuideMainLineConf.astSetCompleteId[i].dwID != 0u)
					{
						uint dwID = newbieGuideMainLineConf.astSetCompleteId[i].dwID;
						if ((ulong)dwID <= (ulong)((long)NewbieGuideManager.WEAKGUIDE_BIT_OFFSET))
						{
							this.SetNewbieGuideComplete(dwID, false, true, true);
						}
						else
						{
							this.SetWeakGuideComplete((uint)((ulong)dwID - (ulong)((long)NewbieGuideManager.WEAKGUIDE_BIT_OFFSET)), true, true);
						}
					}
				}
			}
			if (newbieGuideMainLineConf != null)
			{
				this.SetNewbieGuideComplete(this.mCurrentNewbieGuideId, true, false, true);
			}
		}
		if (null != this.mCurrentScriptControl)
		{
			this.mCurrentScriptControl.Stop();
			this.DestroyCurrentScriptControl();
		}
	}

	public void ForceCompleteNewbieGuideAll(bool bReset, bool setOldPlayerBit = false, bool sync = true)
	{
		if (null != this.mCurrentScriptControl)
		{
			this.mCurrentScriptControl.Stop();
			this.DestroyCurrentScriptControl();
		}
		List<uint> list = new List<uint>();
		Dictionary<uint, bool>.KeyCollection.Enumerator enumerator = this.mCompleteCacheDic.get_Keys().GetEnumerator();
		while (enumerator.MoveNext())
		{
			uint current = enumerator.get_Current();
			if (!this.mCompleteCacheDic.get_Item(current))
			{
				NewbieGuideMainLineConf newbieGuideMainLineConf = Singleton<NewbieGuideDataManager>.GetInstance().GetNewbieGuideMainLineConf(current);
				if (newbieGuideMainLineConf != null && newbieGuideMainLineConf.bOldPlayerGuide != 1)
				{
					list.Add(newbieGuideMainLineConf.dwID);
				}
				if (setOldPlayerBit && newbieGuideMainLineConf != null && newbieGuideMainLineConf.bOldPlayerGuide == 1)
				{
					list.Add(newbieGuideMainLineConf.dwID);
				}
			}
		}
		List<uint>.Enumerator enumerator2 = list.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			this.SetNewbieGuideComplete(enumerator2.get_Current(), bReset, false, sync);
		}
	}

	private bool CheckLevelLimit(NewbieWeakGuideMainLineConf conf)
	{
		if (conf.wTriggerLevelUpperLimit == 0)
		{
			return true;
		}
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		return masterRoleInfo == null || masterRoleInfo.PvpLevel <= (uint)conf.wTriggerLevelUpperLimit;
	}

	private bool CheckLevelLimitLower(NewbieWeakGuideMainLineConf conf)
	{
		if (conf.wTriggerLevelLowerLimit == 0)
		{
			return true;
		}
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		return masterRoleInfo == null || masterRoleInfo.PvpLevel >= (uint)conf.wTriggerLevelLowerLimit;
	}

	public bool IsWeakLineComplete(uint lineId)
	{
		bool result;
		if (this.mWeakCompleteCacheDic.TryGetValue(lineId, ref result))
		{
			return result;
		}
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		return masterRoleInfo != null && masterRoleInfo.IsNewbieAchieveSet((int)(lineId + (uint)NewbieGuideManager.WEAKGUIDE_BIT_OFFSET));
	}

	public bool TriggerWeakNewbieGuide(NewbieWeakGuideMainLineConf conf, NewbieGuideTriggerTimeType type, bool checkCondition)
	{
		if (conf != null && !this.IsWeakLineComplete(conf.dwID) && !Singleton<WatchController>.instance.IsWatching && (!checkCondition || (this.CheckLevelLimit(conf) && this.CheckLevelLimitLower(conf) && this.CheckTriggerCondition(0u, conf.astTriggerCondition))))
		{
			uint weakStartIndexByMianLineConf = this.GetWeakStartIndexByMianLineConf(type, conf);
			return this.TriggerWeakNewbieGuide(conf.dwID, weakStartIndexByMianLineConf);
		}
		return false;
	}

	public void SetWeakGuideComplete(uint lineId, bool reset = true, bool sync = true)
	{
		bool flag;
		if (this.mWeakCompleteCacheDic.TryGetValue(lineId, ref flag))
		{
			this.mWeakCompleteCacheDic.set_Item(lineId, true);
		}
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		this.SetNewbieBit((int)(lineId + (uint)NewbieGuideManager.WEAKGUIDE_BIT_OFFSET), true, sync);
		this.CheckSkipCondition(NewbieGuideSkipConditionType.hasCompleteNewbieGuide, new uint[]
		{
			lineId
		});
	}

	public void ForceSetWeakGuideCompleteAll(bool bReset, bool setOldPlayerBit = false, bool sync = true)
	{
		List<uint> list = new List<uint>();
		Dictionary<uint, bool>.KeyCollection.Enumerator enumerator = this.mWeakCompleteCacheDic.get_Keys().GetEnumerator();
		while (enumerator.MoveNext())
		{
			uint current = enumerator.get_Current();
			if (!this.mWeakCompleteCacheDic.get_Item(current))
			{
				NewbieWeakGuideMainLineConf newbieWeakGuideMainLineConf = Singleton<NewbieGuideDataManager>.GetInstance().GetNewbieWeakGuideMainLineConf(current);
				if (newbieWeakGuideMainLineConf != null && newbieWeakGuideMainLineConf.bOnlyOnce == 1 && newbieWeakGuideMainLineConf.bOldPlayerGuide != 1)
				{
					list.Add(newbieWeakGuideMainLineConf.dwID);
				}
				if (setOldPlayerBit && newbieWeakGuideMainLineConf != null && newbieWeakGuideMainLineConf.bOldPlayerGuide == 1)
				{
					list.Add(newbieWeakGuideMainLineConf.dwID);
				}
			}
		}
		for (int i = 0; i < list.get_Count(); i++)
		{
			MonoSingleton<NewbieGuideManager>.GetInstance().SetWeakGuideComplete(list.get_Item(i), bReset, sync);
		}
	}

	private bool TriggerWeakNewbieGuide(uint weakLineId, uint startIndex)
	{
		return Singleton<NewbieWeakGuideControl>.GetInstance().TriggerWeakGuide(weakLineId, startIndex);
	}

	private uint GetWeakStartIndexByMianLineConf(NewbieGuideTriggerTimeType type, NewbieWeakGuideMainLineConf conf)
	{
		uint result = 1u;
		int num = conf.astTriggerTime.Length;
		for (int i = 0; i < num; i++)
		{
			NewbieGuideTriggerTimeItem newbieGuideTriggerTimeItem = conf.astTriggerTime[i];
			if ((NewbieGuideTriggerTimeType)newbieGuideTriggerTimeItem.wType == type && newbieGuideTriggerTimeItem.dwStartIndex > 0u)
			{
				result = newbieGuideTriggerTimeItem.dwStartIndex;
			}
		}
		return result;
	}

	public void ForceCompleteWeakGuide()
	{
		if (!this.newbieGuideEnable)
		{
			return;
		}
		Singleton<NewbieWeakGuideControl>.GetInstance().ForceCompleteWeakGuide();
	}

	private bool IsBigMapSignGuideCompelte()
	{
		bool result = false;
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (masterRoleInfo != null && masterRoleInfo.IsNewbieAchieveSet(76) && masterRoleInfo.IsNewbieAchieveSet(77))
		{
			result = true;
		}
		return result;
	}

	private bool IsCameraMoveGuideComplete()
	{
		bool result = false;
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (masterRoleInfo != null && masterRoleInfo.IsNewbieAchieveSet(64) && masterRoleInfo.IsNewbieAchieveSet(65) && masterRoleInfo.IsNewbieAchieveSet(66))
		{
			result = true;
		}
		return result;
	}

	private bool IsBattleEquipGuideComplete()
	{
		bool result = false;
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (masterRoleInfo != null && masterRoleInfo.IsClientBitsSet(6))
		{
			result = true;
		}
		return result;
	}

	private bool IsLowHPGuideComplete()
	{
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		return masterRoleInfo == null || masterRoleInfo.acntMobaInfo.iMobaLevel != 0 || masterRoleInfo.PvpLevel > 5u;
	}

	private bool IsHurtByTowerGuideComplete()
	{
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		return masterRoleInfo == null || masterRoleInfo.acntMobaInfo.iMobaLevel != 0 || masterRoleInfo.PvpLevel > 5u;
	}

	public void SetNewbieBit(int inIndex, bool bOpen, bool bSync = false)
	{
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (masterRoleInfo == null)
		{
			return;
		}
		if (inIndex >= 290 && inIndex < 300)
		{
			masterRoleInfo.SetClientBits(NewbieGuideManager.ConvertNewbieBitToClientBit((uint)inIndex), bOpen, bSync);
		}
		else if (inIndex >= 64 && inIndex < 128)
		{
			masterRoleInfo.SetNewbieAchieve(inIndex, bOpen, bSync);
		}
	}

	public bool IsNewbieBitSet(int inIndex)
	{
		bool result = false;
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (masterRoleInfo == null)
		{
			return result;
		}
		if (inIndex >= 290 && inIndex < 300)
		{
			result = masterRoleInfo.IsClientBitsSet(NewbieGuideManager.ConvertNewbieBitToClientBit((uint)inIndex));
		}
		else if (inIndex >= 64 && inIndex < 128)
		{
			result = masterRoleInfo.IsNewbieAchieveSet(inIndex);
		}
		return result;
	}

	private void SetCurrentNewbieGuideId(uint id)
	{
		this.mCurrentNewbieGuideId = id;
	}

	private void DestroyCurrentScriptControl()
	{
		this.mCurrentScriptControl.CompleteEvent -= new NewbieGuideScriptControl.NewbieGuideScriptControlDelegate(this.NewbieGuideCompleteHandler);
		this.mCurrentScriptControl.SaveEvent -= new NewbieGuideScriptControl.NewbieGuideScriptControlDelegate(this.NewbieGuideSaveHandler);
		this.mCurrentScriptControl.addScriptDelegate = null;
		Object.Destroy(this.mCurrentScriptControl);
		this.mCurrentScriptControl = null;
		this.mCurrentNewbieGuideId = 0u;
	}

	public void ClearSingleBattleCache()
	{
		this.mSingleBattleCompleteCacheDic.Clear();
	}

	private void AddSingleBattleCompleteID(uint id)
	{
		this.mSingleBattleCompleteCacheDic.Add(id);
	}

	public bool HasSingleBattleComplete(uint id)
	{
		return this.mSingleBattleCompleteCacheDic.Contains(id);
	}

	private void Save()
	{
		if (this.mIsInit && null != this.mCurrentScriptControl && this.mCurrentScriptControl.CheckSavePoint())
		{
			this.SetNewbieGuideComplete(this.mCurrentNewbieGuideId, false, false, true);
		}
	}

	private void OnChooseCompletenewbieGuide(CUIEvent uiEvent)
	{
		this.IsComplteNewbieGuide = true;
		this.CommitIsCompleteNewbieGuide(true);
	}

	private void OnChooseNotCompleteNewbieGuide(CUIEvent uiEvent)
	{
		this.IsComplteNewbieGuide = false;
		this.CommitIsCompleteNewbieGuide(false);
	}

	private void OnWeakGuideBubbleTimeOut(CUIEvent uiEvent)
	{
		Singleton<NewbieWeakGuideControl>.GetInstance().RemoveEffectByHilight(uiEvent.m_srcWidget);
	}

	private void OnBattleBubbleTimeout(CUIEvent uiEvent)
	{
		uiEvent.m_srcWidget.transform.parent.gameObject.CustomSetActive(false);
	}

	private void onBattleEuipFormClose(CUIEvent uiEvent)
	{
		if (!this.IsBattleEquipGuideComplete())
		{
			CSkillButtonManager cSkillButtonManager = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
			if (cSkillButtonManager != null)
			{
				SkillButton button = cSkillButtonManager.GetButton(SkillSlotType.SLOT_SKILL_9);
				PlayerKDA hostKDA = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetHostKDA();
				if (hostKDA == null)
				{
					return;
				}
				bool flag = false;
				ListView<HeroKDA>.Enumerator enumerator = hostKDA.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Equips.Length > 0)
					{
						flag = true;
						break;
					}
				}
				if (button != null && button.m_button.activeSelf && flag)
				{
					CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_skillBtnFormPath);
					if (form != null)
					{
						Transform transform = form.GetWidget(27).transform.FindChild("Panel_Guide");
						if (transform != null)
						{
							transform.gameObject.CustomSetActive(true);
							CUITimerScript component = transform.FindChild("Timer").GetComponent<CUITimerScript>();
							component.ResetTime();
							component.ReStartTimer();
							CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
							if (masterRoleInfo != null)
							{
								masterRoleInfo.SetClientBits(6, true, true);
							}
						}
					}
				}
			}
		}
	}

	[MessageHandler(1192)]
	public static void OnNTFNewieAllBitSyn(CSPkg msg)
	{
		SCPKG_NTF_NEWIEALLBITSYN stNewieAllBitSyn = msg.stPkgData.stNewieAllBitSyn;
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (masterRoleInfo != null)
		{
			masterRoleInfo.InitGuidedStateBits(stNewieAllBitSyn.stNewbieBits);
		}
	}

	private void CommitIsCompleteNewbieGuide(bool IsComplete)
	{
		CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5242u);
		cSPkg.stPkgData.stAcntSetOldType.bYes = (IsComplete ? 1 : 0);
		Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, IsComplete);
	}
}
