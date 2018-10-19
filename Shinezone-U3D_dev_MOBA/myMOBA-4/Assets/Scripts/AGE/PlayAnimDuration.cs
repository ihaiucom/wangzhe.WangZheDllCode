using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AGE
{
	[EventCategory("Animation")]
	public class PlayAnimDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{
			typeof(Animation)
		})]
		public int targetId;

		public string clipName = string.Empty;

		public float crossFadeTime;

		public int layer = 1;

		public bool bLoop;

		public float startTime;

		public float endTime = 99999f;

		public bool applyActionSpeed;

		public bool playNextAnim;

		private Dictionary<int, Animation> m_animationCache = new Dictionary<int, Animation>();

		public bool alwaysAnimate;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.clipName = string.Empty;
			this.crossFadeTime = 0f;
			this.layer = 1;
			this.bLoop = false;
			this.startTime = 0f;
			this.endTime = 99999f;
			this.applyActionSpeed = false;
			this.alwaysAnimate = false;
			this.m_animationCache.Clear();
		}

		public override BaseEvent Clone()
		{
			PlayAnimDuration playAnimDuration = ClassObjPool<PlayAnimDuration>.Get();
			playAnimDuration.CopyData(this);
			return playAnimDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			PlayAnimDuration playAnimDuration = src as PlayAnimDuration;
			this.targetId = playAnimDuration.targetId;
			this.clipName = playAnimDuration.clipName;
			this.crossFadeTime = playAnimDuration.crossFadeTime;
			this.layer = playAnimDuration.layer;
			this.bLoop = playAnimDuration.bLoop;
			this.startTime = playAnimDuration.startTime;
			this.endTime = playAnimDuration.endTime;
			this.applyActionSpeed = playAnimDuration.applyActionSpeed;
			this.playNextAnim = playAnimDuration.playNextAnim;
			this.alwaysAnimate = playAnimDuration.alwaysAnimate;
			this.m_animationCache.Clear();
		}

		private Animation GetAnimation(GameObject obj)
		{
			int instanceID = obj.GetInstanceID();
			Animation animation = null;
			if (!this.m_animationCache.TryGetValue(instanceID, out animation))
			{
				animation = obj.GetComponent<Animation>();
				this.m_animationCache.Add(instanceID, animation);
			}
			return animation;
		}

		public override void Enter(Action _action, Track _track)
		{
			GameObject gameObject = _action.GetGameObject(this.targetId);
			if (gameObject == null || this.length == 0)
			{
				return;
			}
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			Animation animation;
			if (actorHandle)
			{
				animation = actorHandle.handle.ActorMeshAnimation;
			}
			else
			{
				GameObject obj = gameObject;
				animation = this.GetAnimation(obj);
			}
			if (animation == null)
			{
				return;
			}
			AnimationState animationState = animation[this.clipName];
			if (animationState == null)
			{
				return;
			}
			if (this.alwaysAnimate && animation.cullingType != AnimationCullingType.AlwaysAnimate)
			{
				animation.cullingType = AnimationCullingType.AlwaysAnimate;
			}
			if (this.startTime < 0f)
			{
				this.startTime = 0f;
			}
			if (this.endTime > animationState.clip.length)
			{
				this.endTime = animationState.clip.length;
			}
			float speed;
			if (!this.bLoop)
			{
				speed = (this.endTime - this.startTime) / base.lengthSec * ((!this.applyActionSpeed) ? 1f : _action.playSpeed.single);
			}
			else
			{
				speed = ((!this.applyActionSpeed) ? 1f : _action.playSpeed.single);
			}
			AnimPlayComponent animPlayComponent = null;
			if (actorHandle)
			{
				animPlayComponent = actorHandle.handle.AnimControl;
			}
			if (animPlayComponent != null)
			{
				animPlayComponent.Play(new PlayAnimParam
				{
					animName = this.clipName,
					blendTime = this.crossFadeTime,
					loop = this.bLoop,
					layer = this.layer,
					speed = speed
				});
			}
			else
			{
				if (animationState.enabled)
				{
					animation.Stop();
				}
				if (this.crossFadeTime > 0f)
				{
					animation.CrossFade(this.clipName, this.crossFadeTime);
				}
				else
				{
					animation.Play(this.clipName);
				}
			}
			animationState.speed = speed;
		}

		public override void Leave(Action _action, Track _track)
		{
			GameObject gameObject = _action.GetGameObject(this.targetId);
			if (gameObject == null || this.length == 0)
			{
				return;
			}
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			Animation animation;
			if (actorHandle)
			{
				animation = actorHandle.handle.ActorMeshAnimation;
			}
			else
			{
				GameObject obj = gameObject;
				animation = this.GetAnimation(obj);
			}
			if (animation == null)
			{
				return;
			}
			if (actorHandle)
			{
				actorHandle.handle.AnimControl.Stop(this.clipName, this.playNextAnim);
			}
			else
			{
				animation[this.clipName].enabled = false;
			}
		}
	}
}
