using System;
using System.Collections.Generic;

namespace com.tencent.pandora
{
	public class Message
	{
		public int status;

		public Dictionary<string, object> content = new Dictionary<string, object>();

		public Action<int, Dictionary<string, object>> action;
	}
}
