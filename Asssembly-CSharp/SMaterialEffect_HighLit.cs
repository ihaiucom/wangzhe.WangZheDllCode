using System;
using UnityEngine;

public class SMaterialEffect_HighLit : SMaterialEffect_Base
{
	public string paramName = "_RimColor";

	public float period = 0.8f;

	public float minExp = 2f;

	public float maxExp = 1.5f;

	public float minFactor = 1f;

	public float maxFactor = 2f;

	public SMaterialEffect_Base.STimer timer = default(SMaterialEffect_Base.STimer);

	public Vector3 color = new Vector3(1f, 1f, 0f);

	public SMaterialEffect_HighLit()
	{
		this.shaderKeyword = "_RIM_COLOR_ON";
	}

	public override void Play()
	{
		base.Play();
		this.timer.Start();
		this.SetColor();
	}

	public override bool Update()
	{
		this.SetColor();
		return false;
	}

	private void SetColor()
	{
		this.timer.Update();
		float t = Mathf.PingPong(this.timer.curTime, this.period);
		float w = Mathf.Lerp(this.minExp, this.maxExp, t);
		float num = Mathf.Lerp(this.minFactor, this.maxFactor, t);
		Vector4 vector = default(Vector4);
		vector.x = this.color.x * num;
		vector.y = this.color.y * num;
		vector.z = this.color.z * num;
		vector.w = w;
		ListView<Material> mats = this.owner.mats;
		if (mats != null)
		{
			for (int i = 0; i < mats.Count; i++)
			{
				Material material = mats[i];
				if (material != null)
				{
					material.SetVector(this.paramName, vector);
				}
			}
		}
	}
}
