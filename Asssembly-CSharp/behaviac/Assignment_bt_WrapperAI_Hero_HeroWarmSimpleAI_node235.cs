using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroWarmSimpleAI_node235 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			Vector3 curRouteLastForward = ((ObjAgent)pAgent).GetCurRouteLastForward();
			pAgent.SetVariable<Vector3>("p_direction", curRouteLastForward, 3207227154u);
			return result;
		}
	}
}
