using System;
using UnityEngine;

[Serializable]
public class RGBCurve : ScriptableObject
{
	public AnimationCurve R = new AnimationCurve();

	public AnimationCurve G = new AnimationCurve();

	public AnimationCurve B = new AnimationCurve();

	public float length
	{
		get
		{
			float a = RGBCurve.MaxTime(this.R);
			float b = RGBCurve.MaxTime(this.G);
			float b2 = RGBCurve.MaxTime(this.B);
			return Mathf.Max(Mathf.Max(a, b), b2);
		}
	}

	public static float MaxTime(AnimationCurve curve)
	{
		if (curve == null || curve.length == 0)
		{
			return 0f;
		}
		int length = curve.length;
		return curve[length - 1].time;
	}

	public Vector3 Eval(float t)
	{
		return new Vector3
		{
			x = this.R.Evaluate(t),
			y = this.G.Evaluate(t),
			z = this.B.Evaluate(t)
		};
	}
}
