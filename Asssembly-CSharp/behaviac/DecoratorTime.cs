using System;
using System.Collections.Generic;
using UnityEngine;

namespace behaviac
{
	public class DecoratorTime : DecoratorNode
	{
		private class DecoratorTimeTask : DecoratorTask
		{
			private int m_start;

			private int m_time;

			~DecoratorTimeTask()
			{
			}

			public override void copyto(BehaviorTask target)
			{
				base.copyto(target);
				DecoratorTime.DecoratorTimeTask decoratorTimeTask = (DecoratorTime.DecoratorTimeTask)target;
				decoratorTimeTask.m_start = this.m_start;
				decoratorTimeTask.m_time = this.m_time;
			}

			public override void save(ISerializableNode node)
			{
				base.save(node);
				CSerializationID attrId = new CSerializationID("start");
				node.setAttr<int>(attrId, this.m_start);
				CSerializationID attrId2 = new CSerializationID("time");
				node.setAttr<int>(attrId2, this.m_time);
			}

			public override void load(ISerializableNode node)
			{
				base.load(node);
			}

			protected override bool onenter(Agent pAgent)
			{
				base.onenter(pAgent);
				this.m_start = 0;
				this.m_time = this.GetTime(pAgent);
				return this.m_time > 0;
			}

			protected override EBTStatus decorate(EBTStatus status)
			{
				this.m_start += (int)(Time.deltaTime * 1000f);
				if (this.m_start >= this.m_time)
				{
					return EBTStatus.BT_SUCCESS;
				}
				return EBTStatus.BT_RUNNING;
			}

			private int GetTime(Agent pAgent)
			{
				DecoratorTime decoratorTime = (DecoratorTime)base.GetNode();
				return (decoratorTime != null) ? decoratorTime.GetTime(pAgent) : 0;
			}
		}

		private Property m_time_var;

		~DecoratorTime()
		{
			this.m_time_var = null;
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			using (List<property_t>.Enumerator enumerator = properties.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					property_t current = enumerator.get_Current();
					if (current.name == "Time")
					{
						string text = null;
						string propertyName = null;
						this.m_time_var = Condition.LoadRight(current.value, propertyName, ref text);
					}
				}
			}
		}

		protected virtual int GetTime(Agent pAgent)
		{
			if (this.m_time_var != null)
			{
				return (int)this.m_time_var.GetValue(pAgent);
			}
			return 0;
		}

		protected override BehaviorTask createTask()
		{
			return new DecoratorTime.DecoratorTimeTask();
		}
	}
}
