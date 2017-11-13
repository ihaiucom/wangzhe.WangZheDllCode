using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Monster_BTMonsterBossPassive_node132 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			SkillSlotType inSlot = (SkillSlotType)((int)pAgent.GetVariable(3371127896u));
			return ((ObjAgent)pAgent).CanUseSkill(inSlot);
		}
	}
}
