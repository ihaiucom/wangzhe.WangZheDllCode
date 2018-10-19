using System;
using UnityEngine;

public class SMaterialEffect_Tex : SMaterialEffect_Base
{
	public string texParamName;

	public string fadeParamName;

	public Texture tex;

	public float factorScale = 1f;

	public bool enableFade;

	public bool hasFadeFactor;

	public SMaterialEffect_Base.FadeState fadeState;

	public float fadeInterval;

	public SMaterialEffect_Base.STimer fadeTimer = default(SMaterialEffect_Base.STimer);

	public override void Play()
	{
		base.Play();
		ListView<Material> mats = this.owner.mats;
		for (int i = 0; i < mats.Count; i++)
		{
			Material material = mats[i];
			material.SetTexture(this.texParamName, this.tex);
		}
		if (this.fadeInterval == 0f)
		{
			this.enableFade = false;
		}
		if (this.hasFadeFactor)
		{
			float factor = (!this.enableFade) ? 1f : 0f;
			this.SetFactor(factor);
		}
		else
		{
			this.enableFade = false;
		}
		this.fadeState = SMaterialEffect_Base.FadeState.FadeIn;
		this.fadeTimer.Start();
	}

	public void Replay(Texture newTex)
	{
		base.AllocId();
		if (this.tex != newTex)
		{
			this.tex = newTex;
			ListView<Material> mats = this.owner.mats;
			for (int i = 0; i < mats.Count; i++)
			{
				Material material = mats[i];
				material.SetTexture(this.texParamName, newTex);
			}
		}
		if (this.fadeState == SMaterialEffect_Base.FadeState.FadeOut)
		{
			this.fadeState = SMaterialEffect_Base.FadeState.FadeIn;
		}
	}

	private void SetFactor(float factor)
	{
		factor *= this.factorScale;
		ListView<Material> mats = this.owner.mats;
		for (int i = 0; i < mats.Count; i++)
		{
			Material material = mats[i];
			material.SetFloat(this.fadeParamName, factor);
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
			float factor = 1f;
			if (SMaterialEffect_Base.UpdateFadeState(out factor, ref this.fadeState, ref this.fadeTimer, this.fadeInterval, false))
			{
				this.SetFactor(factor);
			}
			if (this.fadeState == SMaterialEffect_Base.FadeState.Stopped)
			{
				this.Stop();
				return true;
			}
		}
		return false;
	}

	public override void OnRelease()
	{
		this.owner = null;
		this.texParamName = string.Empty;
		this.fadeParamName = string.Empty;
		this.tex = null;
		this.factorScale = 1f;
		this.enableFade = false;
		this.hasFadeFactor = false;
		this.fadeState = SMaterialEffect_Base.FadeState.FadeIn;
		this.fadeInterval = 0f;
	}
}
