using System;

namespace ResData
{
	public enum PassiveConditionType
	{
		NoDamagePassiveCondition = 1,
		DamagePassiveCondition,
		BeKilledPassiveCondition,
		KilledPassiveCondition,
		HpPassiveCondition,
		ExitBattlePassiveCondition,
		AssistPassiveCondition,
		FightStartCondition,
		ActorReviveCondition,
		ActorCritCondition,
		ActorUpgradeCondition,
		UseSkillCondition,
		ImmuneDeadHurtCondition,
		LimitMoveCondition,
		EnterBattlePassiveCondition,
		RemoveBuffPassiveCondition,
		ActorInGrassCondition,
		ExposeCondition
	}
}
