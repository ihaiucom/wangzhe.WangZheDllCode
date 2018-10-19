using System;
using System.Collections.Generic;

namespace com.tencent.pandora
{
	internal class EventHandlerContainer : IDisposable
	{
		private Dictionary<Delegate, RegisterEventHandler> dict = new Dictionary<Delegate, RegisterEventHandler>();

		public void Add(Delegate handler, RegisterEventHandler eventInfo)
		{
			this.dict.Add(handler, eventInfo);
		}

		public void Remove(Delegate handler)
		{
			bool flag = this.dict.Remove(handler);
		}

		public void Dispose()
		{
			foreach (KeyValuePair<Delegate, RegisterEventHandler> current in this.dict)
			{
				current.Value.RemovePending(current.Key);
			}
			this.dict.Clear();
		}
	}
}
