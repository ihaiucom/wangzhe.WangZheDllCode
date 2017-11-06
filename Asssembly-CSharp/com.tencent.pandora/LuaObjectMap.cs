using System;
using System.Collections.Generic;

namespace com.tencent.pandora
{
	public class LuaObjectMap
	{
		private List<object> list;

		private Queue<int> pool;

		public object this[int i]
		{
			get
			{
				return this.list.get_Item(i);
			}
		}

		public LuaObjectMap()
		{
			this.list = new List<object>(1024);
			this.pool = new Queue<int>(1024);
		}

		public int Add(object obj)
		{
			int num;
			if (this.pool.get_Count() > 0)
			{
				num = this.pool.Dequeue();
				this.list.set_Item(num, obj);
			}
			else
			{
				this.list.Add(obj);
				num = this.list.get_Count() - 1;
			}
			return num;
		}

		public bool TryGetValue(int index, out object obj)
		{
			if (index >= 0 && index < this.list.get_Count())
			{
				obj = this.list.get_Item(index);
				return obj != null;
			}
			obj = null;
			return false;
		}

		public object Remove(int index)
		{
			if (index >= 0 && index < this.list.get_Count())
			{
				object obj = this.list.get_Item(index);
				if (obj != null)
				{
					this.pool.Enqueue(index);
				}
				this.list.set_Item(index, null);
				return obj;
			}
			return null;
		}
	}
}
