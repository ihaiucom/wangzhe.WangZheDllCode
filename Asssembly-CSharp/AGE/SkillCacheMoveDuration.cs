using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	internal class SkillCacheMoveDuration : DurationCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		public int moveSpeed;

		private bool bInit;

		private bool bStart;

		private int lastTime;

		private PoolObjHandle<ActorRoot> targetActor;

		private SkillComponent skillControl;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SkillCacheMoveDuration skillCacheMoveDuration = src as SkillCacheMoveDuration;
			this.targetId = skillCacheMoveDuration.targetId;
			this.moveSpeed = skillCacheMoveDuration.moveSpeed;
		}

		public override BaseEvent Clone()
		{
			SkillCacheMoveDuration skillCacheMoveDuration = ClassObjPool<SkillCacheMoveDuration>.Get();
			skillCacheMoveDuration.CopyData(this);
			return skillCacheMoveDuration;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.bInit = false;
			this.bStart = false;
			this.lastTime = 0;
			this.skillControl = null;
			this.targetActor.Release();
		}

		public override void Enter(Action _action, Track _track)
		{
			this.targetActor = _action.GetActorHandle(this.targetId);
			if (!this.targetActor)
			{
				return;
			}
			this.targetActor.handle.ObjLinker.AddCustomMoveLerp(new CustomMoveLerpFunc(this.ActionMoveLerp));
			this.skillControl = this.targetActor.handle.SkillControl;
			if (this.skillControl == null || this.skillControl.SkillUseCache == null)
			{
				return;
			}
			this.bInit = true;
			if (this.moveSpeed > 0 && !this.skillControl.SkillUseCache.GetCacheMoveExpire() && this.skillControl.CurUseSkill != null)
			{
				this.skillControl.CurUseSkill.skillAbort.InitAbort(false);
				this.skillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_1);
				this.skillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_2);
				this.skillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_3);
				this.skillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_4);
				this.bStart = true;
			}
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			base.Process(_action, _track, _localTime);
			if (!this.bInit || this.skillControl == null || this.skillControl.SkillUseCache == null)
			{
				return;
			}
			int deltaTime = _localTime - this.lastTime;
			this.lastTime = _localTime;
			this.skillControl.SkillUseCache.UseSkillCacheMove(this.targetActor, deltaTime, this.moveSpeed);
			if (this.moveSpeed > 0 && !this.skillControl.SkillUseCache.GetCacheMoveExpire() && this.skillControl.CurUseSkill != null)
			{
				this.skillControl.CurUseSkill.skillAbort.InitAbort(false);
				this.skillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_1);
				this.skillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_2);
				this.skillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_3);
				this.skillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_4);
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			if (this.targetActor)
			{
				this.targetActor.handle.ObjLinker.RmvCustomMoveLerp(new CustomMoveLerpFunc(this.ActionMoveLerp));
			}
			if (!this.bInit || this.skillControl == null || this.skillControl.SkillUseCache == null)
			{
				return;
			}
			if (this.moveSpeed > 0 && !this.skillControl.SkillUseCache.GetCacheMoveExpire() && this.skillControl.CurUseSkill != null)
			{
				this.skillControl.CurUseSkill.skillAbort.InitAbort(true);
			}
			this.skillControl.SkillUseCache.SetCacheMoveExpire(true);
		}

		private void ActionMoveLerp(ActorRoot actor, uint nDelta, bool bReset)
		{
			if (actor == null || this.skillControl == null || this.skillControl.SkillUseCache == null)
			{
				return;
			}
			this.skillControl.SkillUseCache.UseSkillCacheLerpMove(this.targetActor, (int)nDelta, this.moveSpeed);
		}

		public override bool Check(Action _action, Track _track)
		{
			return this.bStart;
		}
	}
}
