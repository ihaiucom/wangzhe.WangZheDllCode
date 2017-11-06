using System;
using System.Collections.Generic;

namespace behaviac
{
	public class DecoratorWeight : DecoratorNode
	{
		public class DecoratorWeightTask : DecoratorTask
		{
			public int GetWeight(Agent pAgent)
			{
				DecoratorWeight decoratorWeight = (DecoratorWeight)base.GetNode();
				return (decoratorWeight != null) ? decoratorWeight.GetWeight(pAgent) : 0;
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
				return status;
			}
		}

		private Property m_weight_var;

		~DecoratorWeight()
		{
			this.m_weight_var = null;
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			using (List<property_t>.Enumerator enumerator = properties.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					property_t current = enumerator.get_Current();
					if (current.name == "Weight")
					{
						string text = null;
						string propertyName = null;
						this.m_weight_var = Condition.LoadRight(current.value, propertyName, ref text);
					}
				}
			}
		}

		protected virtual int GetWeight(Agent pAgent)
		{
			if (this.m_weight_var != null)
			{
				return (int)this.m_weight_var.GetValue(pAgent);
			}
			return 0;
		}

		protected override BehaviorTask createTask()
		{
			return new DecoratorWeight.DecoratorWeightTask();
		}
	}
}
