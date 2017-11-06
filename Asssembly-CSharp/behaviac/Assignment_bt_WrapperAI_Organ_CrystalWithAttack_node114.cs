using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node114 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint bigDragon = ((ObjAgent)pAgent).GetBigDragon();
			pAgent.SetVariable<uint>("p_commandTarget", bigDragon, 3098897795u);
			return result;
		}
	}
}
