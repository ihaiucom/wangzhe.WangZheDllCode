using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.tencent.pandora
{
	public class EventTriggerListener : EventTrigger
	{
		public delegate void VoidDelegate(GameObject go, Vector2 pressPos);

		public delegate void VoidDelegateDrag(GameObject go);

		public EventTriggerListener.VoidDelegate onClick;

		public EventTriggerListener.VoidDelegate onDown;

		public EventTriggerListener.VoidDelegate onUp;

		public EventTriggerListener.VoidDelegateDrag onEndDrag;

		public static EventTriggerListener Get(GameObject go)
		{
			EventTriggerListener eventTriggerListener = go.GetComponent<EventTriggerListener>();
			if (eventTriggerListener == null)
			{
				eventTriggerListener = go.AddComponent<EventTriggerListener>();
			}
			return eventTriggerListener;
		}

		public static EventTriggerListener Get(Transform transform)
		{
			EventTriggerListener eventTriggerListener = transform.GetComponent<EventTriggerListener>();
			if (eventTriggerListener == null)
			{
				eventTriggerListener = transform.gameObject.AddComponent<EventTriggerListener>();
			}
			return eventTriggerListener;
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			if (this.onClick != null)
			{
				this.onClick(base.gameObject, eventData.pressPosition);
			}
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (this.onDown != null)
			{
				this.onDown(base.gameObject, eventData.pressPosition);
			}
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			if (this.onUp != null)
			{
				this.onUp(base.gameObject, eventData.pressPosition);
			}
		}

		public override void OnEndDrag(PointerEventData eventData)
		{
			Logger.DEBUG("end of a drag");
			if (this.onEndDrag == null)
			{
				return;
			}
			RectTransform rectTransform = base.gameObject.transform as RectTransform;
			GameObject gameObject = base.gameObject.transform.GetChild(0).gameObject;
			RectTransform rectTransform2 = gameObject.transform as RectTransform;
			if (rectTransform == null || rectTransform2 == null)
			{
				return;
			}
			float height = rectTransform.rect.height;
			float height2 = rectTransform2.rect.height;
			float y = rectTransform2.anchoredPosition.y;
			Logger.DEBUG(string.Format("{0:G},{1:G},{2:G}", height, height2, y));
			float num = (height2 - height <= 0f) ? 0f : (height2 - height);
			if (y - num > 20f)
			{
				Logger.DEBUG("should refresh and do callback");
				if (this.onEndDrag != null)
				{
					this.onEndDrag(base.gameObject);
				}
			}
		}
	}
}
