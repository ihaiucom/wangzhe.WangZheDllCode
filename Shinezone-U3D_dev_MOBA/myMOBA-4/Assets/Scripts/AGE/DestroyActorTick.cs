using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class DestroyActorTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int TargetId = -1;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			DestroyActorTick destroyActorTick = src as DestroyActorTick;
			this.TargetId = destroyActorTick.TargetId;
		}

		public override BaseEvent Clone()
		{
			DestroyActorTick destroyActorTick = ClassObjPool<DestroyActorTick>.Get();
			destroyActorTick.CopyData(this);
			return destroyActorTick;
		}

		public override void OnUse()
		{
			base.OnUse();
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.TargetId);
			if (!actorHandle)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			Singleton<GameObjMgr>.instance.DestroyActor(actorHandle.handle.ObjID);
		}
	}
}
