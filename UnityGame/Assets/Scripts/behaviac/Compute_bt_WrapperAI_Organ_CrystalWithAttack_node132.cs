using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Compute_bt_WrapperAI_Organ_CrystalWithAttack_node132 : Compute
	{
		private uint opr1_p0;

		public Compute_bt_WrapperAI_Organ_CrystalWithAttack_node132()
		{
			this.opr1_p0 = 300u;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int randomInt = ((BTBaseAgent)pAgent).GetRandomInt(this.opr1_p0);
			int num = 400;
			int value = randomInt + num;
			pAgent.SetVariable<int>("p_attackTowerWaitFrame", value, 775476708u);
			return result;
		}
	}
}
