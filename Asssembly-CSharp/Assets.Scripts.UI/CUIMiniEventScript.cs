using Assets.Scripts.Sound;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
	public class CUIMiniEventScript : CUIComponent, IPointerClickHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler
	{
		public delegate void OnUIEventHandler(CUIEvent uiEvent);

		[HideInInspector]
		public enUIEventID m_onDownEventID;

		[NonSerialized]
		public stUIEventParams m_onDownEventParams;

		[HideInInspector]
		public enUIEventID m_onUpEventID;

		[NonSerialized]
		public stUIEventParams m_onUpEventParams;

		[HideInInspector]
		public enUIEventID m_onClickEventID;

		[NonSerialized]
		public stUIEventParams m_onClickEventParams;

		public bool m_closeFormWhenClicked;

		public string[] m_onDownWwiseEvents = new string[0];

		public string[] m_onClickedWwiseEvents = new string[0];

		public CUIMiniEventScript.OnUIEventHandler onClick;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			base.Initialize(formScript);
		}

		protected override void OnDestroy()
		{
			this.onClick = null;
			base.OnDestroy();
		}

		public void SetUIEvent(enUIEventType eventType, enUIEventID eventID)
		{
			if (eventType != enUIEventType.Down)
			{
				if (eventType != enUIEventType.Click)
				{
					if (eventType == enUIEventType.Up)
					{
						this.m_onUpEventID = eventID;
					}
				}
				else
				{
					this.m_onClickEventID = eventID;
				}
			}
			else
			{
				this.m_onDownEventID = eventID;
			}
		}

		public void SetUIEvent(enUIEventType eventType, enUIEventID eventID, stUIEventParams eventParams)
		{
			if (eventType != enUIEventType.Down)
			{
				if (eventType != enUIEventType.Click)
				{
					if (eventType == enUIEventType.Up)
					{
						this.m_onUpEventID = eventID;
						this.m_onUpEventParams = eventParams;
					}
				}
				else
				{
					this.m_onClickEventID = eventID;
					this.m_onClickEventParams = eventParams;
				}
			}
			else
			{
				this.m_onDownEventID = eventID;
				this.m_onDownEventParams = eventParams;
			}
		}

		public virtual void OnPointerDown(PointerEventData eventData)
		{
			this.DispatchUIEvent(enUIEventType.Down, eventData);
		}

		public virtual void OnPointerUp(PointerEventData eventData)
		{
			this.DispatchUIEvent(enUIEventType.Up, eventData);
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			bool flag = true;
			if (this.m_belongedFormScript != null && !this.m_belongedFormScript.m_enableMultiClickedEvent)
			{
				flag = (this.m_belongedFormScript.m_clickedEventDispatchedCounter == 0);
				this.m_belongedFormScript.m_clickedEventDispatchedCounter++;
			}
			if (flag)
			{
				if (this.m_belongedListScript != null && this.m_indexInlist >= 0)
				{
					this.m_belongedListScript.SelectElement(this.m_indexInlist, true);
				}
				this.DispatchUIEvent(enUIEventType.Click, eventData);
				if (this.m_closeFormWhenClicked && this.m_belongedFormScript != null)
				{
					this.m_belongedFormScript.Close();
				}
			}
		}

		private void Update()
		{
		}

		private void DispatchUIEvent(enUIEventType eventType, PointerEventData pointerEventData)
		{
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			if (eventType != enUIEventType.Down)
			{
				if (eventType != enUIEventType.Click)
				{
					if (eventType == enUIEventType.Up)
					{
						if (this.m_onUpEventID == enUIEventID.None)
						{
							return;
						}
						uIEvent.m_eventID = this.m_onUpEventID;
						uIEvent.m_eventParams = this.m_onUpEventParams;
					}
				}
				else
				{
					this.PostWwiseEvent(this.m_onClickedWwiseEvents);
					if (this.m_onClickEventID == enUIEventID.None)
					{
						if (this.onClick != null)
						{
							uIEvent.m_eventID = enUIEventID.None;
							uIEvent.m_eventParams = this.m_onClickEventParams;
							this.onClick(uIEvent);
						}
						return;
					}
					uIEvent.m_eventID = this.m_onClickEventID;
					uIEvent.m_eventParams = this.m_onClickEventParams;
				}
			}
			else
			{
				this.PostWwiseEvent(this.m_onDownWwiseEvents);
				if (this.m_onDownEventID == enUIEventID.None)
				{
					return;
				}
				uIEvent.m_eventID = this.m_onDownEventID;
				uIEvent.m_eventParams = this.m_onDownEventParams;
			}
			uIEvent.m_srcFormScript = this.m_belongedFormScript;
			uIEvent.m_srcWidgetBelongedListScript = this.m_belongedListScript;
			uIEvent.m_srcWidgetIndexInBelongedList = this.m_indexInlist;
			uIEvent.m_srcWidget = base.gameObject;
			uIEvent.m_srcWidgetScript = this;
			uIEvent.m_pointerEventData = pointerEventData;
			if (eventType == enUIEventType.Click && this.onClick != null)
			{
				this.onClick(uIEvent);
			}
			base.DispatchUIEvent(uIEvent);
		}

		protected void PostWwiseEvent(string[] wwiseEvents)
		{
			for (int i = 0; i < wwiseEvents.Length; i++)
			{
				if (!string.IsNullOrEmpty(wwiseEvents[i]))
				{
					Singleton<CSoundManager>.GetInstance().PostEvent(wwiseEvents[i], null);
				}
			}
		}
	}
}
