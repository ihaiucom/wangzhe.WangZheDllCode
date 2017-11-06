using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CSymbolItem : CUseable
	{
		public ResSymbolInfo m_SymbolData;

		public int[] m_pageWearCnt = new int[50];

		public override COM_REWARDS_TYPE MapRewardType
		{
			get
			{
				return COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOL;
			}
		}

		public CSymbolItem(ulong objID, uint baseID, int stackCount = 0, int addTime = 0)
		{
			this.m_SymbolData = GameDataMgr.symbolInfoDatabin.GetDataByKey(baseID);
			if (this.m_SymbolData == null)
			{
				return;
			}
			this.m_type = COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL;
			this.m_objID = objID;
			this.m_baseID = baseID;
			this.m_name = StringHelper.UTF8BytesToString(ref this.m_SymbolData.szName);
			this.m_description = StringHelper.UTF8BytesToString(ref this.m_SymbolData.szDesc);
			this.m_iconID = this.m_SymbolData.dwIcon;
			this.m_stackCount = stackCount;
			this.m_stackMax = this.m_SymbolData.iOverLimit;
			this.m_goldCoinBuy = this.m_SymbolData.dwPVPCoinBuy;
			this.m_dianQuanBuy = this.m_SymbolData.dwCouponsBuy;
			this.m_diamondBuy = this.m_SymbolData.dwDiamondBuy;
			this.m_arenaCoinBuy = this.m_SymbolData.dwArenaCoinBuy;
			this.m_burningCoinBuy = this.m_SymbolData.dwBurningCoinBuy;
			this.m_dianQuanDirectBuy = 0u;
			this.m_guildCoinBuy = this.m_SymbolData.dwGuildCoinBuy;
			this.m_coinSale = this.m_SymbolData.dwCoinSale;
			this.m_grade = (byte)(this.m_SymbolData.wLevel - 1);
			this.m_isSale = this.m_SymbolData.bIsSale;
			this.m_addTime = addTime;
			base.ResetTime();
		}

		public override string GetIconPath()
		{
			return CUIUtility.s_Sprite_Dynamic_Icon_Dir + this.m_iconID;
		}

		public bool IsSaleNeedSecurePwd()
		{
			return this.m_SymbolData.bNeedPswd > 0;
		}

		public void SetPageWearCnt(int pageIndex, ulong[] symbolIdArr)
		{
			this.m_pageWearCnt[pageIndex] = 0;
			for (int i = 0; i < symbolIdArr.Length; i++)
			{
				if (symbolIdArr[i] == this.m_objID)
				{
					this.m_pageWearCnt[pageIndex]++;
				}
			}
		}

		public int GetPageWearCnt(int page)
		{
			return this.m_pageWearCnt[page];
		}

		public int GetMaxWearCnt()
		{
			int num = 0;
			for (int i = 0; i < this.m_pageWearCnt.Length; i++)
			{
				if (num < this.m_pageWearCnt[i])
				{
					num = this.m_pageWearCnt[i];
				}
			}
			return num;
		}

		public override int GetSalableCount()
		{
			if (this.IsGuildSymbol())
			{
				return base.GetSalableCount();
			}
			int num = this.m_stackCount - this.GetMaxWearCnt();
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}

		public bool IsGuildSymbol()
		{
			return this.m_SymbolData != null && this.m_SymbolData.dwGuildFacLv > 0u;
		}

		public uint GetBuyPrice(RES_SHOPBUY_COINTYPE coinType)
		{
			if (coinType == RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SYMBOLCOIN)
			{
				return this.m_SymbolData.dwMakeCoin;
			}
			return base.GetBuyPrice(coinType);
		}
	}
}
