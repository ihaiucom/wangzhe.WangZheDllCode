using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterBaozou_node22 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int index = (int)pAgent.GetVariable(4088106663u);
			Vector3 polygonEdgePoint = ((ObjAgent)pAgent).GetPolygonEdgePoint(index);
			pAgent.SetVariable<Vector3>("p_targetPos", polygonEdgePoint, 282422753u);
			return result;
		}
	}
}
