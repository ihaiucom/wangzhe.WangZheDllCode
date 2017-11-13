using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node130 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint towerID = (uint)pAgent.GetVariable(3098897795u);
			int value = ((ObjAgent)pAgent).GerPathIndexBuTowerId(towerID);
			pAgent.SetVariable<int>("p_pathIndex", value, 3792555376u);
			return result;
		}
	}
}
