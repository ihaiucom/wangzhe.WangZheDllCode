using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node740 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			SkillSlotType curSkillSlotType = ((ObjAgent)pAgent).GetCurSkillSlotType();
			pAgent.SetVariable<SkillSlotType>("p_curSlotType", curSkillSlotType, 7107675u);
			return result;
		}
	}
}
