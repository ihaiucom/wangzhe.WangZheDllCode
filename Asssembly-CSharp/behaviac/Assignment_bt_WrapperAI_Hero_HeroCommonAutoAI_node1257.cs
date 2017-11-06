using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node1257 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint value = 0u;
			pAgent.SetVariable<uint>("p_captainTargetID", value, 2561624163u);
			return result;
		}
	}
}
