using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using System;

namespace Assets.Scripts.GameLogic.Treasure
{
	internal abstract class AverageStrategy : BaseStrategy
	{
		public override bool isSupportDrop
		{
			get
			{
				return true;
			}
		}

		public override void NotifyDropEvent(PoolObjHandle<ActorRoot> actor)
		{
			DebugHelper.Assert(actor);
			if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster && this.hasRemain && (int)FrameRandom.Random(100u) <= MonoSingleton<GlobalConfig>.instance.NormalMonsterDropItemProbability)
			{
				this.PlayDrop();
			}
		}
	}
}
