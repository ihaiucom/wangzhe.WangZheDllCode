using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroGuideFollowNew_node125 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int baronDeadTimes = ((ObjAgent)pAgent).GetBaronDeadTimes();
			int num = 0;
			return (baronDeadTimes > num) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
