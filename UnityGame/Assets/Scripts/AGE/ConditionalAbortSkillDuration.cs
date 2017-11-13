using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class ConditionalAbortSkillDuration : DurationCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int AttackerID;

		[ObjectTemplate(new Type[]
		{

		})]
		public int TargetID;

		private bool done;

		private PoolObjHandle<ActorRoot> targetActor;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			ConditionalAbortSkillDuration conditionalAbortSkillDuration = src as ConditionalAbortSkillDuration;
			this.AttackerID = conditionalAbortSkillDuration.AttackerID;
			this.TargetID = conditionalAbortSkillDuration.TargetID;
			this.done = conditionalAbortSkillDuration.done;
			this.targetActor = conditionalAbortSkillDuration.targetActor;
		}

		public override BaseEvent Clone()
		{
			ConditionalAbortSkillDuration conditionalAbortSkillDuration = ClassObjPool<ConditionalAbortSkillDuration>.Get();
			conditionalAbortSkillDuration.CopyData(this);
			return conditionalAbortSkillDuration;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.AttackerID = -1;
			this.TargetID = -1;
			this.done = false;
			this.targetActor.Release();
		}

		public override void Enter(Action _action, Track _track)
		{
			this.targetActor = _action.GetActorHandle(this.TargetID);
			base.Enter(_action, _track);
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			if (this.done)
			{
				return;
			}
			if (!this.targetActor || this.targetActor.handle.ActorControl.IsDeadState)
			{
				PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.AttackerID);
				if (actorHandle && actorHandle.handle.SkillControl != null)
				{
					actorHandle.handle.SkillControl.ForceAbortCurUseSkill();
				}
				this.done = true;
			}
			base.Process(_action, _track, _localTime);
		}

		public override void Leave(Action _action, Track _track)
		{
			base.Leave(_action, _track);
		}
	}
}
