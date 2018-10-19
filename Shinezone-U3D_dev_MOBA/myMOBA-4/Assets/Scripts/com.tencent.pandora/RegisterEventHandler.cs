using System;
using System.Reflection;

namespace com.tencent.pandora
{
	internal class RegisterEventHandler
	{
		private object target;

		private EventInfo eventInfo;

		private EventHandlerContainer pendingEvents;

		public RegisterEventHandler(EventHandlerContainer pendingEvents, object target, EventInfo eventInfo)
		{
			this.target = target;
			this.eventInfo = eventInfo;
			this.pendingEvents = pendingEvents;
		}

		public Delegate Add(LuaFunction function)
		{
			Delegate @delegate = CodeGeneration.Instance.GetDelegate(this.eventInfo.EventHandlerType, function);
			this.eventInfo.AddEventHandler(this.target, @delegate);
			this.pendingEvents.Add(@delegate, this);
			return @delegate;
		}

		public void Remove(Delegate handlerDelegate)
		{
			this.RemovePending(handlerDelegate);
			this.pendingEvents.Remove(handlerDelegate);
		}

		internal void RemovePending(Delegate handlerDelegate)
		{
			this.eventInfo.RemoveEventHandler(this.target, handlerDelegate);
		}
	}
}
