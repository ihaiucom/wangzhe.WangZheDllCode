using System;
using System.Collections.Generic;

namespace behaviac
{
	public class SelectorStochastic : CompositeStochastic
	{
		private class SelectorStochasticTask : CompositeStochastic.CompositeStochasticTask
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

			protected override bool onenter(Agent pAgent)
			{
				base.onenter(pAgent);
				return true;
			}

			protected override void onexit(Agent pAgent, EBTStatus s)
			{
				base.onexit(pAgent, s);
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
						int index = this.m_set[this.m_activeChildIndex];
						BehaviorTask behaviorTask = this.m_children[index];
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
				}
				return eBTStatus;
			}
		}

		~SelectorStochastic()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is SelectorStochastic && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new SelectorStochastic.SelectorStochasticTask();
		}
	}
}
