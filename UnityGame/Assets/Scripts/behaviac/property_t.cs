using System;

namespace behaviac
{
	public struct property_t
	{
		public string name;

		public string value;

		public property_t(string n, string v)
		{
			this.name = n;
			this.value = v;
		}
	}
}
