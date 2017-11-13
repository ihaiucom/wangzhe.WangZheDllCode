using Assets.Scripts.GameLogic;
using ResData;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Monster_BTMonsterPassive_zhouwang_node146 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			SkillSlotType inSlot = (SkillSlotType)((int)pAgent.GetVariable(7107675u));
			SkillTargetRule skillTargetRule = ((ObjAgent)pAgent).GetSkillTargetRule(inSlot);
			SkillTargetRule skillTargetRule2 = SkillTargetRule.LowerHpEnermy;
			return (skillTargetRule == skillTargetRule2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
