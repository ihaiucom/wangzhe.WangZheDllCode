using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveEvent(PassiveEventType.LifeTimePassiveEvent)]
	public class PassiveLifeTimeEvent : PassiveTimeEvent
	{
		public override void UpdateLogic(int _delta)
		{
			if (this.sourceActor && !this.sourceActor.handle.ActorControl.IsDeadState)
			{
				base.UpdateLogic(_delta);
			}
			else
			{
				this.startTime = 0;
				base.Reset();
			}
		}
	}
}
