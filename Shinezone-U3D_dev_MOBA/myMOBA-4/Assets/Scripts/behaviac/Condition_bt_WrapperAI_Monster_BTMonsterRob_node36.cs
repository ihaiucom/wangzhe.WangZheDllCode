using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Monster_BTMonsterRob_node36 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int num = (int)pAgent.GetVariable(534149562u);
			int num2 = 4;
			bool flag = num < num2;
			return (!flag) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS;
		}
	}
}
