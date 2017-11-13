using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_TowerExampleInput_node47 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int frame = (int)pAgent.GetVariable(1311805792u);
			int waitFrame = ((ObjAgent)pAgent).GetWaitFrame(frame);
			pAgent.SetVariable<int>("p_bornWaitFrame", waitFrame, 1311805792u);
			return result;
		}
	}
}
