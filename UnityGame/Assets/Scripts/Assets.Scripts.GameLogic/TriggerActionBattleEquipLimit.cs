using AGE;
using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public class TriggerActionBattleEquipLimit : TriggerActionBase
	{
		public TriggerActionBattleEquipLimit(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
		{
		}

		public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			if (src && src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				src.handle.EquipComponent.m_isInEquipBoughtArea = true;
			}
			return null;
		}

		public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
		{
			if (src && src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				src.handle.EquipComponent.m_isInEquipBoughtArea = false;
				src.handle.EquipComponent.m_hasLeftEquipBoughtArea = true;
			}
		}
	}
}
