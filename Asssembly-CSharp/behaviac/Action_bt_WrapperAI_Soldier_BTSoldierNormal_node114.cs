using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Soldier_BTSoldierNormal_node114 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int range = (int)pAgent.GetVariable(2451377514u);
			return ((ObjAgent)pAgent).MoveToSkillTargetWithRange(range);
		}
	}
}
