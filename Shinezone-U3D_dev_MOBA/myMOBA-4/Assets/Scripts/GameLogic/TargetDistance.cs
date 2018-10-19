using System;

namespace Assets.Scripts.GameLogic
{
	public class TargetDistance
	{
		public static ulong GetDistance(VInt3 _curPos, VInt3 _inPos)
		{
			return (ulong)(_curPos - _inPos).sqrMagnitudeLong2D;
		}
	}
}
