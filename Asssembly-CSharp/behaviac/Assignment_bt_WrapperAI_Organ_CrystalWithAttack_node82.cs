using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node82 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int attackRange = ((ObjAgent)pAgent).GetAttackRange();
			pAgent.SetVariable<int>("p_srchRange", attackRange, 2451377514u);
			return result;
		}
	}
}
