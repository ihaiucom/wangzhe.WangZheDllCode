using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class CheckConditionDuration : DurationCondition
	{
		public int trackId = -1;

		public bool bHitTargetHero;

		public bool bTriggerBullet;

		public bool bActorDead;

		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		private bool bCondition;

		private PoolObjHandle<ActorRoot> targetActor;

		public override BaseEvent Clone()
		{
			CheckConditionDuration checkConditionDuration = ClassObjPool<CheckConditionDuration>.Get();
			checkConditionDuration.CopyData(this);
			return checkConditionDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			CheckConditionDuration checkConditionDuration = src as CheckConditionDuration;
			this.trackId = checkConditionDuration.trackId;
			this.bHitTargetHero = checkConditionDuration.bHitTargetHero;
			this.bTriggerBullet = checkConditionDuration.bTriggerBullet;
			this.bActorDead = checkConditionDuration.bActorDead;
			this.targetId = checkConditionDuration.targetId;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.trackId = -1;
			this.bCondition = false;
			this.targetActor.Release();
		}

		public override bool Check(Action _action, Track _track)
		{
			return this.bCondition;
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			if (this.bActorDead)
			{
				this.targetActor = _action.GetActorHandle(this.targetId);
			}
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			if (this.bHitTargetHero)
			{
				_action.refParams.GetRefParam("_HitTargetHero", ref this.bCondition);
			}
			else if (this.bTriggerBullet)
			{
				_action.refParams.GetRefParam("_TriggerBullet", ref this.bCondition);
			}
			else if (this.bActorDead && this.targetActor && this.targetActor.handle.ActorControl.IsDeadState)
			{
				this.bCondition = true;
			}
			base.Process(_action, _track, _localTime);
		}
	}
}
