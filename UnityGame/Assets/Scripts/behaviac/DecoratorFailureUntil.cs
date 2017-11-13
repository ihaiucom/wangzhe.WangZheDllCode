using System;
using System.Collections.Generic;

namespace behaviac
{
	public class DecoratorFailureUntil : DecoratorCount
	{
		private class DecoratorFailureUntilTask : DecoratorCount.DecoratorCountTask
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
				if (this.m_n > 0)
				{
					this.m_n--;
					if (this.m_n == 0)
					{
						return EBTStatus.BT_SUCCESS;
					}
					return EBTStatus.BT_FAILURE;
				}
				else
				{
					if (this.m_n == -1)
					{
						return EBTStatus.BT_FAILURE;
					}
					return EBTStatus.BT_SUCCESS;
				}
			}

			public override bool NeedRestart()
			{
				return true;
			}
		}

		~DecoratorFailureUntil()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is DecoratorFailureUntil && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new DecoratorFailureUntil.DecoratorFailureUntilTask();
		}
	}
}
