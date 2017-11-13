using System;
using System.Collections.Generic;

namespace behaviac
{
	public class Noop : BehaviorNode
	{
		private class NoopTask : LeafTask
		{
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

			protected override bool onenter(Agent pAgent)
			{
				return true;
			}

			protected override void onexit(Agent pAgent, EBTStatus s)
			{
			}

			protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
			{
				return EBTStatus.BT_SUCCESS;
			}
		}

		~Noop()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is Noop;
		}

		protected override BehaviorTask createTask()
		{
			return new Noop.NoopTask();
		}
	}
}
