using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using ResData;
using System;

namespace AGE
{
	[EventCategory("MMGame/SkillFunc")]
	public class SkillFuncDuration : DurationCondition
	{
		public RES_SKILLFUNC_TYPE SkillFuncType;

		protected SSkillFuncContext m_context = default(SSkillFuncContext);

		private bool m_bSucceeded;

		protected bool bInit;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.SkillFuncType = RES_SKILLFUNC_TYPE.RES_SKILLFUNC_TYPE_PHYSHURT;
			this.m_context = default(SSkillFuncContext);
			this.m_bSucceeded = false;
			this.bInit = false;
		}

		public override BaseEvent Clone()
		{
			SkillFuncDuration skillFuncDuration = ClassObjPool<SkillFuncDuration>.Get();
			skillFuncDuration.CopyData(this);
			return skillFuncDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SkillFuncDuration skillFuncDuration = src as SkillFuncDuration;
			this.SkillFuncType = skillFuncDuration.SkillFuncType;
			this.m_context = skillFuncDuration.m_context;
			this.m_bSucceeded = skillFuncDuration.m_bSucceeded;
			this.bInit = skillFuncDuration.bInit;
		}

		public override bool Check(Action _action, Track _track)
		{
			return this.m_bSucceeded;
		}

		private void InitContext(Action _action)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(1);
			PoolObjHandle<ActorRoot> actorHandle2 = _action.GetActorHandle(0);
			if (!actorHandle || !actorHandle2)
			{
				return;
			}
			SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			BuffSkill refParamObject2 = _action.refParams.GetRefParamObject<BuffSkill>("SkillObj");
			ResDT_SkillFunc inSkillFunc = null;
			if (refParamObject2 != null && refParamObject2.FindSkillFunc((int)this.SkillFuncType, out inSkillFunc))
			{
				this.m_context.inTargetObj = actorHandle;
				this.m_context.inOriginator = actorHandle2;
				this.m_context.inUseContext = refParamObject;
				this.m_context.inSkillFunc = inSkillFunc;
				this.m_context.LocalParams = new SSkillFuncIntParam[8];
				for (int i = 0; i < 8; i++)
				{
					this.m_context.LocalParams[i] = default(SSkillFuncIntParam);
					this.m_context.LocalParams[i].iParam = 0;
				}
				this.m_context.inAction = new PoolObjHandle<Action>(_action);
				this.m_context.inBuffSkill = new PoolObjHandle<BuffSkill>(refParamObject2);
				this.m_context.inDoCount = 0;
				this.m_context.inOverlayCount = refParamObject2.GetOverlayCount();
				this.m_context.inLastEffect = true;
				if (refParamObject != null)
				{
					this.m_context.inEffectCount = refParamObject.EffectCount;
					this.m_context.inEffectCountInSingleTrigger = refParamObject.EffectCountInSingleTrigger;
					this.m_context.inMarkCount = refParamObject.MarkCount;
				}
				this.m_context.InitSkillFuncContext();
				refParamObject2.SetBuffLevel(this.m_context.iSkillLevel);
				this.bInit = true;
			}
		}

		private void DeinitContext()
		{
		}

		protected bool DoSkillFuncShared(ESkillFuncStage inStage)
		{
			if (!this.bInit || !Singleton<BattleLogic>.GetInstance().isRuning || Singleton<BattleLogic>.GetInstance().isGameOver)
			{
				return false;
			}
			this.m_context.inStage = inStage;
			this.m_context.inDoCount = this.m_context.inDoCount + 1;
			this.m_bSucceeded = Singleton<SkillFuncDelegator>.GetInstance().DoSkillFunc((int)this.SkillFuncType, ref this.m_context);
			return this.m_bSucceeded;
		}

		public override void Enter(Action _action, Track _track)
		{
			this.InitContext(_action);
			this.DoSkillFuncShared(ESkillFuncStage.Enter);
			base.Enter(_action, _track);
		}

		public override void Leave(Action _action, Track _track)
		{
			this.DoSkillFuncShared(ESkillFuncStage.Leave);
			this.DeinitContext();
			base.Leave(_action, _track);
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
		}
	}
}
