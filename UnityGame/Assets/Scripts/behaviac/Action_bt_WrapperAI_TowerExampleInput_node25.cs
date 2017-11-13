using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_TowerExampleInput_node25 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			((ObjAgent)pAgent).ClearTarget();
			return EBTStatus.BT_SUCCESS;
		}
	}
}
