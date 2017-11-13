using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroWarmNormalAI_node489 : Action
	{
		private int method_p1;

		public Action_bt_WrapperAI_Hero_HeroWarmNormalAI_node489()
		{
			this.method_p1 = 9000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint targetHeroId = (uint)pAgent.GetVariable(1128863647u);
			return ((ObjAgent)pAgent).IsDangerUnderEnemyBuilding(targetHeroId, this.method_p1);
		}
	}
}
