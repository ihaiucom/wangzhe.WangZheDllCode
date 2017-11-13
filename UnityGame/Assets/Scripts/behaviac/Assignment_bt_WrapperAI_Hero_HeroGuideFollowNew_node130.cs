using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroGuideFollowNew_node130 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			bool value = true;
			pAgent.SetVariable<bool>("p_followSoldierLine", value, 3984982648u);
			return result;
		}
	}
}
