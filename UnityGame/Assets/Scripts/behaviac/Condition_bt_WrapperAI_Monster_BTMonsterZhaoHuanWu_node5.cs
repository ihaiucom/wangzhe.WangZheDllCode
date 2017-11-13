using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Monster_BTMonsterZhaoHuanWu_node5 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint num = (uint)pAgent.GetVariable(1128863647u);
			uint num2 = 0u;
			return (num > num2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
