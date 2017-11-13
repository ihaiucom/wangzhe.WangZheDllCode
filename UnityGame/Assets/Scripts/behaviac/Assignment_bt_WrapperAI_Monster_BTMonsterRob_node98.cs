using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterRob_node98 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			Vector3 myCurPos = ((ObjAgent)pAgent).GetMyCurPos();
			pAgent.SetVariable<Vector3>("p_orignalPos", myCurPos, 3727637800u);
			return result;
		}
	}
}
