using Assets.Scripts.GameLogic;
using CSProtocol;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node106 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			COM_PLAYERCAMP myCamp = ((ObjAgent)pAgent).GetMyCamp();
			pAgent.SetVariable<COM_PLAYERCAMP>("p_camp", myCamp, 2826458051u);
			return result;
		}
	}
}
