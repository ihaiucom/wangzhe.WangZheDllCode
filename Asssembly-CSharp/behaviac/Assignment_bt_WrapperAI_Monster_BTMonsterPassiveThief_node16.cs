using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterPassiveThief_node16 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint objID = (uint)pAgent.GetVariable(1128863647u);
			Vector3 randomFarPoint = ((ObjAgent)pAgent).GetRandomFarPoint(objID);
			pAgent.SetVariable<Vector3>("p_randomPos", randomFarPoint, 3411867082u);
			return result;
		}
	}
}
