using System;

public class ListLinqView<T> : ListView<T>
{
	public ListLinqView()
	{
	}

	public ListLinqView(int capacity) : base(capacity)
	{
	}

	public T[] ToArray()
	{
		T[] array = new T[base.Count];
		if (array.Length < this.Context.get_Count())
		{
			throw new ArgumentException("Input array has not enough size.");
		}
		for (int i = 0; i < this.Context.get_Count(); i++)
		{
			array[i] = (T)((object)this.Context.get_Item(i));
		}
		return array;
	}
}
