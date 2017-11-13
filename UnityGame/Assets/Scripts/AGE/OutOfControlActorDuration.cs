using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class OutOfControlActorDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int attackId = -1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		public int subType = -1;

		private PoolObjHandle<ActorRoot> actorObj;

		public override void OnUse()
		{
			base.OnUse();
			this.attackId = -1;
			this.targetId = -1;
			this.subType = -1;
			this.actorObj.Release();
		}

		public override BaseEvent Clone()
		{
			OutOfControlActorDuration outOfControlActorDuration = ClassObjPool<OutOfControlActorDuration>.Get();
			outOfControlActorDuration.CopyData(this);
			return outOfControlActorDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			OutOfControlActorDuration outOfControlActorDuration = src as OutOfControlActorDuration;
			this.attackId = outOfControlActorDuration.attackId;
			this.targetId = outOfControlActorDuration.targetId;
			this.actorObj = outOfControlActorDuration.actorObj;
			this.subType = outOfControlActorDuration.subType;
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			this.actorObj = _action.GetActorHandle(this.targetId);
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.attackId);
			if (!this.actorObj || !actorHandle)
			{
				return;
			}
			ObjWrapper actorControl = this.actorObj.handle.ActorControl;
			if (actorControl == null)
			{
				return;
			}
			if (!actorControl.IsDeadState)
			{
				actorControl.TerminateMove();
				actorControl.ClearMoveCommand();
				actorControl.ForceAbortCurUseSkill();
				actorControl.SetOutOfControl(true, (OutOfControlType)this.subType);
				switch (this.subType)
				{
				case 0:
					actorControl.SetTauntTarget(actorHandle);
					break;
				case 2:
					actorControl.SetTerrorActor(actorHandle);
					break;
				}
			}
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			base.Process(_action, _track, _localTime);
		}

		public override void Leave(Action _action, Track _track)
		{
			base.Leave(_action, _track);
			if (!this.actorObj)
			{
				return;
			}
			ObjWrapper actorControl = this.actorObj.handle.ActorControl;
			if (actorControl == null)
			{
				return;
			}
			actorControl.SetOutOfControl(false, (OutOfControlType)this.subType);
		}
	}
}
