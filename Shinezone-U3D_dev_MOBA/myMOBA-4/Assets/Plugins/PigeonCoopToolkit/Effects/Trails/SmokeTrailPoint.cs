using System;
using UnityEngine;

namespace PigeonCoopToolkit.Effects.Trails
{
	public class SmokeTrailPoint : PCTrailPoint
	{
		public Vector3 RandomVec;

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
			this.Position += this.RandomVec * deltaTime;
		}
	}
}
