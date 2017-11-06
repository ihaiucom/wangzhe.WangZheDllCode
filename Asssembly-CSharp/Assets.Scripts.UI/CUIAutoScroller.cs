using System;
using System.Collections;
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
			component.set_text(contentText);
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

		[DebuggerHidden]
		private IEnumerator UpdateScroll()
		{
			CUIAutoScroller.<UpdateScroll>c__Iterator30 <UpdateScroll>c__Iterator = new CUIAutoScroller.<UpdateScroll>c__Iterator30();
			<UpdateScroll>c__Iterator.<>f__this = this;
			return <UpdateScroll>c__Iterator;
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
