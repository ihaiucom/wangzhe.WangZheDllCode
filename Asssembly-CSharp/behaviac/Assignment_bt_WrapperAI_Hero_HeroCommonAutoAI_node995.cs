using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node995 : Assignment
	{
		private int opr_p1;

		public Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node995()
		{
			this.opr_p1 = 3000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			Vector3 aimPos = (Vector3)pAgent.GetVariable(312907864u);
			Vector3 randomPointAroundGivenPoint = ((ObjAgent)pAgent).GetRandomPointAroundGivenPoint(aimPos, this.opr_p1);
			pAgent.SetVariable<Vector3>("p_attackPathCurTargetPos", randomPointAroundGivenPoint, 312907864u);
			return result;
		}
	}
}
