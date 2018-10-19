using System;

namespace com.tencent.pandora
{
	[AttributeUsage(AttributeTargets.Method)]
	public class LuaGlobalAttribute : Attribute
	{
		private string name;

		private string descript;

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public string Description
		{
			get
			{
				return this.descript;
			}
			set
			{
				this.descript = value;
			}
		}
	}
}
