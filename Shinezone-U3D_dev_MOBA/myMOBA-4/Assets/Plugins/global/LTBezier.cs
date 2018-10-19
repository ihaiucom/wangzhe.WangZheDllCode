using System;
using UnityEngine;

public class LTBezier
{
	public float length;

	private Vector3 a;

	private Vector3 aa;

	private Vector3 bb;

	private Vector3 cc;

	private float len;

	private float[] arcLengths;

	public LTBezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float precision)
	{
		this.a = a;
		this.aa = -a + 3f * (b - c) + d;
		this.bb = 3f * (a + c) - 6f * b;
		this.cc = 3f * (b - a);
		this.len = 1f / precision;
		this.arcLengths = new float[(int)this.len + 1];
		this.arcLengths[0] = 0f;
		Vector3 vector = a;
		float num = 0f;
		int num2 = 1;
		while ((float)num2 <= this.len)
		{
			Vector3 vector2 = this.bezierPoint((float)num2 * precision);
			num += (vector - vector2).magnitude;
			this.arcLengths[num2] = num;
			vector = vector2;
			num2++;
		}
		this.length = num;
	}

	private float map(float u)
	{
		float num = u * this.arcLengths[(int)this.len];
		int i = 0;
		int num2 = (int)this.len;
		int num3 = 0;
		while (i < num2)
		{
			num3 = i + ((int)((float)(num2 - i) / 2f) | 0);
			if (this.arcLengths[num3] < num)
			{
				i = num3 + 1;
			}
			else
			{
				num2 = num3;
			}
		}
		if (this.arcLengths[num3] > num)
		{
			num3--;
		}
		if (num3 < 0)
		{
			num3 = 0;
		}
		return ((float)num3 + (num - this.arcLengths[num3]) / (this.arcLengths[num3 + 1] - this.arcLengths[num3])) / this.len;
	}

	private Vector3 bezierPoint(float t)
	{
		return ((this.aa * t + this.bb) * t + this.cc) * t + this.a;
	}

	public Vector3 point(float t)
	{
		return this.bezierPoint(this.map(t));
	}
}
