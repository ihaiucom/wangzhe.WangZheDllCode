using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class ChangeAnimDuration : DurationCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		private PoolObjHandle<ActorRoot> actorHandle;

		public string originalAnimName;

		public string changedAnimName;

		public bool bOnlyRecover;

		private bool isDone;

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.actorHandle.Release();
			this.originalAnimName = string.Empty;
			this.changedAnimName = string.Empty;
			this.isDone = false;
		}

		public override BaseEvent Clone()
		{
			ChangeAnimDuration changeAnimDuration = ClassObjPool<ChangeAnimDuration>.Get();
			changeAnimDuration.CopyData(this);
			return changeAnimDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			ChangeAnimDuration changeAnimDuration = src as ChangeAnimDuration;
			this.targetId = changeAnimDuration.targetId;
			this.actorHandle = changeAnimDuration.actorHandle;
			this.originalAnimName = changeAnimDuration.originalAnimName;
			this.changedAnimName = changeAnimDuration.changedAnimName;
			this.bOnlyRecover = changeAnimDuration.bOnlyRecover;
		}

		private void Init(Action _action, Track _track)
		{
			this.actorHandle = _action.GetActorHandle(this.targetId);
			if (!this.actorHandle)
			{
				this.isDone = true;
				return;
			}
			if (!this.bOnlyRecover)
			{
				this.actorHandle.handle.AnimControl.ChangeAnimParam(this.originalAnimName, this.changedAnimName);
			}
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			this.Init(_action, _track);
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			base.Process(_action, _track, _localTime);
		}

		public override void Leave(Action _action, Track _track)
		{
			this.isDone = true;
			if (this.actorHandle)
			{
				this.actorHandle.handle.AnimControl.RecoverAnimParam(this.originalAnimName);
			}
			base.Leave(_action, _track);
		}

		public override bool Check(Action _action, Track _track)
		{
			return this.isDone;
		}
	}
}
