using Assets.Scripts.UI;
using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CVirtualItem : CUseable
	{
		public enVirtualItemType m_virtualType;

		public override COM_REWARDS_TYPE MapRewardType
		{
			get
			{
				switch (this.m_virtualType)
				{
				case enVirtualItemType.enNoUsed:
					return COM_REWARDS_TYPE.COM_REWARDS_TYPE_COIN;
				case enVirtualItemType.enExp:
					return COM_REWARDS_TYPE.COM_REWARDS_TYPE_EXP;
				case enVirtualItemType.enDianQuan:
					return COM_REWARDS_TYPE.COM_REWARDS_TYPE_COUPONS;
				case enVirtualItemType.enHeart:
					return COM_REWARDS_TYPE.COM_REWARDS_TYPE_AP;
				case enVirtualItemType.enGoldCoin:
					return COM_REWARDS_TYPE.COM_REWARDS_TYPE_HONOUR;
				case enVirtualItemType.enArenaCoin:
					return COM_REWARDS_TYPE.COM_REWARDS_TYPE_ARENA_COIN;
				case enVirtualItemType.enBurningCoin:
					return COM_REWARDS_TYPE.COM_REWARDS_TYPE_BURNING_COIN;
				case enVirtualItemType.enExpPool:
					return COM_REWARDS_TYPE.COM_REWARDS_TYPE_HEROPOOLEXP;
				case enVirtualItemType.enSkinCoin:
					return COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKINCOIN;
				case enVirtualItemType.enSymbolCoin:
					return COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOLCOIN;
				case enVirtualItemType.enDiamond:
					return COM_REWARDS_TYPE.COM_REWARDS_TYPE_DIAMOND;
				case enVirtualItemType.enHuoyueDu:
					return COM_REWARDS_TYPE.COM_REWARDS_TYPE_HUOYUEDU;
				case enVirtualItemType.enMatchPersonalPoint:
					return COM_REWARDS_TYPE.COM_REWARDS_TYPE_MATCH_POINT_PERSON;
				case enVirtualItemType.enMatchTeamPoint:
					return COM_REWARDS_TYPE.COM_REWARDS_TYPE_MATCH_POINT_GUILD;
				}
				return base.MapRewardType;
			}
		}

		public CVirtualItem(enVirtualItemType bType, int bCount)
		{
			this.m_virtualType = bType;
			this.m_stackCount = bCount;
			if (bType == enVirtualItemType.enExp)
			{
				this.m_name = Singleton<CTextManager>.GetInstance().GetText("Experience");
				this.m_description = Singleton<CTextManager>.GetInstance().GetText("Description_EXP");
				this.m_grade = 0;
			}
			else if (bType == enVirtualItemType.enNoUsed)
			{
				this.m_name = string.Empty;
				this.m_description = string.Empty;
				this.m_grade = 2;
			}
			else if (bType == enVirtualItemType.enDianQuan)
			{
				this.m_name = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_DianQuan");
				this.m_description = Singleton<CTextManager>.GetInstance().GetText("Description_DianQuan");
				this.m_grade = 3;
			}
			else if (bType == enVirtualItemType.enHeart)
			{
				this.m_name = Singleton<CTextManager>.GetInstance().GetText("Action_Point");
				this.m_description = Singleton<CTextManager>.GetInstance().GetText("Description_AP");
				this.m_grade = 1;
			}
			else if (bType == enVirtualItemType.enExpPool)
			{
				this.m_name = "经验池";
				this.m_description = "经验池";
				this.m_grade = 0;
			}
			else if (bType == enVirtualItemType.enGoldCoin)
			{
				this.m_name = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_GoldCoin");
				this.m_description = Singleton<CTextManager>.GetInstance().GetText("Description_GoldCoin");
				this.m_grade = 1;
			}
			else if (bType == enVirtualItemType.enBurningCoin)
			{
				this.m_name = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Burning_Coin");
				this.m_description = Singleton<CTextManager>.GetInstance().GetText("Description_Burning_Coin");
				this.m_grade = 1;
			}
			else if (bType == enVirtualItemType.enArenaCoin)
			{
				this.m_name = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Arena_Coin");
				this.m_description = Singleton<CTextManager>.GetInstance().GetText("Description_Arena_Coin");
				this.m_grade = 1;
			}
			else if (bType == enVirtualItemType.enSkinCoin)
			{
				this.m_name = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Skin_Coin");
				this.m_description = Singleton<CTextManager>.GetInstance().GetText("Description_Skin_Coin");
				this.m_grade = 1;
			}
			else if (bType == enVirtualItemType.enGuildConstruct)
			{
				this.m_name = string.Empty;
				this.m_description = string.Empty;
				this.m_grade = 1;
			}
			else if (bType == enVirtualItemType.enSymbolCoin)
			{
				this.m_name = Singleton<CTextManager>.GetInstance().GetText("Symbol_Coin_Text");
				this.m_description = Singleton<CTextManager>.GetInstance().GetText("Symbol_Coin_Desc");
				this.m_grade = 1;
			}
			else if (bType == enVirtualItemType.enDiamond)
			{
				this.m_name = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Diamond");
				this.m_description = Singleton<CTextManager>.GetInstance().GetText("Description_Diamond");
				this.m_grade = 3;
			}
			else if (bType == enVirtualItemType.enAchievementPoint)
			{
				this.m_name = Singleton<CTextManager>.GetInstance().GetText("Achievement_Point");
				this.m_description = Singleton<CTextManager>.GetInstance().GetText("Achievement_Point_Desc");
				this.m_grade = 1;
			}
			else if (bType == enVirtualItemType.enHuoyueDu)
			{
				this.m_name = Singleton<CTextManager>.GetInstance().GetText("HuoyueDu_Text");
				this.m_description = Singleton<CTextManager>.GetInstance().GetText("HuoyueDu_Desc");
				this.m_grade = 1;
			}
			else if (bType == enVirtualItemType.enMatchPersonalPoint)
			{
				this.m_name = Singleton<CTextManager>.GetInstance().GetText("MatchPersonalPoint_Name");
				this.m_description = Singleton<CTextManager>.GetInstance().GetText("MatchPersonalPoint_Desc");
				this.m_grade = 1;
			}
			else if (bType == enVirtualItemType.enMatchTeamPoint)
			{
				this.m_name = Singleton<CTextManager>.GetInstance().GetText("MatchTeamPoint_Name");
				this.m_description = Singleton<CTextManager>.GetInstance().GetText("MatchTeamPoint_Desc");
				this.m_grade = 1;
			}
			else if (bType == enVirtualItemType.enDianJuanJiFen)
			{
				this.m_name = Singleton<CTextManager>.GetInstance().GetText("DianJuanJiFenName");
				this.m_description = Singleton<CTextManager>.GetInstance().GetText("DianJuanJiFenDesc");
				this.m_grade = 3;
			}
			else if (bType == enVirtualItemType.enMentorPoint)
			{
				this.m_name = Singleton<CTextManager>.GetInstance().GetText("MingShiDianName");
				this.m_description = Singleton<CTextManager>.GetInstance().GetText("MingShiDianDesc");
				this.m_grade = 3;
			}
		}

		public override string GetIconPath()
		{
			string result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90000;
			switch (this.m_virtualType)
			{
			case enVirtualItemType.enNoUsed:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90001;
				break;
			case enVirtualItemType.enExp:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90000;
				break;
			case enVirtualItemType.enDianQuan:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90002;
				break;
			case enVirtualItemType.enHeart:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90004;
				break;
			case enVirtualItemType.enGoldCoin:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90001;
				break;
			case enVirtualItemType.enArenaCoin:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90009;
				break;
			case enVirtualItemType.enBurningCoin:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90008;
				break;
			case enVirtualItemType.enExpPool:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90010;
				break;
			case enVirtualItemType.enSkinCoin:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90007;
				break;
			case enVirtualItemType.enGuildConstruct:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90009;
				break;
			case enVirtualItemType.enSymbolCoin:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90011;
				break;
			case enVirtualItemType.enDiamond:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90005;
				break;
			case enVirtualItemType.enAchievementPoint:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90012;
				break;
			case enVirtualItemType.enHuoyueDu:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90010;
				break;
			case enVirtualItemType.enMatchPersonalPoint:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90006;
				break;
			case enVirtualItemType.enMatchTeamPoint:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90006;
				break;
			case enVirtualItemType.enDianJuanJiFen:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90913;
				break;
			case enVirtualItemType.enMentorPoint:
				result = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 90014;
				break;
			}
			return result;
		}
	}
}
