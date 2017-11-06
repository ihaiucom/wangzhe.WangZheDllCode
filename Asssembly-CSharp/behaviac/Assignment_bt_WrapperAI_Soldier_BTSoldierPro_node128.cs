using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Soldier_BTSoldierPro_node128 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint value = (uint)pAgent.GetVariable(2303639248u);
			pAgent.SetVariable<uint>("p_targetID", value, 1128863647u);
			return result;
		}
	}
}
