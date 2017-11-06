using System;

namespace com.tencent.pandora
{
	public class LuaEventHandler
	{
		public LuaFunction handler;

		public void handleEvent(object[] args)
		{
			this.handler.Call(args);
		}
	}
}
