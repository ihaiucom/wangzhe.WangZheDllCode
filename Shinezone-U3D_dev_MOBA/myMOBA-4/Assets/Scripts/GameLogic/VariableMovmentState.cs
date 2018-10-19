using System;

namespace Assets.Scripts.GameLogic
{
	internal class VariableMovmentState : MovingMovmentState
	{
		public VariableMovmentState(PlayerMovement InParent) : base(InParent)
		{
		}

		protected override void CalcSpeed(int delta)
		{
		}
	}
}
