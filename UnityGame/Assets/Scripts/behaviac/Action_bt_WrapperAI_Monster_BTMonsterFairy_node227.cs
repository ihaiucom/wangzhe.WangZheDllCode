using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Monster_BTMonsterFairy_node227 : Action
	{
		private string method_p0;

		private float method_p1;

		private int method_p2;

		private bool method_p3;

		public Action_bt_WrapperAI_Monster_BTMonsterFairy_node227()
		{
			this.method_p0 = "Run";
			this.method_p1 = 0.1f;
			this.method_p2 = 0;
			this.method_p3 = true;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			((ObjAgent)pAgent).PlayAnimation(this.method_p0, this.method_p1, this.method_p2, this.method_p3);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
