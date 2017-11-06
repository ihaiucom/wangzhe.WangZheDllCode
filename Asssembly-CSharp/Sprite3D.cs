using System;
using UnityEngine;

public class Sprite3D : MonoBehaviour
{
	[Serializable]
	public enum EnumVertical
	{
		Top,
		Middle,
		Bottom
	}

	[Serializable]
	public enum EnumHoriontal
	{
		Left,
		Center,
		Right
	}

	[Serializable]
	public enum EnumFillType
	{
		Horiontal,
		Vertical,
		Radial360,
		RepeatHoriontal
	}

	public struct SpriteAttr
	{
		public int x;

		public int y;

		public int width;

		public int height;
	}

	public static readonly int TRANSPARENT_RENDER_QUEUE = 3000;

	[SerializeField]
	private bool m_useAtlas = true;

	[SerializeField]
	private AtlasInfo m_atlas;

	[SerializeField]
	private string m_spriteName;

	[NonSerialized]
	private string m_lastAtlasName;

	[SerializeField]
	private Texture2D m_texture;

	[SerializeField]
	private string m_tag;

	[SerializeField]
	public bool compress = true;

	[SerializeField]
	public int padding;

	[NonSerialized]
	private Sprite3D.SpriteAttr m_spriteAttr;

	[NonSerialized]
	public int m_textureGUID;

	[SerializeField]
	private float m_width = 1f;

	[SerializeField]
	private float m_height = 1f;

	[SerializeField]
	private Sprite3D.EnumVertical m_alignVertical = Sprite3D.EnumVertical.Middle;

	[SerializeField]
	private Sprite3D.EnumHoriontal m_alignHoriontal = Sprite3D.EnumHoriontal.Center;

	[SerializeField]
	private Sprite3D.EnumFillType m_fillType;

	[SerializeField]
	private float m_fillAmount = 1f;

	[SerializeField]
	private Color m_color = Color.white;

	[SerializeField]
	private float m_depth = 1f;

	[SerializeField]
	private uint m_segments = 50u;

	[SerializeField]
	private float m_repeatUnitWidth = 1f;

	[SerializeField]
	private float m_repeatUnitHeight = 1f;

	[SerializeField]
	private float m_repeatSpace = 1f;

	[SerializeField]
	private bool m_repeatSpaceFirst;

	[NonSerialized]
	private bool m_propchanged = true;

	[NonSerialized]
	private Mesh m_mesh;

	[NonSerialized]
	private MeshRenderer m_render;

	[NonSerialized]
	private AtlasInfo.UVDetail m_uv;

	private static float S_Ratio = -1f;

	public bool useAtlas
	{
		get
		{
			return this.m_useAtlas;
		}
		set
		{
			if (value == this.m_useAtlas)
			{
				return;
			}
			this.m_useAtlas = value;
			if (this.m_useAtlas)
			{
				this.m_texture = null;
			}
			else
			{
				this.m_atlas = null;
			}
		}
	}

	public float fillAmount
	{
		get
		{
			return this.m_fillAmount;
		}
		set
		{
			if (this.m_fillAmount == value)
			{
				return;
			}
			this.m_fillAmount = value;
			this.m_propchanged = true;
		}
	}

	public Color color
	{
		get
		{
			return this.m_color;
		}
		set
		{
			if (this.m_color == value)
			{
				return;
			}
			this.m_color = value;
			this.m_propchanged = true;
		}
	}

	public float depth
	{
		get
		{
			return this.m_depth;
		}
		set
		{
			this.m_depth = value;
			this.RecaculateDepth();
		}
	}

	public AtlasInfo atlas
	{
		get
		{
			return this.m_atlas;
		}
		set
		{
			if (this.m_atlas == value)
			{
				return;
			}
			this.m_atlas = value;
			this.useAtlas = true;
			this.m_propchanged = true;
		}
	}

	public string spriteName
	{
		get
		{
			return this.m_spriteName;
		}
		set
		{
			if (this.m_spriteName == value)
			{
				return;
			}
			this.m_spriteName = value;
			this.m_propchanged = true;
		}
	}

