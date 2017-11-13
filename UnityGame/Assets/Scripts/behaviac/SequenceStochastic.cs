using System;
using System.Collections.Generic;

namespace behaviac
{
	public class SequenceStochastic : CompositeStochastic
	{
		private class SequenceStochasticTask : CompositeStochastic.CompositeStochasticTask
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
						int index = this.m_set.get_Item(this.m_activeChildIndex);
						BehaviorTask behaviorTask = this.m_children[index];
						eBTStatus = behaviorTask.exec(pAgent);
					}
					flag = false;
					if (eBTStatus != EBTStatus.BT_SUCCESS)
					{
						break;
					}
					this.m_activeChildIndex++;
					if (this.m_activeChildIndex >= this.m_children.Count)
					{
						return EBTStatus.BT_SUCCESS;
					}
					if (!this.CheckPredicates(pAgent))
					{
						return EBTStatus.BT_FAILURE;
					}
				}
				return eBTStatus;
			}
		}

		~SequenceStochastic()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is SequenceStochastic && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new SequenceStochastic.SequenceStochasticTask();
		}
	}
}
