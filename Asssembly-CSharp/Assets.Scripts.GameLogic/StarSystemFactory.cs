using System;
using System.Reflection;

namespace Assets.Scripts.GameLogic
{
	public class StarSystemFactory
	{
		private DictionaryView<int, Type> Factories = new DictionaryView<int, Type>();

		public StarSystemFactory(Type InAttributeType, Type InInterfaceType)
		{
			DebugHelper.Assert(InAttributeType != null && InInterfaceType != null);
			Assembly assembly = InInterfaceType.get_Assembly();
			Type[] types = assembly.GetTypes();
			int num = 0;
			while (types != null && num < types.Length)
			{
				Type type = types[num];
				object[] customAttributes = type.GetCustomAttributes(InAttributeType, true);
				if (customAttributes != null)
				{
					for (int i = 0; i < customAttributes.Length; i++)
					{
						IIdentifierAttribute<int> identifierAttribute = customAttributes[i] as IIdentifierAttribute<int>;
						if (identifierAttribute != null)
						{
							this.RegisterType(identifierAttribute.ID, type);
						}
					}
				}
				num++;
			}
		}

		private void RegisterType(int InKey, Type InType)
		{
			DebugHelper.Assert(!this.Factories.ContainsKey(InKey));
			this.Factories.Add(InKey, InType);
		}

		public object Create(int InKeyType)
		{
			Type type = null;
			if (this.Factories.TryGetValue(InKeyType, out type))
			{
				DebugHelper.Assert(type != null);
				return Activator.CreateInstance(type);
			}
			return null;
		}
	}
}
