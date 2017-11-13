using System;

namespace Assets.Scripts.GameLogic
{
	public class EnergyCreater<CreateType, CreateTypeAttribute> : Singleton<EnergyCreater<CreateType, CreateTypeAttribute>> where CreateTypeAttribute : EnergyAttribute
	{
		private DictionaryView<int, Type> energyTypeSet = new DictionaryView<int, Type>();

		public override void Init()
		{
			ClassEnumerator classEnumerator = new ClassEnumerator(typeof(CreateTypeAttribute), typeof(CreateType), typeof(CreateTypeAttribute).get_Assembly(), true, false, false);
			foreach (Type current in classEnumerator.results)
			{
				Attribute customAttribute = Attribute.GetCustomAttribute(current, typeof(CreateTypeAttribute));
				this.energyTypeSet.Add((customAttribute as CreateTypeAttribute).attributeType, current);
			}
		}

		public CreateType Create(int _type)
		{
			Type type;
			if (this.energyTypeSet.TryGetValue(_type, out type))
			{
				return (CreateType)((object)Activator.CreateInstance(type));
			}
			return default(CreateType);
		}
	}
}
