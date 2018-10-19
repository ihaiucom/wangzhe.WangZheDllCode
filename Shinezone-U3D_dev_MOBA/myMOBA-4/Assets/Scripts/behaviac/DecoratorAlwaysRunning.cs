using System;
using System.Collections.Generic;

namespace behaviac
{
	public class DecoratorAlwaysRunning : DecoratorNode
	{
		private class DecoratorAlwaysRunningTask : DecoratorTask
		{
			protected override void addChild(BehaviorTask pBehavior)
			{
				base.addChild(pBehavior);
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

			protected override EBTStatus decorate(EBTStatus status)
			{
				return EBTStatus.BT_RUNNING;
			}
		}

		~DecoratorAlwaysRunning()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is DecoratorAlwaysRunning && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new DecoratorAlwaysRunning.DecoratorAlwaysRunningTask();
		}
	}
}
