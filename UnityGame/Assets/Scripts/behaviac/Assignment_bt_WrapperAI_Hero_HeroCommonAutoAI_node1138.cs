using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node1138 : Assignment
	{
		private int opr_p0;

		public Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node1138()
		{
			this.opr_p0 = 90;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int waitFrame = ((ObjAgent)pAgent).GetWaitFrame(this.opr_p0);
			pAgent.SetVariable<int>("p_idleShowLast", waitFrame, 1460711631u);
			return result;
		}
	}
}
