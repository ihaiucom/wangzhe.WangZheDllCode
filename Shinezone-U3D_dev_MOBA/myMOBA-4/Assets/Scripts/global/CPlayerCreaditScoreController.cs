using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

[MessageHandlerClass]
public class CPlayerCreaditScoreController : Singleton<CPlayerCreaditScoreController>
{
	private ResCreditLevelInfo m_CreditLevelInfo;

	public override void Init()
	{
		base.Init();
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_Credit_Score_Reward_Enable, new CUIEventManager.OnUIEventHandler(this.OnRewardEnable));
	}

	public override void UnInit()
	{
		base.UnInit();
		this.m_CreditLevelInfo = null;
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Credit_Score_Reward_Enable, new CUIEventManager.OnUIEventHandler(this.OnRewardEnable));
	}

	private void OnRewardEnable(CUIEvent uiEvent)
	{
		CUIListElementScript cUIListElementScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
		if (cUIListElementScript == null)
		{
			return;
		}
		int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
		if (this.m_CreditLevelInfo == null || srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_CreditLevelInfo.astCreditRewardDetail.Length)
		{
			return;
		}
		Text componetInChild = Utility.GetComponetInChild<Text>(cUIListElementScript.gameObject, "itemCell/ItemName");
		Image componetInChild2 = Utility.GetComponetInChild<Image>(cUIListElementScript.gameObject, "itemCell/imgIcon");
		componetInChild.text = this.m_CreditLevelInfo.astCreditRewardDetail[srcWidgetIndexInBelongedList].szCreditRewardItemDesc;
		componetInChild2.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, this.m_CreditLevelInfo.astCreditRewardDetail[srcWidgetIndexInBelongedList].szCreditRewardItemIcon), uiEvent.m_srcFormScript, true, false, false, false);
	}

	public bool Loaded(CUIFormScript form)
	{
		if (form == null)
		{
			return false;
		}
		GameObject widget = form.GetWidget(9);
		if (widget == null)
		{
			return false;
		}
		GameObject x = Utility.FindChild(widget, "pnlCreditScoreInfo");
		return !(x == null);
	}

	public void Load(CUIFormScript form)
	{
		if (form == null)
		{
			return;
		}
		CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Player/CreditScoreInfo", "pnlCreditScoreInfo", form.GetWidget(9), form);
	}

	public void Draw(CUIFormScript form)
	{
		this.UpdateCreditScore(form);
	}

	private void UpdateCreditScore(CUIFormScript form)
	{
		if (form == null)
		{
			return;
		}
		GameObject widget = form.GetWidget(9);
		if (widget == null)
		{
			return;
		}
		GameObject gameObject = Utility.FindChild(widget, "pnlCreditScoreInfo");
		if (gameObject == null)
		{
			return;
		}
		gameObject.CustomSetActive(true);
		CPlayerProfile profile = Singleton<CPlayerInfoSystem>.GetInstance().GetProfile();
		uint creditScore = profile.creditScore;
		this.m_CreditLevelInfo = this.GetCreditLevelInfoByScore((int)creditScore);
		if (this.m_CreditLevelInfo == null)
		{
			return;
		}
		Text componetInChild = Utility.GetComponetInChild<Text>(gameObject, "pnlContainer/pnlCreditScore/CreditValue/ScoreValue");
		GameObject gameObject2 = Utility.FindChild(gameObject, "pnlContainer/pnlCreditScore/CreditValue/CreditLevel/LevelValue");
		GameObject gameObject3 = Utility.FindChild(gameObject, "pnlContainer/pnlCreditAward/SelfAward");
		GameObject gameObject4 = Utility.FindChild(gameObject, "pnlContainer/pnlCreditAward/ComplaintInfo");
		if (componetInChild != null)
		{
			componetInChild.text = creditScore.ToString();
		}
		Image componetInChild2 = Utility.GetComponetInChild<Image>(gameObject, "pnlContainer/pnlCreditScore/IconBg");
		if (componetInChild2 != null)
		{
			componetInChild2.SetSprite(this.GetBgByCreditLevel((RES_CREDIT_LEVEL_TYPE)this.m_CreditLevelInfo.bCreditLevel), form, true, false, false, false);
		}
		if (gameObject2 != null)
		{
			for (int i = 0; i < 3; i++)
			{
				gameObject2.transform.GetChild(i).gameObject.CustomSetActive((int)this.m_CreditLevelInfo.bCreditLevel > i);
			}
		}
		if (gameObject3 != null)
		{
			GameObject obj = Utility.FindChild(gameObject3, "Title-Red");
			GameObject gameObject5 = Utility.FindChild(gameObject3, "TitleTxt_Red");
			GameObject obj2 = Utility.FindChild(gameObject3, "Title-Blue");
			GameObject gameObject6 = Utility.FindChild(gameObject3, "TitleTxt_Blue");
			if (this.m_CreditLevelInfo.bCreditLevelResult == 0)
			{
				obj.CustomSetActive(true);
				gameObject5.CustomSetActive(true);
				obj2.CustomSetActive(false);
				gameObject6.CustomSetActive(false);
				if (gameObject5 != null)
				{
					Text component = gameObject5.GetComponent<Text>();
					component.text = Singleton<CTextManager>.instance.GetText("Credit_Punish_Title");
				}
			}
			else
			{
				obj.CustomSetActive(false);
				gameObject5.CustomSetActive(false);
				obj2.CustomSetActive(true);
				gameObject6.CustomSetActive(true);
				if (gameObject6 != null)
				{
					Text component2 = gameObject6.GetComponent<Text>();
					component2.text = Singleton<CTextManager>.instance.GetText("Credit_Reward_Title");
				}
			}
			int num = 0;
			for (int j = 0; j < this.m_CreditLevelInfo.astCreditRewardDetail.Length; j++)
			{
				if (!string.IsNullOrEmpty(this.m_CreditLevelInfo.astCreditRewardDetail[j].szCreditRewardItemIcon))
				{
					num++;
				}
			}
			CUIListScript componetInChild3 = Utility.GetComponetInChild<CUIListScript>(gameObject3, "pnlAward");
			if (componetInChild3 != null)
			{
				componetInChild3.SetElementAmount(num);
			}
			if (num > 0)
			{
				gameObject3.CustomSetActive(true);
			}
			else
			{
				gameObject3.CustomSetActive(false);
			}
		}
		if (gameObject4 != null)
		{
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(302u).dwConfValue;
			Text componetInChild4 = Utility.GetComponetInChild<Text>(gameObject4, "Progress/progressBg/txtProgress");
			Image componetInChild5 = Utility.GetComponetInChild<Image>(gameObject4, "Tips/Image");
			Text componetInChild6 = Utility.GetComponetInChild<Text>(gameObject4, "Tips/Text");
			float num2 = Utility.Divide((uint)profile.sumDelCreditValue, dwConfValue);
			GameObject gameObject7;
			if (num2 * 100f > 66f)
			{
				gameObject7 = Utility.FindChild(gameObject4, "Progress/progressBg/imgProgress_Red");
				Utility.FindChild(gameObject4, "Progress/progressBg/imgProgress_Yellow").CustomSetActive(false);
				Utility.FindChild(gameObject4, "Progress/progressBg/imgProgress_Green").CustomSetActive(false);
			}
			else if (num2 * 100f > 33f)
			{
				Utility.FindChild(gameObject4, "Progress/progressBg/imgProgress_Red").CustomSetActive(false);
				gameObject7 = Utility.FindChild(gameObject4, "Progress/progressBg/imgProgress_Yellow");
				Utility.FindChild(gameObject4, "Progress/progressBg/imgProgress_Green").CustomSetActive(false);
			}
			else
			{
				Utility.FindChild(gameObject4, "Progress/progressBg/imgProgress_Red").CustomSetActive(false);
				Utility.FindChild(gameObject4, "Progress/progressBg/imgProgress_Yellow").CustomSetActive(false);
				gameObject7 = Utility.FindChild(gameObject4, "Progress/progressBg/imgProgress_Green");
			}
			gameObject7.CustomSetActive(true);
			gameObject7.GetComponent<Image>().fillAmount = num2;
			componetInChild4.text = string.Format("{0}/{1}", profile.sumDelCreditValue, dwConfValue);
			componetInChild5.SetSprite(this.GetTipsIconByProportion(num2), form, true, false, false, false);
			componetInChild6.text = this.GetTipsByComplaintTypeAndProportion(profile.mostDelCreditType, num2);
		}
	}

	private string GetBgByCreditLevel(RES_CREDIT_LEVEL_TYPE level)
	{
		switch (level)
		{
		case RES_CREDIT_LEVEL_TYPE.RES_CREDIT_LEVEL_TYPE_POOR:
			return CUIUtility.s_Sprite_Dynamic_Icon_Dir + "160006";
		case RES_CREDIT_LEVEL_TYPE.RES_CREDIT_LEVEL_TYPE_GOOD:
			return CUIUtility.s_Sprite_Dynamic_Icon_Dir + "160007";
		case RES_CREDIT_LEVEL_TYPE.RES_CREDIT_LEVEL_TYPE_EXCELLENT:
			return CUIUtility.s_Sprite_Dynamic_Icon_Dir + "160008";
		default:
			return CUIUtility.s_Sprite_Dynamic_Icon_Dir + "160008";
		}
	}

	private string GetSuffixByProportion(float proportion)
	{
		string result = "Green";
		if (proportion * 100f > 66f)
		{
			result = "Red";
		}
		else if (proportion * 100f > 33f)
		{
			result = "Yellow";
		}
		return result;
	}

	private string GetTipsByComplaintTypeAndProportion(uint type, float proportion)
	{
		string suffixByProportion = this.GetSuffixByProportion(proportion);
		return Singleton<CTextManager>.GetInstance().GetText(string.Format("Credit_Score_Tips_{0}_{1}", (COM_CHGCREDIT_TYPE)type, suffixByProportion));
	}

	private string GetTipsIconByProportion(float proportion)
	{
		if (proportion * 100f > 66f)
		{
			return CUIUtility.s_Sprite_Dynamic_Icon_Dir + "160003";
		}
		if (proportion * 100f > 33f)
		{
			return CUIUtility.s_Sprite_Dynamic_Icon_Dir + "160002";
		}
		return CUIUtility.s_Sprite_Dynamic_Icon_Dir + "160001";
	}

	public ResCreditLevelInfo GetCreditLevelInfoByScore(int creditScore)
	{
		ResCreditLevelInfo resCreditLevelInfo = GameDataMgr.creditLevelDatabin.GetAnyData();
		int count = GameDataMgr.creditLevelDatabin.count;
		for (int i = 0; i < count; i++)
		{
			resCreditLevelInfo = GameDataMgr.creditLevelDatabin.GetDataByIndex(i);
			if ((ulong)resCreditLevelInfo.dwCreditThresholdLow <= (ulong)((long)creditScore) && (ulong)resCreditLevelInfo.dwCreditThresholdHigh >= (ulong)((long)creditScore))
			{
				break;
			}
		}
		return resCreditLevelInfo;
	}

	public ResCreditLevelInfo GetCreditLevelInfo(int creditLevel)
	{
		ResCreditLevelInfo resCreditLevelInfo = null;
		int count = GameDataMgr.creditLevelDatabin.count;
		for (int i = 0; i < count; i++)
		{
			resCreditLevelInfo = GameDataMgr.creditLevelDatabin.GetDataByIndex(i);
			if ((int)resCreditLevelInfo.bCreditLevel == creditLevel)
			{
				return resCreditLevelInfo;
			}
			resCreditLevelInfo = null;
		}
		return resCreditLevelInfo;
	}
}
