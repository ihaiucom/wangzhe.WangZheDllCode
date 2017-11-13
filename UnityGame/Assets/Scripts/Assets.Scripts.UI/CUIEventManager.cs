using System;
using System.Collections.Generic;

namespace Assets.Scripts.UI
{
	public class CUIEventManager : Singleton<CUIEventManager>
	{
		public delegate void OnUIEventHandler(CUIEvent uiEvent);

		private CUIEventManager.OnUIEventHandler[] m_uiEventHandlerMap = new CUIEventManager.OnUIEventHandler[15810];

		private List<object> m_uiEvents = new List<object>();

		public bool HasUIEventListener(enUIEventID eventID)
		{
			return this.m_uiEventHandlerMap[(int)((uint)((UIntPtr)((ulong)((long)eventID))))] != null;
		}

		public void AddUIEventListener(enUIEventID eventID, CUIEventManager.OnUIEventHandler onUIEventHandler)
		{
			if (this.m_uiEventHandlerMap[(int)((uint)((UIntPtr)((ulong)((long)eventID))))] == null)
			{
				this.m_uiEventHandlerMap[(int)((uint)((UIntPtr)((ulong)((long)eventID))))] = delegate
				{
				};
				CUIEventManager.OnUIEventHandler[] uiEventHandlerMap = this.m_uiEventHandlerMap;
				UIntPtr uIntPtr = (UIntPtr)((ulong)((long)eventID));
				uiEventHandlerMap[(int)((uint)uIntPtr)] = (CUIEventManager.OnUIEventHandler)Delegate.Combine(uiEventHandlerMap[(int)((uint)uIntPtr)], onUIEventHandler);
			}
			else
			{
				CUIEventManager.OnUIEventHandler[] uiEventHandlerMap2 = this.m_uiEventHandlerMap;
				UIntPtr uIntPtr2 = (UIntPtr)((ulong)((long)eventID));
				uiEventHandlerMap2[(int)((uint)uIntPtr2)] = (CUIEventManager.OnUIEventHandler)Delegate.Remove(uiEventHandlerMap2[(int)((uint)uIntPtr2)], onUIEventHandler);
				CUIEventManager.OnUIEventHandler[] uiEventHandlerMap3 = this.m_uiEventHandlerMap;
				UIntPtr uIntPtr3 = (UIntPtr)((ulong)((long)eventID));
				uiEventHandlerMap3[(int)((uint)uIntPtr3)] = (CUIEventManager.OnUIEventHandler)Delegate.Combine(uiEventHandlerMap3[(int)((uint)uIntPtr3)], onUIEventHandler);
			}
		}

		public void RemoveUIEventListener(enUIEventID eventID, CUIEventManager.OnUIEventHandler onUIEventHandler)
		{
			if (this.m_uiEventHandlerMap[(int)((uint)((UIntPtr)((ulong)((long)eventID))))] != null)
			{
				CUIEventManager.OnUIEventHandler[] uiEventHandlerMap = this.m_uiEventHandlerMap;
				UIntPtr uIntPtr = (UIntPtr)((ulong)((long)eventID));
				uiEventHandlerMap[(int)((uint)uIntPtr)] = (CUIEventManager.OnUIEventHandler)Delegate.Remove(uiEventHandlerMap[(int)((uint)uIntPtr)], onUIEventHandler);
			}
		}

		public void DispatchUIEvent(CUIEvent uiEvent)
		{
			uiEvent.m_inUse = true;
			CUIEventManager.OnUIEventHandler onUIEventHandler = this.m_uiEventHandlerMap[(int)((uint)((UIntPtr)((ulong)((long)uiEvent.m_eventID))))];
			if (onUIEventHandler != null)
			{
				onUIEventHandler(uiEvent);
			}
			uiEvent.Clear();
		}

		public void DispatchUIEvent(enUIEventID eventID)
		{
			CUIEvent uIEvent = this.GetUIEvent();
			uIEvent.m_eventID = eventID;
			this.DispatchUIEvent(uIEvent);
		}

		public void DispatchUIEvent(enUIEventID eventID, stUIEventParams par)
		{
			CUIEvent uIEvent = this.GetUIEvent();
			uIEvent.m_eventID = eventID;
			uIEvent.m_eventParams = par;
			this.DispatchUIEvent(uIEvent);
		}

		public CUIEvent GetUIEvent()
		{
			for (int i = 0; i < this.m_uiEvents.get_Count(); i++)
			{
				CUIEvent cUIEvent = (CUIEvent)this.m_uiEvents.get_Item(i);
				if (!cUIEvent.m_inUse)
				{
					return cUIEvent;
				}
			}
			CUIEvent cUIEvent2 = new CUIEvent();
			this.m_uiEvents.Add(cUIEvent2);
			return cUIEvent2;
		}
	}
}
