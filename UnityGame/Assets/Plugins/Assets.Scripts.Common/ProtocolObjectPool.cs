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
				return ProtocolObjectPool.poolList.get_Count();
			}
		}

		static ProtocolObjectPool()
		{
			ProtocolObjectPool.poolList = new List<ProtocolObjectPool>(1024);
			ProtocolObjectPool.Init();
		}

		public static ProtocolObject Get(int ClassID)
		{
			ProtocolObjectPool protocolObjectPool = ProtocolObjectPool.poolList.get_Item(ClassID);
			if (protocolObjectPool.unusedObjs.get_Count() > 0)
			{
				int num = protocolObjectPool.unusedObjs.get_Count() - 1;
				ProtocolObject protocolObject = protocolObjectPool.unusedObjs.get_Item(num);
				protocolObjectPool.unusedObjs.RemoveAt(num);
				protocolObject.OnUse();
				return protocolObject;
			}
			return (ProtocolObject)Activator.CreateInstance(protocolObjectPool.ClassType);
		}

		public static void Release(ProtocolObject obj)
		{
			int classID = obj.GetClassID();
			ProtocolObjectPool.poolList.get_Item(classID).unusedObjs.Add(obj);
		}

		public static ProtocolObjectPool GetPool(int index)
		{
			return ProtocolObjectPool.poolList.get_Item(index);
		}

		public static void Init()
		{
			if (ProtocolObjectPool.poolList.get_Count() > 0)
			{
				return;
			}
			Type typeFromHandle = typeof(ProtocolObject);
			Type[] types = typeFromHandle.get_Assembly().GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (!type.get_IsAbstract() && type.IsSubclassOf(typeFromHandle))
				{
					FieldInfo field = type.GetField("CLASS_ID", 24);
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
			for (int i = 0; i < ProtocolObjectPool.poolList.get_Count(); i++)
			{
				ProtocolObjectPool protocolObjectPool = ProtocolObjectPool.poolList.get_Item(i);
				if (nReserve == 0)
				{
					protocolObjectPool.unusedObjs.Clear();
				}
				else
				{
					int count = protocolObjectPool.unusedObjs.get_Count();
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
			if (ProtocolObjectPool.poolList.get_Count() <= pool.ClassID)
			{
				for (int i = ProtocolObjectPool.poolList.get_Count(); i <= pool.ClassID; i++)
				{
					ProtocolObjectPool.poolList.Add(null);
				}
			}
			ProtocolObjectPool.poolList.set_Item(pool.ClassID, pool);
		}
	}
}
