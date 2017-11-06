using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterInitiative_node94 : Assignment
	{
		private int opr_p1;

		public Assignment_bt_WrapperAI_Monster_BTMonsterInitiative_node94()
		{
			this.opr_p1 = 3000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int srchR = (int)pAgent.GetVariable(1944425156u);
			SkillSlotType inSlot = (SkillSlotType)((int)pAgent.GetVariable(7107675u));
			uint lowHpTeamMember = ((ObjAgent)pAgent).GetLowHpTeamMember(srchR, this.opr_p1, inSlot);
			pAgent.SetVariable<uint>("p_teamTarget", lowHpTeamMember, 1081825808u);
			return result;
		}
	}
}
