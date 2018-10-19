using System;

namespace Assets.Scripts.GameLogic
{
	internal class AccelerateMovementState : VariableMovmentState
	{
		public AccelerateMovementState(PlayerMovement InParent) : base(InParent)
		{
		}

		protected override void CalcSpeed(int delta)
		{
			base.CurrentSpeed += this.Parent.Acceleration * delta / 1000;
			if (base.CurrentSpeed >= this.Parent.maxSpeed)
			{
				base.CurrentSpeed = this.Parent.maxSpeed;
			}
		}

		public override void UpdateLogic(int nDelta)
		{
			base.UpdateLogic(nDelta);
			if (base.CurrentSpeed >= this.Parent.maxSpeed && !this.IsOnTarget())
			{
				this.Parent.GotoState(UniformMovementState.StateName);
			}
		}
	}
}
