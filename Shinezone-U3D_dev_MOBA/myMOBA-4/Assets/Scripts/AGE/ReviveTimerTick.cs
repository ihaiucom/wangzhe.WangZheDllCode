using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class ReviveTimerTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		public int yOffset;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.yOffset = 0;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			ReviveTimerTick reviveTimerTick = src as ReviveTimerTick;
			this.targetId = reviveTimerTick.targetId;
			this.yOffset = reviveTimerTick.yOffset;
		}

		public override BaseEvent Clone()
		{
			ReviveTimerTick reviveTimerTick = ClassObjPool<ReviveTimerTick>.Get();
			reviveTimerTick.CopyData(this);
			return reviveTimerTick;
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (actorHandle && actorHandle.handle.HudControl != null && actorHandle.handle.ActorControl != null && actorHandle.handle.ActorControl.IsDeadState)
			{
				int reviveTotalTime = actorHandle.handle.ActorControl.GetReviveTotalTime();
				actorHandle.handle.HudControl.ShowReviveTimer(this.yOffset, reviveTotalTime);
			}
		}
	}
}