	public Texture2D texture
	{
		get
		{
			return this.m_texture;
		}
		set
		{
			if (this.m_texture == value)
			{
				return;
			}
			this.m_texture = value;
			this.useAtlas = false;
			Singleton<Canvas3DImpl>.GetInstance().registerAutoAtlas(this);
			this.m_propchanged = true;
		}
	}

	public int textureWidth
	{
		get
		{
			if (this.m_uv != null)
			{
				return this.m_uv.width;
			}
			if (this.m_texture != null)
			{
				return this.m_texture.width;
			}
			return 0;
		}
	}

	public int textureHeight
	{
		get
		{
			if (this.m_uv != null)
			{
				return this.m_uv.height;
			}
			if (this.m_texture != null)
			{
				return this.m_texture.height;
			}
			return 0;
		}
	}

	public float HalfTextureWidth
	{
		get
		{
			return (float)this.textureWidth * 0.5f;
		}
	}

	public float HalfTextureHeight
	{
		get
		{
			return (float)this.textureHeight * 0.5f;
		}
	}

	public string autoAtlasTag
	{
		get
		{
			if (string.IsNullOrEmpty(this.m_tag))
			{
				return this.m_tag;
			}
			return this.m_tag.Trim();
		}
		set
		{
			if (this.m_tag == value)
			{
				return;
			}
			this.m_tag = value;
			Singleton<Canvas3DImpl>.GetInstance().registerAutoAtlas(this);
			this.m_propchanged = true;
		}
	}

	public float width
	{
		get
		{
			return this.m_width;
		}
		set
		{
			if (this.m_width == value)
			{
				return;
			}
			this.m_width = value;
			this.m_propchanged = true;
		}
	}

	public float height
	{
		get
		{
			return this.m_height;
		}
		set
		{
			if (this.m_height == value)
			{
				return;
			}
			this.m_height = value;
			this.m_propchanged = true;
		}
	}

	public float repeatUnitWidth
	{
		get
		{
			return this.m_repeatUnitWidth;
		}
		set
		{
			if (this.m_repeatUnitWidth == value)
			{
				return;
			}
			this.m_repeatUnitWidth = value;
			this.m_propchanged = true;
		}
	}

	public float repeatUnitHeight
	{
		get
		{
			return this.m_repeatUnitHeight;
		}
		set
		{
			if (this.m_repeatUnitHeight == value)
			{
				return;
			}
			this.m_repeatUnitHeight = value;
			this.m_propchanged = true;
		}
	}

	public Sprite3D.EnumHoriontal alignHoriontal
	{
		get
		{
			return this.m_alignHoriontal;
		}
		set
		{
			if (this.m_alignHoriontal == value)
			{
				return;
			}
			this.m_alignHoriontal = value;
			this.m_propchanged = true;
		}
	}

	public Sprite3D.EnumVertical alignVertical
	{
		get
		{
			return this.m_alignVertical;
		}
		set
		{
			if (this.m_alignVertical == value)
			{
				return;
			}
			this.m_alignVertical = value;
			this.m_propchanged = true;
		}
	}

	public Sprite3D.EnumFillType fillType
	{
		get
		{
			return this.m_fillType;
		}
		set
		{
			if (this.m_fillType == value)
			{
				return;
			}
			this.m_fillType = value;
			this.m_propchanged = true;
		}
	}

	public uint segments
	{
		get
		{
			return this.m_segments;
		}
		set
		{
			if (this.m_segments == value)
			{
				return;
			}
			this.m_segments = value;
			this.m_propchanged = true;
		}
	}

	public float repeatSpace
	{
		get
		{
			return this.m_repeatSpace;
		}
		set
		{
			if (this.m_repeatSpace == value)
			{
				return;
			}
			this.m_repeatSpace = value;
			this.m_propchanged = true;
		}
	}

