using System;
using UnityEngine;

public class PlaneShadowSettings : MonoBehaviour
{
	public static float defaultIntensity = 0.45f;

	public static float defaultFadeBegin = 0.1f;

	public static float defaultFadeExp = 4f;

	public static Vector3 defaultProjDir;

	public float _intensity = PlaneShadowSettings.defaultIntensity;

	public float _fadeBegin = PlaneShadowSettings.defaultFadeBegin;

	public float _fadeExp = PlaneShadowSettings.defaultFadeExp;

	public static Vector4 shadowParams;

	public static Vector3 shadowProjDir;

	static PlaneShadowSettings()
	{
		// Note: this type is marked as 'beforefieldinit'.
		Vector3 vector = new Vector3(1f, -1f, 1f);
		PlaneShadowSettings.defaultProjDir = vector.normalized;
		PlaneShadowSettings.shadowParams = new Vector4(PlaneShadowSettings.defaultFadeBegin, PlaneShadowSettings.defaultFadeExp, PlaneShadowSettings.defaultIntensity, 0f);
		PlaneShadowSettings.shadowProjDir = PlaneShadowSettings.defaultProjDir;
	}

	public static void SetDefault()
	{
		PlaneShadowSettings.shadowParams = new Vector4(PlaneShadowSettings.defaultFadeBegin, PlaneShadowSettings.defaultFadeExp, PlaneShadowSettings.defaultIntensity, 0f);
		PlaneShadowSettings.shadowProjDir = PlaneShadowSettings.defaultProjDir;
		PlaneShadowSettings.Apply();
	}

	public static void Apply()
	{
		Shader.SetGlobalVector("_ShadowProjDir", PlaneShadowSettings.shadowProjDir.toVec4(1f));
		Shader.SetGlobalVector("_ShadowFadeParams", PlaneShadowSettings.shadowParams);
	}

	public void ApplySettings()
	{
		this._fadeBegin = Mathf.Clamp01(this._fadeBegin);
		this._fadeExp = Mathf.Clamp(this._fadeExp, 0.01f, 16f);
		this._intensity = Mathf.Clamp(this._intensity, 0.01f, 4f);
		PlaneShadowSettings.shadowProjDir = base.transform.forward.normalized;
		PlaneShadowSettings.shadowParams = new Vector4(this._fadeBegin, this._fadeExp, this._intensity);
		PlaneShadowSettings.Apply();
	}

	private void Start()
	{
		this.ApplySettings();
	}
}
