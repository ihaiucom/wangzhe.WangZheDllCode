using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node26 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int value = 0;
			pAgent.SetVariable<int>("p_pursueTime", value, 717183702u);
			return result;
		}
	}
}
