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
				GameObject inSrc = src ? src.handle.gameObject : null;
				GameObject gameObject = (inTrigger != null) ? inTrigger.GetTriggerObj() : null;
				if (!gameObject)
				{
					gameObject = (atker ? atker.handle.gameObject : null);
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
				GameObject inSrc = src ? src.handle.gameObject : null;
				GameObject inAtker = (inTrigger != null) ? inTrigger.GetTriggerObj() : null;
				MonoSingleton<DialogueProcessor>.GetInstance().PlayDrama(leaveUniqueId, inSrc, inAtker, false);
			}
		}
	}
}
