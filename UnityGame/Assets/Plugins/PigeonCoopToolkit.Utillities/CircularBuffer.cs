using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace PigeonCoopToolkit.Utillities
{
	public class CircularBuffer<T> : IEnumerable, IList<T>, ICollection<T>, IEnumerable<T>
	{
		private T[] _buffer;

		private int _position;

		private long _version;

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new IndexOutOfRangeException();
				}
				int num = (this._position - this.Count + index) % this.Capacity;
				return this._buffer[num];
			}
			set
			{
				this.Insert(index, value);
			}
		}

		public int Capacity
		{
			get;
			private set;
		}

		public int Count
		{
			get;
			private set;
		}

		public CircularBuffer(int capacity)
		{
			if (capacity <= 0)
			{
				throw new ArgumentException("Must be greater than zero", "capacity");
			}
			this.Capacity = capacity;
			this._buffer = new T[capacity];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Add(T item)
		{
			this._buffer[this._position++ % this.Capacity] = item;
			if (this.Count < this.Capacity)
			{
				this.Count++;
			}
			this._version += 1L;
		}

		public void Clear()
		{
			for (int i = 0; i < this.Count; i++)
			{
				this._buffer[i] = default(T);
			}
			this._position = 0;
			this.Count = 0;
			this._version += 1L;
		}

		public bool Contains(T item)
		{
			int num = this.IndexOf(item);
			return num != -1;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			for (int i = 0; i < this.Count; i++)
			{
				array[i + arrayIndex] = this._buffer[(this._position - this.Count + i) % this.Capacity];
			}
		}

		[DebuggerHidden]
		public IEnumerator<T> GetEnumerator()
		{
			CircularBuffer<T>.<GetEnumerator>c__Iterator1 <GetEnumerator>c__Iterator = new CircularBuffer<T>.<GetEnumerator>c__Iterator1();
			<GetEnumerator>c__Iterator.<>f__this = this;
			return <GetEnumerator>c__Iterator;
		}

		public int IndexOf(T item)
		{
			for (int i = 0; i < this.Count; i++)
			{
				T t = this._buffer[(this._position - this.Count + i) % this.Capacity];
				if (item == null && t == null)
				{
					return i;
				}
				if (item != null && item.Equals(t))
				{
					return i;
				}
			}
			return -1;
		}

		public void Insert(int index, T item)
		{
			if (index < 0 || index > this.Count)
			{
				throw new IndexOutOfRangeException();
			}
			if (index == this.Count)
			{
				this.Add(item);
				return;
			}
			int num = Math.Min(this.Count, this.Capacity - 1) - index;
			int num2 = (this._position - this.Count + index) % this.Capacity;
			for (int i = num2 + num; i > num2; i--)
			{
				int num3 = i % this.Capacity;
				int num4 = (i - 1) % this.Capacity;
				this._buffer[num3] = this._buffer[num4];
			}
			this._buffer[num2] = item;
			if (this.Count < this.Capacity)
			{
				this.Count++;
				this._position++;
			}
			this._version += 1L;
		}

		public bool Remove(T item)
		{
			int num = this.IndexOf(item);
			if (num == -1)
			{
				return false;
			}
			this.RemoveAt(num);
			return true;
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this.Count)
			{
				throw new IndexOutOfRangeException();
			}
			for (int i = index; i < this.Count - 1; i++)
			{
				int num = (this._position - this.Count + i) % this.Capacity;
				int num2 = (this._position - this.Count + i + 1) % this.Capacity;
				this._buffer[num] = this._buffer[num2];
			}
			int num3 = (this._position - 1) % this.Capacity;
			this._buffer[num3] = default(T);
			this._position--;
			this.Count--;
			this._version += 1L;
		}
	}
}
