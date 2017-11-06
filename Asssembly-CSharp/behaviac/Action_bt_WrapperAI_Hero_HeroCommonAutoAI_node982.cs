using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroCommonAutoAI_node982 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int inDanger = (int)pAgent.GetVariable(1456164896u);
			((ObjAgent)pAgent).SetInDanger(inDanger);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
