using System;

namespace AGE
{
	public class SObjPool<T> where T : new()
	{
		public static ListView<T> list = new ListView<T>();

		private static int allocNum = 0;

		public static T New()
		{
			T result;
			if (SObjPool<T>.list.Count > 0)
			{
				int index = SObjPool<T>.list.Count - 1;
				result = SObjPool<T>.list[index];
				SObjPool<T>.list.RemoveAt(index);
			}
			else
			{
				SObjPool<T>.allocNum++;
				result = ((default(T) != null) ? default(T) : Activator.CreateInstance<T>());
			}
			return result;
		}

		public static void Delete(T v)
		{
			SObjPool<T>.list.Add(v);
		}

		public static void Alloc(int num)
		{
			int num2 = num - SObjPool<T>.list.Count;
			for (int i = 0; i < num2; i++)
			{
				SObjPool<T>.allocNum++;
				SObjPool<T>.list.Add((default(T) != null) ? default(T) : Activator.CreateInstance<T>());
			}
		}
	}
}
