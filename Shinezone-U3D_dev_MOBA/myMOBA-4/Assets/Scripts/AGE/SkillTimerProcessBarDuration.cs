using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SkillTimerProcessBarDuration : DurationEvent
	{
		private ulong starTime;

		private int totalTime;

		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		private PoolObjHandle<ActorRoot> actorObj;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.starTime = 0uL;
			this.targetId = 0;
			this.totalTime = 0;
			this.actorObj.Release();
		}

		public override BaseEvent Clone()
		{
			SkillTimerProcessBarDuration skillTimerProcessBarDuration = ClassObjPool<SkillTimerProcessBarDuration>.Get();
			skillTimerProcessBarDuration.CopyData(this);
			return skillTimerProcessBarDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SkillTimerProcessBarDuration skillTimerProcessBarDuration = src as SkillTimerProcessBarDuration;
			this.targetId = skillTimerProcessBarDuration.targetId;
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			this.actorObj = _action.GetActorHandle(this.targetId);
			this.starTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
			this.totalTime = this.length;
			if (!this.actorObj || this.totalTime <= 0)
			{
				return;
			}
			SkillTimerEventParam skillTimerEventParam = new SkillTimerEventParam(this.totalTime, this.starTime, this.actorObj);
			Singleton<GameSkillEventSys>.GetInstance().SendEvent<SkillTimerEventParam>(GameSkillEventDef.AllEvent_SetSkillTimer, this.actorObj, ref skillTimerEventParam, GameSkillEventChannel.Channel_AllActor);
		}

		public override void Leave(Action _action, Track _track)
		{
			base.Leave(_action, _track);
			if (!this.actorObj)
			{
				return;
			}
			SkillTimerEventParam skillTimerEventParam = new SkillTimerEventParam(0, 0uL, this.actorObj);
			Singleton<GameSkillEventSys>.GetInstance().SendEvent<SkillTimerEventParam>(GameSkillEventDef.AllEvent_SetSkillTimer, this.actorObj, ref skillTimerEventParam, GameSkillEventChannel.Channel_AllActor);
		}
	}
}
