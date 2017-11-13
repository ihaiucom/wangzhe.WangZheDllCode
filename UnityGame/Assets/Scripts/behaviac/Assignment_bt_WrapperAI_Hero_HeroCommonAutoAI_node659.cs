using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node659 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int value = 30000;
			pAgent.SetVariable<int>("p_srchRange", value, 2451377514u);
			return result;
		}
	}
}
