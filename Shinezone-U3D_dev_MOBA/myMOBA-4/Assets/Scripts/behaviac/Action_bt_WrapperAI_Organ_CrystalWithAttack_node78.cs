using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Organ_CrystalWithAttack_node78 : Action
	{
		private int method_p1;

		private bool method_p2;

		public Action_bt_WrapperAI_Organ_CrystalWithAttack_node78()
		{
			this.method_p1 = 60000;
			this.method_p2 = true;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint enemyObjId = (uint)pAgent.GetVariable(317971950u);
			((ObjAgent)pAgent).NotifyTeamAttackEnemy(enemyObjId, this.method_p1, this.method_p2);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
