using System;

namespace behaviac
{
	internal class Compute_bt_WrapperAI_Monster_BTMonsterInitiative_node39 : Compute
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int num = (int)pAgent.GetVariable(534149562u);
			int num2 = 1;
			int value = num + num2;
			pAgent.SetVariable<int>("p_attackOneTime", value, 534149562u);
			return result;
		}
	}
}
