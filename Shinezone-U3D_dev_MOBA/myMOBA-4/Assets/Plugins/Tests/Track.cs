using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
	public class Track
	{
		public bool isVisiable = true;

		protected int MaxSampling;

		public static readonly int DefaultSampling = 100;

		public static readonly float Clip = 0.1f;

		private bool bAverageStep = true;

		private bool bFixedRange;

		public string tag
		{
			get;
			protected set;
		}

		public Color drawColor
		{
			get;
			set;
		}

		public List<float> samples
		{
			get;
			protected set;
		}

		public float fixedMaxSampleValue
		{
			get;
			protected set;
		}

		public float fixedMinSampleValue
		{
			get;
			protected set;
		}

		public bool isFixedRange
		{
			get
			{
				return this.bFixedRange;
			}
		}

		public int maxSampling
		{
			get
			{
				return this.MaxSampling;
			}
			set
			{
				this.MaxSampling = value;
				this.CollapseSamplings();
			}
		}

		public float maxValue
		{
			get
			{
				float num = -3.40282347E+38f;
				for (int i = 0; i < this.samples.Count; i++)
				{
					if (this.samples[i] > num)
					{
						num = this.samples[i];
					}
				}
				return num;
			}
		}

		public float minValue
		{
			get
			{
				float num = 3.40282347E+38f;
				for (int i = 0; i < this.samples.Count; i++)
				{
					if (this.samples[i] < num)
					{
						num = this.samples[i];
					}
				}
				return num;
			}
		}

		public bool hasSamples
		{
			get
			{
				return this.samples.Count > 0;
			}
		}

		public Track(string InTag, Color InColor)
		{
			this.samples = new List<float>(Track.DefaultSampling);
			this.tag = InTag;
			this.drawColor = InColor;
			this.MaxSampling = Track.DefaultSampling;
		}

		public Track(string InTag, Color InColor, float InMin, float InMax) : this(InTag, InColor)
		{
			this.SetFixedRange(true, InMin, InMax);
		}

		public void SetFixedRange(bool bInFixed, float InMin, float InMax)
		{
			this.bFixedRange = bInFixed;
			this.fixedMinSampleValue = InMin;
			this.fixedMaxSampleValue = InMax;
		}

		private void CollapseSamplings()
		{
			if (this.samples.Count > this.maxSampling)
			{
				this.samples.RemoveRange(0, this.samples.Count - this.maxSampling);
			}
		}

		public void AddSample(float InValue)
		{
			this.samples.Add(InValue);
			this.CollapseSamplings();
		}

		public void SetSample(float InValue, int reverseIndex)
		{
			if (reverseIndex >= 0 && this.samples.Count - reverseIndex > 0)
			{
				this.samples[this.samples.Count - reverseIndex - 1] = InValue;
			}
		}

		private float CalcY(float InPercent)
		{
			return InPercent * (float)Screen.height * (1f - Track.Clip * 2f);
		}

		private float CalcX(int InIndex)
		{
			if (this.bAverageStep)
			{
				return (float)(InIndex * Screen.width) * (1f - Track.Clip * 2f) / (float)this.maxSampling;
			}
			return (float)(InIndex * Screen.width) * (1f - Track.Clip * 2f) / (float)this.samples.Count;
		}

		private float CalcMax(float InMax)
		{
			return (!this.bFixedRange) ? InMax : this.fixedMaxSampleValue;
		}

		private float CalcMin(float InMin)
		{
			return (!this.bFixedRange) ? InMin : this.fixedMinSampleValue;
		}

		public void OnRender(float InMinValue, float InMaxValue)
		{
			float num = this.CalcMax(InMaxValue) - this.CalcMin(InMinValue);
			Vector2 inStart = default(Vector2);
			for (int i = 0; i < this.samples.Count; i++)
			{
				float num2 = this.samples[i] - this.CalcMin(InMinValue);
				float inPercent = num2 / num;
				if (i == 0)
				{
					inStart = new Vector2(this.CalcX(i), this.CalcY(inPercent));
				}
				else
				{
					Vector2 vector = new Vector2(this.CalcX(i), this.CalcY(inPercent));
					Track.DrawLine(inStart, vector, this.drawColor, 1f);
					inStart = vector;
				}
			}
		}

		public void OnRender()
		{
			this.OnRender(this.minValue, this.maxValue);
		}

		public static void DrawLine(Vector2 InStart, Vector2 InEnd, Color InColor, float InWidth)
		{
			Drawing.DrawLine(new Vector2(InStart.x + (float)Screen.width * Track.Clip, (float)Screen.height * (1f - Track.Clip) - InStart.y), new Vector2(InEnd.x + (float)Screen.width * Track.Clip, (float)Screen.height * (1f - Track.Clip) - InEnd.y), InColor, InWidth, true);
		}
	}
}
