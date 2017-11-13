using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node4 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int num = (int)pAgent.GetVariable(717183702u);
			int num2 = 10;
			return (num < num2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
