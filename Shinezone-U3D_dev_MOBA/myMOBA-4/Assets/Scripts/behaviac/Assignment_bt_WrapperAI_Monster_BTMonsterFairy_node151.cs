using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterFairy_node151 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int srchR = (int)pAgent.GetVariable(2451377514u);
			uint value = ((ObjAgent)pAgent).AdvanceCommonAttackSearchEnemy(srchR);
			pAgent.SetVariable<uint>("p_targetID", value, 1128863647u);
			return result;
		}
	}
}
