using System;

namespace Assets.Scripts.GameLogic
{
	internal class AutoMovementMode : MovementMode
	{
		public AutoMovementMode(PlayerMovement InMovement) : base(InMovement)
		{
		}

		public override bool ShouldStop()
		{
			VInt3 lhs = this.Movement.actor.location - this.Movement.SelectedTargetLocation;
			return lhs == VInt3.zero || (this.Movement.Pathfinding != null && this.Movement.Pathfinding.targetReached && this.Movement.Pathfinding.targetSearchPos == this.Movement.SelectedTargetLocation);
		}
	}
}
