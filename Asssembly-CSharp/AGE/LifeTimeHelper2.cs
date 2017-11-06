using System;
using UnityEngine;

namespace AGE
{
	internal class LifeTimeHelper2 : MonoBehaviour
	{
		public ParticleSystem[] particleSys;

		private void Update()
		{
			if (this.particleSys == null)
			{
				Object.Destroy(this);
				return;
			}
			for (int i = 0; i < this.particleSys.Length; i++)
			{
				ParticleSystem particleSystem = this.particleSys[i];
				if (particleSystem != null && !particleSystem.isStopped && particleSystem.IsAlive())
				{
					return;
				}
			}
			ActionManager.DestroyGameObject(base.gameObject);
		}
	}
}
