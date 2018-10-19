using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Organ_CrystalWithAttack_node116 : Action
	{
		private int method_p1;

		private bool method_p2;

		public Action_bt_WrapperAI_Organ_CrystalWithAttack_node116()
		{
			this.method_p1 = 200000;
			this.method_p2 = true;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint enemyObjId = (uint)pAgent.GetVariable(3098897795u);
			((ObjAgent)pAgent).NotifyTeamAttackEnemy(enemyObjId, this.method_p1, this.method_p2);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
