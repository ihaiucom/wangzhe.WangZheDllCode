using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroCommonAutoAI_node485 : Action
	{
		private int method_p0;

		private int method_p1;

		public Action_bt_WrapperAI_Hero_HeroCommonAutoAI_node485()
		{
			this.method_p0 = 12000;
			this.method_p1 = 8500;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			return ((ObjAgent)pAgent).IsAroundTeamThanStrongThanEnemise(this.method_p0, this.method_p1);
		}
	}
}
