using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public struct TargetDistanceFarFilter
	{
		private ulong minDisSqr;

		public TargetDistanceFarFilter(ulong _minDisSqr)
		{
			this.minDisSqr = _minDisSqr;
		}

		public ActorRoot Searcher(List<ActorRoot>.Enumerator _etr, ActorRoot _mainActor)
		{
			ActorRoot result = null;
			while (_etr.MoveNext())
			{
				ActorRoot current = _etr.get_Current();
				ulong sqrMagnitudeLong2D = (ulong)(current.location - _mainActor.location).sqrMagnitudeLong2D;
				if (sqrMagnitudeLong2D > this.minDisSqr)
				{
					result = current;
					this.minDisSqr = sqrMagnitudeLong2D;
				}
			}
			return result;
		}
	}
}
