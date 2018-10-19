using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroGuideFollow_node409 : Action
	{
		private int method_p1;

		public Action_bt_WrapperAI_Hero_HeroGuideFollow_node409()
		{
			this.method_p1 = 2;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint objID = (uint)pAgent.GetVariable(2120643215u);
			((ObjAgent)pAgent).RealMoveToActorRight(objID, this.method_p1);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
