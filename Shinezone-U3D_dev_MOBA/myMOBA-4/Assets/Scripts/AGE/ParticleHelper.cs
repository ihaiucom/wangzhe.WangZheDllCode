using System;
using UnityEngine;

namespace AGE
{
	public class ParticleHelper
	{
		private static int _particleActiveNumber;

		public static void IncParticleActiveNumber()
		{
			ParticleHelper._particleActiveNumber++;
		}

		public static void DecParticleActiveNumber()
		{
			ParticleHelper._particleActiveNumber--;
			if (ParticleHelper._particleActiveNumber < 0)
			{
			}
		}

		public static int GetParticleActiveNumber()
		{
			return ParticleHelper._particleActiveNumber;
		}

		public static ParticleSystem[] Init(GameObject gameObj, Vector3 scaling)
		{
			ParticleSystem[] componentsInChildren = gameObj.GetComponentsInChildren<ParticleSystem>();
			if (componentsInChildren == null || componentsInChildren.Length == 0)
			{
				return null;
			}
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ParticleSystem particleSystem = componentsInChildren[i];
				particleSystem.startSize *= scaling.x;
				particleSystem.startLifetime *= scaling.y;
				particleSystem.startSpeed *= scaling.z;
				particleSystem.transform.localScale *= scaling.x;
				if (!particleSystem.playOnAwake)
				{
					particleSystem.playOnAwake = true;
					particleSystem.Play();
				}
			}
			return componentsInChildren;
		}
	}
}
