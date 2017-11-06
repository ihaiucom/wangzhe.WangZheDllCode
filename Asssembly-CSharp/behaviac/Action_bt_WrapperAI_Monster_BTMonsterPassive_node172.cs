using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Monster_BTMonsterPassive_node172 : Action
	{
		private bool method_p0;

		public Action_bt_WrapperAI_Monster_BTMonsterPassive_node172()
		{
			this.method_p0 = false;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			((ObjAgent)pAgent).SetMonsterEnduranceDown(this.method_p0);
			return EBTStatus.BT_FAILURE;
		}
	}
}
