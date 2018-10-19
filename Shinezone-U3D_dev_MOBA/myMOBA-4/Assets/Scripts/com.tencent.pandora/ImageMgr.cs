using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.tencent.pandora
{
	public class ImageMgr : MonoBehaviour
	{
		private Dictionary<string, Texture2D> dictPictures = new Dictionary<string, Texture2D>();

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void OnDestroy()
		{
			Logger.DEBUG(string.Empty);
			this.dictPictures.Clear();
		}

		public Texture2D GetTexture(string url)
		{
			Logger.DEBUG(string.Empty);
			if (this.dictPictures.ContainsKey(url))
			{
				Logger.DEBUG(string.Empty);
				return this.dictPictures[url];
			}
			return null;
		}

		public void AddTexture(string url, Texture2D texture)
		{
			Logger.DEBUG(string.Empty);
			this.dictPictures[url] = texture;
		}
	}
}
