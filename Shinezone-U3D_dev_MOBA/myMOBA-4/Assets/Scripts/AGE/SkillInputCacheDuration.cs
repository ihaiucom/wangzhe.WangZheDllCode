using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	internal class SkillInputCacheDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		public bool cacheSkill;

		public bool cacheMove;

		private SkillComponent skillControl;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SkillInputCacheDuration skillInputCacheDuration = src as SkillInputCacheDuration;
			this.targetId = skillInputCacheDuration.targetId;
			this.cacheSkill = skillInputCacheDuration.cacheSkill;
			this.cacheMove = skillInputCacheDuration.cacheMove;
		}

		public override BaseEvent Clone()
		{
			SkillInputCacheDuration skillInputCacheDuration = ClassObjPool<SkillInputCacheDuration>.Get();
			skillInputCacheDuration.CopyData(this);
			return skillInputCacheDuration;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.skillControl = null;
		}

		public override void Enter(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (!actorHandle)
			{
				return;
			}
			this.skillControl = actorHandle.handle.SkillControl;
			if (this.skillControl == null)
			{
				return;
			}
			if (this.skillControl.SkillUseCache != null)
			{
				if (this.cacheSkill)
				{
					this.skillControl.SkillUseCache.SetCacheSkill(true);
				}
				if (this.cacheMove)
				{
					this.skillControl.SkillUseCache.SetCacheMove(true);
					DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(actorHandle, actorHandle);
					Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorClearMove, ref defaultGameEventParam);
				}
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			if (this.skillControl == null)
			{
				return;
			}
			if (this.skillControl.SkillUseCache != null)
			{
				if (this.cacheSkill)
				{
					this.skillControl.SkillUseCache.SetCacheSkill(false);
				}
				if (this.cacheMove)
				{
					this.skillControl.SkillUseCache.SetCacheMove(false);
				}
			}
			this.skillControl = null;
		}
	}
}
