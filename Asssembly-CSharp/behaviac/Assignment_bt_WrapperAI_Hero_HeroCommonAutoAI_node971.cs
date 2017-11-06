using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node971 : Assignment
	{
		private int opr_p0;

		public Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node971()
		{
			this.opr_p0 = 5000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int enemyCountInRange = ((ObjAgent)pAgent).GetEnemyCountInRange(this.opr_p0);
			pAgent.SetVariable<int>("p_enemyCount", enemyCountInRange, 3421255u);
			return result;
		}
	}
}
