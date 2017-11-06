using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Monster_BTMonsterRob_node26 : Action
	{
		private ObjBehaviMode method_p0;

		public Action_bt_WrapperAI_Monster_BTMonsterRob_node26()
		{
			this.method_p0 = ObjBehaviMode.State_AutoAI;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			((ObjAgent)pAgent).SetCurBehavior(this.method_p0);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
