using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SetObjBehaviourModeTick : TickEvent
	{
		public ObjBehaviMode Mode;

		public bool bSetSpecificMode;

		public int targetId;

		public override void OnUse()
		{
			base.OnUse();
			this.Mode = ObjBehaviMode.State_Idle;
			this.targetId = -1;
		}

		public override BaseEvent Clone()
		{
			SetObjBehaviourModeTick setObjBehaviourModeTick = ClassObjPool<SetObjBehaviourModeTick>.Get();
			setObjBehaviourModeTick.CopyData(this);
			return setObjBehaviourModeTick;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SetObjBehaviourModeTick setObjBehaviourModeTick = src as SetObjBehaviourModeTick;
			this.Mode = setObjBehaviourModeTick.Mode;
			this.targetId = setObjBehaviourModeTick.targetId;
		}

		public override void Process(Action _action, Track _track)
		{
			base.Process(_action, _track);
			if (this.targetId < 0)
			{
				PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
				if (captain && captain.handle.ActorControl != null)
				{
					captain.handle.ActorControl.SetObjBehaviMode(this.Mode);
				}
			}
			else
			{
				PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
				if (!actorHandle)
				{
					return;
				}
				ObjWrapper actorControl = actorHandle.handle.ActorControl;
				if (actorControl != null)
				{
					actorControl.SetObjBehaviMode(this.Mode);
				}
			}
		}
	}
}
