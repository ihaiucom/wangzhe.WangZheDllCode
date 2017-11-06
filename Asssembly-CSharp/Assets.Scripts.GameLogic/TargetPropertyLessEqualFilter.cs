using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public struct TargetPropertyLessEqualFilter
	{
		private List<ActorRoot> targetList;

		private ulong maxValue;

		public TargetPropertyLessEqualFilter(List<ActorRoot> _list, ulong _maxValue = 18446744073709551615uL)
		{
			this.targetList = _list;
			this.maxValue = _maxValue;
		}

		public void Initial(List<ActorRoot> _list, ulong _maxValue = 18446744073709551615uL)
		{
			this.targetList = _list;
			this.maxValue = _maxValue;
		}

		public void Searcher(VInt3 _curPos, ActorRoot _inActor, DistanceDelegate _delegate)
		{
			ulong num = _delegate(_curPos, _inActor.location);
			if (num < this.maxValue)
			{
				this.targetList.Clear();
				this.maxValue = num;
				this.targetList.Add(_inActor);
			}
			else if (num == this.maxValue)
			{
				this.targetList.Add(_inActor);
			}
		}

		public void Searcher(ActorRoot _curActor, ActorRoot _inActor, DistanceDelegate _delegate)
		{
			ulong num = _delegate(_curActor.location, _inActor.location);
			if (num < this.maxValue)
			{
				this.targetList.Clear();
				this.maxValue = num;
				this.targetList.Add(_inActor);
			}
			else if (num == this.maxValue)
			{
				this.targetList.Add(_inActor);
			}
		}

		public void Searcher(ActorRoot _inActor, RES_FUNCEFT_TYPE _propertyType, PropertyDelegate _delegate)
		{
			ulong num = _delegate(_inActor, _propertyType);
			if (num < this.maxValue)
			{
				this.targetList.Clear();
				this.maxValue = num;
				this.targetList.Add(_inActor);
			}
			else if (num == this.maxValue)
			{
				this.targetList.Add(_inActor);
			}
		}
	}
}
