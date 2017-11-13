using System;
using System.Collections.Generic;

namespace behaviac
{
	public class Predicate : ConditionBase
	{
		public class PredicateTask : AttachmentTask
		{
			~PredicateTask()
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
				return true;
			}

			protected override void onexit(Agent pAgent, EBTStatus s)
			{
			}

			protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
			{
				EBTStatus result = EBTStatus.BT_FAILURE;
				Predicate predicate = base.GetNode() as Predicate;
				if (predicate.m_comparator != null)
				{
					bool flag = Condition.DoCompare(pAgent, predicate.m_comparator, predicate.m_opl, predicate.m_opl_m, predicate.m_opr);
					if (flag)
					{
						result = EBTStatus.BT_SUCCESS;
					}
				}
				else
				{
					result = predicate.update_impl(pAgent, childStatus);
				}
				return result;
			}

			public override bool NeedRestart()
			{
				return true;
			}

			public bool IsAnd()
			{
				Predicate predicate = base.GetNode() as Predicate;
				return predicate.m_bAnd;
			}
		}

		protected Property m_opl;

		protected Property m_opr;

		protected CMethodBase m_opl_m;

		protected VariableComparator m_comparator;

		protected bool m_bAnd;

		public Predicate()
		{
			this.m_bAnd = false;
		}

		~Predicate()
		{
			this.m_opl = null;
			this.m_opr = null;
			this.m_opl_m = null;
			this.m_comparator = null;
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is Predicate && base.IsValid(pAgent, pTask);
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			string typeName = null;
			string propertyName = null;
			string text = null;
			using (List<property_t>.Enumerator enumerator = properties.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					property_t current = enumerator.get_Current();
					if (current.name == "Operator")
					{
						text = current.value;
					}
					else if (current.name == "Opl")
					{
						int num = current.value.IndexOf('(');
						if (num == -1)
						{
							this.m_opl = Condition.LoadLeft(current.value, ref propertyName, null);
						}
						else
						{
							this.m_opl_m = Action.LoadMethod(current.value);
						}
					}
					else if (current.name == "Opr")
					{
						this.m_opr = Condition.LoadRight(current.value, propertyName, ref typeName);
					}
					else if (current.name == "BinaryOperator")
					{
						if (current.value == "Or")
						{
							this.m_bAnd = false;
						}
						else if (current.value == "And")
						{
							this.m_bAnd = true;
						}
					}
				}
			}
			if (!string.IsNullOrEmpty(text) && (this.m_opl != null || this.m_opl_m != null) && this.m_opr != null)
			{
				this.m_comparator = Condition.Create(typeName, text, this.m_opl, this.m_opr);
			}
		}

		protected override BehaviorTask createTask()
		{
			return new Predicate.PredicateTask();
		}
	}
}
