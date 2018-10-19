using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class WinLose : Singleton<WinLose>
	{
		public static string m_FormPath = "UGUI/Form/Battle/Form_BattleResult";

		private GameObject node;

		private bool m_bWin;

		public bool LastSingleGameWin = true;

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ClickReault, new CUIEventManager.OnUIEventHandler(this.onBackToHall));
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ClickReault, new CUIEventManager.OnUIEventHandler(this.onBackToHall));
			this.node = null;
		}

		public void ShowPanel(bool bWin)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.m_isShowTrainingHelper)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Training_HelperUninit);
			}
			Singleton<CBattleSystem>.GetInstance().CloseForm();
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(WinLose.m_FormPath, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			this.node = cUIFormScript.gameObject;
			this.m_bWin = bWin;
			Utility.FindChild(this.node, "Win").CustomSetActive(false);
			Utility.FindChild(this.node, "Lose").CustomSetActive(false);
			if (bWin)
			{
				Utility.FindChild(this.node, "Win").CustomSetActive(true);
				Singleton<CSoundManager>.GetInstance().PlayBattleSound2D("Self_Victory");
				Singleton<CSoundManager>.GetInstance().PostEvent("Set_Victor", null);
			}
			else
			{
				Utility.FindChild(this.node, "Lose").CustomSetActive(true);
				Singleton<CSoundManager>.GetInstance().PlayBattleSound2D("Self_Defeat");
				Singleton<CSoundManager>.GetInstance().PostEvent("Set_Defeat", null);
			}
			Singleton<LobbyLogic>.GetInstance().StopSettlePanelTimer();
		}

		private void onBackToHall(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(WinLose.m_FormPath);
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext.IsGameTypeGuide())
			{
				Singleton<GameBuilder>.instance.EndGame();
				Singleton<CBattleGuideManager>.GetInstance().OpenSettle();
			}
			else if (curLvelContext.IsGameTypeLadder())
			{
				Singleton<SettlementSystem>.instance.ShowLadderSettleForm(this.m_bWin);
			}
			else
			{
				Singleton<SettlementSystem>.instance.ShowPersonalProfit(this.m_bWin);
			}
		}

		private void Test()
		{
		}
	}
}
