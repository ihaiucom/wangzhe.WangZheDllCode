using Assets.Scripts.UI;
using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CExperienceCardSystem : Singleton<CExperienceCardSystem>
	{
		public static string HeroExperienceCardFramePath = CUIUtility.s_Sprite_Dynamic_ExperienceCard_Dir + "Bg_card_hreo";

		public static string SkinExperienceCardFramePath = CUIUtility.s_Sprite_Dynamic_ExperienceCard_Dir + "Bg_card_skin";

		public static string HeroExperienceCardMarkPath = CUIUtility.s_Sprite_Dynamic_ExperienceCard_Dir + "Img_free_hero";

		public static string SkinExperienceCardMarkPath = CUIUtility.s_Sprite_Dynamic_ExperienceCard_Dir + "Img_free_skin";

		public override void Init()
		{
			Singleton<EventRouter>.GetInstance().AddEventHandler<uint, int>("HeroExperienceAdd", new Action<uint, int>(this.OnExperienceHeroAdd));
			Singleton<EventRouter>.GetInstance().AddEventHandler<string, uint, uint>("HeroExperienceTimeUpdate", new Action<string, uint, uint>(this.OnHeroExperienceTimeUpdate));
		}

		private void OnExperienceHeroAdd(uint heroId, int experienceDays)
		{
			if (!Singleton<CHeroSelectBaseSystem>.instance.m_isInHeroSelectState)
			{
				CUICommonSystem.ShowNewHeroOrSkin(heroId, 0u, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority1, 0u, experienceDays);
			}
			else if (!Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode() && Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
			{
				IHeroData selectHeroData = CHeroDataFactory.CreateHeroData(heroId);
				Singleton<CHeroSelectNormalSystem>.instance.HeroSelect_SelectHero(selectHeroData);
			}
		}

		private void OnHeroExperienceTimeUpdate(string heroName, uint oldDeadLine, uint newDeadLine)
		{
			if (0u < oldDeadLine && oldDeadLine < newDeadLine)
			{
				int experienceHeroOrSkinExtendDays = CHeroInfo.GetExperienceHeroOrSkinExtendDays(newDeadLine - oldDeadLine);
				Singleton<CUIManager>.GetInstance().OpenTips("ExpCard_ExtendDays", true, 1f, null, new object[]
				{
					heroName,
					experienceHeroOrSkinExtendDays
				});
			}
		}

		[MessageHandler(1826)]
		public static void OnRecieveLimitSkinAdd(CSPkg msg)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			COMDT_HERO_LIMIT_SKIN_LIST stLimitSkinList = msg.stPkgData.stLimitSkinAdd.stLimitSkinList;
			int num = 0;
			while ((long)num < (long)((ulong)stLimitSkinList.dwNum))
			{
				COMDT_HERO_LIMIT_SKIN cOMDT_HERO_LIMIT_SKIN = stLimitSkinList.astSkinList[num];
				if (masterRoleInfo.heroExperienceSkinDic.ContainsKey(cOMDT_HERO_LIMIT_SKIN.dwSkinID))
				{
					uint num2 = masterRoleInfo.heroExperienceSkinDic[cOMDT_HERO_LIMIT_SKIN.dwSkinID];
					masterRoleInfo.heroExperienceSkinDic[cOMDT_HERO_LIMIT_SKIN.dwSkinID] = cOMDT_HERO_LIMIT_SKIN.dwDeadLine;
					if (0u < num2 && num2 < masterRoleInfo.heroExperienceSkinDic[cOMDT_HERO_LIMIT_SKIN.dwSkinID])
					{
						int experienceHeroOrSkinExtendDays = CHeroInfo.GetExperienceHeroOrSkinExtendDays(masterRoleInfo.heroExperienceSkinDic[cOMDT_HERO_LIMIT_SKIN.dwSkinID] - num2);
						string skinName = CSkinInfo.GetSkinName(cOMDT_HERO_LIMIT_SKIN.dwSkinID);
						Singleton<CUIManager>.GetInstance().OpenTips("ExpCard_ExtendDays", true, 1f, null, new object[]
						{
							skinName,
							experienceHeroOrSkinExtendDays
						});
					}
				}
				else
				{
					masterRoleInfo.heroExperienceSkinDic.Add(cOMDT_HERO_LIMIT_SKIN.dwSkinID, cOMDT_HERO_LIMIT_SKIN.dwDeadLine);
					uint heroId;
					uint skinId;
					CSkinInfo.ResolveHeroSkin(cOMDT_HERO_LIMIT_SKIN.dwSkinID, out heroId, out skinId);
					if (!Singleton<CHeroSelectBaseSystem>.instance.m_isInHeroSelectState)
					{
						CUICommonSystem.ShowNewHeroOrSkin(heroId, skinId, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0u, CHeroInfo.GetExperienceHeroOrSkinValidDays(cOMDT_HERO_LIMIT_SKIN.dwDeadLine));
					}
					else if (!Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode())
					{
						CHeroInfoSystem2.ReqWearHeroSkin(heroId, skinId, true);
					}
				}
				num++;
			}
		}

		[MessageHandler(1827)]
		public static void OnReceiveLimitSkinDel(CSPkg msg)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			SCPKG_LIMITSKIN_DEL stLimitSkinDel = msg.stPkgData.stLimitSkinDel;
			int num = 0;
			while ((long)num < (long)((ulong)stLimitSkinDel.dwNum))
			{
				if (masterRoleInfo.heroExperienceSkinDic.ContainsKey(stLimitSkinDel.SkinID[num]))
				{
					masterRoleInfo.heroExperienceSkinDic.Remove(stLimitSkinDel.SkinID[num]);
				}
				num++;
			}
		}

		[MessageHandler(1828)]
		public static void OnReceiveUseExpCardNtf(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			switch (msg.stPkgData.stUseExpCardNtf.iResult)
			{
			case 2:
				Singleton<CUIManager>.GetInstance().OpenTips("ExpCardError_HeroOwn", true, 1.5f, null, new object[0]);
				break;
			case 3:
				Singleton<CUIManager>.GetInstance().OpenTips("ExpCardError_InsertHero", true, 1.5f, null, new object[0]);
				break;
			case 4:
				Singleton<CUIManager>.GetInstance().OpenTips("ExpCardError_SkinOwn", true, 1.5f, null, new object[0]);
				break;
			case 5:
				Singleton<CUIManager>.GetInstance().OpenTips("ExpCardError_InsertSkin", true, 1.5f, null, new object[0]);
				break;
			case 7:
				Singleton<CUIManager>.GetInstance().OpenTips("ExpCardError_Other", true, 1.5f, null, new object[0]);
				break;
			}
		}
	}
}
