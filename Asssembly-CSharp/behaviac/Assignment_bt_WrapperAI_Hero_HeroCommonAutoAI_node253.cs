using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node253 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint value = 0u;
			pAgent.SetVariable<uint>("p_idleShowFrame", value, 1913039283u);
			return result;
		}
	}
}
