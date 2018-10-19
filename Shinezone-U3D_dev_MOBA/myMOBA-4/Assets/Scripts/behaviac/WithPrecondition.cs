using System;
using System.Collections.Generic;

namespace behaviac
{
	public class WithPrecondition : BehaviorNode
	{
		~WithPrecondition()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is WithPrecondition && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new WithPreconditionTask();
		}
	}
}
