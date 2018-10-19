using PigeonCoopToolkit.Utillities;
using System;
using UnityEngine;

namespace PigeonCoopToolkit.Effects.Trails
{
	[AddComponentMenu("Pigeon Coop Toolkit/Effects/Smooth Trail")]
	public class SmoothTrail : TrailRenderer_Base
	{
		private class ControlPoint
		{
			public Vector3 p;

			public Vector3 forward;
		}

		public float MinControlPointDistance = 0.1f;

		public int MaxControlPoints = 15;

		public int PointsBetweenControlPoints = 4;

		private Vector3 _lastPosition;

		private float _distanceMoved;

		private CircularBuffer<SmoothTrail.ControlPoint> _controlPoints;

		protected void OnEnable()
		{
			base.Start();
			base.ClearSystem(true);
			this._lastPosition = this._t.position;
		}

		protected override void Update()
		{
			if (this._emit)
			{
				this._distanceMoved += Vector3.Distance(this._t.position, this._lastPosition);
				if (!Mathf.Approximately(this._distanceMoved, 0f) && this._distanceMoved >= this.MinControlPointDistance)
				{
					this.AddControlPoint(this._t.position);
					this._distanceMoved = 0f;
				}
				else
				{
					this._controlPoints[this._controlPoints.Count - 1].p = this._t.position;
					if (this.TrailData.UseForwardOverride)
					{
						this._controlPoints[this._controlPoints.Count - 1].forward = ((!this.TrailData.ForwardOverrideRelative) ? this.TrailData.ForwardOverride.normalized : this._t.TransformDirection(this.TrailData.ForwardOverride.normalized));
					}
				}
				this._lastPosition = this._t.position;
			}
			base.Update();
		}

		protected override void OnStartEmit()
		{
			this._lastPosition = this._t.position;
			this._distanceMoved = 0f;
			this._controlPoints = new CircularBuffer<SmoothTrail.ControlPoint>(this.MaxControlPoints);
			this._controlPoints.Add(new SmoothTrail.ControlPoint
			{
				p = this._lastPosition
			});
			if (this.TrailData.UseForwardOverride)
			{
				this._controlPoints[0].forward = ((!this.TrailData.ForwardOverrideRelative) ? this.TrailData.ForwardOverride.normalized : this._t.TransformDirection(this.TrailData.ForwardOverride.normalized));
			}
			base.AddPoint(new PCTrailPoint(), this._lastPosition);
			this.AddControlPoint(this._lastPosition);
		}

		protected override void UpdateTrail(PCTrail trail, float deltaTime)
		{
			if (!trail.IsActiveTrail)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < this._controlPoints.Count; i++)
			{
				trail.Points[num].Position = this._controlPoints[i].p;
				if (this.TrailData.UseForwardOverride)
				{
					trail.Points[num].Forward = this._controlPoints[i].forward;
				}
				num++;
				if (i < this._controlPoints.Count - 1)
				{
					float d = Vector3.Distance(this._controlPoints[i].p, this._controlPoints[i + 1].p) / 2f;
					Vector3 curveStartHandle;
					if (i == 0)
					{
						curveStartHandle = this._controlPoints[i].p + (this._controlPoints[i + 1].p - this._controlPoints[i].p).normalized * d;
					}
					else
					{
						curveStartHandle = this._controlPoints[i].p + (this._controlPoints[i + 1].p - this._controlPoints[i - 1].p).normalized * d;
					}
					int num2 = i + 1;
					Vector3 curveEndHandle;
					if (num2 == this._controlPoints.Count - 1)
					{
						curveEndHandle = this._controlPoints[num2].p + (this._controlPoints[num2 - 1].p - this._controlPoints[num2].p).normalized * d;
					}
					else
					{
						curveEndHandle = this._controlPoints[num2].p + (this._controlPoints[num2 - 1].p - this._controlPoints[num2 + 1].p).normalized * d;
					}
					PCTrailPoint pCTrailPoint = trail.Points[num - 1];
					PCTrailPoint pCTrailPoint2 = trail.Points[num - 1 + this.PointsBetweenControlPoints + 1];
					for (int j = 0; j < this.PointsBetweenControlPoints; j++)
					{
						float t = ((float)j + 1f) / ((float)this.PointsBetweenControlPoints + 1f);
						trail.Points[num].Position = this.GetPointAlongCurve(this._controlPoints[i].p, curveStartHandle, this._controlPoints[i + 1].p, curveEndHandle, t, 0.3f);
						trail.Points[num].SetTimeActive(Mathf.Lerp(pCTrailPoint.TimeActive(), pCTrailPoint2.TimeActive(), t));
						if (this.TrailData.UseForwardOverride)
						{
							trail.Points[num].Forward = Vector3.Lerp(pCTrailPoint.Forward, pCTrailPoint2.Forward, t);
						}
						num++;
					}
				}
			}
			int num3 = this._controlPoints.Count - 1 + (this._controlPoints.Count - 1) * this.PointsBetweenControlPoints;
			int num4 = num3 - this.PointsBetweenControlPoints - 1;
			int num5 = num3 + 1;
			float num6 = trail.Points[num4].GetDistanceFromStart();
			for (int k = num4 + 1; k < num5; k++)
			{
				num6 += Vector3.Distance(trail.Points[k - 1].Position, trail.Points[k].Position);
				trail.Points[k].SetDistanceFromStart(num6);
			}
		}

		protected override void Reset()
		{
			base.Reset();
			this.MinControlPointDistance = 0.1f;
			this.MaxControlPoints = 15;
			this.PointsBetweenControlPoints = 4;
		}

		protected override void OnTranslate(Vector3 t)
		{
			this._lastPosition += t;
			for (int i = 0; i < this._controlPoints.Count; i++)
			{
				this._controlPoints[i].p += t;
			}
		}

		private void AddControlPoint(Vector3 position)
		{
			for (int i = 0; i < this.PointsBetweenControlPoints; i++)
			{
				base.AddPoint(new PCTrailPoint(), position);
			}
			base.AddPoint(new PCTrailPoint(), position);
			SmoothTrail.ControlPoint controlPoint = new SmoothTrail.ControlPoint
			{
				p = position
			};
			if (this.TrailData.UseForwardOverride)
			{
				controlPoint.forward = ((!this.TrailData.ForwardOverrideRelative) ? this.TrailData.ForwardOverride.normalized : this._t.TransformDirection(this.TrailData.ForwardOverride.normalized));
			}
			this._controlPoints.Add(controlPoint);
		}

		protected override int GetMaxNumberOfPoints()
		{
			return this.MaxControlPoints + this.MaxControlPoints * this.PointsBetweenControlPoints;
		}

		public Vector3 GetPointAlongCurve(Vector3 curveStart, Vector3 curveStartHandle, Vector3 curveEnd, Vector3 curveEndHandle, float t, float crease)
		{
			float num = 1f - t;
			float num2 = Mathf.Pow(num, 3f);
			float num3 = Mathf.Pow(num, 2f);
			float num4 = 1f - crease;
			return (num2 * curveStart * num4 + 3f * num3 * t * curveStartHandle * crease + 3f * num * Mathf.Pow(t, 2f) * curveEndHandle * crease + Mathf.Pow(t, 3f) * curveEnd * num4) / (num2 * num4 + 3f * num3 * t * crease + 3f * num * Mathf.Pow(t, 2f) * crease + Mathf.Pow(t, 3f) * num4);
		}
	}
}
