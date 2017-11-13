using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node379 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			bool value = false;
			pAgent.SetVariable<bool>("p_needGoHome", value, 1364365627u);
			return result;
		}
	}
}
