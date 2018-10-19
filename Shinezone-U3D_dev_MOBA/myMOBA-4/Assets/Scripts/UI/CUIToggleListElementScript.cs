using System;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class CUIToggleListElementScript : CUIListElementScript
	{
		private Toggle m_toggle;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			base.Initialize(formScript);
			this.m_toggle = base.GetComponentInChildren<Toggle>(base.gameObject);
			if (this.m_toggle != null)
			{
				this.m_toggle.interactable = false;
			}
		}

		protected override void OnDestroy()
		{
			this.m_toggle = null;
			base.OnDestroy();
		}

		public override void ChangeDisplay(bool selected)
		{
			base.ChangeDisplay(selected);
			if (this.m_toggle != null)
			{
				this.m_toggle.isOn = selected;
			}
		}
	}
}
