using System;

namespace Assets.Scripts.Common
{
	public class ClassObjPool<T> : ClassObjPoolBase where T : PooledClassObject, new()
	{
		private static ClassObjPool<T> instance;

		public static uint NewSeq()
		{
			if (ClassObjPool<T>.instance == null)
			{
				ClassObjPool<T>.instance = new ClassObjPool<T>();
			}
			ClassObjPool<T>.instance.reqSeq += 1u;
			return ClassObjPool<T>.instance.reqSeq;
		}

		public static T Get()
		{
			if (ClassObjPool<T>.instance == null)
			{
				ClassObjPool<T>.instance = new ClassObjPool<T>();
			}
			if (ClassObjPool<T>.instance.pool.Count > 0)
			{
				T t = (T)((object)ClassObjPool<T>.instance.pool[ClassObjPool<T>.instance.pool.Count - 1]);
				ClassObjPool<T>.instance.pool.RemoveAt(ClassObjPool<T>.instance.pool.Count - 1);
				ClassObjPool<T>.instance.reqSeq += 1u;
				t.usingSeq = ClassObjPool<T>.instance.reqSeq;
				t.holder = ClassObjPool<T>.instance;
				t.OnUse();
				return t;
			}
			T t2 = Activator.CreateInstance<T>();
			ClassObjPool<T>.instance.reqSeq += 1u;
			t2.usingSeq = ClassObjPool<T>.instance.reqSeq;
			t2.holder = ClassObjPool<T>.instance;
			t2.OnUse();
			return t2;
		}

		public override void Release(PooledClassObject obj)
		{
			T t = obj as T;
			obj.usingSeq = 0u;
			obj.holder = null;
			this.pool.Add(t);
		}
	}
}
