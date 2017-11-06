using System;
using System.Globalization;
using System.Reflection;

namespace com.tencent.pandora
{
	public class ProxyType : IReflect
	{
		private Type proxy;

		public Type UnderlyingSystemType
		{
			get
			{
				return this.proxy;
			}
		}

		public ProxyType(Type proxy)
		{
			this.proxy = proxy;
		}

		public override string ToString()
		{
			return "ProxyType(" + this.UnderlyingSystemType + ")";
		}

		public FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			return this.proxy.GetField(name, bindingAttr);
		}

		public FieldInfo[] GetFields(BindingFlags bindingAttr)
		{
			return this.proxy.GetFields(bindingAttr);
		}

		public MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
		{
			return this.proxy.GetMember(name, bindingAttr);
		}

		public MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			return this.proxy.GetMembers(bindingAttr);
		}

		public MethodInfo GetMethod(string name, BindingFlags bindingAttr)
		{
			return this.proxy.GetMethod(name, bindingAttr);
		}

		public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
		{
			return this.proxy.GetMethod(name, bindingAttr, binder, types, modifiers);
		}

		public MethodInfo[] GetMethods(BindingFlags bindingAttr)
		{
			return this.proxy.GetMethods(bindingAttr);
		}

		public PropertyInfo GetProperty(string name, BindingFlags bindingAttr)
		{
			return this.proxy.GetProperty(name, bindingAttr);
		}

		public PropertyInfo GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			return this.proxy.GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
		}

		public PropertyInfo[] GetProperties(BindingFlags bindingAttr)
		{
			return this.proxy.GetProperties(bindingAttr);
		}

		public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			return this.proxy.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
		}
	}
}
