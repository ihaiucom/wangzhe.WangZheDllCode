using System;
using System.Collections.Generic;
using System.Reflection;

namespace behaviac
{
	internal class AgentExtra_Generated
	{
		private static Dictionary<string, FieldInfo> _fields = new Dictionary<string, FieldInfo>();

		private static Dictionary<string, PropertyInfo> _properties = new Dictionary<string, PropertyInfo>();

		private static Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();

		public static object GetProperty(Agent agent, string property)
		{
			Type type = agent.GetType();
			string text = type.get_FullName() + property;
			if (AgentExtra_Generated._fields.ContainsKey(text))
			{
				return AgentExtra_Generated._fields.get_Item(text).GetValue(agent);
			}
			if (AgentExtra_Generated._properties.ContainsKey(text))
			{
				return AgentExtra_Generated._properties.get_Item(text).GetValue(agent, null);
			}
			while (type != typeof(object))
			{
				FieldInfo field = type.GetField(property, 60);
				if (field != null)
				{
					AgentExtra_Generated._fields.set_Item(text, field);
					return field.GetValue(agent);
				}
				PropertyInfo property2 = type.GetProperty(property, 60);
				if (property2 != null)
				{
					AgentExtra_Generated._properties.set_Item(text, property2);
					return property2.GetValue(agent, null);
				}
				type = type.get_BaseType();
			}
			return null;
		}

		public static void SetProperty(Agent agent, string property, object value)
		{
			Type type = agent.GetType();
			string text = type.get_FullName() + property;
			if (AgentExtra_Generated._fields.ContainsKey(text))
			{
				AgentExtra_Generated._fields.get_Item(text).SetValue(agent, value);
				return;
			}
			if (AgentExtra_Generated._properties.ContainsKey(text))
			{
				AgentExtra_Generated._properties.get_Item(text).SetValue(agent, value, null);
				return;
			}
			while (type != typeof(object))
			{
				FieldInfo field = type.GetField(property, 60);
				if (field != null)
				{
					AgentExtra_Generated._fields.set_Item(text, field);
					field.SetValue(agent, value);
					return;
				}
				PropertyInfo property2 = type.GetProperty(property, 60);
				if (property2 != null)
				{
					AgentExtra_Generated._properties.set_Item(text, property2);
					property2.SetValue(agent, value, null);
					return;
				}
				type = type.get_BaseType();
			}
		}

		public static object ExecuteMethod(Agent agent, string method, object[] args)
		{
			Type type = agent.GetType();
			string text = type.get_FullName() + method;
			if (AgentExtra_Generated._methods.ContainsKey(text))
			{
				return AgentExtra_Generated._methods.get_Item(text).Invoke(agent, args);
			}
			while (type != typeof(object))
			{
				MethodInfo method2 = type.GetMethod(method, 60);
				if (method2 != null)
				{
					AgentExtra_Generated._methods.set_Item(text, method2);
					return method2.Invoke(agent, args);
				}
				type = type.get_BaseType();
			}
			return null;
		}
	}
}
