using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node64 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			ushort num = (ushort)pAgent.GetVariable(957675590u);
			ushort num2 = 5;
			bool flag = num > num2;
			return (!flag) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS;
		}
	}
}
