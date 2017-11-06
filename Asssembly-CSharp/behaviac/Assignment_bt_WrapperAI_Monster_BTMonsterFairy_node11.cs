using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterFairy_node11 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint myObjID = ((ObjAgent)pAgent).GetMyObjID();
			pAgent.SetVariable<uint>("p_selfID", myObjID, 1205869406u);
			return result;
		}
	}
}
