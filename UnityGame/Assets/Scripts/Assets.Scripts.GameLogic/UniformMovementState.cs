using System;

namespace Assets.Scripts.GameLogic
{
	internal class UniformMovementState : MovingMovmentState
	{
		public static string StateName = typeof(UniformMovementState).get_Name();

		public UniformMovementState(PlayerMovement InParent) : base(InParent)
		{
		}

		protected override void CalcSpeed(int delta)
		{
			base.CurrentSpeed = this.Parent.maxSpeed;
		}
	}
}
