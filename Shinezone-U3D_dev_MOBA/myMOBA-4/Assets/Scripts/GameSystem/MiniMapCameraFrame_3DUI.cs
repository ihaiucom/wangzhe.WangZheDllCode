using Assets.Scripts.Framework;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class MiniMapCameraFrame_3DUI
	{
		private GameObject m_node;

		private Image m_frameImgNormal;

		private Image m_frameImgRed;

		private float m_minimapHalfWidth;

		private float m_minimapHalfHeight;

		private float m_SFrameWidth;

		private float m_SFrameHeight;

		private float m_BFrameWidth;

		private float m_BFrameHeight;

		private float curFrameHalfWidth;

		private float curFrameHalfHeight;

		private bool m_bFrameShowed;

		private ulong m_startCooldownTimestamp;

		public bool IsCameraFrameShow
		{
			get
			{
				return this.m_bFrameShowed;
			}
		}

		public MiniMapCameraFrame_3DUI(GameObject node, float minimapWidth, float minimapHeight)
		{
			this.m_node = node;
			this.m_frameImgNormal = this.m_node.transform.Find("normal").GetComponent<Image>();
			this.m_frameImgRed = this.m_node.transform.Find("red").GetComponent<Image>();
			this.m_minimapHalfWidth = minimapWidth * 0.5f;
			this.m_minimapHalfHeight = minimapHeight * 0.5f;
			this.Init();
			this.Hide();
		}

		public void Init()
		{
			this.m_SFrameWidth = 50f;
			this.m_SFrameHeight = 28f;
			this.m_BFrameWidth = 65f;
			this.m_BFrameHeight = 36f;
		}

		public void Clear()
		{
			this.m_node = null;
			this.m_frameImgNormal = null;
			this.m_frameImgRed = null;
			this.m_startCooldownTimestamp = 0uL;
			this.m_bFrameShowed = false;
		}

		public void Update()
		{
			if (this.m_bFrameShowed && this.m_startCooldownTimestamp > 0uL)
			{
				uint num = (uint)(Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.m_startCooldownTimestamp);
				if (num >= 1500u)
				{
					this.ShowNormal();
				}
			}
		}

		public void Show()
		{
			if (this.m_node != null)
			{
				this.m_node.CustomSetActive(true);
			}
			this.m_bFrameShowed = true;
		}

		public void Hide()
		{
			if (this.m_node != null)
			{
				this.m_node.CustomSetActive(false);
			}
			this.m_bFrameShowed = false;
		}

		public void ShowNormal()
		{
			if (this.m_frameImgNormal != null)
			{
				this.m_frameImgNormal.gameObject.CustomSetActive(true);
			}
			if (this.m_frameImgRed != null)
			{
				this.m_frameImgRed.gameObject.CustomSetActive(false);
			}
			this.m_startCooldownTimestamp = 0uL;
		}

		public void ShowRed()
		{
			if (this.m_frameImgNormal != null)
			{
				this.m_frameImgNormal.gameObject.CustomSetActive(false);
			}
			if (this.m_frameImgRed != null)
			{
				this.m_frameImgRed.gameObject.CustomSetActive(true);
			}
			this.m_startCooldownTimestamp = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
		}

		public void SetPos(float x, float y)
		{
			if (this.m_frameImgNormal == null || this.m_frameImgRed == null)
			{
				return;
			}
			float x2 = Mathf.Clamp(x, -(this.m_minimapHalfWidth - this.curFrameHalfWidth), this.m_minimapHalfWidth - this.curFrameHalfWidth);
			float y2 = Mathf.Clamp(y, -(this.m_minimapHalfHeight - this.curFrameHalfHeight), this.m_minimapHalfHeight - this.curFrameHalfHeight);
			RectTransform component = this.m_frameImgNormal.GetComponent<RectTransform>();
			RectTransform component2 = this.m_frameImgRed.GetComponent<RectTransform>();
			if (component2 == null || component == null)
			{
				return;
			}
			component.anchoredPosition = new Vector2(x2, y2);
			component2.anchoredPosition = new Vector2(x2, y2);
		}

		public void SetFrameSize(CameraHeightType type)
		{
			if (this.m_frameImgNormal == null || this.m_frameImgRed == null)
			{
				return;
			}
			bool flag = type == CameraHeightType.Low;
			RectTransform component = this.m_frameImgNormal.GetComponent<RectTransform>();
			if (component == null)
			{
				return;
			}
			RectTransform component2 = this.m_frameImgRed.GetComponent<RectTransform>();
			if (component2 == null)
			{
				return;
			}
			if (flag)
			{
				component.sizeDelta = new Vector2(this.m_SFrameWidth, this.m_SFrameHeight);
				component2.sizeDelta = new Vector2(this.m_SFrameWidth, this.m_SFrameHeight);
				this.curFrameHalfWidth = this.m_SFrameWidth * 0.5f;
				this.curFrameHalfHeight = this.m_SFrameHeight * 0.5f;
			}
			else
			{
				component.sizeDelta = new Vector2(this.m_BFrameWidth, this.m_BFrameHeight);
				component2.sizeDelta = new Vector2(this.m_BFrameWidth, this.m_BFrameHeight);
				this.curFrameHalfWidth = this.m_BFrameWidth * 0.5f;
				this.curFrameHalfHeight = this.m_BFrameHeight * 0.5f;
			}
		}
	}
}
