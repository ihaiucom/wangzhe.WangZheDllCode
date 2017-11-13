using System;

namespace Assets.Scripts.GameLogic
{
	public abstract class MovementMode : BaseState
	{
		protected PlayerMovement Movement;

		public MovementMode(PlayerMovement InMovement)
		{
			this.Movement = InMovement;
		}

		public virtual VInt3 GetTargetPosition()
		{
			return this.Movement.SelectedTargetLocation;
		}

		public virtual bool ShouldStop()
		{
			return false;
		}

		public virtual void Move(VInt3 delta)
		{
			this.Movement.actor.location = this.Movement.actor.location + delta;
		}
	}
}
