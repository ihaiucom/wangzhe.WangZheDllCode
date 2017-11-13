using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node1069 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int value = 2;
			pAgent.SetVariable<int>("p_useSkillWeightActually", value, 4066292203u);
			return result;
		}
	}
}
