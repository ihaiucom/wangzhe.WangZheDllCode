using Assets.Scripts.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class CUIStepListScript : CUIListScript
	{
		public float m_reductionRate = 0.7f;

		public float m_minSpeedToFixScroll = 50f;

		public float m_fixScrollTime = 0.3f;

		[HideInInspector]
		public enUIEventID m_onStartDraggingEventID;

		public stUIEventParams m_onStartDraggingEventParams;

		private float m_contentExtendSize;

		private float m_stepListCenter;

		private float m_selectAreaMin;

		private float m_selectAreaMax;

		private float m_scrollRectLastScrollSpeed;

		private bool m_scrollRectIsDragged;

		private bool m_fixingScroll;

		private float m_fixScrollSpeed;

		private float m_contentFixScrollTargetPosition;

		private bool m_bDontUpdate;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			this.m_listType = enUIListType.Horizontal;
			this.m_elementSpacing = Vector2.zero;
			this.m_elementLayoutOffset = 0f;
			int elementAmount = this.m_elementAmount;
			this.m_elementAmount = 0;
			base.Initialize(formScript);
			CUIListElementScript component = this.m_elementTemplate.GetComponent<CUIListElementScript>();
			if (component != null)
			{
				component.m_pivotType = enPivotType.Centre;
			}
			this.m_contentExtendSize = (this.m_scrollAreaSize.x - this.m_elementDefaultSize.x) * 0.5f;
			this.m_stepListCenter = this.m_contentExtendSize + this.m_elementDefaultSize.x * 0.5f;
			this.m_selectAreaMin = this.m_contentExtendSize;
			this.m_selectAreaMax = this.m_scrollAreaSize.x - this.m_contentExtendSize;
			if (this.m_scrollRect != null)
			{
				this.m_scrollRectLastScrollSpeed = this.m_scrollRect.velocity.x;
			}
			if (this.m_elementsRect == null)
			{
				this.m_elementsRect = new List<stRect>();
			}
			base.SetElementAmount(elementAmount);
			if (this.m_elementAmount > 0)
			{
				this.SelectElementImmediately(0);
			}
		}

		public void SetDontUpdate(bool bDontUpdate)
		{
			this.m_bDontUpdate = bDontUpdate;
		}

		protected override void Update()
		{
			if (this.m_belongedFormScript != null && this.m_belongedFormScript.IsClosed())
			{
				return;
			}
			if (this.m_useOptimized)
			{
				base.UpdateElementsScroll();
			}
			if (this.m_bDontUpdate || this.m_scrollRect == null)
			{
				return;
			}
			for (int i = 0; i < this.m_elementAmount; i++)
			{
				CUIListElementScript elemenet = base.GetElemenet(i);
				if (elemenet != null)
				{
					elemenet.gameObject.transform.localScale = Vector3.one * this.GetElementScale(i);
				}
			}
			if (this.m_fixingScroll && this.m_selectedElementIndex >= 0 && this.m_selectedElementIndex < this.m_elementAmount)
			{
				this.m_scrollRect.enabled = false;
				float num = this.m_contentRectTransform.anchoredPosition.x + this.m_fixScrollSpeed;
				if ((num > this.m_contentFixScrollTargetPosition && this.m_fixScrollSpeed > 0f) || (num < this.m_contentFixScrollTargetPosition && this.m_fixScrollSpeed < 0f))
				{
					this.m_contentRectTransform.anchoredPosition = new Vector2(this.m_contentFixScrollTargetPosition, this.m_contentRectTransform.anchoredPosition.y);
					this.m_fixScrollSpeed = 0f;
					this.m_fixingScroll = false;
					this.m_scrollRect.StopMovement();
				}
				else
				{
					this.m_contentRectTransform.anchoredPosition = new Vector2(num, this.m_contentRectTransform.anchoredPosition.y);
				}
			}
			else
			{
				this.m_scrollRect.enabled = true;
				FieldInfo field = this.m_scrollRect.GetType().GetField("m_Dragging", BindingFlags.Instance | BindingFlags.NonPublic);
				if (!(bool)field.GetValue(this.m_scrollRect))
				{
					if (this.m_scrollRectIsDragged)
					{
						float num2 = Mathf.Abs(this.m_scrollRect.velocity.x);
						int num3 = -1;
						if (this.m_contentRectTransform.anchoredPosition.x > 0f)
						{
							num3 = 0;
						}
						else if (this.m_contentRectTransform.anchoredPosition.x + this.m_contentSize.x < this.m_scrollAreaSize.x)
						{
							num3 = this.m_elementAmount - 1;
						}
						else if (num2 <= this.m_scrollRectLastScrollSpeed && num2 < this.m_minSpeedToFixScroll)
						{
							for (int j = 0; j < this.m_elementAmount; j++)
							{
								if (num3 < 0 && this.IsElementInSelectedArea(j))
								{
									num3 = j;
									break;
								}
							}
						}
						if (num3 >= 0)
						{
							this.SelectElement(num3, true);
							this.m_scrollRectIsDragged = false;
						}
					}
					else if (this.m_selectedElementIndex >= 0 && this.m_selectedElementIndex < this.m_elementAmount && this.m_contentRectTransform.anchoredPosition.x != this.m_contentFixScrollTargetPosition)
					{
						this.m_contentRectTransform.anchoredPosition = new Vector2(this.m_contentFixScrollTargetPosition, this.m_contentRectTransform.anchoredPosition.y);
						for (int k = 0; k < this.m_elementAmount; k++)
						{
							CUIListElementScript elemenet2 = base.GetElemenet(k);
							if (elemenet2 != null)
							{
								elemenet2.gameObject.transform.localScale = Vector3.one * this.GetElementScale(k);
							}
						}
					}
				}
				else
				{
					if (!this.m_scrollRectIsDragged)
					{
						this.DispatchOnStartDraggingEvent();
					}
					this.m_scrollRectIsDragged = true;
				}
				this.m_scrollRectLastScrollSpeed = Mathf.Abs(this.m_scrollRect.velocity.x);
			}
			base.DetectScroll();
		}

		public void SelectElementImmediately(int index)
		{
			base.SelectElement(index, true);
			if (index < 0 || index >= this.m_elementAmount)
			{
				return;
			}
			this.m_contentFixScrollTargetPosition = this.GetContentTargetPosition(index);
			if (this.m_contentRectTransform.anchoredPosition.x != this.m_contentFixScrollTargetPosition)
			{
				this.m_contentRectTransform.anchoredPosition = new Vector2(this.m_contentFixScrollTargetPosition, this.m_contentRectTransform.anchoredPosition.y);
			}
			this.m_scrollRectIsDragged = false;
			this.m_fixingScroll = false;
		}

		public override void SelectElement(int index, bool isDispatchSelectedChangeEvent = true)
		{
			base.SelectElement(index, isDispatchSelectedChangeEvent);
			if (index < 0 || index >= this.m_elementAmount)
			{
				return;
			}
			this.m_contentFixScrollTargetPosition = this.GetContentTargetPosition(index);
			if (this.m_contentRectTransform.anchoredPosition.x != this.m_contentFixScrollTargetPosition)
			{
				if (this.m_scrollRect != null)
				{
					this.m_scrollRect.StopMovement();
				}
				this.m_fixScrollSpeed = (this.m_contentFixScrollTargetPosition - this.m_contentRectTransform.anchoredPosition.x) / (this.m_fixScrollTime * (float)GameFramework.c_renderFPS);
				if (Mathf.Abs(this.m_fixScrollSpeed) < 0.001f)
				{
					this.m_contentRectTransform.anchoredPosition = new Vector2(this.m_contentFixScrollTargetPosition, this.m_contentRectTransform.anchoredPosition.y);
				}
				else
				{
					this.m_fixingScroll = true;
				}
				this.m_scrollRectIsDragged = false;
			}
		}

		protected override void ProcessElements()
		{
			this.m_contentSize = Vector2.zero;
			Vector2 zero = Vector2.zero;
			this.m_contentSize.x = this.m_contentSize.x + this.m_contentExtendSize;
			zero.x += this.m_contentExtendSize;
			for (int i = 0; i < this.m_elementAmount; i++)
			{
				stRect stRect = base.LayoutElement(i, ref this.m_contentSize, ref zero);
				if (i < this.m_elementsRect.Count)
				{
					this.m_elementsRect[i] = stRect;
				}
				else
				{
					this.m_elementsRect.Add(stRect);
				}
				if (!this.m_useOptimized || base.IsRectInScrollArea(ref stRect))
				{
					CUIListElementScript cUIListElementScript = base.CreateElement(i, ref stRect);
				}
			}
			this.m_contentSize.x = this.m_contentSize.x + this.m_contentExtendSize;
			this.ResizeContent(ref this.m_contentSize, false);
		}

		protected override void ResizeContent(ref Vector2 size, bool resetPosition)
		{
			if (this.m_contentRectTransform != null)
			{
				this.m_contentRectTransform.sizeDelta = size;
				this.m_contentRectTransform.pivot = new Vector2(0f, 0.5f);
				this.m_contentRectTransform.anchorMin = new Vector2(0f, 0.5f);
				this.m_contentRectTransform.anchorMax = new Vector2(0f, 0.5f);
				this.m_contentRectTransform.anchoredPosition = Vector2.zero;
				if (resetPosition)
				{
					this.m_contentRectTransform.anchoredPosition = Vector2.zero;
				}
			}
		}

		private bool IsElementInSelectedArea(int index)
		{
			float num = this.m_elementsRect[index].m_center.x + this.m_contentRectTransform.anchoredPosition.x;
			return num > this.m_selectAreaMin && num < this.m_selectAreaMax;
		}

		private float GetElementScale(int index)
		{
			int num = (int)(this.m_elementsRect[index].m_center.x + this.m_contentRectTransform.anchoredPosition.x);
			int num2 = (int)Mathf.Abs((float)num - this.m_stepListCenter);
			int num3 = num2 / (int)this.m_elementDefaultSize.x;
			int num4 = num2 % (int)this.m_elementDefaultSize.x;
			float num5 = Mathf.Pow(this.m_reductionRate, (float)num3);
			float num6 = Mathf.Pow(this.m_reductionRate, (float)(num3 + 1));
			return num5 - (num5 - num6) * ((float)num4 / this.m_elementDefaultSize.x);
		}

		private float GetContentTargetPosition(int selectedIndex)
		{
			if (selectedIndex < 0 || selectedIndex >= this.m_elementAmount)
			{
				return 0f;
			}
			float num = this.m_stepListCenter - this.m_elementsRect[selectedIndex].m_center.x;
			if (num > 0f)
			{
				num = 0f;
			}
			else if (num < this.m_scrollAreaSize.x - this.m_contentSize.x)
			{
				num = this.m_scrollAreaSize.x - this.m_contentSize.x;
			}
			return num;
		}

		private void DispatchOnStartDraggingEvent()
		{
			if (this.m_onStartDraggingEventID != enUIEventID.None)
			{
				CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
				uIEvent.m_eventID = this.m_onStartDraggingEventID;
				uIEvent.m_eventParams = this.m_onStartDraggingEventParams;
				uIEvent.m_srcFormScript = this.m_belongedFormScript;
				uIEvent.m_srcWidgetBelongedListScript = this.m_belongedListScript;
				uIEvent.m_srcWidgetIndexInBelongedList = this.m_indexInlist;
				uIEvent.m_srcWidget = base.gameObject;
				uIEvent.m_srcWidgetScript = this;
				uIEvent.m_pointerEventData = null;
				base.DispatchUIEvent(uIEvent);
			}
		}
	}
}
