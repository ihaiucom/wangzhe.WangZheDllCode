using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroWarmNormalAI_node483 : Condition
	{
		private int opl_p0;

		public Condition_bt_WrapperAI_Hero_HeroWarmNormalAI_node483()
		{
			this.opl_p0 = 9500;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint num = ((ObjAgent)pAgent).IsUnderEnemyBuilding(this.opl_p0);
			uint num2 = 0u;
			bool flag = num > num2;
			return (!flag) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS;
		}
	}
}
