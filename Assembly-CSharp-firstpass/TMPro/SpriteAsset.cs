using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
	public class SpriteAsset : ScriptableObject
	{
		public Texture spriteSheet;

		public Material material;

		public List<SpriteInfo> spriteInfoList;

		private List<Sprite> m_sprites;

		private void OnEnable()
		{
		}

		public void AddSprites(string path)
		{
		}

		private void OnValidate()
		{
			TMPro_EventManager.ON_SPRITE_ASSET_PROPERTY_CHANGED(true, this);
		}
	}
}
