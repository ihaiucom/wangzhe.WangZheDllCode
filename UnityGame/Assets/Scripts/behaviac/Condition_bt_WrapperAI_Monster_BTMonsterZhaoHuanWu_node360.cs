using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Monster_BTMonsterZhaoHuanWu_node360 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			bool flag = (bool)pAgent.GetVariable(2293816730u);
			bool flag2 = true;
			return (flag == flag2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
