using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using ResData;
using System;

namespace AGE
{
	[EventCategory("MMGame/SkillFunc")]
	public class SkillFuncInstant : TickCondition
	{
		public RES_SKILLFUNC_TYPE SkillFuncType;

		private bool m_bSucceeded;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.SkillFuncType = RES_SKILLFUNC_TYPE.RES_SKILLFUNC_TYPE_PHYSHURT;
			this.m_bSucceeded = false;
		}

		public override BaseEvent Clone()
		{
			SkillFuncInstant skillFuncInstant = ClassObjPool<SkillFuncInstant>.Get();
			skillFuncInstant.CopyData(this);
			return skillFuncInstant;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SkillFuncInstant skillFuncInstant = src as SkillFuncInstant;
			this.SkillFuncType = skillFuncInstant.SkillFuncType;
			this.m_bSucceeded = skillFuncInstant.m_bSucceeded;
		}

		public override bool Check(Action _action, Track _track)
		{
			return this.m_bSucceeded;
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(1);
			PoolObjHandle<ActorRoot> actorHandle2 = _action.GetActorHandle(0);
			SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			BuffSkill refParamObject2 = _action.refParams.GetRefParamObject<BuffSkill>("SkillObj");
			ResDT_SkillFunc inSkillFunc = null;
			if (refParamObject2 != null && refParamObject2.FindSkillFunc((int)this.SkillFuncType, out inSkillFunc))
			{
				SSkillFuncContext sSkillFuncContext = default(SSkillFuncContext);
				sSkillFuncContext.inTargetObj = actorHandle;
				sSkillFuncContext.inOriginator = actorHandle2;
				sSkillFuncContext.inUseContext = refParamObject;
				sSkillFuncContext.inSkillFunc = inSkillFunc;
				sSkillFuncContext.inStage = ESkillFuncStage.Enter;
				sSkillFuncContext.inAction = new PoolObjHandle<Action>(_action);
				sSkillFuncContext.inBuffSkill = new PoolObjHandle<BuffSkill>(refParamObject2);
				sSkillFuncContext.inOverlayCount = refParamObject2.GetOverlayCount();
				sSkillFuncContext.inLastEffect = false;
				if (refParamObject != null)
				{
					sSkillFuncContext.inEffectCount = refParamObject.EffectCount;
					sSkillFuncContext.inEffectCountInSingleTrigger = refParamObject.EffectCountInSingleTrigger;
					sSkillFuncContext.inMarkCount = refParamObject.MarkCount;
				}
				sSkillFuncContext.InitSkillFuncContext();
				refParamObject2.SetBuffLevel(sSkillFuncContext.iSkillLevel);
				sSkillFuncContext.LocalParams = new SSkillFuncIntParam[8];
				for (int i = 0; i < 8; i++)
				{
					sSkillFuncContext.LocalParams[i] = default(SSkillFuncIntParam);
					sSkillFuncContext.LocalParams[i].iParam = 0;
				}
				if (!Singleton<BattleLogic>.GetInstance().isRuning || Singleton<BattleLogic>.GetInstance().isGameOver)
				{
					return;
				}
				this.m_bSucceeded = Singleton<SkillFuncDelegator>.GetInstance().DoSkillFunc((int)this.SkillFuncType, ref sSkillFuncContext);
			}
			base.Process(_action, _track);
		}
	}
}
