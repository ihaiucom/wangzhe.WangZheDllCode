using System;
using UnityEngine;

public class SMaterialEffect_Translucent : SMaterialEffect_Base
{
	public float minAlpha = 0.3f;

	public bool enableFade = true;

	public bool forceUpdateFactor;

	public SMaterialEffect_Base.FadeState fadeState;

	public float fadeInterval = 0.05f;

	public SMaterialEffect_Base.STimer fadeTimer = default(SMaterialEffect_Base.STimer);

	public override void Play()
	{
		if (this.fadeInterval == 0f)
		{
			this.enableFade = false;
		}
		float alpha = (!this.enableFade) ? 1f : 0f;
		this.SetTranslucent(this.owner.mats, true);
		this.SetAlpha(alpha);
		this.fadeState = SMaterialEffect_Base.FadeState.FadeIn;
		this.fadeTimer.Start();
	}

	private void SetAlpha(float factor)
	{
		float value = Mathf.Lerp(1f, this.minAlpha, factor);
		if (this.owner != null)
		{
			ListView<Material> mats = this.owner.mats;
			for (int i = 0; i < mats.Count; i++)
			{
				Material material = mats[i];
				if (material != null)
				{
					material.SetFloat("_AlphaVal", value);
				}
			}
		}
		this.forceUpdateFactor = false;
	}

	private void SetTranslucent(ListView<Material> mats, bool b)
	{
		if (mats != null)
		{
			for (int i = 0; i < mats.Count; i++)
			{
				Material material = mats[i];
				string name = material.shader.name;
				bool shadow;
				bool flag;
				bool occlusion;
				HeroMaterialUtility.GetShaderProperty(name, out shadow, out flag, out occlusion);
				string text = HeroMaterialUtility.MakeShaderName(name, shadow, b, occlusion);
				if (text != material.shader.name)
				{
					material.shader = Shader.Find(text);
				}
			}
		}
	}

	public void BeginFadeOut()
	{
		if (this.fadeState == SMaterialEffect_Base.FadeState.FadeOut)
		{
			return;
		}
		this.fadeTimer.Start();
		this.fadeState = SMaterialEffect_Base.FadeState.FadeOut;
	}

	public override bool Update()
	{
		if (this.enableFade)
		{
			float alpha = 1f;
			if (SMaterialEffect_Base.UpdateFadeState(out alpha, ref this.fadeState, ref this.fadeTimer, this.fadeInterval, this.forceUpdateFactor))
			{
				this.SetAlpha(alpha);
			}
			if (this.fadeState == SMaterialEffect_Base.FadeState.Stopped)
			{
				this.Stop();
				return true;
			}
		}
		return false;
	}

	public override void Stop()
	{
		this.SetTranslucent(this.owner.mats, false);
	}

	public override void OnRelease()
	{
		this.owner = null;
		this.minAlpha = 0.3f;
		this.enableFade = true;
		this.fadeState = SMaterialEffect_Base.FadeState.FadeIn;
		this.fadeInterval = 0.05f;
		this.forceUpdateFactor = false;
	}

	public override void OnMeshChanged(ListView<Material> oldMats, ListView<Material> newMats)
	{
		this.SetTranslucent(oldMats, false);
		this.SetTranslucent(newMats, true);
		this.forceUpdateFactor = true;
	}
}
