using Assets.Scripts.GameSystem;
using CSProtocol;
using ResData;
using System;

public class NewbieGuideCheckSkipConditionUtil
{
    public static NewbieGuideSkipConditionType TranslateToSkipCond(int inNewbieType)
    {
        NewbieGuideSkipConditionType invalid = NewbieGuideSkipConditionType.Invalid;
        switch (inNewbieType)
        {
            case 0:
                return NewbieGuideSkipConditionType.hasCompleteBaseGuide;

            case 1:
                return NewbieGuideSkipConditionType.hasCompleteEquipping;

            case 2:
                return NewbieGuideSkipConditionType.hasCompleteHeroAdv;

            case 3:
                return NewbieGuideSkipConditionType.hasSummonedHero;

            case 4:
                return NewbieGuideSkipConditionType.hasCompleteHeroStar;

            case 5:
                return NewbieGuideSkipConditionType.hasHeroSkillUpgraded;

            case 6:
                return NewbieGuideSkipConditionType.hasCompleteLottery;

            case 7:
                return NewbieGuideSkipConditionType.hasOverThreeHeroes;

            case 8:
                return NewbieGuideSkipConditionType.hasRewardTaskPvp;

            case 9:
                return NewbieGuideSkipConditionType.hasBoughtHero;

            case 10:
                return NewbieGuideSkipConditionType.hasBoughtItem;

            case 11:
                return NewbieGuideSkipConditionType.hasGotChapterReward;

            case 12:
                return NewbieGuideSkipConditionType.hasMopup;

            case 13:
                return NewbieGuideSkipConditionType.hasEnteredPvP;

            case 14:
                return NewbieGuideSkipConditionType.hasEnteredTrial;

            case 15:
                return NewbieGuideSkipConditionType.hasEnteredZhuangzi;

            case 0x10:
                return NewbieGuideSkipConditionType.hasEnteredBurning;

            case 0x11:
                return NewbieGuideSkipConditionType.hasEnteredElitePvE;

            case 0x12:
                return NewbieGuideSkipConditionType.hasEnteredGuild;

            case 0x13:
                return NewbieGuideSkipConditionType.hasUsedSymbol;

            case 20:
                return NewbieGuideSkipConditionType.hasEnteredMysteryShop;

            case 0x15:
                return NewbieGuideSkipConditionType.hasComplete33Guide;

            case 0x1a:
                return NewbieGuideSkipConditionType.hasCompleteHumanAi33Match;

            case 0x1b:
                return NewbieGuideSkipConditionType.hasCompleteHuman33Match;

            case 0x1d:
                return NewbieGuideSkipConditionType.hasIncreaseEquip;

            case 30:
                return NewbieGuideSkipConditionType.hasAdvancedEquip;

            case 0x20:
                return NewbieGuideSkipConditionType.hasCompleteHeroUp;

            case 0x21:
                return NewbieGuideSkipConditionType.hasEnteredTournament;

            case 0x22:
                return NewbieGuideSkipConditionType.hasRewardTaskPve;

            case 0x2c:
                return NewbieGuideSkipConditionType.hasCompleteHumanAi33;

            case 0x3a:
                return NewbieGuideSkipConditionType.hasManufacuredSymbol;

            case 80:
                return NewbieGuideSkipConditionType.hasCoinDrawFive;

            case 0x51:
                return NewbieGuideSkipConditionType.hasComplete11Match;

            case 0x53:
                return NewbieGuideSkipConditionType.hasCompleteCoronaGuide;

            case 0x55:
                return NewbieGuideSkipConditionType.hasCompleteTrainLevel33;

            case 0x62:
                return NewbieGuideSkipConditionType.hasCompleteTrainLevel55;

            case 0x63:
                return invalid;

            case 100:
                return NewbieGuideSkipConditionType.hasDiamondDraw;
        }
        return invalid;
    }


