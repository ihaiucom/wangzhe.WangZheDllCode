using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Material")]
	public class MaterialFadeDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int triggerId;

		public bool FadeIn;

		private ListView<Material> materials;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			MaterialFadeDuration materialFadeDuration = src as MaterialFadeDuration;
			this.triggerId = materialFadeDuration.triggerId;
			this.FadeIn = materialFadeDuration.FadeIn;
		}

		public override BaseEvent Clone()
		{
			MaterialFadeDuration materialFadeDuration = ClassObjPool<MaterialFadeDuration>.Get();
			materialFadeDuration.CopyData(this);
			return materialFadeDuration;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.materials = null;
		}

		public override void Enter(Action _action, Track _track)
		{
			GameObject gameObject = _action.GetGameObject(this.triggerId);
			this.materials = FadeMaterialUtility.GetFadeMaterials(gameObject);
			if (this.materials == null)
			{
				return;
			}
			float value = (!this.FadeIn) ? 1f : 0f;
			for (int i = 0; i < this.materials.Count; i++)
			{
				this.materials[i].SetFloat("_FadeFactor", value);
			}
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			if (this.length == 0 || this.materials == null)
			{
				return;
			}
			float num = ActionUtility.MsToSec(_localTime) / base.lengthSec;
			if (!this.FadeIn)
			{
				num = 1f - num;
			}
			num = Mathf.Clamp01(num);
			for (int i = 0; i < this.materials.Count; i++)
			{
				this.materials[i].SetFloat("_FadeFactor", num);
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			if (this.materials == null)
			{
				return;
			}
			float value = (!this.FadeIn) ? 0f : 1f;
			value = Mathf.Clamp01(value);
			for (int i = 0; i < this.materials.Count; i++)
			{
				Material material = this.materials[i];
				material.SetFloat("_FadeFactor", value);
				if (this.FadeIn)
				{
					Shader fadeShader = FadeMaterialUtility.GetFadeShader(material.shader, false);
					if (fadeShader != null && fadeShader != material.shader)
					{
						material.shader = fadeShader;
					}
				}
			}
			this.materials = null;
		}
	}
}
