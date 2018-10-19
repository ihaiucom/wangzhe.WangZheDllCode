using System;

namespace Assets.Scripts.GameLogic
{
	internal class DecelerateMovementState : VariableMovmentState
	{
		public static string StateName = typeof(DecelerateMovementState).Name;

		public DecelerateMovementState(PlayerMovement InParent) : base(InParent)
		{
		}
	}
}
