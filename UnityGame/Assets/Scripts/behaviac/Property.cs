using System;

namespace behaviac
{
	public class Property
	{
		private static DictionaryView<string, Property> ms_properties = new DictionaryView<string, Property>();

		protected string m_varaibleName;

		protected string m_varaibleFullName;

		protected uint m_variableId;

		protected readonly CMemberBase m_memberBase;

		protected string m_refParName;

		protected uint m_refParNameId;

		protected ParentType m_pt;

		protected string m_instanceName;

		protected object m_defaultValue;

		private bool m_bValidDefaultValue;

		protected readonly bool m_bIsConst;

		public Property(CMemberBase pMemberBase, bool bIsConst)
		{
			this.m_memberBase = pMemberBase;
			this.m_variableId = 0u;
			this.m_bValidDefaultValue = false;
			this.m_bIsConst = bIsConst;
			if (this.m_memberBase != null)
			{
				this.m_pt = this.m_memberBase.GetParentType();
			}
			else
			{
				this.m_pt = ParentType.PT_PAR;
			}
		}

		protected Property(Property copy)
		{
			this.m_varaibleName = copy.m_varaibleName;
			this.m_varaibleFullName = copy.m_varaibleFullName;
			this.m_variableId = copy.m_variableId;
			this.m_refParName = copy.m_refParName;
			this.m_refParNameId = copy.m_refParNameId;
			this.m_memberBase = copy.m_memberBase;
			this.m_pt = copy.m_pt;
			this.m_instanceName = copy.m_instanceName;
			this.m_bValidDefaultValue = copy.m_bValidDefaultValue;
			this.m_defaultValue = copy.m_defaultValue;
			this.m_bIsConst = copy.m_bIsConst;
		}

		~Property()
		{
		}

		public void SetVariableName(string variableName)
		{
			this.m_varaibleFullName = variableName;
			string nameWithoutClassName = Utils.GetNameWithoutClassName(variableName);
			this.m_variableId = Utils.MakeVariableId(nameWithoutClassName);
			this.m_varaibleName = nameWithoutClassName;
		}

		public string GetVariableName()
		{
			return this.m_varaibleName;
		}

		public string GetVariableFullName()
		{
			return this.m_varaibleFullName;
		}

		public uint GetVariableId()
		{
			return this.m_variableId;
		}

		public string GetClassNameString()
		{
			return (this.m_memberBase != null) ? this.m_memberBase.GetClassNameString() : null;
		}

		public string GetInstanceNameString()
		{
			if (!string.IsNullOrEmpty(this.m_instanceName))
			{
				return this.m_instanceName;
			}
			return (this.m_memberBase != null) ? this.m_memberBase.GetInstanceNameString() : null;
		}

		public void SetInstanceNameString(string agentIntanceName)
		{
			bool flag = !string.IsNullOrEmpty(agentIntanceName);
			if (flag)
			{
				if (Agent.IsNameRegistered(agentIntanceName))
				{
					this.m_pt = ParentType.PT_INSTANCE;
					this.m_instanceName = agentIntanceName;
				}
				else
				{
					this.m_pt = ParentType.PT_AGENT;
				}
			}
		}

		public Property clone()
		{
			return new Property(this);
		}

		public void SetFrom(Agent pAgentFrom, Property from, Agent pAgentTo)
		{
			object value = from.GetValue(pAgentFrom);
			this.SetValue(pAgentTo, value);
		}

		public void SetDefaultInteger(int count)
		{
			this.m_bValidDefaultValue = true;
			Utils.ConvertFromInteger<object>(count, ref this.m_defaultValue);
		}

		public uint GetDefaultInteger()
		{
			return 0u;
		}

		public float DifferencePercentage(Property other)
		{
			object defaultValue = this.GetDefaultValue();
			object defaultValue2 = other.GetDefaultValue();
			float range = this.m_memberBase.GetRange();
			float num = 0f;
			float num2 = 0f;
			if (defaultValue.GetType() == typeof(float))
			{
				num = (float)defaultValue;
				num2 = (float)defaultValue2;
			}
			else if (defaultValue.GetType() == typeof(long))
			{
				num = (float)((long)defaultValue);
				num2 = (float)((long)defaultValue2);
			}
			else if (defaultValue.GetType() == typeof(int))
			{
				num = (float)((int)defaultValue);
				num2 = (float)((int)defaultValue2);
			}
			else if (defaultValue.GetType() == typeof(short))
			{
				num = (float)((short)defaultValue);
				num2 = (float)((short)defaultValue2);
			}
			else if (defaultValue.GetType() == typeof(sbyte))
			{
				num = (float)((sbyte)defaultValue);
				num2 = (float)((sbyte)defaultValue2);
			}
			else if (defaultValue.GetType() == typeof(ulong))
			{
				num = (ulong)defaultValue;
				num2 = (ulong)defaultValue2;
			}
			else if (defaultValue.GetType() == typeof(uint))
			{
				num = (uint)defaultValue;
				num2 = (uint)defaultValue2;
			}
			else if (defaultValue.GetType() == typeof(ushort))
			{
				num = (float)((ushort)defaultValue);
				num2 = (float)((ushort)defaultValue2);
			}
			else if (defaultValue.GetType() == typeof(byte))
			{
				num = (float)((byte)defaultValue);
				num2 = (float)((byte)defaultValue2);
			}
			float num3 = num - num2;
			if (num3 < 0f)
			{
				num3 = -num3;
			}
			return num3 / range;
		}

