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
			return this.m_uiEventHandlerMap[(int)((UIntPtr)eventID)] != null;
		}

		public void AddUIEventListener(enUIEventID eventID, CUIEventManager.OnUIEventHandler onUIEventHandler)
		{
			if (this.m_uiEventHandlerMap[(int)((UIntPtr)eventID)] == null)
			{
				this.m_uiEventHandlerMap[(int)((UIntPtr)eventID)] = delegate
				{
				};
				CUIEventManager.OnUIEventHandler[] expr_41_cp_0 = this.m_uiEventHandlerMap;
				UIntPtr expr_41_cp_1 = (UIntPtr)eventID;
				expr_41_cp_0[(int)expr_41_cp_1] = (CUIEventManager.OnUIEventHandler)Delegate.Combine(expr_41_cp_0[(int)expr_41_cp_1], onUIEventHandler);
			}
			else
			{
				CUIEventManager.OnUIEventHandler[] expr_61_cp_0 = this.m_uiEventHandlerMap;
				UIntPtr expr_61_cp_1 = (UIntPtr)eventID;
				expr_61_cp_0[(int)expr_61_cp_1] = (CUIEventManager.OnUIEventHandler)Delegate.Remove(expr_61_cp_0[(int)expr_61_cp_1], onUIEventHandler);
				CUIEventManager.OnUIEventHandler[] expr_7C_cp_0 = this.m_uiEventHandlerMap;
				UIntPtr expr_7C_cp_1 = (UIntPtr)eventID;
				expr_7C_cp_0[(int)expr_7C_cp_1] = (CUIEventManager.OnUIEventHandler)Delegate.Combine(expr_7C_cp_0[(int)expr_7C_cp_1], onUIEventHandler);
			}
		}

		public void RemoveUIEventListener(enUIEventID eventID, CUIEventManager.OnUIEventHandler onUIEventHandler)
		{
			if (this.m_uiEventHandlerMap[(int)((UIntPtr)eventID)] != null)
			{
				CUIEventManager.OnUIEventHandler[] expr_1B_cp_0 = this.m_uiEventHandlerMap;
				UIntPtr expr_1B_cp_1 = (UIntPtr)eventID;
				expr_1B_cp_0[(int)expr_1B_cp_1] = (CUIEventManager.OnUIEventHandler)Delegate.Remove(expr_1B_cp_0[(int)expr_1B_cp_1], onUIEventHandler);
			}
		}

		public void DispatchUIEvent(CUIEvent uiEvent)
		{
			uiEvent.m_inUse = true;
			CUIEventManager.OnUIEventHandler onUIEventHandler = this.m_uiEventHandlerMap[(int)((UIntPtr)uiEvent.m_eventID)];
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
			for (int i = 0; i < this.m_uiEvents.Count; i++)
			{
				CUIEvent cUIEvent = (CUIEvent)this.m_uiEvents[i];
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
