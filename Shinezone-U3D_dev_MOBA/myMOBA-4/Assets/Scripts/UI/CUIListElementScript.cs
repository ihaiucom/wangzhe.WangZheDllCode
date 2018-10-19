using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class CUIListElementScript : CUIComponent
	{
		public delegate void OnSelectedDelegate();

		public GameObject m_selectFrontObj;

		public Sprite m_selectedSprite;

		[HideInInspector]
		public Sprite m_defaultSprite;

		[HideInInspector]
		public Color m_defaultColor;

		[HideInInspector]
		public Color m_defaultTextColor;

		public Text m_textObj;

		public Color m_selectTextColor = new Color(1f, 1f, 1f, 1f);

		[HideInInspector]
		public ImageAlphaTexLayout m_defaultLayout;

		public ImageAlphaTexLayout m_selectedLayout;

		[HideInInspector]
		public Vector2 m_defaultSize;

		[HideInInspector]
		public int m_index;

		[HideInInspector]
		public enPivotType m_pivotType = enPivotType.LeftTop;

		private Image m_image;

		public stRect m_rect;

		public bool m_useSetActiveForDisplay = true;

		public bool m_autoAddUIEventScript = true;

		[HideInInspector]
		public enUIEventID m_onEnableEventID;

		public stUIEventParams m_onEnableEventParams;

		public CUIListElementScript.OnSelectedDelegate onSelected;

		private CanvasGroup m_canvasGroup;

		private string m_dataTag;

		public Color m_orgToggleTextColor = Color.white;

		public bool m_hasSetTextColor;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			base.Initialize(formScript);
			this.m_image = base.gameObject.GetComponent<Image>();
			if (this.m_image != null)
			{
				this.m_defaultSprite = this.m_image.sprite;
				this.m_defaultColor = this.m_image.color;
				if (this.m_image is Image2)
				{
					Image2 image = this.m_image as Image2;
					this.m_defaultLayout = image.alphaTexLayout;
				}
			}
			if (this.m_autoAddUIEventScript)
			{
				CUIEventScript cUIEventScript = base.gameObject.GetComponent<CUIEventScript>();
				if (cUIEventScript == null)
				{
					cUIEventScript = base.gameObject.AddComponent<CUIEventScript>();
					cUIEventScript.Initialize(formScript);
				}
			}
			if (!this.m_useSetActiveForDisplay)
			{
				this.m_canvasGroup = base.gameObject.GetComponent<CanvasGroup>();
				if (this.m_canvasGroup == null)
				{
					this.m_canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
				}
			}
			this.m_defaultSize = this.GetDefaultSize();
			this.InitRectTransform();
			if (this.m_textObj != null)
			{
				this.m_defaultTextColor = this.m_textObj.color;
			}
		}

		protected override void OnDestroy()
		{
			this.m_selectFrontObj = null;
			this.m_selectedSprite = null;
			this.m_defaultSprite = null;
			this.m_textObj = null;
			this.m_image = null;
			this.onSelected = null;
			this.m_canvasGroup = null;
			base.OnDestroy();
		}

		protected virtual Vector2 GetDefaultSize()
		{
			return new Vector2(((RectTransform)base.gameObject.transform).rect.width, ((RectTransform)base.gameObject.transform).rect.height);
		}

		public void SetDataTag(string dataTag)
		{
			this.m_dataTag = dataTag;
		}

		public string GetDataTag()
		{
			return this.m_dataTag;
		}

		public void Enable(CUIListScript belongedList, int index, string name, ref stRect rect, bool selected)
		{
			this.m_belongedListScript = belongedList;
			this.m_index = index;
			base.gameObject.name = name + "_" + index.ToString();
			if (this.m_useSetActiveForDisplay)
			{
				base.gameObject.CustomSetActive(true);
			}
			else
			{
				this.m_canvasGroup.alpha = 1f;
				this.m_canvasGroup.blocksRaycasts = true;
			}
			this.SetComponentBelongedList(base.gameObject);
			this.SetRect(ref rect);
			this.ChangeDisplay(selected);
			this.DispatchOnEnableEvent();
		}

		public void Disable()
		{
			if (this.m_useSetActiveForDisplay)
			{
				base.gameObject.CustomSetActive(false);
			}
			else
			{
				this.m_canvasGroup.alpha = 0f;
				this.m_canvasGroup.blocksRaycasts = false;
			}
		}

		public void OnSelected(BaseEventData baseEventData)
		{
			this.m_belongedListScript.SelectElement(this.m_index, true);
		}

		public virtual void ChangeDisplay(bool selected)
		{
			if (this.m_image != null && this.m_selectedSprite != null)
			{
				if (selected)
				{
					this.m_image.sprite = this.m_selectedSprite;
					this.m_image.color = new Color(this.m_defaultColor.r, this.m_defaultColor.g, this.m_defaultColor.b, 255f);
				}
				else
				{
					this.m_image.sprite = this.m_defaultSprite;
					this.m_image.color = this.m_defaultColor;
				}
				if (this.m_image is Image2)
				{
					Image2 image = this.m_image as Image2;
					image.alphaTexLayout = ((!selected) ? this.m_defaultLayout : this.m_selectedLayout);
				}
			}
			if (this.m_selectFrontObj != null)
			{
				this.m_selectFrontObj.CustomSetActive(selected);
			}
			if (this.m_textObj != null)
			{
				this.m_textObj.color = ((!selected) ? this.m_defaultTextColor : this.m_selectTextColor);
			}
		}

		public void SetComponentBelongedList(GameObject gameObject)
		{
			CUIComponent[] components = gameObject.GetComponents<CUIComponent>();
			if (components != null && components.Length > 0)
			{
				for (int i = 0; i < components.Length; i++)
				{
					components[i].SetBelongedList(this.m_belongedListScript, this.m_index);
				}
			}
			for (int j = 0; j < gameObject.transform.childCount; j++)
			{
				this.SetComponentBelongedList(gameObject.transform.GetChild(j).gameObject);
			}
		}

		public void SetRect(ref stRect rect)
		{
			this.m_rect = rect;
			RectTransform rectTransform = base.gameObject.transform as RectTransform;
			rectTransform.sizeDelta = new Vector2((float)this.m_rect.m_width, (float)this.m_rect.m_height);
			if (this.m_pivotType == enPivotType.Centre)
			{
				rectTransform.anchoredPosition = rect.m_center;
			}
			else
			{
				rectTransform.anchoredPosition = new Vector2((float)rect.m_left, (float)rect.m_top);
			}
		}

		public void SetOnEnableEvent(enUIEventID eventID)
		{
			this.m_onEnableEventID = eventID;
		}

		public void SetOnEnableEvent(enUIEventID eventID, stUIEventParams eventParams)
		{
			this.m_onEnableEventID = eventID;
			this.m_onEnableEventParams = eventParams;
		}

		private void InitRectTransform()
		{
			RectTransform rectTransform = base.gameObject.transform as RectTransform;
			rectTransform.anchorMin = new Vector2(0f, 1f);
			rectTransform.anchorMax = new Vector2(0f, 1f);
			rectTransform.pivot = ((this.m_pivotType != enPivotType.Centre) ? new Vector2(0f, 1f) : new Vector2(0.5f, 0.5f));
			rectTransform.sizeDelta = this.m_defaultSize;
			rectTransform.localRotation = Quaternion.identity;
			rectTransform.localScale = new Vector3(1f, 1f, 1f);
		}

		protected void DispatchOnEnableEvent()
		{
			if (this.m_onEnableEventID != enUIEventID.None)
			{
				CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
				uIEvent.m_eventID = this.m_onEnableEventID;
				uIEvent.m_eventParams = this.m_onEnableEventParams;
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
