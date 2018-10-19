using System;

namespace behaviac
{
	public abstract class ConditionBase : BehaviorNode
	{
		public ConditionBase()
		{
		}

		~ConditionBase()
		{
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is ConditionBase && base.IsValid(pAgent, pTask);
		}
	}
}
