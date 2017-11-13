using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Monster_BTMonsterPassive_node130 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			Vector3 aimPos = (Vector3)pAgent.GetVariable(3727637800u);
			bool flag = ((ObjAgent)pAgent).IsOutOfPursuitRange(aimPos);
			bool flag2 = true;
			return (flag == flag2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
