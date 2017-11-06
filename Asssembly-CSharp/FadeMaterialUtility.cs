using System;
using UnityEngine;

public class FadeMaterialUtility
{
	public const string FadeToken = " (Fade)";

	public const string FadeParamName = "_FadeFactor";

	public static ListView<Material> GetFadeMaterials(GameObject go)
	{
		if (go == null)
		{
			return null;
		}
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		if (componentsInChildren == null || componentsInChildren.Length == 0)
		{
			return null;
		}
		ListView<Material> listView = new ListView<Material>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Renderer renderer = componentsInChildren[i];
			if (renderer != null && renderer.sharedMaterial != null)
			{
				if (renderer.sharedMaterial.HasProperty("_FadeFactor"))
				{
					listView.Add(renderer.material);
				}
				else
				{
					Shader fadeShader = FadeMaterialUtility.GetFadeShader(renderer.sharedMaterial.shader, true);
					if (fadeShader != null)
					{
						renderer.material.shader = fadeShader;
						listView.Add(renderer.material);
					}
				}
			}
		}
		return (listView.Count > 0) ? listView : null;
	}

	public static Shader GetFadeShader(Shader shader, bool withFade)
	{
		if (shader == null)
		{
			return null;
		}
		if (withFade)
		{
			if (shader.name.Contains(" (Fade)"))
			{
				return shader;
			}
			string name = shader.name + " (Fade)";
			return Shader.Find(name);
		}
		else
		{
			int num = shader.name.IndexOf(" (Fade)");
			if (num != -1)
			{
				string name2 = shader.name.Remove(num, " (Fade)".get_Length());
				return Shader.Find(name2);
			}
			return shader;
		}
	}
}
