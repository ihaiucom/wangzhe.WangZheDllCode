using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_TowerExampleInput_node40 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int value = 70;
			pAgent.SetVariable<int>("p_bornWaitFrame", value, 1311805792u);
			return result;
		}
	}
}
