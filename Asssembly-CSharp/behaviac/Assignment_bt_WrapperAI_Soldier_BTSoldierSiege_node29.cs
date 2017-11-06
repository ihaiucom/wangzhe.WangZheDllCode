using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Soldier_BTSoldierSiege_node29 : Assignment
	{
		private TargetPriority opr_p1;

		public Assignment_bt_WrapperAI_Soldier_BTSoldierSiege_node29()
		{
			this.opr_p1 = TargetPriority.TargetPriority_Organ;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int srchR = (int)pAgent.GetVariable(2451377514u);
			uint nearestEnemyWithPriorityWithoutNotInBattleJungleMonster = ((ObjAgent)pAgent).GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonster(srchR, this.opr_p1);
			pAgent.SetVariable<uint>("p_targetID", nearestEnemyWithPriorityWithoutNotInBattleJungleMonster, 1128863647u);
			return result;
		}
	}
}
