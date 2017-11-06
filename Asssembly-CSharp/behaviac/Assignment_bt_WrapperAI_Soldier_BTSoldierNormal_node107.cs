using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node107 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int srchR = (int)pAgent.GetVariable(2451377514u);
			uint withOutActor = (uint)pAgent.GetVariable(193046476u);
			uint nearestEnemyWithoutJungleMonsterAndCallActorWithoutActor = ((ObjAgent)pAgent).GetNearestEnemyWithoutJungleMonsterAndCallActorWithoutActor(srchR, withOutActor);
			pAgent.SetVariable<uint>("p_targetID", nearestEnemyWithoutJungleMonsterAndCallActorWithoutActor, 1128863647u);
			return result;
		}
	}
}
