using System;
using System.Collections.Generic;

namespace behaviac
{
	public class Selector : BehaviorNode
	{
		public class SelectorTask : CompositeTask
		{
			~SelectorTask()
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

			protected override bool onenter(Agent pAgent)
			{
				this.m_activeChildIndex = 0;
				return true;
			}

			protected override void onexit(Agent pAgent, EBTStatus s)
			{
			}

			protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
			{
				bool flag = true;
				EBTStatus eBTStatus;
				while (true)
				{
					eBTStatus = childStatus;
					if (!flag || eBTStatus == EBTStatus.BT_RUNNING)
					{
						BehaviorTask behaviorTask = this.m_children[this.m_activeChildIndex];
						eBTStatus = behaviorTask.exec(pAgent);
					}
					flag = false;
					if (eBTStatus != EBTStatus.BT_FAILURE)
					{
						break;
					}
					this.m_activeChildIndex++;
					if (this.m_activeChildIndex >= this.m_children.Count)
					{
						return EBTStatus.BT_FAILURE;
					}
					if (!this.CheckPredicates(pAgent))
					{
						return EBTStatus.BT_FAILURE;
					}
				}
				return eBTStatus;
			}
		}

		~Selector()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is Selector && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new Selector.SelectorTask();
		}
	}
}
