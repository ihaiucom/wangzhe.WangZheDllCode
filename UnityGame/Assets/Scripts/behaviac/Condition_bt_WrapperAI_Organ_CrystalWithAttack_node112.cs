using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Organ_CrystalWithAttack_node112 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int selfCampAliveHerpNum = ((ObjAgent)pAgent).GetSelfCampAliveHerpNum();
			int num = 4;
			return (selfCampAliveHerpNum >= num) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
