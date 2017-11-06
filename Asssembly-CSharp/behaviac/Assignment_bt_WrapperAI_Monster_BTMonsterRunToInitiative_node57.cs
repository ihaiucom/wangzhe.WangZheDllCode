using System;
using UnityEngine;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterRunToInitiative_node57 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			Vector3 value = (Vector3)pAgent.GetVariable(312907864u);
			pAgent.SetVariable<Vector3>("p_orignalPos", value, 3727637800u);
			return result;
		}
	}
}
