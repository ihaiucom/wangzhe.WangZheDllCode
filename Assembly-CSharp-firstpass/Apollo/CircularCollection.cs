using System;
using System.Collections.Generic;

namespace Apollo
{
	public class CircularCollection<T>
	{
		private int _nReadIndex;

		private List<object> dataCollection;

		private int nWriteIndex
		{
			get;
			set;
		}

		private int nReadIndex
		{
			get
			{
				return this._nReadIndex;
			}
			set
			{
				this._nReadIndex = value;
			}
		}

		public int Capacity
		{
			get
			{
				return this.dataCollection.get_Capacity();
			}
		}

		public int Count
		{
			get
			{
				if (this.nWriteIndex >= this.nReadIndex)
				{
					return this.nWriteIndex - this.nReadIndex;
				}
				return this.dataCollection.get_Capacity() + this.nWriteIndex - this.nReadIndex;
			}
		}

		public int Free
		{
			get
			{
				if (this.nReadIndex > this.nWriteIndex)
				{
					return this.nReadIndex - this.nWriteIndex;
				}
				return this.dataCollection.get_Capacity() + this.nReadIndex - this.nWriteIndex;
			}
		}

		public T Next
		{
			get
			{
				if (this.nReadIndex == this.nWriteIndex)
				{
					return default(T);
				}
				if (this.nReadIndex >= this.dataCollection.get_Capacity())
				{
					this.nReadIndex = 0;
				}
				object obj = this.dataCollection.get_Item(this.nReadIndex++);
				return (obj != null) ? ((T)((object)obj)) : default(T);
			}
		}

		public CircularCollection()
		{
			this.nWriteIndex = 0;
			this.nReadIndex = 0;
			this.dataCollection = new List<object>(50);
		}

		public CircularCollection(int capacity)
		{
			this.nWriteIndex = 0;
			this.nReadIndex = 0;
			this.dataCollection = new List<object>(capacity);
		}

		public void Add(T aData)
		{
			if (this.nWriteIndex >= this.dataCollection.get_Capacity())
			{
				this.nWriteIndex = 0;
			}
			if (this.dataCollection.get_Count() < this.dataCollection.get_Capacity())
			{
				this.dataCollection.Add(aData);
			}
			else
			{
				this.dataCollection.set_Item(this.nWriteIndex, aData);
			}
			this.nWriteIndex++;
		}
	}
}