		public void Instantiate(Agent pAgent)
		{
			object defaultValue = this.GetDefaultValue();
			pAgent.Instantiate<object>(defaultValue, this);
		}

		public void UnInstantiate(Agent pAgent)
		{
			pAgent.UnInstantiate(this.m_varaibleName);
		}

		public void UnLoad(Agent pAgent)
		{
			pAgent.UnLoad(this.m_varaibleName);
		}

		public void SetDefaultValue(Property r)
		{
			object defaultValue = r.GetDefaultValue();
			this.SetDefaultValue(defaultValue);
		}

		public void SetDefaultValue(object v)
		{
			this.m_bValidDefaultValue = true;
			this.m_defaultValue = v;
		}

		public bool SetDefaultValue(string valStr)
		{
			Type type = null;
			if (this.m_memberBase != null)
			{
				type = this.m_memberBase.MemberType;
			}
			else if (this.m_bValidDefaultValue)
			{
				type = this.m_defaultValue.GetType();
			}
			this.m_defaultValue = StringUtils.FromString(type, valStr, false);
			if (this.m_defaultValue != null)
			{
				this.m_bValidDefaultValue = true;
				return true;
			}
			return false;
		}

		public bool SetDefaultValue(string valStr, Type type)
		{
			this.m_defaultValue = StringUtils.FromString(type, valStr, false);
			if (this.m_defaultValue != null)
			{
				this.m_bValidDefaultValue = true;
				return true;
			}
			return false;
		}

		private object GetDefaultValue()
		{
			return this.m_defaultValue;
		}

		public void SetValue(Agent parent, object v)
		{
			string staticClassName = null;
			if (this.m_memberBase != null)
			{
				if (this.m_pt == ParentType.PT_INSTANCE)
				{
					Agent instance = Agent.GetInstance(this.GetInstanceNameString(), parent.GetContextId());
					parent = instance;
				}
				if (this.m_memberBase.ISSTATIC())
				{
					staticClassName = this.m_memberBase.GetClassNameString();
				}
			}
			parent.SetVariableRegistry(this.m_memberBase, this.m_varaibleName, v, staticClassName, this.m_variableId);
		}

		public object GetValue(Agent parent)
		{
			if (parent == null || this.m_bIsConst)
			{
				return this.GetDefaultValue();
			}
			if (this.m_memberBase != null)
			{
				if (this.m_pt == ParentType.PT_INSTANCE)
				{
					Agent instance = Agent.GetInstance(this.GetInstanceNameString(), parent.GetContextId());
					parent = instance;
				}
				return this.m_memberBase.Get(parent);
			}
			return parent.GetVariable(this.m_varaibleName);
		}

		public object GetValue(Agent parent, Agent parHolder)
		{
			if (parent == null || this.m_bIsConst)
			{
				return this.GetDefaultValue();
			}
			if (this.m_memberBase != null)
			{
				if (this.m_pt == ParentType.PT_INSTANCE)
				{
					Agent instance = Agent.GetInstance(this.GetInstanceNameString(), parHolder.GetContextId());
					parHolder = instance;
				}
				return this.m_memberBase.Get(parHolder);
			}
			return parHolder.GetVariable(this.m_varaibleName);
		}

		public ParentType GetParentType()
		{
			return this.m_pt;
		}

		public void SetRefName(string refParName)
		{
			this.m_refParName = refParName;
			this.m_refParNameId = Utils.MakeVariableId(this.m_refParName);
		}

		public string GetRefName()
		{
			return this.m_refParName;
		}

		public uint GetRefNameId()
		{
			return this.m_refParNameId;
		}

