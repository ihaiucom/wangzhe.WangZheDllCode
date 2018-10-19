using AGE;
using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public class TriggerActionSetGlobalVariable : TriggerActionBase
	{
		public TriggerActionSetGlobalVariable(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
		{
		}

		public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			if (Singleton<BattleLogic>.instance.m_globalTrigger != null)
			{
				Singleton<BattleLogic>.instance.m_globalTrigger.CurGlobalVariable = this.EnterUniqueId;
			}
			return null;
		}
	}
}
