using System;

namespace behaviac
{
	internal class Compute_bt_WrapperAI_Soldier_BTSoldierPro_node121 : Compute
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			short num = (short)pAgent.GetVariable(957675590u);
			short num2 = 1;
			short value = (short)(num + num2);
			pAgent.SetVariable<short>("p_chooseTargetCount", value, 957675590u);
			return result;
		}
	}
}
