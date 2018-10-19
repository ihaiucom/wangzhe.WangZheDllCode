using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node58 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint num = (uint)pAgent.GetVariable(2303639248u);
			uint num2 = (uint)pAgent.GetVariable(1128863647u);
			bool flag = num != num2;
			return (!flag) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS;
		}
	}
}
