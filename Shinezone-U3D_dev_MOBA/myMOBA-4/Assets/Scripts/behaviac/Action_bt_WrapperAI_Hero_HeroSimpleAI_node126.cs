using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroSimpleAI_node126 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint objID = (uint)pAgent.GetVariable(1081825808u);
			((ObjAgent)pAgent).SelectTarget(objID);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
