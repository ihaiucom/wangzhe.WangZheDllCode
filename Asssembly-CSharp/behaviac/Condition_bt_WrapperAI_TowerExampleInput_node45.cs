using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_TowerExampleInput_node45 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int num = (int)pAgent.GetVariable(1887636950u);
			int num2 = 1;
			return (num == num2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
