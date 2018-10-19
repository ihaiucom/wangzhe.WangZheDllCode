using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class PointsExchangeActivity : Activity
	{
		public readonly ResWealPointExchange PointsConfig;

		private readonly Dictionary<int, uint> _exchangeCount = new Dictionary<int, uint>();

		private List<int> _delTempList = new List<int>();

		private uint _occPointValue;

		private uint _occConsumeValue;

		private uint _pointPerConsume;

		public override uint ID
		{
			get
			{
				return this.PointsConfig.dwID;
			}
		}

		public override COM_WEAL_TYPE Type
		{
			get
			{
				return COM_WEAL_TYPE.COM_WEAL_PTEXCHANGE;
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
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				return string.Format(base.Brief, new object[]
				{
					this.PerConsume,
					this.PerPoint,
					this.OccuConsume,
					this.OccuPoint,
					masterRoleInfo.JiFen
				});
			}
		}

		public uint OccuPoint
		{
			get
			{
				return this._occPointValue;
			}
		}

		public uint OccuConsume
		{
			get
			{
				return this._occConsumeValue;
			}
		}

		public uint PerConsume
		{
			get
			{
				return (this.PointsConfig != null) ? this.PointsConfig.dwPointGetParam : 1u;
			}
		}

		public uint PerPoint
		{
			get
			{
				return (this.PointsConfig != null) ? this.PointsConfig.dwPointGetCnt : 1u;
			}
		}

		public PointsExchangeActivity(ActivitySys mgr, ResWealPointExchange config) : base(mgr, config.stCommon)
		{
			this.PointsConfig = config;
			for (uint num = 0u; num < (uint)config.bExchangeCnt; num += 1u)
			{
				PointsExchangePhase ap = new PointsExchangePhase(this, num, config.astExchangeInfo[(int)((UIntPtr)num)]);
				base.AddPhase(ap);
			}
		}

		public override void UpdateInfo(ref COMDT_WEAL_UNION actvInfo)
		{
			this._exchangeCount.Clear();
			byte bWealCnt = actvInfo.stPtExchange.bWealCnt;
			COMDT_WEAL_EXCHANGE_OBJ[] astWealList = actvInfo.stPtExchange.astWealList;
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
				ResDT_PointExchange resDT_PointExchange = this.PointsConfig.astExchangeInfo[index];
				if (num < resDT_PointExchange.dwDupCnt)
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
				if (key >= 0 && key < this.PointsConfig.astExchangeInfo.Length)
				{
					ResDT_PointExchange resDT_PointExchange = this.PointsConfig.astExchangeInfo[key];
					if (resDT_PointExchange.bIsDupClr > 0)
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
			if (index < (int)this.PointsConfig.bExchangeCnt)
			{
				return this.PointsConfig.astExchangeInfo[index].dwDupCnt;
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

		public void UpdatePointsInfo(uint occJiFen, uint occConsumeValue)
		{
			this._occPointValue = occJiFen;
			this._occConsumeValue = occConsumeValue;
		}
	}
}
