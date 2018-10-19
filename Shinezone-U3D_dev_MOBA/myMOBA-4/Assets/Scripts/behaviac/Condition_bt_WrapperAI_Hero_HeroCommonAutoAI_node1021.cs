using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroCommonAutoAI_node1021 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint numA = (uint)pAgent.GetVariable(1913039283u);
			int numB = (int)pAgent.GetVariable(2346082870u);
			int mod = ((BTBaseAgent)pAgent).GetMod(numA, numB);
			int num = 0;
			bool flag = mod == num;
			return (!flag) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS;
		}
	}
}
