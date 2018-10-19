using System;
using System.Collections.Generic;

namespace behaviac
{
	public class Condition : ConditionBase
	{
		private class ConditionTask : ConditionBaseTask
		{
			~ConditionTask()
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
				Condition condition = (Condition)base.GetNode();
				if (condition.m_comparator != null)
				{
					if (Condition.DoCompare(pAgent, condition.m_comparator, condition.m_opl, condition.m_opl_m, condition.m_opr))
					{
						result = EBTStatus.BT_SUCCESS;
					}
				}
				else
				{
					result = condition.update_impl(pAgent, childStatus);
				}
				return result;
			}
		}

		protected Property m_opl;

		private Property m_opr;

		private CMethodBase m_opl_m;

		private VariableComparator m_comparator;

		public static bool Register<T>(string typeName)
		{
			return true;
		}

		public static void UnRegister<T>(string typeName)
		{
		}

		public static void RegisterBasicTypes()
		{
		}

		public static void UnRegisterBasicTypes()
		{
		}

		~Condition()
		{
			this.m_opl = null;
			this.m_opr = null;
			this.m_opl_m = null;
			this.m_comparator = null;
		}

		public static VariableComparator Create(string typeName, string comparionOperator, Property lhs, Property rhs)
		{
			E_VariableComparisonType comparisonType = VariableComparator.ParseComparisonType(comparionOperator);
			if (Agent.IsAgentClassName(typeName))
			{
				typeName = "void*";
			}
			VariableComparator variableComparator = VariableComparator.Create(typeName, lhs, rhs);
			variableComparator.SetComparisonType(comparisonType);
			return variableComparator;
		}

		public static Property LoadLeft(string value, ref string propertyName, string constValue)
		{
			Property result = null;
			if (!string.IsNullOrEmpty(value))
			{
				string[] array = value.Split(new char[]
				{
					' '
				});
				if (array != null && array.Length == 2)
				{
					string typeName = array[0].Replace("::", ".");
					propertyName = array[1];
					result = Property.Create(typeName, array[1], constValue, false, false);
				}
				else
				{
					DebugHelper.Assert(array != null && array.Length > 0 && array[0] == "static");
					string typeName2 = array[1].Replace("::", ".");
					propertyName = array[2];
					result = Property.Create(typeName2, array[2], constValue, true, false);
				}
			}
			return result;
		}

		public static Property LoadRight(string value, string propertyName, ref string typeName)
		{
			Property result = null;
			if (!string.IsNullOrEmpty(value))
			{
				if (value.StartsWith("const"))
				{
					string text = value.Substring(6);
					int num = StringUtils.FirstToken(text, ' ', ref typeName);
					typeName = typeName.Replace("::", ".");
					string value2 = text.Substring(num + 1);
					result = Property.Create(typeName, propertyName, value2, false, true);
				}
				else
				{
					string[] array = value.Split(new char[]
					{
						' '
					});
					if (array[0] == "static")
					{
						typeName = array[1].Replace("::", ".");
						result = Property.Create(typeName, array[2], null, true, false);
					}
					else
					{
						typeName = array[0].Replace("::", ".");
						result = Property.Create(typeName, array[1], null, false, false);
					}
				}
			}
			return result;
		}

		public static Property LoadProperty(string value)
		{
			string propertyName = null;
			string text = null;
			return Condition.LoadRight(value, propertyName, ref text);
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			string typeName = null;
			string propertyName = null;
			string text = null;
			foreach (property_t current in properties)
			{
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
			}
			if (!string.IsNullOrEmpty(text) && (this.m_opl != null || this.m_opl_m != null) && this.m_opr != null)
			{
				this.m_comparator = Condition.Create(typeName, text, this.m_opl, this.m_opr);
			}
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is Condition && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new Condition.ConditionTask();
		}

		public static bool DoCompare(Agent pAgent, VariableComparator comparator, Property opl, CMethodBase opl_m, Property opr)
		{
			bool result = false;
			if (opl != null)
			{
				Agent agent = pAgent;
				ParentType parentType = opl.GetParentType();
				if (parentType == ParentType.PT_INSTANCE)
				{
					agent = Agent.GetInstance(opl.GetInstanceNameString(), agent.GetContextId());
				}
				Agent agentR = pAgent;
				parentType = opr.GetParentType();
				if (parentType == ParentType.PT_INSTANCE)
				{
					agentR = Agent.GetInstance(opr.GetInstanceNameString(), agent.GetContextId());
				}
				result = comparator.Execute(agent, agentR);
			}
			else if (opl_m != null)
			{
				ParentType parentType2 = opl_m.GetParentType();
				Agent agent2 = pAgent;
				if (parentType2 == ParentType.PT_INSTANCE)
				{
					agent2 = Agent.GetInstance(opl_m.GetInstanceNameString(), agent2.GetContextId());
				}
				object lhs = opl_m.run(agent2, pAgent);
				Agent agent3 = pAgent;
				parentType2 = opr.GetParentType();
				if (parentType2 == ParentType.PT_INSTANCE)
				{
					agent3 = Agent.GetInstance(opr.GetInstanceNameString(), agent3.GetContextId());
				}
				result = comparator.Execute(lhs, agent2, agent3);
			}
			return result;
		}
	}
}
