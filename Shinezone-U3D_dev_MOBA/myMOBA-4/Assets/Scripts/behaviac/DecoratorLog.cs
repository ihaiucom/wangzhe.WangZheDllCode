using System;
using System.Collections.Generic;

namespace behaviac
{
	public class DecoratorLog : DecoratorNode
	{
		private class DecoratorLogTask : DecoratorTask
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

			protected override EBTStatus decorate(EBTStatus status)
			{
				DecoratorLog decoratorLog = (DecoratorLog)base.GetNode();
				return status;
			}
		}

		protected string m_message;

		~DecoratorLog()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			foreach (property_t current in properties)
			{
				if (current.name == "Log")
				{
					this.m_message = current.value;
				}
			}
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is DecoratorLog && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new DecoratorLog.DecoratorLogTask();
		}
	}
}
