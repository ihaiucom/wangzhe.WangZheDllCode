using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	public class VDStat
	{
		private int CurrentIndex;

		private static readonly int s_MaxStamps = 11;

		private static readonly uint StepCount = 1800u;

		private CampDataStamp[] Stamps = new CampDataStamp[VDStat.s_MaxStamps];

		public int count
		{
			get
			{
				return this.CurrentIndex + 1;
			}
		}

		public VDStat()
		{
			for (int i = 0; i < this.Stamps.Length; i++)
			{
				this.Stamps[i] = new CampDataStamp();
			}
		}

		public void StartRecord()
		{
			this.Clear();
			if (this.ShouldStat())
			{
				Singleton<EventRouter>.instance.AddEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this.OnHeroGoldCoinChanged));
			}
		}

		public bool ShouldStat()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			return curLvelContext.IsMobaMode();
		}

		public void Clear()
		{
			this.CurrentIndex = 0;
			for (int i = 0; i < this.Stamps.Length; i++)
			{
				if (this.Stamps[i] != null)
				{
					this.Stamps[i].Clear();
				}
			}
			Singleton<EventRouter>.instance.RemoveEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this.OnHeroGoldCoinChanged));
		}

		private void OnHeroGoldCoinChanged(PoolObjHandle<ActorRoot> InActor, int InChangedValue, bool bInIsIncome, PoolObjHandle<ActorRoot> target)
		{
			if (InChangedValue > 0 && bInIsIncome && InActor)
			{
				this.TryMoveToNext();
				if (this.CurrentIndex >= 0 && this.CurrentIndex < this.Stamps.Length && this.Stamps[this.CurrentIndex] != null)
				{
					this.Stamps[this.CurrentIndex].OnHeroGoldCoinChanged(ref InActor, InChangedValue, bInIsIncome, target);
				}
			}
		}

		private void TryMoveToNext()
		{
			uint num = Singleton<FrameSynchr>.instance.CurFrameNum / VDStat.StepCount;
			if ((long)this.CurrentIndex < (long)((ulong)num) && (ulong)num < (ulong)((long)this.Stamps.Length))
			{
				this.CurrentIndex = (int)num;
			}
		}

		public void GetMaxCampStat(int InStampIndex, COM_PLAYERCAMP InFrom, COM_PLAYERCAMP InTo, out int OutMaxPositive, out int OutMaxNegative)
		{
			this.Stamps[InStampIndex].GetMaxCampStat(InFrom, InTo, out OutMaxPositive, out OutMaxNegative);
		}

		public int CalcCampStat(COM_PLAYERCAMP InFrom, COM_PLAYERCAMP InTo)
		{
			return this.Stamps[this.CurrentIndex].CalcCampStat(InFrom, InTo);
		}
	}
}
