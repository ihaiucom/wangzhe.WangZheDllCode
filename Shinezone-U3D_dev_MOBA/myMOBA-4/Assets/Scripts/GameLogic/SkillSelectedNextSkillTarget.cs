using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[SkillBaseSelectTarget(SkillTargetRule.NextSkillTarget)]
	public class SkillSelectedNextSkillTarget : SkillBaseSelectTarget
	{
		public override ActorRoot SelectTarget(SkillSlot UseSlot)
		{
			return this.GetNextSkillTarget(UseSlot);
		}

		public override VInt3 SelectTargetDir(SkillSlot UseSlot)
		{
			ActorRoot nextSkillTarget = this.GetNextSkillTarget(UseSlot);
			if (nextSkillTarget != null)
			{
				VInt3 vInt = nextSkillTarget.location - UseSlot.Actor.handle.location;
				vInt.y = 0;
				return vInt.NormalizeTo(1000);
			}
			return UseSlot.Actor.handle.forward;
		}

		private ActorRoot GetNextSkillTarget(SkillSlot UseSlot)
		{
			for (int i = 0; i < UseSlot.NextSkillTargetIDs.Count; i++)
			{
				ActorRoot actorRoot = Singleton<GameObjMgr>.GetInstance().GetActor(UseSlot.NextSkillTargetIDs[i]);
				if (actorRoot != null)
				{
					if (((ulong)UseSlot.SkillObj.cfgData.dwSkillTargetFilter & (ulong)(1L << (int)(actorRoot.TheActorMeta.ActorType & (ActorTypeDef)31))) <= 0uL && actorRoot.HorizonMarker.IsVisibleFor(UseSlot.Actor.handle.TheActorMeta.ActorCamp) && UseSlot.Actor.handle.CanAttack(actorRoot))
					{
						if (DistanceSearchCondition.Fit(actorRoot, UseSlot.Actor.handle, UseSlot.SkillObj.GetMaxSearchDistance(UseSlot.GetSkillLevel())))
						{
							return actorRoot;
						}
					}
				}
			}
			return null;
		}
	}
}
