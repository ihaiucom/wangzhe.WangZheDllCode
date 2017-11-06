using System;

namespace Assets.Scripts.GameLogic
{
	internal class DirectionalMovementMode : MovementMode
	{
		public static string StateName = typeof(DirectionalMovementMode).get_Name();

		public DirectionalMovementMode(PlayerMovement InMovement) : base(InMovement)
		{
		}

		public override VInt3 GetTargetPosition()
		{
			return this.Movement.actor.location + this.Movement.direction;
		}

		public override bool ShouldStop()
		{
			return this.Movement.isStopMoving;
		}

		public override void OnStateEnter()
		{
			this.Movement.MoveDirState.Reset();
		}

		public override void Move(VInt3 delta)
		{
			ActorRoot actor = this.Movement.actor;
			VInt groundY;
			actor.location += PathfindingUtility.Move(actor, delta, out groundY, out actor.hasReachedNavEdge, this.Movement.MoveDirState);
			actor.groundY = groundY;
		}

		public override void OnStateLeave()
		{
		}
	}
}
