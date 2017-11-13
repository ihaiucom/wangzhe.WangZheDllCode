using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroWarmNormalAI_node542 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint objID = (uint)pAgent.GetVariable(3099920505u);
			ActorTypeDef actorType = ((ObjAgent)pAgent).GetActorType(objID);
			ActorTypeDef actorTypeDef = ActorTypeDef.Actor_Type_Hero;
			return (actorType == actorTypeDef) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}
}
