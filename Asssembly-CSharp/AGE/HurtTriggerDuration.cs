using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	internal class HurtTriggerDuration : DurationCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetID;

		public int iTriggerInterval;

		public int iMaxTriggerCount;

		[AssetReference(AssetRefType.SkillCombine)]
		public int iTriggerSkillCombineID;

		public SkillSlotType slotType;

		private PoolObjHandle<ActorRoot> targetActor;

		private bool bFirstTrigger;

		private int iLastTime;

		private int iLocalTime;

		private int iTriggerCount;

		private SkillUseContext context;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			HurtTriggerDuration hurtTriggerDuration = src as HurtTriggerDuration;
			this.targetID = hurtTriggerDuration.targetID;
			this.iTriggerInterval = hurtTriggerDuration.iTriggerInterval;
			this.iTriggerSkillCombineID = hurtTriggerDuration.iTriggerSkillCombineID;
			this.slotType = hurtTriggerDuration.slotType;
			this.iMaxTriggerCount = hurtTriggerDuration.iMaxTriggerCount;
		}

		public override BaseEvent Clone()
		{
			HurtTriggerDuration hurtTriggerDuration = ClassObjPool<HurtTriggerDuration>.Get();
			hurtTriggerDuration.CopyData(this);
			return hurtTriggerDuration;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetID = 0;
			this.iTriggerSkillCombineID = 0;
			this.targetActor.Release();
			this.bFirstTrigger = false;
			this.iLastTime = 0;
			this.iLocalTime = 0;
			this.iTriggerCount = 0;
			this.context = null;
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			this.targetActor = _action.GetActorHandle(this.targetID);
			this.bFirstTrigger = true;
			this.iLastTime = 0;
			this.iLocalTime = 0;
			this.iTriggerCount = 0;
			this.context = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			base.Process(_action, _track, _localTime);
			this.iLocalTime = _localTime;
		}

		public override void Leave(Action _action, Track _track)
		{
			base.Leave(_action, _track);
			this.context = null;
			Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
		}

		private void OnActorDamage(ref HurtEventResultInfo info)
		{
			if (this.iMaxTriggerCount > 0 && this.iTriggerCount >= this.iMaxTriggerCount)
			{
				return;
			}
			if (info.src != this.targetActor)
			{
				return;
			}
			if (info.hpChanged >= 0)
			{
				return;
			}
			if (this.slotType != info.hurtInfo.atkSlot)
			{
				return;
			}
			if (this.bFirstTrigger || this.iLocalTime - this.iLastTime > this.iTriggerInterval)
			{
				this.iLastTime = this.iLastTime;
				this.iTriggerCount++;
				this.TriggerAction();
			}
		}

		private void TriggerAction()
		{
			if (!this.targetActor)
			{
				return;
			}
			if (this.context == null)
			{
				return;
			}
			if (this.iTriggerSkillCombineID > 0)
			{
				this.targetActor.handle.SkillControl.SpawnBuff(this.targetActor, this.context, this.iTriggerSkillCombineID, false);
			}
		}
	}
}
