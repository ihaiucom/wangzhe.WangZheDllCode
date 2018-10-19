using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_TowerExampleInput_node83 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int srchR = (int)pAgent.GetVariable(2451377514u);
			uint nearestEnemyDogfaceFirstAndDogfaceHasPriority = ((ObjAgent)pAgent).GetNearestEnemyDogfaceFirstAndDogfaceHasPriority(srchR);
			pAgent.SetVariable<uint>("p_targetID", nearestEnemyDogfaceFirstAndDogfaceHasPriority, 1128863647u);
			return result;
		}
	}
}
