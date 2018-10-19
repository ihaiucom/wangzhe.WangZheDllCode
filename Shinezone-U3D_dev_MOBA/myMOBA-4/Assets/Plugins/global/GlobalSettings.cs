using System;
using UnityEngine;

[ExecuteInEditMode]
public class GlobalSettings : MonoBehaviour
{
	public bool m_bFog;

	public Color m_FogColor;

	public FogMode m_FogMode;

	public float m_FogDensity;

	public float m_LinearFogStart;

	public float m_LinearFogEnd;

	public Color m_AmbientLight = new Color(0.4f, 0.4f, 0.4f);

	private void Start()
	{
		this.ApplySetting();
	}

	private void Update()
	{
	}

	public void ApplySetting()
	{
		RenderSettings.fog = this.m_bFog;
		RenderSettings.fogColor = this.m_FogColor;
		RenderSettings.fogMode = this.m_FogMode;
		RenderSettings.fogDensity = this.m_FogDensity;
		RenderSettings.fogStartDistance = this.m_LinearFogStart;
		RenderSettings.fogEndDistance = this.m_LinearFogEnd;
		RenderSettings.ambientLight = this.m_AmbientLight;
	}
}
