using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Monster_BTMonsterPassiveThief_node20 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint num = (uint)pAgent.GetVariable(1128863647u);
			uint num2 = 0u;
			bool flag = num > num2;
			return (!flag) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS;
		}
	}
}
