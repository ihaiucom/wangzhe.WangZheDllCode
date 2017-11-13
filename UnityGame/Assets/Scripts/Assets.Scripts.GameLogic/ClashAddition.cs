using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class ClashAddition
	{
		private DictionaryView<uint, DictionaryView<uint, ResClashAddition>> _dataDict;

		public void FightStart()
		{
			this._dataDict = new DictionaryView<uint, DictionaryView<uint, ResClashAddition>>();
			object[] rawDatas = GameDataMgr.clashAdditionDB.RawDatas;
			for (int i = 0; i < rawDatas.Length; i++)
			{
				ResClashAddition resClashAddition = rawDatas[i] as ResClashAddition;
				DictionaryView<uint, ResClashAddition> dictionaryView;
				if (!this._dataDict.ContainsKey(resClashAddition.dwAttackerMark))
				{
					dictionaryView = new DictionaryView<uint, ResClashAddition>();
					this._dataDict.Add(resClashAddition.dwAttackerMark, dictionaryView);
				}
				else
				{
					dictionaryView = this._dataDict[resClashAddition.dwAttackerMark];
				}
				if (!dictionaryView.ContainsKey(resClashAddition.dwSuffererMark))
				{
					dictionaryView.Add(resClashAddition.dwSuffererMark, resClashAddition);
				}
			}
		}

		public void FightOver()
		{
			this._dataDict = null;
		}

		public int CalcDamageAddition(uint attackerMark, uint suffererMark)
		{
			if (this._dataDict != null && this._dataDict.ContainsKey(attackerMark))
			{
				DictionaryView<uint, ResClashAddition> dictionaryView = this._dataDict[attackerMark];
				if (dictionaryView.ContainsKey(suffererMark))
				{
					ResClashAddition resClashAddition = dictionaryView[suffererMark];
					return Singleton<BattleLogic>.GetInstance().dynamicProperty.GetDynamicDamage(resClashAddition.dwDynamicConfig, (int)resClashAddition.dwDamageAddition);
				}
			}
			return 10000;
		}
	}
}
