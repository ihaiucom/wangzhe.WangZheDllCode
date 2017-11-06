using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroGuideFollow_node138 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int num = (int)pAgent.GetVariable(3589669257u);
			int num2 = 50;
			return (num > num2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
