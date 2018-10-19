using System;
using System.Threading;

namespace com.tencent.pandora
{
	public class LockFreeQueue<T>
	{
		public int head;

		public int tail;

		public T[] items;

		private int capacity;

		public int Count
		{
			get
			{
				return this.tail - this.head;
			}
		}

		public LockFreeQueue() : this(64)
		{
		}

		public LockFreeQueue(int count)
		{
			this.items = new T[count];
			this.tail = (this.head = 0);
			this.capacity = count;
		}

		public bool IsEmpty()
		{
			return this.head == this.tail;
		}

		public void Clear()
		{
			this.head = (this.tail = 0);
		}

		private bool IsFull()
		{
			return this.tail - this.head >= this.capacity;
		}

		public void Enqueue(T item)
		{
			while (this.IsFull())
			{
				Thread.Sleep(1);
			}
			int num = this.tail % this.capacity;
			this.items[num] = item;
			this.tail++;
		}

		public T Dequeue()
		{
			if (this.IsEmpty())
			{
				return default(T);
			}
			int num = this.head % this.capacity;
			T result = this.items[num];
			this.head++;
			return result;
		}
	}
}
