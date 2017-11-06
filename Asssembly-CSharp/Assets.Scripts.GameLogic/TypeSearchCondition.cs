using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class TypeSearchCondition
	{
		public static bool Fit(ActorRoot _actor, ActorTypeDef _actorType)
		{
			return _actor.TheActorMeta.ActorType == _actorType;
		}

		public static bool Fit(ActorRoot _actor, ActorTypeDef _actorType, bool isBoss)
		{
			return _actor.TheActorMeta.ActorType == _actorType && _actor.ActorControl.IsBossOrHeroAutoAI() == isBoss;
		}

		public static bool FitWithJungleMonsterNotInBattle(ActorRoot _actor, bool bWithMonsterNotInBattle)
		{
			if (_actor.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Monster)
			{
				return false;
			}
			if (bWithMonsterNotInBattle)
			{
				return true;
			}
			MonsterWrapper monsterWrapper = _actor.ActorControl as MonsterWrapper;
			if (monsterWrapper != null)
			{
				ResMonsterCfgInfo cfgInfo = monsterWrapper.cfgInfo;
				if (cfgInfo != null && cfgInfo.bMonsterType == 2)
				{
					ObjBehaviMode myBehavior = monsterWrapper.myBehavior;
					if (myBehavior == ObjBehaviMode.State_Idle || myBehavior == ObjBehaviMode.State_Dead || myBehavior == ObjBehaviMode.State_Null)
					{
						return false;
					}
				}
			}
			return true;
		}

		public static bool FitSoldier(ActorRoot _actor)
		{
			if (_actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
			{
				MonsterWrapper monsterWrapper = _actor.ActorControl as MonsterWrapper;
				if (monsterWrapper != null)
				{
					ResMonsterCfgInfo cfgInfo = monsterWrapper.cfgInfo;
					if (cfgInfo != null && cfgInfo.bMonsterType == 1)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
