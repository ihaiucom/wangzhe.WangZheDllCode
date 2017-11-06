using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_CrystalBehavior_node5 : Action
	{
		private string method_p0;

		public Action_bt_WrapperAI_CrystalBehavior_node5()
		{
			this.method_p0 = "CrystalBrokenAg";
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			return ((ObjAgent)pAgent).PlayHelperAgeAction(this.method_p0);
		}
	}
}
