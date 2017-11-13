using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node40 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint value = 0u;
			pAgent.SetVariable<uint>("p_abandonTargetID", value, 193046476u);
			return result;
		}
	}
}
