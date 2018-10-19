using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class CUITimerScript : CUIComponent
	{
		public enTimerType m_timerType;

		public enTimerDisplayType m_timerDisplayType;

		public double m_totalTime;

		private double m_currentTime;

		public double m_onChangedIntervalTime = 1.0;

		private double m_lastOnChangedTime;

		private bool m_isRunning;

		private bool m_isPaused;

		private double m_startTime;

		private double m_pauseTime;

		private double m_pauseElastTime;

		public bool m_runImmediately;

		public bool m_closeBelongedFormWhenTimeup;

		public bool m_pausedWhenAppPaused = true;

		[HideInInspector]
		public enUIEventID[] m_eventIDs = new enUIEventID[Enum.GetValues(typeof(enTimerEventType)).Length];

		public stUIEventParams[] m_eventParams = new stUIEventParams[Enum.GetValues(typeof(enTimerEventType)).Length];

		private Text m_timerText;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			base.Initialize(formScript);
			if (this.m_runImmediately)
			{
				this.StartTimer();
			}
			this.m_timerText = base.GetComponentInChildren<Text>(base.gameObject);
			if (this.m_timerDisplayType == enTimerDisplayType.None && this.m_timerText != null)
			{
				this.m_timerText.gameObject.CustomSetActive(false);
			}
			this.RefreshTimeDisplay();
		}

		protected override void OnDestroy()
		{
			this.m_timerText = null;
			base.OnDestroy();
		}

		public override void Close()
		{
			base.Close();
			this.ResetTime();
		}

		private void Update()
		{
			if (this.m_belongedFormScript != null && this.m_belongedFormScript.IsClosed())
			{
				return;
			}
			this.UpdateTimer();
		}

		public void SetTotalTime(float time)
		{
			this.m_totalTime = (double)time;
			this.RefreshTimeDisplay();
		}

		public void SetTimerEventId(enTimerEventType eventType, enUIEventID eventId)
		{
			if (eventType >= enTimerEventType.TimeStart && eventType < (enTimerEventType)this.m_eventIDs.Length)
			{
				this.m_eventIDs[(int)eventType] = eventId;
			}
		}

		public void SetCurrentTime(float time)
		{
			this.m_currentTime = (double)time;
		}

		public float GetCurrentTime()
		{
			return (float)this.m_currentTime;
		}

		public void SetOnChangedIntervalTime(float intervalTime)
		{
			this.m_onChangedIntervalTime = (double)intervalTime;
		}

		public void ResetTime()
		{
			this.m_startTime = (double)Time.realtimeSinceStartup;
			this.m_pauseTime = 0.0;
			this.m_pauseElastTime = 0.0;
			if (this.m_timerType == enTimerType.CountUp)
			{
				this.m_currentTime = 0.0;
			}
			else if (this.m_timerType == enTimerType.CountDown)
			{
				this.m_currentTime = this.m_totalTime;
			}
			this.m_lastOnChangedTime = this.m_currentTime;
		}

		public void StartTimer()
		{
			if (this.m_isRunning)
			{
				return;
			}
			this.ResetTime();
			this.m_isRunning = true;
			this.DispatchTimerEvent(enTimerEventType.TimeStart);
		}

		public void ReStartTimer()
		{
			this.EndTimer();
			this.StartTimer();
		}

		public void OnApplicationPause(bool pause)
		{
			if (!this.m_pausedWhenAppPaused)
			{
				return;
			}
			if (pause)
			{
				this.PauseTimer();
			}
			else
			{
				this.ResumeTimer();
			}
		}

		public void PauseTimer()
		{
			if (this.m_isPaused)
			{
				return;
			}
			this.m_pauseTime = (double)Time.realtimeSinceStartup;
			this.m_isPaused = true;
		}

		public void ResumeTimer()
		{
			if (!this.m_isPaused)
			{
				return;
			}
			this.m_pauseElastTime += (double)Time.realtimeSinceStartup - this.m_pauseTime;
			this.m_isPaused = false;
		}

		public void EndTimer()
		{
			this.ResetTime();
			this.m_isRunning = false;
		}

		private void UpdateTimer()
		{
			if (!this.m_isRunning || this.m_isPaused)
			{
				return;
			}
			bool flag = false;
			double currentTime = this.m_currentTime;
			enTimerType timerType = this.m_timerType;
			if (timerType != enTimerType.CountUp)
			{
				if (timerType == enTimerType.CountDown)
				{
					this.m_currentTime = this.m_totalTime - ((double)Time.realtimeSinceStartup - this.m_startTime - this.m_pauseElastTime);
					flag = (this.m_currentTime <= 0.0);
				}
			}
			else
			{
				this.m_currentTime = (double)Time.realtimeSinceStartup - this.m_startTime - this.m_pauseElastTime;
				flag = (this.m_currentTime >= this.m_totalTime);
			}
			if ((int)currentTime != (int)this.m_currentTime)
			{
				this.RefreshTimeDisplay();
			}
			if ((double)Mathf.Abs((float)(this.m_currentTime - this.m_lastOnChangedTime)) >= this.m_onChangedIntervalTime)
			{
				this.m_lastOnChangedTime = this.m_currentTime;
				this.DispatchTimerEvent(enTimerEventType.TimeChanged);
			}
			if (flag)
			{
				this.EndTimer();
				this.DispatchTimerEvent(enTimerEventType.TimeUp);
				if (this.m_closeBelongedFormWhenTimeup)
				{
					this.m_belongedFormScript.Close();
				}
			}
		}

		private void RefreshTimeDisplay()
		{
			if (this.m_timerText == null)
			{
				return;
			}
			if (this.m_timerDisplayType != enTimerDisplayType.None)
			{
				int num = (int)this.m_currentTime;
				switch (this.m_timerDisplayType)
				{
				case enTimerDisplayType.H_M_S:
				{
					int num2 = num / 3600;
					num -= num2 * 3600;
					int num3 = num / 60;
					int num4 = num - num3 * 60;
					this.m_timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", num2, num3, num4);
					break;
				}
				case enTimerDisplayType.M_S:
				{
					int num5 = num / 60;
					int num6 = num - num5 * 60;
					this.m_timerText.text = string.Format("{0:D2}:{1:D2}", num5, num6);
					break;
				}
				case enTimerDisplayType.S:
					this.m_timerText.text = string.Format("{0:D}", num);
					break;
				case enTimerDisplayType.H_M:
				{
					int num7 = num / 3600;
					num -= num7 * 3600;
					int num8 = num / 60;
					this.m_timerText.text = string.Format("{0:D2}:{1:D2}", num7, num8);
					break;
				}
				case enTimerDisplayType.D_H_M_S:
				{
					int num9 = num / 86400;
					num -= num9 * 86400;
					int num10 = num / 3600;
					num -= num10 * 3600;
					int num11 = num / 60;
					int num12 = num - num11 * 60;
					this.m_timerText.text = string.Format("{0}天{1:D2}:{2:D2}:{3:D2}", new object[]
					{
						num9,
						num10,
						num11,
						num12
					});
					break;
				}
				case enTimerDisplayType.D_H_M:
				{
					int num13 = num / 86400;
					num -= num13 * 86400;
					int num14 = num / 3600;
					num -= num14 * 3600;
					int num15 = num / 60;
					this.m_timerText.text = string.Format("{0}天{1:D2}:{2:D2}", num13, num14, num15);
					break;
				}
				case enTimerDisplayType.D:
				{
					int num16 = num / 86400;
					this.m_timerText.text = string.Format("{0}天", num16);
					break;
				}
				}
			}
		}

		private void DispatchTimerEvent(enTimerEventType eventType)
		{
			if (this.m_eventIDs[(int)eventType] == enUIEventID.None)
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
			uIEvent.m_eventID = this.m_eventIDs[(int)eventType];
			uIEvent.m_eventParams = this.m_eventParams[(int)eventType];
			base.DispatchUIEvent(uIEvent);
		}

		public bool IsRunning()
		{
			return this.m_isRunning;
		}
	}
}
