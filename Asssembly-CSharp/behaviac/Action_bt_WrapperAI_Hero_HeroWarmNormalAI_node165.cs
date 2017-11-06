using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroWarmNormalAI_node165 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int range = (int)pAgent.GetVariable(750283102u);
			return ((ObjAgent)pAgent).HasEnemyInRange(range);
		}
	}
}
