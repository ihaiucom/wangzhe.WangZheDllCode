using Assets.Scripts.Common;
using ResData;
using System;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.GameLogic.Treasure
{
	internal abstract class BaseStrategy : ITreasureChestStrategy
	{
		protected int MaxCount;

		protected int DropedCount;

		public event OnDropTreasureChestDelegate OnDropTreasure
		{
			[MethodImpl(32)]
			add
			{
				this.OnDropTreasure = (OnDropTreasureChestDelegate)Delegate.Combine(this.OnDropTreasure, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.OnDropTreasure = (OnDropTreasureChestDelegate)Delegate.Remove(this.OnDropTreasure, value);
			}
		}

		public int maxCount
		{
			get
			{
				return this.MaxCount;
			}
		}

		public virtual bool isSupportDrop
		{
			get
			{
				return false;
			}
		}

		public int droppedCount
		{
			get
			{
				return this.DropedCount;
			}
		}

		protected virtual bool hasRemain
		{
			get
			{
				return this.DropedCount < this.maxCount;
			}
		}

		public virtual void Initialize(int InMaxCount)
		{
			this.MaxCount = InMaxCount;
			this.DropedCount = 0;
			DebugHelper.Assert(this.MaxCount < 128, "你tm在逗我？");
		}

		public virtual void Stop()
		{
		}

		public virtual void NotifyDropEvent(PoolObjHandle<ActorRoot> actor)
		{
		}

		public virtual void NotifyMatchEnd()
		{
			if (!Singleton<StarSystem>.instance.isFailure)
			{
				this.FinishDrop();
			}
			this.MaxCount = 0;
			this.DropedCount = 0;
		}

		public virtual void PlayDrop()
		{
			this.DropedCount++;
			DebugHelper.Assert(this.DropedCount <= this.maxCount, "尼玛你是认真的么？");
			if (this.OnDropTreasure != null)
			{
				this.OnDropTreasure();
			}
		}

		protected virtual void FinishDrop()
		{
			while (this.DropedCount < this.MaxCount)
			{
				this.PlayDrop();
			}
		}

		public ResMonsterCfgInfo FindMonsterConfig(int ConfigID)
		{
			return MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(ConfigID);
		}
	}
}
