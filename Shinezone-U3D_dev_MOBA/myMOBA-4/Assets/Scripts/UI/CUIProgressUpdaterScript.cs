using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class CUIProgressUpdaterScript : CUIComponent
	{
		public enum enFillDirection
		{
			Clockwise,
			CounterClockwise
		}

		public CUIProgressUpdaterScript.enFillDirection m_fillDirection;

		public float m_fillAmountPerSecond = 1f;

		[HideInInspector]
		public enUIEventID m_fillEndEventID;

		[HideInInspector]
		private float m_targetFillAmount;

		private float m_protectFillAmout;

		private Image m_image;

		private bool m_isRunning;

		[Range(0f, 1f)]
		public float m_startFillAmount;

		[Range(0f, 1f)]
		public float m_endFillAmount = 1f;

		private float m_fillRate;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			this.m_image = base.gameObject.GetComponent<Image>();
			if (this.m_image == null)
			{
				return;
			}
			if (this.m_image.type != Image.Type.Filled)
			{
				return;
			}
			if (this.m_startFillAmount >= this.m_endFillAmount)
			{
			}
			this.m_fillRate = (this.m_endFillAmount - this.m_startFillAmount) / 1f;
			this.m_isRunning = false;
			base.Initialize(formScript);
		}

		protected override void OnDestroy()
		{
			this.m_image = null;
			base.OnDestroy();
		}

		protected virtual void Update()
		{
			if (!this.m_isRunning || this.m_image == null || this.m_image.type != Image.Type.Filled)
			{
				return;
			}
			if (this.m_fillDirection == CUIProgressUpdaterScript.enFillDirection.Clockwise)
			{
				float fillAmount = this.m_image.fillAmount + this.m_fillAmountPerSecond * this.m_fillRate * Time.deltaTime;
				this.m_image.fillAmount = fillAmount;
				if (this.m_image.fillAmount >= this.m_protectFillAmout)
				{
					this.m_image.color = CUIUtility.s_Color_BraveScore_BaojiKedu_On;
				}
				else
				{
					this.m_image.color = CUIUtility.s_Color_BraveScore_BaojiKedu_Off;
				}
				if (this.m_image.fillAmount >= this.m_targetFillAmount)
				{
					this.m_isRunning = false;
					this.DispatchFillEndEvent();
				}
			}
			else if (this.m_fillDirection == CUIProgressUpdaterScript.enFillDirection.CounterClockwise)
			{
				float fillAmount2 = this.m_image.fillAmount - this.m_fillAmountPerSecond * this.m_fillRate * Time.deltaTime;
				this.m_image.fillAmount = fillAmount2;
				if (this.m_image.fillAmount >= this.m_protectFillAmout)
				{
					this.m_image.color = CUIUtility.s_Color_BraveScore_BaojiKedu_On;
				}
				else
				{
					this.m_image.color = CUIUtility.s_Color_BraveScore_BaojiKedu_Off;
				}
				if (this.m_image.fillAmount <= this.m_targetFillAmount)
				{
					this.m_isRunning = false;
					this.DispatchFillEndEvent();
				}
			}
		}

		private void DispatchFillEndEvent()
		{
			if (this.m_fillEndEventID != enUIEventID.None)
			{
				CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
				uIEvent.m_srcFormScript = this.m_belongedFormScript;
				uIEvent.m_srcWidget = base.gameObject;
				uIEvent.m_srcWidgetScript = this;
				uIEvent.m_srcWidgetBelongedListScript = this.m_belongedListScript;
				uIEvent.m_srcWidgetIndexInBelongedList = this.m_indexInlist;
				uIEvent.m_pointerEventData = null;
				uIEvent.m_eventID = this.m_fillEndEventID;
				uIEvent.m_eventParams = default(stUIEventParams);
				base.DispatchUIEvent(uIEvent);
			}
		}

		public void StartFill(float targetFillAmount, float protectFillAmout, CUIProgressUpdaterScript.enFillDirection fillDirection = CUIProgressUpdaterScript.enFillDirection.Clockwise, float curFillAmount = -1f)
		{
			this.m_targetFillAmount = Mathf.Clamp(targetFillAmount, this.m_startFillAmount, this.m_endFillAmount);
			this.m_fillDirection = fillDirection;
			this.m_protectFillAmout = protectFillAmout;
			if (curFillAmount >= 0f)
			{
				this.m_image.fillAmount = curFillAmount;
				if (this.m_image.fillAmount >= this.m_protectFillAmout)
				{
					this.m_image.color = CUIUtility.s_Color_BraveScore_BaojiKedu_On;
				}
				else
				{
					this.m_image.color = CUIUtility.s_Color_BraveScore_BaojiKedu_Off;
				}
			}
			this.m_isRunning = true;
		}

		public void ResetFillAmount()
		{
			if (this.m_image != null)
			{
				this.m_image.fillAmount = this.m_startFillAmount;
			}
		}
	}
}
