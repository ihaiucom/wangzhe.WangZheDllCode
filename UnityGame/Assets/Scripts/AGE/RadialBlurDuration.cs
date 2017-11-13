using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using System;
using UnityEngine;

namespace AGE
{
	public class RadialBlurDuration : DurationEvent
	{
		public float falloffExp = 1.5f;

		public float blurScale = 50f;

		private Camera[] cameras;

		public override bool SupportEditMode()
		{
			return false;
		}

		public override BaseEvent Clone()
		{
			RadialBlurDuration radialBlurDuration = ClassObjPool<RadialBlurDuration>.Get();
			radialBlurDuration.CopyData(this);
			return radialBlurDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			RadialBlurDuration radialBlurDuration = src as RadialBlurDuration;
			this.falloffExp = radialBlurDuration.falloffExp;
			this.blurScale = radialBlurDuration.blurScale;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.falloffExp = 1.5f;
			this.blurScale = 50f;
			this.cameras = null;
		}

		public override void Enter(Action _action, Track _track)
		{
			if (!GameSettings.AllowRadialBlur)
			{
				return;
			}
			this.cameras = Object.FindObjectsOfType<Camera>();
			if (this.cameras == null)
			{
				return;
			}
			int mask = LayerMask.GetMask(new string[]
			{
				"Scene"
			});
			for (int i = 0; i < this.cameras.Length; i++)
			{
				Camera camera = this.cameras[i];
				if ((camera.cullingMask & mask) != 0)
				{
					RadialBlur radialBlur = camera.GetComponent<RadialBlur>();
					if (radialBlur == null)
					{
						radialBlur = camera.gameObject.AddComponent<RadialBlur>();
					}
					radialBlur.blurScale = this.blurScale;
					radialBlur.falloffExp = this.falloffExp;
					radialBlur.UpdateParameters();
				}
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			if (this.cameras == null)
			{
				return;
			}
			int mask = LayerMask.GetMask(new string[]
			{
				"Scene"
			});
			for (int i = 0; i < this.cameras.Length; i++)
			{
				Camera camera = this.cameras[i];
				if (!(camera == null) && (camera.cullingMask & mask) != 0)
				{
					RadialBlur component = camera.GetComponent<RadialBlur>();
					if (component)
					{
						Object.Destroy(component);
					}
				}
			}
			this.cameras = null;
		}
	}
}
