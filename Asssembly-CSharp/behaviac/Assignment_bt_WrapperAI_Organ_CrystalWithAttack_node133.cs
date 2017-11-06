using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node133 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int frame = (int)pAgent.GetVariable(775476708u);
			int waitFrame = ((ObjAgent)pAgent).GetWaitFrame(frame);
			pAgent.SetVariable<int>("p_attackTowerWaitFrame", waitFrame, 775476708u);
			return result;
		}
	}
}
