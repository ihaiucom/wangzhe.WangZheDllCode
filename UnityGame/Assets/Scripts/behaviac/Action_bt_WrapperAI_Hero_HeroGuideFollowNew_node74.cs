using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroGuideFollowNew_node74 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			SkillSlotType skill = (SkillSlotType)((int)pAgent.GetVariable(7107675u));
			return ((ObjAgent)pAgent).SetSkill(skill);
		}
	}
}