	public static int TranslateFromSkipCond(NewbieGuideSkipConditionType inCondType)
	{
		int result = -1;
		switch (inCondType)
		{
		case NewbieGuideSkipConditionType.hasCompleteEquipping:
			result = 1;
			return result;
		case NewbieGuideSkipConditionType.hasRewardTaskPvp:
			result = 8;
			return result;
		case NewbieGuideSkipConditionType.hasCompleteHeroAdv:
			result = 2;
			return result;
		case NewbieGuideSkipConditionType.hasSummonedHero:
			result = 3;
			return result;
		case NewbieGuideSkipConditionType.hasOverThreeHeroes:
			result = 7;
			return result;
		case NewbieGuideSkipConditionType.hasCompleteHeroStar:
			result = 4;
			return result;
		case NewbieGuideSkipConditionType.hasHeroSkillUpgraded:
			result = 5;
			return result;
		case NewbieGuideSkipConditionType.hasBoughtHero:
			result = 9;
			return result;
		case NewbieGuideSkipConditionType.hasBoughtItem:
			result = 10;
			return result;
		case NewbieGuideSkipConditionType.hasGotChapterReward:
			result = 11;
			return result;
		case NewbieGuideSkipConditionType.hasMopup:
			result = 12;
			return result;
		case NewbieGuideSkipConditionType.hasEnteredPvP:
			result = 13;
			return result;
		case NewbieGuideSkipConditionType.hasEnteredTrial:
			result = 14;
			return result;
		case NewbieGuideSkipConditionType.hasEnteredZhuangzi:
			result = 15;
			return result;
		case NewbieGuideSkipConditionType.hasEnteredBurning:
			result = 16;
			return result;
		case NewbieGuideSkipConditionType.hasEnteredElitePvE:
			result = 17;
			return result;
		case NewbieGuideSkipConditionType.hasEnteredGuild:
			result = 18;
			return result;
		case NewbieGuideSkipConditionType.hasUsedSymbol:
			result = 19;
			return result;
		case NewbieGuideSkipConditionType.hasEnteredMysteryShop:
			result = 20;
			return result;
		case NewbieGuideSkipConditionType.hasAdvancedEquip:
			result = 30;
			return result;
		case NewbieGuideSkipConditionType.hasCompleteBaseGuide:
			result = 0;
			return result;
		case NewbieGuideSkipConditionType.hasCompleteHumanAi33Match:
			result = 26;
			return result;
		case NewbieGuideSkipConditionType.hasCompleteHuman33Match:
			result = 27;
			return result;
		case NewbieGuideSkipConditionType.hasComplete33Guide:
			result = 21;
			return result;
		case NewbieGuideSkipConditionType.hasCompleteLottery:
			result = 6;
			return result;
		case NewbieGuideSkipConditionType.hasIncreaseEquip:
			result = 29;
			return result;
		case NewbieGuideSkipConditionType.hasRewardTaskPve:
			result = 34;
			return result;
		case NewbieGuideSkipConditionType.hasCompleteHeroUp:
			result = 32;
			return result;
		case NewbieGuideSkipConditionType.hasEnteredTournament:
			result = 33;
			return result;
		case NewbieGuideSkipConditionType.hasCompleteHumanAi33:
			result = 44;
			return result;
		case NewbieGuideSkipConditionType.hasManufacuredSymbol:
			result = 58;
			return result;
		case NewbieGuideSkipConditionType.hasCoinDrawFive:
			result = 80;
			return result;
		case NewbieGuideSkipConditionType.hasCompleteTrainLevel55:
			result = 98;
			return result;
		case NewbieGuideSkipConditionType.hasComplete11Match:
			result = 81;
			return result;
		case NewbieGuideSkipConditionType.hasCompleteTrainLevel33:
			result = 85;
			return result;
		case NewbieGuideSkipConditionType.hasDiamondDraw:
			result = 100;
			return result;
		case NewbieGuideSkipConditionType.hasCompleteCoronaGuide:
			result = 83;
			return result;
		}
		DebugHelper.Assert(false);
		return result;
	}

	public static bool CheckSkipCondition(NewbieGuideSkipConditionItem item, uint[] param)
	{
		switch (item.wType)
		{
		case 1:
		{
			bool result = false;
			if (param != null && param.Length > 0)
			{
				if (param[0] == item.Param[0])
				{
					result = Singleton<CAdventureSys>.GetInstance().IsLevelFinished((int)param[0]);
				}
			}
			else
			{
				result = Singleton<CAdventureSys>.GetInstance().IsLevelFinished((int)item.Param[0]);
			}
			return result;
		}
		case 2:
			return MonoSingleton<NewbieGuideManager>.GetInstance().IsNewbieGuideComplete(item.Param[0]);
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 11:
		case 12:
		case 13:
		case 14:
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
		case 21:
		case 22:
		case 23:
		case 24:
		case 31:
		case 32:
		case 33:
		case 34:
		case 35:
		case 36:
		case 37:
		case 38:
		case 40:
		case 41:
		case 45:
		case 46:
		case 47:
		case 48:
		case 51:
		{
			int num = NewbieGuideCheckSkipConditionUtil.TranslateFromSkipCond((NewbieGuideSkipConditionType)item.wType);
			return num == -1 || Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().IsGuidedStateSet(num);
		}
		case 30:
			return MonoSingleton<NewbieGuideManager>.GetInstance().IsNewbieBitSet((int)item.Param[0]);
		case 42:
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
				if (useableContainer != null)
				{
					int useableStackCount = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, item.Param[0]);
					return useableStackCount >= 2;
				}
			}
			return false;
		}
		case 43:
		{
			int num2 = NewbieGuideCheckSkipConditionUtil.TranslateFromSkipCond((NewbieGuideSkipConditionType)item.wType);
			if (num2 == -1)
			{
				return true;
			}
			if (item.Param[0] == 0u)
			{
				return Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().IsGuidedStateSet(num2);
			}
			return !Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().IsGuidedStateSet(num2);
		}
		case 44:
		{
			CRoleInfo masterRoleInfo2 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			return masterRoleInfo2.IsNewbieAchieveSet((int)(item.Param[0] + (uint)NewbieGuideManager.WEAKGUIDE_BIT_OFFSET));
		}
		case 49:
		{
			CRoleInfo masterRoleInfo3 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo3 == null)
			{
				return false;
			}
			bool flag = masterRoleInfo3.IsGuidedStateSet(89);
			bool flag2 = masterRoleInfo3.IsGuidedStateSet(90);
			bool arg_25E_0 = flag || flag2;
			return masterRoleInfo3.IsGuidedStateSet(89) || masterRoleInfo3.IsGuidedStateSet(90);
		}
		}
		return true;
	}
}
