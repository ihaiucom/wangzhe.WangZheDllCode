using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Compute_bt_WrapperAI_Organ_CrystalWithAttack_node31 : Compute
	{
		private uint opr1_p0;

		public Compute_bt_WrapperAI_Organ_CrystalWithAttack_node31()
		{
			this.opr1_p0 = 15u;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int randomInt = ((BTBaseAgent)pAgent).GetRandomInt(this.opr1_p0);
			int num = 15;
			int value = randomInt + num;
			pAgent.SetVariable<int>("p_waitFrame", value, 3589669257u);
			return result;
		}
	}
}
