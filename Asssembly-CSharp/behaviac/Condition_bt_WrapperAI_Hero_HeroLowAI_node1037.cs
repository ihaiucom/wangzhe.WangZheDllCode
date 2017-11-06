using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroLowAI_node1037 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus eBTStatus = ((ObjAgent)pAgent).IsPlayOnNetwork();
			EBTStatus eBTStatus2 = EBTStatus.BT_FAILURE;
			return (eBTStatus == eBTStatus2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
