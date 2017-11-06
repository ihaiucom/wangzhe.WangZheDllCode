using Assets.Scripts.Framework;
using ResData;
using System;
using System.Collections.Generic;

public class NewbieGuideDataManager : Singleton<NewbieGuideDataManager>
{
	private NewbieGuideMainLineConf[] mMainLineCacheArr;

	private NewbieWeakGuideMainLineConf[] mWeakMainLineCacheArr;

	private DictionaryView<uint, ListView<NewbieGuideScriptConf>> mScriptCacheDic;

	private DictionaryView<uint, ListView<NewbieGuideWeakConf>> mWeakScriptCacheDic;

	private NewbieGuideSpecialTipConf[] mSpecialTipCacheArr;

	private ListView<NewbieGuideMainLineConf> mCacheMainLineSourceList;

	private ListView<NewbieGuideMainLineConf> mCacheMainLineTargetList;

	private ListView<NewbieWeakGuideMainLineConf> mCacheWeakSourceList;

	private ListView<NewbieWeakGuideMainLineConf> mCacheWeakTargetList;

	public NewbieGuideDataManager()
	{
		this.mMainLineCacheArr = new NewbieGuideMainLineConf[GameDataMgr.newbieMainLineDatabin.Count()];
		GameDataMgr.newbieMainLineDatabin.CopyTo(ref this.mMainLineCacheArr);
		this.SortMainLineList(this.mMainLineCacheArr);
		this.mScriptCacheDic = new DictionaryView<uint, ListView<NewbieGuideScriptConf>>();
		NewbieGuideScriptConf[] array = new NewbieGuideScriptConf[GameDataMgr.newbieScriptDatabin.RawDatas.Length];
		GameDataMgr.newbieScriptDatabin.RawDatas.CopyTo(array, 0);
		int num = array.Length;
		for (int i = 0; i < num; i++)
		{
			NewbieGuideScriptConf newbieGuideScriptConf = array[i];
			ListView<NewbieGuideScriptConf> listView;
			if (!this.mScriptCacheDic.TryGetValue((uint)newbieGuideScriptConf.wMainLineID, out listView))
			{
				listView = new ListView<NewbieGuideScriptConf>();
				this.mScriptCacheDic.Add((uint)newbieGuideScriptConf.wMainLineID, listView);
			}
			listView.Add(newbieGuideScriptConf);
		}
		this.mSpecialTipCacheArr = new NewbieGuideSpecialTipConf[GameDataMgr.newbieSpecialTipDatabin.count];
		GameDataMgr.newbieSpecialTipDatabin.CopyTo(ref this.mSpecialTipCacheArr);
		this.mWeakMainLineCacheArr = new NewbieWeakGuideMainLineConf[GameDataMgr.newbieWeakMainLineDataBin.count];
		GameDataMgr.newbieWeakMainLineDataBin.CopyTo(ref this.mWeakMainLineCacheArr);
		this.SortWeakMainLineList(this.mWeakMainLineCacheArr);
		NewbieGuideWeakConf[] array2 = new NewbieGuideWeakConf[GameDataMgr.newbieWeakDatabin.count];
		GameDataMgr.newbieWeakDatabin.CopyTo(ref array2);
		this.mWeakScriptCacheDic = new DictionaryView<uint, ListView<NewbieGuideWeakConf>>();
		num = array2.Length;
		for (int j = 0; j < num; j++)
		{
			NewbieGuideWeakConf newbieGuideWeakConf = array2[j];
			ListView<NewbieGuideWeakConf> listView2;
			if (!this.mWeakScriptCacheDic.TryGetValue(newbieGuideWeakConf.dwWeakLineID, out listView2))
			{
				listView2 = new ListView<NewbieGuideWeakConf>();
				this.mWeakScriptCacheDic.Add(newbieGuideWeakConf.dwWeakLineID, listView2);
			}
			listView2.Add(newbieGuideWeakConf);
		}
		this.mCacheMainLineSourceList = new ListView<NewbieGuideMainLineConf>();
		this.mCacheMainLineTargetList = new ListView<NewbieGuideMainLineConf>();
		this.mCacheWeakSourceList = new ListView<NewbieWeakGuideMainLineConf>();
		this.mCacheWeakTargetList = new ListView<NewbieWeakGuideMainLineConf>();
	}

	private void SortMainLineList(NewbieGuideMainLineConf[] list)
	{
		for (int i = 1; i < list.Length; i++)
		{
			NewbieGuideMainLineConf newbieGuideMainLineConf = list[i];
			int num = i;
			while (num > 0 && this.SortMainLineConf(list[num - 1], newbieGuideMainLineConf))
			{
				list[num] = list[num - 1];
				num--;
			}
			list[num] = newbieGuideMainLineConf;
		}
	}

