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
			using (Dictionary<Delegate, RegisterEventHandler>.Enumerator enumerator = this.dict.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<Delegate, RegisterEventHandler> current = enumerator.get_Current();
					current.get_Value().RemovePending(current.get_Key());
				}
			}
			this.dict.Clear();
		}
	}
}
