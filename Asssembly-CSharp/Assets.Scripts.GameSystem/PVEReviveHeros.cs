using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class PVEReviveHeros : Singleton<PVEReviveHeros>
	{
		public delegate void SetTriggerAllDeathResult(bool bReviveOk);

		private const int m_ReviveNumMax = 3;

		private const int m_iMaxBuffCount = 3;

		private static int m_iRemainReviveNum = 3;

		public uint[] m_WishBuffId = new uint[3];

		public BUFF_ITEM m_iSelectedBuffItem;

		public static readonly string PATH_REVIVEHERO = "UGUI/Form/System/PvE/Settle/Form_ReviveHero.prefab";

		private CUIFormScript m_form;

		private int m_CurDifficulType;

		private int m_iLevelID;

		private enPayType m_relivePayType;

		private uint m_relivePayValue;

		private int m_iTimer = -1;

		public static event PVEReviveHeros.SetTriggerAllDeathResult SetReviveResult
		{
			[MethodImpl(32)]
			add
			{
				PVEReviveHeros.SetReviveResult = (PVEReviveHeros.SetTriggerAllDeathResult)Delegate.Combine(PVEReviveHeros.SetReviveResult, value);
			}
			[MethodImpl(32)]
			remove
			{
				PVEReviveHeros.SetReviveResult = (PVEReviveHeros.SetTriggerAllDeathResult)Delegate.Remove(PVEReviveHeros.SetReviveResult, value);
			}
		}

		public int CurReviveNum
		{
			get
			{
				return 3 - PVEReviveHeros.m_iRemainReviveNum + 1;
			}
		}

		public int CurDifficulType
		{
			get
			{
				return this.m_CurDifficulType;
			}
		}

		public int CurLevelId
		{
			get
			{
				return this.m_iLevelID;
			}
		}

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReviveHero_OnSelectBuff0, new CUIEventManager.OnUIEventHandler(this.OnSelectBuffItem0));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReviveHero_OnSelectBuff1, new CUIEventManager.OnUIEventHandler(this.OnSelectBuffItem1));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReviveHero_OnSelectBuff2, new CUIEventManager.OnUIEventHandler(this.OnSelectBuffItem2));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReviveHero_OnReviveBtnClick, new CUIEventManager.OnUIEventHandler(this.OnClickReviveBtn));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReviveHero_OnExitBtnClick, new CUIEventManager.OnUIEventHandler(this.OnClickExitBtn));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReviveHero_OnConfirmRevive, new CUIEventManager.OnUIEventHandler(this.OnConfirmRevive));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReviveHero_OnReviveFailed, new CUIEventManager.OnUIEventHandler(this.OnReviveHerosFailed));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReviveHero_OnReviveTimeout, new CUIEventManager.OnUIEventHandler(this.OnReviveTimeout));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_BuyDianQuanPanelClose, new CUIEventManager.OnUIEventHandler(this.OnBuyDianQuanPanelClose));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_GameEnd, new RefAction<DefaultGameEventParam>(this.OnGameEnd));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.OnFightStart));
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnSelectBuff0, new CUIEventManager.OnUIEventHandler(this.OnSelectBuffItem0));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnSelectBuff1, new CUIEventManager.OnUIEventHandler(this.OnSelectBuffItem1));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnSelectBuff2, new CUIEventManager.OnUIEventHandler(this.OnSelectBuffItem2));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnReviveBtnClick, new CUIEventManager.OnUIEventHandler(this.OnClickReviveBtn));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnExitBtnClick, new CUIEventManager.OnUIEventHandler(this.OnClickExitBtn));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnConfirmRevive, new CUIEventManager.OnUIEventHandler(this.OnConfirmRevive));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnReviveFailed, new CUIEventManager.OnUIEventHandler(this.OnReviveHerosFailed));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnReviveTimeout, new CUIEventManager.OnUIEventHandler(this.OnReviveTimeout));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_BuyDianQuanPanelClose, new CUIEventManager.OnUIEventHandler(this.OnBuyDianQuanPanelClose));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_GameEnd, new RefAction<DefaultGameEventParam>(this.OnGameEnd));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.OnFightStart));
		}

		private void ResetReviveCondition()
		{
			PVEReviveHeros.m_iRemainReviveNum = 3;
		}

		public bool CheckAndPopReviveForm(PVEReviveHeros.SetTriggerAllDeathResult SetReviveResultFunc, bool bDelayOpenForm = true)
		{
			if (!this.IsCareCondition())
			{
				return false;
			}
			if (PVEReviveHeros.m_iRemainReviveNum <= 0)
			{
				return false;
			}
			if (SetReviveResultFunc == null)
			{
				return false;
			}
			PVEReviveHeros.SetReviveResult = SetReviveResultFunc;
			if (!bDelayOpenForm)
			{
				this.m_form = Singleton<CUIManager>.GetInstance().OpenForm(PVEReviveHeros.PATH_REVIVEHERO, false, true);
				if (this.m_form != null)
				{
					this.InitFromView(this.m_form);
					this.PauseGame(true);
					return true;
				}
				return false;
			}
			else
			{
				if (this.m_iTimer == -1)
				{
					this.m_iTimer = Singleton<CTimerManager>.GetInstance().AddTimer(2000, -1, new CTimer.OnTimeUpHandler(this.OnOpenFormTimeUpHandler));
					return this.m_iTimer != -1;
				}
				Singleton<CTimerManager>.GetInstance().ResetTimer(this.m_iTimer);
				Singleton<CTimerManager>.GetInstance().ResumeTimer(this.m_iTimer);
				return true;
			}
		}

		private void UpdateBuffItemChooseStat(int iItemIndex, bool bChoose)
		{
			if (iItemIndex < 0 || iItemIndex >= 3)
			{
				return;
			}
			string name = "buffInfoPanel/buffItem" + iItemIndex;
			GameObject gameObject = this.m_form.transform.Find(name).gameObject;
			GameObject gameObject2 = gameObject.transform.Find("chooseImage").gameObject;
			gameObject2.CustomSetActive(bChoose);
		}

		private void InitBuffItem(CUIFormScript formScript, SLevelContext curLevelContext)
		{
			this.m_iLevelID = curLevelContext.m_mapID;
			this.m_CurDifficulType = curLevelContext.m_levelDifficulty;
			for (int i = 0; i < 3; i++)
			{
				string name = "buffInfoPanel/buffItem" + i;
				GameObject gameObject = this.m_form.transform.Find(name).gameObject;
				this.UpdateBuffItemChooseStat(i, i == (int)this.m_iSelectedBuffItem);
				uint num = curLevelContext.m_reviveInfo[curLevelContext.m_levelDifficulty].ReviveBuff[i];
				this.m_WishBuffId[i] = num;
				ResSkillCombineCfgInfo dataByKey = GameDataMgr.skillCombineDatabin.GetDataByKey(num);
				if (dataByKey != null)
				{
					if (dataByKey.szIconPath.get_Chars(0) != '\0')
					{
						Image component = gameObject.transform.Find("imageIcon").GetComponent<Image>();
						string prefebPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey.szIconPath));
						GameObject spritePrefeb = CUIUtility.GetSpritePrefeb(prefebPath, true, true);
						component.SetSprite(spritePrefeb, false);
					}
					Text component2 = gameObject.transform.Find("buffNameText").GetComponent<Text>();
					component2.set_text(StringHelper.UTF8BytesToString(ref dataByKey.szSkillCombineDesc));
				}
			}
		}

		private void InitFromView(CUIFormScript formScript)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			this.InitBuffItem(formScript, curLvelContext);
			this.m_relivePayType = CMallSystem.ResBuyTypeToPayType((int)curLvelContext.m_reviveInfo[curLvelContext.m_levelDifficulty].astReviveCost[3 - PVEReviveHeros.m_iRemainReviveNum].bCostType);
			this.m_relivePayValue = curLvelContext.m_reviveInfo[curLvelContext.m_levelDifficulty].astReviveCost[3 - PVEReviveHeros.m_iRemainReviveNum].dwCostPrice;
			string text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ReliveMessage"), Singleton<CTextManager>.GetInstance().GetText(CMallSystem.s_payTypeNameKeys[(int)this.m_relivePayType]));
			GameObject widget = formScript.GetWidget(0);
			if (widget != null)
			{
				Text component = widget.GetComponent<Text>();
				if (component != null)
				{
					component.set_text(text);
				}
			}
			GameObject widget2 = formScript.GetWidget(1);
			if (widget2 != null)
			{
				Image component2 = widget2.GetComponent<Image>();
				if (component2 != null)
				{
					component2.SetSprite(CMallSystem.GetPayTypeIconPath(this.m_relivePayType), formScript, true, false, false, false);
				}
			}
			GameObject widget3 = formScript.GetWidget(2);
			if (widget3 != null)
			{
				Text component3 = widget3.GetComponent<Text>();
				if (component3 != null)
				{
					component3.set_text(this.m_relivePayValue.ToString());
				}
			}
			GameObject gameObject = this.m_form.transform.Find("buffInfoPanel").gameObject;
			if (gameObject == null)
			{
				return;
			}
			GameObject gameObject2 = gameObject.transform.Find("ReviveText/NumText").gameObject;
			if (gameObject2 == null)
			{
				return;
			}
			gameObject2.GetComponent<Text>().set_text(PVEReviveHeros.m_iRemainReviveNum.ToString());
			CUITimerScript component4 = gameObject.transform.Find("Timer").GetComponent<CUITimerScript>();
			byte reviveTimeMax = curLvelContext.m_reviveTimeMax;
			component4.SetTotalTime((float)reviveTimeMax);
		}

		private void OnSelectBuffItem0(CUIEvent uiEvent)
		{
			if (this.m_iSelectedBuffItem != BUFF_ITEM.BUFF_ITEM_0)
			{
				this.UpdateBuffItemChooseStat((int)this.m_iSelectedBuffItem, false);
				this.m_iSelectedBuffItem = BUFF_ITEM.BUFF_ITEM_0;
				this.UpdateBuffItemChooseStat((int)this.m_iSelectedBuffItem, true);
			}
		}

		private void OnSelectBuffItem1(CUIEvent uiEvent)
		{
			if (this.m_iSelectedBuffItem != BUFF_ITEM.BUFF_ITEM_1)
			{
				this.UpdateBuffItemChooseStat((int)this.m_iSelectedBuffItem, false);
				this.m_iSelectedBuffItem = BUFF_ITEM.BUFF_ITEM_1;
				this.UpdateBuffItemChooseStat((int)this.m_iSelectedBuffItem, true);
			}
		}

		private void OnSelectBuffItem2(CUIEvent uiEvent)
		{
			if (this.m_iSelectedBuffItem != BUFF_ITEM.BUFF_ITEM_2)
			{
				this.UpdateBuffItemChooseStat((int)this.m_iSelectedBuffItem, false);
				this.m_iSelectedBuffItem = BUFF_ITEM.BUFF_ITEM_2;
				this.UpdateBuffItemChooseStat((int)this.m_iSelectedBuffItem, true);
			}
		}

		private void OnClickReviveBtn(CUIEvent uiEvent)
		{
			stUIEventParams stUIEventParams = default(stUIEventParams);
			stUIEventParams.commonUInt32Param1 = (uint)PVEReviveHeros.m_iRemainReviveNum;
			CMallSystem.TryToPay(enPayPurpose.Relive, string.Empty, this.m_relivePayType, this.m_relivePayValue, enUIEventID.ReviveHero_OnConfirmRevive, ref stUIEventParams, enUIEventID.ReviveHero_OnReviveFailed, true, true, false);
		}

		private void OnConfirmRevive(CUIEvent uiEvent)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4303u);
			cSPkg.stPkgData.stPveReviveReq.bReviveNo = (byte)(3 - PVEReviveHeros.m_iRemainReviveNum + 1);
			Singleton<BattleLogic>.instance.m_bIsPayStat = true;
			if (!Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true))
			{
				string text = Singleton<CTextManager>.instance.GetText("Common_Network_Err");
				Singleton<CUIManager>.GetInstance().OpenMessageBox(text, enUIEventID.ReviveHero_OnReviveFailed, false);
			}
		}

		[MessageHandler(4304)]
		public static void OnReviveHeroRsp(CSPkg msg)
		{
			Singleton<BattleLogic>.instance.m_bIsPayStat = false;
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stPveReviveRsp.iErrCode == 0 && ((int)msg.stPkgData.stPveReviveRsp.bDifficultType == Singleton<PVEReviveHeros>.instance.CurDifficulType || msg.stPkgData.stPveReviveRsp.iLevelID == Singleton<PVEReviveHeros>.instance.CurLevelId || (int)msg.stPkgData.stPveReviveRsp.bReviveNo == Singleton<PVEReviveHeros>.instance.CurReviveNum))
			{
				PVEReviveHeros.ReviveAllHeros();
				return;
			}
			string text = Singleton<CTextManager>.instance.GetText("PVE_Revive_Data_Error");
			Singleton<CUIManager>.GetInstance().OpenTips(text, false, 1.5f, null, new object[0]);
			Singleton<PVEReviveHeros>.instance.ReviveHerosFailed();
		}

		public static void ReviveAllHeros()
		{
			int inSkillCombineId = (int)Singleton<PVEReviveHeros>.instance.m_WishBuffId[(int)Singleton<PVEReviveHeros>.instance.m_iSelectedBuffItem];
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain)
			{
				return;
			}
			hostPlayer.Captain.handle.ActorControl.SetReviveContext(0, 10000, false, false, false, 10000, 0, true, 0u);
			hostPlayer.Captain.handle.ActorControl.SetReviveContextEnable(false);
			hostPlayer.Captain.handle.ActorControl.Revive(false);
			SkillUseParam skillUseParam = default(SkillUseParam);
			skillUseParam.SetOriginator(hostPlayer.Captain);
			hostPlayer.Captain.handle.SkillControl.SpawnBuff(hostPlayer.Captain, ref skillUseParam, inSkillCombineId, true);
			ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = hostPlayer.GetAllHeroes();
			int count = allHeroes.Count;
			for (int i = 0; i < count; i++)
			{
				if (allHeroes[i] && allHeroes[i] != hostPlayer.Captain && allHeroes[i])
				{
					allHeroes[i].handle.ActorControl.SetReviveContext(0, 10000, false, false, false, 10000, 0, true, 0u);
					allHeroes[i].handle.ActorControl.SetReviveContextEnable(false);
					allHeroes[i].handle.ActorControl.Revive(false);
					skillUseParam.SetOriginator(allHeroes[i]);
					allHeroes[i].handle.SkillControl.SpawnBuff(allHeroes[i], ref skillUseParam, inSkillCombineId, true);
				}
			}
			PVEReviveHeros.m_iRemainReviveNum--;
			Singleton<PVEReviveHeros>.instance.ReviveHerosSucess();
		}

		private void OnClickExitBtn(CUIEvent uiEvent)
		{
			this.ReviveHerosFailed();
		}

		private void OnReviveHerosFailed(CUIEvent uiEvent)
		{
			this.ReviveHerosFailed();
		}

		private void OnReviveTimeout(CUIEvent uiEvent)
		{
			if (this.m_form != null)
			{
				this.m_form.Close();
			}
			this.ReviveHerosFailed();
		}

		private void OnBuyDianQuanPanelClose(CUIEvent uiEvent)
		{
			if (!this.IsCareCondition())
			{
				return;
			}
			this.CheckAndPopReviveForm(PVEReviveHeros.SetReviveResult, false);
		}

		private void ReviveHerosFailed()
		{
			this.PauseGame(false);
			if (PVEReviveHeros.SetReviveResult != null)
			{
				PVEReviveHeros.SetReviveResult(false);
			}
		}

		private void ReviveHerosSucess()
		{
			this.PauseGame(false);
			if (PVEReviveHeros.SetReviveResult != null)
			{
				PVEReviveHeros.SetReviveResult(true);
			}
		}

		private void PauseGame(bool bPause)
		{
			Singleton<FrameSynchr>.instance.SetSynchrRunning(!bPause);
		}

		private void OnFightStart(ref DefaultGameEventParam prm)
		{
			this.ResetReviveCondition();
		}

		private bool IsCareCondition()
		{
			if (!Singleton<BattleLogic>.instance.isFighting)
			{
				return false;
			}
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			return !curLvelContext.IsMobaMode() && curLvelContext.IsGameTypeAdventure();
		}

		private void OnGameEnd(ref DefaultGameEventParam prm)
		{
			if (!this.IsCareCondition())
			{
				return;
			}
			if (this.m_form != null)
			{
				this.m_form.Close();
			}
		}

		private void OnOpenFormTimeUpHandler(int timerSequence)
		{
			Singleton<CTimerManager>.instance.PauseTimer(timerSequence);
			if (!this.IsCareCondition())
			{
				return;
			}
			this.m_form = Singleton<CUIManager>.GetInstance().OpenForm(PVEReviveHeros.PATH_REVIVEHERO, false, true);
			if (this.m_form != null)
			{
				this.InitFromView(this.m_form);
				this.PauseGame(true);
			}
			else
			{
				this.ReviveHerosFailed();
			}
		}

		public void ClearTimeOutTimer()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnReviveTimeout, new CUIEventManager.OnUIEventHandler(this.OnReviveTimeout));
		}
	}
}