	private bool SortMainLineConf(NewbieGuideMainLineConf confA, NewbieGuideMainLineConf confB)
	{
		return confA.dwPriority <= confB.dwPriority && (confA.dwPriority < confB.dwPriority || confA.dwID >= confB.dwID);
	}

	public ListView<NewbieGuideScriptConf> GetScriptList(uint mainLineID)
	{
		ListView<NewbieGuideScriptConf> result;
		if (this.mScriptCacheDic.TryGetValue(mainLineID, out result))
		{
			return result;
		}
		return null;
	}

	public NewbieGuideMainLineConf GetNewbieGuideMainLineConf(uint id)
	{
		int num = this.mMainLineCacheArr.Length;
		for (int i = 0; i < num; i++)
		{
			NewbieGuideMainLineConf newbieGuideMainLineConf = this.mMainLineCacheArr[i];
			if (newbieGuideMainLineConf.dwID == id)
			{
				return newbieGuideMainLineConf;
			}
		}
		return null;
	}

	public List<uint> GetMainLineIDList()
	{
		List<uint> list = new List<uint>();
		int num = this.mMainLineCacheArr.Length;
		for (int i = 0; i < num; i++)
		{
			NewbieGuideMainLineConf newbieGuideMainLineConf = this.mMainLineCacheArr[i];
			list.Add(newbieGuideMainLineConf.dwID);
		}
		return list;
	}

	public List<uint> GetWeakMianLineIDList()
	{
		List<uint> list = new List<uint>();
		int num = this.mWeakMainLineCacheArr.Length;
		for (int i = 0; i < num; i++)
		{
			NewbieWeakGuideMainLineConf newbieWeakGuideMainLineConf = this.mWeakMainLineCacheArr[i];
			list.Add(newbieWeakGuideMainLineConf.dwID);
		}
		return list;
	}

	public NewbieWeakGuideMainLineConf GetNewbieWeakGuideMainLineConf(uint id)
	{
		int num = this.mWeakMainLineCacheArr.Length;
		for (int i = 0; i < num; i++)
		{
			NewbieWeakGuideMainLineConf newbieWeakGuideMainLineConf = this.mWeakMainLineCacheArr[i];
			if (newbieWeakGuideMainLineConf.dwID == id)
			{
				return newbieWeakGuideMainLineConf;
			}
		}
		return null;
	}

	public ListView<NewbieGuideMainLineConf> GetNewbieGuideMainLineConfListBySkipType(NewbieGuideSkipConditionType type)
	{
		this.mCacheMainLineSourceList.Clear();
		this.mCacheMainLineSourceList.AddRange(this.mMainLineCacheArr);
		ListView<NewbieGuideMainLineConf> listView = new ListView<NewbieGuideMainLineConf>();
		int count = this.mCacheMainLineSourceList.Count;
		for (int i = 0; i < count; i++)
		{
			NewbieGuideMainLineConf newbieGuideMainLineConf = this.mCacheMainLineSourceList[i];
			for (int j = 0; j < newbieGuideMainLineConf.astSkipCondition.Length; j++)
			{
				if ((NewbieGuideSkipConditionType)newbieGuideMainLineConf.astSkipCondition[j].wType == type)
				{
					listView.Add(newbieGuideMainLineConf);
					break;
				}
			}
		}
		return listView;
	}

	public ListView<NewbieGuideMainLineConf> GetNewbieGuideMainLineConfListByTriggerTimeType(NewbieGuideTriggerTimeType type, uint[] param)
	{
		this.mCacheMainLineSourceList.Clear();
		this.mCacheMainLineSourceList.AddRange(this.mMainLineCacheArr);
		this.mCacheMainLineTargetList.Clear();
		int count = this.mCacheMainLineSourceList.Count;
		for (int i = 0; i < count; i++)
		{
			NewbieGuideMainLineConf newbieGuideMainLineConf = this.mCacheMainLineSourceList[i];
			if (this.IsContainsTriggerTimeType(newbieGuideMainLineConf, type, param))
			{
				this.mCacheMainLineTargetList.Add(newbieGuideMainLineConf);
			}
		}
		return this.mCacheMainLineTargetList;
	}

	public bool IsContainsTriggerTimeType(NewbieGuideMainLineConf data, NewbieGuideTriggerTimeType type, uint[] param)
	{
		int num = data.astTriggerTime.Length;
		for (int i = 0; i < num; i++)
		{
			NewbieGuideTriggerTimeItem newbieGuideTriggerTimeItem = data.astTriggerTime[i];
			if (type == (NewbieGuideTriggerTimeType)newbieGuideTriggerTimeItem.wType && NewbieGuideCheckTriggerTimeUtil.CheckTriggerTime(newbieGuideTriggerTimeItem, param))
			{
				return true;
			}
		}
		return false;
	}

