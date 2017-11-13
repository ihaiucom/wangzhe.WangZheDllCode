using Assets.Scripts.GameSystem;
using CSProtocol;
using ResData;
using System;

public class NewbieGuideCheckSkipConditionUtil
{
	public static NewbieGuideSkipConditionType TranslateToSkipCond(int inNewbieType)
	{
		NewbieGuideSkipConditionType result = NewbieGuideSkipConditionType.Invalid;
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
		case 16:
			return NewbieGuideSkipConditionType.hasEnteredBurning;
		case 17:
			return NewbieGuideSkipConditionType.hasEnteredElitePvE;
		case 18:
			return NewbieGuideSkipConditionType.hasEnteredGuild;
		case 19:
			return NewbieGuideSkipConditionType.hasUsedSymbol;
		case 20:
			return NewbieGuideSkipConditionType.hasEnteredMysteryShop;
		case 21:
			return NewbieGuideSkipConditionType.hasComplete33Guide;
		case 22:
		case 23:
		case 24:
		case 25:
		case 28:
		case 31:
		case 35:
		case 36:
		case 37:
		case 38:
		case 39:
		case 40:
		case 41:
		case 42:
		case 43:
		case 45:
		case 46:
		case 47:
		case 48:
		case 49:
		case 50:
		case 51:
		case 52:
		case 53:
		case 54:
		case 55:
		case 56:
		case 57:
			IL_F6:
			switch (inNewbieType)
			{
			case 80:
				return NewbieGuideSkipConditionType.hasCoinDrawFive;
			case 81:
				return NewbieGuideSkipConditionType.hasComplete11Match;
			case 82:
			case 84:
				IL_117:
				switch (inNewbieType)
				{
				case 98:
					return NewbieGuideSkipConditionType.hasCompleteTrainLevel55;
				case 99:
					return result;
				case 100:
					return NewbieGuideSkipConditionType.hasDiamondDraw;
				default:
					return result;
				}
				break;
			case 83:
				return NewbieGuideSkipConditionType.hasCompleteCoronaGuide;
			case 85:
				return NewbieGuideSkipConditionType.hasCompleteTrainLevel33;
			}
			goto IL_117;
		case 26:
			return NewbieGuideSkipConditionType.hasCompleteHumanAi33Match;
		case 27:
			return NewbieGuideSkipConditionType.hasCompleteHuman33Match;
		case 29:
			return NewbieGuideSkipConditionType.hasIncreaseEquip;
		case 30:
			return NewbieGuideSkipConditionType.hasAdvancedEquip;
		case 32:
			return NewbieGuideSkipConditionType.hasCompleteHeroUp;
		case 33:
			return NewbieGuideSkipConditionType.hasEnteredTournament;
		case 34:
			return NewbieGuideSkipConditionType.hasRewardTaskPve;
		case 44:
			return NewbieGuideSkipConditionType.hasCompleteHumanAi33;
		case 58:
			return NewbieGuideSkipConditionType.hasManufacuredSymbol;
		}
		goto IL_F6;
	}

	public static int TranslateFromSkipCond(NewbieGuideSkipConditionType inCondType)
	{
		int result = -1;
		switch (inCondType)
		{
		case NewbieGuideSkipConditionType.hasCompleteEquipping:
			return 1;
		case NewbieGuideSkipConditionType.hasRewardTaskPvp:
			return 8;
		case NewbieGuideSkipConditionType.hasCompleteHeroAdv:
			return 2;
		case NewbieGuideSkipConditionType.hasSummonedHero:
			return 3;
		case NewbieGuideSkipConditionType.hasOverThreeHeroes:
			return 7;
		case NewbieGuideSkipConditionType.hasCompleteHeroStar:
			return 4;
		case NewbieGuideSkipConditionType.hasHeroSkillUpgraded:
			return 5;
		case NewbieGuideSkipConditionType.hasBoughtHero:
			return 9;
		case NewbieGuideSkipConditionType.hasBoughtItem:
			return 10;
		case NewbieGuideSkipConditionType.hasGotChapterReward:
			return 11;
		case NewbieGuideSkipConditionType.hasMopup:
			return 12;
		case NewbieGuideSkipConditionType.hasEnteredPvP:
			return 13;
		case NewbieGuideSkipConditionType.hasEnteredTrial:
			return 14;
		case NewbieGuideSkipConditionType.hasEnteredZhuangzi:
			return 15;
		case NewbieGuideSkipConditionType.hasEnteredBurning:
			return 16;
		case NewbieGuideSkipConditionType.hasEnteredElitePvE:
			return 17;
		case NewbieGuideSkipConditionType.hasEnteredGuild:
			return 18;
		case NewbieGuideSkipConditionType.hasUsedSymbol:
			return 19;
		case NewbieGuideSkipConditionType.hasEnteredMysteryShop:
			return 20;
		case NewbieGuideSkipConditionType.hasAdvancedEquip:
			return 30;
		case NewbieGuideSkipConditionType.hasCompleteBaseGuide:
			return 0;
		case NewbieGuideSkipConditionType.hasCompleteHumanAi33Match:
			return 26;
		case NewbieGuideSkipConditionType.hasCompleteHuman33Match:
			return 27;
		case NewbieGuideSkipConditionType.hasComplete33Guide:
			return 21;
		case NewbieGuideSkipConditionType.hasCompleteLottery:
			return 6;
		case NewbieGuideSkipConditionType.hasIncreaseEquip:
			return 29;
		case NewbieGuideSkipConditionType.hasRewardTaskPve:
			return 34;
		case NewbieGuideSkipConditionType.hasCompleteHeroUp:
			return 32;
		case NewbieGuideSkipConditionType.hasEnteredTournament:
			return 33;
		case NewbieGuideSkipConditionType.hasCompleteHumanAi33:
			return 44;
		case NewbieGuideSkipConditionType.hasManufacuredSymbol:
			return 58;
		case NewbieGuideSkipConditionType.hasCoinDrawFive:
			return 80;
		case NewbieGuideSkipConditionType.hasCompleteTrainLevel55:
			return 98;
		case NewbieGuideSkipConditionType.hasComplete11Match:
			return 81;
		case NewbieGuideSkipConditionType.hasCompleteTrainLevel33:
			return 85;
		case NewbieGuideSkipConditionType.hasDiamondDraw:
			return 100;
		case NewbieGuideSkipConditionType.hasCompleteCoronaGuide:
			return 83;
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
			bool arg_258_0 = flag || flag2;
			return masterRoleInfo3.IsGuidedStateSet(89) || masterRoleInfo3.IsGuidedStateSet(90);
		}
		}
		return true;
	}
}
