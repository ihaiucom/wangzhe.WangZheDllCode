using Assets.Scripts.Sound;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class CUIMiniEventWithDragScript : CUIComponent, IPointerClickHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
	{
		public delegate void OnUIEventHandler(CUIEvent uiEvent);

		private const float c_clickAreaValue = 40f;

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

		private bool m_isDown;

		private bool m_isHold;

		private bool m_canClick;

		public float c_holdTimeValue = 1f;

		private float m_downTimestamp;

		private Vector2 m_downPosition;

		private PointerEventData m_holdPointerEventData;

		private bool m_needClearInputStatus;

		public ScrollRect m_faterScrollRect;

		public CUIMiniEventWithDragScript.OnUIEventHandler onClick;

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
			this.m_holdPointerEventData = null;
			this.onClick = null;
			base.OnDestroy();
		}

		public override void Close()
		{
			this.ExecuteClearInputStatus();
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

		public void OnPointerDown(PointerEventData eventData)
		{
			this.m_isDown = true;
			this.m_isHold = false;
			this.m_canClick = true;
			this.m_downTimestamp = Time.realtimeSinceStartup;
			this.m_downPosition = eventData.get_position();
			this.m_holdPointerEventData = eventData;
			this.m_needClearInputStatus = false;
			this.DispatchUIEvent(enUIEventType.Down, eventData);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (this.m_isHold && this.m_holdPointerEventData != null)
			{
				this.DispatchUIEvent(enUIEventType.HoldEnd, this.m_holdPointerEventData);
			}
			this.DispatchUIEvent(enUIEventType.Up, eventData);
			this.ClearInputStatus();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			bool flag = true;
			if (this.m_belongedFormScript != null && !this.m_belongedFormScript.m_enableMultiClickedEvent)
			{
				flag = (this.m_belongedFormScript.m_clickedEventDispatchedCounter == 0);
				this.m_belongedFormScript.m_clickedEventDispatchedCounter++;
			}
			if (this.m_canClick && flag)
			{
				this.DispatchUIEvent(enUIEventType.Click, eventData);
				if (this.m_closeFormWhenClicked && this.m_belongedFormScript != null)
				{
					this.m_belongedFormScript.Close();
				}
			}
			this.ClearInputStatus();
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (this.m_canClick && this.m_belongedFormScript != null && this.m_belongedFormScript.ChangeScreenValueToForm(Vector2.Distance(eventData.get_position(), this.m_downPosition)) > 40f)
			{
				this.m_canClick = false;
			}
			this.DispatchUIEvent(enUIEventType.DragStart, eventData);
			if (this.m_faterScrollRect != null)
			{
				this.m_faterScrollRect.OnBeginDrag(eventData);
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (this.m_canClick && this.m_belongedFormScript != null && this.m_belongedFormScript.ChangeScreenValueToForm(Vector2.Distance(eventData.get_position(), this.m_downPosition)) > 40f)
			{
				this.m_canClick = false;
			}
			this.DispatchUIEvent(enUIEventType.Drag, eventData);
			if (this.m_faterScrollRect != null)
			{
				this.m_faterScrollRect.OnDrag(eventData);
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (this.m_canClick && this.m_belongedFormScript != null && this.m_belongedFormScript.ChangeScreenValueToForm(Vector2.Distance(eventData.get_position(), this.m_downPosition)) > 40f)
			{
				this.m_canClick = false;
			}
			this.DispatchUIEvent(enUIEventType.DragEnd, eventData);
			if (this.m_faterScrollRect != null)
			{
				this.m_faterScrollRect.OnEndDrag(eventData);
			}
			this.ClearInputStatus();
		}

		public void OnDrop(PointerEventData eventData)
		{
			this.DispatchUIEvent(enUIEventType.Drop, eventData);
		}

		public bool ClearInputStatus()
		{
			this.m_needClearInputStatus = true;
			return this.m_isDown;
		}

		public void ExecuteClearInputStatus()
		{
			this.m_isDown = false;
			this.m_isHold = false;
			this.m_canClick = false;
			this.m_downTimestamp = 0f;
			this.m_downPosition = Vector2.zero;
			this.m_holdPointerEventData = null;
		}

		private void Update()
		{
			if (this.m_needClearInputStatus)
			{
				return;
			}
			if (this.m_isDown)
			{
				if (!this.m_isHold)
				{
					if (Time.realtimeSinceStartup - this.m_downTimestamp >= this.c_holdTimeValue)
					{
						this.m_isHold = true;
						this.m_canClick = false;
						this.DispatchUIEvent(enUIEventType.HoldStart, this.m_holdPointerEventData);
					}
				}
				else
				{
					this.DispatchUIEvent(enUIEventType.Hold, this.m_holdPointerEventData);
				}
			}
		}

		private void LateUpdate()
		{
			if (this.m_needClearInputStatus)
			{
				this.ExecuteClearInputStatus();
				this.m_needClearInputStatus = false;
			}
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
					this.PostWwiseEvent(this.m_onDownWwiseEvents);
					this.PostWwiseEvent(this.m_onClickedWwiseEvents);
					if (this.m_onClickEventID == enUIEventID.None)
					{
						if (this.onClick != null)
						{
							uIEvent.m_eventID = enUIEventID.None;
							uIEvent.m_eventParams = this.m_onClickEventParams;
							uIEvent.m_srcWidget = base.gameObject;
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

		private void PostWwiseEvent(string[] wwiseEvents)
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
