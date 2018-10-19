using Assets.Scripts.Framework;
using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class OutlineFilter : MonoBehaviour
{
	private Material material;

	private Material clearAlphaMat;

	public int filterType = 1;

	[HideInInspector]
	[NonSerialized]
	public Camera particlesCam;

	public bool clearAlpha;

	public float blendFactor = 0.5f;

	private static bool s_isRenderingParticles;

	public static void EnableSurfaceShaderOutline(bool enable)
	{
		Shader.SetGlobalFloat("_SGamelGlobalAlphaModifier", (!enable) ? 1f : 0f);
	}

	public void LoadShaders()
	{
		if (!this.material)
		{
			string name = "SGame_Post/OutlineFilter";
			Shader shader = Shader.Find(name);
			if (shader == null)
			{
			}
			this.material = new Material(shader);
			this.material.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!this.clearAlphaMat)
		{
			string name2 = "SGame_Post/ClearAlpha";
			Shader shader2 = Shader.Find(name2);
			if (shader2 == null)
			{
			}
			this.clearAlphaMat = new Material(shader2);
			this.clearAlphaMat.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	public void UpdateFilterType(bool heroShow)
	{
		int num = Mathf.Max(Screen.width, Screen.height);
		int num2 = Mathf.Min(Screen.width, Screen.height);
		bool flag = num >= 1600 || num2 >= 900 || Screen.dpi >= 350f;
		bool flag2 = num >= 1136 || num2 >= 640 || Screen.dpi >= 300f;
		if (GameSettings.ShouldReduceResolution())
		{
			flag = false;
			flag2 = true;
		}
		if (flag)
		{
			this.filterType = 2;
			this.blendFactor = 0.55f;
		}
		else if (flag2)
		{
			this.filterType = 1;
			this.blendFactor = 0.5f;
		}
		else
		{
			this.filterType = 0;
			this.blendFactor = 0.5f;
		}
		if (heroShow)
		{
			this.filterType++;
		}
		if (GameSettings.SupportHDMode() && !GameSettings.EnableHDMode)
		{
			this.filterType = Mathf.Max(0, this.filterType - 1);
		}
		this.LoadShaders();
		this.UpdateParameters();
	}

	private void Start()
	{
		this.LoadShaders();
		this.UpdateParameters();
	}

	public void UpdateParameters()
	{
		if (!this.material || !Camera.main)
		{
			return;
		}
		this.material.SetFloat("_BlendFactor", this.blendFactor);
		float num = 1f / Camera.main.pixelWidth;
		float num2 = 1f / Camera.main.pixelHeight;
		if (this.filterType == 0)
		{
			Vector4 vector = new Vector4(-num, 0f, num, 0f);
			Vector4 vector2 = new Vector4(0f, -num2, 0f, num2);
			this.material.SetVector("_TexelOffset0", vector);
			this.material.SetVector("_TexelOffset1", vector2);
			this.material.DisableKeyword("_HIGHQUALITY_ON");
		}
		else if (this.filterType == 1)
		{
			Vector4 vector3 = new Vector4(-num, num2, num, num2);
			Vector4 vector4 = new Vector4(-num, -num2, num, -num2);
			this.material.SetVector("_TexelOffset0", vector3);
			this.material.SetVector("_TexelOffset1", vector4);
			this.material.DisableKeyword("_HIGHQUALITY_ON");
		}
		else if (this.filterType == 2)
		{
			num *= 2f;
			num2 *= 2f;
			Vector4 vector5 = new Vector4(-num, 0f, num, 0f);
			Vector4 vector6 = new Vector4(0f, -num2, 0f, num2);
			this.material.SetVector("_TexelOffset0", vector5);
			this.material.SetVector("_TexelOffset1", vector6);
			this.material.DisableKeyword("_HIGHQUALITY_ON");
		}
		else if (this.filterType == 3)
		{
			num *= 2f;
			num2 *= 2f;
			Vector4 vector7 = new Vector4(-num, num2, num, num2);
			Vector4 vector8 = new Vector4(-num, -num2, num, -num2);
			this.material.SetVector("_TexelOffset0", vector7);
			this.material.SetVector("_TexelOffset1", vector8);
			this.material.DisableKeyword("_HIGHQUALITY_ON");
		}
		else
		{
			Vector4 vector9 = new Vector4(-num, 0f, num, 0f);
			Vector4 vector10 = new Vector4(0f, -num2, 0f, num2);
			Vector4 vector11 = new Vector4(-num, num2, num, num2);
			Vector4 vector12 = new Vector4(-num, -num2, num, -num2);
			this.material.SetVector("_TexelOffset0", vector9);
			this.material.SetVector("_TexelOffset1", vector10);
			this.material.SetVector("_TexelOffset2", vector11);
			this.material.SetVector("_TexelOffset3", vector12);
			this.material.EnableKeyword("_HIGHQUALITY_ON");
		}
	}

	private void drawParticles(RenderTexture colorRT, RenderTexture depthRT)
	{
		if (!this.particlesCam)
		{
			return;
		}
		try
		{
			this.particlesCam.enabled = true;
			RenderTexture targetTexture = this.particlesCam.targetTexture;
			this.particlesCam.SetTargetBuffers(colorRT.colorBuffer, depthRT.depthBuffer);
			this.particlesCam.Render();
			this.particlesCam.targetTexture = targetTexture;
			this.particlesCam.enabled = false;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private void ClearAlpha()
	{
		if (this.clearAlphaMat == null)
		{
			return;
		}
		if (base.GetComponent<Camera>() == null)
		{
			return;
		}
		GL.PushMatrix();
		this.clearAlphaMat.SetPass(0);
		GL.LoadOrtho();
		GL.Viewport(base.GetComponent<Camera>().pixelRect);
		GL.Begin(7);
		GL.TexCoord2(0f, 0f);
		GL.Vertex3(0f, 0f, 0f);
		GL.TexCoord2(0f, 1f);
		GL.Vertex3(0f, 1f, 0f);
		GL.TexCoord2(1f, 1f);
		GL.Vertex3(1f, 1f, 0f);
		GL.TexCoord2(1f, 0f);
		GL.Vertex3(1f, 0f, 0f);
		GL.End();
		GL.PopMatrix();
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (OutlineFilter.s_isRenderingParticles)
		{
			return;
		}
		if (destination == null)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0);
			source.filterMode = FilterMode.Point;
			Graphics.Blit(source, temporary, this.material, 0);
			this.drawParticles(temporary, source);
			Graphics.Blit(temporary, destination);
			RenderTexture.ReleaseTemporary(temporary);
		}
		else
		{
			Graphics.Blit(source, destination, this.material, 0);
			this.drawParticles(destination, source);
		}
	}

	public static void EnableOutlineFilter()
	{
		int mask = LayerMask.GetMask(new string[]
		{
			"Scene"
		});
		int mask2 = LayerMask.GetMask(new string[]
		{
			"Particles"
		});
		Camera[] array = UnityEngine.Object.FindObjectsOfType<Camera>();
		for (int i = 0; i < array.Length; i++)
		{
			Camera camera = array[i];
			if ((camera.cullingMask & mask) != 0)
			{
				OutlineFilter outlineFilter = camera.gameObject.AddComponent<OutlineFilter>();
				camera.cullingMask &= ~mask2;
				Camera camera2 = new GameObject
				{
					transform = 
					{
						parent = camera.transform,
						localPosition = Vector3.zero,
						localRotation = Quaternion.identity
					},
					name = camera.name + " particles"
				}.AddComponent<Camera>();
				camera2.aspect = camera.aspect;
				camera2.backgroundColor = camera.backgroundColor;
				camera2.nearClipPlane = camera.nearClipPlane;
				camera2.farClipPlane = camera.farClipPlane;
				camera2.fieldOfView = camera.fieldOfView;
				camera2.orthographic = camera.orthographic;
				camera2.orthographicSize = camera.orthographicSize;
				camera2.pixelRect = camera.pixelRect;
				camera2.rect = camera.rect;
				camera2.clearFlags = CameraClearFlags.Nothing;
				camera2.cullingMask = mask2;
				camera2.enabled = false;
				outlineFilter.particlesCam = camera2;
				outlineFilter.UpdateFilterType(false);
			}
		}
	}

	public static void DisableOutlineFilter()
	{
		int mask = LayerMask.GetMask(new string[]
		{
			"Particles"
		});
		Camera[] array = UnityEngine.Object.FindObjectsOfType<Camera>();
		for (int i = 0; i < array.Length; i++)
		{
			Camera camera = array[i];
			if (camera)
			{
				OutlineFilter component = camera.GetComponent<OutlineFilter>();
				if (component)
				{
					UnityEngine.Object.Destroy(component);
					camera.cullingMask |= mask;
					Camera[] componentsInChildren = camera.GetComponentsInChildren<Camera>();
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						Camera camera2 = componentsInChildren[j];
						if (camera2 && !(camera2 == camera))
						{
							GameObject gameObject = camera2.gameObject;
							UnityEngine.Object.Destroy(camera2);
							UnityEngine.Object.Destroy(gameObject);
						}
					}
				}
			}
		}
	}
}
