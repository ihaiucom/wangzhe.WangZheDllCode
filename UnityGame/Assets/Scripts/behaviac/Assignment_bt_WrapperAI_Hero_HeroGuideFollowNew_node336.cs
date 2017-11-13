using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroGuideFollowNew_node336 : Assignment
	{
		private int opr_p0;

		public Assignment_bt_WrapperAI_Hero_HeroGuideFollowNew_node336()
		{
			this.opr_p0 = 10000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint nearestEnemyWithoutNotInBattleJungleMonster = ((ObjAgent)pAgent).GetNearestEnemyWithoutNotInBattleJungleMonster(this.opr_p0);
			pAgent.SetVariable<uint>("p_targetID", nearestEnemyWithoutNotInBattleJungleMonster, 1128863647u);
			return result;
		}
	}
}
