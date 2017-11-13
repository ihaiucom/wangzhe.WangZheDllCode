using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	internal class FocusCameraDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		private PoolObjHandle<ActorRoot> targetActor;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			FocusCameraDuration focusCameraDuration = src as FocusCameraDuration;
			this.targetId = focusCameraDuration.targetId;
		}

		public override BaseEvent Clone()
		{
			FocusCameraDuration focusCameraDuration = ClassObjPool<FocusCameraDuration>.Get();
			focusCameraDuration.CopyData(this);
			return focusCameraDuration;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetActor.Release();
		}

		public override void Enter(Action _action, Track _track)
		{
			this.targetActor = _action.GetActorHandle(this.targetId);
		}

		public override void Leave(Action _action, Track _track)
		{
			if (!this.targetActor)
			{
				return;
			}
			if (ActorHelper.IsHostCtrlActor(ref this.targetActor) && !Singleton<WatchController>.GetInstance().IsWatching)
			{
				MonoSingleton<CameraSystem>.instance.SetFocusActor(this.targetActor);
			}
		}
	}
}
