using System;
using System.Collections.Generic;

namespace behaviac
{
	public class DecoratorLoopUntil : DecoratorCount
	{
		private class DecoratorLoopUntilTask : DecoratorCount.DecoratorCountTask
		{
			public override bool NeedRestart()
			{
				return true;
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
				if (this.m_n > 0)
				{
					this.m_n--;
				}
				if (this.m_n == 0)
				{
					return EBTStatus.BT_SUCCESS;
				}
				DecoratorLoopUntil decoratorLoopUntil = (DecoratorLoopUntil)base.GetNode();
				if (decoratorLoopUntil.m_until)
				{
					if (status == EBTStatus.BT_SUCCESS)
					{
						return EBTStatus.BT_SUCCESS;
					}
				}
				else if (status == EBTStatus.BT_FAILURE)
				{
					return EBTStatus.BT_FAILURE;
				}
				return EBTStatus.BT_RUNNING;
			}
		}

		protected bool m_until;

		public DecoratorLoopUntil()
		{
			this.m_until = true;
		}

		~DecoratorLoopUntil()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			using (List<property_t>.Enumerator enumerator = properties.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					property_t current = enumerator.get_Current();
					if (current.name == "Until")
					{
						if (current.value == "true")
						{
							this.m_until = true;
						}
						else if (current.value == "false")
						{
							this.m_until = false;
						}
					}
				}
			}
		}

		protected override BehaviorTask createTask()
		{
			return new DecoratorLoopUntil.DecoratorLoopUntilTask();
		}
	}
}
