using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public struct stShopItemInfo
	{
		public COM_ITEM_TYPE enItemType;

		public RES_SHOPBUY_COINTYPE enCostType;

		public string sName;

		public uint dwItemId;

		public ushort wItemCnt;

		public ushort wSaleDiscount;

		public bool isSoldOut;

		public float fPrice;
	}
}
