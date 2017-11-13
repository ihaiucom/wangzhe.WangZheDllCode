using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroWarmNormalAI_node1104 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint myTargetID = ((ObjAgent)pAgent).GetMyTargetID();
			uint num = (uint)pAgent.GetVariable(1128863647u);
			return (myTargetID == num) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
