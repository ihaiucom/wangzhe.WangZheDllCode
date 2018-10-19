using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroWarmSimpleAI_node594 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint teamMemberTarget = ((ObjAgent)pAgent).GetTeamMemberTarget();
			pAgent.SetVariable<uint>("p_targetID", teamMemberTarget, 1128863647u);
			return result;
		}
	}
}
