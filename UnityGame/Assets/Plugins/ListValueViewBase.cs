using System;

public abstract class ListValueViewBase
{
	protected const int DefaultCapacity = 4;

	protected int _size;

	protected int _version;

	public int Count
	{
		get
		{
			return this._size;
		}
	}
}
