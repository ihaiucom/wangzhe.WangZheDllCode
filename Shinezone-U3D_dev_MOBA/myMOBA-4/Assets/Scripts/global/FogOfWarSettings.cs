using System;
using UnityEngine;

public class FogOfWarSettings : MonoBehaviour
{
	public static Color defaultColor = new Color(0.168627456f, 0.105882354f, 0.360784322f);

	public static float defaultFadeDistance = 20f;

	public static float defaultFadeThreshold = 0.5f;

	public static float defaultIntensity = 0.4f;

	public static float defaultFowIntensity = 0.6f;

	public Color color = FogOfWarSettings.defaultColor;

	public float fadeDistance = FogOfWarSettings.defaultFadeDistance;

	public float fadeThreshold = FogOfWarSettings.defaultFadeThreshold;

	public float intensity = FogOfWarSettings.defaultIntensity;

	public float fowIntensity = FogOfWarSettings.defaultFowIntensity;

	public static void SetDefault()
	{
		FogOfWarSettings.Apply(FogOfWarSettings.defaultColor, FogOfWarSettings.defaultFadeDistance, FogOfWarSettings.defaultFadeThreshold, FogOfWarSettings.defaultIntensity, FogOfWarSettings.defaultFowIntensity);
	}

	private static void Apply(Color _color, float _distance, float _threshold, float _intensity, float _fowIntensity)
	{
		Vector4 vec = default(Vector4);
		vec.x = _color.r;
		vec.y = _color.g;
		vec.z = _color.b;
		vec.w = 1f;
		Shader.SetGlobalVector("_FOWColor", vec);
		vec.x = 1f / Math.Max(0.01f, _distance);
		vec.y = -Mathf.Clamp01(_threshold);
		vec.z = _intensity;
		vec.w = _fowIntensity;
		Shader.SetGlobalVector("_FOWParams", vec);
	}

	public void ApplySettings()
	{
		FogOfWarSettings.Apply(this.color, this.fadeDistance, this.fadeThreshold, this.intensity, this.fowIntensity);
	}

	private void Start()
	{
		this.ApplySettings();
	}
}
