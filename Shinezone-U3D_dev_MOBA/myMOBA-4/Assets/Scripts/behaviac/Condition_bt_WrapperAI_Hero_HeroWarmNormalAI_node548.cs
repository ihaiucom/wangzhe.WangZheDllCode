using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroWarmNormalAI_node548 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus eBTStatus = ((ObjAgent)pAgent).IsLadder();
			EBTStatus eBTStatus2 = EBTStatus.BT_SUCCESS;
			bool flag = eBTStatus != eBTStatus2;
			return (!flag) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS;
		}
	}
}
