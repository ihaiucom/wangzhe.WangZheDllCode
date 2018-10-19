using ResData;
using System;
using System.Collections.Generic;
using System.Reflection;
using tsf4g_tdr_csharp;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	public class DatabinTableBase
	{
		protected const int _headsize = 136;

		protected Dictionary<long, object> mapItems = new Dictionary<long, object>();

		protected string DataName;

		protected string KeyName;

		protected bool bLoaded;

		protected bool isDoubleKey;

		protected string KeyName1;

		protected string KeyName2;

		protected bool bAllowUnLoad = true;

		protected Type ValueType;

		protected bool bSimple;

		public string Name
		{
			get
			{
				return this.DataName;
			}
		}

		public bool isLoaded
		{
			get
			{
				return this.bLoaded;
			}
		}

		public bool isAllowUnLoad
		{
			set
			{
				this.bAllowUnLoad = value;
			}
		}

		public int count
		{
			get
			{
				return this.Count();
			}
		}

		public DatabinTableBase(Type InValueType)
		{
			this.ValueType = InValueType;
		}

		public Dictionary<long, object>.Enumerator GetEnumerator()
		{
			return this.mapItems.GetEnumerator();
		}

		public void Unload()
		{
			if (!this.bAllowUnLoad)
			{
				return;
			}
			this.bLoaded = false;
			this.bSimple = false;
			this.mapItems.Clear();
		}

		public void LoadTdrBin(byte[] rawData, Type InValueType)
		{
			if (rawData.Length > 136)
			{
				TdrReadBuf tdrReadBuf = new TdrReadBuf(ref rawData, rawData.Length);
				TResHeadAll tResHeadAll = new TResHeadAll();
				tResHeadAll.load(ref tdrReadBuf);
				int iCount = tResHeadAll.mHead.iCount;
				DebugHelper.Assert(iCount < 100000, "有这么恐怖吗，超过10w条配置数据。。。。");
				for (int i = 0; i < iCount; i++)
				{
					tsf4g_csharp_interface tsf4g_csharp_interface = Activator.CreateInstance(InValueType) as tsf4g_csharp_interface;
					DebugHelper.Assert(tsf4g_csharp_interface != null, "Failed Create Object, Type:{0}", new object[]
					{
						InValueType.Name
					});
					tsf4g_csharp_interface.load(ref tdrReadBuf, 0u);
					long dataKey = this.GetDataKey(tsf4g_csharp_interface, InValueType);
					try
					{
						this.mapItems.Add(dataKey, tsf4g_csharp_interface);
					}
					catch (ArgumentException var_6_A3)
					{
						DebugHelper.Assert(false, "RecordTable<{2}>.LoadTdrBin: Key Repeat: {0}, DataBinName:{1}", new object[]
						{
							dataKey,
							this.Name,
							InValueType.Name
						});
					}
				}
			}
			else
			{
				Debug.Log("RecordTable<T>.LoadTdrBin:read record error! file length is zero. ");
			}
		}

		protected object GetDataByKeyInner(long key)
		{
			object result;
			if (this.bLoaded && this.mapItems.TryGetValue(key, out result))
			{
				return result;
			}
			return null;
		}

		protected long GetDataKey(object data, Type InValueType)
		{
			Type type = data.GetType();
			DebugHelper.Assert(type == InValueType, "Invalid Config for Databin:{0}", new object[]
			{
				this.Name
			});
			long result;
			if (this.isDoubleKey)
			{
				FieldInfo field = type.GetField(this.KeyName1);
				object value = field.GetValue(data);
				DebugHelper.Assert(value != null, "Can't Find Key {0} in DataBin:{1}", new object[]
				{
					this.KeyName1,
					this.Name
				});
				FieldInfo field2 = type.GetField(this.KeyName2);
				object value2 = field2.GetValue(data);
				DebugHelper.Assert(value2 != null, "Can't Find Key {0} in DataBin:{1}", new object[]
				{
					this.KeyName2,
					this.Name
				});
				try
				{
					if (value != null && value2 != null)
					{
						ulong num = Convert.ToUInt64(value);
						num <<= 32;
						int num2 = Convert.ToInt32(value2);
						result = (long)(num + (ulong)((long)num2));
						return result;
					}
					result = 0L;
					return result;
				}
				catch (Exception ex)
				{
					DebugHelper.Assert(false, "Exception in Databin Get Key1, {0}, Key2{1},{2}", new object[]
					{
						value,
						value2,
						ex.Message
					});
					result = 0L;
					return result;
				}
			}
			FieldInfo field3 = type.GetField(this.KeyName);
			object value3 = field3.GetValue(data);
			DebugHelper.Assert(value3 != null, "Can't Find Key {0} in DataBin:{1}", new object[]
			{
				this.KeyName,
				this.Name
			});
			try
			{
				result = ((value3 == null) ? 0L : Convert.ToInt64(value3));
			}
			catch (Exception ex2)
			{
				DebugHelper.Assert(false, "Exception in Databin Get Key, {0}, {1}", new object[]
				{
					value3,
					ex2.Message
				});
				result = 0L;
			}
			return result;
		}

		public void Reload()
		{
			if (this.isLoaded)
			{
				return;
			}
			Singleton<ResourceLoader>.GetInstance().LoadDatabin(this.Name, new ResourceLoader.BinLoadCompletedDelegate(this.onRecordLoaded));
		}

		public int Count()
		{
			this.Reload();
			return this.mapItems.Count;
		}

		protected void onRecordLoaded(ref byte[] rawData)
		{
			this.LoadTdrBin(rawData, this.ValueType);
			this.bLoaded = true;
		}
	}
}
