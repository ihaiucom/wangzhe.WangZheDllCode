using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroCommonAutoAI_node980 : Action
	{
		private int method_p1;

		public Action_bt_WrapperAI_Hero_HeroCommonAutoAI_node980()
		{
			this.method_p1 = 9000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int srchR = (int)pAgent.GetVariable(2451377514u);
			return ((ObjAgent)pAgent).IsAroundTeamThanStrongThanEnemise(srchR, this.method_p1);
		}
	}
}
