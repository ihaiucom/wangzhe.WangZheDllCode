using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class SoldierSelector
	{
		private ResSoldierTypeInfo[] SoldierConfigList;

		public uint StatTotalCount;

		public int CurrentIndex
		{
			get;
			private set;
		}

		public int TotalCount
		{
			get;
			private set;
		}

		public int CurrentCount
		{
			get;
			private set;
		}

		public uint LastSoldierId
		{
			get;
			private set;
		}

		public bool isFinished
		{
			get
			{
				return (long)this.TotalCount >= (long)((ulong)this.StatTotalCount);
			}
		}

		public void Reset(ResSoldierWaveInfo InWaveInfo)
		{
			this.CurrentIndex = 0;
			this.TotalCount = 0;
			this.CurrentCount = 0;
			this.StatTotalCount = 0u;
			this.SoldierConfigList = InWaveInfo.astNormalSoldierInfo;
			for (int i = 0; i < 5; i++)
			{
				this.StatTotalCount += this.SoldierConfigList[i].dwSoldierNum;
			}
		}

		public uint NextSoldierID()
		{
			if ((long)this.CurrentCount < (long)((ulong)this.SoldierConfigList[this.CurrentIndex].dwSoldierNum))
			{
				this.CurrentCount++;
				this.TotalCount++;
				this.LastSoldierId = this.SoldierConfigList[this.CurrentIndex].dwSoldierID;
				return this.LastSoldierId;
			}
			this.CurrentCount = 0;
			this.CurrentIndex++;
			for (int i = this.CurrentIndex; i < 5; i++)
			{
				if ((long)this.CurrentCount < (long)((ulong)this.SoldierConfigList[i].dwSoldierNum))
				{
					this.CurrentCount++;
					this.TotalCount++;
					this.CurrentIndex = i;
					this.LastSoldierId = this.SoldierConfigList[i].dwSoldierID;
					return this.LastSoldierId;
				}
			}
			this.LastSoldierId = 0u;
			return 0u;
		}

		public uint PeekNextSoldierId()
		{
			return this.SoldierConfigList[0].dwSoldierID;
		}
	}
}
