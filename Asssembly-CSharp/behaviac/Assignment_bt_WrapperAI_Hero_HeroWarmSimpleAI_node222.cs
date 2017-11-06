using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroWarmSimpleAI_node222 : Assignment
	{
		private int opr_p0;

		public Assignment_bt_WrapperAI_Hero_HeroWarmSimpleAI_node222()
		{
			this.opr_p0 = 5000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			Vector3 nearestShenfuInRange = ((ObjAgent)pAgent).GetNearestShenfuInRange(this.opr_p0);
			pAgent.SetVariable<Vector3>("p_attackPathCurTargetPos", nearestShenfuInRange, 312907864u);
			return result;
		}
	}
}
