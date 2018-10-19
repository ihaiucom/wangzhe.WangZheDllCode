using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_TowerExampleInput_node33 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int myOrganType = ((ObjAgent)pAgent).GetMyOrganType();
			pAgent.SetVariable<int>("p_organType", myOrganType, 1887636950u);
			return result;
		}
	}
}
