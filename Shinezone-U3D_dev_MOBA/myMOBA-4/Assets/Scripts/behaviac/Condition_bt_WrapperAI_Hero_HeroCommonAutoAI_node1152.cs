using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroCommonAutoAI_node1152 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			ObjDeadMode actorDeadState = ((ObjAgent)pAgent).GetActorDeadState();
			ObjDeadMode objDeadMode = ObjDeadMode.DeadState_Normal;
			bool flag = actorDeadState == objDeadMode;
			return (!flag) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS;
		}
	}
}
