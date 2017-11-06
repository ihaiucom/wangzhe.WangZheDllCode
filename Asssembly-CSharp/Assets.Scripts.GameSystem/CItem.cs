using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CItem : CUseable
	{
		public ResPropInfo m_itemData;

		public override COM_REWARDS_TYPE MapRewardType
		{
			get
			{
				return COM_REWARDS_TYPE.COM_REWARDS_TYPE_ITEM;
			}
		}

		public CItem(ulong objID, uint baseID, int stackCount = 0, int addTime = 0)
		{
			this.m_itemData = GameDataMgr.itemDatabin.GetDataByKey(baseID);
			if (this.m_itemData == null)
			{
				Debug.Log("not item id" + baseID);
				return;
			}
			this.m_type = COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP;
			this.m_objID = objID;
			this.m_baseID = baseID;
			this.m_name = StringHelper.UTF8BytesToString(ref this.m_itemData.szName);
			this.m_description = StringHelper.UTF8BytesToString(ref this.m_itemData.szDesc);
			this.m_mallDescription = StringHelper.UTF8BytesToString(ref this.m_itemData.szDescAdd);
			this.m_iconID = this.m_itemData.dwIcon;
			this.m_stackCount = stackCount;
			this.m_stackMax = this.m_itemData.iOverLimit;
			this.m_goldCoinBuy = this.m_itemData.dwPVPCoinBuy;
			this.m_dianQuanBuy = this.m_itemData.dwCouponsBuy;
			this.m_diamondBuy = this.m_itemData.dwDiamondBuy;
			this.m_arenaCoinBuy = this.m_itemData.dwArenaCoinBuy;
			this.m_burningCoinBuy = this.m_itemData.dwBurningCoinBuy;
			this.m_dianQuanDirectBuy = this.m_itemData.dwCouponsDirectBuy;
			this.m_guildCoinBuy = this.m_itemData.dwGuildCoinBuy;
			this.m_coinSale = this.m_itemData.dwCoinSale;
			this.m_grade = this.m_itemData.bGrade;
			this.m_bCanUse = this.m_itemData.bIsCanUse;
			this.m_isSale = this.m_itemData.bIsSale;
			this.m_isBatchUse = this.m_itemData.bIsBatchUse;
			this.m_addTime = addTime;
			base.ResetTime();
		}

		public override string GetIconPath()
		{
			return CUIUtility.s_Sprite_Dynamic_Icon_Dir + this.m_iconID;
		}

		public static bool IsHeroExperienceCard(uint itemId)
		{
			ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(itemId);
			return dataByKey != null && (int)dataByKey.EftParam[0] == 4;
		}

		public static bool IsSkinExperienceCard(uint itemId)
		{
			ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(itemId);
			return dataByKey != null && (int)dataByKey.EftParam[0] == 5;
		}

		public static bool IsPlayerNameChangeCard(uint itemId)
		{
			ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(itemId);
			return dataByKey != null && (int)dataByKey.EftParam[0] == 6;
		}

		public static bool IsGuildNameChangeCard(uint itemId)
		{
			ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(itemId);
			return dataByKey != null && (int)dataByKey.EftParam[0] == 7;
		}

		public static bool IsHeroExChangeCoupons(uint itemID)
		{
			ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(134u);
			DebugHelper.Assert(dataByKey != null, "global cfg databin err: hero exchange coupons id doesnt exist");
			return itemID == dataByKey.dwConfValue;
		}

		public static bool IsSkinExChangeCoupons(uint itemID)
		{
			ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(135u);
			DebugHelper.Assert(dataByKey != null, "global cfg databin err: skin exchange coupons id doesnt exist");
			return itemID == dataByKey.dwConfValue;
		}

		public static bool IsCryStalItem(uint itemID)
		{
			return Singleton<CMallRouletteController>.GetInstance().IsCryStalItem(itemID);
		}

		public static uint GetExperienceCardHeroOrSkinId(uint itemId)
		{
			ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(itemId);
			if (dataByKey != null)
			{
				return (uint)dataByKey.EftParam[1];
			}
			return 0u;
		}
	}
}
