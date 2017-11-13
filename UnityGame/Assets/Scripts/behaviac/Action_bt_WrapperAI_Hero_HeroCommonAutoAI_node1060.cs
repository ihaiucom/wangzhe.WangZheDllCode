using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroCommonAutoAI_node1060 : Action
	{
		public override bool exitaction_impl(Agent pAgent)
		{
			((ObjAgent)pAgent).StopCurAgeAction();
			return true;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			return ((ObjAgent)pAgent).PlayReviveAgeAction();
		}
	}
}
