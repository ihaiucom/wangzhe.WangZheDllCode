using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroGuideFollow_node222 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint hostPlayrCaptain = ((ObjAgent)pAgent).GetHostPlayrCaptain();
			pAgent.SetVariable<uint>("p_captainID", hostPlayrCaptain, 2120643215u);
			return result;
		}
	}
}
