using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterPassive_zhouwang_node234 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint myTargetID = ((ObjAgent)pAgent).GetMyTargetID();
			pAgent.SetVariable<uint>("p_targetID", myTargetID, 1128863647u);
			return result;
		}
	}
}
