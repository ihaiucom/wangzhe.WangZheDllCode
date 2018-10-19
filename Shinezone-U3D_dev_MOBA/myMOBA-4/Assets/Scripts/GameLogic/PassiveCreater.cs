using System;

namespace Assets.Scripts.GameLogic
{
	public class PassiveCreater<CreateType, CreateTypeAttribute> : Singleton<PassiveCreater<CreateType, CreateTypeAttribute>> where CreateTypeAttribute : PassivetAttribute
	{
		private DictionaryView<int, Type> eventTypeSet = new DictionaryView<int, Type>();

		public override void Init()
		{
			ClassEnumerator classEnumerator = new ClassEnumerator(typeof(CreateTypeAttribute), typeof(CreateType), typeof(CreateTypeAttribute).Assembly, true, false, false);
			foreach (Type current in classEnumerator.results)
			{
				Attribute customAttribute = Attribute.GetCustomAttribute(current, typeof(CreateTypeAttribute));
				this.eventTypeSet.Add((customAttribute as CreateTypeAttribute).attributeType, current);
			}
		}

		public CreateType Create(int _type)
		{
			Type type;
			if (this.eventTypeSet.TryGetValue(_type, out type))
			{
				return (CreateType)((object)Activator.CreateInstance(type));
			}
			return default(CreateType);
		}
	}
}
