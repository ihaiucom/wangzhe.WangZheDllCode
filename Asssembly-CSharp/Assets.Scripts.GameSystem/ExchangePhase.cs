using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class ExchangePhase : ActivityPhase
	{
		private uint _id;

		public ResDT_WealExchagne_Info Config;

		internal ushort _usedTimes;

		public override uint ID
		{
			get
			{
				return this._id;
			}
		}

		public override uint RewardID
		{
			get
			{
				return 0u;
			}
		}

		public override int StartTime
		{
			get
			{
				return 0;
			}
		}

		public override int CloseTime
		{
			get
			{
				return 0;
			}
		}

		public override bool ReadyForGet
		{
			get
			{
				if (base.Owner.timeState != Activity.TimeState.Going || this.Config.bIsShowHotSpot == 0)
				{
					return false;
				}
				ResDT_Item_Info resDT_Item_Info = null;
				ResDT_Item_Info resDT_Item_Info2 = null;
				if (this.Config.bColItemCnt > 0)
				{
					resDT_Item_Info = this.Config.astColItemInfo[0];
				}
				if (this.Config.bColItemCnt > 1)
				{
					resDT_Item_Info2 = this.Config.astColItemInfo[1];
				}
				CUseableContainer useableContainer = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
				if (useableContainer == null)
				{
					return false;
				}
				bool result = true;
				if (resDT_Item_Info != null)
				{
					uint dwItemID = resDT_Item_Info.dwItemID;
					ushort wItemType = resDT_Item_Info.wItemType;
					int useableStackCount = useableContainer.GetUseableStackCount((COM_ITEM_TYPE)wItemType, dwItemID);
					ushort wItemCnt = resDT_Item_Info.wItemCnt;
					if (useableStackCount < (int)wItemCnt)
					{
						result = false;
					}
				}
				if (resDT_Item_Info2 != null)
				{
					uint dwItemID2 = resDT_Item_Info2.dwItemID;
					ushort wItemType2 = resDT_Item_Info2.wItemType;
					int useableStackCount2 = useableContainer.GetUseableStackCount((COM_ITEM_TYPE)wItemType2, dwItemID2);
					ushort wItemCnt2 = resDT_Item_Info2.wItemCnt;
					if (useableStackCount2 < (int)wItemCnt2)
					{
						result = false;
					}
				}
				if (!this.CheckExchange())
				{
					result = false;
				}
				return result;
			}
		}

		public ExchangePhase(Activity owner, ResDT_WealExchagne_Info config) : base(owner)
		{
			this._id = (uint)config.bIdx;
			this.Config = config;
			this._usedTimes = 0;
		}

		public bool CheckExchange()
		{
			ExchangeActivity exchangeActivity = base.Owner as ExchangeActivity;
			uint num = 0u;
			uint num2 = 0u;
			if (exchangeActivity != null)
			{
				num = exchangeActivity.GetMaxExchangeCount((int)this.Config.bIdx);
				num2 = exchangeActivity.GetExchangeCount((int)this.Config.bIdx);
			}
			return num2 < num;
		}

		public int GetMaxExchangeCount()
		{
			int num = 0;
			ResDT_Item_Info resDT_Item_Info = null;
			ResDT_Item_Info resDT_Item_Info2 = null;
			if (this.Config.bColItemCnt > 0)
			{
				resDT_Item_Info = this.Config.astColItemInfo[0];
			}
			if (this.Config.bColItemCnt > 1)
			{
				resDT_Item_Info2 = this.Config.astColItemInfo[1];
			}
			CUseableContainer useableContainer = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
			if (useableContainer != null)
			{
				if (resDT_Item_Info != null)
				{
					uint dwItemID = resDT_Item_Info.dwItemID;
					ushort wItemType = resDT_Item_Info.wItemType;
					int useableStackCount = useableContainer.GetUseableStackCount((COM_ITEM_TYPE)wItemType, dwItemID);
					ushort wItemCnt = resDT_Item_Info.wItemCnt;
					int num2 = useableStackCount / (int)wItemCnt;
					num = num2;
				}
				if (resDT_Item_Info2 != null)
				{
					uint dwItemID2 = resDT_Item_Info2.dwItemID;
					ushort wItemType2 = resDT_Item_Info2.wItemType;
					int useableStackCount2 = useableContainer.GetUseableStackCount((COM_ITEM_TYPE)wItemType2, dwItemID2);
					ushort wItemCnt2 = resDT_Item_Info2.wItemCnt;
					int num3 = useableStackCount2 / (int)wItemCnt2;
					num = Math.Min(num, num3);
				}
				ExchangeActivity exchangeActivity = base.Owner as ExchangeActivity;
				if (exchangeActivity != null)
				{
					uint maxExchangeCount = exchangeActivity.GetMaxExchangeCount((int)this.Config.bIdx);
					uint exchangeCount = exchangeActivity.GetExchangeCount((int)this.Config.bIdx);
					if (maxExchangeCount > 0u)
					{
						num = Math.Min(num, (int)(maxExchangeCount - exchangeCount));
					}
				}
			}
			return num;
		}
	}
}
