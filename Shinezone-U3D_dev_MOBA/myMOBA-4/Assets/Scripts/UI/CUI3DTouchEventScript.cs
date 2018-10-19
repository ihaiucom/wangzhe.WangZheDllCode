using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
	public class CUI3DTouchEventScript : CUIMiniEventScript
	{
		[HideInInspector]
		public enUIEventID m_onTouchedEventID;

		[NonSerialized]
		public stUIEventParams m_onTouchedEventParams;

		public float m_3DTouchStrength = 4f;

		public string[] m_onTouchedWwiseEvents = new string[0];

		private bool m_isDown;

		private uint m_onTouchedEventDispatchedCount;

		private Vector3[] m_corners = new Vector3[4];

		private PointerEventData m_touchedPointerEventData;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			base.Initialize(formScript);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
		}
	}
}
