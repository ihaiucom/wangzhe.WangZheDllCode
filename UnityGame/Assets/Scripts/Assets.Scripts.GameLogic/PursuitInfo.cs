using System;

namespace Assets.Scripts.GameLogic
{
	[Serializable]
	public class PursuitInfo
	{
		public int PursuitAngle;

		public VInt3 PursuitOrigin;

		public int PursuitRadius;

		public VInt3 PursuitDir = VInt3.right;

		public int CosHalfAngle;

		public bool IsVaild()
		{
			return this.PursuitAngle > 0 && this.PursuitRadius > 0;
		}
	}
}
