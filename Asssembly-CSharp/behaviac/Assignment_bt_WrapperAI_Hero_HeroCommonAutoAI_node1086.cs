using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node1086 : Assignment
	{
		private int opr_p0;

		private TargetPriority opr_p1;

		public Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node1086()
		{
			this.opr_p0 = 12000;
			this.opr_p1 = TargetPriority.TargetPriority_Hero;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint withOutActor = (uint)pAgent.GetVariable(193046476u);
			uint nearestEnemyWithPriorityWithoutNotInBattleJungleMonsterWithoutActor = ((ObjAgent)pAgent).GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonsterWithoutActor(this.opr_p0, this.opr_p1, withOutActor);
			pAgent.SetVariable<uint>("p_targetID", nearestEnemyWithPriorityWithoutNotInBattleJungleMonsterWithoutActor, 1128863647u);
			return result;
		}
	}
}
