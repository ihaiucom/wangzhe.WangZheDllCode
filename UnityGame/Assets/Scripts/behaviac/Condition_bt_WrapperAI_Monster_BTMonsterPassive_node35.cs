using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Monster_BTMonsterPassive_node35 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int num = (int)pAgent.GetVariable(534149562u);
			int num2 = 4;
			return (num < num2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
