using System;

namespace Assets.Scripts.GameLogic
{
	internal class IdleMovmentState : MovementState
	{
		public IdleMovmentState(PlayerMovement InParent) : base(InParent)
		{
		}

		public override void OnStateEnter()
		{
		}

		public override int GetCurrentSpeed()
		{
			return 0;
		}

		public override void ChangeTarget()
		{
			this.Parent.GotoState("AccelerateMovementState");
		}

		public override void ChangeDirection()
		{
			this.Parent.GotoState("AccelerateMovementState");
		}

		public override bool IsMoving()
		{
			return false;
		}
	}
}
