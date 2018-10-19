using System;
using System.Collections.Generic;

public abstract class ListViewBase
{
	protected List<object> Context;

	public int Count
	{
		get
		{
			return this.Context.Count;
		}
	}

	public void Clear()
	{
		this.Context.Clear();
	}

	public void RemoveAt(int index)
	{
		this.Context.RemoveAt(index);
	}

	public void RemoveRange(int index, int count)
	{
		this.Context.RemoveRange(index, count);
	}

	public void Reverse()
	{
		this.Context.Reverse();
	}

	public void Sort()
	{
		this.Context.Sort();
	}
}
