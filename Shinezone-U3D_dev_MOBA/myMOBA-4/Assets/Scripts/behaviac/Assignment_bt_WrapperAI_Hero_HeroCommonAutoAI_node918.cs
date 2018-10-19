using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node918 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint value = (uint)pAgent.GetVariable(1049827932u);
			pAgent.SetVariable<uint>("p_baseID", value, 3990708623u);
			return result;
		}
	}
}
