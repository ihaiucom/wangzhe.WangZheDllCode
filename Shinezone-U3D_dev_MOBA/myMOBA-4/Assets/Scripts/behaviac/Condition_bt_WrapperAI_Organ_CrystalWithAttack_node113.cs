using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Organ_CrystalWithAttack_node113 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int enemyCampAliveHerpNum = ((ObjAgent)pAgent).GetEnemyCampAliveHerpNum();
			int num = 4;
			bool flag = enemyCampAliveHerpNum <= num;
			return (!flag) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS;
		}
	}
}
