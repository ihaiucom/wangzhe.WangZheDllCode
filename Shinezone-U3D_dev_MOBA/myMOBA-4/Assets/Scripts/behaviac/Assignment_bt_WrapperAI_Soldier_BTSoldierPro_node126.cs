using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Soldier_BTSoldierPro_node126 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			short value = 0;
			pAgent.SetVariable<short>("p_chooseTargetCount", value, 957675590u);
			return result;
		}
	}
}
