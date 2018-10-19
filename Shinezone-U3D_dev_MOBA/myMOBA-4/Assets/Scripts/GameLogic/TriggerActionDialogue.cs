using AGE;
using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class TriggerActionDialogue : TriggerActionBase
	{
		public TriggerActionDialogue(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
		{
		}

		public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			if (this.EnterUniqueId > 0)
			{
				int enterUniqueId = this.EnterUniqueId;
				GameObject inSrc = (!src) ? null : src.handle.gameObject;
				GameObject gameObject = (inTrigger == null) ? null : inTrigger.GetTriggerObj();
				if (!gameObject)
				{
					gameObject = ((!atker) ? null : atker.handle.gameObject);
				}
				MonoSingleton<DialogueProcessor>.GetInstance().PlayDrama(enterUniqueId, inSrc, gameObject, false);
			}
			return null;
		}

		public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
		{
			if (this.LeaveUniqueId > 0)
			{
				int leaveUniqueId = this.LeaveUniqueId;
				GameObject inSrc = (!src) ? null : src.handle.gameObject;
				GameObject inAtker = (inTrigger == null) ? null : inTrigger.GetTriggerObj();
				MonoSingleton<DialogueProcessor>.GetInstance().PlayDrama(leaveUniqueId, inSrc, inAtker, false);
			}
		}
	}
}
