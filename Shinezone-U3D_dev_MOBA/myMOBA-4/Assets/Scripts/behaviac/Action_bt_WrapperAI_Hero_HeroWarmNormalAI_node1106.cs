using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroWarmNormalAI_node1106 : Action
	{
		private int method_p0;

		public Action_bt_WrapperAI_Hero_HeroWarmNormalAI_node1106()
		{
			this.method_p0 = 12000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			((ObjAgent)pAgent).NotifySelfCampSelfWillAttack(this.method_p0);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
