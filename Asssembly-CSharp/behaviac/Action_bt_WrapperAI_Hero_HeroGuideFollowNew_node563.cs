using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroGuideFollowNew_node563 : Action
	{
		private int method_p0;

		public Action_bt_WrapperAI_Hero_HeroGuideFollowNew_node563()
		{
			this.method_p0 = 11000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			return ((ObjAgent)pAgent).HasEnemyInRange(this.method_p0);
		}
	}
}
