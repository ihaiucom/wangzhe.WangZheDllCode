using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveEvent(PassiveEventType.LifeRandomPassiveEvent)]
	public class PassiveLifeRandomEvent : PassiveRandomEvent
	{
		public override void UpdateLogic(int _delta)
		{
			if (this.sourceActor && !this.sourceActor.handle.ActorControl.IsDeadState)
			{
				base.UpdateLogic(_delta);
			}
		}
	}
}
