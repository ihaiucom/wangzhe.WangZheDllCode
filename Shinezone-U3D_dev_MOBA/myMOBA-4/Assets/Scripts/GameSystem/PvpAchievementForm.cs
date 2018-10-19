using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	internal class PvpAchievementForm
	{
		public enum ShareDefeatWidget
		{
			ButtonClose,
			BarragePanel,
			DisplayRect,
			BtnBarrige,
			BtnFriend,
			BtnZone
		}

		private enum ShareVictoryWidget
		{
			AchievementRoot,
			BadgeIcon,
			Times
		}

		public static string s_formSharePVPVictoryPath = "UGUI/Form/System/PvP/Settlement/Form_SharePVPVictory.prefab";

		public static string s_formSharePVPDefeatPath = "UGUI/Form/System/PvP/Settlement/Form_SharePVPDefeat.prefab";

		public static string s_imageSharePVPBadge = CUIUtility.s_Sprite_Dynamic_PvpAchievementShare_Dir + "Img_PVP_ShareIcon_";

		private RES_SHOW_ACHIEVEMENT_TYPE m_curAchievemnt = RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_COUNT;

		private bool[] m_allAchievements;

		private ListView<string> barrageList = new ListView<string>();

		public void Init(bool bWin)
		{
			this.m_allAchievements = new bool[8];
			this.m_curAchievemnt = RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_COUNT;
			if (Singleton<BattleLogic>.GetInstance().battleStat == null || Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat == null)
			{
				return;
			}
			PlayerKDA hostKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
			if (hostKDA == null)
			{
				return;
			}
			uint mvpPlayer = Singleton<BattleStatistic>.instance.GetMvpPlayer(hostKDA.PlayerCamp, bWin);
			if (mvpPlayer != 0u && mvpPlayer == hostKDA.PlayerId)
			{
				this.m_allAchievements[7] = true;
			}
			COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
			if (acntInfo != null)
			{
				if (acntInfo.dwCurWeekContinousWinNum == 15u)
				{
					this.m_allAchievements[0] = true;
				}
				else if (acntInfo.dwCurWeekContinousWinNum == 10u)
				{
					this.m_allAchievements[2] = true;
				}
				else if (acntInfo.dwCurWeekContinousWinNum == 5u)
				{
					this.m_allAchievements[4] = true;
				}
			}
			ListView<HeroKDA>.Enumerator enumerator = hostKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				HeroKDA current = enumerator.Current;
				if (current != null)
				{
					if (current.LegendaryNum > 0)
					{
						this.m_allAchievements[5] = true;
					}
					if (current.PentaKillNum > 0)
					{
						this.m_allAchievements[1] = true;
					}
					if (current.QuataryKillNum > 0)
					{
						this.m_allAchievements[3] = true;
					}
					if (current.TripleKillNum > 0)
					{
						this.m_allAchievements[6] = true;
					}
				}
			}
		}

		private int GetAchievementCount(RES_SHOW_ACHIEVEMENT_TYPE achievement)
		{
			int result = 0;
			switch (achievement)
			{
			case RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_LEGENDARY:
				result = this.GetKVDataCount(RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_GODLIKE_CNT);
				break;
			case RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_TRIPLEKILL:
				result = this.GetKVDataCount(RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_TRIPLE_KILL_CNT);
				break;
			case RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_MVP:
				result = this.GetKVDataCount(RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_MVP_CNT);
				break;
			}
			return result;
		}

		private int GetKVDataCount(RES_STATISTIC_SETTLE_DATA_TYPE type)
		{
			int result = 0;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return result;
			}
			int num = 0;
			while ((long)num < (long)((ulong)masterRoleInfo.pvpDetail.stKVDetail.dwNum))
			{
				COMDT_STATISTIC_KEY_VALUE_INFO cOMDT_STATISTIC_KEY_VALUE_INFO = masterRoleInfo.pvpDetail.stKVDetail.astKVDetail[num];
				if (cOMDT_STATISTIC_KEY_VALUE_INFO.dwKey == (uint)type)
				{
					result = (int)cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue;
					break;
				}
				num++;
			}
			return result;
		}

		public bool CheckAchievement()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null || masterRoleInfo.PvpLevel < GameDataMgr.globalInfoDatabin.GetDataByKey(159u).dwConfValue)
			{
				return false;
			}
			for (int i = 0; i < this.m_allAchievements.Length; i++)
			{
				if (this.m_allAchievements[i])
				{
					return true;
				}
			}
			return false;
		}

		private int GetAchievementCount()
		{
			int num = 0;
			for (int i = 0; i < this.m_allAchievements.Length; i++)
			{
				if (this.m_allAchievements[i])
				{
					num++;
				}
			}
			return num;
		}

		public void ShowVictory()
		{
			if (this.m_allAchievements == null || this.m_allAchievements.Length != 8)
			{
				return;
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(PvpAchievementForm.s_formSharePVPVictoryPath, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_LeftSwitchPVPAchievement, new CUIEventManager.OnUIEventHandler(this.LeftSwitchAchievementHandle));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_RightSwitchPVPAchievement, new CUIEventManager.OnUIEventHandler(this.RightSwitchAchievementHandle));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_OpenSharePVPAchievement, new CUIEventManager.OnUIEventHandler(this.PvpAchievementShareBtnHandle));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ClosePVPAchievement, new CUIEventManager.OnUIEventHandler(this.OnPVPAchievementCloseHandle));
			for (int i = 0; i < this.m_allAchievements.Length; i++)
			{
				if (this.m_allAchievements[i])
				{
					this.SwitchAchievement((RES_SHOW_ACHIEVEMENT_TYPE)i);
					break;
				}
			}
			if (CSysDynamicBlock.bSocialBlocked)
			{
				Transform transform = cUIFormScript.transform.Find("AchievementRoot/ButtonGrid/Button_Share");
				if (transform)
				{
					transform.gameObject.CustomSetActive(false);
				}
			}
		}

		private void HideVictory()
		{
			Singleton<CUIManager>.GetInstance().CloseForm(PvpAchievementForm.s_formSharePVPVictoryPath);
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_LeftSwitchPVPAchievement, new CUIEventManager.OnUIEventHandler(this.LeftSwitchAchievementHandle));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_RightSwitchPVPAchievement, new CUIEventManager.OnUIEventHandler(this.RightSwitchAchievementHandle));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_OpenSharePVPAchievement, new CUIEventManager.OnUIEventHandler(this.PvpAchievementShareBtnHandle));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Share_ClosePVPAchievement, new CUIEventManager.OnUIEventHandler(this.OnPVPAchievementCloseHandle));
		}

		private void LeftSwitchAchievementHandle(CUIEvent ievent)
		{
			int curAchievemnt = (int)this.m_curAchievemnt;
			for (int i = curAchievemnt - 1; i > 0; i--)
			{
				if (this.m_allAchievements[i])
				{
					this.SwitchAchievement((RES_SHOW_ACHIEVEMENT_TYPE)i);
					return;
				}
			}
			for (int j = this.m_allAchievements.Length - 1; j > curAchievemnt; j--)
			{
				if (this.m_allAchievements[j])
				{
					this.SwitchAchievement((RES_SHOW_ACHIEVEMENT_TYPE)j);
					return;
				}
			}
		}

		private void RightSwitchAchievementHandle(CUIEvent ievent)
		{
			int curAchievemnt = (int)this.m_curAchievemnt;
			for (int i = curAchievemnt + 1; i < this.m_allAchievements.Length; i++)
			{
				if (this.m_allAchievements[i])
				{
					this.SwitchAchievement((RES_SHOW_ACHIEVEMENT_TYPE)i);
					return;
				}
			}
			for (int j = 0; j < curAchievemnt; j++)
			{
				if (this.m_allAchievements[j])
				{
					this.SwitchAchievement((RES_SHOW_ACHIEVEMENT_TYPE)j);
					return;
				}
			}
		}

		private void SwitchAchievement(RES_SHOW_ACHIEVEMENT_TYPE achievement)
		{
			this.m_curAchievemnt = achievement;
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PvpAchievementForm.s_formSharePVPVictoryPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			Utility.FindChild(widget.gameObject, "Title/Text").GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("Share_PvpAchievement_Title" + (int)achievement);
			Utility.FindChild(widget.gameObject, "Grid").CustomSetActive(this.GetAchievementCount() > 1);
			if (this.IsVictoryStreak(achievement))
			{
				CUIUtility.SetImageSprite(form.GetWidget(1).GetComponent<Image>(), PvpAchievementForm.s_imageSharePVPBadge + 0 + ".prefab", form, true, false, false, false);
			}
			else
			{
				CUIUtility.SetImageSprite(form.GetWidget(1).GetComponent<Image>(), PvpAchievementForm.s_imageSharePVPBadge + (int)achievement + ".prefab", form, true, false, false, false);
			}
			GameObject gameObject = form.GetWidget(2).gameObject;
			if (gameObject != null)
			{
				int achievementCount = this.GetAchievementCount(achievement);
				if (achievementCount != 0)
				{
					Utility.FindChild(gameObject, "Number").GetComponent<Text>().text = achievementCount.ToString();
					Utility.FindChild(gameObject, "Text").GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("Share_PvpAchievement_Desc_" + (int)achievement);
				}
				gameObject.CustomSetActive(achievementCount != 0);
			}
		}

		private bool IsVictoryStreak(RES_SHOW_ACHIEVEMENT_TYPE achievement)
		{
			return achievement == RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_FIFTEENVICTORY || achievement == RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_FIVEVICTORY || achievement == RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_TENVICTORY;
		}

		private void PvpAchievementShareBtnHandle(CUIEvent ievent)
		{
			MonoSingleton<ShareSys>.GetInstance().OpenShowSharePVPFrom(this.m_curAchievemnt);
		}

		private void OnPVPAchievementCloseHandle(CUIEvent uiEvent)
		{
			this.HideVictory();
			Singleton<SettlementSystem>.GetInstance().ShowSettlementPanel(false);
			MonoSingleton<ShareSys>.instance.m_bShowTimeline = false;
		}

		public void ShowDefeat()
		{
			if (this.m_allAchievements == null || this.m_allAchievements.Length != 8)
			{
				return;
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(PvpAchievementForm.s_formSharePVPDefeatPath, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			Transform transform = cUIFormScript.transform.Find("ShareFrame/Image");
			if (transform)
			{
				MonoSingleton<ShareSys>.GetInstance().SetShareDefeatImage(transform, cUIFormScript);
			}
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_CloseSharePVPDefeat, new CUIEventManager.OnUIEventHandler(this.OnCloseSharePVPDefeat));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShareDefeatAddBarrage, new CUIEventManager.OnUIEventHandler(this.OnSharePVPDefeatAddBarrage));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShareDefeatSelectBarrage, new CUIEventManager.OnUIEventHandler(this.OnSharePVPDefeatSelectBarrage));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShareDefeatBarrageEnable, new CUIEventManager.OnUIEventHandler(this.OnBarrageEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_CloseShareDefeatBarrage, new CUIEventManager.OnUIEventHandler(this.OnCloseBarrage));
			DatabinTable<ResDefeatBarrageText, ushort> databinTable = new DatabinTable<ResDefeatBarrageText, ushort>("Databin/Client/Text/DefeatBarrageText.bytes", "wID");
			for (int i = 0; i < this.m_allAchievements.Length; i++)
			{
				if (this.m_allAchievements[i])
				{
					this.m_curAchievemnt = (RES_SHOW_ACHIEVEMENT_TYPE)i;
					break;
				}
			}
			this.barrageList.Clear();
			if (databinTable != null)
			{
				Dictionary<long, object>.Enumerator enumerator = databinTable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<long, object> current = enumerator.Current;
					ResDefeatBarrageText resDefeatBarrageText = (ResDefeatBarrageText)current.Value;
					if ((RES_SHOW_ACHIEVEMENT_TYPE)resDefeatBarrageText.wAchievementType == this.m_curAchievemnt)
					{
						this.barrageList.Add(resDefeatBarrageText.szContent);
					}
				}
			}
			MonoSingleton<ShareSys>.GetInstance().UpdateSharePVPForm(cUIFormScript, cUIFormScript.GetWidget(2));
		}

		private void HideDefeat()
		{
			Singleton<CUIManager>.GetInstance().CloseForm(PvpAchievementForm.s_formSharePVPDefeatPath);
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_CloseSharePVPDefeat, new CUIEventManager.OnUIEventHandler(this.OnCloseSharePVPDefeat));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShareDefeatAddBarrage, new CUIEventManager.OnUIEventHandler(this.OnSharePVPDefeatAddBarrage));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShareDefeatSelectBarrage, new CUIEventManager.OnUIEventHandler(this.OnSharePVPDefeatSelectBarrage));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShareDefeatBarrageEnable, new CUIEventManager.OnUIEventHandler(this.OnBarrageEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_CloseShareDefeatBarrage, new CUIEventManager.OnUIEventHandler(this.OnCloseBarrage));
		}

		private void OnCloseSharePVPDefeat(CUIEvent uiEvent)
		{
			Singleton<CChatController>.instance.ShowPanel(true, false);
			this.HideDefeat();
		}

		private void OnSharePVPDefeatAddBarrage(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PvpAchievementForm.s_formSharePVPDefeatPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(1);
			if (widget != null && !widget.activeSelf)
			{
				widget.CustomSetActive(true);
			}
			GameObject gameObject = Utility.FindChild(widget, "BarrageList");
			if (gameObject == null)
			{
				return;
			}
			CUIListScript component = gameObject.GetComponent<CUIListScript>();
			if (component == null)
			{
				return;
			}
			component.SetElementAmount(this.barrageList.Count);
			component.MoveElementInScrollArea(0, true);
			for (int i = 0; i < this.barrageList.Count; i++)
			{
				if (component.GetElemenet(i) != null && component.IsElementInScrollArea(i))
				{
					this.UpdateOneBarrageElement(component.GetElemenet(i).gameObject, i, false);
				}
			}
			if (component.GetSelectedIndex() == -1)
			{
				component.SelectElement(0, true);
			}
			if (!gameObject.activeSelf)
			{
				gameObject.CustomSetActive(true);
			}
		}

		private void UpdateOneBarrageElement(GameObject go, int index, bool selected = false)
		{
			if (go == null || this.barrageList.Count <= index)
			{
				return;
			}
			Utility.FindChild(go, "Text").GetComponent<Text>().text = this.barrageList[index];
		}

		private void OnSharePVPDefeatSelectBarrage(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PvpAchievementForm.s_formSharePVPDefeatPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(1);
			if (widget == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(widget, "BarrageList");
			if (gameObject == null)
			{
				return;
			}
			if (gameObject.activeSelf)
			{
				gameObject.CustomSetActive(false);
			}
			GameObject gameObject2 = Utility.FindChild(widget, "BarrageBg/BarrageText");
			int selectedIndex = gameObject.GetComponent<CUIListScript>().GetSelectedIndex();
			if (gameObject2 != null && selectedIndex < this.barrageList.Count)
			{
				gameObject2.GetComponent<Text>().text = this.barrageList[selectedIndex];
			}
		}

		private void OnBarrageEnable(CUIEvent uiEvent)
		{
			this.UpdateOneBarrageElement(uiEvent.m_srcWidget, uiEvent.m_srcWidgetIndexInBelongedList, false);
		}

		private void OnCloseBarrage(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PvpAchievementForm.s_formSharePVPDefeatPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(1);
			if (widget != null && widget.activeSelf)
			{
				widget.CustomSetActive(false);
			}
		}
	}
}
