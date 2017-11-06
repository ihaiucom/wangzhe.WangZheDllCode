using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroCommonAutoAI_node256 : Action
	{
		private int method_p0;

		public Action_bt_WrapperAI_Hero_HeroCommonAutoAI_node256()
		{
			this.method_p0 = 7000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			((ObjAgent)pAgent).NotifySelfCampSelfBeAttacked(this.method_p0);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
