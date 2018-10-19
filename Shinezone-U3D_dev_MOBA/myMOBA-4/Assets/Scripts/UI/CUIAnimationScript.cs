using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class CUIAnimationScript : CUIComponent
	{
		private Animation m_animation;

		private AnimationState m_currentAnimationState;

		private float m_currentAnimationTime;

		[HideInInspector]
		public enUIEventID[] m_eventIDs = new enUIEventID[Enum.GetValues(typeof(enAnimationEventType)).Length];

		public stUIEventParams[] m_eventParams = new stUIEventParams[Enum.GetValues(typeof(enAnimationEventType)).Length];

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			base.Initialize(formScript);
			this.m_animation = base.gameObject.GetComponent<Animation>();
			if (this.m_animation != null && this.m_animation.playAutomatically && this.m_animation.clip != null)
			{
				this.m_currentAnimationState = this.m_animation[this.m_animation.clip.name];
				this.m_currentAnimationTime = 0f;
				this.DispatchAnimationEvent(enAnimationEventType.AnimationStart);
			}
		}

		protected override void OnDestroy()
		{
			this.m_animation = null;
			this.m_currentAnimationState = null;
			base.OnDestroy();
		}

		private void Update()
		{
			if (this.m_belongedFormScript != null && this.m_belongedFormScript.IsClosed())
			{
				return;
			}
			if (this.m_currentAnimationState == null)
			{
				return;
			}
			if (this.m_currentAnimationState.wrapMode != WrapMode.Loop && this.m_currentAnimationState.wrapMode != WrapMode.PingPong && this.m_currentAnimationState.wrapMode != WrapMode.ClampForever)
			{
				if (this.m_currentAnimationTime > this.m_currentAnimationState.length)
				{
					this.m_currentAnimationState = null;
					this.m_currentAnimationTime = 0f;
					this.DispatchAnimationEvent(enAnimationEventType.AnimationEnd);
				}
				else
				{
					this.m_currentAnimationTime += Time.deltaTime;
				}
			}
		}

		public bool IsHaveAnimation(string aniName)
		{
			return !(this.m_animation == null) && this.m_animation[aniName] != null;
		}

		public void PlayAnimation(string animName, bool forceRewind)
		{
			if (this.m_currentAnimationState != null && this.m_currentAnimationState.name.Equals(animName) && !forceRewind)
			{
				return;
			}
			if (this.m_currentAnimationState != null)
			{
				this.m_animation.Stop(this.m_currentAnimationState.name);
				this.m_currentAnimationState = null;
				this.m_currentAnimationTime = 0f;
			}
			this.m_currentAnimationState = this.m_animation[animName];
			this.m_currentAnimationTime = 0f;
			if (this.m_currentAnimationState != null)
			{
				this.m_animation.Play(animName);
				this.DispatchAnimationEvent(enAnimationEventType.AnimationStart);
			}
		}

		public void StopAnimation(string animName)
		{
			if (this.m_currentAnimationState == null || !this.m_currentAnimationState.name.Equals(animName))
			{
				return;
			}
			this.m_animation.Stop(animName);
			this.DispatchAnimationEvent(enAnimationEventType.AnimationEnd);
			this.m_currentAnimationState = null;
			this.m_currentAnimationTime = 0f;
		}

		public void StopAnimation()
		{
			if (this.m_animation != null)
			{
				this.m_animation.Stop();
			}
		}

		public string GetCurrentAnimation()
		{
			return (!(this.m_currentAnimationState == null)) ? this.m_currentAnimationState.name : null;
		}

		public bool IsAnimationStopped(string animationName)
		{
			return string.IsNullOrEmpty(animationName) || this.m_currentAnimationState == null || this.m_currentAnimationTime == 0f || !string.Equals(this.m_currentAnimationState.name, animationName);
		}

		public void DispatchAnimationEvent(enAnimationEventType animationEventType)
		{
			if (this.m_eventIDs[(int)animationEventType] == enUIEventID.None)
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
			uIEvent.m_eventID = this.m_eventIDs[(int)animationEventType];
			uIEvent.m_eventParams = this.m_eventParams[(int)animationEventType];
			base.DispatchUIEvent(uIEvent);
		}

		public void SetAnimationEvent(enAnimationEventType animationEventType, enUIEventID eventId, stUIEventParams eventParams = default(stUIEventParams))
		{
			this.m_eventIDs[(int)animationEventType] = eventId;
			this.m_eventParams[(int)animationEventType] = eventParams;
		}

		public void SetAnimationSpeed(string animName, float speed)
		{
			if (this.m_animation != null && this.m_animation[animName] != null)
			{
				this.m_animation[animName].speed = speed;
			}
		}
	}
}
