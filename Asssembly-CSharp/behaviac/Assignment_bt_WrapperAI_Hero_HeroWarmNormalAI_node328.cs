using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node328 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			Vector3 routeCurWaypointPosPre = ((ObjAgent)pAgent).GetRouteCurWaypointPosPre();
			pAgent.SetVariable<Vector3>("p_attackPathCurTargetPos", routeCurWaypointPosPre, 312907864u);
			return result;
		}
	}
}
