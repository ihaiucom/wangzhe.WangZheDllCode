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
			return (num > num2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
