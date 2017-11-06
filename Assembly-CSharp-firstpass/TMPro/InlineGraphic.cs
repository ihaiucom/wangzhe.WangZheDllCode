using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMPro
{
	public class InlineGraphic : MaskableGraphic
	{
		public Texture texture;

		private InlineGraphicManager m_manager;

		public override Texture mainTexture
		{
			get
			{
				if (this.texture == null)
				{
					return Graphic.s_WhiteTexture;
				}
				return this.texture;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_manager = base.GetComponentInParent<InlineGraphicManager>();
			if (this.m_manager != null && this.m_manager.spriteAsset != null)
			{
				this.texture = this.m_manager.spriteAsset.spriteSheet;
			}
		}

		public void UpdateMaterial()
		{
			base.UpdateMaterial();
		}

		protected override void UpdateGeometry()
		{
		}

		protected override void OnFillVBO(List<UIVertex> vbo)
		{
			base.OnFillVBO(vbo);
		}
	}
}
