using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node100 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int frame = (int)pAgent.GetVariable(3589669257u);
			int waitFrame = ((ObjAgent)pAgent).GetWaitFrame(frame);
			pAgent.SetVariable<int>("p_waitFrame", waitFrame, 3589669257u);
			return result;
		}
	}
}
