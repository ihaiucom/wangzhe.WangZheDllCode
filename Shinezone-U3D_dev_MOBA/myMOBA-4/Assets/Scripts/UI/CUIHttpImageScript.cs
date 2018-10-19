using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
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
			this.m_imageDefaultSprite = this.m_image.sprite;
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

        private IEnumerator DownloadImage(string url)
        {
            m_httpImageState = enHttpImageState.Loading;
            var startTime = Time.realtimeSinceStartup;
            var www = new WWW(url);
            yield return www;

            m_httpImageState = enHttpImageState.Loaded;
            if (!string.IsNullOrEmpty(www.error))
            {
                yield break;
            }
            if (m_loadingCover != null)
            {
                m_loadingCover.CustomSetActive(false);
            }
            string contentType = null;
            www.responseHeaders.TryGetValue("CONTENT-TYPE", out contentType);
            if (contentType != null)
            {
                contentType = contentType.ToLower();
            }
            if (string.IsNullOrEmpty(contentType) || !contentType.Contains("image/"))
            {
                yield break;
            }
            var isGif = string.Equals(contentType, "image/gif");
            Texture2D texture = null;
            if (!isGif)
            {
                texture = www.texture;
            }
            else
            {
                var memoryStream = new MemoryStream(www.bytes);
                try
                {
                    texture = GifHelper.GifToTexture(memoryStream, 0);
                }
                finally
                {
                    if (memoryStream != null)
                    {
                        memoryStream.Dispose();
                    }
                }
            }

            if (texture != null)
            {
                if (m_image != null)
                {
                    m_image.SetSprite(Sprite.Create(texture, new Rect(0f, 0f, (float)texture.width, (float)texture.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
                    if (m_setNativeSize)
                    {
                        SetNativeSize();
                    }
                }
                if (m_cacheTexture)
                {
                    CUIHttpImageScript.s_cachedTextureManager.AddCachedTexture(url, texture.width, texture.height, isGif, www.bytes);
                }
            }
        }
		
	}
}
