using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroWarmNormalAI_node468 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			SkillSlotType skillSlotType = (SkillSlotType)((int)pAgent.GetVariable(7107675u));
			SkillSlotType skillSlotType2 = SkillSlotType.SLOT_SKILL_0;
			return (skillSlotType == skillSlotType2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
