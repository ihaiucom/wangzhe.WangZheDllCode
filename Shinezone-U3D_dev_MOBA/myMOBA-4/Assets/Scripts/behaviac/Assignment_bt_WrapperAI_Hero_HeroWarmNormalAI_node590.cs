using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node590 : Assignment
	{
		private TargetPriority opr_p1;

		public Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node590()
		{
			this.opr_p1 = TargetPriority.TargetPriority_Hero;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int srchR = (int)pAgent.GetVariable(2451377514u);
			uint withOutActor = (uint)pAgent.GetVariable(193046476u);
			uint nearestEnemyWithPriorityWithoutNotInBattleJungleMonsterWithoutActor = ((ObjAgent)pAgent).GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonsterWithoutActor(srchR, this.opr_p1, withOutActor);
			pAgent.SetVariable<uint>("p_targetID", nearestEnemyWithPriorityWithoutNotInBattleJungleMonsterWithoutActor, 1128863647u);
			return result;
		}
	}
}
