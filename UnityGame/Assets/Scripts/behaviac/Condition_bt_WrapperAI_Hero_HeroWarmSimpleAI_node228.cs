using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroWarmSimpleAI_node228 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			Vector3 par = (Vector3)pAgent.GetVariable(312907864u);
			float vector3Y = ((BTBaseAgent)pAgent).GetVector3Y(par);
			float num = -1000f;
			return (vector3Y > num) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
