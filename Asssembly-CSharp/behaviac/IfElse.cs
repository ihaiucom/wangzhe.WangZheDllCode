using System;
using System.Collections.Generic;

namespace behaviac
{
	public class IfElse : BehaviorNode
	{
		private class IfElseTask : CompositeTask
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
				this.m_activeChildIndex = -1;
				return this.m_children.Count == 3;
			}

			protected override void onexit(Agent pAgent, EBTStatus s)
			{
			}

			protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
			{
				if (childStatus != EBTStatus.BT_RUNNING)
				{
					return childStatus;
				}
				if (this.m_activeChildIndex == -1)
				{
					BehaviorTask behaviorTask = this.m_children[0];
					EBTStatus eBTStatus = behaviorTask.exec(pAgent);
					if (eBTStatus == EBTStatus.BT_SUCCESS)
					{
						this.m_activeChildIndex = 1;
					}
					else if (eBTStatus == EBTStatus.BT_FAILURE)
					{
						this.m_activeChildIndex = 2;
					}
				}
				if (this.m_activeChildIndex != -1)
				{
					BehaviorTask behaviorTask2 = this.m_children[this.m_activeChildIndex];
					return behaviorTask2.exec(pAgent);
				}
				return EBTStatus.BT_RUNNING;
			}
		}

		~IfElse()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is IfElse && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new IfElse.IfElseTask();
		}
	}
}
