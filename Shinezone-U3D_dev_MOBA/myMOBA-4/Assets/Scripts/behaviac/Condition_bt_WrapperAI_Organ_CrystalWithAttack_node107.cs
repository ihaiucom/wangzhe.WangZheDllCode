using Assets.Scripts.GameLogic;
using CSProtocol;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Organ_CrystalWithAttack_node107 : Condition
	{
		private int opl_p1;

		public Condition_bt_WrapperAI_Organ_CrystalWithAttack_node107()
		{
			this.opl_p1 = 45000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint objID = (uint)pAgent.GetVariable(3098897795u);
			COM_PLAYERCAMP camp = (COM_PLAYERCAMP)((int)pAgent.GetVariable(2826458051u));
			int heroNumInRange = ((ObjAgent)pAgent).GetHeroNumInRange(objID, this.opl_p1, camp);
			int num = 2;
			bool flag = heroNumInRange >= num;
			return (!flag) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS;
		}
	}
}
