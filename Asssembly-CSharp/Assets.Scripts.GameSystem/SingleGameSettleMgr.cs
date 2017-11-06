using Assets.Scripts.GameLogic;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	internal class SingleGameSettleMgr : Singleton<SingleGameSettleMgr>
	{
		public static string PATH_BURNING_WINLOSE = "UGUI/Form/System/BurnExpedition/Form_WinLose.prefab";

		public static string PATH_BURNING_SETTLE = "UGUI/Form/System/BurnExpedition/Form_Settle.prefab";

		public SCPKG_SINGLEGAMEFINRSP m_settleData;

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Burn_WinLoseConfirm, new CUIEventManager.OnUIEventHandler(this.OnOpenSettle));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Burn_SettleConfirm, new CUIEventManager.OnUIEventHandler(this.OnSettleConfirm));
		}

		public override void UnInit()
		{
			base.UnInit();
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Burn_WinLoseConfirm, new CUIEventManager.OnUIEventHandler(this.OnOpenSettle));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Burn_SettleConfirm, new CUIEventManager.OnUIEventHandler(this.OnSettleConfirm));
			this.m_settleData = null;
		}

		public void StartSettle(SCPKG_SINGLEGAMEFINRSP settleData)
		{
			if (settleData == null || settleData.stDetail == null || settleData.stDetail.stMultipleDetail == null)
			{
				return;
			}
			if (settleData.stDetail.stMultipleDetail.bZeroProfit == 1)
			{
				Singleton<CUIManager>.instance.OpenTips("ZeroProfit_Tips", true, 1.5f, null, new object[0]);
			}
			this.m_settleData = settleData;
			if (settleData.iErrCode == 0)
			{
				if (settleData.bPressExit == 1)
				{
					settleData.stDetail.stGameInfo.bGameResult = 2;
				}
				byte bGameType = settleData.stDetail.stGameInfo.bGameType;
				if (bGameType == 1 || bGameType != 2)
				{
				}
				switch (bGameType)
				{
				case 0:
				case 3:
				{
					bool bFirstPass = false;
					if (bGameType == 0 && settleData.stDetail.stGameInfo.bGameResult == 1)
					{
						bFirstPass = Singleton<CAdventureSys>.GetInstance().UpdateAdvProgress(true);
					}
					Singleton<PVESettleSys>.GetInstance().StartSettle(settleData.stDetail, bFirstPass);
					return;
				}
				case 1:
					Singleton<BattleStatistic>.GetInstance().acntInfo = settleData.stDetail.stAcntInfo;
					Singleton<BattleStatistic>.GetInstance().SpecialItemInfo = settleData.stDetail.stSpecReward;
					Singleton<BattleStatistic>.GetInstance().multiDetail = settleData.stDetail.stMultipleDetail;
					Singleton<BattleStatistic>.GetInstance().Rewards = settleData.stDetail.stReward;
					Singleton<BattleStatistic>.GetInstance().heroSettleInfo = settleData.stDetail.stHeroList;
					SLevelContext.SetMasterPvpDetailWhenGameSettle(settleData.stDetail.stGameInfo);
					return;
				case 2:
					Singleton<BattleStatistic>.GetInstance().acntInfo = settleData.stDetail.stAcntInfo;
					Singleton<BattleStatistic>.GetInstance().SpecialItemInfo = settleData.stDetail.stSpecReward;
					Singleton<BattleStatistic>.GetInstance().Rewards = settleData.stDetail.stReward;
					Singleton<BattleStatistic>.GetInstance().multiDetail = settleData.stDetail.stMultipleDetail;
					Singleton<CBattleGuideManager>.GetInstance().StartSettle(settleData.stDetail);
					return;
				case 5:
				case 9:
				{
					SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
					if (curLvelContext != null && curLvelContext.m_isWarmBattle)
					{
						Singleton<BattleStatistic>.GetInstance().acntInfo = settleData.stDetail.stAcntInfo;
						Singleton<BattleStatistic>.GetInstance().SpecialItemInfo = settleData.stDetail.stSpecReward;
						Singleton<BattleStatistic>.GetInstance().Rewards = settleData.stDetail.stReward;
						Singleton<BattleStatistic>.GetInstance().multiDetail = settleData.stDetail.stMultipleDetail;
						Singleton<BattleStatistic>.GetInstance().heroSettleInfo = settleData.stDetail.stHeroList;
						SLevelContext.SetMasterPvpDetailWhenGameSettle(settleData.stDetail.stGameInfo);
					}
					return;
				}
				case 7:
					BurnExpeditionUT.Handle_Burn_Settle(ref settleData);
					this.OpenWinLose(settleData.stDetail.stGameInfo.bGameResult == 1);
					return;
				case 8:
					this.OpenWinLose(settleData.stDetail.stGameInfo.bGameResult == 1);
					Singleton<CArenaSystem>.GetInstance().BattleReturn(settleData.stDetail.stGameInfo.bGameResult == 1);
					return;
				}
				Singleton<CUIManager>.GetInstance().OpenTips("Invalid GameType -- " + bGameType, false, 1.5f, null, new object[0]);
			}
		}

		private void OpenWinLose(bool bWin)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(SingleGameSettleMgr.PATH_BURNING_WINLOSE, false, true);
			if (bWin)
			{
				Singleton<CSoundManager>.GetInstance().PostEvent("Set_Victor", null);
			}
			else
			{
				Singleton<CSoundManager>.GetInstance().PostEvent("Set_Defeat", null);
			}
			CSingleGameSettleView.ShowBurnWinLose(form, bWin);
		}

		private void CloseBurnArenaSettleView()
		{
			Singleton<CUIManager>.GetInstance().CloseForm(SingleGameSettleMgr.PATH_BURNING_SETTLE);
			Singleton<GameBuilder>.instance.EndGame();
		}

		private void OnOpenSettle(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(SingleGameSettleMgr.PATH_BURNING_WINLOSE);
			CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(SingleGameSettleMgr.PATH_BURNING_SETTLE, false, true);
			CSingleGameSettleView.SetBurnSettleData(form, ref this.m_settleData.stDetail);
			if (this.m_settleData.stDetail.stGameInfo.bGameType == 8)
			{
				Singleton<CArenaSystem>.GetInstance().ShowBattleResult(this.m_settleData);
			}
		}

		private void OnSettleConfirm(CUIEvent uiEvent)
		{
			this.CloseBurnArenaSettleView();
			if (this.m_settleData.stDetail.stGameInfo.bGameType == 7)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Burn_OpenForm);
			}
			else if (this.m_settleData.stDetail.stGameInfo.bGameType == 8)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Arena_OpenForm);
			}
		}
	}
}
