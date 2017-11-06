using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class CUIHttpImageScript : CUIComponent
	{
		public string m_imageUrl;

		public bool m_setNativeSize;

		public bool m_cacheTexture = true;

		public float m_cachedTextureValidDays = 2f;

		public bool m_forceSetImageUrl;

		public GameObject m_loadingCover;

		private Image m_image;

		private ImageAlphaTexLayout m_imageDefaultAlphaTexLayout;

		private Sprite m_imageDefaultSprite;

		private enHttpImageState m_httpImageState;

		private static CCachedTextureManager s_cachedTextureManager;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			base.Initialize(formScript);
			this.m_image = base.gameObject.GetComponent<Image>();
			this.m_imageDefaultSprite = this.m_image.get_sprite();
			if (this.m_image is Image2)
			{
				this.m_imageDefaultAlphaTexLayout = (this.m_image as Image2).alphaTexLayout;
			}
			if (this.m_cacheTexture && CUIHttpImageScript.s_cachedTextureManager == null)
			{
				CUIHttpImageScript.s_cachedTextureManager = new CCachedTextureManager();
			}
			this.m_httpImageState = enHttpImageState.Unload;
			if (this.m_loadingCover != null)
			{
				this.m_loadingCover.CustomSetActive(true);
			}
			if (base.gameObject.activeInHierarchy && !string.IsNullOrEmpty(this.m_imageUrl))
			{
				this.LoadTexture(this.m_imageUrl);
			}
		}

		protected override void OnDestroy()
		{
			this.m_loadingCover = null;
			this.m_image = null;
			this.m_imageDefaultSprite = null;
			base.OnDestroy();
		}

		private void OnEnable()
		{
			if (this.m_isInitialized && this.m_httpImageState == enHttpImageState.Unload && !string.IsNullOrEmpty(this.m_imageUrl))
			{
				this.LoadTexture(this.m_imageUrl);
			}
		}

		private void OnDisable()
		{
			if (this.m_isInitialized && this.m_httpImageState == enHttpImageState.Loading)
			{
				base.StopAllCoroutines();
				this.m_httpImageState = enHttpImageState.Unload;
				if (this.m_loadingCover != null)
				{
					this.m_loadingCover.CustomSetActive(true);
				}
			}
		}

		public void SetImageUrl(string url)
		{
			if (!this.m_forceSetImageUrl && string.Equals(url, this.m_imageUrl))
			{
				return;
			}
			this.m_imageUrl = url;
			if (this.m_image != null)
			{
				this.m_image.SetSprite(this.m_imageDefaultSprite, this.m_imageDefaultAlphaTexLayout);
			}
			if (base.gameObject.activeInHierarchy && this.m_httpImageState == enHttpImageState.Loading)
			{
				base.StopAllCoroutines();
			}
			this.m_httpImageState = enHttpImageState.Unload;
			if (this.m_loadingCover != null)
			{
				this.m_loadingCover.CustomSetActive(true);
			}
			if (base.gameObject.activeInHierarchy && !string.IsNullOrEmpty(this.m_imageUrl))
			{
				this.LoadTexture(this.m_imageUrl);
			}
		}

		public void SetImageSprite(string prefabPath, CUIFormScript formScript)
		{
			if (this.m_image != null)
			{
				this.m_image.SetSprite(prefabPath, formScript, true, false, false, false);
			}
		}

		public void SetNativeSize()
		{
			if (this.m_image != null)
			{
				this.m_image.SetNativeSize();
			}
		}

		private void LoadTexture(string url)
		{
			if (this.m_httpImageState == enHttpImageState.Loaded)
			{
				return;
			}
			if (this.m_cacheTexture)
			{
				Texture2D cachedTexture = CUIHttpImageScript.s_cachedTextureManager.GetCachedTexture(url, this.m_cachedTextureValidDays);
				if (cachedTexture != null)
				{
					if (this.m_image != null)
					{
						this.m_image.SetSprite(Sprite.Create(cachedTexture, new Rect(0f, 0f, (float)cachedTexture.width, (float)cachedTexture.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
						if (this.m_setNativeSize)
						{
							this.SetNativeSize();
						}
						this.m_httpImageState = enHttpImageState.Loaded;
						if (this.m_loadingCover != null)
						{
							this.m_loadingCover.CustomSetActive(false);
						}
					}
				}
				else
				{
					base.StartCoroutine(this.DownloadImage(url));
				}
			}
			else
			{
				base.StartCoroutine(this.DownloadImage(url));
			}
		}

		[DebuggerHidden]
		private IEnumerator DownloadImage(string url)
		{
			CUIHttpImageScript.<DownloadImage>c__Iterator31 <DownloadImage>c__Iterator = new CUIHttpImageScript.<DownloadImage>c__Iterator31();
			<DownloadImage>c__Iterator.url = url;
			<DownloadImage>c__Iterator.<$>url = url;
			<DownloadImage>c__Iterator.<>f__this = this;
			return <DownloadImage>c__Iterator;
		}
	}
}
