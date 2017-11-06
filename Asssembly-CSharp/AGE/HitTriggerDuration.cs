using Assets.Scripts.Common;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class HitTriggerDuration : DurationCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int triggerId;

		[ObjectTemplate(new Type[]
		{

		})]
		public int attackerId;

		public int triggerInterval = 30;

		public bool bFilterEnemy;

		public bool bFilterFriend = true;

		public bool bFilterHero;

		public bool bFileterMonter;

		public bool bFileterOrgan;

		public bool bFilterEye = true;

		public bool bFilterMyself = true;

		public bool bFilterDead = true;

		public bool bFilterDeadControlHero = true;

		public bool bFilterCurrentTarget;

		public bool bFilterMoveDirection;

		public int Angle = -1;

		public int TriggerActorCount = -1;

		public HitTriggerSelectMode SelectMode;

		public int TriggerActorInterval = 30;

		public int CollideMaxCount = -1;

		public bool bEdgeCheck;

		public bool bExtraBuff;

		[AssetReference(AssetRefType.SkillCombine)]
		public int SelfSkillCombineID_1;

		[AssetReference(AssetRefType.SkillCombine)]
		public int SelfSkillCombineID_2;

		[AssetReference(AssetRefType.SkillCombine)]
		public int SelfSkillCombineID_3;

		[AssetReference(AssetRefType.SkillCombine)]
		public int TargetSkillCombine_1;

		[AssetReference(AssetRefType.SkillCombine)]
		public int TargetSkillCombine_2;

		[AssetReference(AssetRefType.SkillCombine)]
		public int TargetSkillCombine_3;

		public bool TargetSkillLeaveRemove_1;

		public bool TargetSkillLeaveRemove_2;

		public bool TargetSkillLeaveRemove_3;

		public bool bTriggerBullet;

		[AssetReference(AssetRefType.Action)]
		public string BulletActionName;

		public bool bAgeImmeExcute;

		public bool bUseTriggerObj = true;

		public bool bCheckSight;

		private HitTriggerDurationContext Context = new HitTriggerDurationContext();

		public bool bTriggerMode;

		public bool bTriggerBounceBullet;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			HitTriggerDuration hitTriggerDuration = src as HitTriggerDuration;
			this.triggerId = hitTriggerDuration.triggerId;
			this.attackerId = hitTriggerDuration.attackerId;
			this.triggerInterval = hitTriggerDuration.triggerInterval;
			this.bFilterEnemy = hitTriggerDuration.bFilterEnemy;
			this.bFilterFriend = hitTriggerDuration.bFilterFriend;
			this.bFilterHero = hitTriggerDuration.bFilterHero;
			this.bFileterMonter = hitTriggerDuration.bFileterMonter;
			this.bFileterOrgan = hitTriggerDuration.bFileterOrgan;
			this.bFilterEye = hitTriggerDuration.bFilterEye;
			this.bFilterMyself = hitTriggerDuration.bFilterMyself;
			this.bFilterDead = hitTriggerDuration.bFilterDead;
			this.bFilterDeadControlHero = hitTriggerDuration.bFilterDeadControlHero;
			this.bFilterCurrentTarget = hitTriggerDuration.bFilterCurrentTarget;
			this.bFilterMoveDirection = hitTriggerDuration.bFilterMoveDirection;
			this.Angle = hitTriggerDuration.Angle;
			this.TriggerActorCount = hitTriggerDuration.TriggerActorCount;
			this.SelectMode = hitTriggerDuration.SelectMode;
			this.TriggerActorInterval = hitTriggerDuration.TriggerActorInterval;
			this.CollideMaxCount = hitTriggerDuration.CollideMaxCount;
			this.bEdgeCheck = hitTriggerDuration.bEdgeCheck;
			this.bExtraBuff = hitTriggerDuration.bExtraBuff;
			this.SelfSkillCombineID_1 = hitTriggerDuration.SelfSkillCombineID_1;
			this.SelfSkillCombineID_2 = hitTriggerDuration.SelfSkillCombineID_2;
			this.SelfSkillCombineID_3 = hitTriggerDuration.SelfSkillCombineID_3;
			this.TargetSkillCombine_1 = hitTriggerDuration.TargetSkillCombine_1;
			this.TargetSkillCombine_2 = hitTriggerDuration.TargetSkillCombine_2;
			this.TargetSkillCombine_3 = hitTriggerDuration.TargetSkillCombine_3;
			this.bTriggerBullet = hitTriggerDuration.bTriggerBullet;
			this.BulletActionName = hitTriggerDuration.BulletActionName;
			this.bAgeImmeExcute = hitTriggerDuration.bAgeImmeExcute;
			this.bUseTriggerObj = hitTriggerDuration.bUseTriggerObj;
			this.bCheckSight = hitTriggerDuration.bCheckSight;
			this.bTriggerMode = hitTriggerDuration.bTriggerMode;
			this.bTriggerBounceBullet = hitTriggerDuration.bTriggerBounceBullet;
			this.TargetSkillLeaveRemove_1 = hitTriggerDuration.TargetSkillLeaveRemove_1;
			this.TargetSkillLeaveRemove_2 = hitTriggerDuration.TargetSkillLeaveRemove_2;
			this.TargetSkillLeaveRemove_3 = hitTriggerDuration.TargetSkillLeaveRemove_3;
			this.Context.CopyData(ref hitTriggerDuration.Context);
		}

		public override BaseEvent Clone()
		{
			HitTriggerDuration hitTriggerDuration = ClassObjPool<HitTriggerDuration>.Get();
			hitTriggerDuration.CopyData(this);
			return hitTriggerDuration;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.bEdgeCheck = false;
			this.bTriggerMode = false;
			this.bTriggerBounceBullet = false;
			this.bFilterEye = true;
			this.bFilterMoveDirection = false;
			this.Angle = -1;
			this.Context.OnUse();
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			this.Context.Reset(this);
			this.Context.Enter(_action, _track);
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			this.Context.Process(_action, _track, _localTime);
			base.Process(_action, _track, _localTime);
		}

		public override void Leave(Action _action, Track _track)
		{
			base.Leave(_action, _track);
			this.Context.Leave(_action, _track);
		}

		public override bool Check(Action _action, Track _track)
		{
			return this.Context.hit;
		}
	}
}
