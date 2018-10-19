using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterRob_node133 : Assignment
	{
		private TargetPriority opr_p1;

		public Assignment_bt_WrapperAI_Monster_BTMonsterRob_node133()
		{
			this.opr_p1 = TargetPriority.TargetPriority_Monster;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int srchR = (int)pAgent.GetVariable(2451377514u);
			uint nearestEnemyWithTargetPriority = ((ObjAgent)pAgent).GetNearestEnemyWithTargetPriority(srchR, this.opr_p1);
			pAgent.SetVariable<uint>("p_targetID", nearestEnemyWithTargetPriority, 1128863647u);
			return result;
		}
	}
}
