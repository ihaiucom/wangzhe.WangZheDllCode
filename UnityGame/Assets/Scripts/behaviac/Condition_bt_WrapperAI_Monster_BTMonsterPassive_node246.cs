using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Monster_BTMonsterPassive_node246 : Condition
	{
		private int opl_p1;

		public Condition_bt_WrapperAI_Monster_BTMonsterPassive_node246()
		{
			this.opl_p1 = 600;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			Vector3 aimPos = (Vector3)pAgent.GetVariable(3727637800u);
			bool flag = ((ObjAgent)pAgent).IsDistanceToPosLessThanRange(aimPos, this.opl_p1);
			bool flag2 = true;
			return (flag == flag2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
