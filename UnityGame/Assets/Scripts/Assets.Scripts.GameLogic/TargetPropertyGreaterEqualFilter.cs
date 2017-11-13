using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public struct TargetPropertyGreaterEqualFilter
	{
		private List<ActorRoot> targetList;

		private ulong minValue;

		public TargetPropertyGreaterEqualFilter(List<ActorRoot> _list, ulong _minValue = 0uL)
		{
			this.targetList = _list;
			this.minValue = _minValue;
		}

		public void Searcher(ActorRoot _inActor, RES_FUNCEFT_TYPE _propertyType, PropertyDelegate _delegate)
		{
			ulong num = _delegate(_inActor, _propertyType);
			if (num > this.minValue)
			{
				this.targetList.Clear();
				this.minValue = num;
				this.targetList.Add(_inActor);
			}
			else if (num == this.minValue)
			{
				this.targetList.Add(_inActor);
			}
		}
	}
}
