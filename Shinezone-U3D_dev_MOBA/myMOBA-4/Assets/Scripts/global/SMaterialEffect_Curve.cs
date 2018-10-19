using System;
using UnityEngine;

public class SMaterialEffect_Curve : SMaterialEffect_Base
{
	public RGBCurve curve;

	public string paramName = string.Empty;

	public float maxTime;

	public SMaterialEffect_Base.STimer timer = default(SMaterialEffect_Base.STimer);

	public override void Play()
	{
		base.Play();
		this.timer.Start();
		this.maxTime = this.curve.length;
		this.maxTime = Mathf.Max(this.maxTime, 0.001f);
		this.SetColor(0f);
	}

	public override bool Update()
	{
		float num = this.timer.Update();
		this.SetColor(num);
		if (num >= this.maxTime)
		{
			this.Stop();
			return true;
		}
		return false;
	}

	private void SetColor(float time)
	{
		Vector3 vector = this.curve.Eval(time);
		Vector4 vector2 = new Vector4(vector.x, vector.y, vector.z, 1f);
		ListView<Material> mats = this.owner.mats;
		for (int i = 0; i < mats.Count; i++)
		{
			Material material = mats[i];
			material.SetVector(this.paramName, vector2);
		}
	}

	public override void OnRelease()
	{
		this.owner = null;
		this.curve = null;
	}
}
