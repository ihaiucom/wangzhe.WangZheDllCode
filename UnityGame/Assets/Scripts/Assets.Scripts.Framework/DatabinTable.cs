using System;
using System.Collections.Generic;

namespace Assets.Scripts.Framework
{
	public class DatabinTable<T, K> : DatabinTableBase
	{
		public DatabinTable(string InName, string InKey) : base(typeof(T))
		{
			this.DataName = InName;
			this.KeyName = InKey;
			this.isDoubleKey = false;
			this.mapItems.Clear();
			this.bLoaded = false;
			Singleton<ResourceLoader>.GetInstance().LoadDatabin(InName, new ResourceLoader.BinLoadCompletedDelegate(base.onRecordLoaded));
		}

		public DatabinTable(string InName, string InKey1, string InKey2) : base(typeof(T))
		{
			this.DataName = InName;
			this.KeyName1 = InKey1;
			this.KeyName2 = InKey2;
			this.isDoubleKey = true;
			this.mapItems.Clear();
			this.bLoaded = false;
			Singleton<ResourceLoader>.GetInstance().LoadDatabin(InName, new ResourceLoader.BinLoadCompletedDelegate(base.onRecordLoaded));
		}

		public void CopyTo(ref T[] InArrayRef)
		{
			base.Reload();
			DebugHelper.Assert(InArrayRef.Length == this.mapItems.get_Count(), "Failed Databin CopyTo,size miss.");
			int num = 0;
			Dictionary<long, object>.Enumerator enumerator = this.mapItems.GetEnumerator();
			while (enumerator.MoveNext())
			{
				T[] array = InArrayRef;
				int num2 = num++;
				KeyValuePair<long, object> current = enumerator.get_Current();
				array[num2] = (T)((object)current.get_Value());
			}
		}

		public T GetDataByKey(uint key)
		{
			return this.GetDataByKey((long)((ulong)key));
		}

		public T GetDataByKey(long key)
		{
			base.Reload();
			T t = (T)((object)base.GetDataByKeyInner(key));
			if (t == null && this.bSimple && key != 0L)
			{
				base.Unload();
				base.Reload();
				this.bSimple = false;
				t = (T)((object)base.GetDataByKeyInner(key));
			}
			return t;
		}

		public void UpdataData(uint key, T data)
		{
			this.UpdateData((long)((ulong)key), data);
		}

		private void UpdateData(long key, T data)
		{
			base.Reload();
			if (this.mapItems.ContainsKey(key))
			{
				this.mapItems.set_Item(key, data);
			}
			else
			{
				this.mapItems.Add(key, data);
			}
		}

		public T FindIf(Func<T, bool> InFunc)
		{
			base.Reload();
			if (base.isLoaded)
			{
				Dictionary<long, object>.Enumerator enumerator = this.mapItems.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<long, object> current = enumerator.get_Current();
					if (InFunc.Invoke((T)((object)current.get_Value())))
					{
						KeyValuePair<long, object> current2 = enumerator.get_Current();
						return (T)((object)current2.get_Value());
					}
				}
			}
			return default(T);
		}

		public void Accept(Action<T> InVisitor)
		{
			base.Reload();
			DebugHelper.Assert(base.isLoaded, "you can't visit databin when it is not loaded.");
			if (base.isLoaded)
			{
				Dictionary<long, object>.Enumerator enumerator = this.mapItems.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<long, object> current = enumerator.get_Current();
					InVisitor.Invoke((T)((object)current.get_Value()));
				}
			}
		}

		public T GetDataByIndex(int id)
		{
			base.Reload();
			if (base.isLoaded)
			{
				Dictionary<long, object>.Enumerator enumerator = this.mapItems.GetEnumerator();
				int num = 0;
				while (enumerator.MoveNext())
				{
					if (num == id)
					{
						KeyValuePair<long, object> current = enumerator.get_Current();
						return (T)((object)current.get_Value());
					}
					num++;
				}
			}
			return default(T);
		}

		public T GetAnyData()
		{
			base.Reload();
			if (base.isLoaded && this.mapItems.get_Count() > 0)
			{
				Dictionary<long, object>.Enumerator enumerator = this.mapItems.GetEnumerator();
				enumerator.MoveNext();
				KeyValuePair<long, object> current = enumerator.get_Current();
				return (T)((object)current.get_Value());
			}
			return default(T);
		}

		public void ReduceDatabin(List<long> dataList)
		{
			base.Reload();
			if (base.isLoaded)
			{
				Dictionary<long, object>.Enumerator enumerator = this.mapItems.GetEnumerator();
				List<long> list = new List<long>();
				while (enumerator.MoveNext())
				{
					KeyValuePair<long, object> current = enumerator.get_Current();
					if (!dataList.Contains(current.get_Key()))
					{
						List<long> list2 = list;
						KeyValuePair<long, object> current2 = enumerator.get_Current();
						list2.Add(current2.get_Key());
					}
				}
				for (int i = 0; i < list.get_Count(); i++)
				{
					this.mapItems.Remove(list.get_Item(i));
				}
				this.bSimple = true;
			}
		}
	}
}
