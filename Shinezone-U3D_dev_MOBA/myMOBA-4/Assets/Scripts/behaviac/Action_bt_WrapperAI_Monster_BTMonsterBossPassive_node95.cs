using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Monster_BTMonsterBossPassive_node95 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint objID = (uint)pAgent.GetVariable(1028319457u);
			((ObjAgent)pAgent).LeaveActor(objID);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
