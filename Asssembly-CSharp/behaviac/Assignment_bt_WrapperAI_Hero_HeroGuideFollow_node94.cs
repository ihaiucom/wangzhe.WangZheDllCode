using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroGuideFollow_node94 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			bool value = true;
			pAgent.SetVariable<bool>("p_dragonHasBeKilled", value, 1129250041u);
			return result;
		}
	}
}
