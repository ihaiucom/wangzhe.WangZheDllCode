using System;
using System.Collections.Generic;

namespace behaviac
{
	public class False : ConditionBase
	{
		private class FalseTask : ConditionBaseTask
		{
			~FalseTask()
			{
			}

			public override void copyto(BehaviorTask target)
			{
				base.copyto(target);
			}

			public override void save(ISerializableNode node)
			{
				base.save(node);
			}

			public override void load(ISerializableNode node)
			{
				base.load(node);
			}

			protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
			{
				return EBTStatus.BT_FAILURE;
			}
		}

		~False()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is False && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new False.FalseTask();
		}
	}
}
