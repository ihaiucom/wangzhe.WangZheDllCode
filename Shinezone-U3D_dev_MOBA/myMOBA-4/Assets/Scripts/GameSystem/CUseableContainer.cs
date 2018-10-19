using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CUseableContainer : CContainer
	{
		private const ulong WEIGHT_ITEM_TYPE = 100000000000uL;

		private const ulong WEIGHT_ITEM_TYPE_TYPE = 10000000000uL;

		private const int WEIGHT_ITEM_GET_TIME = 1;

		private ListView<CUseable> m_useableList = new ListView<CUseable>();

		public CUseableContainer(enCONTAINER_TYPE type)
		{
			this.Init(type);
		}

		public override void Init(enCONTAINER_TYPE type)
		{
			this.m_type = type;
			this.m_useableList.Clear();
		}

		public int GetCurUseableCount()
		{
			return this.m_useableList.Count;
		}

		public void Add(CUseable useable)
		{
			this.m_useableList.Add(useable);
		}

		public void Remove(CUseable useable)
		{
			if (this.m_useableList != null && useable != null)
			{
				this.m_useableList.Remove(useable);
			}
		}

		public CUseable GetUseableByIndex(int index)
		{
			if (this.m_useableList.Count <= index || index < 0)
			{
				return null;
			}
			return this.m_useableList[index];
		}

		public int GetUsebableIndexByUid(ulong uid)
		{
			for (int i = 0; i < this.m_useableList.Count; i++)
			{
				if (this.m_useableList[i].m_objID == uid)
				{
					return i;
				}
			}
			return -1;
		}

		public CUseable GetUseableByBaseID(COM_ITEM_TYPE itemType, uint baseID)
		{
			CUseable result = null;
			if (this.m_useableList != null)
			{
				for (int i = 0; i < this.m_useableList.Count; i++)
				{
					CUseable cUseable = this.m_useableList[i];
					if (cUseable != null && cUseable.m_baseID == baseID && cUseable.m_type == itemType)
					{
						result = cUseable;
						break;
					}
				}
			}
			return result;
		}

		public CUseable GetUseableByObjID(ulong objID)
		{
			CUseable result = null;
			if (this.m_useableList != null)
			{
				for (int i = 0; i < this.m_useableList.Count; i++)
				{
					CUseable cUseable = this.m_useableList[i];
					if (cUseable != null && cUseable.m_objID == objID)
					{
						result = cUseable;
						break;
					}
				}
			}
			return result;
		}

		public int GetUseableStackCount(COM_ITEM_TYPE itemType, uint baseID)
		{
			int num = 0;
			if (this.m_useableList != null)
			{
				for (int i = 0; i < this.m_useableList.Count; i++)
				{
					CUseable cUseable = this.m_useableList[i];
					if (cUseable != null && cUseable.m_type == itemType && cUseable.m_baseID == baseID)
					{
						num += cUseable.m_stackCount;
					}
				}
			}
			return num;
		}

		private void ComputeSortItemValue()
		{
			for (int i = 0; i < this.m_useableList.Count; i++)
			{
				if (this.m_useableList[i] != null)
				{
					CUseable cUseable = this.m_useableList[i];
					cUseable.m_itemSortNum = 0uL;
					cUseable.m_itemSortNum += (ulong)((long)(COM_ITEM_TYPE.COM_OBJTYPE_MAX - cUseable.m_type) * 100000000000L);
					if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
					{
						CItem cItem = cUseable as CItem;
						cUseable.m_itemSortNum += (4uL - (ulong)cItem.m_itemData.bType) * 10000000000uL;
					}
					cUseable.m_itemSortNum += cUseable.m_getTime * 1uL;
					if (cUseable.m_itemSortNum >= 18446744073709551615uL || cUseable.m_itemSortNum <= 0uL)
					{
						cUseable.m_itemSortNum = 0uL;
					}
				}
			}
		}

		private void SortItemBySortItemValue()
		{
			for (int i = 0; i < this.m_useableList.Count; i++)
			{
				for (int j = i + 1; j < this.m_useableList.Count; j++)
				{
					CUseable cUseable = this.m_useableList[i];
					CUseable cUseable2 = this.m_useableList[j];
					if (cUseable.m_itemSortNum < cUseable2.m_itemSortNum)
					{
						CUseable value = this.m_useableList[i];
						this.m_useableList[i] = this.m_useableList[j];
						this.m_useableList[j] = value;
					}
				}
			}
		}

		public void SortItemBag()
		{
			this.m_useableList.Sort(new Comparison<CUseable>(CUseableContainer.ComparisonItem));
		}

		private static int ComparisonItem(CUseable a, CUseable b)
		{
			DebugHelper.Assert(a != null && b != null, "a = {0}, b = {1}", new object[]
			{
				a,
				b
			});
			a.m_itemSortNum = ((a != null) ? ((!(a is CItem)) ? 0uL : CUseableContainer.GetSortNumByPropType((RES_PROP_TYPE_TYPE)(a as CItem).m_itemData.bType)) : 0uL);
			b.m_itemSortNum = ((b != null) ? ((!(b is CItem)) ? 0uL : CUseableContainer.GetSortNumByPropType((RES_PROP_TYPE_TYPE)(b as CItem).m_itemData.bType)) : 0uL);
			if (a.m_itemSortNum < b.m_itemSortNum)
			{
				return 1;
			}
			if (a.m_itemSortNum > b.m_itemSortNum)
			{
				return -1;
			}
			return (a.m_getTime != b.m_getTime) ? ((a.m_getTime <= b.m_getTime) ? 1 : -1) : 0;
		}

		private static ulong GetSortNumByPropType(RES_PROP_TYPE_TYPE propType)
		{
			switch (propType)
			{
			case RES_PROP_TYPE_TYPE.RES_PROP_TYPE_COMMON:
				return 100uL;
			case RES_PROP_TYPE_TYPE.RES_PROP_TYPE_VALCHG:
				return 300uL;
			case RES_PROP_TYPE_TYPE.RES_PROP_TYPE_EQUIPCOMP:
				return 0uL;
			case RES_PROP_TYPE_TYPE.RES_PROP_TYPE_HEROCOMP:
				return 0uL;
			case RES_PROP_TYPE_TYPE.RES_PROP_TYPE_GIFTS:
				return 800uL;
			case RES_PROP_TYPE_TYPE.RES_PROP_TYPE_HEROSTAR:
				return 0uL;
			case RES_PROP_TYPE_TYPE.RES_PROP_TYPE_SWEEP:
				return 0uL;
			case RES_PROP_TYPE_TYPE.RES_PROP_TYPE_MONTHWEEK_CARD:
				return 600uL;
			case RES_PROP_TYPE_TYPE.RES_PROP_TYPE_TICKET:
				return 400uL;
			case RES_PROP_TYPE_TYPE.RES_PROP_TYPE_HORN:
				return 700uL;
			case RES_PROP_TYPE_TYPE.RES_PROP_TYPE_EXPCARD:
				return 200uL;
			case RES_PROP_TYPE_TYPE.RES_PROP_TYPE_BATTLERECORD_CARD:
				return 500uL;
			default:
				return 0uL;
			}
		}

		public void Clear()
		{
			this.m_useableList.Clear();
		}

		public int GetMaxAddTime()
		{
			return CRoleInfo.GetCurrentUTCTime();
		}

		public CUseable Add(COM_ITEM_TYPE useableType, ulong objID, uint baseID, int iCount, int addTime)
		{
			CUseable cUseable = null;
			if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP || useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP || useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
			{
				cUseable = this.GetUseableByObjID(objID);
				if (cUseable == null)
				{
					CUseable cUseable2 = CUseableManager.CreateUseable(useableType, objID, baseID, iCount, addTime);
					this.Add(cUseable2);
					cUseable = cUseable2;
				}
				else
				{
					cUseable.m_stackCount += iCount;
					cUseable.ResetTime();
				}
			}
			return cUseable;
		}

		public void Remove(ulong objID, int iCount)
		{
			CUseable useableByObjID = this.GetUseableByObjID(objID);
			if (useableByObjID != null)
			{
				useableByObjID.m_stackCount -= iCount;
				if (useableByObjID.m_stackCount <= 0)
				{
					this.Remove(useableByObjID);
				}
			}
		}
	}
}
