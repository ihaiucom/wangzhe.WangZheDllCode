using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroLowAI_node187 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint value = (uint)pAgent.GetVariable(1128863647u);
			pAgent.SetVariable<uint>("p_abandonTargetID", value, 193046476u);
			return result;
		}
	}
}
