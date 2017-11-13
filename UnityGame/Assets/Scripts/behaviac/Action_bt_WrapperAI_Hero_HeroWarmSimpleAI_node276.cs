using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroWarmSimpleAI_node276 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			Vector3 dest = (Vector3)pAgent.GetVariable(1405244869u);
			((ObjAgent)pAgent).RealMoveDirection(dest);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
