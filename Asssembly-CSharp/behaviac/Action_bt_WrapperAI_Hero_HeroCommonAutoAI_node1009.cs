using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroCommonAutoAI_node1009 : Action
	{
		private bool method_p0;

		public Action_bt_WrapperAI_Hero_HeroCommonAutoAI_node1009()
		{
			this.method_p0 = false;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			((ObjAgent)pAgent).SetIsAttackByEnemyHero(this.method_p0);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
