using System;

namespace com.tencent.pandora
{
	public class DelegateType
	{
		public string name;

		public Type type;

		public string strType = string.Empty;

		public DelegateType(Type t)
		{
			this.type = t;
			this.strType = ToLuaExport.GetTypeStr(t);
			if (t.IsGenericType)
			{
				this.name = ToLuaExport.GetGenericLibName(t);
			}
			else
			{
				this.name = ToLuaExport.GetTypeStr(t);
				this.name = this.name.Replace(".", "_");
			}
		}

		public DelegateType SetName(string str)
		{
			this.name = str;
			return this;
		}
	}
}
