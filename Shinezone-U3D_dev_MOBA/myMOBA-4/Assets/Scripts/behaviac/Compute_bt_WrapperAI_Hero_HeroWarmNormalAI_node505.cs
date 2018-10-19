using System;

namespace behaviac
{
	internal class Compute_bt_WrapperAI_Hero_HeroWarmNormalAI_node505 : Compute
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int num = (int)pAgent.GetVariable(274145469u);
			int num2 = 5;
			int value = num * num2;
			pAgent.SetVariable<int>("p_useSkillWeightActually", value, 4066292203u);
			return result;
		}
	}
}
