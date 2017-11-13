using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroGuideFollow_node149 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			Vector3 restoredHpPos = ((ObjAgent)pAgent).GetRestoredHpPos();
			pAgent.SetVariable<Vector3>("p_restorePos", restoredHpPos, 2613103010u);
			return result;
		}
	}
}
