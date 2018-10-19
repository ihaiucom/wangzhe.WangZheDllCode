using System;
using System.Collections.Generic;
using UnityEngine;

namespace AGE
{
	public class CurveContainer
	{
		public struct Curve
		{
			public struct Point
			{
				public float time;

				public float value;

				public float slope;
			}

			public ListView<CurveContainer.Curve.Point> points;

			public readonly string name;

			public Curve(string _name)
			{
				this.name = _name;
				this.points = new ListView<CurveContainer.Curve.Point>();
			}

			private float LocatePoint(float _curTime)
			{
				int count = this.points.Count;
				if (_curTime < 0f)
				{
					_curTime = 0f;
				}
				else if (_curTime > 1f)
				{
					_curTime = 1f;
				}
				int num = 0;
				int num2 = this.points.Count - 1;
				while (num != num2)
				{
					int num3 = (num + num2) / 2 + 1;
					if (_curTime < this.points[num3].time)
					{
						num2 = num3 - 1;
					}
					else
					{
						num = num3;
					}
				}
				int num4;
				if (num == 0 && _curTime < this.points[0].time)
				{
					num4 = -1;
				}
				else
				{
					num4 = num;
				}
				float result;
				if (num4 < 0)
				{
					result = -1f + _curTime / this.points[0].time;
				}
				else if (num4 == count - 1)
				{
					result = (float)(count - 1) + (_curTime - this.points[count - 1].time) / (1f - this.points[count - 1].time);
				}
				else
				{
					result = (float)num4 + (_curTime - this.points[num4].time) / (this.points[num4 + 1].time - this.points[num4].time);
				}
				return result;
			}

			public float Sample(float _curTime)
			{
				if (this.points.Count == 0)
				{
					return 0f;
				}
				float num = this.LocatePoint(_curTime);
				if (num < 0f)
				{
					return this.points[0].value;
				}
				if (num >= (float)(this.points.Count - 1))
				{
					return this.points[this.points.Count - 1].value;
				}
				int num2 = (int)num;
				float num3 = (_curTime - this.points[num2].time) / (this.points[num2 + 1].time - this.points[num2].time);
				if (Math.Abs(this.points[num2].slope) < 1000f && Math.Abs(this.points[num2 + 1].slope) < 1000f)
				{
					float num4 = (_curTime - this.points[num2].time) * this.points[num2].slope + this.points[num2].value;
					float num5 = (_curTime - this.points[num2 + 1].time) * this.points[num2 + 1].slope + this.points[num2 + 1].value;
					float num6 = (this.points[num2].slope * 2.3f + this.points[num2 + 1].slope * 1f) / 3.3f;
					float num7 = (this.points[num2].slope * 1f + this.points[num2 + 1].slope * 2.3f) / 3.3f;
					float num8 = (_curTime - this.points[num2].time) * num6 + this.points[num2].value;
					float num9 = (_curTime - this.points[num2 + 1].time) * num7 + this.points[num2 + 1].value;
					float num10 = num3;
					float num11 = 1f - num10;
					return num4 * num11 * num11 * num11 + num8 * num11 * num11 * num10 * 3f + num9 * num11 * num10 * num10 * 3f + num5 * num10 * num10 * num10;
				}
				return this.points[num2].slope;
			}

			public int AddPoint(float _time, float _value, float _slope, bool _addDirectly)
			{
				CurveContainer.Curve.Point item = default(CurveContainer.Curve.Point);
				item.time = _time;
				item.value = _value;
				item.slope = _slope;
				int num = 0;
				if (this.points.Count == 0 || _addDirectly)
				{
					this.points.Add(item);
				}
				else
				{
					float num2 = this.LocatePoint(_time);
					num = (int)(num2 + 1f);
					if (num > this.points.Count)
					{
						num = this.points.Count;
					}
					this.points.Insert(num, item);
				}
				return num;
			}
		}

		protected const float maxSlope = 1000f;

		protected List<CurveContainer.Curve> curves;

		private CurveContainer(string[] _curveNames)
		{
			for (int i = 0; i < _curveNames.Length; i++)
			{
				string name = _curveNames[i];
				this.AddCurve(name);
			}
		}

		public float[] SampleCurves(float _curTime)
		{
			float[] array = new float[this.curves.Count];
			for (int i = 0; i < this.curves.Count; i++)
			{
				array[i] = this.curves[i].Sample(_curTime);
			}
			return array;
		}

		public void AddCurve(string _name)
		{
			this.curves.Add(new CurveContainer.Curve(_name));
		}

		public float SampleFloat(float _time)
		{
			float[] array = this.SampleCurves(_time);
			return array[0];
		}

		public Vector2 SampleVector2(float _time)
		{
			float[] array = this.SampleCurves(_time);
			return new Vector2(array[0], array[1]);
		}

		public Vector3 SampleVector3(float _time)
		{
			float[] array = this.SampleCurves(_time);
			return new Vector3(array[0], array[1], array[2]);
		}

		public Quaternion SampleEulerAngle(float _time)
		{
			float[] array = this.SampleCurves(_time);
			return Quaternion.Euler(array[0], array[1], array[2]);
		}

		public Color SampleColor(float _time)
		{
			float[] array = this.SampleCurves(_time);
			return new Color(array[0], array[1], array[2], array[3]);
		}
	}
}
