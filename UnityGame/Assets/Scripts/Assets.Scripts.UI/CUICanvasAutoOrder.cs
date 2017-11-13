using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class CUICanvasAutoOrder : CUIComponent
	{
		public bool bAutoOrder = true;

		public int iOrderValue;

		public override void Initialize(CUIFormScript formScript)
		{
			base.Initialize(formScript);
			Canvas component = base.GetComponent<Canvas>();
			Canvas component2 = formScript.GetComponent<Canvas>();
			if (component == null || component2 == null)
			{
				return;
			}
			component.worldCamera = component2.worldCamera;
			component.renderMode = component2.renderMode;
			if (this.bAutoOrder)
			{
				component.sortingOrder = this.m_belongedFormScript.GetSortingOrder() + this.iOrderValue;
			}
			else
			{
				component.sortingOrder = this.iOrderValue;
			}
			RectTransform rectTransform = base.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.pivot = new Vector2(0f, 1f);
				rectTransform.anchorMin = new Vector2(0f, 0f);
				rectTransform.anchorMax = new Vector2(1f, 1f);
				rectTransform.anchoredPosition = new Vector2(-5f, 5f);
				rectTransform.sizeDelta = new Vector2(10f, 10f);
				rectTransform.localRotation = Quaternion.identity;
				rectTransform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
	}
}
