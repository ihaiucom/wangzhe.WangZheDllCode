using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/SkillFunc")]
	public class SkillFuncPerioidc : SkillFuncDuration
	{
		public int PeriodicInterval = 1000;

		private int intervalTimer;

		private int lastTime;

		public override void OnUse()
		{
			base.OnUse();
			this.PeriodicInterval = 1000;
			this.intervalTimer = 0;
			this.lastTime = 0;
		}

		public override BaseEvent Clone()
		{
			SkillFuncPerioidc skillFuncPerioidc = ClassObjPool<SkillFuncPerioidc>.Get();
			skillFuncPerioidc.CopyData(this);
			return skillFuncPerioidc;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SkillFuncPerioidc skillFuncPerioidc = src as SkillFuncPerioidc;
			this.PeriodicInterval = skillFuncPerioidc.PeriodicInterval;
			this.intervalTimer = skillFuncPerioidc.intervalTimer;
			this.lastTime = skillFuncPerioidc.lastTime;
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			if (!this.bInit)
			{
				return;
			}
			this.PeriodicInterval = (int)this.m_context.inSkillFunc.dwSkillFuncFreq;
			if (this.PeriodicInterval > 0)
			{
				this.lastTime = 0;
				this.intervalTimer = 0;
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			base.Leave(_action, _track);
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			if (this.PeriodicInterval <= 0 || !this.bInit)
			{
				return;
			}
			int num = _localTime - this.lastTime;
			this.lastTime = _localTime;
			this.intervalTimer += num;
			if (this.intervalTimer >= this.PeriodicInterval)
			{
				this.intervalTimer = 0;
				base.DoSkillFuncShared(ESkillFuncStage.Update);
				bool status = this.Check(_action, _track);
				_action.SetCondition(_track, status);
			}
		}
	}
}
