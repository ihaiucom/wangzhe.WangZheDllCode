using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class CUICanvasScript : CUIComponent
	{
		private Canvas m_Canvas;

		public bool m_isNeedMaskParticle;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			this.m_Canvas = base.GetComponent<Canvas>();
			base.Initialize(formScript);
		}

		public override void Hide()
		{
			base.Hide();
			CUIUtility.SetGameObjectLayer(base.gameObject, 31);
		}

		public override void Appear()
		{
			base.Appear();
			CUIUtility.SetGameObjectLayer(base.gameObject, 5);
		}

		public override void SetSortingOrder(int sortingOrder)
		{
			if (this.m_Canvas != null && this.m_isNeedMaskParticle)
			{
				this.m_Canvas.overrideSorting = true;
				this.m_Canvas.sortingOrder = sortingOrder + 1;
			}
		}
	}
}
