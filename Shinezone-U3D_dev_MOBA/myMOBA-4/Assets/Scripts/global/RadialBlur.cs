using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class RadialBlur : MonoBehaviour
{
	private Material material;

	public Vector2 center = Vector2.zero;

	public float falloffExp = 1.5f;

	public float blurScale = 50f;

	public int numIterations = 2;

	public void LoadShaders()
	{
		if (!this.material)
		{
			string name = "SGame_Post/RadialBlur";
			Shader shader = Shader.Find(name);
			if (shader == null)
			{
			}
			this.material = new Material(shader);
			this.material.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	protected void Start()
	{
		this.LoadShaders();
		this.UpdateParameters();
	}

	public void UpdateParameters()
	{
		if (!this.material)
		{
			return;
		}
		this.material.SetVector("_ScreenCenter", new Vector4(this.center.x, this.center.y, 0f, 0f));
		this.material.SetFloat("_FalloffExp", this.falloffExp);
		this.material.SetFloat("_BlurScale", this.blurScale);
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.numIterations = Mathf.Clamp(this.numIterations, 1, 4);
		if (this.numIterations == 1)
		{
			Graphics.Blit(source, destination, this.material, 0);
		}
		else if (this.numIterations == 2)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height);
			Graphics.Blit(source, temporary, this.material, 0);
			Graphics.Blit(temporary, destination, this.material, 0);
			RenderTexture.ReleaseTemporary(temporary);
		}
		else
		{
			RenderTexture[] array = new RenderTexture[]
			{
				RenderTexture.GetTemporary(source.width, source.height),
				RenderTexture.GetTemporary(source.width, source.height)
			};
			int num = 0;
			RenderTexture source2 = source;
			for (int i = 0; i < this.numIterations - 1; i++)
			{
				Graphics.Blit(source2, array[num], this.material, 0);
				source2 = array[num];
				num = (num + 1) % 2;
			}
			Graphics.Blit(source2, destination, this.material, 0);
			RenderTexture.ReleaseTemporary(array[0]);
			RenderTexture.ReleaseTemporary(array[1]);
		}
	}
}
