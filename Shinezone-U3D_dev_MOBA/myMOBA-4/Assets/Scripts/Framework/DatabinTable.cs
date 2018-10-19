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
			DebugHelper.Assert(InArrayRef.Length == this.mapItems.Count, "Failed Databin CopyTo,size miss.");
			int num = 0;
			Dictionary<long, object>.Enumerator enumerator = this.mapItems.GetEnumerator();
			while (enumerator.MoveNext())
			{
				T[] arg_4F_0 = InArrayRef;
				int expr_37 = num++;
				KeyValuePair<long, object> current = enumerator.Current;
				arg_4F_0[expr_37] = (T)((object)current.Value);
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
				this.mapItems[key] = data;
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
					KeyValuePair<long, object> current = enumerator.Current;
					if (InFunc((T)((object)current.Value)))
					{
						KeyValuePair<long, object> current2 = enumerator.Current;
						return (T)((object)current2.Value);
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
					KeyValuePair<long, object> current = enumerator.Current;
					InVisitor((T)((object)current.Value));
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
						KeyValuePair<long, object> current = enumerator.Current;
						return (T)((object)current.Value);
					}
					num++;
				}
			}
			return default(T);
		}

		public T GetAnyData()
		{
			base.Reload();
			if (base.isLoaded && this.mapItems.Count > 0)
			{
				Dictionary<long, object>.Enumerator enumerator = this.mapItems.GetEnumerator();
				enumerator.MoveNext();
				KeyValuePair<long, object> current = enumerator.Current;
				return (T)((object)current.Value);
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
					KeyValuePair<long, object> current = enumerator.Current;
					if (!dataList.Contains(current.Key))
					{
						List<long> arg_53_0 = list;
						KeyValuePair<long, object> current2 = enumerator.Current;
						arg_53_0.Add(current2.Key);
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					this.mapItems.Remove(list[i]);
				}
				this.bSimple = true;
			}
		}
	}
}
