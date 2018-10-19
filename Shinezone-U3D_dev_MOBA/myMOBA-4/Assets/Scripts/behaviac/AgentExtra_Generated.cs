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
			string key = type.FullName + property;
			if (AgentExtra_Generated._fields.ContainsKey(key))
			{
				return AgentExtra_Generated._fields[key].GetValue(agent);
			}
			if (AgentExtra_Generated._properties.ContainsKey(key))
			{
				return AgentExtra_Generated._properties[key].GetValue(agent, null);
			}
			while (type != typeof(object))
			{
				FieldInfo field = type.GetField(property, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (field != null)
				{
					AgentExtra_Generated._fields[key] = field;
					return field.GetValue(agent);
				}
				PropertyInfo property2 = type.GetProperty(property, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (property2 != null)
				{
					AgentExtra_Generated._properties[key] = property2;
					return property2.GetValue(agent, null);
				}
				type = type.BaseType;
			}
			return null;
		}

		public static void SetProperty(Agent agent, string property, object value)
		{
			Type type = agent.GetType();
			string key = type.FullName + property;
			if (AgentExtra_Generated._fields.ContainsKey(key))
			{
				AgentExtra_Generated._fields[key].SetValue(agent, value);
				return;
			}
			if (AgentExtra_Generated._properties.ContainsKey(key))
			{
				AgentExtra_Generated._properties[key].SetValue(agent, value, null);
				return;
			}
			while (type != typeof(object))
			{
				FieldInfo field = type.GetField(property, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (field != null)
				{
					AgentExtra_Generated._fields[key] = field;
					field.SetValue(agent, value);
					return;
				}
				PropertyInfo property2 = type.GetProperty(property, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (property2 != null)
				{
					AgentExtra_Generated._properties[key] = property2;
					property2.SetValue(agent, value, null);
					return;
				}
				type = type.BaseType;
			}
		}

		public static object ExecuteMethod(Agent agent, string method, object[] args)
		{
			Type type = agent.GetType();
			string key = type.FullName + method;
			if (AgentExtra_Generated._methods.ContainsKey(key))
			{
				return AgentExtra_Generated._methods[key].Invoke(agent, args);
			}
			while (type != typeof(object))
			{
				MethodInfo method2 = type.GetMethod(method, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (method2 != null)
				{
					AgentExtra_Generated._methods[key] = method2;
					return method2.Invoke(agent, args);
				}
				type = type.BaseType;
			}
			return null;
		}
	}
}
