using System;

namespace behaviac
{
	internal class Compute_bt_WrapperAI_Hero_HeroCommonAutoAI_node1022 : Compute
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint num = (uint)pAgent.GetVariable(1913039283u);
			uint num2 = 1u;
			uint value = num + num2;
			pAgent.SetVariable<uint>("p_idleShowFrame", value, 1913039283u);
			return result;
		}
	}
}
