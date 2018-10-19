using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class CUIJoystickScript : CUIComponent, IPointerDownHandler, IEventSystemHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
	{
		public bool m_isAxisMoveable = true;

		public Vector2 m_axisScreenPositionOffsetMin;

		public Vector2 m_axisScreenPositionOffsetMax;

		public float m_cursorDisplayMaxRadius = 128f;

		public float m_cursorRespondMinRadius = 15f;

		[HideInInspector]
		public enUIEventID m_onAxisChangedEventID;

		public stUIEventParams m_onAxisChangedEventParams;

		[HideInInspector]
		public enUIEventID m_onAxisDownEventID;

		public stUIEventParams m_onAxisDownEventParams;

		[HideInInspector]
		public enUIEventID m_onAxisReleasedEventID;

		public stUIEventParams m_onAxisReleasedEventParams;

		private RectTransform m_axisRectTransform;

		private RectTransform m_cursorRectTransform;

		private Image m_axisImage;

		private Image m_cursorImage;

		public float m_axisFadeoutAlpha = 0.6f;

		private Vector2 m_axisOriginalScreenPosition;

		private Vector2 m_axisTargetScreenPosition;

		private Vector2 m_axisCurrentScreenPosition;

		private Vector2 m_axis;

		private RectTransform m_borderRectTransform;

		private CanvasGroup m_borderCanvasGroup;

		private Vector2 m_vecJoystickSize;

		private Vector2 m_vecAxisAnchorMin;

		private Vector2 m_vecAxisAnchorMax;

		private RectTransform m_joystickRectTransform;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			base.Initialize(formScript);
			this.m_axisRectTransform = (base.gameObject.transform.FindChild("Axis") as RectTransform);
			if (this.m_axisRectTransform != null)
			{
				this.m_axisRectTransform.anchoredPosition = Vector2.zero;
				this.m_axisOriginalScreenPosition = CUIUtility.WorldToScreenPoint(this.m_belongedFormScript.GetCamera(), this.m_axisRectTransform.position);
				this.m_axisImage = this.m_axisRectTransform.gameObject.GetComponent<Image>();
				this.m_cursorRectTransform = (this.m_axisRectTransform.FindChild("Cursor") as RectTransform);
				if (this.m_cursorRectTransform != null)
				{
					this.m_cursorRectTransform.anchoredPosition = Vector2.zero;
					this.m_cursorImage = this.m_cursorRectTransform.gameObject.GetComponent<Image>();
				}
				this.AxisFadeout();
				this.m_vecAxisAnchorMin = this.m_axisRectTransform.anchorMin;
				this.m_vecAxisAnchorMax = this.m_axisRectTransform.anchorMax;
			}
			this.m_joystickRectTransform = (base.gameObject.transform as RectTransform);
			if (this.m_joystickRectTransform != null)
			{
				this.m_vecJoystickSize = this.m_joystickRectTransform.sizeDelta;
			}
			this.m_borderRectTransform = (base.gameObject.transform.FindChild("Axis/Border") as RectTransform);
			if (this.m_borderRectTransform != null)
			{
				this.m_borderCanvasGroup = this.m_borderRectTransform.gameObject.GetComponent<CanvasGroup>();
				if (this.m_borderCanvasGroup == null)
				{
					this.m_borderCanvasGroup = this.m_borderRectTransform.gameObject.AddComponent<CanvasGroup>();
				}
				this.HideBorder();
			}
		}

		protected override void OnDestroy()
		{
			this.m_axisRectTransform = null;
			this.m_cursorRectTransform = null;
			this.m_axisImage = null;
			this.m_cursorImage = null;
			this.m_borderRectTransform = null;
			this.m_borderCanvasGroup = null;
			base.OnDestroy();
		}

		private void Update()
		{
			if (this.m_belongedFormScript != null && this.m_belongedFormScript.IsClosed())
			{
				return;
			}
			if (this.m_isAxisMoveable)
			{
				this.UpdateAxisPosition();
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			this.DispatchOnAxisDownEvent();
			this.MoveAxis(eventData.position, true);
			this.AxisFadeIn();
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			this.MoveAxis(eventData.position, false);
		}

		public void OnDrag(PointerEventData eventData)
		{
			this.MoveAxis(eventData.position, false);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			this.ResetAxis();
			this.DispatchOnAxisReleasedEvent();
		}

		public Vector2 GetAxis()
		{
			return this.m_axis;
		}

		public void ResetAxis()
		{
			this.m_axisRectTransform.anchoredPosition = Vector2.zero;
			this.m_cursorRectTransform.anchoredPosition = Vector2.zero;
			this.m_axisOriginalScreenPosition = CUIUtility.WorldToScreenPoint(this.m_belongedFormScript.GetCamera(), this.m_axisRectTransform.position);
			this.m_axisCurrentScreenPosition = Vector2.zero;
			this.m_axisTargetScreenPosition = Vector2.zero;
			this.UpdateAxis(Vector2.zero);
			this.AxisFadeout();
		}

		private void UpdateAxis(Vector2 axis)
		{
			if (this.m_axis != axis)
			{
				this.m_axis = axis;
				this.DispatchOnAxisChangedEvent();
			}
			if (this.m_axis == Vector2.zero)
			{
				this.HideBorder();
			}
			else
			{
				this.ShowBorder(this.m_axis);
			}
		}

		private void DispatchOnAxisChangedEvent()
		{
			if (this.m_onAxisChangedEventID != enUIEventID.None)
			{
				CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
				uIEvent.m_eventID = this.m_onAxisChangedEventID;
				uIEvent.m_eventParams = this.m_onAxisChangedEventParams;
				uIEvent.m_srcFormScript = this.m_belongedFormScript;
				uIEvent.m_srcWidgetBelongedListScript = this.m_belongedListScript;
				uIEvent.m_srcWidgetIndexInBelongedList = this.m_indexInlist;
				uIEvent.m_srcWidget = base.gameObject;
				uIEvent.m_srcWidgetScript = this;
				uIEvent.m_pointerEventData = null;
				base.DispatchUIEvent(uIEvent);
			}
		}

		private void DispatchOnAxisDownEvent()
		{
			if (this.m_onAxisDownEventID != enUIEventID.None)
			{
				CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
				uIEvent.m_eventID = this.m_onAxisDownEventID;
				uIEvent.m_eventParams = this.m_onAxisDownEventParams;
				uIEvent.m_srcFormScript = this.m_belongedFormScript;
				uIEvent.m_srcWidgetBelongedListScript = this.m_belongedListScript;
				uIEvent.m_srcWidgetIndexInBelongedList = this.m_indexInlist;
				uIEvent.m_srcWidget = base.gameObject;
				uIEvent.m_srcWidgetScript = this;
				uIEvent.m_pointerEventData = null;
				base.DispatchUIEvent(uIEvent);
			}
		}

		private void DispatchOnAxisReleasedEvent()
		{
			if (this.m_onAxisReleasedEventID != enUIEventID.None)
			{
				CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
				uIEvent.m_eventID = this.m_onAxisReleasedEventID;
				uIEvent.m_eventParams = this.m_onAxisReleasedEventParams;
				uIEvent.m_srcFormScript = this.m_belongedFormScript;
				uIEvent.m_srcWidgetBelongedListScript = this.m_belongedListScript;
				uIEvent.m_srcWidgetIndexInBelongedList = this.m_indexInlist;
				uIEvent.m_srcWidget = base.gameObject;
				uIEvent.m_srcWidgetScript = this;
				uIEvent.m_pointerEventData = null;
				base.DispatchUIEvent(uIEvent);
			}
		}

		private void MoveAxis(Vector2 position, bool isDown)
		{
			if (isDown || (this.m_axisCurrentScreenPosition == Vector2.zero && this.m_axisTargetScreenPosition == Vector2.zero))
			{
				this.m_axisCurrentScreenPosition = this.GetFixAixsScreenPosition(position);
				this.m_axisTargetScreenPosition = this.m_axisCurrentScreenPosition;
				DebugHelper.Assert(this.m_belongedFormScript != null);
				this.m_axisRectTransform.position = CUIUtility.ScreenToWorldPoint((!(this.m_belongedFormScript != null)) ? null : this.m_belongedFormScript.GetCamera(), this.m_axisCurrentScreenPosition, this.m_axisRectTransform.position.z);
			}
			Vector2 vector = position - this.m_axisCurrentScreenPosition;
			Vector2 vector2 = vector;
			float magnitude = vector.magnitude;
			float num = magnitude;
			if (this.m_belongedFormScript != null)
			{
				num = this.m_belongedFormScript.ChangeScreenValueToForm(magnitude);
				vector2.x = this.m_belongedFormScript.ChangeScreenValueToForm(vector.x);
				vector2.y = this.m_belongedFormScript.ChangeScreenValueToForm(vector.y);
			}
			DebugHelper.Assert(this.m_cursorRectTransform != null);
			this.m_cursorRectTransform.anchoredPosition = ((num <= this.m_cursorDisplayMaxRadius) ? vector2 : (vector2.normalized * this.m_cursorDisplayMaxRadius));
			if (this.m_isAxisMoveable && num > this.m_cursorDisplayMaxRadius)
			{
				DebugHelper.Assert(this.m_belongedFormScript != null);
				this.m_axisTargetScreenPosition = this.m_axisCurrentScreenPosition + (position - CUIUtility.WorldToScreenPoint((!(this.m_belongedFormScript != null)) ? null : this.m_belongedFormScript.GetCamera(), this.m_cursorRectTransform.position));
				this.m_axisTargetScreenPosition = this.GetFixAixsScreenPosition(this.m_axisTargetScreenPosition);
			}
			if (num < this.m_cursorRespondMinRadius)
			{
				this.UpdateAxis(Vector2.zero);
			}
			else
			{
				this.UpdateAxis(vector);
			}
		}

		private void UpdateAxisPosition()
		{
			if (this.m_axisCurrentScreenPosition != this.m_axisTargetScreenPosition)
			{
				Vector2 vector = this.m_axisTargetScreenPosition - this.m_axisCurrentScreenPosition;
				Vector2 b = (this.m_axisTargetScreenPosition - this.m_axisCurrentScreenPosition) / 3f;
				if (vector.sqrMagnitude <= 1f)
				{
					this.m_axisCurrentScreenPosition = this.m_axisTargetScreenPosition;
				}
				else
				{
					this.m_axisCurrentScreenPosition += b;
				}
				this.m_axisRectTransform.position = CUIUtility.ScreenToWorldPoint(this.m_belongedFormScript.GetCamera(), this.m_axisCurrentScreenPosition, this.m_axisRectTransform.position.z);
			}
		}

		private Vector2 GetFixAixsScreenPosition(Vector2 axisScreenPosition)
		{
			axisScreenPosition.x = CUIUtility.ValueInRange(axisScreenPosition.x, this.m_axisOriginalScreenPosition.x + this.m_belongedFormScript.ChangeFormValueToScreen(this.m_axisScreenPositionOffsetMin.x), this.m_axisOriginalScreenPosition.x + this.m_belongedFormScript.ChangeFormValueToScreen(this.m_axisScreenPositionOffsetMax.x));
			axisScreenPosition.y = CUIUtility.ValueInRange(axisScreenPosition.y, this.m_axisOriginalScreenPosition.y + this.m_belongedFormScript.ChangeFormValueToScreen(this.m_axisScreenPositionOffsetMin.y), this.m_axisOriginalScreenPosition.y + this.m_belongedFormScript.ChangeFormValueToScreen(this.m_axisScreenPositionOffsetMax.y));
			return axisScreenPosition;
		}

		private void HideBorder()
		{
			if (this.m_borderRectTransform == null || this.m_borderCanvasGroup == null)
			{
				return;
			}
			if (this.m_borderCanvasGroup.alpha != 0f)
			{
				this.m_borderCanvasGroup.alpha = 0f;
			}
			if (this.m_borderCanvasGroup.blocksRaycasts)
			{
				this.m_borderCanvasGroup.blocksRaycasts = false;
			}
		}

		private void ShowBorder(Vector2 axis)
		{
			if (this.m_borderRectTransform == null || this.m_borderCanvasGroup == null)
			{
				return;
			}
			if (this.m_borderCanvasGroup.alpha != 1f)
			{
				this.m_borderCanvasGroup.alpha = 1f;
			}
			if (!this.m_borderCanvasGroup.blocksRaycasts)
			{
				this.m_borderCanvasGroup.blocksRaycasts = true;
			}
			this.m_borderRectTransform.right = axis;
		}

		private void AxisFadeIn()
		{
			if (this.m_axisImage != null)
			{
				this.m_axisImage.color = new Color(1f, 1f, 1f, 1f);
			}
			if (this.m_cursorImage != null)
			{
				this.m_cursorImage.color = new Color(1f, 1f, 1f, 1f);
			}
		}

		private void AxisFadeout()
		{
			if (this.m_axisImage != null)
			{
				this.m_axisImage.color = new Color(1f, 1f, 1f, this.m_axisFadeoutAlpha);
			}
			if (this.m_cursorImage != null)
			{
				this.m_cursorImage.color = new Color(1f, 1f, 1f, this.m_axisFadeoutAlpha);
			}
		}

		public void ChangeJoystickResponseArea(bool bIsChangeToSmall)
		{
			if (bIsChangeToSmall)
			{
				if (this.m_axisRectTransform != null && this.m_joystickRectTransform != null)
				{
					this.m_joystickRectTransform.sizeDelta = this.m_axisRectTransform.sizeDelta + new Vector2(20f, 20f);
					this.m_axisRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
					this.m_axisRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
					this.m_axisRectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
				}
			}
			else if (this.m_axisRectTransform != null && this.m_joystickRectTransform != null)
			{
				this.m_joystickRectTransform.sizeDelta = this.m_vecJoystickSize;
				this.m_axisRectTransform.anchorMin = this.m_vecAxisAnchorMin;
				this.m_axisRectTransform.anchorMax = this.m_vecAxisAnchorMax;
				this.m_axisRectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
			}
		}
	}
}
