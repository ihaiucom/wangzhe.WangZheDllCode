using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node1084 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint objID = (uint)pAgent.GetVariable(2120643215u);
			uint givenActorTarget = ((ObjAgent)pAgent).GetGivenActorTarget(objID);
			pAgent.SetVariable<uint>("p_targetID", givenActorTarget, 1128863647u);
			return result;
		}
	}
}
