using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CHeroItem : CUseable
	{
		public ResHeroCfgInfo m_heroData;

		public override COM_REWARDS_TYPE MapRewardType
		{
			get
			{
				return COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO;
			}
		}

		public CHeroItem(ulong objID, uint baseID, int stackCount = 0, int addTime = 0)
		{
			this.m_heroData = GameDataMgr.heroDatabin.GetDataByKey(baseID);
			if (this.m_heroData == null)
			{
				Debug.Log("not hero id" + baseID);
				return;
			}
			ResHeroShop resHeroShop = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(baseID, out resHeroShop);
			this.m_type = COM_ITEM_TYPE.COM_OBJTYPE_HERO;
			this.m_objID = objID;
			this.m_baseID = baseID;
			this.m_name = StringHelper.UTF8BytesToString(ref this.m_heroData.szName);
			this.m_description = StringHelper.UTF8BytesToString(ref this.m_heroData.szHeroDesc);
			this.m_iconID = uint.Parse(StringHelper.UTF8BytesToString(ref this.m_heroData.szImagePath));
			this.m_stackCount = stackCount;
			this.m_stackMax = 1;
			this.m_goldCoinBuy = 0u;
			this.m_dianQuanBuy = ((resHeroShop == null) ? 1u : resHeroShop.dwBuyCoupons);
			this.m_diamondBuy = ((resHeroShop == null) ? 1u : resHeroShop.dwBuyDiamond);
			this.m_arenaCoinBuy = ((resHeroShop == null) ? 1u : resHeroShop.dwBuyArenaCoin);
			this.m_burningCoinBuy = ((resHeroShop == null) ? 1u : resHeroShop.dwBuyBurnCoin);
			this.m_dianQuanDirectBuy = 0u;
			this.m_coinSale = 0u;
			this.m_grade = 3;
			this.m_isSale = 0;
			this.m_addTime = 0;
			base.ResetTime();
		}

		public override string GetIconPath()
		{
			return CUIUtility.s_Sprite_Dynamic_Icon_Dir + this.m_iconID;
		}
	}
}
