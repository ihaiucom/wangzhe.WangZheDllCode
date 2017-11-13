using System;

namespace behaviac
{
	internal class Compute_bt_WrapperAI_Hero_HeroGuideFollow_node136 : Compute
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int num = (int)pAgent.GetVariable(3589669257u);
			int num2 = 1;
			int value = num + num2;
			pAgent.SetVariable<int>("p_waitFrame", value, 3589669257u);
			return result;
		}
	}
}
