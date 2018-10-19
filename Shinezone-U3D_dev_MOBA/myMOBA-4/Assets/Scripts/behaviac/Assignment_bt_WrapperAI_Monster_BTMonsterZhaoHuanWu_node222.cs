using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Monster_BTMonsterZhaoHuanWu_node222 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint master = ((ObjAgent)pAgent).GetMaster();
			pAgent.SetVariable<uint>("p_captainID", master, 2120643215u);
			return result;
		}
	}
}
