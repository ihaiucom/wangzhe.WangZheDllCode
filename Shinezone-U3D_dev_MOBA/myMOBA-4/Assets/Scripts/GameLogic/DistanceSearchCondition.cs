using System;

namespace Assets.Scripts.GameLogic
{
	public class DistanceSearchCondition
	{
		public static bool Fit(ActorRoot _actor, ActorRoot _mainActor, int _dis)
		{
			ulong num = (ulong)((long)_dis * (long)_dis);
			ulong sqrMagnitudeLong2D = (ulong)(_actor.location - _mainActor.location).sqrMagnitudeLong2D;
			return num >= sqrMagnitudeLong2D;
		}

		public static bool Fit(VInt3 _curPos, ActorRoot _mainActor, int _dis)
		{
			ulong num = (ulong)((long)_dis * (long)_dis);
			ulong sqrMagnitudeLong2D = (ulong)(_curPos - _mainActor.location).sqrMagnitudeLong2D;
			return num >= sqrMagnitudeLong2D;
		}
	}
}
