using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameLogic.Treasure
{
	internal abstract class NormalStrategy : BaseStrategy
	{
		public override bool isSupportDrop
		{
			get
			{
				return true;
			}
		}

		protected override bool hasRemain
		{
			get
			{
				return this.DropedCount < this.maxCount - 1;
			}
		}

		public override void NotifyDropEvent(PoolObjHandle<ActorRoot> actor)
		{
			DebugHelper.Assert(actor);
			if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
			{
				ResMonsterCfgInfo resMonsterCfgInfo = base.FindMonsterConfig(actor.handle.TheActorMeta.ConfigId);
				DebugHelper.Assert(resMonsterCfgInfo != null, "怪物数据档里面找不到id:{0}", new object[]
				{
					actor.handle.TheActorMeta.ConfigId
				});
				if (resMonsterCfgInfo != null)
				{
					RES_DROP_PROBABILITY_TYPE iDropProbability = (RES_DROP_PROBABILITY_TYPE)resMonsterCfgInfo.iDropProbability;
					if (iDropProbability == RES_DROP_PROBABILITY_TYPE.RES_PROBABILITY_SETTLE)
					{
						this.FinishDrop();
					}
					else if (this.hasRemain && iDropProbability != (RES_DROP_PROBABILITY_TYPE)0 && FrameRandom.Random(100u) <= (ushort)iDropProbability)
					{
						this.PlayDrop();
					}
				}
			}
		}
	}
}
