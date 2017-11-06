using System;
using System.Collections.Generic;

namespace behaviac
{
	public class WaitforSignal : BehaviorNode
	{
		~WaitforSignal()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is WaitforSignal && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new WaitforSignalTask();
		}
	}
}
