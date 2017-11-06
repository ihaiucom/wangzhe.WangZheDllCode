using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroSimpleAI_node590 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int srchR = (int)pAgent.GetVariable(2451377514u);
			uint withOutActor = (uint)pAgent.GetVariable(193046476u);
			uint nearestEnemyWithoutNotInBattleJungleMonsterWithoutActor = ((ObjAgent)pAgent).GetNearestEnemyWithoutNotInBattleJungleMonsterWithoutActor(srchR, withOutActor);
			pAgent.SetVariable<uint>("p_targetID", nearestEnemyWithoutNotInBattleJungleMonsterWithoutActor, 1128863647u);
			return result;
		}
	}
}
