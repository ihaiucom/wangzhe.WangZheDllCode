using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_TowerExampleInput_node16 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			((ObjAgent)pAgent).HelpToAttack();
			return EBTStatus.BT_SUCCESS;
		}
	}
}
