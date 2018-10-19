using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class ExchangeActivity : Activity
	{
		private readonly ResCltWealExchange _config;

		private readonly Dictionary<int, uint> _exchangeCount = new Dictionary<int, uint>();

		private List<int> _delTempList = new List<int>();

		public override uint ID
		{
			get
			{
				return this._config.dwID;
			}
		}

		public override COM_WEAL_TYPE Type
		{
			get
			{
				return COM_WEAL_TYPE.COM_WEAL_EXCHANGE;
			}
		}

		public override bool Completed
		{
			get
			{
				return false;
			}
		}

		public override bool ReadyForDot
		{
			get
			{
				bool result = false;
				for (int i = 0; i < base.PhaseList.Count; i++)
				{
					ActivityPhase activityPhase = base.PhaseList[i];
					if (activityPhase != null && activityPhase.ReadyForGet)
					{
						result = true;
						break;
					}
				}
				return result;
			}
		}

		public override string Brief
		{
			get
			{
				return base.Brief;
			}
		}

		public ExchangeActivity(ActivitySys mgr, ResCltWealExchange config) : base(mgr, config.stCommon)
		{
			this._config = config;
			for (uint num = 0u; num < (uint)config.bExchangeCnt; num += 1u)
			{
				ExchangePhase ap = new ExchangePhase(this, config.astExchangeList[(int)((UIntPtr)num)]);
				base.AddPhase(ap);
			}
		}

		public override void UpdateInfo(ref COMDT_WEAL_UNION actvInfo)
		{
			this._exchangeCount.Clear();
			byte bWealCnt = actvInfo.stExchange.bWealCnt;
			COMDT_WEAL_EXCHANGE_OBJ[] astWealList = actvInfo.stExchange.astWealList;
			for (int i = 0; i < (int)bWealCnt; i++)
			{
				this._exchangeCount.Add((int)astWealList[i].bWealIdx, astWealList[i].dwExchangeCnt);
			}
		}

		public void IncreaseExchangeCount(int index, uint increaseCount = 1u)
		{
			uint num = 0u;
			if (this._exchangeCount.TryGetValue(index, out num))
			{
				ResDT_WealExchagne_Info resDT_WealExchagne_Info = this._config.astExchangeList[index - 1];
				if (num < (uint)resDT_WealExchagne_Info.wDupCnt)
				{
					this._exchangeCount.Remove(index);
					this._exchangeCount.Add(index, num + increaseCount);
				}
			}
			else
			{
				this._exchangeCount.Add(index, increaseCount);
			}
		}

		public void ResetExchangeCount()
		{
			this._delTempList.Clear();
			Dictionary<int, uint>.Enumerator enumerator = this._exchangeCount.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, uint> current = enumerator.Current;
				int key = current.Key;
				if (key >= 0 && key < this._config.astExchangeList.Length)
				{
					ResDT_WealExchagne_Info resDT_WealExchagne_Info = this._config.astExchangeList[key - 1];
					if (resDT_WealExchagne_Info.bIsDupClr > 0)
					{
						this._delTempList.Add(key);
					}
				}
			}
			List<int>.Enumerator enumerator2 = this._delTempList.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				int current2 = enumerator2.Current;
				this._exchangeCount.Remove(current2);
			}
			this._delTempList.Clear();
		}

		public uint GetMaxExchangeCount(int index)
		{
			if (index - 1 < (int)this._config.bExchangeCnt)
			{
				return (uint)this._config.astExchangeList[index - 1].wDupCnt;
			}
			return 0u;
		}

		public uint GetExchangeCount(int index)
		{
			uint num = 0u;
			return (!this._exchangeCount.TryGetValue(index, out num)) ? 0u : num;
		}

		public void UpdateView()
		{
			base.NotifyTimeStateChanged();
		}
	}
}
