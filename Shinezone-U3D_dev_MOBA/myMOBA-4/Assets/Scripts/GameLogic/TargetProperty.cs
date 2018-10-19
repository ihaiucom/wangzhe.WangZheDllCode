using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class TargetProperty
	{
		public static ulong GetPropertyHpRate(ActorRoot _inActor, RES_FUNCEFT_TYPE _type)
		{
			int num = _inActor.ValueComponent.actorHp * 100;
			int totalValue = _inActor.ValueComponent.mActorValue[_type].totalValue;
			if (totalValue != 0)
			{
				num /= totalValue;
			}
			else
			{
				DebugHelper.Assert(false, "Gameobj MaxHp = 0 Exception,  ActorName:{0}", new object[]
				{
					(!(_inActor.gameObject == null)) ? _inActor.name : "null"
				});
			}
			return (ulong)((long)num);
		}

		public static ulong GetPropertyValue(ActorRoot _inActor, RES_FUNCEFT_TYPE _type)
		{
			return (ulong)((long)_inActor.ValueComponent.mActorValue[_type].totalValue);
		}
	}
}
