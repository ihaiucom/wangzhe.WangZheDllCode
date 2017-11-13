using System;

namespace behaviac
{
	internal class Compute_bt_WrapperAI_Hero_HeroCommonAutoAI_node1030 : Compute
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int num = (int)pAgent.GetVariable(1460711631u);
			int num2 = 1;
			int value = num - num2;
			pAgent.SetVariable<int>("p_idleShowLast", value, 1460711631u);
			return result;
		}
	}
}
