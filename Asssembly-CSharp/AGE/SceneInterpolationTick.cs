using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class SceneInterpolationTick : TickEvent
	{
		[ObjectTemplate(true)]
		public int targetId = -1;

		public float fadeTime = 2f;

		public override BaseEvent Clone()
		{
			SceneInterpolationTick sceneInterpolationTick = ClassObjPool<SceneInterpolationTick>.Get();
			sceneInterpolationTick.CopyData(this);
			return sceneInterpolationTick;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SceneInterpolationTick sceneInterpolationTick = src as SceneInterpolationTick;
			this.targetId = sceneInterpolationTick.targetId;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
		}

		public override void Process(Action _action, Track _track)
		{
			GameObject gameObject = _action.GetGameObject(this.targetId);
			if (gameObject == null)
			{
				return;
			}
			SceneInterpolation sceneInterpolation = gameObject.GetComponent<SceneInterpolation>();
			if (sceneInterpolation == null)
			{
				sceneInterpolation = gameObject.AddComponent<SceneInterpolation>();
			}
			sceneInterpolation.FadeTime = this.fadeTime;
			sceneInterpolation.Play();
		}
	}
}
