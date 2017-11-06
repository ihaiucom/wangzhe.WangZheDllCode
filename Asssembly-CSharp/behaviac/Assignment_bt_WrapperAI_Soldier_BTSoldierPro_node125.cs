using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Soldier_BTSoldierPro_node125 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint objID = (uint)pAgent.GetVariable(1128863647u);
			int srchR = (int)pAgent.GetVariable(2451377514u);
			uint newTargetByPriority = ((ObjAgent)pAgent).GetNewTargetByPriority(objID, srchR);
			pAgent.SetVariable<uint>("p_tempTargetId", newTargetByPriority, 2303639248u);
			return result;
		}
	}
}
