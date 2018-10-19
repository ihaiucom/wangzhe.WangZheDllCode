using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroRoundAI_node136 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			Vector3 routeCurWaypointPos = ((ObjAgent)pAgent).GetRouteCurWaypointPos();
			pAgent.SetVariable<Vector3>("p_attackPathCurTargetPos", routeCurWaypointPos, 312907864u);
			return result;
		}
	}
}
