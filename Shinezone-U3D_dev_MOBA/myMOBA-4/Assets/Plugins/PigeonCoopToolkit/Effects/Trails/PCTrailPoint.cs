using System;
using UnityEngine;

namespace PigeonCoopToolkit.Effects.Trails
{
	public class PCTrailPoint
	{
		public Vector3 Forward;

		public Vector3 Position;

		public int PointNumber;

		private float _timeActive;

		private float _distance;

		public virtual void Update(float deltaTime)
		{
			this._timeActive += deltaTime;
		}

		public float TimeActive()
		{
			return this._timeActive;
		}

		public void SetTimeActive(float time)
		{
			this._timeActive = time;
		}

		public void SetDistanceFromStart(float distance)
		{
			this._distance = distance;
		}

		public float GetDistanceFromStart()
		{
			return this._distance;
		}
	}
}
