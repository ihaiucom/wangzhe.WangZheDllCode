using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node962 : Assignment
	{
		private int opr_p0;

		public Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node962()
		{
			this.opr_p0 = 7000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int ourCampActorsCount = ((ObjAgent)pAgent).GetOurCampActorsCount(this.opr_p0);
			pAgent.SetVariable<int>("p_friendCount", ourCampActorsCount, 3012176472u);
			return result;
		}
	}
}
