using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroWarmSimpleAI_node344 : Action
	{
		private int method_p0;

		public Action_bt_WrapperAI_Hero_HeroWarmSimpleAI_node344()
		{
			this.method_p0 = 8000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			return ((ObjAgent)pAgent).HasEnemyBuildingAndEnemyBuildingWillAttackSelf(this.method_p0);
		}
	}
}
