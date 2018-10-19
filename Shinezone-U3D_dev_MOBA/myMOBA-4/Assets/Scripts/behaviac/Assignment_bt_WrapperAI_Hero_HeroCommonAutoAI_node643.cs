using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node643 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint leader = ((ObjAgent)pAgent).GetLeader();
			pAgent.SetVariable<uint>("p_captainID", leader, 2120643215u);
			return result;
		}
	}
}
