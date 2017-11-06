using System;

namespace behaviac
{
	internal class Compute_bt_WrapperAI_Soldier_BTSoldierNormal_node20 : Compute
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int num = (int)pAgent.GetVariable(717183702u);
			int num2 = 1;
			int value = num + num2;
			pAgent.SetVariable<int>("p_pursueTime", value, 717183702u);
			return result;
		}
	}
}
