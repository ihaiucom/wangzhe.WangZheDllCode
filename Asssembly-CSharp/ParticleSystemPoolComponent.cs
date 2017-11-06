using System;
using UnityEngine;

public class ParticleSystemPoolComponent : MonoBehaviour
{
	public struct ParticleSystemCache
	{
		public ParticleSystem par;

		public bool emmitState;
	}

	public ParticleSystemPoolComponent.ParticleSystemCache[] cache;
}
