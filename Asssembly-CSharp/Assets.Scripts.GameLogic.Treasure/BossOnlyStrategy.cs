using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic.Treasure
{
	internal abstract class BossOnlyStrategy : BaseStrategy
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
			if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
			{
				ResMonsterCfgInfo resMonsterCfgInfo = base.FindMonsterConfig(actor.handle.TheActorMeta.ConfigId);
				if (resMonsterCfgInfo != null && resMonsterCfgInfo.bMonsterGrade == 3)
				{
					this.FinishDrop();
				}
			}
		}
	}
}
