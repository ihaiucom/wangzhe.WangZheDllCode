using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Soldier_BTSoldierPro_node123 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			short num = (short)pAgent.GetVariable(957675590u);
			short num2 = 5;
			return (num > num2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
