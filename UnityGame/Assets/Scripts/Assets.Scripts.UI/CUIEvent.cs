using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
	public class CUIEvent
	{
		public CUIFormScript m_srcFormScript;

		public GameObject m_srcWidget;

		public CUIComponent m_srcWidgetScript;

		public CUIListScript m_srcWidgetBelongedListScript;

		public PointerEventData m_pointerEventData;

		public stUIEventParams m_eventParams;

		public enUIEventID m_eventID;

		public bool m_inUse;

		public int m_srcWidgetIndexInBelongedList
		{
			get;
			set;
		}

		public void Clear()
		{
			this.m_srcFormScript = null;
			this.m_srcWidget = null;
			this.m_srcWidgetScript = null;
			this.m_srcWidgetBelongedListScript = null;
			this.m_srcWidgetIndexInBelongedList = -1;
			this.m_pointerEventData = null;
			this.m_eventID = enUIEventID.None;
			this.m_inUse = false;
		}
	}
}
