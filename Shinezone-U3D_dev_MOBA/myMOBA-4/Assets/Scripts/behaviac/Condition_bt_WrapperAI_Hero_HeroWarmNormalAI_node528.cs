using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroWarmNormalAI_node528 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int srchR = (int)pAgent.GetVariable(2451377514u);
			int enemyCountInRange = ((ObjAgent)pAgent).GetEnemyCountInRange(srchR);
			int num = 3;
			bool flag = enemyCountInRange < num;
			return (!flag) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS;
		}
	}
}
