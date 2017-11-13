using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Monster_BTMonsterPassive_zhouwang_node21 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			Vector3 dest = (Vector3)pAgent.GetVariable(3447928192u);
			((ObjAgent)pAgent).LookAtDirection(dest);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
