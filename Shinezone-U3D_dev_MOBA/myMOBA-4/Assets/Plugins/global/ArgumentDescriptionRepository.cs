using System;
using System.Collections.Generic;

public class ArgumentDescriptionRepository : Singleton<ArgumentDescriptionRepository>
{
	private class Greater : IComparer<int>
	{
		public int Compare(int x, int y)
		{
			return (x <= y) ? ((x != y) ? 1 : 0) : -1;
		}
	}

	public SortedList<int, IArgumentDescription> Descriptions = new SortedList<int, IArgumentDescription>(new ArgumentDescriptionRepository.Greater());

	public ArgumentDescriptionRepository()
	{
		ClassEnumerator classEnumerator = new ClassEnumerator(typeof(ArgumentAttribute), typeof(IArgumentDescription), typeof(ArgumentAttribute).Assembly, true, false, false);
		ListView<Type>.Enumerator enumerator = classEnumerator.results.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Type current = enumerator.Current;
			ArgumentAttribute argumentAttribute = current.GetCustomAttributes(typeof(ArgumentAttribute), false)[0] as ArgumentAttribute;
			IArgumentDescription value = Activator.CreateInstance(current) as IArgumentDescription;
			this.Descriptions.Add(argumentAttribute.order, value);
		}
	}

	public IArgumentDescription GetDescription(Type InType)
	{
		IEnumerator<KeyValuePair<int, IArgumentDescription>> enumerator = this.Descriptions.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, IArgumentDescription> current = enumerator.Current;
			if (current.Value.Accept(InType))
			{
				KeyValuePair<int, IArgumentDescription> current2 = enumerator.Current;
				return current2.Value;
			}
		}
		DebugHelper.Assert(false, "can't find valid description for {0}, internal error!", new object[]
		{
			InType.Name
		});
		return null;
	}
}
