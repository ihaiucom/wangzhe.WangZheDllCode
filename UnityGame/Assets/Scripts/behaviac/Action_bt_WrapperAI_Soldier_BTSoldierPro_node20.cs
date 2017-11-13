using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Soldier_BTSoldierPro_node20 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			SkillSlotType inSlot = (SkillSlotType)((int)pAgent.GetVariable(7107675u));
			return ((ObjAgent)pAgent).RealUseSkill(inSlot);
		}
	}
}
