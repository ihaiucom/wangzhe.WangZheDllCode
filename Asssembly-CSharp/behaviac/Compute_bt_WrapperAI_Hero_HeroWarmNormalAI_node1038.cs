using System;

namespace behaviac
{
	internal class Compute_bt_WrapperAI_Hero_HeroWarmNormalAI_node1038 : Compute
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int num = (int)pAgent.GetVariable(20770612u);
			int num2 = 2;
			int value = num * num2;
			pAgent.SetVariable<int>("p_waitToPlayBornAge", value, 20770612u);
			return result;
		}
	}
}
