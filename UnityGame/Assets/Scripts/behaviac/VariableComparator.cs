using System;

namespace behaviac
{
	public class VariableComparator
	{
		protected Property m_lhs;

		protected Property m_rhs;

		protected E_VariableComparisonType m_comparisonType;

		public VariableComparator(Property lhs, Property rhs)
		{
			this.m_lhs = lhs;
			this.m_rhs = rhs;
		}

		private VariableComparator(VariableComparator copy)
		{
			this.m_lhs = copy.m_lhs;
			this.m_rhs = copy.m_rhs;
		}

		public static E_VariableComparisonType ParseComparisonType(string comparionOperator)
		{
			if (comparionOperator == "Equal")
			{
				return E_VariableComparisonType.VariableComparisonType_Equal;
			}
			if (comparionOperator == "NotEqual")
			{
				return E_VariableComparisonType.VariableComparisonType_NotEqual;
			}
			if (comparionOperator == "Greater")
			{
				return E_VariableComparisonType.VariableComparisonType_Greater;
			}
			if (comparionOperator == "GreaterEqual")
			{
				return E_VariableComparisonType.VariableComparisonType_GreaterEqual;
			}
			if (comparionOperator == "Less")
			{
				return E_VariableComparisonType.VariableComparisonType_Less;
			}
			if (comparionOperator == "LessEqual")
			{
				return E_VariableComparisonType.VariableComparisonType_LessEqual;
			}
			return E_VariableComparisonType.VariableComparisonType_Equal;
		}

		~VariableComparator()
		{
			this.m_lhs = null;
			this.m_rhs = null;
		}

		public VariableComparator clone()
		{
			return new VariableComparator(this);
		}

		public static VariableComparator Create(string typeName, Property lhs, Property rhs)
		{
			return new VariableComparator(lhs, rhs);
		}

		public bool Execute(Agent agentL, Agent agentR)
		{
			object value = this.m_lhs.GetValue(agentL);
			object value2 = this.m_rhs.GetValue(agentR);
			switch (this.m_comparisonType)
			{
			case E_VariableComparisonType.VariableComparisonType_Equal:
				if (object.ReferenceEquals(value, null))
				{
					return object.ReferenceEquals(value2, null);
				}
				return value.Equals(value2);
			case E_VariableComparisonType.VariableComparisonType_NotEqual:
				if (object.ReferenceEquals(value, null))
				{
					return !object.ReferenceEquals(value2, null);
				}
				return !value.Equals(value2);
			case E_VariableComparisonType.VariableComparisonType_Greater:
				return Details.Greater(value, value2);
			case E_VariableComparisonType.VariableComparisonType_GreaterEqual:
				return Details.GreaterEqual(value, value2);
			case E_VariableComparisonType.VariableComparisonType_Less:
				return Details.Less(value, value2);
			case E_VariableComparisonType.VariableComparisonType_LessEqual:
				return Details.LessEqual(value, value2);
			default:
				return false;
			}
		}

		public bool Execute(object lhs, Agent parent, Agent agentR)
		{
			object value = this.m_rhs.GetValue(agentR);
			switch (this.m_comparisonType)
			{
			case E_VariableComparisonType.VariableComparisonType_Equal:
				return lhs.Equals(value);
			case E_VariableComparisonType.VariableComparisonType_NotEqual:
				return !lhs.Equals(value);
			case E_VariableComparisonType.VariableComparisonType_Greater:
				return Details.Greater(lhs, value);
			case E_VariableComparisonType.VariableComparisonType_GreaterEqual:
				return Details.GreaterEqual(lhs, value);
			case E_VariableComparisonType.VariableComparisonType_Less:
				return Details.Less(lhs, value);
			case E_VariableComparisonType.VariableComparisonType_LessEqual:
				return Details.LessEqual(lhs, value);
			default:
				return false;
			}
		}

		public void SetComparisonType(E_VariableComparisonType type)
		{
			this.m_comparisonType = type;
		}
	}
}
