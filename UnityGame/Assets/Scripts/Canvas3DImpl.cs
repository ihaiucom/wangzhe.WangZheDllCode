using System;
using System.Collections.Generic;
using UnityEngine;

public class Canvas3DImpl : Singleton<Canvas3DImpl>
{
	private class AutoAtlasInfo
	{
		private static int[] textureSize = new int[]
		{
			128,
			256,
			512,
			1024
		};

		private int counter = 1;

		private DictionaryView<int, AtlasInfo.UVDetail> textures = new DictionaryView<int, AtlasInfo.UVDetail>();

		public bool needRebuildAtlas;

		private bool needCompress;

		private int padding;

		private HashSet<Sprite3D> sprites = new HashSet<Sprite3D>();

		public Texture2D atlas;

		public Texture2D altasAlpha;

		private Material mat;

		private Dictionary<int, Texture2D> waitForCombineTextures = new Dictionary<int, Texture2D>();

		public void Register(Sprite3D sprite)
		{
			if (null == sprite || null == sprite.texture)
			{
				return;
			}
			int nativeTextureID = sprite.texture.GetNativeTextureID();
			sprite.m_textureGUID = nativeTextureID;
			AtlasInfo.UVDetail uVDetail = null;
			this.padding = Mathf.Max(this.padding, sprite.padding);
			this.needCompress |= sprite.compress;
			if (this.textures.TryGetValue(nativeTextureID, out uVDetail))
			{
				this.sprites.Add(sprite);
				if (null != this.mat)
				{
					sprite.SetMaterial(this.mat);
				}
				sprite.SetAutoAtlas(this.atlas, uVDetail);
				return;
			}
			uVDetail = new AtlasInfo.UVDetail();
			uVDetail.width = 0;
			uVDetail.height = 0;
			uVDetail.width = sprite.texture.width;
			uVDetail.height = sprite.texture.height;
			uVDetail.rotate = false;
			this.textures.Add(nativeTextureID, uVDetail);
			this.waitForCombineTextures.Add(nativeTextureID, sprite.texture);
			this.needRebuildAtlas = true;
			this.sprites.Add(sprite);
		}

		public void Unregister(Sprite3D sprite)
		{
			this.sprites.Remove(sprite);
			if (this.sprites.get_Count() == 0)
			{
				this.textures.Clear();
				if (this.mat != null)
				{
					Object.Destroy(this.mat);
				}
				this.mat = null;
				if (this.atlas != null)
				{
					Object.Destroy(this.atlas);
				}
				this.atlas = null;
			}
		}

		public void Rebuild()
		{
			this.needRebuildAtlas = false;
			bool flag = false;
			for (int i = 0; i < Canvas3DImpl.AutoAtlasInfo.textureSize.Length; i++)
			{
				if (!(this.atlas != null) || Canvas3DImpl.AutoAtlasInfo.textureSize[i] >= this.atlas.width)
				{
					flag = this.Pack(Canvas3DImpl.AutoAtlasInfo.textureSize[i]);
					if (flag)
					{
						break;
					}
				}
			}
			if (!flag)
			{
				HashSet<Sprite3D>.Enumerator enumerator = this.sprites.GetEnumerator();
				enumerator.MoveNext();
				Debug.LogError("Dynamic Combine Atlas Failed, maybe too many pictures of atlas tag:\"" + enumerator.get_Current().autoAtlasTag + "\"");
			}
		}

