using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroCommonAutoAI_node73 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int range = (int)pAgent.GetVariable(2451377514u);
			return ((ObjAgent)pAgent).MoveToSkillTargetWithRange(range);
		}
	}
}
