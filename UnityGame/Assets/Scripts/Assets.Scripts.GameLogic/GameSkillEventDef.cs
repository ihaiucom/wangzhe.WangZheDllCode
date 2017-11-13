using System;

namespace Assets.Scripts.GameLogic
{
	public enum GameSkillEventDef
	{
		AllEvent_BuffChange,
		AllEvent_ChangeSkillCD,
		AllEvent_LimitMove,
		AllEvent_CancelLimitMove,
		AllEvent_Blindess,
		AllEvent_UseSkill,
		AllEvent_SetSkillTimer,
		Event_SkillCDStart,
		Event_SkillCDEnd,
		Event_DisableSkill,
		Event_EnableSkill,
		Event_ChangeSkill,
		Event_RecoverSkill,
		Event_UpdateSkillUI,
		Event_LimitSkill,
		Event_CancelLimitSkill,
		Event_SpawnBuff,
		Event_SelectTarget,
		Event_ClearTarget,
		Event_ProtectDisappear,
		Event_SkillCooldown,
		Enent_EnergyShortage,
		Event_NoSkillTarget,
		Event_UseCanceled,
		Event_LockTarget,
		Event_ClearLockTarget,
		Event_TriggerCountChange,
		Event_ChangeSkillBean,
		Event_SkillBeanShortage,
		Event_SpawnEye,
		Event_Max
	}
}
