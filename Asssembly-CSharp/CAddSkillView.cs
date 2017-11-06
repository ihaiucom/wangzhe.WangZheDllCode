using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CAddSkillView
{
	public static readonly Color SELECTED_COLOR = Color.white;

	public static readonly Color UN_SELECTED_COLOR = new Color(0.333333343f, 0.5019608f, 0.5882353f);

	public static readonly uint HeroID = 109u;

	public static void OpenForm(GameObject form)
	{
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
		int num = GameDataMgr.addedSkiilDatabin.Count();
		CUIToggleListScript component = form.transform.Find("Panel_Grid/ToggleList").GetComponent<CUIToggleListScript>();
		component.SetElementAmount(num);
		form.transform.Find("Panel_TopBg/LevelText").GetComponent<Text>().set_text((masterRoleInfo != null) ? Singleton<CTextManager>.instance.GetText("Added_Skill_Common_Tips_2", new string[]
		{
			masterRoleInfo.PvpLevel.ToString()
		}) : Singleton<CTextManager>.instance.GetText("Added_Skill_Common_Tips_2", new string[]
		{
			"1"
		}));
		ResSkillUnlock dataByIndex;
		for (int i = 0; i < num; i++)
		{
			CUIListElementScript elemenet = component.GetElemenet(i);
			CUIEventScript component2 = elemenet.GetComponent<CUIEventScript>();
			dataByIndex = GameDataMgr.addedSkiilDatabin.GetDataByIndex(i);
			uint dwUnlockSkillID = dataByIndex.dwUnlockSkillID;
			ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey(dwUnlockSkillID);
			bool flag = masterRoleInfo == null || masterRoleInfo.PvpLevel < (uint)dataByIndex.wAcntLevel;
			if (dataByKey != null)
			{
				string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey.szIconPath));
				Image component3 = elemenet.transform.Find("Icon").GetComponent<Image>();
				component3.SetSprite(prefabPath, form.GetComponent<CUIFormScript>(), true, false, false, false);
				component2.m_onClickEventID = enUIEventID.AddedSkill_GetDetail;
				component2.m_onClickEventParams.tag = (int)dataByIndex.wAcntLevel;
				elemenet.transform.Find("SkillNameTxt").GetComponent<Text>().set_text(Utility.UTF8Convert(dataByKey.szSkillName));
				elemenet.transform.Find("Lock").gameObject.CustomSetActive(flag);
				component3.set_color(flag ? CUIUtility.s_Color_GrayShader : Color.white);
				if (flag)
				{
					Utility.GetComponetInChild<Text>(elemenet.gameObject, "Lock/Text").set_text(Singleton<CTextManager>.instance.GetText("Added_Skill_Common_Tips_3", new string[]
					{
						dataByIndex.wAcntLevel.ToString()
					}));
				}
				if ((ulong)dwUnlockSkillID == (ulong)((long)CAddSkillSys.SendSkillId))
				{
					CUICommonSystem.SetObjActive(elemenet.transform.FindChild("HelpBtn"), true);
					if (masterRoleInfo.PvpLevel >= (uint)dataByIndex.wAcntLevel)
					{
						Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(elemenet.gameObject, enNewFlagKey.New_SendSkill_V14, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
					}
				}
			}
			else
			{
				DebugHelper.Assert(false, string.Format("ResSkillCfgInfo[{0}] can not be found!", dwUnlockSkillID));
			}
		}
		dataByIndex = GameDataMgr.addedSkiilDatabin.GetDataByIndex(0);
		if (dataByIndex != null)
		{
			component.SelectElement(0, true);
			CAddSkillView.OnRefresh(form, dataByIndex.wAcntLevel);
		}
		if (CSysDynamicBlock.bLobbyEntryBlocked)
		{
			CUIToggleListScript component4 = form.transform.FindChild("Panel_Grid/ToggleList").GetComponent<CUIToggleListScript>();
			CUIListElementScript elemenet2 = component4.GetElemenet(10);
			if (elemenet2)
			{
				elemenet2.gameObject.CustomSetActive(false);
			}
			Transform transform = form.transform.FindChild("Skill-Send-Test");
			if (transform != null)
			{
				transform.gameObject.CustomSetActive(false);
			}
		}
	}

	public static void OnRefresh(GameObject form, ushort addedSkillLevel)
	{
		CUIToggleListScript component = form.transform.Find("Panel_Grid/ToggleList").GetComponent<CUIToggleListScript>();
		CUIListElementScript cUIListElementScript = null;
		int selected = component.GetSelected();
		for (int i = 0; i < component.GetElementAmount(); i++)
		{
			cUIListElementScript = component.GetElemenet(i);
			if (i == selected)
			{
				cUIListElementScript.transform.Find("SkillNameTxt").GetComponent<Text>().set_color(CAddSkillView.SELECTED_COLOR);
			}
			else
			{
				cUIListElementScript.transform.Find("SkillNameTxt").GetComponent<Text>().set_color(CAddSkillView.UN_SELECTED_COLOR);
			}
		}
		ResSkillUnlock dataByKey = GameDataMgr.addedSkiilDatabin.GetDataByKey((uint)addedSkillLevel);
		uint dwUnlockSkillID = dataByKey.dwUnlockSkillID;
		ResSkillCfgInfo dataByKey2 = GameDataMgr.skillDatabin.GetDataByKey(dwUnlockSkillID);
		if (dataByKey2 == null)
		{
			DebugHelper.Assert(false, string.Format("ResSkillCfgInfo[{0}] can not be found!", dwUnlockSkillID));
			return;
		}
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
		bool flag = masterRoleInfo == null || masterRoleInfo.PvpLevel < (uint)dataByKey.wAcntLevel;
		Image component2 = form.transform.Find("Panel_SkillDesc/IconImg").GetComponent<Image>();
		Image component3 = form.transform.Find("Panel_SkillDesc/ContentImg").GetComponent<Image>();
		Text component4 = form.transform.Find("Panel_SkillDesc/SkillNameTxt").GetComponent<Text>();
		Text component5 = form.transform.Find("Panel_SkillDesc/SkillNameTxt2").GetComponent<Text>();
		Text component6 = form.transform.Find("Panel_SkillDesc/SkillUnlockTxt").GetComponent<Text>();
		Text component7 = form.transform.Find("Panel_SkillDesc/SkillDescTxt").GetComponent<Text>();
		string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey2.szIconPath));
		component2.SetSprite(prefabPath, form.GetComponent<CUIFormScript>(), true, false, false, false);
		prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_AddedSkill_Dir, dwUnlockSkillID);
		component3.SetSprite(prefabPath, form.GetComponent<CUIFormScript>(), true, false, false, false);
		Text text = component4;
		string text2 = Utility.UTF8Convert(dataByKey2.szSkillName);
		component5.set_text(text2);
		text.set_text(text2);
		component6.set_text(string.Format("Lv.{0}", dataByKey.wAcntLevel));
		component7.set_text(CUICommonSystem.GetSkillDescLobby(dataByKey2.szSkillDesc, CAddSkillView.HeroID));
		if (flag)
		{
			component6.set_text(Singleton<CTextManager>.instance.GetText("Added_Skill_Common_Tips_3", new string[]
			{
				dataByKey.wAcntLevel.ToString()
			}));
		}
		else
		{
			component6.set_text(Singleton<CTextManager>.instance.GetText("Added_Skill_Common_Tips_4"));
		}
		if ((ulong)dwUnlockSkillID == (ulong)((long)CAddSkillSys.SendSkillId) && masterRoleInfo.PvpLevel >= (uint)addedSkillLevel)
		{
			Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(21u, null, false);
			Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(cUIListElementScript.gameObject, enNewFlagKey.New_SendSkill_V14, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
		}
	}

	public static bool NewPlayerLevelUnlockAddSkill(int inNewLevel, int inOldLevel, out uint outSkillId)
	{
		bool result = false;
		outSkillId = 0u;
		int num = GameDataMgr.addedSkiilDatabin.Count();
		for (int i = 0; i < num; i++)
		{
			ResSkillUnlock dataByIndex = GameDataMgr.addedSkiilDatabin.GetDataByIndex(i);
			if (dataByIndex != null)
			{
				int wAcntLevel = (int)dataByIndex.wAcntLevel;
				if (inNewLevel >= wAcntLevel && inOldLevel < wAcntLevel)
				{
					outSkillId = dataByIndex.dwUnlockSkillID;
					result = true;
					break;
				}
			}
		}
		return result;
	}
}
