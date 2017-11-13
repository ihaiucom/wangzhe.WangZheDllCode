using System;
using UnityEngine;

[AddComponentMenu("SGame/Effect/ParticleScaler"), ExecuteInEditMode]
public class ParticleScaler : MonoBehaviour, IPooledMonoBehaviour
{
	public float particleScale = 1f;

	public bool alsoScaleGameobject = true;

	private float prevScale = 1f;

	[HideInInspector]
	public bool scriptGenerated;

	private bool m_gotten;

	public void OnCreate()
	{
	}

	public void OnGet()
	{
		if (this.m_gotten)
		{
			return;
		}
		this.m_gotten = true;
		this.prevScale = this.particleScale;
		if (this.scriptGenerated && this.particleScale != 1f)
		{
			this.prevScale = 1f;
			this.CheckAndApplyScale();
		}
	}

	public void OnRecycle()
	{
		this.m_gotten = false;
	}

	private void Start()
	{
		this.OnGet();
	}

	public void CheckAndApplyScale()
	{
		if (this.prevScale != this.particleScale && this.particleScale > 0f)
		{
			if (this.alsoScaleGameobject)
			{
				base.transform.localScale = new Vector3(this.particleScale, this.particleScale, this.particleScale);
			}
			float scaleFactor = this.particleScale / this.prevScale;
			this.ScaleLegacySystems(scaleFactor);
			this.ScaleShurikenSystems(scaleFactor);
			this.ScaleTrailRenderers(scaleFactor);
			this.prevScale = this.particleScale;
		}
	}

	private void Update()
	{
	}

	private void ScaleShurikenSystems(float scaleFactor)
	{
		ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>(true);
		ParticleSystem[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			ParticleSystem particleSystem = array[i];
			particleSystem.startSpeed *= scaleFactor;
			particleSystem.startSize *= scaleFactor;
			particleSystem.gravityModifier *= scaleFactor;
		}
	}

	private void ScaleLegacySystems(float scaleFactor)
	{
		ParticleEmitter[] componentsInChildren = base.GetComponentsInChildren<ParticleEmitter>(true);
		ParticleAnimator[] componentsInChildren2 = base.GetComponentsInChildren<ParticleAnimator>(true);
		ParticleEmitter[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			ParticleEmitter particleEmitter = array[i];
			particleEmitter.minSize *= scaleFactor;
			particleEmitter.maxSize *= scaleFactor;
			particleEmitter.worldVelocity *= scaleFactor;
			particleEmitter.localVelocity *= scaleFactor;
			particleEmitter.rndVelocity *= scaleFactor;
		}
		ParticleAnimator[] array2 = componentsInChildren2;
		for (int j = 0; j < array2.Length; j++)
		{
			ParticleAnimator particleAnimator = array2[j];
			particleAnimator.force *= scaleFactor;
			particleAnimator.rndForce *= scaleFactor;
		}
	}

	private void ScaleTrailRenderers(float scaleFactor)
	{
		TrailRenderer[] componentsInChildren = base.GetComponentsInChildren<TrailRenderer>(true);
		TrailRenderer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			TrailRenderer trailRenderer = array[i];
			trailRenderer.startWidth *= scaleFactor;
			trailRenderer.endWidth *= scaleFactor;
		}
	}
}
