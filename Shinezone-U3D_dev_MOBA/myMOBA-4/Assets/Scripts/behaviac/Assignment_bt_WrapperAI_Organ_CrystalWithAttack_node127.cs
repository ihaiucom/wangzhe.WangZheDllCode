using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node127 : Assignment
	{
		private int opr_p0;

		private int opr_p1;

		public Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node127()
		{
			this.opr_p0 = 30000;
			this.opr_p1 = 20000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint commandAttackOrgan = ((ObjAgent)pAgent).GetCommandAttackOrgan(this.opr_p0, this.opr_p1);
			pAgent.SetVariable<uint>("p_commandTarget", commandAttackOrgan, 3098897795u);
			return result;
		}
	}
}
