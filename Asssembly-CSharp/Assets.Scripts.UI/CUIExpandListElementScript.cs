using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class CUIExpandListElementScript : CUIListElementScript
	{
		public Vector2 m_retractedSize = new Vector2(-1f, -1f);

		[HideInInspector]
		public Vector2 m_expandedSize;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			this.m_expandedSize = this.GetExpandedSize();
			base.Initialize(formScript);
		}

		protected override Vector2 GetDefaultSize()
		{
			if (this.m_retractedSize.x <= 0f)
			{
				this.m_retractedSize.x = ((RectTransform)base.gameObject.transform).rect.width;
			}
			if (this.m_retractedSize.y <= 0f)
			{
				this.m_retractedSize.y = ((RectTransform)base.gameObject.transform).rect.height;
			}
			return this.m_retractedSize;
		}

		protected Vector2 GetExpandedSize()
		{
			return new Vector2((base.gameObject.transform as RectTransform).rect.width, (base.gameObject.transform as RectTransform).rect.height);
		}

		public override void ChangeDisplay(bool selected)
		{
		}
	}
}
