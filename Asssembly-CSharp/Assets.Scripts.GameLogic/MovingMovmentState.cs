using System;

namespace Assets.Scripts.GameLogic
{
	internal abstract class MovingMovmentState : MovementState
	{
		private CrypticInt32 _CurrentSpeed;

		public int CurrentSpeed
		{
			get
			{
				return this._CurrentSpeed;
			}
			set
			{
				this._CurrentSpeed = value;
			}
		}

		public MovingMovmentState(PlayerMovement InParent) : base(InParent)
		{
			this._CurrentSpeed = 0;
		}

		public override void OnStateLeave()
		{
			this._CurrentSpeed = 0;
		}

		public override int GetCurrentSpeed()
		{
			return this._CurrentSpeed;
		}

		public override bool IsMoving()
		{
			return true;
		}

		public override void UpdateLogic(int nDelta)
		{
			if (this.Parent.isExcuteMoving)
			{
				this.MoveToTarget(nDelta);
			}
			else
			{
				base.UpdateLogic(nDelta);
			}
		}

		protected virtual void MoveToTarget(int delta)
		{
			this.CalcSpeed(delta);
			MPathfinding pathfinding = this.Parent.Pathfinding;
			if (pathfinding != null && pathfinding.enabled)
			{
				pathfinding.speed = this.CurrentSpeed;
				VInt3 forward = VInt3.forward;
				pathfinding.Move(out forward, delta);
				base.UpdateRotateDir(forward, delta);
			}
			else
			{
				VInt3 location = this.Parent.actor.location;
				VInt3 lhs = VInt3.MoveTowards(location, this.Parent.targetLocation, this.CurrentSpeed * delta / 1000);
				this.Parent.movingMode.Move(lhs - location);
				if (this.Parent.isRotateImmediately)
				{
					this.PointingAtTarget(delta);
				}
			}
			this.Velocity = this.Parent.actor.forward * this.CurrentSpeed / 1000f;
			if (this.IsOnTarget())
			{
				this.Parent.GotoState("IdleMovmentState");
			}
			this.Parent.nLerpStep = 2;
		}

		protected abstract void CalcSpeed(int delta);
	}
}
