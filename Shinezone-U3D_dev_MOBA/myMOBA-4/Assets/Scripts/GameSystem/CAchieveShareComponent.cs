using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CAchieveShareComponent
	{
		private const string AchievementShareFormPrefabPath = "UGUI/Form/System/Achieve/Form_Achievement_Share.prefab";

		private const float TweenTime = 2f;

		private CUIFormScript m_shareForm;

		private float m_containerWidth = 260f;

		private uint m_nextPoint;

		private uint m_startPoint;

		private uint m_endPoint;

		private float m_achievePointsFrom;

		private float m_achievePointsTo = 1f;

		private bool m_isNewTrophy;

		private CAchieveItem2 m_curAchieveItem;

		private CTrophyRewardInfo m_curTrophyRewardInfo;

		private static LTDescr m_trophyPointsProgressLTD;

		private static LTDescr m_TrophyPointsAddLTD;

		private static LTDescr m_trophyLevelLTD;

		private bool m_isShowing;

		public CAchieveShareComponent()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Close_Share_Form, new CUIEventManager.OnUIEventHandler(this.OnContinueProcessDoneAchievements));
		}

		public void Process(bool force = false)
		{
			if (this.m_isShowing)
			{
				return;
			}
			CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
			if (masterAchieveInfo.MostLatelyDoneAchievements.Count == 0)
			{
				return;
			}
			if (!Singleton<CLobbySystem>.GetInstance().IsInLobbyForm() || Singleton<CMatchingSystem>.GetInstance().IsInMatching || Singleton<CMatchingSystem>.GetInstance().IsInMatchingTeam || Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX) != null)
			{
				return;
			}
			if (!force)
			{
				string[] array = new string[]
				{
					Singleton<CMallSystem>.GetInstance().sMallFormPath,
					"Form_NewHeroOrSkin.prefab"
				};
				for (int i = 0; i < array.Length; i++)
				{
					CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(array[i]);
					if (form != null && !form.IsClosed())
					{
						return;
					}
				}
			}
			CUIFormScript form2 = Singleton<CUIManager>.GetInstance().GetForm("Form_NobeLevelUp.prefab");
			if (form2 != null && !form2.IsClosed())
			{
				return;
			}
			uint num = masterAchieveInfo.MostLatelyDoneAchievements[0];
			if (!masterAchieveInfo.m_AchiveItemDic.ContainsKey(num))
			{
				return;
			}
			this.OpenShareForm(num);
		}

		private void OpenShareForm(uint achievementId)
		{
			this.m_shareForm = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Achieve/Form_Achievement_Share.prefab", false, true);
			if (this.m_shareForm == null)
			{
				return;
			}
			this.m_isShowing = true;
			this.RefreshData(achievementId);
			this.RefreshShareForm();
		}

		public void ResetData()
		{
			this.m_curAchieveItem = null;
			this.m_curTrophyRewardInfo = null;
			this.m_nextPoint = 0u;
			this.m_startPoint = 0u;
			this.m_endPoint = 0u;
			this.m_achievePointsFrom = 0f;
			this.m_achievePointsTo = 1f;
			this.m_isNewTrophy = false;
			this.m_curAchieveItem = null;
			this.m_curTrophyRewardInfo = null;
		}

		private void RefreshData(uint achievementId)
		{
			this.ResetData();
			CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
			this.m_curAchieveItem = masterAchieveInfo.m_AchiveItemDic[achievementId];
			CAchieveItem2 head = this.m_curAchieveItem.GetHead();
			if (head == this.m_curAchieveItem)
			{
				this.m_isNewTrophy = true;
			}
			else
			{
				this.m_isNewTrophy = false;
			}
			uint num = 0u;
			masterAchieveInfo.GetTrophyProgress(ref num, ref this.m_nextPoint);
			uint num2 = 0u;
			for (int i = 0; i < masterAchieveInfo.MostLatelyDoneAchievements.Count; i++)
			{
				CAchieveItem2 cAchieveItem = masterAchieveInfo.m_AchiveItemDic[masterAchieveInfo.MostLatelyDoneAchievements[i]];
				num2 += cAchieveItem.Cfg.dwPoint;
			}
			this.m_startPoint = num - num2;
			this.m_endPoint = this.m_startPoint + this.m_curAchieveItem.Cfg.dwPoint;
			CTrophyRewardInfo trophyRewardInfoByPoint = masterAchieveInfo.GetTrophyRewardInfoByPoint(this.m_startPoint);
			CTrophyRewardInfo cTrophyRewardInfo = this.m_curTrophyRewardInfo = masterAchieveInfo.GetTrophyRewardInfoByPoint(this.m_endPoint);
			CTrophyRewardInfo trophyRewardInfoByIndex = masterAchieveInfo.GetTrophyRewardInfoByIndex(cTrophyRewardInfo.Index + 1);
			if (trophyRewardInfoByPoint.Cfg.dwTrophyLvl == cTrophyRewardInfo.Cfg.dwTrophyLvl)
			{
				this.m_achievePointsFrom = Utility.Divide(this.m_startPoint - trophyRewardInfoByIndex.MinPoint, trophyRewardInfoByIndex.GetPointStep());
				this.m_achievePointsTo = Utility.Divide(this.m_endPoint - trophyRewardInfoByIndex.MinPoint, trophyRewardInfoByIndex.GetPointStep());
			}
			else
			{
				this.m_achievePointsFrom = 0f;
				this.m_achievePointsTo = Utility.Divide(this.m_endPoint - trophyRewardInfoByIndex.MinPoint, trophyRewardInfoByIndex.GetPointStep());
			}
		}

		private void RefreshShareForm()
		{
			if (this.m_curTrophyRewardInfo == null)
			{
				return;
			}
			if (this.m_shareForm == null)
			{
				this.m_shareForm = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Achieve/Form_Achievement_Share.prefab");
			}
			if (this.m_shareForm == null)
			{
				return;
			}
			CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
			CTrophyRewardInfo trophyRewardInfoByIndex = masterAchieveInfo.GetTrophyRewardInfoByIndex(this.m_curTrophyRewardInfo.Index + 1);
			if (this.m_isNewTrophy)
			{
			}
			Text component = this.m_shareForm.GetWidget(0).GetComponent<Text>();
			component.text = this.m_curAchieveItem.Cfg.szName;
			Text component2 = this.m_shareForm.GetWidget(5).GetComponent<Text>();
			component2.text = this.m_curAchieveItem.GetAchievementDesc();
			Text component3 = this.m_shareForm.GetWidget(7).GetComponent<Text>();
			component3.text = this.m_curAchieveItem.GetAchievementTips();
			GameObject widget = this.m_shareForm.GetWidget(6);
			Image component4 = widget.GetComponent<Image>();
			component4.SetSprite(this.m_curAchieveItem.GetAchieveImagePath(), this.m_shareForm, true, false, false, false);
			GameObject widget2 = this.m_shareForm.GetWidget(18);
			CAchievementSystem.SetAchieveBaseIcon(widget2.transform, this.m_curAchieveItem, this.m_shareForm);
			Image component5 = this.m_shareForm.GetWidget(20).GetComponent<Image>();
			GameObject widget3 = this.m_shareForm.GetWidget(12);
			GameObject widget4 = this.m_shareForm.GetWidget(19);
			Text component6 = this.m_shareForm.GetWidget(11).GetComponent<Text>();
			Text component7 = widget3.GetComponent<Text>();
			Text component8 = this.m_shareForm.GetWidget(16).GetComponent<Text>();
			component5.SetSprite(this.m_curTrophyRewardInfo.GetTrophyImagePath(), this.m_shareForm, true, false, false, false);
			component8.text = string.Format("{0}/{1}", this.m_endPoint - trophyRewardInfoByIndex.MinPoint, trophyRewardInfoByIndex.GetPointStep());
			component6.text = this.m_curTrophyRewardInfo.Cfg.dwTrophyLvl.ToString();
			if (masterAchieveInfo.GetWorldRank() == 0u)
			{
				widget4.CustomSetActive(true);
				widget3.CustomSetActive(false);
			}
			else
			{
				widget3.CustomSetActive(true);
				component7.text = masterAchieveInfo.GetWorldRank().ToString();
				widget4.CustomSetActive(false);
			}
			this.DoTrophyTween();
			Text component9 = this.m_shareForm.GetWidget(10).GetComponent<Text>();
			ShareSys.SetSharePlatfText(component9);
			if (CSysDynamicBlock.bSocialBlocked)
			{
				Transform transform = this.m_shareForm.transform.Find("Panel_ShareAchievement_Btn");
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(false);
				}
				Transform transform2 = this.m_shareForm.transform.Find("Panel_NewAchievement_Btn/Btn_Share");
				if (transform2 != null)
				{
					transform2.gameObject.CustomSetActive(false);
				}
			}
		}

		private void DoTrophyTween()
		{
			if (this.m_shareForm == null)
			{
				return;
			}
			GameObject widget = this.m_shareForm.GetWidget(13);
			RectTransform component = this.m_shareForm.GetWidget(15).GetComponent<RectTransform>();
			RectTransform component2 = this.m_shareForm.GetWidget(14).GetComponent<RectTransform>();
			Text component3 = this.m_shareForm.GetWidget(17).GetComponent<Text>();
			RectTransform component4 = widget.GetComponent<RectTransform>();
			if (component4 != null)
			{
				this.m_containerWidth = component4.rect.width;
			}
			component.sizeDelta = new Vector2(this.m_containerWidth * this.m_achievePointsFrom, component.sizeDelta.y);
			if (component2 != null)
			{
				CAchieveShareComponent.m_trophyPointsProgressLTD = LeanTween.value(component2.gameObject, new Action<float>(this.TrophyPointsProgressTween), this.m_achievePointsFrom, this.m_achievePointsTo, 2f);
			}
			if (component3 != null)
			{
				CAchieveShareComponent.m_TrophyPointsAddLTD = LeanTween.value(component3.gameObject, new Action<float>(this.TrophyPointsAddTween), 0f, this.m_curAchieveItem.Cfg.dwPoint, 2f);
			}
		}

		private void TrophyPointsProgressTween(float value)
		{
			if (this.m_shareForm == null)
			{
				return;
			}
			RectTransform component = this.m_shareForm.GetWidget(14).GetComponent<RectTransform>();
			if (component.gameObject != null)
			{
				if (value > 1f)
				{
					this.DoTrophyPointsProgressTweenEnd(value);
				}
				component.sizeDelta = new Vector2(value * this.m_containerWidth, component.sizeDelta.y);
			}
		}

		private void TrophyPointsAddTween(float value)
		{
			if (this.m_shareForm == null)
			{
				return;
			}
			Text component = this.m_shareForm.GetWidget(17).GetComponent<Text>();
			if (component != null)
			{
				component.text = string.Format("+{0}", value.ToString("N0"));
				if (value >= this.m_curAchieveItem.Cfg.dwPoint)
				{
					this.DoTrophyPointsAddTweenEnd();
				}
			}
		}

		private void DoTrophyPointsProgressTweenEnd(float value)
		{
			RectTransform component = this.m_shareForm.GetWidget(14).GetComponent<RectTransform>();
			if (CAchieveShareComponent.m_trophyPointsProgressLTD != null && component != null)
			{
				component.sizeDelta = new Vector2(value * this.m_containerWidth, component.sizeDelta.y);
				CAchieveShareComponent.m_trophyPointsProgressLTD.cancel();
				CAchieveShareComponent.m_trophyPointsProgressLTD = null;
			}
		}

		private void DoTrophyPointsAddTweenEnd()
		{
			Text component = this.m_shareForm.GetWidget(17).GetComponent<Text>();
			if (CAchieveShareComponent.m_TrophyPointsAddLTD != null && component != null)
			{
				component.text = string.Format("+{0}", this.m_curAchieveItem.Cfg.dwPoint.ToString("N0"));
				CAchieveShareComponent.m_TrophyPointsAddLTD.cancel();
				CAchieveShareComponent.m_TrophyPointsAddLTD = null;
			}
		}

		private void OnContinueProcessDoneAchievements(CUIEvent uiEvent)
		{
			this.m_isShowing = false;
			CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
			if (masterAchieveInfo.MostLatelyDoneAchievements.Count == 0)
			{
				return;
			}
			masterAchieveInfo.MostLatelyDoneAchievements.RemoveAt(0);
			Singleton<CTimerManager>.GetInstance().AddTimer(200, 1, delegate(int sequence)
			{
				this.Process(false);
			});
		}
	}
}
