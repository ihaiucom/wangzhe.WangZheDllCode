using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroWarmSimpleAI_node268 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			((ObjAgent)pAgent).ShowTogetherSignal();
			return EBTStatus.BT_SUCCESS;
		}
	}
}
