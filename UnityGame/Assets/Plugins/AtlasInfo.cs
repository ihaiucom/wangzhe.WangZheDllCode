using System;
using UnityEngine;

public class AtlasInfo : ScriptableObject
{
	[Serializable]
	public class UVDetail
	{
		public Vector2 uvTL;

		public Vector2 uvTR;

		public Vector2 uvBL;

		public Vector2 uvBR;

		public bool rotate;

		public string Name;

		public int x;

		public int y;

		public int width;

		public int height;
	}

	public Texture2D texture;

	public Texture2D textureAlpha;

	public AtlasInfo.UVDetail[] uvDetails;

	public Material specialMaterial;

	[NonSerialized]
	private Material m_material;

	public Material material
	{
		get
		{
			if (null == this.m_material)
			{
				if (this.specialMaterial != null)
				{
					this.m_material = this.specialMaterial;
				}
				else
				{
					Shader shader = Singleton<CResourceManager>.GetInstance().GetResource("Shaders/UI/UI3D.shader", typeof(Shader), enResourceType.BattleScene, true, true).m_content as Shader;
					this.m_material = new Material(shader);
				}
				this.m_material.SetTexture("_MainTex", this.texture);
				if (null != this.textureAlpha)
				{
					this.m_material.SetTexture("_AlphaTex", this.textureAlpha);
					this.m_material.EnableKeyword("_SEPERATE_ALPHA_TEX_ON");
				}
			}
			return this.m_material;
		}
	}

	public AtlasInfo.UVDetail GetUV(string atlasName)
	{
		if (string.IsNullOrEmpty(atlasName))
		{
			return null;
		}
		for (int i = 0; i < this.uvDetails.Length; i++)
		{
			if (this.uvDetails[i].Name == atlasName)
			{
				return this.uvDetails[i];
			}
		}
		return null;
	}
}
