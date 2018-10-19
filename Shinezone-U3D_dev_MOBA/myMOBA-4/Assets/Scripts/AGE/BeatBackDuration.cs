using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class BeatBackDuration : DurationCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int attackerId;

		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		public VInt3 attackerPos = VInt3.zero;

		public int initSpeed = 1000;

		public int accelerateSpeed = 1000;

		public bool enableRotate = true;

		public int rotationTime;

		public BeatBackDirType dirType;

		public int atteDistance;

		public BeatBackCheckType checkType = BeatBackCheckType.Done;

		private bool done_;

		private VInt3 moveDirection = VInt3.zero;

		private Quaternion fromRot = Quaternion.identity;

		private Quaternion toRot = Quaternion.identity;

		private int lastTime_;

		private PoolObjHandle<ActorRoot> actor_;

		private AccelerateMotionControler motionControler;

		private bool bHitNavEdge;

		private bool bMoveEnd;

		public override void OnUse()
		{
			base.OnUse();
			this.attackerId = 0;
			this.targetId = 0;
			this.attackerPos = VInt3.zero;
			this.initSpeed = 1000;
			this.accelerateSpeed = 1000;
			this.enableRotate = true;
			this.rotationTime = 0;
			this.dirType = BeatBackDirType.Position;
			this.atteDistance = 0;
			this.done_ = false;
			this.moveDirection = VInt3.zero;
			this.fromRot = Quaternion.identity;
			this.toRot = Quaternion.identity;
			this.lastTime_ = 0;
			this.bMoveEnd = false;
			this.bHitNavEdge = false;
			this.checkType = BeatBackCheckType.Done;
			this.motionControler = null;
		}

		public override BaseEvent Clone()
		{
			BeatBackDuration beatBackDuration = ClassObjPool<BeatBackDuration>.Get();
			beatBackDuration.CopyData(this);
			return beatBackDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			BeatBackDuration beatBackDuration = src as BeatBackDuration;
			this.attackerId = beatBackDuration.attackerId;
			this.targetId = beatBackDuration.targetId;
			this.attackerPos = beatBackDuration.attackerPos;
			this.initSpeed = beatBackDuration.initSpeed;
			this.accelerateSpeed = beatBackDuration.accelerateSpeed;
			this.enableRotate = beatBackDuration.enableRotate;
			this.rotationTime = beatBackDuration.rotationTime;
			this.dirType = beatBackDuration.dirType;
			this.atteDistance = beatBackDuration.atteDistance;
			this.done_ = beatBackDuration.done_;
			this.moveDirection = beatBackDuration.moveDirection;
			this.fromRot = beatBackDuration.fromRot;
			this.toRot = beatBackDuration.toRot;
			this.lastTime_ = beatBackDuration.lastTime_;
			this.bMoveEnd = beatBackDuration.bMoveEnd;
			this.bHitNavEdge = beatBackDuration.bHitNavEdge;
			this.checkType = beatBackDuration.checkType;
		}

		public override void Enter(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = new PoolObjHandle<ActorRoot>(null);
			this.done_ = false;
			this.lastTime_ = 0;
			base.Enter(_action, _track);
			VInt3 location = this.attackerPos;
			if (this.attackerId != -1)
			{
				GameObject gameObject = _action.GetGameObject(this.attackerId);
				if (gameObject == null)
				{
					return;
				}
				actorHandle = _action.GetActorHandle(this.attackerId);
				if (!actorHandle)
				{
					return;
				}
				location = actorHandle.handle.location;
			}
			this.actor_ = _action.GetActorHandle(this.targetId);
			if (!this.actor_)
			{
				return;
			}
			if (!this.actor_.handle.isMovable)
			{
				this.actor_.Release();
				this.done_ = true;
				return;
			}
			if (this.dirType == BeatBackDirType.Position)
			{
				VInt3 vInt = this.actor_.handle.location - location;
				vInt.y = 0;
				this.moveDirection = vInt.NormalizeTo(1000);
			}
			else if (this.dirType == BeatBackDirType.Directional)
			{
				if (!actorHandle)
				{
					this.done_ = true;
					return;
				}
				this.moveDirection = actorHandle.handle.forward;
			}
			if (this.enableRotate)
			{
				this.fromRot = this.actor_.handle.rotation;
				this.actor_.handle.MovementComponent.SetRotate(-this.moveDirection, true);
				if (this.rotationTime > 0)
				{
					this.toRot = Quaternion.LookRotation((Vector3)this.actor_.handle.forward);
				}
				else
				{
					this.actor_.handle.rotation = Quaternion.LookRotation((Vector3)this.actor_.handle.forward);
				}
			}
			int motionSpeed = this.initSpeed;
			this.motionControler = new AccelerateMotionControler();
			if (this.atteDistance > 0)
			{
				VInt vInt2 = (this.actor_.handle.location - location).magnitude2D;
				if (vInt2.i > this.atteDistance)
				{
					motionSpeed = 0;
				}
				else
				{
					motionSpeed = (this.atteDistance - vInt2.i) * this.initSpeed / this.atteDistance;
				}
			}
			this.motionControler.InitMotionControler(motionSpeed, this.accelerateSpeed);
			this.actor_.handle.ObjLinker.AddCustomMoveLerp(new CustomMoveLerpFunc(this.ActionMoveLerp));
		}

		private void ActionMoveLerp(ActorRoot actor, uint nDelta, bool bReset)
		{
			if (actor == null || this.done_)
			{
				return;
			}
			int motionLerpDistance = this.motionControler.GetMotionLerpDistance((int)nDelta);
			VInt3 delta = this.moveDirection * motionLerpDistance / 1000f;
			Vector3 vector = actor.myTransform.position;
			VInt vInt;
			VInt3 ob = PathfindingUtility.MoveLerp(actor, (VInt3)vector, delta, out vInt);
			if (actor.MovementComponent.isFlying)
			{
				float y = vector.y;
				vector += (Vector3)ob;
				Vector3 position = vector;
				position.y = y;
				actor.myTransform.position = position;
			}
			else
			{
				actor.myTransform.position += (Vector3)ob;
			}
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			if (!this.actor_)
			{
				return;
			}
			bool flag = this.lastTime_ < this.rotationTime;
			int deltaTime = _localTime - this.lastTime_;
			this.lastTime_ = _localTime;
			if (flag && this.enableRotate)
			{
				float t = Mathf.Min(1f, (float)(_localTime / this.rotationTime));
				Quaternion rotation = Quaternion.Slerp(this.fromRot, this.toRot, t);
				this.actor_.handle.rotation = rotation;
			}
			if (this.done_)
			{
				return;
			}
			int motionDeltaDistance = this.motionControler.GetMotionDeltaDistance(deltaTime);
			VInt3 delta = this.moveDirection * motionDeltaDistance / 1000f;
			VInt groundY;
			VInt3 rhs = PathfindingUtility.Move(this.actor_.handle, delta, out groundY, null);
			if (this.actor_.handle.MovementComponent.isFlying)
			{
				int y = this.actor_.handle.location.y;
				this.actor_.handle.location += rhs;
				VInt3 location = this.actor_.handle.location;
				location.y = y;
				this.actor_.handle.location = location;
			}
			else
			{
				this.actor_.handle.location += rhs;
			}
			this.actor_.handle.groundY = groundY;
			if (delta.x != rhs.x || delta.z != rhs.z)
			{
				this.done_ = true;
				this.bHitNavEdge = true;
			}
			base.Process(_action, _track, _localTime);
		}

		public override void Leave(Action _action, Track _track)
		{
			if (!this.actor_)
			{
				return;
			}
			this.actor_.handle.ObjLinker.RmvCustomMoveLerp(new CustomMoveLerpFunc(this.ActionMoveLerp));
			this.done_ = true;
			this.bMoveEnd = true;
		}

		public override bool Check(Action _action, Track _track)
		{
			switch (this.checkType)
			{
			case BeatBackCheckType.Hit:
				return this.bHitNavEdge;
			case BeatBackCheckType.Move:
				return this.bMoveEnd;
			case BeatBackCheckType.Done:
				return this.done_;
			default:
				return this.done_;
			}
		}
	}
}