	public bool repeatSpaceFirst
	{
		get
		{
			return this.m_repeatSpaceFirst;
		}
		set
		{
			if (this.m_repeatSpaceFirst == value)
			{
				return;
			}
			this.m_repeatSpaceFirst = value;
			this.m_propchanged = true;
		}
	}

	private void Awake()
	{
		this.m_lastAtlasName = null;
		this.m_propchanged = true;
		this.m_depth = Mathf.Max(1f, this.m_depth);
		this.m_mesh = null;
		this.PrepareMesh();
		this.RefreshUVDetail();
	}

	private void Start()
	{
		Singleton<Canvas3DImpl>.GetInstance().registerSprite3D(this);
		Singleton<Canvas3DImpl>.GetInstance().registerAutoAtlas(this);
		this.RefreshAtlasMaterial();
	}

	private void OnDestroy()
	{
		Singleton<Canvas3DImpl>.GetInstance().unregisterSprite3d(this);
		Singleton<Canvas3DImpl>.GetInstance().unregisterAutoAtlas(this);
	}

	private void OnEnable()
	{
		Singleton<Canvas3DImpl>.GetInstance().RefreshLayout(null);
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		if (this.m_propchanged)
		{
			this.GenerateMesh();
			this.m_propchanged = false;
		}
	}

	public void SetAutoAtlas(Texture2D atlas, AtlasInfo.UVDetail uv)
	{
		this.m_texture = atlas;
		this.SetUV(uv);
	}

	public void RefreshAtlasMaterial()
	{
		if (this.m_atlas != null)
		{
			this.m_render.sharedMaterial = this.m_atlas.material;
		}
	}

	public void SetMaterial(Material mat)
	{
		Material sharedMaterial = this.m_render.sharedMaterial;
		this.m_render.sharedMaterial = mat;
		Object.DestroyObject(sharedMaterial);
	}

	public void RefreshAutoAtlasMaterial()
	{
		if (this.m_texture != null)
		{
			if (this.m_render.sharedMaterial == null)
			{
				Shader shader = Singleton<CResourceManager>.GetInstance().GetResource("Shaders/UI/UI3D.shader", typeof(Shader), enResourceType.BattleScene, true, true).m_content as Shader;
				Material material = new Material(shader);
				material.SetTexture("_MainTex", this.texture);
				this.m_render.sharedMaterial = material;
			}
			else
			{
				this.m_render.sharedMaterial.SetTexture("_MainTex", this.texture);
			}
		}
	}

	private void RefreshUVDetail()
	{
		if (null == this.m_atlas)
		{
			return;
		}
		if (this.m_lastAtlasName == this.m_spriteName)
		{
			return;
		}
		this.SetUV(this.m_atlas.GetUV(this.m_spriteName));
		this.m_lastAtlasName = this.m_spriteName;
	}

	public void SetUV(AtlasInfo.UVDetail uv)
	{
		this.PrepareMesh();
		if (this.m_mesh == null || this.m_mesh.triangles.Length == 0 || this.m_uv == null || this.m_uv.uvBL != uv.uvBL || this.m_uv.uvBR != uv.uvBR || this.m_uv.uvTL != uv.uvTL || this.m_uv.uvTR != uv.uvTR)
		{
			this.m_propchanged = true;
		}
		this.m_uv = uv;
		this.RefreshAutoAtlasMaterial();
	}

	private void RecaculateDepth()
	{
		if (this.m_mesh == null)
		{
			return;
		}
		Bounds bounds = default(Bounds);
		bounds.center = new Vector3(0.5f * this.m_width, 0.5f * this.m_height, this.m_depth / 10f);
		bounds.size = new Vector3(this.m_width, this.m_height, this.m_depth / 4f * 2f);
		this.m_mesh.bounds = bounds;
	}

