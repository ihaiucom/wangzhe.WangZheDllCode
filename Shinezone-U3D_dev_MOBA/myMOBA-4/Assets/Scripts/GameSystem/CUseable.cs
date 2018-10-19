using CSProtocol;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CUseable
	{
		public COM_ITEM_TYPE m_type;

		public ulong m_objID;

		public uint m_baseID;

		public string m_name = string.Empty;

		public string m_description = string.Empty;

		public string m_mallDescription = string.Empty;

		public uint m_iconID;

		public int m_stackCount = 1;

		public int m_stackMulti;

		public int m_stackMax;

		public uint m_goldCoinBuy;

		public uint m_dianQuanBuy;

		public uint m_diamondBuy;

		public uint m_burningCoinBuy;

		public uint m_arenaCoinBuy;

		public uint m_skinCoinBuy;

		public uint m_dianQuanDirectBuy;

		public uint m_guildCoinBuy;

		public uint m_coinSale;

		public byte m_grade;

		public byte m_isSale;

		public byte m_isBatchUse;

		public byte m_bCanUse = 1;

		public int m_addTime;

		public ulong m_getTime;

		public ulong m_itemSortNum;

		public int ExtraFromType;

		public int ExtraFromData;

		public virtual COM_REWARDS_TYPE MapRewardType
		{
			get
			{
				return COM_REWARDS_TYPE.COM_REWARDS_TYPE_MAX;
			}
		}

		public virtual bool HasOwnMax
		{
			get
			{
				return false;
			}
		}

		public void ResetTime()
		{
			this.m_getTime = (ulong)((long)this.m_addTime + (long)((ulong)Time.time));
		}

		public virtual string GetIconPath()
		{
			return string.Empty;
		}

		public virtual int GetSalableCount()
		{
			return this.m_stackCount;
		}

		public uint GetBuyPrice(RES_SHOPBUY_COINTYPE coinType)
		{
			switch (coinType)
			{
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
				return this.m_dianQuanBuy;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
				return this.m_goldCoinBuy;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
				return this.m_burningCoinBuy;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
				return this.m_arenaCoinBuy;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SKINCOIN:
				return this.m_skinCoinBuy;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_GUILDCOIN:
				return this.m_guildCoinBuy;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND:
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_MIXPAY:
				return this.m_diamondBuy;
			}
			return 0u;
		}

		public void SetMultiple(ref COMDT_REWARD_MULTIPLE_DETAIL multipleDetail, bool preCond = true)
		{
			this.m_stackMulti = ((!preCond) ? 0 : CUseable.GetMultiple(ref multipleDetail, this));
		}

		public static int GetMultipleValue(COMDT_MULTIPLE_INFO_NEW multipleInfo, int multiType)
		{
			int num = 0;
			while ((long)num < (long)((ulong)multipleInfo.dwMultipleNum))
			{
				if (multipleInfo.astMultipleData[num].iType == multiType)
				{
					return multipleInfo.astMultipleData[num].iValue;
				}
				num++;
			}
			return 0;
		}

		public static int GetMultiple(uint baseVal, ref COMDT_REWARD_MULTIPLE_DETAIL multipleDetail, ushort rewardType, short subType = -1)
		{
			for (int i = 0; i < (int)multipleDetail.bNum; i++)
			{
				COMDT_REWARD_MULTIPLE_INFO cOMDT_REWARD_MULTIPLE_INFO = multipleDetail.astMultiple[i];
				if (cOMDT_REWARD_MULTIPLE_INFO.wRewardType == rewardType && (subType < 0 || (uint)subType == cOMDT_REWARD_MULTIPLE_INFO.dwRewardTypeParam))
				{
					int num = 0;
					int num2 = 0;
					while ((long)num2 < (long)((ulong)cOMDT_REWARD_MULTIPLE_INFO.stNewMultipleInfo.dwMultipleNum))
					{
						byte bOperator = cOMDT_REWARD_MULTIPLE_INFO.stNewMultipleInfo.astMultipleData[num2].bOperator;
						if (bOperator != 0)
						{
							if (bOperator == 1)
							{
								double num3 = baseVal * (double)cOMDT_REWARD_MULTIPLE_INFO.stNewMultipleInfo.astMultipleData[num2].iValue / 10000.0;
								if (num3 > 0.0)
								{
									num += (int)(num3 + 0.9999);
								}
								else if (num3 < 0.0)
								{
									num += (int)(num3 - 0.9999);
								}
							}
						}
						else
						{
							num += cOMDT_REWARD_MULTIPLE_INFO.stNewMultipleInfo.astMultipleData[num2].iValue;
						}
						num2++;
					}
					return num;
				}
			}
			return 0;
		}

		public static uint GetMultipleInfo(out COMDT_MULTIPLE_DATA[] multipleData, ref COMDT_REWARD_MULTIPLE_DETAIL multipleDetail, ushort rewardType, short subType = -1)
		{
			for (int i = 0; i < (int)multipleDetail.bNum; i++)
			{
				COMDT_REWARD_MULTIPLE_INFO cOMDT_REWARD_MULTIPLE_INFO = multipleDetail.astMultiple[i];
				if (cOMDT_REWARD_MULTIPLE_INFO.wRewardType == rewardType && (subType < 0 || (uint)subType == cOMDT_REWARD_MULTIPLE_INFO.dwRewardTypeParam))
				{
					multipleData = cOMDT_REWARD_MULTIPLE_INFO.stNewMultipleInfo.astMultipleData;
					return cOMDT_REWARD_MULTIPLE_INFO.stNewMultipleInfo.dwMultipleNum;
				}
			}
			multipleData = null;
			return 0u;
		}

		public static uint GetQqVipExtraCoin(uint totalCoin, ref COMDT_REWARD_MULTIPLE_DETAIL multipleDetail, ushort rewardType)
		{
			for (int i = 0; i < (int)multipleDetail.bNum; i++)
			{
				COMDT_REWARD_MULTIPLE_INFO cOMDT_REWARD_MULTIPLE_INFO = multipleDetail.astMultiple[i];
				if (cOMDT_REWARD_MULTIPLE_INFO.wRewardType == rewardType)
				{
					int multipleValue = CUseable.GetMultipleValue(cOMDT_REWARD_MULTIPLE_INFO.stNewMultipleInfo, 1);
					int multipleValue2 = CUseable.GetMultipleValue(cOMDT_REWARD_MULTIPLE_INFO.stNewMultipleInfo, 2);
					float num = (float)multipleValue2 / 10000f;
					float num2 = totalCoin / ((float)multipleValue / 10000f + num) * num;
					if (num2 > 0f)
					{
						return (uint)(num2 + 0.9999f);
					}
					if (num2 < 0f)
					{
						return (uint)(num2 - 0.9999f);
					}
				}
			}
			return 0u;
		}

		public static int GetMultiple(ref COMDT_REWARD_MULTIPLE_DETAIL multipleDetail, CUseable usb)
		{
			for (int i = 0; i < (int)multipleDetail.bNum; i++)
			{
				COMDT_REWARD_MULTIPLE_INFO cOMDT_REWARD_MULTIPLE_INFO = multipleDetail.astMultiple[i];
				if (cOMDT_REWARD_MULTIPLE_INFO.wRewardType == (ushort)usb.MapRewardType)
				{
					int multipleValue = CUseable.GetMultipleValue(cOMDT_REWARD_MULTIPLE_INFO.stNewMultipleInfo, 1);
					float num = (float)multipleValue / 10000f;
					int result = 0;
					if (multipleValue != 0)
					{
						if (num > 0f)
						{
							result = (int)(num + 0.9999f);
						}
						else if (num < 0f)
						{
							result = (int)(num - 0.9999f);
						}
					}
					if (cOMDT_REWARD_MULTIPLE_INFO.wRewardType != 1)
					{
						return result;
					}
					if ((uint)((CItem)usb).m_itemData.bClass == cOMDT_REWARD_MULTIPLE_INFO.dwRewardTypeParam)
					{
						return result;
					}
				}
			}
			return 0;
		}
	}
}
