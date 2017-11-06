using System;

namespace behaviac
{
	internal class Compute_bt_WrapperAI_Hero_HeroWarmSimpleAI_node575 : Compute
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int num = (int)pAgent.GetVariable(3103756267u);
			int num2 = 1;
			int value = num + num2;
			pAgent.SetVariable<int>("p_pursueFrame", value, 3103756267u);
			return result;
		}
	}
}
