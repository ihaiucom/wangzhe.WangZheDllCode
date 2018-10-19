using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("ActionControl")]
	public class StopCurrentActionSkill : DurationEvent
	{
		public enum CheckConditionType
		{
			ActorDead
		}

		public int checkTargetId = -1;

		public StopCurrentActionSkill.CheckConditionType CheckCondition;

		private PoolObjHandle<ActorRoot> skillOriginator;

		private PoolObjHandle<ActorRoot> checkTargetActor;

		public override BaseEvent Clone()
		{
			StopCurrentActionSkill stopCurrentActionSkill = ClassObjPool<StopCurrentActionSkill>.Get();
			stopCurrentActionSkill.CopyData(this);
			return stopCurrentActionSkill;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			StopCurrentActionSkill stopCurrentActionSkill = src as StopCurrentActionSkill;
			this.checkTargetId = stopCurrentActionSkill.checkTargetId;
			this.CheckCondition = stopCurrentActionSkill.CheckCondition;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.checkTargetId = -1;
			this.CheckCondition = StopCurrentActionSkill.CheckConditionType.ActorDead;
			this.skillOriginator.Release();
			this.checkTargetActor.Release();
		}

		public override void Enter(Action _action, Track _track)
		{
			this.checkTargetActor = _action.GetActorHandle(this.checkTargetId);
			if (!this.checkTargetActor)
			{
				DebugHelper.Assert(false, "�����Ķ���Ϊ�գ���������Ӧage����");
				return;
			}
			base.Enter(_action, _track);
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			if (this.CheckCondition == StopCurrentActionSkill.CheckConditionType.ActorDead)
			{
				ObjWrapper actorControl = this.checkTargetActor.handle.ActorControl;
				if (actorControl != null && actorControl.IsDeadState)
				{
					BaseSkill refParamObject = _action.refParams.GetRefParamObject<BaseSkill>("SkillObj");
					if (refParamObject != null)
					{
						refParamObject.Stop();
					}
				}
			}
			base.Process(_action, _track, _localTime);
		}
	}
}
