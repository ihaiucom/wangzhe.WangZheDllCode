using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroCommonAutoAI_node400 : Condition
	{
		private int opl_p0;

		public Condition_bt_WrapperAI_Hero_HeroCommonAutoAI_node400()
		{
			this.opl_p0 = 5000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			bool flag = ((ObjAgent)pAgent).HasMemberHpLessThan(this.opl_p0);
			bool flag2 = false;
			return (flag == flag2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
