using System;
using System.Collections;
using System.Collections.Generic;

public class ListValueView<T> : ListValueViewBase, IEnumerable, IEnumerable<T>
{
	[Serializable]
	public struct Enumerator : IDisposable, IEnumerator, IEnumerator<T>
	{
		private ListValueView<T> l;

		private int next;

		private int ver;

		private T current;

		object IEnumerator.Current
		{
			get
			{
				this.VerifyState();
				if (this.next <= 0)
				{
					throw new InvalidOperationException();
				}
				return this.current;
			}
		}

		public T Current
		{
			get
			{
				return this.current;
			}
		}

		internal Enumerator(ListValueView<T> l)
		{
			this.l = l;
			this.ver = l._version;
			this.next = 0;
			this.current = l._items[0];
		}

		void IEnumerator.Reset()
		{
			this.VerifyState();
			this.next = 0;
		}

		public void Dispose()
		{
			this.l = null;
		}

		private void VerifyState()
		{
			if (this.l == null)
			{
				throw new ObjectDisposedException(base.GetType().get_FullName());
			}
			if (this.ver != this.l._version)
			{
				throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
			}
		}

		public bool MoveNext()
		{
			this.VerifyState();
			if (this.next < 0)
			{
				return false;
			}
			if (this.next < this.l._size)
			{
				this.current = this.l._items[this.next++];
				return true;
			}
			this.next = -1;
			return false;
		}
	}

	private T[] _items;

	private static readonly T[] EmptyArray = new T[0];

	public int Capacity
	{
		get
		{
			return this._items.Length;
		}
		set
		{
			if (value < this._size)
			{
				throw new ArgumentOutOfRangeException();
			}
			Array.Resize<T>(ref this._items, value);
		}
	}

	public T this[int index]
	{
		get
		{
			if (index >= this._size)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return this._items[index];
		}
		set
		{
			if (index < 0 || index > this._size)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (index == this._size)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this._items[index] = value;
		}
	}

	public ListValueView()
	{
		this._items = ListValueView<T>.EmptyArray;
	}

	public ListValueView(int capacity)
	{
		if (capacity < 0)
		{
			throw new ArgumentOutOfRangeException("capacity");
		}
		this._items = new T[capacity];
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	private void GrowIfNeeded(int newCount)
	{
		int num = this._size + newCount;
		if (num > this._items.Length)
		{
			this.Capacity = Math.Max(Math.Max(this.Capacity * 2, 4), num);
		}
	}

	public void Add(T item)
	{
		if (this._size == this._items.Length)
		{
			this.GrowIfNeeded(1);
		}
		this._items[this._size++] = item;
		this._version++;
	}

	public void Clear()
	{
		Array.Clear(this._items, 0, this._items.Length);
		this._size = 0;
		this._version++;
	}

	private void Shift(int start, int delta)
	{
		if (delta < 0)
		{
			start -= delta;
		}
		if (start < this._size)
		{
			Array.Copy(this._items, start, this._items, start + delta, this._size - start);
		}
		this._size += delta;
		if (delta < 0)
		{
			Array.Clear(this._items, this._size, -delta);
		}
	}

	public void RemoveAt(int index)
	{
		if (index < 0 || index >= this._size)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		this.Shift(index, -1);
		Array.Clear(this._items, this._size, 1);
		this._version++;
	}

	private void CheckRange(int idx, int count)
	{
		if (idx < 0)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		if (idx + count > this._size)
		{
			throw new ArgumentException("index and count exceed length of list");
		}
	}

	public void RemoveRange(int index, int count)
	{
		this.CheckRange(index, count);
		if (count > 0)
		{
			this.Shift(index, -count);
			Array.Clear(this._items, this._size, count);
			this._version++;
		}
	}

	public int IndexOf(T item)
	{
		return Array.IndexOf<T>(this._items, item, 0, this._size);
	}

	public int IndexOf(T item, int index)
	{
		if (index < 0 || index > this._size)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		return Array.IndexOf<T>(this._items, item, index, this._size - index);
	}

	public int IndexOf(T item, int index, int count)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		if (index + count > this._size)
		{
			throw new ArgumentOutOfRangeException("index and count exceed length of list");
		}
		return Array.IndexOf<T>(this._items, item, index, count);
	}

	public ListValueView<T>.Enumerator GetEnumerator()
	{
		return new ListValueView<T>.Enumerator(this);
	}
}
