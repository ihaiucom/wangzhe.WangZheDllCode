using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroGuideFollow_node158 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int value = 0;
			pAgent.SetVariable<int>("p_waitFrame", value, 3589669257u);
			return result;
		}
	}
}
