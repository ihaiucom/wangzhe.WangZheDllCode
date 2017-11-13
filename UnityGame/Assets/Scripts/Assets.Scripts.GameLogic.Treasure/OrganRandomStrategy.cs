using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameLogic.Treasure
{
	internal abstract class OrganRandomStrategy : BaseStrategy
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
			if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
			{
				ResOrganCfgInfo dataCfgInfoByCurLevelDiff = OrganDataHelper.GetDataCfgInfoByCurLevelDiff(actor.handle.TheActorMeta.ConfigId);
				DebugHelper.Assert(dataCfgInfoByCurLevelDiff != null, "can't find organ config, id={0}", new object[]
				{
					actor.handle.TheActorMeta.ConfigId
				});
				if (dataCfgInfoByCurLevelDiff == null)
				{
					return;
				}
				if (dataCfgInfoByCurLevelDiff.bOrganType == 2)
				{
					this.FinishDrop();
				}
				else if (this.hasRemain && (int)FrameRandom.Random(100u) <= MonoSingleton<GlobalConfig>.instance.OrganDropItemProbability)
				{
					this.PlayDrop();
				}
			}
		}
	}
}
