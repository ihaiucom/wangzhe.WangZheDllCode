using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterPassive_node31 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int pursuitRange = ((ObjAgent)pAgent).GetPursuitRange();
			pAgent.SetVariable<int>("p_pursuitRange", pursuitRange, 53663573u);
			return result;
		}
	}
}
