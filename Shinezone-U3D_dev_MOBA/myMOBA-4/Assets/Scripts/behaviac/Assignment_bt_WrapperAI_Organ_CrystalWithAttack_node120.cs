using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node120 : Assignment
	{
		private int opr_p0;

		public Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node120()
		{
			this.opr_p0 = 17000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint commandAttackHero = ((ObjAgent)pAgent).GetCommandAttackHero(this.opr_p0);
			pAgent.SetVariable<uint>("p_commandTarget", commandAttackHero, 3098897795u);
			return result;
		}
	}
}
