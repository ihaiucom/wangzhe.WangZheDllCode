using Assets.Scripts.Common;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class BulletTriggerDuration : DurationCondition
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

		public bool bFilterMyself = true;

		public bool bFilterDead = true;

		public bool bMoveOnXAxis;

		public int distanceZ0;

		public int distanceZ1;

		public int distanceX;

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

		public bool bTriggerBullet;

		[AssetReference(AssetRefType.Action)]
		public string BulletActionName;

		public bool bAgeImmeExcute;

		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		[ObjectTemplate(new Type[]
		{

		})]
		public int destId = -1;

		public ActorMoveType MoveType;

		public VInt3 targetPosition;

		public VInt3 offsetDir = VInt3.zero;

		public int velocity = 15000;

		public int acceleration;

		public int distance = 50000;

		public int gravity;

		public bool bMoveRotate = true;

		public bool bAdjustSpeed;

		public bool bBulletUseDir;

		public bool bUseIndicatorDir;

		public bool bReachDestStop = true;

		private HitTriggerDurationContext HitTriggerContext = new HitTriggerDurationContext();

		private MoveBulletDurationContext MoveBulletContext = new MoveBulletDurationContext();

		public EDependCheckType DependCheckType;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			BulletTriggerDuration bulletTriggerDuration = src as BulletTriggerDuration;
			this.triggerId = bulletTriggerDuration.triggerId;
			this.attackerId = bulletTriggerDuration.attackerId;
			this.triggerInterval = bulletTriggerDuration.triggerInterval;
			this.bFilterEnemy = bulletTriggerDuration.bFilterEnemy;
			this.bFilterFriend = bulletTriggerDuration.bFilterFriend;
			this.bFilterHero = bulletTriggerDuration.bFilterHero;
			this.bFileterMonter = bulletTriggerDuration.bFileterMonter;
			this.bFileterOrgan = bulletTriggerDuration.bFileterOrgan;
			this.bFilterMyself = bulletTriggerDuration.bFilterMyself;
			this.bFilterDead = bulletTriggerDuration.bFilterDead;
			this.TriggerActorCount = bulletTriggerDuration.TriggerActorCount;
			this.SelectMode = bulletTriggerDuration.SelectMode;
			this.TriggerActorInterval = bulletTriggerDuration.TriggerActorInterval;
			this.CollideMaxCount = bulletTriggerDuration.CollideMaxCount;
			this.bEdgeCheck = bulletTriggerDuration.bEdgeCheck;
			this.bExtraBuff = bulletTriggerDuration.bExtraBuff;
			this.SelfSkillCombineID_1 = bulletTriggerDuration.SelfSkillCombineID_1;
			this.SelfSkillCombineID_2 = bulletTriggerDuration.SelfSkillCombineID_2;
			this.SelfSkillCombineID_3 = bulletTriggerDuration.SelfSkillCombineID_3;
			this.TargetSkillCombine_1 = bulletTriggerDuration.TargetSkillCombine_1;
			this.TargetSkillCombine_2 = bulletTriggerDuration.TargetSkillCombine_2;
			this.TargetSkillCombine_3 = bulletTriggerDuration.TargetSkillCombine_3;
			this.bTriggerBullet = bulletTriggerDuration.bTriggerBullet;
			this.BulletActionName = bulletTriggerDuration.BulletActionName;
			this.targetId = bulletTriggerDuration.targetId;
			this.destId = bulletTriggerDuration.destId;
			this.MoveType = bulletTriggerDuration.MoveType;
			this.targetPosition = bulletTriggerDuration.targetPosition;
			this.offsetDir = bulletTriggerDuration.offsetDir;
			this.velocity = bulletTriggerDuration.velocity;
			this.distance = bulletTriggerDuration.distance;
			this.gravity = bulletTriggerDuration.gravity;
			this.bMoveRotate = bulletTriggerDuration.bMoveRotate;
			this.bAdjustSpeed = bulletTriggerDuration.bAdjustSpeed;
			this.bBulletUseDir = bulletTriggerDuration.bBulletUseDir;
			this.bUseIndicatorDir = bulletTriggerDuration.bUseIndicatorDir;
			this.bReachDestStop = bulletTriggerDuration.bReachDestStop;
			this.acceleration = bulletTriggerDuration.acceleration;
			this.DependCheckType = bulletTriggerDuration.DependCheckType;
			this.bMoveOnXAxis = bulletTriggerDuration.bMoveOnXAxis;
			this.distanceZ0 = bulletTriggerDuration.distanceZ0;
			this.distanceZ1 = bulletTriggerDuration.distanceZ1;
			this.distanceX = bulletTriggerDuration.distanceX;
			this.HitTriggerContext.CopyData(ref bulletTriggerDuration.HitTriggerContext);
			this.MoveBulletContext.CopyData(ref bulletTriggerDuration.MoveBulletContext);
		}

		public override BaseEvent Clone()
		{
			BulletTriggerDuration bulletTriggerDuration = ClassObjPool<BulletTriggerDuration>.Get();
			bulletTriggerDuration.CopyData(this);
			return bulletTriggerDuration;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.HitTriggerContext.OnUse();
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			this.MoveBulletContext.Reset(this);
			this.HitTriggerContext.Reset(this);
			this.MoveBulletContext.Enter(_action);
			this.HitTriggerContext.Enter(_action, _track);
		}

		private bool ShouldStop()
		{
			bool flag = false;
			if (this.DependCheckType == EDependCheckType.Hit)
			{
				flag = this.HitTriggerContext.hit;
			}
			else if (this.DependCheckType == EDependCheckType.Move)
			{
				flag = this.MoveBulletContext.stopCondtion;
			}
			else if (this.DependCheckType == EDependCheckType.HitOrMove)
			{
				flag = (this.HitTriggerContext.hit || this.MoveBulletContext.stopCondtion);
			}
			else if (this.DependCheckType == EDependCheckType.HitAndMove)
			{
				flag = (this.HitTriggerContext.hit && this.MoveBulletContext.stopCondtion);
			}
			if (flag)
			{
				this.MoveBulletContext.stopLerpCondtion = true;
			}
			return flag;
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			int num = 2;
			while (!this.ShouldStop() && this.MoveBulletContext.ProcessSubdivide(_action, _track, _localTime, num--) > 0)
			{
				this.HitTriggerContext.Process(_action, _track, this.MoveBulletContext.lastTime);
			}
			base.Process(_action, _track, _localTime);
		}

		public override void Leave(Action _action, Track _track)
		{
			base.Leave(_action, _track);
			this.MoveBulletContext.Leave(_action, _track);
		}

		public override bool Check(Action _action, Track _track)
		{
			switch (this.DependCheckType)
			{
			case EDependCheckType.Hit:
				return this.HitTriggerContext.hit;
			case EDependCheckType.Move:
				return this.MoveBulletContext.stopCondtion;
			case EDependCheckType.HitAndMove:
				return this.HitTriggerContext.hit && this.MoveBulletContext.stopCondtion;
			case EDependCheckType.HitOrMove:
				return this.HitTriggerContext.hit || this.MoveBulletContext.stopCondtion;
			default:
				return this.HitTriggerContext.hit;
			}
		}
	}
}
