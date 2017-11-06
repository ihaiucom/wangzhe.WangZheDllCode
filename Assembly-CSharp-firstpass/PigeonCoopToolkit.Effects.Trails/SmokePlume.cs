using System;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonCoopToolkit.Effects.Trails
{
	[AddComponentMenu("Pigeon Coop Toolkit/Effects/Smoke Plume")]
	public class SmokePlume : TrailRenderer_Base
	{
		public float TimeBetweenPoints = 0.1f;

		public Vector3 ConstantForce = Vector3.up * 0.5f;

		public float RandomForceScale = 0.05f;

		public int MaxNumberOfPoints = 50;

		private float _timeSincePoint;

		protected void OnEnable()
		{
			base.Start();
			base.ClearSystem(true);
			this._timeSincePoint = 0f;
		}

		protected override void OnStartEmit()
		{
			this._timeSincePoint = 0f;
		}

		protected override void Reset()
		{
			base.Reset();
			this.TrailData.SizeOverLife = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.5f, 0.2f),
				new Keyframe(1f, 0.2f)
			});
			this.TrailData.Lifetime = 6f;
			this.ConstantForce = Vector3.up * 0.5f;
			this.TimeBetweenPoints = 0.1f;
			this.RandomForceScale = 0.05f;
			this.MaxNumberOfPoints = 50;
		}

		protected override void Update()
		{
			if (this._emit)
			{
				this._timeSincePoint += (this._noDecay ? 0f : Time.deltaTime);
				if (this._timeSincePoint >= this.TimeBetweenPoints)
				{
					base.AddPoint(new SmokeTrailPoint(), this._t.position);
					this._timeSincePoint = 0f;
				}
			}
			base.Update();
		}

		protected override void InitialiseNewPoint(PCTrailPoint newPoint)
		{
			((SmokeTrailPoint)newPoint).RandomVec = Random.onUnitSphere * this.RandomForceScale;
		}

		protected override void UpdateTrail(PCTrail trail, float deltaTime)
		{
			if (this._noDecay)
			{
				return;
			}
			using (IEnumerator<PCTrailPoint> enumerator = trail.Points.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PCTrailPoint current = enumerator.get_Current();
					current.Position += this.ConstantForce * deltaTime;
				}
			}
		}

		protected override int GetMaxNumberOfPoints()
		{
			return this.MaxNumberOfPoints;
		}
	}
}