	public ListView<NewbieWeakGuideMainLineConf> GetNewBieGuideWeakMainLineConfListByTiggerTimeType(NewbieGuideTriggerTimeType type, uint[] param)
	{
		this.mCacheWeakSourceList.Clear();
		this.mCacheWeakSourceList.AddRange(this.mWeakMainLineCacheArr);
		this.mCacheWeakTargetList.Clear();
		int count = this.mCacheWeakSourceList.Count;
		for (int i = 0; i < count; i++)
		{
			NewbieWeakGuideMainLineConf newbieWeakGuideMainLineConf = this.mCacheWeakSourceList[i];
			if (this.IsContainsTriggerTimeType(newbieWeakGuideMainLineConf, type, param))
			{
				this.mCacheWeakTargetList.Add(newbieWeakGuideMainLineConf);
			}
		}
		return this.mCacheWeakTargetList;
	}

	public bool IsContainsTriggerTimeType(NewbieWeakGuideMainLineConf conf, NewbieGuideTriggerTimeType type, uint[] param)
	{
		int num = conf.astTriggerTime.Length;
		for (int i = 0; i < num; i++)
		{
			NewbieGuideTriggerTimeItem newbieGuideTriggerTimeItem = conf.astTriggerTime[i];
			if (type == (NewbieGuideTriggerTimeType)newbieGuideTriggerTimeItem.wType && NewbieGuideCheckTriggerTimeUtil.CheckTriggerTime(newbieGuideTriggerTimeItem, param))
			{
				return true;
			}
		}
		return false;
	}

	public ListView<NewbieWeakGuideMainLineConf> GetNewbieGuideWeakMianLineConfListBySkipType(NewbieGuideSkipConditionType type)
	{
		this.mCacheWeakSourceList.Clear();
		this.mCacheWeakSourceList.AddRange(this.mWeakMainLineCacheArr);
		ListView<NewbieWeakGuideMainLineConf> listView = new ListView<NewbieWeakGuideMainLineConf>();
		int count = this.mCacheWeakSourceList.Count;
		for (int i = 0; i < count; i++)
		{
			NewbieWeakGuideMainLineConf newbieWeakGuideMainLineConf = this.mCacheWeakSourceList[i];
			for (int j = 0; j < newbieWeakGuideMainLineConf.astSkipCondition.Length; j++)
			{
				if ((NewbieGuideSkipConditionType)newbieWeakGuideMainLineConf.astSkipCondition[j].wType == type)
				{
					listView.Add(newbieWeakGuideMainLineConf);
					break;
				}
			}
		}
		return listView;
	}

	public bool IsContainsSkipConditionType(NewbieWeakGuideMainLineConf conf, NewbieGuideSkipConditionType type, uint[] param)
	{
		int num = conf.astSkipCondition.Length;
		for (int i = 0; i < num; i++)
		{
			NewbieGuideSkipConditionItem newbieGuideSkipConditionItem = conf.astSkipCondition[i];
			if ((NewbieGuideSkipConditionType)newbieGuideSkipConditionItem.wType == type && NewbieGuideCheckSkipConditionUtil.CheckSkipCondition(newbieGuideSkipConditionItem, param))
			{
				return true;
			}
		}
		return false;
	}

	private void SortWeakMainLineList(NewbieWeakGuideMainLineConf[] list)
	{
		for (int i = 1; i < list.Length; i++)
		{
			NewbieWeakGuideMainLineConf newbieWeakGuideMainLineConf = list[i];
			int num = i;
			while (num > 0 && this.SortWeakMianLineConf(list[num - 1], newbieWeakGuideMainLineConf))
			{
				list[num] = list[num - 1];
				num--;
			}
			list[num] = newbieWeakGuideMainLineConf;
		}
	}

	private bool SortWeakMianLineConf(NewbieWeakGuideMainLineConf confA, NewbieWeakGuideMainLineConf confB)
	{
		return confA.dwPriority <= confB.dwPriority && (confA.dwPriority < confB.dwPriority || confA.dwID >= confB.dwID);
	}

	public ListView<NewbieGuideWeakConf> GetWeakScriptList(uint mainLineID)
	{
		ListView<NewbieGuideWeakConf> result = new ListView<NewbieGuideWeakConf>();
		if (this.mWeakScriptCacheDic.TryGetValue(mainLineID, out result))
		{
			return result;
		}
		return null;
	}

	public NewbieGuideSpecialTipConf GetSpecialTipConfig(uint id)
	{
		int num = this.mSpecialTipCacheArr.Length;
		for (int i = 0; i < num; i++)
		{
			NewbieGuideSpecialTipConf newbieGuideSpecialTipConf = this.mSpecialTipCacheArr[i];
			if (newbieGuideSpecialTipConf.dwID == id)
			{
				return newbieGuideSpecialTipConf;
			}
		}
		return null;
	}
}
