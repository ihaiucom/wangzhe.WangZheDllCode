using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;

public class CAddSkillSys : Singleton<CAddSkillSys>
{
	public const string ADD_SKILL_FORM_PATH = "UGUI/Form/System/AddedSkill/Form_AddedSkill.prefab";

	public static int SendSkillId = 80117;

	public override void Init()
	{
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.AddedSkill_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenForm));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.AddedSkill_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.AddedSkill_GetDetail, new CUIEventManager.OnUIEventHandler(this.OnGetDetail));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.AddedSkill_ShowChuanSongHelp, new CUIEventManager.OnUIEventHandler(this.OnShowChuanSongHelp));
		base.Init();
	}

	public override void UnInit()
	{
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.AddedSkill_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenForm));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.AddedSkill_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.AddedSkill_GetDetail, new CUIEventManager.OnUIEventHandler(this.OnGetDetail));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.AddedSkill_ShowChuanSongHelp, new CUIEventManager.OnUIEventHandler(this.OnShowChuanSongHelp));
		base.UnInit();
	}

	private void OnOpenForm(CUIEvent cuiEvent)
	{
		CUICommonSystem.ResetLobbyFormFadeRecover();
		if (!this.IsOpenAddSkillSys())
		{
			return;
		}
		CUIFormScript cUIFormScript = Singleton<CUIManager>.instance.OpenForm("UGUI/Form/System/AddedSkill/Form_AddedSkill.prefab", false, true);
		if (cUIFormScript != null)
		{
			CAddSkillView.OpenForm(cUIFormScript.gameObject);
		}
		CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_AddSkillBtn);
		Singleton<CUINewFlagSystem>.instance.SetNewFlagForLobbyAddedSkill(false);
	}

	public bool IsOpenAddSkillSys()
	{
		if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL))
		{
			return true;
		}
		ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(22u);
		Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
		return false;
	}

	private void OnCloseForm(CUIEvent cuiEvent)
	{
		Singleton<CUIManager>.instance.CloseForm("UGUI/Form/System/AddedSkill/Form_AddedSkill.prefab");
		Singleton<CResourceManager>.instance.UnloadUnusedAssets();
	}

	private void OnGetDetail(CUIEvent cuiEvent)
	{
		CUIFormScript form = Singleton<CUIManager>.instance.GetForm("UGUI/Form/System/AddedSkill/Form_AddedSkill.prefab");
		if (form != null && !form.IsHided())
		{
			CAddSkillView.OnRefresh(form.gameObject, (ushort)cuiEvent.m_eventParams.tag);
		}
	}

	private void OnShowChuanSongHelp(CUIEvent uiEvent)
	{
		Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(20u, null, false);
	}

	public static bool IsSelSkillAvailable()
	{
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
		return Singleton<CHeroSelectBaseSystem>.instance.IsMobaMode() && Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL) && masterRoleInfo != null;
	}

	public static ListView<ResSkillUnlock> GetSelSkillAvailable(ResDT_UnUseSkill unUseSkillInfo)
	{
		ListView<ResSkillUnlock> listView = new ListView<ResSkillUnlock>();
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
		if (unUseSkillInfo == null)
		{
			return listView;
		}
		for (int i = 0; i < GameDataMgr.addedSkiilDatabin.count; i++)
		{
			ResSkillUnlock dataByIndex = GameDataMgr.addedSkiilDatabin.GetDataByIndex(i);
			if (masterRoleInfo != null && masterRoleInfo.PvpLevel >= (uint)dataByIndex.wAcntLevel)
			{
				bool flag = true;
				if (unUseSkillInfo != null)
				{
					for (int j = 0; j < unUseSkillInfo.UnUseSkillList.Length; j++)
					{
						if (unUseSkillInfo.UnUseSkillList[j] == dataByIndex.dwUnlockSkillID)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					listView.Add(dataByIndex);
				}
			}
		}
		return listView;
	}

	public static bool IsSelSkillAvailable(ResDT_UnUseSkill unUseSkillInfo, uint selSkillId)
	{
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
		ResSkillUnlock resSkillUnlock = null;
		if (unUseSkillInfo == null)
		{
			return false;
		}
		for (int i = 0; i < GameDataMgr.addedSkiilDatabin.count; i++)
		{
			ResSkillUnlock dataByIndex = GameDataMgr.addedSkiilDatabin.GetDataByIndex(i);
			if (dataByIndex.dwUnlockSkillID == selSkillId)
			{
				resSkillUnlock = dataByIndex;
				break;
			}
		}
		if (resSkillUnlock != null && masterRoleInfo != null && masterRoleInfo.PvpLevel >= (uint)resSkillUnlock.wAcntLevel)
		{
			if (unUseSkillInfo != null)
			{
				for (int j = 0; j < unUseSkillInfo.UnUseSkillList.Length; j++)
				{
					if (unUseSkillInfo.UnUseSkillList[j] == resSkillUnlock.dwUnlockSkillID)
					{
						return false;
					}
				}
			}
			return true;
		}
		return false;
	}

	public static uint GetSelfSelSkill(ResDT_UnUseSkill unUseSkillInfo, uint heroId)
	{
		if (!CAddSkillSys.IsSelSkillAvailable())
		{
			return 0u;
		}
		if (unUseSkillInfo == null)
		{
			return 0u;
		}
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
		if (masterRoleInfo == null)
		{
			return 0u;
		}
		uint num = 0u;
		CHeroInfo heroInfo = masterRoleInfo.GetHeroInfo(heroId, true);
		if (heroInfo != null)
		{
			num = heroInfo.skillInfo.SelSkillID;
		}
		else if (masterRoleInfo.IsFreeHero(heroId))
		{
			COMDT_FREEHERO_INFO freeHeroSymbol = masterRoleInfo.GetFreeHeroSymbol(heroId);
			if (freeHeroSymbol != null)
			{
				num = freeHeroSymbol.dwSkillID;
			}
		}
		if (!CAddSkillSys.IsSelSkillAvailable(unUseSkillInfo, num))
		{
			num = GameDataMgr.addedSkiilDatabin.GetAnyData().dwUnlockSkillID;
		}
		if (!CAddSkillSys.IsSelSkillAvailable(unUseSkillInfo, num))
		{
			num = GameDataMgr.globalInfoDatabin.GetDataByKey(154u).dwConfValue;
		}
		return num;
	}
}
