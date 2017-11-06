using System;

namespace Assets.Scripts.GameLogic
{
	public enum EGlobalGameEvent
	{
		SpawnGroupDead,
		ActorDead,
		FightPrepare,
		ActorDamage,
		FightStart,
		UseSkill,
		ActorInit,
		OpenTalentTip,
		CloseTalentTip,
		EnterCombat,
		TalentLevelChange,
		BattleGoldChange,
		SkillUseCanceled
	}
}
