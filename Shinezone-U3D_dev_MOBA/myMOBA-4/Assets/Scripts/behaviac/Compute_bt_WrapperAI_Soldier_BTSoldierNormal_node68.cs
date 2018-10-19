using System;

namespace behaviac
{
	internal class Compute_bt_WrapperAI_Soldier_BTSoldierNormal_node68 : Compute
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			ushort num = (ushort)pAgent.GetVariable(957675590u);
			ushort num2 = 1;
            ushort value = (ushort)(num + num2);
			pAgent.SetVariable<ushort>("p_chooseTargetCount", value, 957675590u);
			return result;
		}
	}
}
