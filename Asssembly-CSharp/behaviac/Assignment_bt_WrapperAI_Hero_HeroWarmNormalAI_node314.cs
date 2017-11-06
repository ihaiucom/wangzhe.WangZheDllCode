using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node314 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int attackRange = ((ObjAgent)pAgent).GetAttackRange();
			pAgent.SetVariable<int>("p_attackRange", attackRange, 1643942054u);
			return result;
		}
	}
}
