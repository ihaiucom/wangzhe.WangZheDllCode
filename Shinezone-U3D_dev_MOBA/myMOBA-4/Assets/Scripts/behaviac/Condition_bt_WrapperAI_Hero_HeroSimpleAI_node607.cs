using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroSimpleAI_node607 : Condition
	{
		private int opl_p1;

		public Condition_bt_WrapperAI_Hero_HeroSimpleAI_node607()
		{
			this.opl_p1 = 15000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			Vector3 aimPos = (Vector3)pAgent.GetVariable(2613103010u);
			bool flag = ((ObjAgent)pAgent).IsDistanceToPosMoreThanRange(aimPos, this.opl_p1);
			bool flag2 = true;
			bool flag3 = flag == flag2;
			return (!flag3) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS;
		}
	}
}
