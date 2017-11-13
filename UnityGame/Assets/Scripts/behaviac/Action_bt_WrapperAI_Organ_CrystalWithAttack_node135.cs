using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Organ_CrystalWithAttack_node135 : Action
	{
		private int method_p0;

		public Action_bt_WrapperAI_Organ_CrystalWithAttack_node135()
		{
			this.method_p0 = 30000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint centerObjId = (uint)pAgent.GetVariable(3098897795u);
			((ObjAgent)pAgent).NotifyTeamSelectRandomRoute(this.method_p0, centerObjId);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
