using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public struct TargetDistanceNearFilter
	{
		private ulong maxDisSqr;

		public TargetDistanceNearFilter(ulong _maxDisSqr)
		{
			this.maxDisSqr = _maxDisSqr;
		}

		public ActorRoot Searcher(List<ActorRoot>.Enumerator _etr, ActorRoot _mainActor)
		{
			ActorRoot result = null;
			while (_etr.MoveNext())
			{
				ActorRoot current = _etr.Current;
				ulong sqrMagnitudeLong2D = (ulong)(current.location - _mainActor.location).sqrMagnitudeLong2D;
				if (sqrMagnitudeLong2D < this.maxDisSqr)
				{
					result = current;
					this.maxDisSqr = sqrMagnitudeLong2D;
				}
			}
			return result;
		}
	}
}
