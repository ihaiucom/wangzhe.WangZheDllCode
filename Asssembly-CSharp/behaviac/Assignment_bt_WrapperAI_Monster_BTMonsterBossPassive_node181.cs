using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterBossPassive_node181 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			SkillSlotType mustUseNextSkillSlotType = ((ObjAgent)pAgent).GetMustUseNextSkillSlotType();
			pAgent.SetVariable<SkillSlotType>("p_nextSkill", mustUseNextSkillSlotType, 3371127896u);
			return result;
		}
	}
}
