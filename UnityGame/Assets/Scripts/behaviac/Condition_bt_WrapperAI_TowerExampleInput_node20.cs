using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_TowerExampleInput_node20 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint objID = (uint)pAgent.GetVariable(1128863647u);
			bool flag = ((ObjAgent)pAgent).IsTargetCanBeAttacked(objID);
			bool flag2 = true;
			return (flag == flag2) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
