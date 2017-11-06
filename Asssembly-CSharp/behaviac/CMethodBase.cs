using System;
using System.Collections.Generic;
using System.Reflection;

namespace behaviac
{
	public class CMethodBase
	{
		private struct Param_t
		{
			public Property paramProperty;

			public DictionaryView<string, Property> paramStructMembers;
		}

		private MethodMetaInfoAttribute descAttrbute_;

		private MethodBase method_;

		private string m_name;

		private CStringID m_id = default(CStringID);

		private string m_instanceName;

		private ParentType m_parentType;

		private object[] m_param_values;

		private CMethodBase.Param_t[] m_params;

		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public CMethodBase(MethodBase m, MethodMetaInfoAttribute a, string methodNameOverride)
		{
			this.method_ = m;
			this.descAttrbute_ = a;
			this.m_name = ((!string.IsNullOrEmpty(methodNameOverride)) ? methodNameOverride : this.method_.get_Name());
			this.m_id.SetId(this.m_name);
		}

		protected CMethodBase(CMethodBase copy)
		{
			this.m_instanceName = copy.m_instanceName;
			this.m_parentType = copy.m_parentType;
			this.method_ = copy.method_;
			this.descAttrbute_ = copy.descAttrbute_;
			this.m_name = copy.m_name;
			this.m_id = copy.m_id;
		}

		public ParentType GetParentType()
		{
			return this.m_parentType;
		}

		public CStringID GetId()
		{
			return this.m_id;
		}

		public string GetInstanceNameString()
		{
			return this.m_instanceName;
		}

		public void SetInstanceNameString(string agentInstanceName, ParentType pt)
		{
			this.m_instanceName = agentInstanceName;
			this.m_parentType = pt;
		}

		public virtual Property CreateProperty(string defaultValue, bool bConst)
		{
			return Property.Create(defaultValue, null, bConst);
		}

		public void Load(Agent parent, List<string> paramsToken)
		{
			ParameterInfo[] parameters = this.method_.GetParameters();
			this.m_param_values = new object[parameters.Length];
			if (paramsToken.get_Count() == parameters.Length)
			{
				this.m_params = new CMethodBase.Param_t[parameters.Length];
				for (int i = 0; i < paramsToken.get_Count(); i++)
				{
					ParameterInfo parameterInfo = parameters[i];
					bool flag = paramsToken.get_Item(i).get_Chars(0) == '{';
					if (flag)
					{
						DictionaryView<string, Property> dictionaryView = new DictionaryView<string, Property>();
						string empty = string.Empty;
						if (StringUtils.ParseForStruct(parameterInfo.get_ParameterType(), paramsToken.get_Item(i), ref empty, dictionaryView))
						{
							object obj = StringUtils.FromString(parameterInfo.get_ParameterType(), empty, false);
							this.m_param_values[i] = obj;
							this.m_params[i].paramStructMembers = dictionaryView;
						}
					}
					else
					{
						bool flag2 = paramsToken.get_Item(i).get_Chars(0) == '"';
						if (flag2 || paramsToken.get_Item(i).IndexOf(' ') == -1)
						{
							string valStr = flag2 ? paramsToken.get_Item(i).Substring(1, paramsToken.get_Item(i).get_Length() - 2) : paramsToken.get_Item(i);
							object obj2 = StringUtils.FromString(parameterInfo.get_ParameterType(), valStr, false);
							this.m_param_values[i] = obj2;
						}
						else
						{
							string[] array = paramsToken.get_Item(i).Split(new char[]
							{
								' '
							});
							if (array.Length == 2)
							{
								string typeName = array[0].Replace("::", ".");
								Property paramProperty = Property.Create(typeName, array[1], null, false, false);
								this.m_params[i].paramProperty = paramProperty;
							}
							else if (array.Length == 3)
							{
								string typeName2 = array[1].Replace("::", ".");
								Property paramProperty2 = Property.Create(typeName2, array[2], null, true, false);
								this.m_params[i].paramProperty = paramProperty2;
							}
						}
					}
				}
			}
		}

		public bool IsStatic()
		{
			return false;
		}

		public string GetName()
		{
			return this.m_name;
		}

		public string GetClassNameString()
		{
			if (this.IsNamedEvent())
			{
				return this.method_.get_DeclaringType().get_DeclaringType().get_FullName();
			}
			return this.method_.get_DeclaringType().get_FullName();
		}

		public virtual bool IsNamedEvent()
		{
			return false;
		}

		public virtual CMethodBase clone()
		{
			return new CMethodBase(this);
		}

		public object run(Agent parent, Agent parHolder)
		{
			if (this.m_params != null)
			{
				for (int i = 0; i < this.m_params.Length; i++)
				{
					Property paramProperty = this.m_params[i].paramProperty;
					if (paramProperty != null)
					{
						this.m_param_values[i] = paramProperty.GetValue(parent, parHolder);
					}
					if (this.m_params[i].paramStructMembers != null)
					{
						Type type = this.m_param_values[i].GetType();
						Agent.CTagObjectDescriptor descriptorByName = Agent.GetDescriptorByName(type.get_FullName());
						foreach (KeyValuePair<string, Property> current in this.m_params[i].paramStructMembers)
						{
							CMemberBase member = descriptorByName.GetMember(current.get_Key());
							if (member != null)
							{
								object value = current.get_Value().GetValue(parent, parHolder);
								member.Set(this.m_param_values[i], value);
							}
						}
					}
				}
			}
			object result = this.method_.Invoke(parent, this.m_param_values);
			if (this.m_params != null)
			{
				for (int j = 0; j < this.m_params.Length; j++)
				{
					Property paramProperty2 = this.m_params[j].paramProperty;
					if (paramProperty2 != null)
					{
						object v = this.m_param_values[j];
						paramProperty2.SetValue(parHolder, v);
					}
					if (this.m_params[j].paramStructMembers != null)
					{
						Type type2 = this.m_param_values[j].GetType();
						Agent.CTagObjectDescriptor descriptorByName2 = Agent.GetDescriptorByName(type2.get_FullName());
						foreach (KeyValuePair<string, Property> current2 in this.m_params[j].paramStructMembers)
						{
							CMemberBase member2 = descriptorByName2.GetMember(current2.get_Key());
							if (member2 != null)
							{
								object v2 = member2.Get(this.m_param_values[j]);
								current2.get_Value().SetValue(parHolder, v2);
							}
						}
					}
				}
			}
			return result;
		}

		public object run(Agent parent, Agent parHolder, object param)
		{
			if (this.m_param_values.Length == 1)
			{
				this.m_param_values[0] = param;
			}
			return this.method_.Invoke(parent, this.m_param_values);
		}
	}
}
