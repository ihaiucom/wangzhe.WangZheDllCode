using System;
using System.Collections.Generic;

namespace Assets.Scripts.Common
{
	public abstract class ClassObjPoolBase : IObjPoolCtrl
	{
		protected List<object> pool = new List<object>(128);

		protected uint reqSeq;

		public int capacity
		{
			get
			{
				return this.pool.get_Capacity();
			}
			set
			{
				this.pool.set_Capacity(value);
			}
		}

		public abstract void Release(PooledClassObject obj);
	}
}
