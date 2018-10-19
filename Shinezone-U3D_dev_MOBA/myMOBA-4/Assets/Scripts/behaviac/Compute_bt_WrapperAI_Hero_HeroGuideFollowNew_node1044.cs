using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Compute_bt_WrapperAI_Hero_HeroGuideFollowNew_node1044 : Compute
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int campIndex = ((ObjAgent)pAgent).GetCampIndex();
			int num = 4;
			int value = campIndex * num;
			pAgent.SetVariable<int>("p_waitToPlayBornAge", value, 20770612u);
			return result;
		}
	}
}
