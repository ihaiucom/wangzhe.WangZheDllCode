using System;

namespace com.tencent.gsdk
{
	public class StartSpeedRet
	{
		public int type;

		public int flag;

		public string desc;

		public StartSpeedRet(int type, int flag, string desc)
		{
			this.type = type;
			this.flag = flag;
			this.desc = desc;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"type: ",
				this.type,
				"; flag: ",
				this.flag,
				"; desc: ",
				this.desc,
				"\n"
			});
		}
	}
}
