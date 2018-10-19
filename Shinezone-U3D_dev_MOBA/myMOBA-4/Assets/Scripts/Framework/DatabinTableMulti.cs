using ResData;
using System;
using System.Collections.Generic;
using System.Reflection;
using tsf4g_tdr_csharp;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	public class DatabinTableMulti<T, K> where T : class, tsf4g_csharp_interface, new()
	{
		private const int _headsize = 136;

		private MultiValueHashDictionary<long, object> mapItems = new MultiValueHashDictionary<long, object>();

		private string name;

		private string keyName;

		private bool bLoaded;

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public object[] RawDatas
		{
			get
			{
				this.Reload();
				return this.mapItems.GetAllValueArray();
			}
		}

		public DatabinTableMulti(string name, string key)
		{
			this.bLoaded = false;
			this.name = name;
			this.keyName = key;
			Singleton<ResourceLoader>.GetInstance().LoadDatabin(name, new ResourceLoader.BinLoadCompletedDelegate(this.onRecordLoaded));
		}

		public bool isLoaded()
		{
			return this.bLoaded;
		}

		public void Unload()
		{
			this.mapItems.Clear();
			this.bLoaded = false;
		}

		public void Reload()
		{
			if (this.isLoaded())
			{
				return;
			}
			Singleton<ResourceLoader>.GetInstance().LoadDatabin(this.Name, new ResourceLoader.BinLoadCompletedDelegate(this.onRecordLoaded));
		}

		private void onRecordLoaded(ref byte[] rawData)
		{
			this.LoadTdrBin(rawData);
			this.bLoaded = true;
		}

		private long GetDataKey(T data)
		{
			Type type = data.GetType();
			FieldInfo field = type.GetField(this.keyName);
			DebugHelper.Assert(field != null, "Failed Get Databin key feild {0}, Databin:{1}", new object[]
			{
				this.keyName,
				this.name
			});
			object value = field.GetValue(data);
			try
			{
				return Convert.ToInt64(value);
			}
			catch (Exception ex)
			{
				DebugHelper.Assert(false, "Exception in Databin Get Key, {0}, {1}", new object[]
				{
					value,
					ex.Message
				});
			}
			return 0L;
		}

		public void LoadTdrBin(byte[] rawData)
		{
			if (rawData.Length > 136)
			{
				TdrReadBuf tdrReadBuf = new TdrReadBuf(ref rawData, rawData.Length);
				TResHeadAll tResHeadAll = new TResHeadAll();
				tResHeadAll.load(ref tdrReadBuf);
				int iCount = tResHeadAll.mHead.iCount;
				for (int i = 0; i < iCount; i++)
				{
					T t = Activator.CreateInstance<T>();
					t.load(ref tdrReadBuf, 0u);
					long dataKey = this.GetDataKey(t);
					this.mapItems.Add(dataKey, t);
				}
			}
			else
			{
				Debug.Log("RecordTable<T>.LoadTdrBin:read record error! file length is zero. ");
			}
		}

		private static T GetSingleValue(HashSet<object> data)
		{
			if (data != null)
			{
				HashSet<object>.Enumerator enumerator = data.GetEnumerator();
				if (enumerator.MoveNext())
				{
					return (T)((object)enumerator.Current);
				}
			}
			return (T)((object)null);
		}

		public HashSet<object> GetDataByKey(uint key)
		{
			this.Reload();
			if (this.bLoaded)
			{
				return this.mapItems.GetValues((long)((ulong)key), true);
			}
			return null;
		}

		public HashSet<object> GetDataByKey(int key)
		{
			this.Reload();
			if (this.bLoaded)
			{
				return this.mapItems.GetValues((long)key, true);
			}
			return null;
		}

		public T GetDataByKeySingle(uint key)
		{
			return DatabinTableMulti<T, K>.GetSingleValue(this.GetDataByKey(key));
		}

		public HashSet<object> GetDataByIndex(int id)
		{
			this.Reload();
			if (this.bLoaded)
			{
				DictionaryView<long, HashSet<object>>.Enumerator enumerator = this.mapItems.GetEnumerator();
				int num = 0;
				while (enumerator.MoveNext())
				{
					if (num == id)
					{
						KeyValuePair<long, HashSet<object>> current = enumerator.Current;
						return current.Value;
					}
					num++;
				}
			}
			return null;
		}

		public T GetDataByIdSingle(int id)
		{
			return DatabinTableMulti<T, K>.GetSingleValue(this.GetDataByIndex(id));
		}

		public int Count()
		{
			this.Reload();
			return this.mapItems.Count;
		}
	}
}
