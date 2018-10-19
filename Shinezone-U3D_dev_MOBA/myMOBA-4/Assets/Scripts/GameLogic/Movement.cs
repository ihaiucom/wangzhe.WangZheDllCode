using System;

namespace Assets.Scripts.GameLogic
{
	public abstract class Movement : LogicComponent
	{
		public abstract VInt3 targetLocation
		{
			get;
		}

		public abstract int speed
		{
			get;
		}

		public abstract VInt3 velocity
		{
			get;
		}

		public abstract int maxSpeed
		{
			get;
			set;
		}

		public abstract int acceleration
		{
			get;
		}

		public abstract int rotateSpeed
		{
			get;
		}

		public abstract bool isMoving
		{
			get;
		}

		public abstract bool isFinished
		{
			get;
		}

		public abstract bool isAccelerating
		{
			get;
		}

		public abstract bool isRotatingLock
		{
			get;
		}

		public abstract bool isDecelerate
		{
			get;
		}

		public abstract bool isDirectionalMoveMode
		{
			get;
		}

		public abstract bool isAutoMoveMode
		{
			get;
		}

		public abstract uint uCommandId
		{
			get;
			set;
		}

		public abstract bool isRotateImmediately
		{
			get;
			set;
		}

		public abstract bool isFlying
		{
			get;
			set;
		}

		public abstract bool isLerpFlying
		{
			get;
			set;
		}

		public abstract bool isExcuteMoving
		{
			get;
		}

		public abstract MPathfinding Pathfinding
		{
			get;
		}

		public abstract void SetMoveParam(VInt3 InVector, bool bDirection, bool bInRotateImmediately, uint cmdId = 0u);

		public abstract void SetRotate(VInt3 InDirection, bool bInRotateImmediately);

		public abstract void ExcuteMove();

		public abstract void StopMove();

		public abstract float GetDistance(uint nDelta);

		public abstract void GravityModeLerp(uint nDelta, bool bReset);
	}
}
