using Assets.Scripts.Sound;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class CUISliderEventScript : CUIComponent
	{
		[HideInInspector]
		public bool m_sliderEventEnable = true;

		[HideInInspector]
		public enUIEventID m_onValueChangedEventID;

		public string[] m_onValueChangedWwiseEvents = new string[0];

		private Slider m_slider;

		private int m_DescribeCount;

		private Text[] m_Describes;

		private Text m_Handletext;

		private float m_value;

		public float value
		{
			get
			{
				if (this.m_slider)
				{
					return this.m_slider.value;
				}
				return -1f;
			}
			set
			{
				this.m_value = value;
				if (this.m_slider && this.m_value <= this.m_slider.maxValue && this.m_value >= 0f)
				{
					this.m_slider.value = this.m_value;
					this.m_Handletext.text = this.m_Describes[(int)this.m_value].text;
				}
			}
		}

		public int MaxValue
		{
			get
			{
				if (this.m_slider)
				{
					return (int)this.m_slider.maxValue;
				}
				return 0;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.m_slider && this.m_slider.interactable;
			}
			set
			{
				if (this.m_slider)
				{
					this.m_slider.interactable = value;
				}
			}
		}

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			this.m_slider = base.gameObject.GetComponent<Slider>();
			if (this.m_slider == null)
			{
				return;
			}
			this.m_DescribeCount = base.transform.Find("Background").childCount;
			this.m_Describes = new Text[this.m_DescribeCount];
			for (int i = 0; i < this.m_DescribeCount; i++)
			{
				this.m_Describes[i] = base.transform.Find(string.Format("Background/Text{0}", i + 1)).GetComponent<Text>();
			}
			this.m_Handletext = base.transform.Find("Handle Slide Area/Handle/Text").GetComponent<Text>();
			this.m_slider.onValueChanged.RemoveAllListeners();
			this.m_slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderValueChanged));
			base.Initialize(formScript);
		}

		protected override void OnDestroy()
		{
			this.m_slider = null;
			this.m_Describes = null;
			this.m_Handletext = null;
			base.OnDestroy();
		}

		private void OnSliderValueChanged(float value)
		{
			if (!this.m_sliderEventEnable || value < 0f || value >= (float)this.m_DescribeCount)
			{
				return;
			}
			this.value = value;
			this.PostWwiseEvent(this.m_onValueChangedWwiseEvents);
			this.DispatchSliderEvent();
		}

		private void DispatchSliderEvent()
		{
			if (this.m_onValueChangedEventID == enUIEventID.None)
			{
				return;
			}
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_srcFormScript = this.m_belongedFormScript;
			uIEvent.m_srcWidget = base.gameObject;
			uIEvent.m_srcWidgetScript = this;
			uIEvent.m_srcWidgetBelongedListScript = this.m_belongedListScript;
			uIEvent.m_srcWidgetIndexInBelongedList = this.m_indexInlist;
			uIEvent.m_pointerEventData = null;
			uIEvent.m_eventID = this.m_onValueChangedEventID;
			uIEvent.m_eventParams.sliderValue = this.value;
			base.DispatchUIEvent(uIEvent);
		}

		private void PostWwiseEvent(string[] wwiseEvents)
		{
			for (int i = 0; i < wwiseEvents.Length; i++)
			{
				if (!string.IsNullOrEmpty(wwiseEvents[i]))
				{
					Singleton<CSoundManager>.GetInstance().PostEvent(wwiseEvents[i], null);
				}
			}
		}

		public Slider GetSlider()
		{
			return this.m_slider;
		}
	}
}
