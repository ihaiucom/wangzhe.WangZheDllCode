using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node514 : Assignment
	{
		private TargetPriority opr_p1;

		public Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node514()
		{
			this.opr_p1 = TargetPriority.TargetPriority_Hero;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int srchR = (int)pAgent.GetVariable(2451377514u);
			uint nearestEnemyWithPriorityWithoutNotInBattleJungleMonster = ((ObjAgent)pAgent).GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonster(srchR, this.opr_p1);
			pAgent.SetVariable<uint>("p_tempActorID", nearestEnemyWithPriorityWithoutNotInBattleJungleMonster, 3099920505u);
			return result;
		}
	}
}
