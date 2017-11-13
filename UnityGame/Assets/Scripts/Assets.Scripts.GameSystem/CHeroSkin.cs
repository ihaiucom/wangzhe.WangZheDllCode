using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CHeroSkin : CUseable
	{
		public object[] m_heroSkinData;

		public uint m_heroId;

		public uint m_skinId;

		public override COM_REWARDS_TYPE MapRewardType
		{
			get
			{
				return COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN;
			}
		}

		public CHeroSkin(ulong objID, uint baseID, int stackCount = 0, int addTime = 0)
		{
			ResHeroSkin dataByKey = GameDataMgr.heroSkinDatabin.GetDataByKey(baseID);
			if (dataByKey != null)
			{
				ResHeroSkinShop resHeroSkinShop = null;
				GameDataMgr.skinShopInfoDict.TryGetValue(baseID, out resHeroSkinShop);
				this.m_heroId = dataByKey.dwHeroID;
				this.m_skinId = dataByKey.dwSkinID;
				this.m_type = COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN;
				this.m_objID = objID;
				this.m_baseID = baseID;
				this.m_name = StringHelper.UTF8BytesToString(ref dataByKey.szSkinName);
				if (resHeroSkinShop != null)
				{
					this.m_description = StringHelper.UTF8BytesToString(ref resHeroSkinShop.szSkinDesc);
				}
				else
				{
					this.m_description = string.Empty;
				}
				this.m_iconID = uint.Parse(StringHelper.UTF8BytesToString(ref dataByKey.szSkinPicID));
				this.m_stackCount = stackCount;
				this.m_stackMax = 1;
				this.m_skinCoinBuy = ((resHeroSkinShop != null) ? resHeroSkinShop.dwChgItemCnt : 1u);
				this.m_dianQuanBuy = ((resHeroSkinShop != null) ? resHeroSkinShop.dwBuyCoupons : 1u);
				this.m_diamondBuy = ((resHeroSkinShop != null) ? resHeroSkinShop.dwBuyDiamond : 1u);
				this.m_coinSale = 0u;
				this.m_grade = 3;
				this.m_isSale = 0;
				this.m_addTime = 0;
				base.ResetTime();
			}
		}

		public override string GetIconPath()
		{
			return CUIUtility.s_Sprite_Dynamic_Icon_Dir + this.m_iconID;
		}
	}
}
