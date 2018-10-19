using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class CUIAutoScroller : CUIComponent
	{
		private const int TargetFrameRate = 30;

		public GameObject m_content;

		public int m_scrollSpeed = 1;

		public int m_loop = 1;

		private int m_loopCnt;

		private RectTransform m_contentRectTransform;

		private bool m_isScrollRunning;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			if (this.m_content != null)
			{
				this.m_contentRectTransform = (this.m_content.transform as RectTransform);
			}
			base.Initialize(formScript);
		}

		protected override void OnDestroy()
		{
			this.m_content = null;
			this.m_contentRectTransform = null;
			base.OnDestroy();
		}

		public void StartAutoScroll(bool bForce = false)
		{
			if (!bForce && this.m_isScrollRunning)
			{
				return;
			}
			this.m_loopCnt = this.m_loop;
			this.m_isScrollRunning = true;
			this.ResetContentTransform();
			base.StartCoroutine("UpdateScroll");
		}

		public void StopAutoScroll()
		{
			if (!this.m_isScrollRunning)
			{
				return;
			}
			this.m_isScrollRunning = false;
			base.StopCoroutine("UpdateScroll");
			this.ResetContentTransform();
		}

		public bool IsScrollRunning()
		{
			return this.m_isScrollRunning;
		}

		public void SetText(string contentText)
		{
			if (this.m_content == null)
			{
				return;
			}
			Text component = this.m_content.GetComponent<Text>();
			if (component == null)
			{
				return;
			}
			component.text = contentText;
		}

		private void ResetContentTransform()
		{
			if (this.m_contentRectTransform == null)
			{
				return;
			}
			this.m_contentRectTransform.pivot = new Vector2(0f, 0.5f);
			this.m_contentRectTransform.anchorMin = new Vector2(0f, 0.5f);
			this.m_contentRectTransform.anchorMax = new Vector2(0f, 0.5f);
			this.m_contentRectTransform.anchoredPosition = new Vector2((base.transform as RectTransform).rect.width, 0f);
		}

        private IEnumerator UpdateScroll()
        {
            if (m_contentRectTransform == null)
            {
                yield break;
            }

            while (m_contentRectTransform.anchoredPosition.x > -m_contentRectTransform.rect.width)
            {
                m_contentRectTransform.anchoredPosition = new Vector2(m_contentRectTransform.anchoredPosition.x - ((Time.deltaTime * 30f) * m_scrollSpeed), m_contentRectTransform.anchoredPosition.y);
                if (m_contentRectTransform.anchoredPosition.x <= -m_contentRectTransform.rect.width)
                {
                    if (m_loopCnt > 0)
                    {
                        m_loopCnt--;
                    }
                    if (m_loopCnt != 0)
                    {
                        ResetContentTransform();
                    }
                }
                yield return null;
            }
            m_isScrollRunning = false;
            DispatchScrollFinishEvent();
        }

		private void DispatchScrollFinishEvent()
		{
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_eventID = enUIEventID.UIComponent_AutoScroller_Scroll_Finish;
			uIEvent.m_srcFormScript = this.m_belongedFormScript;
			uIEvent.m_srcWidget = base.gameObject;
			uIEvent.m_srcWidgetScript = this;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
		}
	}
}
