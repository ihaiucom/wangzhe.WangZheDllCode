using System;

namespace Assets.Scripts.GameLogic
{
	public enum ObjBehaviMode
	{
		State_Idle,
		State_Dead,
		Direction_Move,
		Destination_Move,
		Normal_Attack,
		Attack_Move,
		Attack_Path,
		Attack_Lock,
		UseSkill_0,
		UseSkill_1,
		UseSkill_2,
		UseSkill_3,
		UseSkill_4,
		UseSkill_5,
		UseSkill_6,
		UseSkill_7,
		UseSkill_9,
		UseSkill_10,
		State_AutoAI,
		State_GameOver,
		State_OutOfControl,
		State_Born,
		State_Revive,
		State_Null
	}
}
