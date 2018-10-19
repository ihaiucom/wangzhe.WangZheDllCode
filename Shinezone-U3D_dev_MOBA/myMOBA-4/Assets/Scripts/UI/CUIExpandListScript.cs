using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class CUIExpandListScript : CUIListScript
	{
		public float m_expandedTime = 0.15f;

		public float m_contentFixingSpeed = 1200f;

		private Vector2 m_elementExpandedSize;

		private enExpandListSelectingState m_selectingState;

		private Vector2 m_contentAnchoredPosition;

		private Vector2 m_targetContentAnchoredPosition;

		private float m_timeSlice;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			if (this.m_listType == enUIListType.VerticalGrid)
			{
				this.m_listType = enUIListType.Vertical;
			}
			else if (this.m_listType == enUIListType.HorizontalGrid)
			{
				this.m_listType = enUIListType.Horizontal;
			}
			if (this.m_elementsRect == null)
			{
				this.m_elementsRect = new List<stRect>();
			}
			base.Initialize(formScript);
			if (this.m_elementTemplate != null)
			{
				CUIExpandListElementScript component = this.m_elementTemplate.GetComponent<CUIExpandListElementScript>();
				if (component != null)
				{
					this.m_elementExpandedSize = component.m_expandedSize;
				}
			}
		}

		protected override void Update()
		{
			if (this.m_belongedFormScript != null && this.m_belongedFormScript.IsClosed())
			{
				return;
			}
			if (this.m_selectingState != enExpandListSelectingState.None)
			{
				if (this.m_scrollRect.enabled)
				{
					this.m_scrollRect.StopMovement();
					this.m_scrollRect.enabled = false;
				}
				this.UpdateSelectedElement(this.m_selectingState);
			}
			else if (!this.m_scrollRect.enabled)
			{
				this.m_scrollRect.StopMovement();
				this.m_scrollRect.enabled = true;
			}
			if (this.m_useOptimized)
			{
				base.UpdateElementsScroll();
			}
		}

		public override void SelectElement(int index, bool isDispatchSelectedChangeEvent = true)
		{
			if (this.m_selectingState != enExpandListSelectingState.None)
			{
				return;
			}
			this.m_lastSelectedElementIndex = this.m_selectedElementIndex;
			this.m_selectedElementIndex = index;
			if (this.m_lastSelectedElementIndex == this.m_selectedElementIndex)
			{
				this.m_selectedElementIndex = -1;
			}
			if (this.m_lastSelectedElementIndex >= 0)
			{
				CUIListElementScript elemenet = base.GetElemenet(this.m_lastSelectedElementIndex);
				if (elemenet != null)
				{
					elemenet.ChangeDisplay(false);
				}
			}
			if (this.m_selectedElementIndex >= 0)
			{
				CUIListElementScript elemenet2 = base.GetElemenet(this.m_selectedElementIndex);
				if (elemenet2 != null)
				{
					elemenet2.ChangeDisplay(true);
					if (elemenet2.onSelected != null)
					{
						elemenet2.onSelected();
					}
				}
			}
			base.DispatchElementSelectChangedEvent();
			this.m_contentAnchoredPosition = this.m_contentRectTransform.anchoredPosition;
			this.m_timeSlice = 0f;
			if (this.m_lastSelectedElementIndex >= 0)
			{
				this.m_selectingState = enExpandListSelectingState.Retract;
			}
			else if (this.m_selectedElementIndex >= 0)
			{
				this.m_targetContentAnchoredPosition = this.GetTargetContentAnchoredPosition(this.m_selectedElementIndex);
				this.m_selectingState = enExpandListSelectingState.Move;
				this.m_timeSlice = 0f;
			}
		}

		public void SelectElementImmediately(int index)
		{
			base.SelectElement(index, true);
			this.m_contentSize = Vector2.zero;
			Vector2 zero = Vector2.zero;
			if (this.m_listType == enUIListType.Horizontal)
			{
				zero.x += this.m_elementLayoutOffset;
			}
			else if (this.m_listType == enUIListType.Vertical)
			{
				zero.y += this.m_elementLayoutOffset;
			}
			for (int i = 0; i < this.m_elementAmount; i++)
			{
				stRect stRect = this.LayoutExpandElement(i, (float)((i != index) ? 0 : 1), ref this.m_contentSize, ref zero);
				if (i < this.m_elementsRect.Count)
				{
					this.m_elementsRect[i] = stRect;
				}
				else
				{
					this.m_elementsRect.Add(stRect);
				}
			}
			this.ResizeContent(ref this.m_contentSize, false);
			if (index < 0 || index >= this.m_elementAmount)
			{
				this.m_contentRectTransform.anchoredPosition = Vector2.zero;
			}
			else
			{
				this.m_contentRectTransform.anchoredPosition = this.GetTargetContentAnchoredPosition(index);
			}
			for (int j = 0; j < this.m_elementAmount; j++)
			{
				stRect stRect2 = this.m_elementsRect[j];
				CUIListElementScript elemenet = base.GetElemenet(j);
				if (elemenet != null)
				{
					elemenet.SetRect(ref stRect2);
				}
				else if (!this.m_useOptimized || base.IsRectInScrollArea(ref stRect2))
				{
					base.CreateElement(j, ref stRect2);
				}
			}
		}

		protected override void ProcessElements()
		{
			this.m_contentSize = Vector2.zero;
			Vector2 zero = Vector2.zero;
			if (this.m_listType == enUIListType.Horizontal)
			{
				zero.x += this.m_elementLayoutOffset;
			}
			else if (this.m_listType == enUIListType.Vertical)
			{
				zero.y += this.m_elementLayoutOffset;
			}
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
					base.CreateElement(i, ref stRect);
				}
			}
			this.ResizeContent(ref this.m_contentSize, false);
		}

		private stRect LayoutExpandElement(int index, float expandedRate, ref Vector2 contentSize, ref Vector2 offset)
		{
			stRect result = default(stRect);
			if (this.m_listType == enUIListType.Horizontal)
			{
				result.m_width = (int)(this.m_elementDefaultSize.x + (this.m_elementExpandedSize.x - this.m_elementDefaultSize.x) * expandedRate);
				result.m_height = (int)this.m_elementDefaultSize.y;
			}
			else
			{
				result.m_width = (int)this.m_elementDefaultSize.x;
				result.m_height = (int)(this.m_elementDefaultSize.y + (this.m_elementExpandedSize.y - this.m_elementDefaultSize.y) * expandedRate);
			}
			result.m_left = (int)offset.x;
			result.m_top = (int)offset.y;
			result.m_right = result.m_left + result.m_width;
			result.m_bottom = result.m_top - result.m_height;
			result.m_center = new Vector2((float)result.m_left + (float)result.m_width * 0.5f, (float)result.m_top - (float)result.m_height * 0.5f);
			if ((float)result.m_right > contentSize.x)
			{
				contentSize.x = (float)result.m_right;
			}
			if ((float)(-(float)result.m_bottom) > contentSize.y)
			{
				contentSize.y = (float)(-(float)result.m_bottom);
			}
			if (this.m_listType == enUIListType.Horizontal)
			{
				offset.x += (float)result.m_width + this.m_elementSpacing.x;
			}
			else if (this.m_listType == enUIListType.Vertical)
			{
				offset.y -= (float)result.m_height + this.m_elementSpacing.y;
			}
			return result;
		}

		private void UpdateSelectedElement(enExpandListSelectingState selectingState)
		{
			switch (selectingState)
			{
			case enExpandListSelectingState.Retract:
				if (this.m_timeSlice < this.m_expandedTime)
				{
					this.m_timeSlice += Time.deltaTime;
					this.m_contentSize = Vector2.zero;
					Vector2 zero = Vector2.zero;
					if (this.m_listType == enUIListType.Horizontal)
					{
						zero.x += this.m_elementLayoutOffset;
					}
					else if (this.m_listType == enUIListType.Vertical)
					{
						zero.y += this.m_elementLayoutOffset;
					}
					for (int i = 0; i < this.m_elementAmount; i++)
					{
						float num = 0f;
						if (i == this.m_lastSelectedElementIndex)
						{
							num = 1f - this.m_timeSlice / this.m_expandedTime;
							num = Mathf.Clamp(num, 0f, 1f);
						}
						stRect stRect = this.LayoutExpandElement(i, num, ref this.m_contentSize, ref zero);
						if (i < this.m_elementsRect.Count)
						{
							this.m_elementsRect[i] = stRect;
						}
						else
						{
							this.m_elementsRect.Add(stRect);
						}
					}
					this.ResizeContent(ref this.m_contentSize, false);
					if (this.m_selectedElementIndex < 0 || this.m_selectedElementIndex >= this.m_elementAmount)
					{
						if (this.m_listType == enUIListType.Horizontal)
						{
							if (this.m_contentAnchoredPosition.x > 0f)
							{
								this.m_contentAnchoredPosition.x = 0f;
							}
							else if (this.m_contentAnchoredPosition.x + this.m_contentSize.x < this.m_scrollAreaSize.x)
							{
								this.m_contentAnchoredPosition.x = this.m_scrollAreaSize.x - this.m_contentSize.x;
							}
						}
						else if (this.m_listType == enUIListType.Vertical)
						{
							if (this.m_contentAnchoredPosition.y < 0f)
							{
								this.m_contentAnchoredPosition.y = 0f;
							}
							else if (this.m_contentAnchoredPosition.y - this.m_contentSize.y > -this.m_scrollAreaSize.y)
							{
								this.m_contentAnchoredPosition.y = -this.m_scrollAreaSize.y + this.m_contentSize.y;
							}
						}
					}
				}
				else if (this.m_selectedElementIndex >= 0 && this.m_selectedElementIndex < this.m_elementAmount)
				{
					this.m_targetContentAnchoredPosition = this.GetTargetContentAnchoredPosition(this.m_selectedElementIndex);
					this.m_selectingState = enExpandListSelectingState.Move;
					this.m_timeSlice = 0f;
				}
				else
				{
					this.m_selectingState = enExpandListSelectingState.None;
				}
				break;
			case enExpandListSelectingState.Move:
				if (this.m_contentAnchoredPosition != this.m_targetContentAnchoredPosition)
				{
					if (this.m_listType == enUIListType.Horizontal)
					{
						int num2 = (this.m_targetContentAnchoredPosition.x <= this.m_contentAnchoredPosition.x) ? -1 : 1;
						this.m_contentAnchoredPosition.x = this.m_contentAnchoredPosition.x + Time.deltaTime * this.m_contentFixingSpeed * (float)num2;
						if ((num2 > 0 && this.m_contentAnchoredPosition.x >= this.m_targetContentAnchoredPosition.x) || (num2 < 0 && this.m_contentAnchoredPosition.x <= this.m_targetContentAnchoredPosition.x))
						{
							this.m_contentAnchoredPosition = this.m_targetContentAnchoredPosition;
						}
					}
					else if (this.m_listType == enUIListType.Vertical)
					{
						int num3 = (this.m_targetContentAnchoredPosition.y <= this.m_contentAnchoredPosition.y) ? -1 : 1;
						this.m_contentAnchoredPosition.y = this.m_contentAnchoredPosition.y + Time.deltaTime * this.m_contentFixingSpeed * (float)num3;
						if ((num3 > 0 && this.m_contentAnchoredPosition.y >= this.m_targetContentAnchoredPosition.y) || (num3 < 0 && this.m_contentAnchoredPosition.y <= this.m_targetContentAnchoredPosition.y))
						{
							this.m_contentAnchoredPosition = this.m_targetContentAnchoredPosition;
						}
					}
				}
				else
				{
					this.m_selectingState = enExpandListSelectingState.Expand;
					this.m_timeSlice = 0f;
				}
				break;
			case enExpandListSelectingState.Expand:
				if (this.m_timeSlice < this.m_expandedTime)
				{
					this.m_timeSlice += Time.deltaTime;
					this.m_contentSize = Vector2.zero;
					Vector2 zero2 = Vector2.zero;
					if (this.m_listType == enUIListType.Horizontal)
					{
						zero2.x += this.m_elementLayoutOffset;
					}
					else if (this.m_listType == enUIListType.Vertical)
					{
						zero2.y += this.m_elementLayoutOffset;
					}
					for (int j = 0; j < this.m_elementAmount; j++)
					{
						float num4 = 0f;
						if (j == this.m_selectedElementIndex)
						{
							num4 = this.m_timeSlice / this.m_expandedTime;
							num4 = Mathf.Clamp(num4, 0f, 1f);
						}
						stRect stRect2 = this.LayoutExpandElement(j, num4, ref this.m_contentSize, ref zero2);
						if (j < this.m_elementsRect.Count)
						{
							this.m_elementsRect[j] = stRect2;
						}
						else
						{
							this.m_elementsRect.Add(stRect2);
						}
					}
					this.ResizeContent(ref this.m_contentSize, false);
				}
				else
				{
					this.m_selectingState = enExpandListSelectingState.None;
				}
				break;
			}
			for (int k = 0; k < this.m_elementAmount; k++)
			{
				stRect stRect3 = this.m_elementsRect[k];
				CUIListElementScript elemenet = base.GetElemenet(k);
				if (elemenet != null)
				{
					elemenet.SetRect(ref stRect3);
				}
			}
			this.m_contentRectTransform.anchoredPosition = this.m_contentAnchoredPosition;
		}

		private Vector2 GetTargetContentAnchoredPosition(int selectedElementIndex)
		{
			if (selectedElementIndex < 0 || selectedElementIndex >= this.m_elementAmount)
			{
				return this.m_contentAnchoredPosition;
			}
			stRect stRect = this.m_elementsRect[this.m_selectedElementIndex];
			stRect.m_width = (int)this.m_elementExpandedSize.x;
			stRect.m_height = (int)this.m_elementExpandedSize.y;
			stRect.m_right = stRect.m_left + stRect.m_width;
			stRect.m_bottom = stRect.m_top - stRect.m_height;
			Vector2 contentAnchoredPosition = this.m_contentAnchoredPosition;
			if (this.m_listType == enUIListType.Horizontal)
			{
				if (contentAnchoredPosition.x + (float)stRect.m_right > this.m_scrollAreaSize.x)
				{
					contentAnchoredPosition.x = this.m_scrollAreaSize.x - (float)stRect.m_right;
				}
				if (contentAnchoredPosition.x + (float)stRect.m_left < 0f)
				{
					contentAnchoredPosition.x = (float)(-(float)stRect.m_left);
				}
			}
			else if (this.m_listType == enUIListType.Vertical)
			{
				if (contentAnchoredPosition.y + (float)stRect.m_bottom < -this.m_scrollAreaSize.y)
				{
					contentAnchoredPosition.y = -this.m_scrollAreaSize.y - (float)stRect.m_bottom;
				}
				if (contentAnchoredPosition.y + (float)stRect.m_top > 0f)
				{
					contentAnchoredPosition.y = (float)(-(float)stRect.m_top);
				}
			}
			return contentAnchoredPosition;
		}
	}
}
