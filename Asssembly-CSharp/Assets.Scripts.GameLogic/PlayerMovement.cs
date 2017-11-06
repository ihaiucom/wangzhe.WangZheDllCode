using Assets.Scripts.Framework;
using Pathfinding;
using Pathfinding.RVO;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class PlayerMovement : Movement
	{
		private CrypticInt32 _MaxSpeed = 6000;

		public int Acceleration = 999999;

		public int RotateSpeed = 720;

		public int DecelerateDistance = 1000;

		public int MinDecelerateSpeed;

		public int IgnoreDistance = 1;

		private VInt3 TargetLocation;

		private VInt3 Direction;

		public MoveDirectionState MoveDirState = new MoveDirectionState();

		private bool bStopMoving = true;

		private bool bRotatingLock;

		private bool bExcecuteMoving;

		private StateMachine MovingState;

		private StateMachine MovingMode;

		private bool bRotateImmediately;

		private bool bFlying;

		private bool bLerpFlying;

		public int nLerpStep;

		private uint uCmdID;

		private MPathfinding pathfinding;

		public uint m_uiNonMoveTotalTime;

		public uint m_uiMoveIntervalMax;

		public ulong m_ulLastMoveEndTime;

		public GravityMovement GravityMode;

		public override MPathfinding Pathfinding
		{
			get
			{
				return this.pathfinding;
			}
		}

		public VInt3 SelectedTargetLocation
		{
			get
			{
				return this.TargetLocation;
			}
		}

		public override VInt3 targetLocation
		{
			get
			{
				return ((MovementMode)this.MovingMode.TopState()).GetTargetPosition();
			}
		}

		public override int speed
		{
			get
			{
				return ((MovementState)this.MovingState.TopState()).GetCurrentSpeed();
			}
		}

		public override int maxSpeed
		{
			get
			{
				return this._MaxSpeed;
			}
			set
			{
				this._MaxSpeed = value;
			}
		}

		public override int rotateSpeed
		{
			get
			{
				return this.RotateSpeed;
			}
		}

		public override int acceleration
		{
			get
			{
				return this.Acceleration;
			}
		}

		public override VInt3 velocity
		{
			get
			{
				return ((MovementState)this.MovingState.TopState()).GetVelocity();
			}
		}

		public override bool isMoving
		{
			get
			{
				return ((MovementState)this.MovingState.TopState()).IsMoving();
			}
		}

		public override bool isExcuteMoving
		{
			get
			{
				return this.bExcecuteMoving;
			}
		}

		public override bool isAccelerating
		{
			get
			{
				return this.MovingState.TopState() is AccelerateMovementState;
			}
		}

		public override bool isRotatingLock
		{
			get
			{
				return this.bRotatingLock;
			}
		}

		public override bool isRotateImmediately
		{
			get
			{
				return this.bRotateImmediately;
			}
			set
			{
				this.bRotateImmediately = value;
			}
		}

		public override bool isFlying
		{
			get
			{
				return this.bFlying;
			}
			set
			{
				this.bFlying = value;
			}
		}

		public override bool isLerpFlying
		{
			get
			{
				return this.bLerpFlying;
			}
			set
			{
				this.bLerpFlying = value;
			}
		}

		public override bool isDecelerate
		{
			get
			{
				return this.MovingState.TopState() is DecelerateMovementState;
			}
		}

		public override bool isAutoMoveMode
		{
			get
			{
				return this.MovingMode.TopState() is AutoMovementMode;
			}
		}

		public override bool isDirectionalMoveMode
		{
			get
			{
				return this.MovingMode.TopState() is DirectionalMovementMode;
			}
		}

		public MovementMode movingMode
		{
			get
			{
				return this.MovingMode.TopState() as MovementMode;
			}
		}

		public VInt3 direction
		{
			get
			{
				return this.Direction;
			}
		}

		public VInt3 adjustedDirection
		{
			get
			{
				return this.Direction;
			}
		}

		public bool isStopMoving
		{
			get
			{
				return this.bStopMoving;
			}
		}

		public bool isStandOnTarget
		{
			get
			{
				return ((MovementMode)this.MovingMode.TopState()).ShouldStop();
			}
		}

		public override bool isFinished
		{
			get
			{
				return ((MovementMode)this.MovingMode.TopState()).ShouldStop();
			}
		}

		public override uint uCommandId
		{
			get
			{
				return this.uCmdID;
			}
			set
			{
				this.uCmdID = value;
			}
		}

		public override void OnUse()
		{
			base.OnUse();
			this.ResetVariables();
			this.pathfinding = null;
			this.MovingState = null;
			this.MovingMode = null;
			this.GravityMode = null;
		}

		public override void Born(ActorRoot owner)
		{
			base.Born(owner);
			this._MaxSpeed = 6000;
			this.MovingState = new StateMachine();
			this.MovingState.RegisterState<IdleMovmentState>(new IdleMovmentState(this), "IdleMovmentState");
			this.MovingState.RegisterState<AccelerateMovementState>(new AccelerateMovementState(this), "AccelerateMovementState");
			this.MovingState.RegisterState<UniformMovementState>(new UniformMovementState(this), "UniformMovementState");
			this.MovingState.RegisterState<DecelerateMovementState>(new DecelerateMovementState(this), "DecelerateMovementState");
			this.MovingState.ChangeState("IdleMovmentState");
			this.MovingMode = new StateMachine();
			this.MovingMode.RegisterState<AutoMovementMode>(new AutoMovementMode(this), "AutoMovementMode");
			this.MovingMode.RegisterState<DirectionalMovementMode>(new DirectionalMovementMode(this), "DirectionalMovementMode");
			this.MovingMode.RegisterState<HoldonMovementMode>(new HoldonMovementMode(this), "HoldonMovementMode");
			this.MovingMode.ChangeState("HoldonMovementMode");
			this.GravityMode = new GravityMovement(this);
			this.CreateNavSearchAgent();
		}

		public override void Init()
		{
			base.Init();
			this.GravityMode.Init();
			if (AstarPath.active != null)
			{
				this.pathfinding = new MPathfinding();
				if (!this.pathfinding.Init(this.actorPtr))
				{
					this.pathfinding = null;
				}
			}
		}

		private void ResetVariables()
		{
			this.Acceleration = 999999;
			this._MaxSpeed = 6000;
			this.RotateSpeed = 720;
			this.DecelerateDistance = 1000;
			this.IgnoreDistance = 1;
			this.MinDecelerateSpeed = 0;
			this.bStopMoving = true;
			this.bExcecuteMoving = false;
			this.bRotatingLock = false;
			this.bRotateImmediately = false;
			this.bFlying = false;
			this.bLerpFlying = false;
			this.nLerpStep = 0;
			this.TargetLocation = VInt3.zero;
			this.Direction = VInt3.zero;
			this.MoveDirState.Reset();
			this.uCmdID = 0u;
			this.m_uiNonMoveTotalTime = 0u;
			this.m_ulLastMoveEndTime = 0uL;
			this.m_uiMoveIntervalMax = 0u;
		}

		public override void Fight()
		{
			base.Fight();
			this.m_ulLastMoveEndTime = Singleton<FrameSynchr>.instance.LogicFrameTick;
		}

		public override void Deactive()
		{
			this.ResetVariables();
			this.MovingState.ChangeState("IdleMovmentState");
			this.MovingMode.ChangeState("HoldonMovementMode");
			base.Deactive();
		}

		public override void Reactive()
		{
			base.Reactive();
			if (this.GravityMode != null)
			{
				this.GravityMode.Reset();
			}
			if (this.pathfinding != null)
			{
				this.pathfinding.Reset();
				if (this.pathfinding.rvo)
				{
					this.pathfinding.rvo.maxSpeed = this.maxSpeed;
				}
			}
		}

		private void CreateNavSearchAgent()
		{
			if (!this.actor.isMovable)
			{
				return;
			}
			Seeker seeker = base.gameObject.GetComponent<Seeker>();
			if (!seeker)
			{
				seeker = base.gameObject.AddComponent<Seeker>();
			}
			FunnelModifier funnelModifier = base.gameObject.GetComponent<FunnelModifier>();
			if (!funnelModifier)
			{
				funnelModifier = base.gameObject.AddComponent<FunnelModifier>();
			}
			seeker.startEndModifier.Priority = 3;
			funnelModifier.Priority = 2;
			if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster || this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				RVOController rVOController = base.gameObject.GetComponent<RVOController>();
				if (!rVOController)
				{
					rVOController = base.gameObject.AddComponent<RVOController>();
				}
				rVOController.maxSpeed = this.maxSpeed;
				rVOController.enabled = true;
				rVOController.EnsureActorAndSimulator();
			}
		}

		public override void GravityModeLerp(uint nDelta, bool bReset)
		{
			if (this.GravityMode != null)
			{
				this.GravityMode.GravityMoveLerp((int)nDelta, bReset);
			}
		}

		public override void SetRotate(VInt3 InDirection, bool bInRotateImmediately)
		{
			if (InDirection == VInt3.zero || this.actor.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_MoveRotate))
			{
				return;
			}
			InDirection = InDirection.NormalizeTo(1000);
			this.actor.forward = InDirection;
			this.actor.ObjLinker.SetForward(InDirection, -1);
			this.actor.rotation = Quaternion.LookRotation((Vector3)InDirection);
			this.bRotateImmediately = bInRotateImmediately;
			this.bRotatingLock = true;
		}

		public override void SetMoveParam(VInt3 InVector, bool bDirection, bool bInRotateImmediately, uint cmdId = 0u)
		{
			this.uCommandId = cmdId;
			this.bRotateImmediately = bInRotateImmediately;
			if (bDirection)
			{
				this.Direction = InVector;
				this.Direction.NormalizeTo(1000);
				this.MoveDirState.SetNewDirection(ref this.Direction);
				if (this.pathfinding != null)
				{
					this.pathfinding.enabled = false;
					this.pathfinding.InvalidPath();
				}
				if (!this.isDirectionalMoveMode)
				{
					this.MovingMode.ChangeState("DirectionalMovementMode");
				}
				((MovementState)this.MovingState.TopState()).ChangeDirection();
			}
			else
			{
				this.TargetLocation = InVector;
				if (Math.Abs(this.TargetLocation.x) > 80000 || Math.Abs(this.TargetLocation.z) > 80000)
				{
				}
				this.Direction = InVector - this.actor.location;
				this.Direction.NormalizeTo(1000);
				this.MoveDirState.SetNewDirection(ref this.Direction);
				if (this.pathfinding != null)
				{
					this.pathfinding.enabled = true;
					this.pathfinding.SearchPath(this.TargetLocation);
				}
				if (!this.isAutoMoveMode)
				{
					this.MovingMode.ChangeState("AutoMovementMode");
				}
				((MovementState)this.MovingState.TopState()).ChangeTarget();
			}
		}

		public override void ExcuteMove()
		{
			this.bExcecuteMoving = true;
			this.bRotatingLock = false;
			this.bStopMoving = false;
			ulong logicFrameTick = Singleton<FrameSynchr>.instance.LogicFrameTick;
			uint num = (uint)(logicFrameTick - this.m_ulLastMoveEndTime);
			this.m_uiMoveIntervalMax = ((this.m_uiMoveIntervalMax > num) ? this.m_uiMoveIntervalMax : num);
			this.m_uiNonMoveTotalTime += num;
		}

		public override void StopMove()
		{
			this.bStopMoving = true;
			this.bExcecuteMoving = false;
			this.TargetLocation = this.actor.location;
			if (this.pathfinding != null)
			{
				this.pathfinding.StopMove();
			}
			this.MovingMode.ChangeState("HoldonMovementMode");
			this.GotoState("IdleMovmentState");
			this.m_ulLastMoveEndTime = Singleton<FrameSynchr>.instance.LogicFrameTick;
		}

		public override float GetDistance(uint nDelta)
		{
			return (float)((long)this.maxSpeed * (long)((ulong)nDelta) / 1000L) * 0.001f;
		}

		public void GotoState(string InStateName)
		{
			this.MovingState.ChangeState(InStateName);
		}

		public override void UpdateLogic(int delta)
		{
			if (this.pathfinding != null)
			{
				this.pathfinding.UpdateLogic(delta);
			}
			if (this.bExcecuteMoving || this.bRotateImmediately)
			{
				((MovementState)this.MovingState.TopState()).UpdateLogic(delta);
			}
			this.GravityMode.Move(delta);
		}
	}
}
