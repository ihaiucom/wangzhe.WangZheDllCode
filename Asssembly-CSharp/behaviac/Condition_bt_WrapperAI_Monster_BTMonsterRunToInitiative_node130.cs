using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Monster_BTMonsterRunToInitiative_node130 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			Vector3 aimPos = (Vector3)pAgent.GetVariable(3727637800u);
			int range = (int)pAgent.GetVariable(53663573u);
			bool flag = ((ObjAgent)pAgent).IsDistanceToPosLessThanRange(aimPos, range);
			bool flag2 = false;
			return (flag == flag2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
