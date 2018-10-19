using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class CUIHttpTextScript : CUIComponent
	{
		public string m_textUrl;

		public GameObject m_loadingCover;

		private ScrollRect m_scrollRectScript;

		private Text m_titleTextScript;

		private Text m_textScript;

		private enHttpTextState m_httpTextState;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			base.Initialize(formScript);
			this.m_scrollRectScript = CUIUtility.GetComponentInChildren<ScrollRect>(base.gameObject);
			this.m_textScript = ((!(this.m_scrollRectScript != null)) ? null : CUIUtility.GetComponentInChildren<Text>(this.m_scrollRectScript.gameObject));
			Transform transform = base.gameObject.transform.FindChild("Title");
			this.m_titleTextScript = ((!(transform != null)) ? null : CUIUtility.GetComponentInChildren<Text>(transform.gameObject));
			this.m_httpTextState = enHttpTextState.Unload;
			if (this.m_loadingCover != null)
			{
				this.m_loadingCover.CustomSetActive(true);
			}
			if (base.gameObject.activeInHierarchy && !string.IsNullOrEmpty(this.m_textUrl))
			{
				this.LoadText(this.m_textUrl);
			}
		}

		protected override void OnDestroy()
		{
			this.m_loadingCover = null;
			this.m_scrollRectScript = null;
			this.m_titleTextScript = null;
			this.m_textScript = null;
			base.OnDestroy();
		}

		private void OnEnable()
		{
			if (this.m_isInitialized && this.m_httpTextState == enHttpTextState.Unload && !string.IsNullOrEmpty(this.m_textUrl))
			{
				this.LoadText(this.m_textUrl);
			}
		}

		private void OnDisable()
		{
			if (this.m_isInitialized && this.m_httpTextState == enHttpTextState.Loading)
			{
				base.StopAllCoroutines();
				this.m_httpTextState = enHttpTextState.Unload;
				if (this.m_loadingCover != null)
				{
					this.m_loadingCover.CustomSetActive(true);
				}
			}
		}

		public void SetTextUrl(string url, bool forceReset = false)
		{
			if (string.IsNullOrEmpty(url) || (string.Equals(url, this.m_textUrl) && !forceReset))
			{
				return;
			}
			this.m_textUrl = url;
			if (this.m_titleTextScript != null)
			{
				this.m_titleTextScript.text = string.Empty;
			}
			if (this.m_textScript != null)
			{
				this.m_textScript.text = string.Empty;
			}
			if (base.gameObject.activeInHierarchy && this.m_httpTextState == enHttpTextState.Loading)
			{
				base.StopAllCoroutines();
			}
			this.m_httpTextState = enHttpTextState.Unload;
			if (this.m_loadingCover != null)
			{
				this.m_loadingCover.CustomSetActive(true);
			}
			if (base.gameObject.activeInHierarchy)
			{
				this.LoadText(this.m_textUrl);
			}
		}

		private void LoadText(string url)
		{
			if (this.m_httpTextState == enHttpTextState.Loaded)
			{
				return;
			}
			base.StartCoroutine(this.DownloadText(url));
		}

        private IEnumerator DownloadText(string url)
        {
            m_httpTextState = enHttpTextState.Loading;
            var www = new WWW(url);
            yield return www;

            m_httpTextState = enHttpTextState.Loaded;
            if (string.IsNullOrEmpty(www.error))
            {
                if (m_loadingCover != null)
                {
                    m_loadingCover.CustomSetActive(false);
                }
                var text = www.text;
                var title = string.Empty;
                var content = string.Empty;
                var hasTitle = false;
                var lineBreakPosition = text.IndexOf('\n');
                if (lineBreakPosition >= 0)
                {
                    var title2 = text.Substring(0, lineBreakPosition).Trim();
                    if (((title2 != null) && (title2.Length >= 2)) && ((title2[0] == '[') && (title2[title2.Length - 1] == ']')))
                    {
                        hasTitle = true;
                        title = title2.Substring(1, title2.Length - 2).Trim();
                        content = text.Substring(lineBreakPosition).Trim();
                    }
                }
                if (!hasTitle)
                {
                    title = string.Empty;
                    content = text;
                }
                if (m_titleTextScript != null)
                {
                    m_titleTextScript.text = title;
                }
                if (m_textScript != null)
                {
                    m_textScript.text = content;
                    m_textScript.rectTransform.anchoredPosition = new Vector2(m_textScript.rectTransform.anchoredPosition.x, 0f);
                }
            }
        }
		
	}
}
