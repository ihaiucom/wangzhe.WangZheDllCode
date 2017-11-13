using System;

namespace behaviac
{
	internal class Compute_bt_WrapperAI_Hero_HeroWarmSimpleAI_node398 : Compute
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int num = (int)pAgent.GetVariable(2351850927u);
			int num2 = 26;
			int value = num + num2;
			pAgent.SetVariable<int>("p_waitBornFrame", value, 2351850927u);
			return result;
		}
	}
}
