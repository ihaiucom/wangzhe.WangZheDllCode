using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroCommonAutoAI_node1285 : Condition
	{
		private int opl_p0;

		public Condition_bt_WrapperAI_Hero_HeroCommonAutoAI_node1285()
		{
			this.opl_p0 = 14000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int enemyHeroCountInRange = ((ObjAgent)pAgent).GetEnemyHeroCountInRange(this.opl_p0);
			int num = 0;
			return (enemyHeroCountInRange == num) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
