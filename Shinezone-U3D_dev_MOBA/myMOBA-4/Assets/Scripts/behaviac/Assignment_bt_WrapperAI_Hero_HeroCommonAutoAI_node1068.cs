using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node1068 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int value = (int)pAgent.GetVariable(274145469u);
			pAgent.SetVariable<int>("p_useSkillWeightActually", value, 4066292203u);
			return result;
		}
	}
}
