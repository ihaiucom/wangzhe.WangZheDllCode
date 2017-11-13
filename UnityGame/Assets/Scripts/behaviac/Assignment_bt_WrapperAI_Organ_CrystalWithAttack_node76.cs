using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node76 : Assignment
	{
		private int opr_p0;

		private TargetPriority opr_p1;

		public Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node76()
		{
			this.opr_p0 = 35000;
			this.opr_p1 = TargetPriority.TargetPriority_Hero;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint nearestEnemyWithPriorityWithoutJungleMonster = ((ObjAgent)pAgent).GetNearestEnemyWithPriorityWithoutJungleMonster(this.opr_p0, this.opr_p1);
			pAgent.SetVariable<uint>("p_nearCommanTarget", nearestEnemyWithPriorityWithoutJungleMonster, 317971950u);
			return result;
		}
	}
}
