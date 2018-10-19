using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node67 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			ushort value = 0;
			pAgent.SetVariable<ushort>("p_chooseTargetCount", value, 957675590u);
			return result;
		}
	}
}
