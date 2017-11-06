using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Soldier_BTSoldierPro_node68 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint objID = (uint)pAgent.GetVariable(1081825808u);
			((ObjAgent)pAgent).SelectTarget(objID);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
