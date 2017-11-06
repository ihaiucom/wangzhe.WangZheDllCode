using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node916 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint needHelpTarget = ((ObjAgent)pAgent).GetNeedHelpTarget();
			pAgent.SetVariable<uint>("p_helpTargtID", needHelpTarget, 1049827932u);
			return result;
		}
	}
}
