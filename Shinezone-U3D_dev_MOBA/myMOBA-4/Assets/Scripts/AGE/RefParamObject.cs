using System;

namespace AGE
{
	public class RefParamObject
	{
		public object value;

		public bool dirty;

		public RefParamObject(object v)
		{
			this.value = v;
			this.dirty = false;
		}
	}
}
