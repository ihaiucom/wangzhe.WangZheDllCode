using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroWarmNormalAI_node358 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int num = (int)pAgent.GetVariable(3012176472u);
			int num2 = (int)pAgent.GetVariable(3421255u);
			return (num < num2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
