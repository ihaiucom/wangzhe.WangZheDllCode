using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterBossPassive_node33 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int value = 0;
			pAgent.SetVariable<int>("p_attackOneTime", value, 534149562u);
			return result;
		}
	}
}