		private bool Pack(int size)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = this.padding;
			Vector2 zero = Vector2.zero;
			DictionaryView<int, AtlasInfo.UVDetail> dictionaryView = new DictionaryView<int, AtlasInfo.UVDetail>();
			DictionaryView<int, AtlasInfo.UVDetail>.Enumerator enumerator = this.textures.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, AtlasInfo.UVDetail> current = enumerator.Current;
				int width = current.get_Value().width;
				KeyValuePair<int, AtlasInfo.UVDetail> current2 = enumerator.Current;
				int height = current2.get_Value().height;
				AtlasInfo.UVDetail uVDetail = new AtlasInfo.UVDetail();
				uVDetail.rotate = false;
				DictionaryView<int, AtlasInfo.UVDetail> dictionaryView2 = dictionaryView;
				KeyValuePair<int, AtlasInfo.UVDetail> current3 = enumerator.Current;
				dictionaryView2.Add(current3.get_Key(), uVDetail);
				if (num3 + height + num4 <= size && num2 + width + num4 <= size)
				{
					uVDetail.x = num2;
					uVDetail.y = num3;
					uVDetail.width = width;
					uVDetail.height = height;
					num3 += height + num4;
					if (num < num2 + width + num4)
					{
						num = num2 + width + num4;
					}
				}
				else
				{
					if (num + width > size || height > size)
					{
						return false;
					}
					num2 = num;
					uVDetail.x = num2;
					uVDetail.y = 0;
					uVDetail.width = width;
					uVDetail.height = height;
					num3 = height + num4;
					num = num2 + width + num4;
				}
			}
			TextureFormat textureFormat = TextureFormat.ARGB32;
			if (this.needCompress)
			{
				textureFormat = TextureFormat.ARGB32;
			}
			Texture2D texture2D = new Texture2D(size, size, textureFormat, false);
			Color[] pixels = new Color[texture2D.width * texture2D.height];
			texture2D.SetPixels(pixels);
			texture2D.name = string.Concat(new object[]
			{
				"Auto_UI3D_Atlas_",
				size,
				"_",
				this.counter,
				"_format",
				textureFormat.ToString()
			});
			this.counter++;
			enumerator.Reset();
			while (enumerator.MoveNext())
			{
				Texture2D texture2D2 = null;
				Dictionary<int, Texture2D> dictionary = this.waitForCombineTextures;
				KeyValuePair<int, AtlasInfo.UVDetail> current4 = enumerator.Current;
				if (!dictionary.TryGetValue(current4.get_Key(), ref texture2D2))
				{
					texture2D2 = this.atlas;
				}
				KeyValuePair<int, AtlasInfo.UVDetail> current5 = enumerator.Current;
				AtlasInfo.UVDetail value = current5.get_Value();
				DictionaryView<int, AtlasInfo.UVDetail> dictionaryView3 = dictionaryView;
				KeyValuePair<int, AtlasInfo.UVDetail> current6 = enumerator.Current;
				AtlasInfo.UVDetail uVDetail2 = dictionaryView3[current6.get_Key()];
				Color[] pixels2 = texture2D2.GetPixels(value.x, value.y, value.width, value.height);
				texture2D.SetPixels(uVDetail2.x, uVDetail2.y, value.width, value.height, pixels2);
				texture2D.Apply(false, false);
				uVDetail2.uvTL = new Vector2((float)uVDetail2.x / (float)texture2D.width, (float)(uVDetail2.y + uVDetail2.height) / (float)texture2D.height);
				uVDetail2.uvTR = new Vector2((float)(uVDetail2.x + uVDetail2.width) / (float)texture2D.width, (float)(uVDetail2.y + uVDetail2.height) / (float)texture2D.height);
				uVDetail2.uvBL = new Vector2((float)uVDetail2.x / (float)texture2D.width, (float)uVDetail2.y / (float)texture2D.height);
				uVDetail2.uvBR = new Vector2((float)(uVDetail2.x + uVDetail2.width) / (float)texture2D.width, (float)uVDetail2.y / (float)texture2D.height);
			}
			this.textures = dictionaryView;
			Object.Destroy(this.atlas);
			this.atlas = texture2D;
			Shader shader = Singleton<CResourceManager>.GetInstance().GetResource("Shaders/UI/UI3D.shader", typeof(Shader), enResourceType.BattleScene, true, true).m_content as Shader;
			this.mat = new Material(shader);
			this.mat.SetTexture("_MainTex", this.atlas);
			HashSet<Sprite3D>.Enumerator enumerator2 = this.sprites.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				enumerator2.get_Current().SetMaterial(this.mat);
				enumerator2.get_Current().SetAutoAtlas(this.atlas, this.textures[enumerator2.get_Current().m_textureGUID]);
			}
			Dictionary<int, Texture2D>.Enumerator enumerator3 = this.waitForCombineTextures.GetEnumerator();
			this.waitForCombineTextures.Clear();
			Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
			return true;
		}
	}

	private DictionaryView<int, Sprite3D> m_childSprites = new DictionaryView<int, Sprite3D>(32);

	private DictionaryView<int, TextMesh> m_childText = new DictionaryView<int, TextMesh>(32);

	private DictionaryView<int, Mesh> m_childMesh = new DictionaryView<int, Mesh>(32);

	private int m_depth;

	private bool m_needRefreshLayout;

	private DictionaryView<string, Canvas3DImpl.AutoAtlasInfo> m_atlas = new DictionaryView<string, Canvas3DImpl.AutoAtlasInfo>();

	private bool m_needRebuildAtlas;

	public void Clear()
	{
		this.m_childSprites.Clear();
		this.m_childText.Clear();
		this.m_childMesh.Clear();
		Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
	}

	public void Reset()
	{
		this.m_childSprites.Clear();
		this.m_childText.Clear();
		this.m_childMesh.Clear();
		this.m_atlas.Clear();
		Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
	}

	public void Update(Transform root)
	{
		if (this.m_needRefreshLayout)
		{
			this._DoRefreshLayout(root);
			this.m_needRefreshLayout = false;
		}
		if (this.m_needRebuildAtlas)
		{
			this._DoRebuildAtlas();
			this.m_needRebuildAtlas = true;
		}
	}

	public void registerAutoAtlas(Sprite3D sprite)
	{
		if (sprite.texture == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(sprite.autoAtlasTag))
		{
			sprite.SetUV(new AtlasInfo.UVDetail
			{
				uvBL = new Vector2(0f, 0f),
				uvTL = new Vector2(0f, 1f),
				uvBR = new Vector2(1f, 0f),
				uvTR = new Vector2(1f, 1f),
				width = sprite.texture.width,
				height = sprite.texture.height
			});
			return;
		}
		Canvas3DImpl.AutoAtlasInfo autoAtlasInfo = null;
		if (!this.m_atlas.TryGetValue(sprite.autoAtlasTag, out autoAtlasInfo))
		{
			autoAtlasInfo = new Canvas3DImpl.AutoAtlasInfo();
			this.m_atlas.Add(sprite.autoAtlasTag, autoAtlasInfo);
		}
		autoAtlasInfo.Register(sprite);
		this.m_needRebuildAtlas = true;
	}

	public void unregisterAutoAtlas(Sprite3D sprite)
	{
		if (sprite.texture == null || string.IsNullOrEmpty(sprite.autoAtlasTag))
		{
			return;
		}
		Canvas3DImpl.AutoAtlasInfo autoAtlasInfo = null;
		if (!this.m_atlas.TryGetValue(sprite.autoAtlasTag, out autoAtlasInfo))
		{
			return;
		}
		autoAtlasInfo.Unregister(sprite);
	}

	private void _DoRebuildAtlas()
	{
		DictionaryView<string, Canvas3DImpl.AutoAtlasInfo>.Enumerator enumerator = this.m_atlas.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, Canvas3DImpl.AutoAtlasInfo> current = enumerator.Current;
			Canvas3DImpl.AutoAtlasInfo value = current.get_Value();
			if (value.needRebuildAtlas)
			{
				value.Rebuild();
			}
		}
	}

	public void registerSprite3D(Sprite3D sprite)
	{
		if (!this.m_childSprites.ContainsKey(sprite.transform.GetInstanceID()))
		{
			this.m_childSprites.Add(sprite.transform.GetInstanceID(), sprite);
		}
	}

	public void unregisterSprite3d(Sprite3D sprite)
	{
		this.m_childSprites.Remove(sprite.transform.GetInstanceID());
	}

	private void RefreshHierachy(Transform root)
	{
		if (!root.gameObject.activeSelf)
		{
			return;
		}
		for (int i = root.childCount - 1; i >= 0; i--)
		{
			this.RefreshHierachy(root.GetChild(i));
		}
		Sprite3D sprite3D = null;
		if (this.m_childSprites.TryGetValue(root.GetInstanceID(), out sprite3D))
		{
			if (null != sprite3D)
			{
				this.m_depth++;
				sprite3D.depth = (float)this.m_depth;
			}
		}
		else
		{
			sprite3D = root.GetComponent<Sprite3D>();
			this.m_childSprites.Add(root.GetInstanceID(), sprite3D);
			if (null != sprite3D)
			{
				this.m_depth++;
				sprite3D.depth = (float)this.m_depth;
			}
		}
		TextMesh textMesh = null;
		if (sprite3D == null)
		{
			if (this.m_childText.TryGetValue(root.GetInstanceID(), out textMesh))
			{
				if (null != textMesh)
				{
					this.m_depth++;
					textMesh.offsetZ = (float)this.m_depth / 10f;
				}
			}
			else
			{
				textMesh = root.GetComponent<TextMesh>();
				this.m_childText.Add(root.GetInstanceID(), textMesh);
				if (null != textMesh)
				{
					this.m_depth++;
					textMesh.offsetZ = (float)this.m_depth / 10f;
				}
			}
		}
		Mesh mesh = null;
		if (sprite3D == null && textMesh == null)
		{
			if (this.m_childMesh.TryGetValue(root.GetInstanceID(), out mesh))
			{
				if (null != mesh)
				{
					this.m_depth++;
					Bounds bounds = mesh.bounds;
					bounds.center = new Vector3(bounds.center.x, bounds.center.y, (float)this.m_depth / 10f);
				}
			}
			else
			{
				SkinnedMeshRenderer component = root.GetComponent<SkinnedMeshRenderer>();
				if (null != component)
				{
					mesh = component.sharedMesh;
				}
				else
				{
					MeshFilter component2 = root.GetComponent<MeshFilter>();
					if (null != component2)
					{
						mesh = component2.sharedMesh;
					}
				}
				this.m_childMesh.Add(root.GetInstanceID(), mesh);
				if (null != mesh)
				{
					this.m_depth++;
					Bounds bounds2 = mesh.bounds;
					bounds2.center = new Vector3(bounds2.center.x, bounds2.center.y, (float)this.m_depth / 10f);
				}
			}
		}
	}

	public void RebuildAtlasImmediately()
	{
		this._DoRebuildAtlas();
	}

	public void RefreshLayout(Transform root = null)
	{
		this.m_needRefreshLayout = true;
	}

	private void _DoRefreshLayout(Transform root)
	{
		this.m_depth = 0;
		this.RefreshHierachy(root);
	}
}
