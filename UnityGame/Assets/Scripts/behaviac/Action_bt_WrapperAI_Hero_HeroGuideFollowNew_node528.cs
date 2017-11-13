using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroGuideFollowNew_node528 : Action
	{
		private EActType method_p0;

		public Action_bt_WrapperAI_Hero_HeroGuideFollowNew_node528()
		{
			this.method_p0 = EActType.Run;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			((ObjAgent)pAgent).PlayHeroActSound(this.method_p0);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
