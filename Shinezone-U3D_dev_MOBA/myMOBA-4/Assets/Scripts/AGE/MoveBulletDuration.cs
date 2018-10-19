using Assets.Scripts.Common;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class MoveBulletDuration : DurationCondition
	{
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

		public bool bResetMoveDistance;

		public bool bMoveOnXAxis;

		public int distanceZ0;

		public int distanceZ1;

		public int distanceX;

		public int rotateBodyDegreeSpeed;

		public int rotateBodyRadius;

		public int rotateBodyHeight = 1200;

		public int rotateBodyFindEnemyLatency = -1;

		public int rotateBodyFindEnemyRadius = -1;

		public int rotateBodyFindEnemyCd;

		public int rotateBodyBulletCount;

		public bool bFindTargetByRotateBodyBullet;

		private MoveBulletDurationContext Context = new MoveBulletDurationContext();

		public override bool SupportEditMode()
		{
			return true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			MoveBulletDuration moveBulletDuration = src as MoveBulletDuration;
			this.targetId = moveBulletDuration.targetId;
			this.destId = moveBulletDuration.destId;
			this.MoveType = moveBulletDuration.MoveType;
			this.targetPosition = moveBulletDuration.targetPosition;
			this.offsetDir = moveBulletDuration.offsetDir;
			this.velocity = moveBulletDuration.velocity;
			this.distance = moveBulletDuration.distance;
			this.gravity = moveBulletDuration.gravity;
			this.bMoveRotate = moveBulletDuration.bMoveRotate;
			this.bAdjustSpeed = moveBulletDuration.bAdjustSpeed;
			this.acceleration = moveBulletDuration.acceleration;
			this.bBulletUseDir = moveBulletDuration.bBulletUseDir;
			this.bUseIndicatorDir = moveBulletDuration.bUseIndicatorDir;
			this.bReachDestStop = moveBulletDuration.bReachDestStop;
			this.bResetMoveDistance = moveBulletDuration.bResetMoveDistance;
			this.bMoveOnXAxis = moveBulletDuration.bMoveOnXAxis;
			this.rotateBodyDegreeSpeed = moveBulletDuration.rotateBodyDegreeSpeed;
			this.rotateBodyRadius = moveBulletDuration.rotateBodyRadius;
			this.rotateBodyHeight = moveBulletDuration.rotateBodyHeight;
			this.rotateBodyFindEnemyLatency = moveBulletDuration.rotateBodyFindEnemyLatency;
			this.rotateBodyFindEnemyRadius = moveBulletDuration.rotateBodyFindEnemyRadius;
			this.rotateBodyFindEnemyCd = moveBulletDuration.rotateBodyFindEnemyCd;
			this.rotateBodyBulletCount = moveBulletDuration.rotateBodyBulletCount;
			this.bFindTargetByRotateBodyBullet = moveBulletDuration.bFindTargetByRotateBodyBullet;
			this.Context.CopyData(ref moveBulletDuration.Context);
		}

		public override BaseEvent Clone()
		{
			MoveBulletDuration moveBulletDuration = ClassObjPool<MoveBulletDuration>.Get();
			moveBulletDuration.CopyData(this);
			return moveBulletDuration;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.offsetDir = VInt3.zero;
		}

		public override void Enter(Action _action, Track _track)
		{
			this.Context.Reset(this);
			this.Context.Enter(_action);
			base.Enter(_action, _track);
		}

		public override void Leave(Action _action, Track _track)
		{
			base.Leave(_action, _track);
			this.Context.Leave(_action, _track);
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			this.Context.Process(_action, _track, _localTime);
			base.Process(_action, _track, _localTime);
		}

		public override bool Check(Action _action, Track _track)
		{
			return this.Context.stopCondtion;
		}
	}
}
