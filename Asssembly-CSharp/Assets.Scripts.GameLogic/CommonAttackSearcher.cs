using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using System;

namespace Assets.Scripts.GameLogic
{
	internal class CommonAttackSearcher : Singleton<CommonAttackSearcher>
	{
		public uint CommonAttackSearchEnemy(PoolObjHandle<ActorRoot> InActor, int srchR)
		{
			Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref InActor);
			SelectEnemyType selectEnemyType;
			if (ownerPlayer == null)
			{
				selectEnemyType = SelectEnemyType.SelectLowHp;
			}
			else
			{
				selectEnemyType = ownerPlayer.AttackTargetMode;
			}
			if (selectEnemyType == SelectEnemyType.SelectLowHp)
			{
				return this.CommonAttackSearchLowestHpTarget(InActor.handle.ActorControl, srchR);
			}
			return this.CommonAttackSearchNearestTarget(InActor.handle.ActorControl, srchR);
		}

		public uint AdvanceCommonAttackSearchEnemy(PoolObjHandle<ActorRoot> InActor, int srchR)
		{
			uint result = 0u;
			SkillSlot skillSlot = null;
			bool flag = false;
			Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref InActor);
			if (InActor.handle.SkillControl.TryGetSkillSlot(SkillSlotType.SLOT_SKILL_0, out skillSlot) && skillSlot.skillIndicator != null)
			{
				flag = skillSlot.skillIndicator.GetUseAdvanceMode();
			}
			if (flag)
			{
				if (skillSlot != null || skillSlot.skillIndicator != null)
				{
					ActorRoot useSkillTargetDefaultAttackMode = skillSlot.skillIndicator.GetUseSkillTargetDefaultAttackMode();
					if (useSkillTargetDefaultAttackMode != null)
					{
						result = useSkillTargetDefaultAttackMode.ObjID;
					}
				}
				return result;
			}
			SelectEnemyType selectEnemyType;
			if (ownerPlayer == null)
			{
				selectEnemyType = SelectEnemyType.SelectLowHp;
			}
			else
			{
				selectEnemyType = ownerPlayer.AttackTargetMode;
			}
			if (selectEnemyType == SelectEnemyType.SelectLowHp)
			{
				return this.CommonAttackSearchLowestHpTarget(InActor.handle.ActorControl, srchR);
			}
			return this.CommonAttackSearchNearestTarget(InActor.handle.ActorControl, srchR);
		}

		public uint CommonAttackSearchLowestHpTarget(ObjWrapper _wrapper, int _srchR)
		{
			ActorRoot actorRoot = null;
			ActorRoot actorRoot2 = Singleton<TargetSearcher>.GetInstance().GetLowestHpTarget(_wrapper.actor, _wrapper.AttackRange, TargetPriority.TargetPriority_Hero, 0u, true, false);
			if (actorRoot2 == null || actorRoot2.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)
			{
				actorRoot = Singleton<TargetSearcher>.GetInstance().GetLowestHpTarget(_wrapper.actor, _wrapper.GreaterRange, TargetPriority.TargetPriority_Hero, 0u, true, false);
			}
			if (actorRoot != null && actorRoot.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				actorRoot2 = actorRoot;
			}
			if (actorRoot2 == null)
			{
				actorRoot2 = Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(_wrapper.actor, _srchR, 0u, false);
			}
			if (actorRoot2 != null)
			{
				return actorRoot2.ObjID;
			}
			actorRoot2 = Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(_wrapper.actor, _srchR, 0u, true);
			if (actorRoot2 != null)
			{
				return actorRoot2.ObjID;
			}
			return 0u;
		}

		public uint CommonAttackSearchNearestTarget(ObjWrapper _wrapper, int _srchR)
		{
			ActorRoot nearestEnemy = Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(_wrapper.actor, _wrapper.AttackRange, TargetPriority.TargetPriority_Hero, 0u, false);
			if (nearestEnemy != null)
			{
				return nearestEnemy.ObjID;
			}
			nearestEnemy = Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(_wrapper.actor, _wrapper.GreaterRange, TargetPriority.TargetPriority_Hero, 0u, false);
			if (nearestEnemy != null)
			{
				return nearestEnemy.ObjID;
			}
			nearestEnemy = Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(_wrapper.actor, _srchR, 0u, false);
			if (nearestEnemy != null)
			{
				return nearestEnemy.ObjID;
			}
			nearestEnemy = Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(_wrapper.actor, _srchR, 0u, true);
			if (nearestEnemy != null)
			{
				return nearestEnemy.ObjID;
			}
			return 0u;
		}

		public uint CommonAttackSearchLowestHpPriorityHero(ObjWrapper _wrapper, int _srchR)
		{
			return 0u;
		}

		public uint CommonAttackSearchNearestPriorityHero(ObjWrapper _wrapper, int _srchR)
		{
			return 0u;
		}

		public uint CommonAttackSearchLowestHpPriorityMonster(ObjWrapper _wrapper, int _srchR)
		{
			return 0u;
		}

		public uint CommonAttackSearchNearestPriorityMonster(ObjWrapper _wrapper, int _srchR)
		{
			return 0u;
		}
	}
}