	public void PrepareMesh()
	{
		if (this.m_mesh == null)
		{
			MeshFilter meshFilter = base.gameObject.GetComponent<MeshFilter>();
			if (null == meshFilter)
			{
				meshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			this.m_mesh = new Mesh();
			meshFilter.mesh = this.m_mesh;
			this.m_render = base.gameObject.GetComponent<MeshRenderer>();
			if (null == this.m_render)
			{
				this.m_render = base.gameObject.AddComponent<MeshRenderer>();
			}
		}
	}

	public void GenerateMesh()
	{
		this.RefreshUVDetail();
		if (this.m_uv == null)
		{
			return;
		}
		this.PrepareMesh();
		this.m_mesh.Clear();
		if (this.m_fillType == Sprite3D.EnumFillType.Horiontal)
		{
			this.GenerateHoriontalFillMesh();
		}
		else if (this.m_fillType == Sprite3D.EnumFillType.Vertical)
		{
			this.GenerateVerticalFillMesh();
		}
		else if (this.m_fillType == Sprite3D.EnumFillType.Radial360)
		{
			this.GenerateRadial360FillMesh();
		}
		else if (this.m_fillType == Sprite3D.EnumFillType.RepeatHoriontal)
		{
			this.GenerateRepeatHoriontalFillMesh();
		}
	}

	public Sprite GenerateSprite()
	{
		Sprite result = null;
		if (this.m_atlas == null)
		{
			return result;
		}
		AtlasInfo.UVDetail uV = this.m_atlas.GetUV(this.m_spriteName);
		if (uV == null)
		{
			return result;
		}
		if (this.m_useAtlas)
		{
			result = Sprite.Create(this.m_atlas.texture, new Rect((float)uV.x, (float)(this.m_atlas.texture.height - uV.height), (float)uV.width, (float)uV.height), Vector2.zero);
		}
		else
		{
			result = Sprite.Create(this.m_texture, new Rect(0f, 0f, (float)this.m_texture.width, (float)this.m_texture.height), Vector2.zero);
		}
		return result;
	}

	public void SetNativeSize(Camera camera, float depth)
	{
		float num = Sprite3D.Ratio();
		this.RefreshUVDetail();
		if (camera != null)
		{
			Vector3 vector = camera.transform.TransformPoint(0f, 0f, depth);
			Vector3 vector2 = camera.WorldToScreenPoint(vector);
			Vector3 position = new Vector3(vector2.x + (float)this.textureWidth * num, vector2.y, vector2.z);
			Vector3 position2 = new Vector3(vector2.x, vector2.y + (float)this.textureHeight * num, vector2.z);
			Vector3 b = camera.ScreenToWorldPoint(position);
			Vector3 b2 = camera.ScreenToWorldPoint(position2);
			if (this.fillType == Sprite3D.EnumFillType.RepeatHoriontal)
			{
				this.repeatUnitWidth = Vector3.Distance(vector, b);
				this.repeatUnitHeight = Vector3.Distance(vector, b2);
			}
			else
			{
				this.width = Vector3.Distance(vector, b);
				this.height = Vector3.Distance(vector, b2);
			}
		}
	}

	public void SetNativeSize(Camera camera, float depth, float screenWidth, float screenHeight)
	{
		if (camera != null)
		{
			Vector3 vector = camera.transform.TransformPoint(0f, 0f, depth);
			Vector3 vector2 = camera.WorldToScreenPoint(vector);
			Vector3 position = new Vector3(vector2.x + screenWidth, vector2.y, vector2.z);
			Vector3 position2 = new Vector3(vector2.x, vector2.y + screenHeight, vector2.z);
			Vector3 b = camera.ScreenToWorldPoint(position);
			Vector3 b2 = camera.ScreenToWorldPoint(position2);
			if (this.fillType == Sprite3D.EnumFillType.RepeatHoriontal)
			{
				this.repeatUnitWidth = Vector3.Distance(vector, b);
				this.repeatUnitHeight = Vector3.Distance(vector, b2);
			}
			else
			{
				this.width = Vector3.Distance(vector, b);
				this.height = Vector3.Distance(vector, b2);
			}
		}
	}

	public static float Ratio()
	{
		if (Sprite3D.S_Ratio == -1f)
		{
			int num = 960;
			int num2 = 640;
			Sprite3D.S_Ratio = Mathf.Min((float)Screen.height / (float)num2, (float)Screen.width / (float)num);
		}
		return Sprite3D.S_Ratio;
	}

	public static float SetRatio(int newWidth, int newHeight)
	{
		int num = 960;
		int num2 = 640;
		Sprite3D.S_Ratio = Mathf.Min((float)newHeight / (float)num2, (float)newWidth / (float)num);
		return Sprite3D.S_Ratio;
	}

	private void GenerateRadial360FillMesh()
	{
		this.m_fillAmount = Mathf.Clamp01(this.m_fillAmount);
		if (this.m_fillAmount <= 0f)
		{
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		this.m_mesh.MarkDynamic();
		float num = 0f;
		float num2 = 0f;
		if (this.m_alignHoriontal == Sprite3D.EnumHoriontal.Center)
		{
			num = -0.5f * this.m_width;
		}
		else if (this.m_alignHoriontal == Sprite3D.EnumHoriontal.Right)
		{
			num = -this.m_width;
		}
		if (this.m_alignVertical == Sprite3D.EnumVertical.Middle)
		{
			num2 = -0.5f * this.m_height;
		}
		else if (this.m_alignVertical == Sprite3D.EnumVertical.Top)
		{
			num2 = -this.m_height;
		}
		int num3 = (int)(this.m_segments * this.m_fillAmount) + 1;
		float num4 = 2f * (this.width + this.height) / this.m_segments;
		Vector3[] array = new Vector3[num3 + 1];
		Vector2[] array2 = new Vector2[num3 + 1];
		array[0] = new Vector3(num + 0.5f * this.m_width, num2 + 0.5f * this.height, 0f);
		array2[0] = this.m_uv.uvTL.Lerp(this.m_uv.uvBR, 0.5f);
		int num5 = 0;
		int num6 = 0;
		float num7 = 0f;
		for (int i = 0; i < num3; i++)
		{
			if (num5 == 0)
			{
				float num8 = num + 0.5f * this.width + (float)num6 * num4;
				if (num8 >= num + this.width)
				{
					num7 = num8 - num - this.width;
					num8 = num + this.width;
					num5 = 1;
					num6 = 1;
				}
				else
				{
					num6++;
				}
				array[i + 1] = new Vector3(num8, num2 + this.height, 0f);
			}
			else if (num5 == 1)
			{
				float num9 = num2 + this.height - (float)num6 * num4 - num7;
				if (num9 <= num2)
				{
					num7 = num2 - num9;
					num9 = num2;
					num5 = 2;
					num6 = 1;
				}
				else
				{
					num6++;
				}
				array[i + 1] = new Vector3(num + this.width, num9, 0f);
			}
			else if (num5 == 2)
			{
				float num10 = num + this.width - (float)num6 * num4 - num7;
				if (num10 <= num)
				{
					num7 = num - num10;
					num10 = num;
					num5 = 3;
					num6 = 1;
				}
				else
				{
					num6++;
				}
				array[i + 1] = new Vector3(num10, num2, 0f);
			}
			else if (num5 == 3)
			{
				float num11 = num2 + (float)num6 * num4 + num7;
				if (num11 >= num2 + this.height)
				{
					num7 = num11 - num2 - this.height;
					num11 = num2 + this.height;
					num5 = 4;
					num6 = 1;
				}
				else
				{
					num6++;
				}
				array[i + 1] = new Vector3(num, num11);
			}
			else if (num5 == 4)
			{
				float num12 = num + (float)num6 * num4 + num7;
				if (num12 > num + 0.5f * this.width)
				{
					num12 = num + 0.5f * this.width;
				}
				num6++;
				array[i + 1] = new Vector3(num12, num2 + this.height, 0f);
			}
			float x = array[i + 1].x;
			float y = array[i + 1].y;
			array2[i + 1] = new Vector2(Mathf.Lerp(this.m_uv.uvTL.x, this.m_uv.uvTR.x, (x - num) / this.width), Mathf.Lerp(this.m_uv.uvBL.y, this.m_uv.uvTL.y, (y - num2) / this.height));
		}
		Color[] array3 = new Color[num3 + 1];
		for (int j = 0; j < array3.Length; j++)
		{
			array3[j] = this.m_color;
		}
		int[] array4 = new int[(num3 - 1) * 3];
		for (int k = 0; k < num3 - 1; k++)
		{
			array4[k * 3] = 0;
			array4[k * 3 + 1] = k + 1;
			array4[k * 3 + 2] = k + 2;
		}
		this.m_mesh.vertices = array;
		this.m_mesh.uv = array2;
		this.m_mesh.colors = array3;
		this.m_mesh.triangles = array4;
		this.RecaculateDepth();
	}

	private void GenerateHoriontalFillMesh()
	{
		this.m_fillAmount = Mathf.Clamp01(this.m_fillAmount);
		if (this.m_fillAmount <= 0f)
		{
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		this.m_mesh.MarkDynamic();
		float num = 0f;
		float num2 = 0f;
		if (this.m_alignHoriontal == Sprite3D.EnumHoriontal.Center)
		{
			num = -0.5f * this.m_width;
		}
		else if (this.m_alignHoriontal == Sprite3D.EnumHoriontal.Right)
		{
			num = -this.m_width;
		}
		if (this.m_alignVertical == Sprite3D.EnumVertical.Middle)
		{
			num2 = -0.5f * this.m_height;
		}
		else if (this.m_alignVertical == Sprite3D.EnumVertical.Top)
		{
			num2 = -this.m_height;
		}
		Vector3[] vertices = new Vector3[]
		{
			new Vector3(num, num2 + this.m_height, 0f),
			new Vector3(num + this.m_width * this.m_fillAmount, num2 + this.m_height, 0f),
			new Vector3(num, num2, 0f),
			new Vector3(num + this.m_width * this.m_fillAmount, num2, 0f)
		};
		Vector2[] uv = new Vector2[]
		{
			this.m_uv.uvTL,
			this.m_uv.uvTL.Lerp(this.m_uv.uvTR, this.m_fillAmount),
			this.m_uv.uvBL,
			this.m_uv.uvBL.Lerp(this.m_uv.uvBR, this.m_fillAmount)
		};
		Color[] colors = new Color[]
		{
			this.m_color,
			this.m_color,
			this.m_color,
			this.m_color
		};
		int[] triangles = new int[]
		{
			0,
			1,
			2,
			3,
			2,
			1
		};
		this.m_mesh.vertices = vertices;
		this.m_mesh.uv = uv;
		this.m_mesh.colors = colors;
		this.m_mesh.triangles = triangles;
		this.RecaculateDepth();
	}

	private void GenerateVerticalFillMesh()
	{
		this.m_fillAmount = Mathf.Clamp01(this.m_fillAmount);
		if (this.m_fillAmount <= 0f)
		{
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		this.m_mesh.MarkDynamic();
		float num = 0f;
		float num2 = 0f;
		if (this.m_alignHoriontal == Sprite3D.EnumHoriontal.Center)
		{
			num = -0.5f * this.m_width;
		}
		else if (this.m_alignHoriontal == Sprite3D.EnumHoriontal.Right)
		{
			num = -this.m_width;
		}
		if (this.m_alignVertical == Sprite3D.EnumVertical.Middle)
		{
			num2 = -0.5f * this.m_height;
		}
		else if (this.m_alignVertical == Sprite3D.EnumVertical.Top)
		{
			num2 = -this.m_height;
		}
		Vector3[] vertices = new Vector3[]
		{
			new Vector3(num, num2 + this.m_height * this.m_fillAmount, 0f),
			new Vector3(num + this.m_width, num2 + this.m_height * this.m_fillAmount, 0f),
			new Vector3(num, num2, 0f),
			new Vector3(num + this.m_width, num2, 0f)
		};
		Vector2[] uv = new Vector2[]
		{
			this.m_uv.uvBL.Lerp(this.m_uv.uvTL, this.m_fillAmount),
			this.m_uv.uvBR.Lerp(this.m_uv.uvTR, this.m_fillAmount),
			this.m_uv.uvBL,
			this.m_uv.uvBR
		};
		Color[] colors = new Color[]
		{
			this.m_color,
			this.m_color,
			this.m_color,
			this.m_color
		};
		int[] triangles = new int[]
		{
			0,
			1,
			2,
			3,
			2,
			1
		};
		this.m_mesh.vertices = vertices;
		this.m_mesh.uv = uv;
		this.m_mesh.colors = colors;
		this.m_mesh.triangles = triangles;
		this.RecaculateDepth();
	}

	private void GenerateRepeatHoriontalFillMesh()
	{
		this.m_fillAmount = Mathf.Clamp01(this.m_fillAmount);
		if (this.m_fillAmount <= 0f)
		{
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		this.m_mesh.MarkDynamic();
		float num = 0f;
		float num2 = 0f;
		if (this.m_alignHoriontal == Sprite3D.EnumHoriontal.Center)
		{
			num = -0.5f * this.m_width;
		}
		else if (this.m_alignHoriontal == Sprite3D.EnumHoriontal.Right)
		{
			num = -this.m_width;
		}
		if (this.m_alignVertical == Sprite3D.EnumVertical.Middle)
		{
			num2 = -0.5f * this.m_height;
		}
		else if (this.m_alignVertical == Sprite3D.EnumVertical.Top)
		{
			num2 = -this.m_height;
		}
		int num3 = Mathf.CeilToInt(this.fillAmount * this.width / this.repeatSpace);
		if (this.repeatSpaceFirst && this.repeatSpace * (float)num3 - this.repeatUnitWidth > this.fillAmount * this.m_width)
		{
			num3--;
		}
		Vector3[] array = new Vector3[num3 * 4];
		Vector2[] array2 = new Vector2[num3 * 4];
		Color[] array3 = new Color[num3 * 4];
		int[] array4 = new int[num3 * 6];
		float b = num + this.m_width * this.m_fillAmount;
		float repeatSpace = this.repeatSpace;
		for (int i = 0; i < num3; i++)
		{
			float num4 = num + (float)i * repeatSpace;
			if (this.repeatSpaceFirst)
			{
				num4 += this.repeatSpace - this.repeatUnitWidth;
			}
			array[i * 4] = new Vector3(num4, num2 + this.repeatUnitHeight, 0f);
			array[i * 4 + 1] = new Vector3(Mathf.Min(num4 + this.repeatUnitWidth, b), num2 + this.repeatUnitHeight, 0f);
			array[i * 4 + 2] = new Vector3(num4, num2, 0f);
			array[i * 4 + 3] = new Vector3(Mathf.Min(num4 + this.repeatUnitWidth, b), num2, 0f);
			float lerp = (Mathf.Min(num4 + this.repeatUnitWidth, b) - num4) / this.repeatUnitWidth;
			array2[i * 4] = this.m_uv.uvTL;
			array2[i * 4 + 1] = this.m_uv.uvTL.Lerp(this.m_uv.uvTR, lerp);
			array2[i * 4 + 2] = this.m_uv.uvBL;
			array2[i * 4 + 3] = this.m_uv.uvBL.Lerp(this.m_uv.uvBR, lerp);
			array3[i * 4] = this.m_color;
			array3[i * 4 + 1] = this.m_color;
			array3[i * 4 + 2] = this.m_color;
			array3[i * 4 + 3] = this.m_color;
			array4[i * 6] = i * 4;
			array4[i * 6 + 1] = i * 4 + 1;
			array4[i * 6 + 2] = i * 4 + 2;
			array4[i * 6 + 3] = i * 4 + 3;
			array4[i * 6 + 4] = i * 4 + 2;
			array4[i * 6 + 5] = i * 4 + 1;
		}
		this.m_mesh.vertices = array;
		this.m_mesh.uv = array2;
		this.m_mesh.colors = array3;
		this.m_mesh.triangles = array4;
		this.RecaculateDepth();
	}
}
