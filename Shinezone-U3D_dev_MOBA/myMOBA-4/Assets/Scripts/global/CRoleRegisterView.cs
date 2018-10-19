using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CRoleRegisterView
{
	public enum enHeroTypeSelectWidgets
	{
		enTankTypeToggle,
		enAdTypeToggle,
		enApTypeToggle
	}

	public static string RoleName
	{
		get
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CRoleRegisterSys.s_roleCreateFormPath);
			if (form != null)
			{
				return CUIUtility.RemoveEmoji(form.transform.FindChild("NameInputText").GetComponent<InputField>().text);
			}
			return string.Empty;
		}
		set
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CRoleRegisterSys.s_roleCreateFormPath);
			if (form != null)
			{
				InputField component = form.transform.FindChild("NameInputText").GetComponent<InputField>();
				component.text = value;
				component.MoveTextEnd(false);
			}
		}
	}

	public static void DeactivateInputField()
	{
		CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CRoleRegisterSys.s_roleCreateFormPath);
		if (form != null)
		{
			InputField component = form.transform.FindChild("NameInputText").GetComponent<InputField>();
			component.DeactivateInputField();
		}
	}

	public static void OpenGameDifSelectForm()
	{
		CUIFormScript cUIFormScript = Singleton<CUIManager>.instance.OpenForm(CRoleRegisterSys.s_gameDifficultSelectFormPath, false, true);
		if (cUIFormScript != null)
		{
			GameObject gameObject = Utility.FindChild(cUIFormScript.gameObject, "ToggleGroup/Toggle1");
			gameObject.GetComponent<CUIEventScript>().m_onClickEventParams.tag = 1;
			gameObject = Utility.FindChild(cUIFormScript.gameObject, "ToggleGroup/Toggle2");
			gameObject.GetComponent<CUIEventScript>().m_onClickEventParams.tag = 2;
			gameObject = Utility.FindChild(cUIFormScript.gameObject, "ToggleGroup/Toggle3");
			gameObject.GetComponent<CUIEventScript>().m_onClickEventParams.tag = 3;
			CRoleRegisterView.SetGameDifficult(0);
		}
	}

	public static void CloseGameDifSelectForm()
	{
		Singleton<CUIManager>.instance.CloseForm(CRoleRegisterSys.s_gameDifficultSelectFormPath);
	}

	public static void SetGameDifficult(int difficult)
	{
		CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CRoleRegisterSys.s_gameDifficultSelectFormPath);
		if (form != null)
		{
			GameObject gameObject = Utility.FindChild(form.gameObject, "ConfirmBtn").gameObject;
			GameObject gameObject2 = Utility.FindChild(form.gameObject, "Panel/LevelContent").gameObject;
			gameObject.CustomSetActive(true);
			gameObject2.CustomSetActive(false);
			if (form != null)
			{
				gameObject.GetComponent<CUIEventScript>().m_onClickEventParams.tag = difficult;
				switch (difficult)
				{
				case 1:
					gameObject.GetComponent<CUIEventScript>().enabled = true;
					gameObject.GetComponent<Button>().interactable = true;
					gameObject.GetComponentInChildren<Text>().color = Color.white;
					break;
				case 2:
					gameObject.GetComponent<CUIEventScript>().enabled = true;
					gameObject.GetComponent<Button>().interactable = true;
					gameObject.GetComponentInChildren<Text>().color = Color.white;
					break;
				case 3:
					gameObject.GetComponent<CUIEventScript>().enabled = true;
					gameObject.GetComponent<Button>().interactable = true;
					gameObject.GetComponentInChildren<Text>().color = Color.white;
					break;
				default:
					gameObject.GetComponent<CUIEventScript>().enabled = false;
					gameObject.GetComponent<Button>().interactable = false;
					gameObject.GetComponentInChildren<Text>().color = Color.gray;
					break;
				}
			}
		}
	}

	public static void OpenHeroTypeSelectForm()
	{
		CUIFormScript cUIFormScript = Singleton<CUIManager>.instance.OpenForm(CRoleRegisterSys.s_heroTypeSelectFormPath, false, true);
		if (cUIFormScript != null)
		{
			GameObject gameObject = Utility.FindChild(cUIFormScript.gameObject, "ToggleGroup/Toggle1");
			gameObject.GetComponent<CUIEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE3);
			gameObject = Utility.FindChild(cUIFormScript.gameObject, "ToggleGroup/Toggle2");
			gameObject.GetComponent<CUIEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1);
			gameObject = Utility.FindChild(cUIFormScript.gameObject, "ToggleGroup/Toggle3");
			gameObject.GetComponent<CUIEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE2);
			CRoleRegisterView.SetHeroType(0u);
		}
	}

	public static void SetHeroType(uint heroType)
	{
		CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CRoleRegisterSys.s_heroTypeSelectFormPath);
		if (form != null)
		{
			GameObject gameObject = Utility.FindChild(form.gameObject, "ConfirmBtn").gameObject;
			gameObject.CustomSetActive(true);
			GameObject widget = form.GetWidget(0);
			GameObject widget2 = form.GetWidget(1);
			GameObject widget3 = form.GetWidget(2);
			gameObject.GetComponent<CUIEventScript>().m_onClickEventParams.tag = (int)heroType;
			if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1))
			{
				gameObject.GetComponent<CUIEventScript>().enabled = true;
				gameObject.GetComponent<Button>().interactable = true;
				gameObject.GetComponentInChildren<Text>().color = Color.white;
				widget.transform.FindChild("Background/Checkmark").gameObject.CustomSetActive(true);
				widget2.transform.FindChild("Background/Checkmark").gameObject.CustomSetActive(false);
				widget3.transform.FindChild("Background/Checkmark").gameObject.CustomSetActive(false);
				widget.transform.FindChild("Mask").gameObject.CustomSetActive(false);
				widget2.transform.FindChild("Mask").gameObject.CustomSetActive(true);
				widget3.transform.FindChild("Mask").gameObject.CustomSetActive(true);
			}
			else if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE2))
			{
				gameObject.GetComponent<CUIEventScript>().enabled = true;
				gameObject.GetComponent<Button>().interactable = true;
				gameObject.GetComponentInChildren<Text>().color = Color.white;
				widget.transform.FindChild("Background/Checkmark").gameObject.CustomSetActive(false);
				widget2.transform.FindChild("Background/Checkmark").gameObject.CustomSetActive(true);
				widget3.transform.FindChild("Background/Checkmark").gameObject.CustomSetActive(false);
				widget.transform.FindChild("Mask").gameObject.CustomSetActive(true);
				widget2.transform.FindChild("Mask").gameObject.CustomSetActive(false);
				widget3.transform.FindChild("Mask").gameObject.CustomSetActive(true);
			}
			else if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE3))
			{
				gameObject.GetComponent<CUIEventScript>().enabled = true;
				gameObject.GetComponent<Button>().interactable = true;
				gameObject.GetComponentInChildren<Text>().color = Color.white;
				widget.transform.FindChild("Background/Checkmark").gameObject.CustomSetActive(false);
				widget2.transform.FindChild("Background/Checkmark").gameObject.CustomSetActive(false);
				widget3.transform.FindChild("Background/Checkmark").gameObject.CustomSetActive(true);
				widget.transform.FindChild("Mask").gameObject.CustomSetActive(true);
				widget2.transform.FindChild("Mask").gameObject.CustomSetActive(true);
				widget3.transform.FindChild("Mask").gameObject.CustomSetActive(false);
			}
			else
			{
				gameObject.GetComponent<CUIEventScript>().enabled = false;
				gameObject.GetComponent<Button>().interactable = false;
				gameObject.GetComponentInChildren<Text>().color = Color.gray;
				widget.transform.FindChild("Background/Checkmark").gameObject.CustomSetActive(false);
				widget2.transform.FindChild("Background/Checkmark").gameObject.CustomSetActive(false);
				widget3.transform.FindChild("Background/Checkmark").gameObject.CustomSetActive(false);
				widget.transform.FindChild("Mask").gameObject.CustomSetActive(false);
				widget2.transform.FindChild("Mask").gameObject.CustomSetActive(false);
				widget3.transform.FindChild("Mask").gameObject.CustomSetActive(false);
			}
			CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
			component.m_onClickEventParams.tagUInt = heroType;
		}
	}

	public static void RefreshRecommendTips()
	{
		CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CRoleRegisterSys.s_heroTypeSelectFormPath);
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (form != null && masterRoleInfo != null)
		{
			GameObject obj = null;
			if (masterRoleInfo.acntMobaInfo.bMobaUsedType == 1)
			{
				uint guideLevelHeroTypeBtMobaHeroType = CRoleRegisterView.GetGuideLevelHeroTypeBtMobaHeroType(masterRoleInfo.acntMobaInfo.iRecommendHeroType);
				if (guideLevelHeroTypeBtMobaHeroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1))
				{
					obj = form.GetWidget(0).transform.FindChild("TuiJian").gameObject;
				}
				else if (guideLevelHeroTypeBtMobaHeroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE2))
				{
					obj = form.GetWidget(1).transform.FindChild("TuiJian").gameObject;
				}
				else if (guideLevelHeroTypeBtMobaHeroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE3))
				{
					obj = form.GetWidget(2).transform.FindChild("TuiJian").gameObject;
				}
			}
			else if (masterRoleInfo.acntMobaInfo.bMobaUsedType == 2)
			{
				obj = form.GetWidget(0).transform.FindChild("TuiJian").gameObject;
			}
			obj.CustomSetActive(true);
		}
	}

	public static uint GetGuideLevelHeroTypeBtMobaHeroType(int mobaHeroType)
	{
		uint globeValue = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1);
		uint globeValue2 = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE2);
		uint globeValue3 = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE3);
		switch (mobaHeroType + 1)
		{
		case 0:
		case 1:
		case 2:
			return globeValue;
		case 3:
			return globeValue3;
		case 4:
		case 5:
			return globeValue;
		case 6:
			return globeValue2;
		case 7:
			return globeValue3;
		default:
			return globeValue;
		}
	}
}
