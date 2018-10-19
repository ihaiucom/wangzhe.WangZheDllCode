using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterBossPassive_node135 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			SkillSlotType inSlot = (SkillSlotType)((int)pAgent.GetVariable(3371127896u));
			int skillAttackRange = ((ObjAgent)pAgent).GetSkillAttackRange(inSlot);
			pAgent.SetVariable<int>("p_skillAttackRange", skillAttackRange, 1944425156u);
			return result;
		}
	}
}
