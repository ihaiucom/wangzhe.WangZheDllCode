using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class BurnExpeditionController : Singleton<BurnExpeditionController>
	{
		public class BurnExpedition_EventID
		{
			public const string Burn_GET_BURNING_PROGRESS_RSP = "Burn_GET_BURNING_PROGRESS_RSP";

			public const string Burn_GET_BURNING_REWARD_RSP = "Burn_GET_BURNING_REWARD_RSP";

			public const string Burn_RESET_BURNING_PROGRESS_RSP = "Burn_RESET_BURNING_PROGRESS_RSP";

			public const string Burn_PROGRESS_Change = "Burn_PROGRESS_Change";

			public const string Burn_REWARD_Change = "Burn_REWARD_Change";

			public const string Burn_RESET_PROGRESS_Change = "Burn_RESET_PROGRESS_Change";

			public const string Burn_Settle = "Burn_Settle";
		}

		private static int burn_info_key = 4;

		public static int Max_Level_Index = 5;

		public static string Map_FormPath = "UGUI/Form/System/BurnExpedition/Form_Burn_Box.prefab";

		public static string Box_FormPath = "UGUI/Form/System/BurnExpedition/Form_Check_Box.prefab";

		public BurnExpeditionModel model;

		public BurnExpeditionView view;

		public BurnExpeditionController()
		{
			this.model = new BurnExpeditionModel();
			this.view = new BurnExpeditionView();
		}

		public override void Init()
		{
			base.Init();
			this.Register_Event();
		}

		public void Clear()
		{
			if (this.view != null)
			{
				this.view.Clear();
			}
		}

		public void ClearAll()
		{
			if (this.model != null)
			{
				this.model.ClearAll();
			}
		}

		public void Register_Event()
		{
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Burn_GET_BURNING_PROGRESS_RSP", new Action<CSPkg>(this.On_GET_BURNING_PROGRESS_RSP));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Burn_GET_BURNING_REWARD_RSP", new Action<CSPkg>(this.On_GET_BURNING_REWARD_RSP));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Burn_RESET_BURNING_PROGRESS_RSP", new Action<CSPkg>(this.On_RESET_BURNING_PROGRESS_RSP));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Burn_PROGRESS_Change", new Action(this.On_Burn_PROGRESS_Change));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Burn_OpenForm, new CUIEventManager.OnUIEventHandler(this.On_Burn_OpenForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Burn_Reset, new CUIEventManager.OnUIEventHandler(this.On_Burn_Reset));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Burn_Reset_Ok, new CUIEventManager.OnUIEventHandler(this.On_Burn_Reset_OK));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Burn_Challenge, new CUIEventManager.OnUIEventHandler(this.On_Burn_Challenge));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Burn_CloseEnemyInfo, new CUIEventManager.OnUIEventHandler(this.On_Burn_CloseEnemyInfo));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Burn_LevelButton, new CUIEventManager.OnUIEventHandler(this.On_Burn_LevelButton));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Burn_BoxButton, new CUIEventManager.OnUIEventHandler(this.On_Burn_BoxButton));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Burn_Return, new CUIEventManager.OnUIEventHandler(this.On_Burn_Return));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Burn_BuffClick, new CUIEventManager.OnUIEventHandler(this.On_Burn_BuffClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Burn_GotoShop, new CUIEventManager.OnUIEventHandler(this.On_Burn_GotoShop));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Burn_Info_Open, new CUIEventManager.OnUIEventHandler(this.On_Burn_Info_Open));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Burn_OnCloseForm, new CUIEventManager.OnUIEventHandler(this.On_Burn_CloseForm));
			Singleton<GameEventSys>.GetInstance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.OnFightPrepare));
		}

		private void OnFightPrepare(ref DefaultGameEventParam prm)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null && (curLvelContext.IsGameTypeBurning() || curLvelContext.IsGameTypeArena()))
			{
				Singleton<CUILoadingSystem>.GetInstance().HideLoading();
			}
		}

		private void On_RESET_BURNING_PROGRESS_RSP(CSPkg obj)
		{
			SCPKG_RESET_BURNING_PROGRESS_RSP stResetBurningProgressRsp = obj.stPkgData.stResetBurningProgressRsp;
			if (stResetBurningProgressRsp.iErrCode == 0)
			{
				this.model.RandomRobotIcon();
				this.model.Reset_Data();
				this.model.SetLevelDetail(this.model.curDifficultyType, stResetBurningProgressRsp.stNewProgress.stOfSucc);
				this.model.CalcProgress();
				Singleton<EventRouter>.GetInstance().BroadCastEvent("Burn_PROGRESS_Change");
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(UT.GetText("Burn_Error_Reset"), stResetBurningProgressRsp.iErrCode), false);
			}
		}

		private void On_GET_BURNING_REWARD_RSP(CSPkg msg)
		{
			SCPKG_GET_BURNING_REWARD_RSP stGetBurningRewardRsp = msg.stPkgData.stGetBurningRewardRsp;
			if (stGetBurningRewardRsp.iErrCode == 0)
			{
				if (stGetBurningRewardRsp.stRewardDetail.stOfSucc.bNextLevelNo != 0)
				{
					this.model.Set_ENEMY_TEAM_INFO(this.model.curDifficultyType, (int)(stGetBurningRewardRsp.stRewardDetail.stOfSucc.bNextLevelNo - 1), stGetBurningRewardRsp.stRewardDetail.stOfSucc.stNextEnemyInfo);
				}
				ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(stGetBurningRewardRsp.stRewardDetail.stOfSucc.stReward);
				for (int i = 0; i < useableListFromReward.Count; i++)
				{
					useableListFromReward[i].SetMultiple(ref stGetBurningRewardRsp.stRewardDetail.stOfSucc.stMultipleDetail, true);
				}
				Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(useableListFromReward), null, false, enUIEventID.None, false, false, "Form_Award");
				this.model.FinishBox(this.model.curSelect_BoxIndex);
				if (this.model.curSelect_BoxIndex <= BurnExpeditionController.Max_Level_Index - 1)
				{
					this.model.UnLockLevel(this.model.curSelect_BoxIndex + 1);
					if (this.view != null)
					{
						this.view.Show_Line(this.model.curSelect_BoxIndex + 1);
					}
				}
				this.model.CalcProgress();
				if (this.view != null)
				{
					this.view.Show_Map();
				}
			}
			else if (stGetBurningRewardRsp.iErrCode != 12)
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(UT.GetText("Burn_Error_GetAward"), stGetBurningRewardRsp.iErrCode), false);
			}
		}

		private void On_GET_BURNING_PROGRESS_RSP(CSPkg obj)
		{
			SCPKG_GET_BURNING_PROGRESS_RSP stGetBurningProgressRsp = obj.stPkgData.stGetBurningProgressRsp;
			if (stGetBurningProgressRsp.iErrCode == 0)
			{
				if (this.model != null)
				{
					this.model.RandomRobotIcon();
					if (stGetBurningProgressRsp.stBurningProgress != null && this.model != null)
					{
						this.model.SetProgress(stGetBurningProgressRsp.stBurningProgress);
					}
					Singleton<EventRouter>.GetInstance().BroadCastEvent("Burn_PROGRESS_Change");
				}
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(UT.GetText("Burn_Error_Progress"), stGetBurningProgressRsp.iErrCode), false);
			}
		}

		private void On_Burn_PROGRESS_Change()
		{
			if (this.view != null)
			{
				this.view.Show_Map();
			}
		}

		private void On_Burn_OpenForm(CUIEvent uievent)
		{
			if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_LIUGUOYUANZHENG))
			{
				if (!Singleton<SCModuleControl>.instance.GetActiveModule(COM_CLIENT_PLAY_TYPE.COM_CLIENT_PLAY_BURNING))
				{
					Singleton<CUIManager>.instance.OpenMessageBox(Singleton<SCModuleControl>.instance.PvpAndPvpOffTips, false);
					return;
				}
				if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
				{
					Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
					return;
				}
				MonoSingleton<NewbieGuideManager>.GetInstance().SetNewbieBit(16, true, false);
				if (this.model._data == null)
				{
					BurnExpeditionNetCore.Send_Get_BURNING_PROGRESS_REQ();
				}
				if (this.view != null)
				{
					this.view.OpenForm();
				}
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEnterBurn, new uint[0]);
			}
			else
			{
				ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(4u);
				Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
			}
		}

		private void On_Burn_CloseEnemyInfo(CUIEvent uievent)
		{
			if (this.view != null)
			{
				this.view.SetEnemyNodeShow(false);
			}
		}

		private void On_Burn_Challenge(CUIEvent uievent)
		{
			CSDT_SINGLE_GAME_OF_BURNING reportInfo = BurnExpeditionUT.Create_CSDT_SINGLE_GAME_OF_BURNING(this.model.curSelect_LevelIndex);
			ResLevelCfgInfo resLevelCfgInfo = BurnExpeditionUT.Get_LevelConfigInfo(Singleton<BurnExpeditionController>.instance.model.curSelect_LevelIndex);
			DebugHelper.Assert(resLevelCfgInfo != null);
			if (resLevelCfgInfo == null)
			{
				return;
			}
			byte mapHeroMaxCount = (byte)resLevelCfgInfo.iHeroNum;
			uint dwBattleListID = resLevelCfgInfo.dwBattleListID;
			Singleton<CHeroSelectBaseSystem>.instance.SetPVEDataWithBurnExpedition(dwBattleListID, reportInfo, StringHelper.UTF8BytesToString(ref resLevelCfgInfo.szName));
			Singleton<CHeroSelectBaseSystem>.instance.OpenForm(enSelectGameType.enBurning, mapHeroMaxCount, 0u, 0, 0);
		}

		private void On_Burn_Reset(CUIEvent uievent)
		{
			int num = this.model.Get_ResetNum(this.model.curDifficultyType);
			if (num > 0)
			{
				Singleton<CUIManager>.instance.OpenMessageBoxWithCancel(UT.GetText("Burn_Buy_Count_Tip"), enUIEventID.Burn_Reset_Ok, enUIEventID.Burn_Reset_Cancel, false);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(UT.GetText("Burn_Error_OutOfCount"), false);
			}
		}

		private void On_Burn_Reset_OK(CUIEvent evt)
		{
			BurnExpeditionNetCore.Send_RESET_BURNING_PROGRESS_REQ((byte)this.model.curDifficultyType);
		}

		private void On_Burn_Return(CUIEvent uievent)
		{
			if (this.view != null)
			{
				this.view.Clear();
			}
		}

		private void On_Burn_BoxButton(CUIEvent uievent)
		{
			this.model.curSelect_BoxIndex = BurnExpeditionUT.GetIndex(uievent.m_srcWidget.name) - 1;
			byte levelNo = this.model.Get_LevelNo(this.model.curSelect_BoxIndex);
			int levelID = this.model.Get_LevelID(this.model.curSelect_BoxIndex);
			bool activeSelf = uievent.m_srcWidget.transform.FindChild("current_node").gameObject.activeSelf;
			if (activeSelf)
			{
				uint goldNum;
				uint burn_num;
				bool flag = this.model.Get_Box_Info(Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().Level, this.model.curSelect_BoxIndex, out goldNum, out burn_num);
				if (flag)
				{
					this.view.Check_Box_Info(goldNum, burn_num);
				}
			}
			else if (this.model.Get_ChestRewardStatus(this.model.curSelect_BoxIndex) == COM_LEVEL_STATUS.COM_LEVEL_STATUS_UNLOCKED)
			{
				BurnExpeditionNetCore.Send_GET_BURNING_REWARD_REQ(levelNo, levelID);
			}
		}

		private void On_Burn_LevelButton(CUIEvent uievent)
		{
			int num = BurnExpeditionUT.GetIndex(uievent.m_srcWidget.name) - 1;
			if (this.model.Get_LevelStatus(num) != COM_LEVEL_STATUS.COM_LEVEL_STATUS_UNLOCKED)
			{
				return;
			}
			this.model.curSelect_LevelIndex = num;
			if (this.view != null)
			{
				this.view.Show_ENEMY(this.model.curSelect_LevelIndex);
			}
		}

		private void On_Burn_BuffClick(CUIEvent uievent)
		{
			int index = BurnExpeditionUT.GetIndex(uievent.m_srcWidget.name);
			if (this.model.curSelect_BuffIndex != index)
			{
				this.model.curSelect_BuffIndex = index;
				if (this.view != null)
				{
					this.view._Show_Buff_Selected_Index(index);
				}
			}
		}

		private void On_Burn_GotoShop(CUIEvent uievent)
		{
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Shop_OpenBurningShop;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		private void On_Burn_Info_Open(CUIEvent uievent)
		{
			Singleton<CUIManager>.GetInstance().OpenInfoForm(BurnExpeditionController.burn_info_key);
		}

		private void On_Burn_CloseForm(CUIEvent uievent)
		{
			CExploreView.RefreshExploreList();
		}
	}
}
