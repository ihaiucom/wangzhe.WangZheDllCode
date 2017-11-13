using System;
using System.Collections.Generic;

namespace behaviac
{
	public class DecoratorNot : DecoratorNode
	{
		private class DecoratorNotTask : DecoratorTask
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
				if (status == EBTStatus.BT_FAILURE)
				{
					return EBTStatus.BT_SUCCESS;
				}
				if (status == EBTStatus.BT_SUCCESS)
				{
					return EBTStatus.BT_FAILURE;
				}
				return status;
			}
		}

		~DecoratorNot()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is DecoratorNot && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new DecoratorNot.DecoratorNotTask();
		}
	}
}
