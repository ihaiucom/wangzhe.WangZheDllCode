using Assets.Scripts.Framework;
using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DynamicShadow : MonoBehaviour
{
	private Camera depthCam_;

	private RenderTexture[] depthTextures_;

	private Shader depthShader_;

	private Material blurMat_;

	private int shadowSize = 1024;

	private bool useDepthTex;

	private bool supportShadow;

	public float ShadowIntensity = 0.5f;

	public LayerMask ShadowCastersMask = -1;

	private static ListView<DynamicShadow> shadowList = new ListView<DynamicShadow>();

	private bool SupportHighpFloat()
	{
		string text = SystemInfo.graphicsDeviceName.ToLower();
		return text.Contains("adreno") || (text.Contains("powervr") || text.Contains("sgx")) || (text.Contains("nvidia") || text.Contains("tegra")) || (text.Contains("mali") || text.Contains("arm"));
	}

	private bool SupportRGBABilinear()
	{
		string text = SystemInfo.graphicsDeviceName.ToLower();
		return text.Contains("adreno") || (!text.Contains("powervr") && !text.Contains("sgx") && !text.Contains("nvidia") && !text.Contains("tegra") && (text.Contains("mali") || text.Contains("arm")));
	}

	public static void InitDefaultGlobalVariables()
	{
		Shader.DisableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
		Shader.SetGlobalVector("_SGameShadowParams", new Vector4(-0.486254424f, -0.270757377f, 0.8308111f, 0.5f));
		Shader.SetGlobalTexture("_SGameShadowTexture", null);
	}

	public static void EnableDynamicShow(GameObject go, bool enable)
	{
		if (go == null)
		{
			return;
		}
		if (enable && !GameSettings.IsHighQuality)
		{
			return;
		}
		DynamicShadow[] componentsInChildren = go.GetComponentsInChildren<DynamicShadow>();
		if (componentsInChildren == null || componentsInChildren.Length == 0)
		{
			return;
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			DynamicShadow dynamicShadow = componentsInChildren[i];
			if (enable)
			{
				dynamicShadow.InitShadow();
				if (!DynamicShadow.shadowList.Contains(dynamicShadow))
				{
					DynamicShadow.shadowList.Add(dynamicShadow);
				}
			}
			else
			{
				dynamicShadow.CloseShadow();
				DynamicShadow.shadowList.Remove(dynamicShadow);
			}
		}
		if (enable)
		{
			Shader.EnableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
		}
		else if (DynamicShadow.shadowList.Count == 0)
		{
			Shader.DisableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
		}
	}

	public static void DisableAllDynamicShadows()
	{
		for (int i = 0; i < DynamicShadow.shadowList.Count; i++)
		{
			DynamicShadow dynamicShadow = DynamicShadow.shadowList[i];
			if (dynamicShadow)
			{
				dynamicShadow.CloseShadow();
			}
		}
		DynamicShadow.shadowList.Clear();
		Shader.DisableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
	}

	private void InitShadow()
	{
		if (this.depthTextures_ != null)
		{
			return;
		}
		this.depthShader_ = Shader.Find("SGame_Post/ShadowDepth");
		this.blurMat_ = new Material(Shader.Find("SGame_Post/ShadowBlur"));
		this.useDepthTex = false;
		this.supportShadow = false;
		this.supportShadow = true;
		int num = 1;
		if (this.supportShadow)
		{
			Shader.EnableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
			RenderTextureFormat format = (!this.useDepthTex) ? RenderTextureFormat.ARGB32 : RenderTextureFormat.Depth;
			this.depthTextures_ = new RenderTexture[num];
			bool flag = this.SupportRGBABilinear();
			for (int i = 0; i < this.depthTextures_.Length; i++)
			{
				this.depthTextures_[i] = new RenderTexture(this.shadowSize, this.shadowSize, 24, format, RenderTextureReadWrite.Default);
				this.depthTextures_[i].generateMips = false;
				this.depthTextures_[i].filterMode = ((!flag) ? FilterMode.Point : FilterMode.Bilinear);
			}
			this.depthCam_ = base.GetComponent<Camera>();
			if (this.depthCam_ == null)
			{
				this.depthCam_ = base.gameObject.AddComponent<Camera>();
				this.depthCam_.transform.localPosition = new Vector3(0f, 0f, -5f);
			}
			this.depthCam_.enabled = true;
			this.depthCam_.targetTexture = this.depthTextures_[0];
			this.depthCam_.depth = -50f;
			this.depthCam_.farClipPlane = 100f;
			this.depthCam_.backgroundColor = new Color(1f, 1f, 1f, 1f);
			this.depthCam_.cullingMask = this.ShadowCastersMask;
			this.depthCam_.clearFlags = CameraClearFlags.Color;
			this.depthCam_.SetReplacementShader(this.depthShader_, "RenderType");
			return;
		}
		Shader.DisableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
	}

	private void CloseShadow()
	{
		if (this.depthTextures_ != null)
		{
			for (int i = 0; i < this.depthTextures_.Length; i++)
			{
				this.depthTextures_[i].Release();
				this.depthTextures_[i] = null;
			}
		}
		this.depthTextures_ = null;
		if (this.depthCam_)
		{
			this.depthCam_.enabled = false;
		}
		this.depthCam_ = null;
		Shader.SetGlobalTexture("_SGameShadowTexture", null);
	}

	private void Start()
	{
	}

	private void LateUpdate()
	{
		if (this.depthCam_ == null || this.depthTextures_ == null || !this.supportShadow)
		{
			return;
		}
		Matrix4x4 matrix4x = GL.GetGPUProjectionMatrix(this.depthCam_.projectionMatrix, false);
		matrix4x *= this.depthCam_.worldToCameraMatrix;
		Vector4 vec = base.transform.forward.normalized.toVec4(this.ShadowIntensity);
		Shader.SetGlobalMatrix("_SGameShadowMatrix", matrix4x);
		Shader.SetGlobalVector("_SGameShadowParams", vec);
	}

	private void OnDestroy()
	{
		this.CloseShadow();
	}

	private void OnPreRender()
	{
		Shader.SetGlobalTexture("_SGameShadowTexture", null);
	}

	private void OnPostRender()
	{
		if (this.depthCam_ == null || this.depthTextures_ == null || !this.supportShadow)
		{
			return;
		}
		Shader.SetGlobalTexture("_SGameShadowTexture", this.depthTextures_[0]);
	}
}
