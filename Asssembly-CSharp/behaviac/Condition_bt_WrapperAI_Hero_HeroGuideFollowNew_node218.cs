using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroGuideFollowNew_node218 : Condition
	{
		private int opl_p1;

		public Condition_bt_WrapperAI_Hero_HeroGuideFollowNew_node218()
		{
			this.opl_p1 = 10000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint objID = (uint)pAgent.GetVariable(1128863647u);
			bool flag = ((ObjAgent)pAgent).IsDistanceToActorLessThanRange(objID, this.opl_p1);
			bool flag2 = true;
			return (flag == flag2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
