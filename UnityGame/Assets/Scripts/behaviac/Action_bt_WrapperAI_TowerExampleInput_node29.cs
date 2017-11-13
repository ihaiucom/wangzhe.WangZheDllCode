using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_TowerExampleInput_node29 : Action
	{
		private string method_p0;

		public Action_bt_WrapperAI_TowerExampleInput_node29()
		{
			this.method_p0 = "TowerBornAg";
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			return ((ObjAgent)pAgent).PlayHelperAgeAction(this.method_p0);
		}
	}
}
