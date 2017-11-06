using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterPassive_node20 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			Vector3 myForward = ((ObjAgent)pAgent).GetMyForward();
			pAgent.SetVariable<Vector3>("p_orignalDirection", myForward, 3447928192u);
			return result;
		}
	}
}
