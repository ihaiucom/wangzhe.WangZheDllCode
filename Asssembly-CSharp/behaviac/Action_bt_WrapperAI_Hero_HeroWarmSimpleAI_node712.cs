using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroWarmSimpleAI_node712 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			SkillSlotType inSlot = (SkillSlotType)((int)pAgent.GetVariable(7107675u));
			uint objID = (uint)pAgent.GetVariable(1128863647u);
			return ((ObjAgent)pAgent).CheckSkillFilter(inSlot, objID);
		}
	}
}
