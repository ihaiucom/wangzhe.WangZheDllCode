using Assets.Scripts.GameLogic;
using ResData;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroSimpleAI_node660 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			RES_LEVEL_HEROAITYPE mapAIMode = ((ObjAgent)pAgent).GetMapAIMode();
			RES_LEVEL_HEROAITYPE rES_LEVEL_HEROAITYPE = RES_LEVEL_HEROAITYPE.RES_LEVEL_HEROAITYPE_3V3;
			return (mapAIMode != rES_LEVEL_HEROAITYPE) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
