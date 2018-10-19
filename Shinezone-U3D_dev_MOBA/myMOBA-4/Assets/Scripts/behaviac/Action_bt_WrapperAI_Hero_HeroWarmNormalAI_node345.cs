using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroWarmNormalAI_node345 : Action
	{
		private SkillAbortType method_p0;

		public Action_bt_WrapperAI_Hero_HeroWarmNormalAI_node345()
		{
			this.method_p0 = SkillAbortType.TYPE_MOVE;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			return ((ObjAgent)pAgent).AbortCurUseSkillByType(this.method_p0);
		}
	}
}
