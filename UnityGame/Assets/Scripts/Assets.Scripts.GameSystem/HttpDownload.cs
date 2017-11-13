using Assets.Scripts.UI;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class HttpDownload : MonoSingleton<HttpDownload>
	{
		public delegate void DownloadHandler(string error, byte[] data);

		private const int K_TRY_MAXCOUNT = 5;

		private WWW m_www;

		private string prefabFile = "UGUI/Form/Common/Form_ProgressBar.prefab";

		private HttpDownload.DownloadHandler m_resultHandler;

		private CUIFormScript m_progressForm;

		private Image m_progressImage;

		private Text m_progressText;

		private CUITimerScript m_timer;

		private float m_lastBytesUpdateStamp;

		private float m_lastProgress;

		private string m_url;

		private bool m_isMyTimeout;

		private int m_tryTimes;

		private void Update()
		{
			if (this.m_progressImage != null && this.m_www != null)
			{
				this.m_progressImage.set_fillAmount(this.m_www.progress);
				if (Math.Abs(this.m_www.progress - this.m_lastProgress) < 0.01f)
				{
					if (Time.time - this.m_lastBytesUpdateStamp > 5f)
					{
						base.StopAllCoroutines();
						if (this.m_tryTimes <= 5)
						{
							this.m_www = null;
							this.m_tryTimes++;
							this.m_isMyTimeout = true;
							base.StartCoroutine(this.ProcessFile(this.m_url));
							this.m_lastProgress = 0f;
							this.m_lastBytesUpdateStamp = Time.time;
						}
					}
				}
				else
				{
					if (this.m_progressText != null)
					{
						this.m_progressText.set_text(Singleton<CTextManager>.GetInstance().GetText("Downloading", new string[]
						{
							((int)(this.m_www.progress * 100f)).ToString()
						}));
					}
					this.m_lastProgress = this.m_www.progress;
					this.m_lastBytesUpdateStamp = Time.time;
				}
			}
		}

		public bool Download(string url, HttpDownload.DownloadHandler resultHandler, string progressTips = null)
		{
			if (string.IsNullOrEmpty(url))
			{
				return false;
			}
			if (this.m_www != null && !this.m_www.isDone)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Download_lastDownNotOver", true, 1.5f, null, new object[0]);
				return false;
			}
			this.m_isMyTimeout = false;
			this.m_tryTimes = 0;
			this.m_url = url;
			this.m_www = new WWW(url);
			this.m_resultHandler = resultHandler;
			this.m_lastProgress = 0f;
			this.m_lastBytesUpdateStamp = Time.time;
			this.m_progressForm = Singleton<CUIManager>.GetInstance().OpenForm(this.prefabFile, true, true);
			if (this.m_progressForm != null)
			{
				this.m_progressImage = null;
				this.m_progressText = null;
				GameObject widget = this.m_progressForm.GetWidget(0);
				if (widget != null)
				{
					this.m_progressImage = widget.GetComponent<Image>();
				}
				widget = this.m_progressForm.GetWidget(1);
				if (widget != null)
				{
					this.m_progressText = widget.GetComponent<Text>();
				}
				if (this.m_progressText != null)
				{
					this.m_progressText.set_text(Singleton<CTextManager>.GetInstance().GetText("Downloading", new string[]
					{
						"0"
					}));
				}
				this.m_progressImage.set_fillAmount(0f);
				if (progressTips != null)
				{
					widget = this.m_progressForm.GetWidget(3);
					if (widget != null)
					{
						Text component = widget.GetComponent<Text>();
						component.set_text(progressTips);
					}
				}
			}
			base.StartCoroutine(this.ProcessFile(url));
			return true;
		}

		[DebuggerHidden]
		private IEnumerator ProcessFile(string url)
		{
			HttpDownload.<ProcessFile>c__Iterator27 <ProcessFile>c__Iterator = new HttpDownload.<ProcessFile>c__Iterator27();
			<ProcessFile>c__Iterator.url = url;
			<ProcessFile>c__Iterator.<$>url = url;
			<ProcessFile>c__Iterator.<>f__this = this;
			return <ProcessFile>c__Iterator;
		}
	}
}
