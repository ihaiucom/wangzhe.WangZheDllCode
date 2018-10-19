using System;
using System.Collections.Generic;

namespace behaviac
{
	public class DecoratorCountLimit : DecoratorCount
	{
		private class DecoratorCountLimitTask : DecoratorCount.DecoratorCountTask
		{
			private bool m_bInited;

			public override void copyto(BehaviorTask target)
			{
				base.copyto(target);
				DecoratorCountLimit.DecoratorCountLimitTask decoratorCountLimitTask = (DecoratorCountLimit.DecoratorCountLimitTask)target;
				decoratorCountLimitTask.m_bInited = this.m_bInited;
			}

			public override void save(ISerializableNode node)
			{
				base.save(node);
				CSerializationID attrId = new CSerializationID("inited");
				node.setAttr<bool>(attrId, this.m_bInited);
			}

			public override void load(ISerializableNode node)
			{
				base.load(node);
			}

			public override bool CheckPredicates(Agent pAgent)
			{
				bool result = false;
				if (this.m_attachments != null)
				{
					result = base.CheckPredicates(pAgent);
				}
				return result;
			}

			protected override bool onenter(Agent pAgent)
			{
				if (this.CheckPredicates(pAgent))
				{
					this.m_bInited = false;
				}
				if (!this.m_bInited)
				{
					this.m_bInited = true;
					int count = base.GetCount(pAgent);
					this.m_n = count;
				}
				if (this.m_n > 0)
				{
					this.m_n--;
					return true;
				}
				return this.m_n != 0 && this.m_n == -1;
			}

			protected override EBTStatus decorate(EBTStatus status)
			{
				return status;
			}
		}

		~DecoratorCountLimit()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is DecoratorCountLimit && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new DecoratorCountLimit.DecoratorCountLimitTask();
		}
	}
}
