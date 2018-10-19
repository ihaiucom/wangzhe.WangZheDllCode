using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class CSymbolInfo
	{
		public enum enSymbolOperationType
		{
			Wear,
			TakeOff,
			Replace
		}

		private ulong[][] m_symbolPageArr = new ulong[50][];

		public int m_pageMaxLvlLimit;

		public static int s_maxSymbolLevel = 5;

		public int m_pageCount;

		private string[] m_pageNameArr = new string[50];

		private COMDT_SYMBOLPAGE_EXTRA[] GridBuyInfo;

		public static ListView<ResHeroSymbolLvl> s_symbolPvpLvlList = new ListView<ResHeroSymbolLvl>();

		public int m_pageBuyCnt;

		public uint m_selSymbolRcmdHeroId;

		public ushort m_selSymbolRcmdLevel = 1;

		public void SetData(COMDT_ACNT_SYMBOLINFO svrInfo)
		{
			this.m_pageCount = (int)svrInfo.bValidPageCnt;
			this.m_pageBuyCnt = (int)svrInfo.bBuyPageCnt;
			for (int i = 0; i < (int)svrInfo.bValidPageCnt; i++)
			{
				this.SetSymbolPageData(i, svrInfo.astPageList[i]);
			}
			this.SetSymbolPageMaxLevel();
			this.InitSymbolPvpLevelList();
			this.GridBuyInfo = svrInfo.astPageExtra;
			this.m_selSymbolRcmdHeroId = svrInfo.stRecommend.dwSelHeroID;
			this.m_selSymbolRcmdLevel = svrInfo.stRecommend.wSelSymbolLvl;
		}

		private void InitSymbolPvpLevelList()
		{
			CSymbolInfo.s_symbolPvpLvlList.Clear();
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.heroSymbolLvlDatabin.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<long, object> current = enumerator.Current;
				ResHeroSymbolLvl resHeroSymbolLvl = (ResHeroSymbolLvl)current.Value;
				if (resHeroSymbolLvl != null)
				{
					CSymbolInfo.s_symbolPvpLvlList.Add(resHeroSymbolLvl);
				}
			}
		}

		private void SetSymbolPageData(int pageIndex, COMDT_SYMBOLPAGE_DETAIL pageDetail)
		{
			if (pageIndex < 0 || pageIndex >= this.m_pageCount)
			{
				return;
			}
			if (this.m_symbolPageArr[pageIndex] == null)
			{
				this.m_symbolPageArr[pageIndex] = new ulong[30];
			}
			this.m_pageNameArr[pageIndex] = StringHelper.UTF8BytesToString(ref pageDetail.szName);
			for (int i = 0; i < pageDetail.UniqueID.Length; i++)
			{
				ulong num = pageDetail.UniqueID[i];
				this.m_symbolPageArr[pageIndex][i] = num;
				if (num > 0uL)
				{
					this.SetSymbolItemWearCnt(num, pageIndex);
				}
			}
		}

		public ulong[] GetPageSymbolData(int pageIndex)
		{
			if (pageIndex < 0 || pageIndex >= this.m_pageCount)
			{
				return null;
			}
			return this.m_symbolPageArr[pageIndex];
		}

		public enSymbolWearState GetSymbolPosWearState(int page, int pos, out int param)
		{
			param = 0;
			if (page < 0 || page >= this.m_pageCount || pos < 0 || pos >= 30)
			{
				return enSymbolWearState.OtherState;
			}
			ulong[] array = this.m_symbolPageArr[page];
			if (array[pos] > 0uL)
			{
				return enSymbolWearState.WearSuccess;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			uint pvpLevel = masterRoleInfo.PvpLevel;
			uint latestOpenLevel = this.GetLatestOpenLevel(pvpLevel);
			ResSymbolPos symbolPos = CSymbolInfo.GetSymbolPos(pos);
			DebugHelper.Assert(symbolPos != null, "symbolPos!=null");
			if (symbolPos == null)
			{
				return enSymbolWearState.OtherState;
			}
			param = (int)symbolPos.wOpenLevel;
			if (pvpLevel >= (uint)symbolPos.wOpenLevel)
			{
				return enSymbolWearState.OpenToWear;
			}
			if (this.GridBuyInfo[pos].bBuyFlag == 1)
			{
				return enSymbolWearState.OpenToWear;
			}
			ulong[] array2 = this.m_symbolPageArr[page];
			int num = array2.Length;
			int i = 0;
			while (i < num)
			{
				ResSymbolPos symbolPos2 = CSymbolInfo.GetSymbolPos(i);
				if (pvpLevel < (uint)symbolPos2.wOpenLevel && this.GridBuyInfo[(int)(symbolPos2.bSymbolPos - 1)].bBuyFlag == 0 && symbolPos.dwSymbolColor == symbolPos2.dwSymbolColor)
				{
					if (this.GridBuyInfo[(int)(symbolPos.bSymbolPos - 2)].bBuyFlag == 1)
					{
						return ((long)param != (long)((ulong)latestOpenLevel)) ? enSymbolWearState.CanBuy : enSymbolWearState.CanBuyAndWillOpen;
					}
					ResSymbolPos symbolPos3 = CSymbolInfo.GetSymbolPos(pos - 1);
					if (symbolPos3 != null && pvpLevel >= (uint)symbolPos3.wOpenLevel)
					{
						return ((long)param != (long)((ulong)latestOpenLevel)) ? enSymbolWearState.CanBuy : enSymbolWearState.CanBuyAndWillOpen;
					}
					return enSymbolWearState.UnOpen;
				}
				else
				{
					i++;
				}
			}
			return enSymbolWearState.WillOpen;
		}

		public bool IsAnySymbolItemCanWear(int pageIndex, int pos, ref ListView<CSymbolItem> symbolList)
		{
			ResSymbolPos symbolPos = CSymbolInfo.GetSymbolPos(pos);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null || pageIndex < 0 || pageIndex >= this.m_pageCount || symbolPos == null || symbolList == null || ((uint)symbolPos.wOpenLevel > masterRoleInfo.PvpLevel && this.GridBuyInfo[pos].bBuyFlag == 0))
			{
				return false;
			}
			for (int i = 0; i < symbolList.Count; i++)
			{
				if (this.IsSymbolPosMatchItem(pageIndex, symbolPos, symbolList[i]))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsSymbolPosMatchItem(int pageIndex, ResSymbolPos symbolPos, CSymbolItem symbol)
		{
			return pageIndex >= 0 && pageIndex < this.m_pageCount && symbolPos != null && symbol != null && (CSymbolInfo.CheckSymbolColor(symbolPos, symbol.m_SymbolData.bColor) && symbol.m_stackCount > symbol.GetPageWearCnt(pageIndex) && (int)symbol.m_SymbolData.wLevel <= this.m_pageMaxLvlLimit && symbol.GetPageWearCnt(pageIndex) < CSymbolWearController.s_maxSameIDSymbolEquipNum);
		}

		public int GetNextCanEquipPos(int page, int nowPos, ref ListView<CSymbolItem> symbolList)
		{
			int num = -1;
			if (page < 0 || page >= this.m_pageCount || nowPos < 0 || nowPos >= 30)
			{
				return num;
			}
			ulong[] array = this.m_symbolPageArr[page];
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			uint pvpLevel = masterRoleInfo.PvpLevel;
			uint dwSymbolColor = CSymbolInfo.GetSymbolPos(nowPos).dwSymbolColor;
			int firstColorIndex = this.GetFirstColorIndex(dwSymbolColor);
			int num2 = -1;
			for (int i = firstColorIndex; i < array.Length; i++)
			{
				if (i != nowPos && array[i] == 0uL)
				{
					ResSymbolPos symbolPos = CSymbolInfo.GetSymbolPos(i);
					if (symbolPos != null && (pvpLevel >= (uint)symbolPos.wOpenLevel || this.GridBuyInfo[i].bBuyFlag == 1))
					{
						if (this.IsAnySymbolItemCanWear(page, i, ref symbolList))
						{
							num = i;
							break;
						}
						if (num2 == -1)
						{
							num2 = i;
						}
					}
				}
			}
			if (num == -1)
			{
				for (int j = 0; j < firstColorIndex; j++)
				{
					if (j != nowPos && array[j] == 0uL)
					{
						ResSymbolPos symbolPos2 = CSymbolInfo.GetSymbolPos(j);
						if (symbolPos2 != null && (pvpLevel >= (uint)symbolPos2.wOpenLevel || this.GridBuyInfo[j].bBuyFlag == 1))
						{
							if (this.IsAnySymbolItemCanWear(page, j, ref symbolList))
							{
								num = j;
								break;
							}
							if (num2 == -1)
							{
								num2 = j;
							}
						}
					}
				}
			}
			if (num == -1)
			{
				num = num2;
			}
			return num;
		}

		private int GetFirstColorIndex(uint symBolColor)
		{
			int result = 0;
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.symbolPosDatabin.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<long, object> current = enumerator.Current;
				ResSymbolPos resSymbolPos = (ResSymbolPos)current.Value;
				if (resSymbolPos.dwSymbolColor == symBolColor)
				{
					result = (int)(resSymbolPos.bSymbolPos - 1);
					break;
				}
			}
			return result;
		}

		public bool GetWearPos(CSymbolItem item, int page, out int pos, out enFindSymbolWearPosCode findCode)
		{
			pos = 0;
			findCode = enFindSymbolWearPosCode.FindNone;
			if (item == null || (int)item.m_SymbolData.wLevel > this.m_pageMaxLvlLimit)
			{
				findCode = enFindSymbolWearPosCode.FindSymbolLevelLimit;
				return false;
			}
			ulong[] array = this.m_symbolPageArr[page];
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			int num = array.Length;
			bool flag = true;
			for (int i = 0; i < num; i++)
			{
				ResSymbolPos symbolPos = CSymbolInfo.GetSymbolPos(i);
				if (((ulong)symbolPos.dwSymbolColor & (ulong)(1L << (int)(item.m_SymbolData.bColor & 31))) != 0uL)
				{
					flag = (flag && array[i] != 0uL);
				}
				if (array[i] == 0uL && (masterRoleInfo.PvpLevel >= (uint)symbolPos.wOpenLevel || this.GridBuyInfo[i].bBuyFlag == 1) && ((ulong)symbolPos.dwSymbolColor & (ulong)(1L << (int)(item.m_SymbolData.bColor & 31))) != 0uL)
				{
					pos = i;
					findCode = enFindSymbolWearPosCode.FindSuccess;
					return true;
				}
			}
			findCode = ((!flag) ? enFindSymbolWearPosCode.FindSymbolNotOpen : enFindSymbolWearPosCode.FindSymbolPosFull);
			return false;
		}

		public static ResSymbolPos GetSymbolPos(int pos)
		{
			ResSymbolPos dataByKey = GameDataMgr.symbolPosDatabin.GetDataByKey((uint)((byte)(pos + 1)));
			if (dataByKey == null)
			{
				return null;
			}
			return dataByKey;
		}

		public static bool CheckSymbolColor(ResSymbolPos symbolPos, byte color)
		{
			return symbolPos != null && ((ulong)symbolPos.dwSymbolColor & (ulong)(1L << (int)(color & 31))) != 0uL;
		}

		public void OnSymbolChange(int page, int pos, ulong objId, out uint cfgId, out CSymbolInfo.enSymbolOperationType operType)
		{
			cfgId = 0u;
			operType = CSymbolInfo.enSymbolOperationType.Wear;
			if (page < 0 || page >= this.m_pageCount)
			{
				return;
			}
			CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
			if (this.m_symbolPageArr[page][pos] > 0uL)
			{
				if (objId > 0uL)
				{
					ulong objId2 = this.m_symbolPageArr[page][pos];
					this.m_symbolPageArr[page][pos] = objId;
					this.SetSymbolItemWearCnt(objId2, page);
					operType = CSymbolInfo.enSymbolOperationType.Replace;
				}
				else
				{
					objId = this.m_symbolPageArr[page][pos];
					this.m_symbolPageArr[page][pos] = 0uL;
					operType = CSymbolInfo.enSymbolOperationType.TakeOff;
				}
			}
			else
			{
				this.m_symbolPageArr[page][pos] = objId;
				operType = CSymbolInfo.enSymbolOperationType.Wear;
			}
			CUseable useableByObjID = useableContainer.GetUseableByObjID(objId);
			if (useableByObjID != null)
			{
				CSymbolItem cSymbolItem = (CSymbolItem)useableByObjID;
				if (cSymbolItem != null)
				{
					cfgId = cSymbolItem.m_SymbolData.dwID;
					cSymbolItem.SetPageWearCnt(page, this.m_symbolPageArr[page]);
				}
			}
		}

		public void OnSymbolPageClear(int pageIndex)
		{
			if (pageIndex < 0 || pageIndex >= this.m_pageCount)
			{
				return;
			}
			for (int i = 0; i < 30; i++)
			{
				if (this.m_symbolPageArr[pageIndex][i] > 0uL)
				{
					ulong objId = this.m_symbolPageArr[pageIndex][i];
					this.m_symbolPageArr[pageIndex][i] = 0uL;
					this.SetSymbolItemWearCnt(objId, pageIndex);
				}
			}
		}

		public void SetSymbolItemWearCnt(ulong objId, int pageIndex)
		{
			if (pageIndex < 0 || pageIndex >= this.m_pageCount)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(false, "SetSymbolItemWearCnt role is null");
				return;
			}
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			if (objId > 0uL && useableContainer != null)
			{
				CUseable useableByObjID = useableContainer.GetUseableByObjID(objId);
				if (useableByObjID != null)
				{
					CSymbolItem cSymbolItem = (CSymbolItem)useableByObjID;
					if (cSymbolItem != null)
					{
						cSymbolItem.SetPageWearCnt(pageIndex, this.m_symbolPageArr[pageIndex]);
					}
				}
			}
		}

		public static uint GetComposeCfgId(int level, RES_SYMBOLCOMP_TYPE compType, uint cfgId)
		{
			return (uint)((long)((int)compType * 10000 + level * 1000) + (long)((ulong)cfgId));
		}

		public void GetSymbolPageProp(int pageIndex, ref int[] propArr, ref int[] propPctArr, bool bPvp)
		{
			if (pageIndex < 0 || pageIndex >= 50 || propArr == null || propPctArr == null || this.m_symbolPageArr == null)
			{
				return;
			}
			int num = 37;
			for (int i = 0; i < num; i++)
			{
				propArr[i] = 0;
				propPctArr[i] = 0;
			}
			ulong[] array = this.m_symbolPageArr[pageIndex];
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null && array != null)
			{
				CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
				if (useableContainer == null)
				{
					DebugHelper.Assert(false, "GetSymbolPageProp container is null");
					return;
				}
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] > 0uL)
					{
						CUseable useableByObjID = useableContainer.GetUseableByObjID(array[i]);
						if (useableByObjID != null)
						{
							ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(useableByObjID.m_baseID);
							if (dataByKey != null)
							{
								ResDT_FuncEft_Obj[] array2 = (!bPvp) ? dataByKey.astPveEftList : dataByKey.astFuncEftList;
								if (array2 != null)
								{
									for (int j = 0; j < array2.Length; j++)
									{
										if (array2[j].wType != 0 && array2[j].wType < 37 && array2[j].iValue != 0)
										{
											if (array2[j].bValType == 0)
											{
												propArr[(int)array2[j].wType] += array2[j].iValue;
											}
											else if (array2[j].bValType == 1)
											{
												propPctArr[(int)array2[j].wType] += array2[j].iValue;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void SetSymbolPageMaxLevel()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			ResHeroSymbolLvl dataByKey = GameDataMgr.heroSymbolLvlDatabin.GetDataByKey((uint)((ushort)masterRoleInfo.PvpLevel));
			if (dataByKey == null)
			{
				return;
			}
			this.m_pageMaxLvlLimit = (int)dataByKey.wSymbolMaxLvl;
		}

		public bool IsPageFull()
		{
			ResShopInfo cfgShopInfo = CPurchaseSys.GetCfgShopInfo(RES_SHOPBUY_TYPE.RES_BUYTYPE_SYMBOLPAGE, this.m_pageBuyCnt + 1);
			return cfgShopInfo == null;
		}

		public void GetNewPageCost(out RES_SHOPBUY_COINTYPE costType, out uint costVal)
		{
			costType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN;
			costVal = 0u;
			ResShopInfo cfgShopInfo = CPurchaseSys.GetCfgShopInfo(RES_SHOPBUY_TYPE.RES_BUYTYPE_SYMBOLPAGE, this.m_pageBuyCnt + 1);
			if (cfgShopInfo != null)
			{
				costType = (RES_SHOPBUY_COINTYPE)cfgShopInfo.bCoinType;
				costVal = cfgShopInfo.dwCoinPrice;
			}
		}

		public SymbolBuyCode CheckBuySymbolPage()
		{
			if (this.IsPageFull())
			{
				return SymbolBuyCode.PageFull;
			}
			RES_SHOPBUY_COINTYPE rES_SHOPBUY_COINTYPE = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN;
			uint num = 0u;
			this.GetNewPageCost(out rES_SHOPBUY_COINTYPE, out num);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (rES_SHOPBUY_COINTYPE == RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN && masterRoleInfo.GoldCoin < num)
			{
				return SymbolBuyCode.CoinNotEnough;
			}
			if (rES_SHOPBUY_COINTYPE == RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS && masterRoleInfo.DianQuan < (ulong)num)
			{
				return SymbolBuyCode.DiamondNotEnough;
			}
			return SymbolBuyCode.BuySuccess;
		}

		public void SetSymbolPageCount(int cnt)
		{
			this.m_pageCount = cnt;
			for (int i = 0; i < this.m_pageCount; i++)
			{
				if (this.m_symbolPageArr[i] == null)
				{
					this.m_symbolPageArr[i] = new ulong[30];
				}
			}
		}

		public void SetSymbolPageBuyCnt(int cnt)
		{
			this.m_pageBuyCnt = cnt;
		}

		public string GetSymbolPageName(int pageIdx)
		{
			if (pageIdx < 0 || pageIdx >= 50)
			{
				return string.Empty;
			}
			string text = this.m_pageNameArr[pageIdx];
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			string text2 = Singleton<CTextManager>.GetInstance().GetText("Symbol_Page_Name");
			return string.Format(text2, pageIdx + 1);
		}

		public void SetSymbolPageName(int pageIndex, string pageName)
		{
			if (pageIndex < 0 || pageIndex >= 50)
			{
				return;
			}
			this.m_pageNameArr[pageIndex] = pageName;
		}

		public int GetSymbolPageEft(int pageIndex)
		{
			ulong[] pageSymbolData = this.GetPageSymbolData(pageIndex);
			if (pageSymbolData == null)
			{
				return 0;
			}
			int num = 0;
			CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
			for (int i = 0; i < pageSymbolData.Length; i++)
			{
				if (pageSymbolData[i] > 0uL)
				{
					CUseable useableByObjID = useableContainer.GetUseableByObjID(pageSymbolData[i]);
					CSymbolItem cSymbolItem = (CSymbolItem)useableByObjID;
					if (cSymbolItem != null)
					{
						num += cSymbolItem.m_SymbolData.iCombatEft;
					}
				}
			}
			return num;
		}

		public static int GetMinWearPvpLvl(int symbolLvl)
		{
			int num = CSymbolInfo.s_symbolPvpLvlList.Count;
			bool flag = false;
			for (int i = num - 1; i >= 0; i--)
			{
				if ((int)CSymbolInfo.s_symbolPvpLvlList[i].wSymbolMaxLvl == symbolLvl && num > (int)CSymbolInfo.s_symbolPvpLvlList[i].wPvpLevel)
				{
					num = (int)CSymbolInfo.s_symbolPvpLvlList[i].wPvpLevel;
					flag = true;
				}
			}
			if (!flag)
			{
				num = 1;
			}
			return num;
		}

		public bool CheckAnyWearSymbol(out int posId, out uint symbolId, byte colorType = 0)
		{
			posId = 0;
			symbolId = 0u;
			CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
			int curUseableCount = useableContainer.GetCurUseableCount();
			CSymbolItem cSymbolItem = null;
			int num = 0;
			for (int i = 0; i < curUseableCount; i++)
			{
				CUseable useableByIndex = useableContainer.GetUseableByIndex(i);
				if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
				{
					CSymbolItem cSymbolItem2 = (CSymbolItem)useableByIndex;
					enFindSymbolWearPosCode enFindSymbolWearPosCode;
					if (cSymbolItem2 != null && cSymbolItem2.m_stackCount > cSymbolItem2.GetPageWearCnt(0) && cSymbolItem2.GetPageWearCnt(0) < CSymbolWearController.s_maxSameIDSymbolEquipNum && this.GetWearPos(cSymbolItem2, 0, out num, out enFindSymbolWearPosCode))
					{
						if (colorType != 0)
						{
							if (cSymbolItem2.m_SymbolData.bColor == colorType && (cSymbolItem == null || cSymbolItem2.m_SymbolData.wLevel < cSymbolItem.m_SymbolData.wLevel))
							{
								cSymbolItem = cSymbolItem2;
								posId = num;
								symbolId = cSymbolItem2.m_baseID;
							}
						}
						else if (cSymbolItem == null || cSymbolItem2.m_SymbolData.bColor < cSymbolItem.m_SymbolData.bColor || (cSymbolItem2.m_SymbolData.bColor == cSymbolItem.m_SymbolData.bColor && cSymbolItem2.m_SymbolData.wLevel < cSymbolItem.m_SymbolData.wLevel))
						{
							cSymbolItem = cSymbolItem2;
							posId = num;
							symbolId = cSymbolItem2.m_baseID;
						}
					}
				}
			}
			return cSymbolItem != null;
		}

		public static int GetSymbolPosOpenCnt(int pvpLvl)
		{
			int num = 0;
			for (int i = 0; i < 30; i++)
			{
				ResSymbolPos symbolPos = CSymbolInfo.GetSymbolPos(i);
				if ((int)symbolPos.wOpenLevel <= pvpLvl)
				{
					num++;
				}
			}
			return num;
		}

		public static int GetSymbolLvlLimit(int pvpLvl)
		{
			ResHeroSymbolLvl dataByKey = GameDataMgr.heroSymbolLvlDatabin.GetDataByKey((uint)((ushort)pvpLvl));
			if (dataByKey == null)
			{
				return 0;
			}
			return (int)dataByKey.wSymbolMaxLvl;
		}

		public string GetMaxWearSymbolPageName(CSymbolItem symbol)
		{
			if (symbol == null)
			{
				return string.Empty;
			}
			string text = "\n";
			for (int i = 0; i < symbol.m_pageWearCnt.Length; i++)
			{
				if (symbol.m_stackCount <= symbol.m_pageWearCnt[i])
				{
					text += string.Format("{0}; ", this.GetSymbolPageName(i));
				}
			}
			return text;
		}

		public int GetSymbolPageMaxLvl(int pageIndex)
		{
			if (pageIndex < 0 || pageIndex >= this.m_pageCount)
			{
				return 0;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(false, "role is null");
				return 0;
			}
			int num = 0;
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			for (int i = 0; i < 30; i++)
			{
				if (this.m_symbolPageArr[pageIndex][i] > 0uL)
				{
					ulong objID = this.m_symbolPageArr[pageIndex][i];
					CUseable useableByObjID = useableContainer.GetUseableByObjID(objID);
					if (useableByObjID != null)
					{
						CSymbolItem cSymbolItem = (CSymbolItem)useableByObjID;
						if (cSymbolItem != null)
						{
							num += (int)cSymbolItem.m_SymbolData.wLevel;
						}
					}
				}
			}
			return num;
		}

		public static int GetSymbolLvWithArray(uint[] symbolArr)
		{
			int num = 0;
			for (int i = 0; i < symbolArr.Length; i++)
			{
				ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(symbolArr[i]);
				if (dataByKey != null)
				{
					num += (int)dataByKey.wLevel;
				}
			}
			return num;
		}

		public int GetNextFreePageLevel()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(false, "GetNextFreePageLevel role is null");
				return 0;
			}
			if (this.m_pageCount >= 50)
			{
				return 0;
			}
			for (int i = 0; i < CSymbolInfo.s_symbolPvpLvlList.Count; i++)
			{
				if ((uint)CSymbolInfo.s_symbolPvpLvlList[i].wPvpLevel > masterRoleInfo.PvpLevel && CSymbolInfo.s_symbolPvpLvlList[i].bPresentSymbolPage > 0)
				{
					return (int)CSymbolInfo.s_symbolPvpLvlList[i].wPvpLevel;
				}
			}
			return 0;
		}

		public bool IsPageEmpty(int pageIndex)
		{
			if (pageIndex < 0 || pageIndex >= this.m_pageCount)
			{
				return false;
			}
			for (int i = 0; i < 30; i++)
			{
				if (this.m_symbolPageArr[pageIndex][i] > 0uL)
				{
					return false;
				}
			}
			return true;
		}

		public void UpdateBuyGridInfo(int gridPos)
		{
			DebugHelper.Assert(gridPos > 0, string.Format("grid pos must be above 0!! pos: {0}", gridPos));
			DebugHelper.Assert(gridPos <= this.GridBuyInfo.Length, string.Format("grid pos must less than GridBuyInfo.Length!! pos: {0}", gridPos));
			this.GridBuyInfo[gridPos - 1].bBuyFlag = 1;
		}

		public bool IsGridPosHasBuy(int gridPos)
		{
			return this.GridBuyInfo != null && gridPos > 1 && gridPos <= this.GridBuyInfo.Length && this.GridBuyInfo[gridPos - 1].bBuyFlag == 1;
		}

		private uint GetLatestOpenLevel(uint playerLevel)
		{
			ResSymbolPos resSymbolPos = null;
			for (int i = 0; i < this.GridBuyInfo.Length; i++)
			{
				ResSymbolPos symbolPos = CSymbolInfo.GetSymbolPos(i);
				if ((uint)symbolPos.wOpenLevel > playerLevel && this.GridBuyInfo[i].bBuyFlag == 0)
				{
					if (resSymbolPos == null)
					{
						resSymbolPos = symbolPos;
					}
					if (symbolPos.wOpenLevel < resSymbolPos.wOpenLevel)
					{
						resSymbolPos = symbolPos;
					}
				}
			}
			if (resSymbolPos == null)
			{
				return playerLevel + 1u;
			}
			return (uint)resSymbolPos.wOpenLevel;
		}
	}
}
