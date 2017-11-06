using System;
using UnityEngine;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node34 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			Vector3 lhs = (Vector3)pAgent.GetVariable(1602776625u);
			Vector3 rhs = (Vector3)pAgent.GetVariable(1487184177u);
			return (lhs == rhs) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
