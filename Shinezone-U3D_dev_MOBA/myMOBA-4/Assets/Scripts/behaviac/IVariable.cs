using System;
using System.Reflection;

namespace behaviac
{
	public class IVariable
	{
		protected uint m_id;

		protected string m_name;

		protected Property m_property;

		protected CMemberBase m_pMember;

		public byte m_instantiated;

		private object m_value;

		public IVariable(CMemberBase pMember, string variableName, uint id)
		{
			this.m_id = id;
			this.m_name = variableName;
			this.m_property = null;
			this.m_pMember = pMember;
			this.m_instantiated = 1;
		}

		public IVariable(CMemberBase pMember, Property property_)
		{
			this.m_property = property_;
			this.m_pMember = pMember;
			this.m_instantiated = 1;
			this.m_name = this.m_property.GetVariableName();
			this.m_id = this.m_property.GetVariableId();
		}

		public IVariable(IVariable copy)
		{
			this.m_id = copy.m_id;
			this.m_name = copy.m_name;
			this.m_property = copy.m_property;
			this.m_pMember = copy.m_pMember;
			this.m_instantiated = copy.m_instantiated;
			this.m_value = copy.m_value;
		}

		~IVariable()
		{
		}

		public uint GetId()
		{
			return this.m_id;
		}

		public Property GetProperty()
		{
			return this.m_property;
		}

		public void SetProperty(Property p)
		{
			if (p != null)
			{
			}
			this.m_property = p;
		}

		public object GetValue(Agent pAgent)
		{
			if (this.m_pMember != null)
			{
				return this.m_pMember.Get(pAgent);
			}
			return this.m_value;
		}

		private static void DeepCopy(out object result, object obj)
		{
			if (obj == null)
			{
				result = obj;
				return;
			}
			Type type = obj.GetType();
			if (type.IsValueType || type == typeof(string))
			{
				result = obj;
				return;
			}
			if (type.IsArray)
			{
				Type type2 = type.GetElementType();
				if (type2 == null)
				{
					type2 = Utility.GetType(type.FullName.Replace("[]", string.Empty));
				}
				Array array = obj as Array;
				Array array2 = Array.CreateInstance(type2, array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					object value;
					IVariable.DeepCopy(out value, array.GetValue(i));
					array2.SetValue(value, i);
				}
				result = Convert.ChangeType(array2, type);
			}
			else if (type.IsClass)
			{
				result = obj;
				bool flag = type == typeof(Agent) || type.IsSubclassOf(typeof(Agent));
				if (flag)
				{
					result = obj;
					return;
				}
				object obj2 = Activator.CreateInstance(type);
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				FieldInfo[] array3 = fields;
				for (int j = 0; j < array3.Length; j++)
				{
					FieldInfo fieldInfo = array3[j];
					object value2 = fieldInfo.GetValue(obj);
					if (value2 != null)
					{
						object value3;
						IVariable.DeepCopy(out value3, value2);
						fieldInfo.SetValue(obj2, value3);
					}
				}
				result = obj2;
			}
			else
			{
				result = obj;
			}
		}

		public void SetValue(object value, Agent pAgent)
		{
			bool flag = false;
			if (this.m_pMember != null)
			{
				this.m_pMember.Set(pAgent, value);
				flag = true;
			}
			if (!flag && !Details.Equal(this.m_value, value))
			{
				this.m_value = value;
			}
		}

		public void Log(Agent pAgent)
		{
			string value = StringUtils.ToString(this.m_value);
			string typeName = string.Empty;
			if (!object.ReferenceEquals(this.m_value, null))
			{
				typeName = Utils.GetNativeTypeName(this.m_value.GetType());
			}
			else
			{
				typeName = "Agent";
			}
			string varName = this.m_name;
			if (!object.ReferenceEquals(pAgent, null))
			{
				CMemberBase cMemberBase = pAgent.FindMember(this.m_name);
				if (cMemberBase != null)
				{
					string arg = cMemberBase.GetClassNameString().Replace(".", "::");
					varName = string.Format("{0}::{1}", arg, this.m_name);
				}
			}
			LogManager.Log(pAgent, typeName, varName, value);
		}

		public void Reset()
		{
		}

		public IVariable clone()
		{
			return new IVariable(this);
		}

		public void CopyTo(Agent pAgent)
		{
			if (this.m_pMember != null)
			{
				this.m_pMember.Set(pAgent, this.m_value);
			}
		}

		public void Save(ISerializableNode node)
		{
			CSerializationID chidlId = new CSerializationID("var");
			ISerializableNode serializableNode = node.newChild(chidlId);
			CSerializationID attrId = new CSerializationID("name");
			serializableNode.setAttr(attrId, this.m_name);
			CSerializationID attrId2 = new CSerializationID("value");
			serializableNode.setAttr<object>(attrId2, this.m_value);
		}

		public void Load(ISerializableNode node)
		{
		}

		public void SetFromString(Agent pAgent, CMemberBase pMember, string valueString)
		{
			if (!string.IsNullOrEmpty(valueString) && pMember != null)
			{
				object obj = StringUtils.FromString(pMember.MemberType, valueString, false);
				if (!Details.Equal(this.m_value, obj))
				{
					this.m_value = obj;
					if (!object.ReferenceEquals(pAgent, null) && pMember != null)
					{
						pMember.Set(pAgent, obj);
					}
				}
			}
		}
	}
}
