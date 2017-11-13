using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroSimpleAI_node497 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			EBTStatus value = ((ObjAgent)pAgent).IsLowAI();
			pAgent.SetVariable<EBTStatus>("p_LowAI", value, 3095862510u);
			return result;
		}
	}
}
