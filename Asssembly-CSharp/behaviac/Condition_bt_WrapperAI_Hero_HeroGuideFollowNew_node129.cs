using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroGuideFollowNew_node129 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			bool flag = (bool)pAgent.GetVariable(3984982648u);
			bool flag2 = false;
			return (flag == flag2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
