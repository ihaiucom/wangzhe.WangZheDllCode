using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class LeaveTriggerDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int TargetID;

		[AssetReference(AssetRefType.SkillCombine)]
		public int TargetSkillCombine_1;

		[AssetReference(AssetRefType.SkillCombine)]
		public int TargetSkillCombine_2;

		[AssetReference(AssetRefType.SkillCombine)]
		public int TargetSkillCombine_3;

		private PoolObjHandle<ActorRoot> targetActor;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			LeaveTriggerDuration leaveTriggerDuration = src as LeaveTriggerDuration;
			this.TargetID = leaveTriggerDuration.TargetID;
			this.TargetSkillCombine_1 = leaveTriggerDuration.TargetSkillCombine_1;
			this.TargetSkillCombine_2 = leaveTriggerDuration.TargetSkillCombine_2;
			this.TargetSkillCombine_3 = leaveTriggerDuration.TargetSkillCombine_3;
			this.targetActor = leaveTriggerDuration.targetActor;
		}

		public override BaseEvent Clone()
		{
			LeaveTriggerDuration leaveTriggerDuration = ClassObjPool<LeaveTriggerDuration>.Get();
			leaveTriggerDuration.CopyData(this);
			return leaveTriggerDuration;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.TargetID = 0;
			this.TargetSkillCombine_1 = 0;
			this.TargetSkillCombine_2 = 0;
			this.TargetSkillCombine_3 = 0;
			this.targetActor.Release();
		}

		public override void Enter(Action _action, Track _track)
		{
			this.targetActor = _action.GetActorHandle(this.TargetID);
			base.Enter(_action, _track);
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			base.Process(_action, _track, _localTime);
		}

		public override void Leave(Action _action, Track _track)
		{
			SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			this.targetActor.handle.SkillControl.SpawnBuff(refParamObject.Originator, refParamObject, this.TargetSkillCombine_1, false);
			this.targetActor.handle.SkillControl.SpawnBuff(refParamObject.Originator, refParamObject, this.TargetSkillCombine_2, false);
			this.targetActor.handle.SkillControl.SpawnBuff(refParamObject.Originator, refParamObject, this.TargetSkillCombine_3, false);
			base.Leave(_action, _track);
		}
	}
}
