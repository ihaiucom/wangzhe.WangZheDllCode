using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterBaozou_node4 : Assignment
	{
		private uint opr_p0;

		public Assignment_bt_WrapperAI_Monster_BTMonsterBaozou_node4()
		{
			this.opr_p0 = 1000u;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int randomInt = ((BTBaseAgent)pAgent).GetRandomInt(this.opr_p0);
			pAgent.SetVariable<int>("p_randomIndex", randomInt, 4088106663u);
			return result;
		}
	}
}
