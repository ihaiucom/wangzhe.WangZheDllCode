using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node617 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint nearestMemberNotCaptain = ((ObjAgent)pAgent).GetNearestMemberNotCaptain();
			pAgent.SetVariable<uint>("p_nearstMember", nearestMemberNotCaptain, 2686243218u);
			return result;
		}
	}
}
