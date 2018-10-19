using System;
using System.Collections.Generic;

namespace behaviac
{
	public class And : ConditionBase
	{
		~And()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is And && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new AndTask();
		}
	}
}