		private static string ParseInstanceNameProperty(string fullName, ref string instanceName)
		{
			int num = fullName.IndexOf('.');
			if (num != -1)
			{
				instanceName = fullName.Substring(0, num).Replace("::", ".");
				return fullName.Substring(num + 1);
			}
			return fullName;
		}

		public static Property Create(string typeName, string variableName, string value, bool bStatic, bool bConst)
		{
			if (!bConst)
			{
				string instanceName = null;
				bool flag = Utils.IsParVar(variableName);
				string text;
				if (flag)
				{
					text = string.Format("{0}::{1}", typeName, variableName);
				}
				else
				{
					text = Property.ParseInstanceNameProperty(variableName, ref instanceName);
				}
				bool flag2 = false;
				if (!string.IsNullOrEmpty(text))
				{
					flag2 = Property.ms_properties.ContainsKey(text);
				}
				if (!flag2)
				{
					Property property = Property.create(flag, bConst, typeName, text, instanceName, value);
					if (!string.IsNullOrEmpty(text))
					{
						Property.ms_properties[text] = property;
					}
					return property.clone();
				}
				Property property2 = Property.ms_properties[text];
				Property property3 = property2.clone();
				if (!string.IsNullOrEmpty(value) && !Property.IsAgentPtr(typeName, value))
				{
					property3.SetDefaultValue(value);
				}
				return property3;
			}
			else
			{
				CMemberBase cMemberBase = null;
				bool flag3 = !string.IsNullOrEmpty(variableName);
				string instanceName2 = null;
				string text2 = variableName;
				if (flag3)
				{
					text2 = Property.ParseInstanceNameProperty(variableName, ref instanceName2);
					cMemberBase = Agent.FindMemberBase(text2);
				}
				if (cMemberBase != null)
				{
					return cMemberBase.CreateProperty(value, true);
				}
				bool bParVar = flag3 && Utils.IsParVar(variableName);
				return Property.create(bParVar, bConst, typeName, text2, instanceName2, value);
			}
		}

		public static Property Create(string value, CMemberBase pMemberBase, bool bConst)
		{
			Property property = new Property(pMemberBase, bConst);
			if (string.IsNullOrEmpty(value) || property.SetDefaultValue(value))
			{
			}
			return property;
		}

		public static void DeleteFromCache(Property property_)
		{
			string variableFullName = property_.GetVariableFullName();
			if (!string.IsNullOrEmpty(variableFullName) && Property.ms_properties.ContainsKey(variableFullName))
			{
				Property.ms_properties.Remove(variableFullName);
			}
		}

		public static void Cleanup()
		{
			Property.ms_properties.Clear();
		}

		private static Type GetTypeFromName(string typeName)
		{
			if (typeName == "void*")
			{
				return typeof(Agent);
			}
			Type type = Agent.GetTypeFromName(typeName);
			if (type == null)
			{
				typeName = typeName.Replace("::", ".");
				type = Utils.GetType(typeName);
				if (type == null)
				{
					type = Utils.GetTypeFromName(typeName);
				}
			}
			return type;
		}

		private static bool IsAgentPtr(string typeName, string valueStr)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(valueStr) && valueStr == "null")
			{
				result = true;
			}
			return result;
		}

		private static Property create(bool bParVar, bool bConst, string typeName, string variableName, string instanceName, string valueStr)
		{
			bool flag = !string.IsNullOrEmpty(variableName);
			if (flag && !bParVar)
			{
				Property property = Agent.CreateProperty(typeName, variableName, valueStr);
				if (flag && property != null && !bConst)
				{
					property.SetVariableName(variableName);
					property.SetInstanceNameString(instanceName);
				}
				return property;
			}
			bool flag2 = Property.IsAgentPtr(typeName, valueStr);
			Property property2 = new Property(null, bConst);
			object defaultValue;
			if (!flag2)
			{
				bool flag3 = false;
				if (typeName.StartsWith("vector<"))
				{
					flag3 = true;
				}
				Type typeFromName;
				if (flag3)
				{
					int num = typeName.IndexOf('<');
					int num2 = typeName.IndexOf('>');
					int num3 = num2 - num - 1;
					string typeName2 = typeName.Substring(num + 1, num3);
					typeFromName = Property.GetTypeFromName(typeName2);
				}
				else
				{
					typeFromName = Property.GetTypeFromName(typeName);
				}
				defaultValue = StringUtils.FromString(typeFromName, valueStr, flag3);
			}
			else
			{
				defaultValue = null;
			}
			property2.SetDefaultValue(defaultValue);
			if (flag && property2 != null && !bConst)
			{
				property2.SetVariableName(variableName);
			}
			return property2;
		}
	}
}
