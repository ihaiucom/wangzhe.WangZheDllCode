using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Organ_CrystalWithAttack_node131 : Action
	{
		private int method_p1;

		public Action_bt_WrapperAI_Organ_CrystalWithAttack_node131()
		{
			this.method_p1 = 70000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int index = (int)pAgent.GetVariable(3792555376u);
			uint centerObjId = (uint)pAgent.GetVariable(3098897795u);
			((ObjAgent)pAgent).NotifyTeamSelectRoute(index, this.method_p1, centerObjId);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
