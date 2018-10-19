using System;
using System.Collections.Generic;

namespace behaviac
{
	public abstract class DecoratorCount : DecoratorNode
	{
		protected abstract class DecoratorCountTask : DecoratorTask
		{
			protected int m_n;

			public DecoratorCountTask()
			{
			}

			~DecoratorCountTask()
			{
			}

			public override void copyto(BehaviorTask target)
			{
				base.copyto(target);
				DecoratorCount.DecoratorCountTask decoratorCountTask = (DecoratorCount.DecoratorCountTask)target;
				decoratorCountTask.m_n = this.m_n;
			}

			public override void save(ISerializableNode node)
			{
				base.save(node);
				CSerializationID attrId = new CSerializationID("count");
				node.setAttr<int>(attrId, this.m_n);
			}

			public override void load(ISerializableNode node)
			{
				base.load(node);
			}

			protected override bool onenter(Agent pAgent)
			{
				base.onenter(pAgent);
				if (this.m_n == 0 || !this.NeedRestart())
				{
					int count = this.GetCount(pAgent);
					if (count == 0)
					{
						return false;
					}
					this.m_n = count;
				}
				return true;
			}

			public int GetCount(Agent pAgent)
			{
				DecoratorCount decoratorCount = (DecoratorCount)base.GetNode();
				return (decoratorCount == null) ? 0 : decoratorCount.GetCount(pAgent);
			}
		}

		private Property m_count_var;

		public DecoratorCount()
		{
		}

		~DecoratorCount()
		{
			this.m_count_var = null;
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			foreach (property_t current in properties)
			{
				if (current.name == "Count")
				{
					string text = null;
					string propertyName = null;
					this.m_count_var = Condition.LoadRight(current.value, propertyName, ref text);
				}
			}
		}

		protected virtual int GetCount(Agent pAgent)
		{
			if (this.m_count_var != null)
			{
				return (int)this.m_count_var.GetValue(pAgent);
			}
			return 0;
		}
	}
}
