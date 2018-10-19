using System;
using System.Collections.Generic;
using System.Reflection;

namespace Assets.Scripts.Common
{
	public class ProtocolObjectPool
	{
		public int ClassID;

		public Type ClassType;

		public List<ProtocolObject> unusedObjs = new List<ProtocolObject>(128);

		private static List<ProtocolObjectPool> poolList;

		public static int PoolCount
		{
			get
			{
				return ProtocolObjectPool.poolList.Count;
			}
		}

		static ProtocolObjectPool()
		{
			ProtocolObjectPool.poolList = new List<ProtocolObjectPool>(1024);
			ProtocolObjectPool.Init();
		}

		public static ProtocolObject Get(int ClassID)
		{
			ProtocolObjectPool protocolObjectPool = ProtocolObjectPool.poolList[ClassID];
			if (protocolObjectPool.unusedObjs.Count > 0)
			{
				int index = protocolObjectPool.unusedObjs.Count - 1;
				ProtocolObject protocolObject = protocolObjectPool.unusedObjs[index];
				protocolObjectPool.unusedObjs.RemoveAt(index);
				protocolObject.OnUse();
				return protocolObject;
			}
			return (ProtocolObject)Activator.CreateInstance(protocolObjectPool.ClassType);
		}

		public static void Release(ProtocolObject obj)
		{
			int classID = obj.GetClassID();
			ProtocolObjectPool.poolList[classID].unusedObjs.Add(obj);
		}

		public static ProtocolObjectPool GetPool(int index)
		{
			return ProtocolObjectPool.poolList[index];
		}

		public static void Init()
		{
			if (ProtocolObjectPool.poolList.Count > 0)
			{
				return;
			}
			Type typeFromHandle = typeof(ProtocolObject);
			Type[] types = typeFromHandle.Assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (!type.IsAbstract && type.IsSubclassOf(typeFromHandle))
				{
					FieldInfo field = type.GetField("CLASS_ID", BindingFlags.Static | BindingFlags.Public);
					int classID = (int)field.GetValue(null);
					ProtocolObjectPool.AddPool(new ProtocolObjectPool
					{
						ClassType = type,
						ClassID = classID
					});
				}
			}
		}

		public static void Clear(int nReserve = 0)
		{
			for (int i = 0; i < ProtocolObjectPool.poolList.Count; i++)
			{
				ProtocolObjectPool protocolObjectPool = ProtocolObjectPool.poolList[i];
				if (nReserve == 0)
				{
					protocolObjectPool.unusedObjs.Clear();
				}
				else
				{
					int count = protocolObjectPool.unusedObjs.Count;
					int num = count - nReserve;
					if (num > 0)
					{
						protocolObjectPool.unusedObjs.RemoveRange(count - num, num);
					}
				}
			}
		}

		private static void AddPool(ProtocolObjectPool pool)
		{
			if (ProtocolObjectPool.poolList.Count <= pool.ClassID)
			{
				for (int i = ProtocolObjectPool.poolList.Count; i <= pool.ClassID; i++)
				{
					ProtocolObjectPool.poolList.Add(null);
				}
			}
			ProtocolObjectPool.poolList[pool.ClassID] = pool;
		}
	}
}
