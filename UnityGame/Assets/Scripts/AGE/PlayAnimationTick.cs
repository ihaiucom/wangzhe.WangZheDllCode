using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AGE
{
	[EventCategory("Animation")]
	public class PlayAnimationTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{
			typeof(Animation)
		})]
		public int targetId;

		public string clipName = string.Empty;

		public float crossFadeTime;

		public float playSpeed = 1f;

		public int layer = 1;

		public bool loop;

		public bool applyActionSpeed;

		public bool bNoTimeScale;

		public bool alwaysAnimate;

		private Dictionary<int, Animation> m_animationCache = new Dictionary<int, Animation>();

		public override bool SupportEditMode()
		{
			return true;
		}

		public override BaseEvent Clone()
		{
			PlayAnimationTick playAnimationTick = ClassObjPool<PlayAnimationTick>.Get();
			playAnimationTick.CopyData(this);
			return playAnimationTick;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			PlayAnimationTick playAnimationTick = src as PlayAnimationTick;
			this.targetId = playAnimationTick.targetId;
			this.clipName = playAnimationTick.clipName;
			this.crossFadeTime = playAnimationTick.crossFadeTime;
			this.playSpeed = playAnimationTick.playSpeed;
			this.layer = playAnimationTick.layer;
			this.loop = playAnimationTick.loop;
			this.bNoTimeScale = playAnimationTick.bNoTimeScale;
			this.alwaysAnimate = playAnimationTick.alwaysAnimate;
			this.applyActionSpeed = playAnimationTick.applyActionSpeed;
			this.m_animationCache.Clear();
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.clipName = string.Empty;
			this.crossFadeTime = 0f;
			this.playSpeed = 1f;
			this.layer = 1;
			this.loop = false;
			this.bNoTimeScale = false;
			this.alwaysAnimate = false;
			this.applyActionSpeed = false;
			this.m_animationCache.Clear();
		}

		private Animation GetAnimation(GameObject obj)
		{
			int instanceID = obj.GetInstanceID();
			Animation animation = null;
			if (!this.m_animationCache.TryGetValue(instanceID, ref animation))
			{
				animation = obj.GetComponent<Animation>();
				this.m_animationCache.Add(instanceID, animation);
			}
			return animation;
		}

		public override void Process(Action _action, Track _track)
		{
			GameObject gameObject = _action.GetGameObject(this.targetId);
			if (gameObject == null)
			{
				return;
			}
			GameObject gameObject2 = gameObject;
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (this.GetAnimation(gameObject) == null)
			{
				if (actorHandle)
				{
					gameObject2 = actorHandle.handle.ActorMesh;
				}
				else
				{
					gameObject2 = null;
				}
			}
			if (gameObject2 == null)
			{
				return;
			}
			Animation animation = this.GetAnimation(gameObject2);
			AnimationState animationState = animation[this.clipName];
			if (animationState == null)
			{
				return;
			}
			if (this.alwaysAnimate && animation.cullingType != AnimationCullingType.AlwaysAnimate)
			{
				animation.cullingType = AnimationCullingType.AlwaysAnimate;
			}
			float speed = this.playSpeed * (this.applyActionSpeed ? _action.playSpeed.single : 1f);
			if (this.bNoTimeScale)
			{
				DialogueProcessor.PlayAnimNoTimeScale(animation, this.clipName, this.loop, null);
			}
			else
			{
				AnimPlayComponent animPlayComponent = actorHandle ? actorHandle.handle.AnimControl : null;
				if (animPlayComponent != null)
				{
					animPlayComponent.Play(new PlayAnimParam
					{
						animName = this.clipName,
						blendTime = this.crossFadeTime,
						loop = this.loop,
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
			}
			animationState.speed = speed;
		}

		public override void ProcessBlend(Action _action, Track _track, TickEvent _prevEvent, float _blendWeight)
		{
			GameObject gameObject = _action.GetGameObject(this.targetId);
			if (gameObject == null)
			{
				return;
			}
			GameObject gameObject2 = gameObject;
			if (this.GetAnimation(gameObject) == null)
			{
				PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
				if (actorHandle)
				{
					gameObject2 = actorHandle.handle.ActorMesh;
				}
				else
				{
					gameObject2 = null;
				}
			}
			if (gameObject2 == null)
			{
				return;
			}
			if (_prevEvent == null)
			{
				return;
			}
			Animation animation = this.GetAnimation(gameObject2);
			AnimationState animationState = animation[this.clipName];
			if (animationState == null)
			{
				return;
			}
			animationState.speed = this.playSpeed * (this.applyActionSpeed ? _action.playSpeed.single : 1f);
		}

		public override void PostProcess(Action _action, Track _track, int _localTime)
		{
			GameObject gameObject = _action.GetGameObject(this.targetId);
			if (gameObject == null)
			{
				return;
			}
			GameObject gameObject2 = gameObject;
			if (this.GetAnimation(gameObject) == null)
			{
				PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
				if (actorHandle)
				{
					gameObject2 = actorHandle.handle.ActorMesh;
				}
				else
				{
					gameObject2 = null;
				}
			}
			if (gameObject2 == null)
			{
				return;
			}
			Animation animation = this.GetAnimation(gameObject2);
			AnimationState animationState = animation[this.clipName];
			if (animationState == null)
			{
				return;
			}
			animationState.speed = this.playSpeed * (this.applyActionSpeed ? _action.playSpeed.single : 1f);
		}
	}
}
