using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node144 : Assignment
	{
		private uint opr_p0;

		public Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node144()
		{
			this.opr_p0 = 10u;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int randomInt = ((BTBaseAgent)pAgent).GetRandomInt(this.opr_p0);
			pAgent.SetVariable<int>("p_waitRandomFrames", randomInt, 1456164896u);
			return result;
		}
	}
}
