using System;
using System.Collections.Generic;

namespace behaviac
{
	public class Or : ConditionBase
	{
		private class OrTask : Selector.SelectorTask
		{
			~OrTask()
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
				for (int i = 0; i < this.m_children.Count; i++)
				{
					BehaviorTask behaviorTask = this.m_children[i];
					EBTStatus eBTStatus = behaviorTask.exec(pAgent);
					if (eBTStatus == EBTStatus.BT_SUCCESS)
					{
						return eBTStatus;
					}
				}
				return EBTStatus.BT_FAILURE;
			}
		}

		~Or()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is Or && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new Or.OrTask();
		}
	}
}
