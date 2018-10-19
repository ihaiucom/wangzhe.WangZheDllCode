using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node104 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint smallDragon = ((ObjAgent)pAgent).GetSmallDragon();
			pAgent.SetVariable<uint>("p_commandTarget", smallDragon, 3098897795u);
			return result;
		}
	}
}
