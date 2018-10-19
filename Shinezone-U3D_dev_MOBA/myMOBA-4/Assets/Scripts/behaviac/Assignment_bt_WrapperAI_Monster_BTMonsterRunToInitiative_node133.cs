using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterRunToInitiative_node133 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int srchR = (int)pAgent.GetVariable(2451377514u);
			uint nearestEnemy = ((ObjAgent)pAgent).GetNearestEnemy(srchR);
			pAgent.SetVariable<uint>("p_targetID", nearestEnemy, 1128863647u);
			return result;
		}
	}
}
