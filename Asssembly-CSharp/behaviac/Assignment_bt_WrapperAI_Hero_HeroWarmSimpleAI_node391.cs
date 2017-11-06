using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroWarmSimpleAI_node391 : Assignment
	{
		private int opr_p1;

		private int opr_p2;

		private int opr_p3;

		public Assignment_bt_WrapperAI_Hero_HeroWarmSimpleAI_node391()
		{
			this.opr_p1 = 4000;
			this.opr_p2 = 2000;
			this.opr_p3 = 5000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			Vector3 aimPos = (Vector3)pAgent.GetVariable(312907864u);
			Vector3 randomPointByGivenPointAndMinRange = ((ObjAgent)pAgent).GetRandomPointByGivenPointAndMinRange(aimPos, this.opr_p1, this.opr_p2, this.opr_p3);
			pAgent.SetVariable<Vector3>("p_attackPathCurTargetPos", randomPointByGivenPointAndMinRange, 312907864u);
			return result;
		}
	}
}
