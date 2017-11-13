using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class CUIListScript : CUIComponent
	{
		public enUIListType m_listType;

		public GameObject m_comboClickPanel;

		public int m_elementAmount;

		public Vector2 m_elementSpacing;

		public float m_elementLayoutOffset;

		public bool m_autoCenteredElements;

		public bool m_autoCenteredBothSides;

		public GameObject m_ZeroTipsObj;

		protected int m_selectedElementIndex = -1;

		protected int m_lastSelectedElementIndex = -1;

		public bool m_alwaysDispatchSelectedChangeEvent;

		[HideInInspector]
		public enUIEventID m_listSelectChangedEventID;

		public stUIEventParams m_listSelectChangedEventParams;

		[HideInInspector]
		public enUIEventID m_listScrollChangedEventID;

		public stUIEventParams m_listScrollChangedEventParams;

		protected ListView<CUIListElementScript> m_elementScripts;

		protected ListView<CUIListElementScript> m_unUsedElementScripts;

		protected GameObject m_elementTemplate;

		protected string m_elementName;

		protected Vector2 m_elementDefaultSize;

		protected List<Vector2> m_elementsSize;

		protected List<stRect> m_elementsRect;

		[HideInInspector]
		public ScrollRect m_scrollRect;

		[HideInInspector]
		public Vector2 m_scrollAreaSize;

		protected GameObject m_content;

		protected RectTransform m_contentRectTransform;

		protected Vector2 m_contentSize;

		protected Scrollbar m_scrollBar;

		protected Vector2 m_lastContentPosition;

		public bool m_useOptimized;

		[HideInInspector]
		public bool m_useExternalElement;

		[HideInInspector]
		public string m_externalElementPrefabPath;

		[HideInInspector]
		public bool m_autoAdjustScrollAreaSize;

		[HideInInspector]
		public Vector2 m_scrollRectAreaMaxSize = new Vector2(100f, 100f);

		public bool m_scrollExternal;

		public GameObject m_extraContent;

		private bool m_isExtraContentEnabled = true;

		public bool m_isDisableScrollRect;

		public float m_fSpeed = 0.2f;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			base.Initialize(formScript);
			this.m_selectedElementIndex = -1;
			this.m_lastSelectedElementIndex = -1;
			this.m_scrollRect = base.GetComponentInChildren<ScrollRect>(base.gameObject);
			if (this.m_scrollRect != null)
			{
				this.m_scrollRect.enabled = false;
				RectTransform rectTransform = this.m_scrollRect.transform as RectTransform;
				this.m_scrollAreaSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
				this.m_content = this.m_scrollRect.get_content().gameObject;
			}
			this.m_scrollBar = base.GetComponentInChildren<Scrollbar>(base.gameObject);
			if (this.m_listType == enUIListType.Vertical || this.m_listType == enUIListType.VerticalGrid)
			{
				if (this.m_scrollBar != null)
				{
					this.m_scrollBar.set_direction(2);
				}
				DebugHelper.Assert(this.m_scrollRect != null);
				if (this.m_scrollRect != null)
				{
					this.m_scrollRect.set_horizontal(false);
					this.m_scrollRect.set_vertical(true);
					this.m_scrollRect.set_horizontalScrollbar(null);
					this.m_scrollRect.set_verticalScrollbar(this.m_scrollBar);
				}
			}
			else if (this.m_listType == enUIListType.Horizontal || this.m_listType == enUIListType.HorizontalGrid)
			{
				if (this.m_scrollBar != null)
				{
					this.m_scrollBar.set_direction(0);
				}
				DebugHelper.Assert(this.m_scrollRect != null);
				if (this.m_scrollRect != null)
				{
					this.m_scrollRect.set_horizontal(true);
					this.m_scrollRect.set_vertical(false);
					this.m_scrollRect.set_horizontalScrollbar(this.m_scrollBar);
					this.m_scrollRect.set_verticalScrollbar(null);
				}
			}
			this.m_elementScripts = new ListView<CUIListElementScript>();
			this.m_unUsedElementScripts = new ListView<CUIListElementScript>();
			if (this.m_useOptimized && this.m_elementsRect == null)
			{
				this.m_elementsRect = new List<stRect>();
			}
			CUIListElementScript cUIListElementScript = null;
			if (this.m_useExternalElement)
			{
				GameObject gameObject = null;
				if (this.m_externalElementPrefabPath != null)
				{
					gameObject = (GameObject)Singleton<CResourceManager>.GetInstance().GetResource(this.m_externalElementPrefabPath, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content;
					if (gameObject != null)
					{
						cUIListElementScript = gameObject.GetComponent<CUIListElementScript>();
					}
				}
				if (cUIListElementScript != null && gameObject != null)
				{
					cUIListElementScript.Initialize(formScript);
					this.m_elementTemplate = gameObject;
					this.m_elementName = gameObject.name;
					this.m_elementDefaultSize = cUIListElementScript.m_defaultSize;
				}
			}
			else
			{
				cUIListElementScript = base.GetComponentInChildren<CUIListElementScript>(base.gameObject);
				if (cUIListElementScript != null)
				{
					cUIListElementScript.Initialize(formScript);
					this.m_elementTemplate = cUIListElementScript.gameObject;
					this.m_elementName = cUIListElementScript.gameObject.name;
					this.m_elementDefaultSize = cUIListElementScript.m_defaultSize;
					if (this.m_elementTemplate != null)
					{
						this.m_elementTemplate.name = this.m_elementName + "_Template";
					}
				}
			}
			if (this.m_elementTemplate != null)
			{
				CUIListElementScript component = this.m_elementTemplate.GetComponent<CUIListElementScript>();
				if (component != null && component.m_useSetActiveForDisplay)
				{
					this.m_elementTemplate.CustomSetActive(false);
				}
				else
				{
					if (!this.m_elementTemplate.activeSelf)
					{
						this.m_elementTemplate.SetActive(true);
					}
					CanvasGroup canvasGroup = this.m_elementTemplate.GetComponent<CanvasGroup>();
					if (canvasGroup == null)
					{
						canvasGroup = this.m_elementTemplate.AddComponent<CanvasGroup>();
					}
					canvasGroup.alpha = 0f;
					canvasGroup.blocksRaycasts = false;
				}
			}
			if (this.m_content != null)
			{
				this.m_contentRectTransform = (this.m_content.transform as RectTransform);
				this.m_contentRectTransform.pivot = new Vector2(0f, 1f);
				this.m_contentRectTransform.anchorMin = new Vector2(0f, 1f);
				this.m_contentRectTransform.anchorMax = new Vector2(0f, 1f);
				this.m_contentRectTransform.anchoredPosition = Vector2.zero;
				this.m_contentRectTransform.localRotation = Quaternion.identity;
				this.m_contentRectTransform.localScale = new Vector3(1f, 1f, 1f);
				this.m_lastContentPosition = this.m_contentRectTransform.anchoredPosition;
			}
			if (this.m_extraContent != null)
			{
				RectTransform rectTransform2 = this.m_extraContent.transform as RectTransform;
				rectTransform2.pivot = new Vector2(0f, 1f);
				rectTransform2.anchorMin = new Vector2(0f, 1f);
				rectTransform2.anchorMax = new Vector2(0f, 1f);
				rectTransform2.anchoredPosition = Vector2.zero;
				rectTransform2.localRotation = Quaternion.identity;
				rectTransform2.localScale = Vector3.one;
				if (this.m_elementTemplate != null)
				{
					rectTransform2.sizeDelta = new Vector2((this.m_elementTemplate.transform as RectTransform).rect.width, rectTransform2.sizeDelta.y);
				}
				if (rectTransform2.parent != null && this.m_content != null)
				{
					rectTransform2.parent.SetParent(this.m_content.transform);
				}
				this.m_extraContent.SetActive(false);
			}
			this.m_isExtraContentEnabled = true;
			if (this.m_elementTemplate != null)
			{
				this.SetElementAmount(this.m_elementAmount);
			}
		}

		public void SetExternalElementPrefab(string externalElmtPrefPath)
		{
			CUIListElementScript cUIListElementScript = null;
			GameObject gameObject = null;
			if (externalElmtPrefPath != null)
			{
				gameObject = (GameObject)Singleton<CResourceManager>.GetInstance().GetResource(externalElmtPrefPath, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content;
				if (gameObject != null)
				{
					cUIListElementScript = gameObject.GetComponent<CUIListElementScript>();
				}
			}
			if (cUIListElementScript != null && gameObject != null)
			{
				cUIListElementScript.Initialize(this.m_belongedFormScript);
				this.m_elementTemplate = gameObject;
				this.m_elementName = gameObject.name;
				this.m_elementDefaultSize = cUIListElementScript.m_defaultSize;
			}
			if (this.m_elementTemplate != null)
			{
				this.SetElementAmount(this.m_elementAmount);
			}
		}

		protected override void OnDestroy()
		{
			if (LeanTween.IsInitialised())
			{
				LeanTween.cancel(base.gameObject);
			}
			this.m_ZeroTipsObj = null;
			this.m_elementTemplate = null;
			this.m_scrollRect = null;
			this.m_content = null;
			this.m_contentRectTransform = null;
			this.m_scrollBar = null;
			this.m_extraContent = null;
			if (this.m_elementScripts != null)
			{
				this.m_elementScripts.Clear();
				this.m_elementScripts = null;
			}
			if (this.m_unUsedElementScripts != null)
			{
				this.m_unUsedElementScripts.Clear();
				this.m_unUsedElementScripts = null;
			}
			base.OnDestroy();
		}

		protected virtual void Update()
		{
			if (this.m_belongedFormScript != null && this.m_belongedFormScript.IsClosed())
			{
				return;
			}
			if (this.m_useOptimized)
			{
				this.UpdateElementsScroll();
			}
			if (this.m_scrollRect != null && !this.m_scrollExternal && !this.m_isDisableScrollRect)
			{
				if (this.m_contentSize.x > this.m_scrollAreaSize.x || this.m_contentSize.y > this.m_scrollAreaSize.y)
				{
					if (!this.m_scrollRect.enabled)
					{
						this.m_scrollRect.enabled = true;
					}
				}
				else if ((double)Mathf.Abs(this.m_contentRectTransform.anchoredPosition.x) < 0.001 && (double)Mathf.Abs(this.m_contentRectTransform.anchoredPosition.y) < 0.001 && this.m_scrollRect.enabled)
				{
					this.m_scrollRect.enabled = false;
				}
				this.DetectScroll();
			}
		}

		protected void DetectScroll()
		{
			if (this.m_contentRectTransform.anchoredPosition != this.m_lastContentPosition)
			{
				this.m_lastContentPosition = this.m_contentRectTransform.anchoredPosition;
				this.DispatchScrollChangedEvent();
			}
		}

		public void SetElementAmount(int amount)
		{
			this.SetElementAmount(amount, null);
		}

		public virtual void SetElementAmount(int amount, List<Vector2> elementsSize)
		{
			if (amount < 0)
			{
				amount = 0;
			}
			if (elementsSize != null && amount != elementsSize.get_Count())
			{
				return;
			}
			this.RecycleElement(false);
			this.m_elementAmount = amount;
			this.m_elementsSize = elementsSize;
			this.ProcessElements();
			this.ProcessUnUsedElement();
			if (this.m_ZeroTipsObj != null)
			{
				if (amount <= 0)
				{
					this.m_ZeroTipsObj.SetActive(true);
				}
				else
				{
					this.m_ZeroTipsObj.SetActive(false);
				}
			}
		}

		public void SetElementSelectChangedEvent(enUIEventID eventID)
		{
			this.m_listSelectChangedEventID = eventID;
		}

		public void SetElementSelectChangedEvent(enUIEventID eventID, stUIEventParams eventParams)
		{
			this.m_listSelectChangedEventID = eventID;
			this.m_listSelectChangedEventParams = eventParams;
		}

		public virtual void SelectElement(int index, bool isDispatchSelectedChangeEvent = true)
		{
			this.m_lastSelectedElementIndex = this.m_selectedElementIndex;
			this.m_selectedElementIndex = index;
			if (this.m_lastSelectedElementIndex == this.m_selectedElementIndex)
			{
				if (this.m_alwaysDispatchSelectedChangeEvent && isDispatchSelectedChangeEvent)
				{
					this.DispatchElementSelectChangedEvent();
				}
				return;
			}
			if (this.m_lastSelectedElementIndex >= 0)
			{
				CUIListElementScript elemenet = this.GetElemenet(this.m_lastSelectedElementIndex);
				if (elemenet != null)
				{
					elemenet.ChangeDisplay(false);
				}
			}
			if (this.m_selectedElementIndex >= 0)
			{
				CUIListElementScript elemenet2 = this.GetElemenet(this.m_selectedElementIndex);
				if (elemenet2 != null)
				{
					elemenet2.ChangeDisplay(true);
					if (elemenet2.onSelected != null)
					{
						elemenet2.onSelected();
					}
				}
			}
			if (isDispatchSelectedChangeEvent)
			{
				this.DispatchElementSelectChangedEvent();
			}
		}

		public int GetElementAmount()
		{
			return this.m_elementAmount;
		}

		public CUIListElementScript GetElemenet(int index)
		{
			if (index < 0 || index >= this.m_elementAmount)
			{
				return null;
			}
			if (this.m_useOptimized)
			{
				for (int i = 0; i < this.m_elementScripts.Count; i++)
				{
					if (this.m_elementScripts[i].m_index == index)
					{
						return this.m_elementScripts[i];
					}
				}
				return null;
			}
			return this.m_elementScripts[index];
		}

		public CUIListElementScript GetSelectedElement()
		{
			return this.GetElemenet(this.m_selectedElementIndex);
		}

		public CUIListElementScript GetLastSelectedElement()
		{
			return this.GetElemenet(this.m_lastSelectedElementIndex);
		}

		public Vector2 GetEelementDefaultSize()
		{
			return this.m_elementDefaultSize;
		}

		public int GetSelectedIndex()
		{
			return this.m_selectedElementIndex;
		}

		public int GetLastSelectedIndex()
		{
			return this.m_lastSelectedElementIndex;
		}

		public bool IsElementInScrollArea(int index)
		{
			if (index < 0 || index >= this.m_elementAmount)
			{
				return false;
			}
			stRect stRect = this.m_useOptimized ? this.m_elementsRect.get_Item(index) : this.m_elementScripts[index].m_rect;
			return this.IsRectInScrollArea(ref stRect);
		}

		public Vector2 GetContentSize()
		{
			return this.m_contentSize;
		}

		public Vector2 GetScrollAreaSize()
		{
			return this.m_scrollAreaSize;
		}

		public Vector2 GetContentPosition()
		{
			return this.m_lastContentPosition;
		}

		public bool IsNeedScroll()
		{
			return this.m_contentSize.x > this.m_scrollAreaSize.x || this.m_contentSize.y > this.m_scrollAreaSize.y;
		}

		public void ResetContentPosition()
		{
			if (LeanTween.IsInitialised())
			{
				LeanTween.cancel(base.gameObject);
			}
			if (this.m_contentRectTransform != null)
			{
				this.m_contentRectTransform.anchoredPosition = Vector2.zero;
			}
		}

		public void MoveElementInScrollArea(int index, bool moveImmediately)
		{
			if (index < 0 || index >= this.m_elementAmount)
			{
				return;
			}
			Vector2 zero = Vector2.zero;
			Vector2 zero2 = Vector2.zero;
			stRect stRect = this.m_useOptimized ? this.m_elementsRect.get_Item(index) : this.m_elementScripts[index].m_rect;
			zero2.x = this.m_contentRectTransform.anchoredPosition.x + (float)stRect.m_left;
			zero2.y = this.m_contentRectTransform.anchoredPosition.y + (float)stRect.m_top;
			if (zero2.x < 0f)
			{
				zero.x = -zero2.x;
			}
			else if (zero2.x + (float)stRect.m_width > this.m_scrollAreaSize.x)
			{
				zero.x = this.m_scrollAreaSize.x - (zero2.x + (float)stRect.m_width);
			}
			if (zero2.y > 0f)
			{
				zero.y = -zero2.y;
			}
			else if (zero2.y - (float)stRect.m_height < -this.m_scrollAreaSize.y)
			{
				zero.y = -this.m_scrollAreaSize.y - (zero2.y - (float)stRect.m_height);
			}
			if (moveImmediately)
			{
				this.m_contentRectTransform.anchoredPosition += zero;
			}
			else
			{
				Vector2 to = this.m_contentRectTransform.anchoredPosition + zero;
				LeanTween.value(base.gameObject, delegate(Vector2 pos)
				{
					this.m_contentRectTransform.anchoredPosition = pos;
				}, this.m_contentRectTransform.anchoredPosition, to, this.m_fSpeed);
			}
		}

		public virtual bool IsSelectedIndex(int index)
		{
			return this.m_selectedElementIndex == index;
		}

		public void ShowExtraContent()
		{
			if (this.m_extraContent != null && this.m_isExtraContentEnabled)
			{
				this.m_extraContent.CustomSetActive(true);
			}
		}

		public void HideExtraContent()
		{
			if (this.m_extraContent != null && this.m_isExtraContentEnabled)
			{
				this.m_extraContent.CustomSetActive(false);
			}
		}

		public void MoveContent(Vector2 offset)
		{
			if (this.m_contentRectTransform != null)
			{
				Vector2 vector = (this.m_content.transform as RectTransform).anchoredPosition;
				vector += offset;
				if (offset.x != 0f)
				{
					if (vector.x > 0f)
					{
						vector.x = 0f;
					}
					else if (vector.x + this.m_contentSize.x < this.m_scrollAreaSize.x)
					{
						vector.x = this.m_scrollAreaSize.x - this.m_contentSize.x;
					}
				}
				if (offset.y != 0f)
				{
					if (vector.y < 0f)
					{
						vector.y = 0f;
					}
					else if (this.m_contentSize.y - vector.y < this.m_scrollAreaSize.y)
					{
						vector.y = this.m_contentSize.y - this.m_scrollAreaSize.y;
					}
				}
				this.m_contentRectTransform.anchoredPosition = vector;
			}
		}

		protected void DispatchElementSelectChangedEvent()
		{
			if (this.m_listSelectChangedEventID != enUIEventID.None)
			{
				CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
				uIEvent.m_eventID = this.m_listSelectChangedEventID;
				uIEvent.m_eventParams = this.m_listSelectChangedEventParams;
				uIEvent.m_srcFormScript = this.m_belongedFormScript;
				uIEvent.m_srcWidgetBelongedListScript = this.m_belongedListScript;
				uIEvent.m_srcWidgetIndexInBelongedList = this.m_indexInlist;
				uIEvent.m_srcWidget = base.gameObject;
				uIEvent.m_srcWidgetScript = this;
				uIEvent.m_pointerEventData = null;
				base.DispatchUIEvent(uIEvent);
			}
		}

		protected void DispatchScrollChangedEvent()
		{
			if (this.m_listScrollChangedEventID != enUIEventID.None)
			{
				CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
				uIEvent.m_eventID = this.m_listScrollChangedEventID;
				uIEvent.m_eventParams = this.m_listScrollChangedEventParams;
				uIEvent.m_srcFormScript = this.m_belongedFormScript;
				uIEvent.m_srcWidgetBelongedListScript = this.m_belongedListScript;
				uIEvent.m_srcWidgetIndexInBelongedList = this.m_indexInlist;
				uIEvent.m_srcWidget = base.gameObject;
				uIEvent.m_srcWidgetScript = this;
				uIEvent.m_pointerEventData = null;
				base.DispatchUIEvent(uIEvent);
			}
		}

		protected virtual void ProcessElements()
		{
			this.m_contentSize = Vector2.zero;
			Vector2 zero = Vector2.zero;
			if (this.m_listType == enUIListType.Vertical || this.m_listType == enUIListType.VerticalGrid)
			{
				zero.y += this.m_elementLayoutOffset;
			}
			else
			{
				zero.x += this.m_elementLayoutOffset;
			}
			for (int i = 0; i < this.m_elementAmount; i++)
			{
				stRect stRect = this.LayoutElement(i, ref this.m_contentSize, ref zero);
				if (this.m_useOptimized)
				{
					if (i < this.m_elementsRect.get_Count())
					{
						this.m_elementsRect.set_Item(i, stRect);
					}
					else
					{
						this.m_elementsRect.Add(stRect);
					}
				}
				if (!this.m_useOptimized || this.IsRectInScrollArea(ref stRect))
				{
					this.CreateElement(i, ref stRect);
				}
			}
			if (this.m_extraContent != null)
			{
				if (this.m_elementAmount > 0 && this.m_isExtraContentEnabled)
				{
					this.ProcessExtraContent(ref this.m_contentSize, zero);
				}
				else
				{
					this.m_extraContent.CustomSetActive(false);
				}
			}
			this.ResizeContent(ref this.m_contentSize, false);
		}

		private void ProcessExtraContent(ref Vector2 contentSize, Vector2 offset)
		{
			RectTransform rectTransform = this.m_extraContent.transform as RectTransform;
			rectTransform.anchoredPosition = offset;
			this.m_extraContent.CustomSetActive(true);
			if (this.m_listType == enUIListType.Horizontal || this.m_listType == enUIListType.HorizontalGrid)
			{
				contentSize.x += rectTransform.rect.width + this.m_elementSpacing.x;
			}
			else
			{
				contentSize.y += rectTransform.rect.height + this.m_elementSpacing.y;
			}
		}

		protected void UpdateElementsScroll()
		{
			int i = 0;
			while (i < this.m_elementScripts.Count)
			{
				if (!this.IsRectInScrollArea(ref this.m_elementScripts[i].m_rect))
				{
					this.RecycleElement(this.m_elementScripts[i], true);
				}
				else
				{
					i++;
				}
			}
			for (int j = 0; j < this.m_elementAmount; j++)
			{
				stRect stRect = this.m_elementsRect.get_Item(j);
				if (this.IsRectInScrollArea(ref stRect))
				{
					bool flag = false;
					for (int k = 0; k < this.m_elementScripts.Count; k++)
					{
						if (this.m_elementScripts[k].m_index == j)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						this.CreateElement(j, ref stRect);
					}
				}
			}
		}

		protected stRect LayoutElement(int index, ref Vector2 contentSize, ref Vector2 offset)
		{
			stRect result = default(stRect);
			result.m_width = (int)((this.m_elementsSize == null || this.m_listType == enUIListType.Vertical || this.m_listType == enUIListType.VerticalGrid || this.m_listType == enUIListType.HorizontalGrid) ? this.m_elementDefaultSize.x : this.m_elementsSize.get_Item(index).x);
			result.m_height = (int)((this.m_elementsSize == null || this.m_listType == enUIListType.Horizontal || this.m_listType == enUIListType.VerticalGrid || this.m_listType == enUIListType.HorizontalGrid) ? this.m_elementDefaultSize.y : this.m_elementsSize.get_Item(index).y);
			result.m_left = (int)offset.x;
			result.m_top = (int)offset.y;
			result.m_right = result.m_left + result.m_width;
			result.m_bottom = result.m_top - result.m_height;
			result.m_center = new Vector2((float)result.m_left + (float)result.m_width * 0.5f, (float)result.m_top - (float)result.m_height * 0.5f);
			if ((float)result.m_right > contentSize.x)
			{
				contentSize.x = (float)result.m_right;
			}
			if (-(float)result.m_bottom > contentSize.y)
			{
				contentSize.y = -(float)result.m_bottom;
			}
			switch (this.m_listType)
			{
			case enUIListType.Vertical:
				offset.y -= (float)result.m_height + this.m_elementSpacing.y;
				break;
			case enUIListType.Horizontal:
				offset.x += (float)result.m_width + this.m_elementSpacing.x;
				break;
			case enUIListType.VerticalGrid:
				offset.x += (float)result.m_width + this.m_elementSpacing.x;
				if (offset.x + (float)result.m_width > this.m_scrollAreaSize.x)
				{
					offset.x = 0f;
					offset.y -= (float)result.m_height + this.m_elementSpacing.y;
				}
				break;
			case enUIListType.HorizontalGrid:
				offset.y -= (float)result.m_height + this.m_elementSpacing.y;
				if (-offset.y + (float)result.m_height > this.m_scrollAreaSize.y)
				{
					offset.y = 0f;
					offset.x += (float)result.m_width + this.m_elementSpacing.x;
				}
				break;
			}
			return result;
		}

		protected virtual void ResizeContent(ref Vector2 size, bool resetPosition)
		{
			float num = 0f;
			float num2 = 0f;
			if (this.m_autoAdjustScrollAreaSize)
			{
				Vector2 scrollAreaSize = this.m_scrollAreaSize;
				this.m_scrollAreaSize = size;
				if (this.m_scrollAreaSize.x > this.m_scrollRectAreaMaxSize.x)
				{
					this.m_scrollAreaSize.x = this.m_scrollRectAreaMaxSize.x;
				}
				if (this.m_scrollAreaSize.y > this.m_scrollRectAreaMaxSize.y)
				{
					this.m_scrollAreaSize.y = this.m_scrollRectAreaMaxSize.y;
				}
				Vector2 vector = this.m_scrollAreaSize - scrollAreaSize;
				if (vector != Vector2.zero)
				{
					RectTransform rectTransform = base.gameObject.transform as RectTransform;
					if (rectTransform.anchorMin == rectTransform.anchorMax)
					{
						rectTransform.sizeDelta += vector;
					}
				}
			}
			else if (this.m_autoCenteredElements)
			{
				if (this.m_listType == enUIListType.Vertical && size.y < this.m_scrollAreaSize.y)
				{
					num2 = (size.y - this.m_scrollAreaSize.y) / 2f;
					if (this.m_autoCenteredBothSides)
					{
						num = (this.m_scrollAreaSize.x - size.x) / 2f;
					}
				}
				else if (this.m_listType == enUIListType.Horizontal && size.x < this.m_scrollAreaSize.x)
				{
					num = (this.m_scrollAreaSize.x - size.x) / 2f;
					if (this.m_autoCenteredBothSides)
					{
						num2 = (size.y - this.m_scrollAreaSize.y) / 2f;
					}
				}
				else if (this.m_listType == enUIListType.VerticalGrid && size.x < this.m_scrollAreaSize.x)
				{
					float num3 = size.x;
					float num4 = num3 + this.m_elementSpacing.x;
					while (true)
					{
						num3 = num4 + this.m_elementDefaultSize.x;
						if (num3 > this.m_scrollAreaSize.x)
						{
							break;
						}
						size.x = num3;
						num4 = num3 + this.m_elementSpacing.x;
					}
					num = (this.m_scrollAreaSize.x - size.x) / 2f;
				}
				else if (this.m_listType == enUIListType.HorizontalGrid && size.y < this.m_scrollAreaSize.y)
				{
					float num5 = size.y;
					float num6 = num5 + this.m_elementSpacing.y;
					while (true)
					{
						num5 = num6 + this.m_elementDefaultSize.y;
						if (num5 > this.m_scrollAreaSize.y)
						{
							break;
						}
						size.y = num5;
						num6 = num5 + this.m_elementSpacing.y;
					}
					num2 = (size.y - this.m_scrollAreaSize.y) / 2f;
				}
			}
			if (size.x < this.m_scrollAreaSize.x)
			{
				size.x = this.m_scrollAreaSize.x;
			}
			if (size.y < this.m_scrollAreaSize.y)
			{
				size.y = this.m_scrollAreaSize.y;
			}
			if (this.m_contentRectTransform != null)
			{
				this.m_contentRectTransform.sizeDelta = size;
				if (resetPosition)
				{
					this.m_contentRectTransform.anchoredPosition = Vector2.zero;
				}
				if (this.m_autoCenteredElements)
				{
					if (num != 0f)
					{
						this.m_contentRectTransform.anchoredPosition = new Vector2(num, this.m_contentRectTransform.anchoredPosition.y);
					}
					if (num2 != 0f)
					{
						this.m_contentRectTransform.anchoredPosition = new Vector2(this.m_contentRectTransform.anchoredPosition.x, num2);
					}
				}
			}
		}

		protected bool IsRectInScrollArea(ref stRect rect)
		{
			Vector2 zero = Vector2.zero;
			zero.x = this.m_contentRectTransform.anchoredPosition.x + (float)rect.m_left;
			zero.y = this.m_contentRectTransform.anchoredPosition.y + (float)rect.m_top;
			return zero.x + (float)rect.m_width >= 0f && zero.x <= this.m_scrollAreaSize.x && zero.y - (float)rect.m_height <= 0f && zero.y >= -this.m_scrollAreaSize.y;
		}

		protected void RecycleElement(bool disableElement)
		{
			while (this.m_elementScripts.Count > 0)
			{
				CUIListElementScript cUIListElementScript = this.m_elementScripts[0];
				this.m_elementScripts.RemoveAt(0);
				if (disableElement)
				{
					cUIListElementScript.Disable();
				}
				this.m_unUsedElementScripts.Add(cUIListElementScript);
			}
		}

		protected void RecycleElement(CUIListElementScript elementScript, bool disableElement)
		{
			if (disableElement)
			{
				elementScript.Disable();
			}
			this.m_elementScripts.Remove(elementScript);
			this.m_unUsedElementScripts.Add(elementScript);
		}

		protected CUIListElementScript CreateElement(int index, ref stRect rect)
		{
			CUIListElementScript cUIListElementScript = null;
			if (this.m_unUsedElementScripts.Count > 0)
			{
				cUIListElementScript = this.m_unUsedElementScripts[0];
				this.m_unUsedElementScripts.RemoveAt(0);
			}
			else if (this.m_elementTemplate != null)
			{
				GameObject gameObject = base.Instantiate(this.m_elementTemplate);
				gameObject.transform.SetParent(this.m_content.transform);
				gameObject.transform.localScale = Vector3.one;
				base.InitializeComponent(gameObject);
				cUIListElementScript = gameObject.GetComponent<CUIListElementScript>();
			}
			if (cUIListElementScript != null)
			{
				cUIListElementScript.Enable(this, index, this.m_elementName, ref rect, this.IsSelectedIndex(index));
				this.m_elementScripts.Add(cUIListElementScript);
			}
			return cUIListElementScript;
		}

		protected void ProcessUnUsedElement()
		{
			if (this.m_unUsedElementScripts != null && this.m_unUsedElementScripts.Count > 0)
			{
				for (int i = 0; i < this.m_unUsedElementScripts.Count; i++)
				{
					this.m_unUsedElementScripts[i].Disable();
				}
			}
		}

		public void EnableExtraContent(bool isEnabled)
		{
			this.m_isExtraContentEnabled = isEnabled;
		}
	}
}
