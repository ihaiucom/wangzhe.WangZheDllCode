using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Monster_BTMonsterBossPassive_node180 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			SkillSlotType skillSlotType = (SkillSlotType)((int)pAgent.GetVariable(3371127896u));
			SkillSlotType skillSlotType2 = SkillSlotType.SLOT_SKILL_VALID;
			bool flag = skillSlotType != skillSlotType2;
			return (!flag) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS;
		}
	}
}
