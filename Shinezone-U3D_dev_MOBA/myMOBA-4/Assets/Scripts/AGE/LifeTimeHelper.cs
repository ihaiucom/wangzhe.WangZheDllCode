using System;
using UnityEngine;

namespace AGE
{
	public class LifeTimeHelper : MonoBehaviour
	{
		public float startTime;

		public bool checkParticleLife;

		private void Update()
		{
			if (this.checkParticleLife && base.GetComponent<ParticleSystem>().playOnAwake && base.GetComponent<ParticleSystem>() != null && (base.GetComponent<ParticleSystem>().isStopped || !base.GetComponent<ParticleSystem>().IsAlive()))
			{
				ActionManager.DestroyGameObject(base.gameObject);
			}
		}
	}
}
