using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterPassive_node110 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint terrorMeActor = ((ObjAgent)pAgent).GetTerrorMeActor();
			pAgent.SetVariable<uint>("p_terrorMeActor", terrorMeActor, 1028319457u);
			return result;
		}
	}
}
