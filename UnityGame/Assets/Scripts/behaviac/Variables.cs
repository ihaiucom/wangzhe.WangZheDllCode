using System;
using System.Collections.Generic;

namespace behaviac
{
	public class Variables
	{
		public DictionaryView<uint, IVariable> m_variables = new DictionaryView<uint, IVariable>(15);

		~Variables()
		{
			this.Clear();
		}

		public void Clear()
		{
			this.m_variables.Clear();
		}

		public bool IsExisting(uint varId)
		{
			return this.m_variables.ContainsKey(varId);
		}

		public void Instantiate(Property property_, object value)
		{
			uint variableId = property_.GetVariableId();
			IVariable variable = null;
			if (!this.m_variables.TryGetValue(variableId, out variable))
			{
				variable = new IVariable(null, property_);
				variable.SetValue(value, null);
				this.m_variables.Add(variableId, variable);
			}
			else
			{
				if (variable.m_instantiated == 0)
				{
					variable.SetProperty(property_);
				}
				IVariable variable2 = variable;
				IVariable expr_53 = variable2;
				expr_53.m_instantiated += 1;
			}
		}

		public void UnInstantiate(string variableName)
		{
			uint key = Utils.MakeVariableId(variableName);
			IVariable variable = null;
			if (this.m_variables.TryGetValue(key, out variable))
			{
				IVariable variable2 = variable;
				IVariable expr_1F = variable2;
				expr_1F.m_instantiated -= 1;
				if (variable.m_instantiated == 0)
				{
					variable.SetProperty(null);
				}
			}
		}

		public void UnLoad(string variableName)
		{
			uint key = Utils.MakeVariableId(variableName);
			this.m_variables.Remove(key);
		}

		public void SetFromString(Agent pAgent, string variableName, string valueStr)
		{
			string nameWithoutClassName = Utils.GetNameWithoutClassName(variableName);
			CMemberBase pMember = pAgent.FindMember(nameWithoutClassName);
			uint key = Utils.MakeVariableId(nameWithoutClassName);
			IVariable variable = null;
			if (this.m_variables.TryGetValue(key, out variable))
			{
				variable.SetFromString(pAgent, pMember, valueStr);
			}
		}

		public void Set(Agent pAgent, CMemberBase pMember, string variableName, object value, uint varId)
		{
			if (varId == 0u)
			{
				varId = Utils.MakeVariableId(variableName);
			}
			IVariable variable = null;
			if (!this.m_variables.TryGetValue(varId, out variable))
			{
				if (pMember == null)
				{
					if (pAgent != null)
					{
						pMember = pAgent.FindMember(variableName);
					}
					else
					{
						pMember = Agent.FindMemberBase(variableName);
					}
				}
				variable = new IVariable(pMember, variableName, varId);
				this.m_variables.Add(varId, variable);
			}
			variable.SetValue(value, pAgent);
		}

		public object Get(Agent pAgent, uint varId)
		{
			IVariable variable = null;
			if (this.m_variables.TryGetValue(varId, out variable))
			{
				Property property = variable.GetProperty();
				if (property != null)
				{
					string refName = property.GetRefName();
					if (!string.IsNullOrEmpty(refName))
					{
						return this.Get(pAgent, property.GetRefNameId());
					}
				}
				return variable.GetValue(pAgent);
			}
			CMemberBase cMemberBase = pAgent.FindMember(varId);
			if (cMemberBase != null)
			{
				return cMemberBase.Get(pAgent);
			}
			return null;
		}

		public void Log(Agent pAgent, bool bForce)
		{
		}

		public void Reset()
		{
			foreach (KeyValuePair<uint, IVariable> current in this.m_variables)
			{
				current.get_Value().Reset();
			}
		}

		public void Unload()
		{
			this.m_variables.Clear();
		}

		public void CopyTo(Agent pAgent, Variables target)
		{
			target.m_variables.Clear();
			foreach (KeyValuePair<uint, IVariable> current in this.m_variables)
			{
				IVariable variable = current.get_Value().clone();
				target.m_variables[variable.GetId()] = variable;
			}
			if (!object.ReferenceEquals(pAgent, null))
			{
				foreach (KeyValuePair<uint, IVariable> current2 in target.m_variables)
				{
					current2.get_Value().CopyTo(pAgent);
				}
			}
		}
	}
}
