using System;

namespace Assets.Scripts.GameLogic
{
	internal class HoldonMovementMode : MovementMode
	{
		public HoldonMovementMode(PlayerMovement InMovement) : base(InMovement)
		{
		}

		public override bool ShouldStop()
		{
			return true;
		}

		public override void OnStateEnter()
		{
			this.Movement.isRotateImmediately = false;
		}
	}
}
