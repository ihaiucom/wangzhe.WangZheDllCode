using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using System;

namespace AGE
{
	[EventCategory("MMGame/System")]
	public class MainCameraLookAt : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int srcId = -1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		public override BaseEvent Clone()
		{
			MainCameraLookAt mainCameraLookAt = ClassObjPool<MainCameraLookAt>.Get();
			mainCameraLookAt.CopyData(this);
			return mainCameraLookAt;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			MainCameraLookAt mainCameraLookAt = src as MainCameraLookAt;
			this.srcId = mainCameraLookAt.srcId;
			this.targetId = mainCameraLookAt.targetId;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.srcId = -1;
			this.targetId = -1;
		}

		public override void Process(Action _action, Track _track)
		{
			base.Process(_action, _track);
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.srcId);
			PoolObjHandle<ActorRoot> actorHandle2 = _action.GetActorHandle(this.targetId);
			if (actorHandle2 && actorHandle && ActorHelper.IsHostCtrlActor(ref actorHandle) && !Singleton<WatchController>.GetInstance().IsWatching)
			{
				MonoSingleton<CameraSystem>.instance.SetFocusActor(actorHandle2);
			}
		}
	}
}
