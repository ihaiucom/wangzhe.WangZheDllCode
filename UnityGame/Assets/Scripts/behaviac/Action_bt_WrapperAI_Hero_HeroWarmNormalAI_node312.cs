using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Hero_HeroWarmNormalAI_node312 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint objID = (uint)pAgent.GetVariable(1128863647u);
			int range = (int)pAgent.GetVariable(1944425156u);
			((ObjAgent)pAgent).RealMoveInMoveAttack(objID, range);
			return EBTStatus.BT_SUCCESS;
		}
	}
}
