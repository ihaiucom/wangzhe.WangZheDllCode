using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroSimpleAI_node500 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus eBTStatus = (EBTStatus)((int)pAgent.GetVariable(3095862510u));
			EBTStatus eBTStatus2 = EBTStatus.BT_SUCCESS;
			bool flag = eBTStatus == eBTStatus2;
			return (!flag) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS;
		}
	}
}
