using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace TMPro
{
	[AddComponentMenu("Mesh/TextMesh Pro"), ExecuteInEditMode, RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(TextContainer))]
	public class TextMeshPro : MonoBehaviour
	{
		private enum TextInputSources
		{
			Text,
			SetText,
			SetCharArray
		}

		[SerializeField]
		private string m_text;

		[SerializeField]
		private TextMeshProFont m_fontAsset;

		private Material m_fontMaterial;

		private Material m_sharedMaterial;

		[SerializeField]
		private FontStyles m_fontStyle;

		private FontStyles m_style;

		[SerializeField]
		private bool m_isOverlay;

		[FormerlySerializedAs("m_fontColor"), SerializeField]
		private Color32 m_fontColor32 = Color.white;

		[SerializeField]
		private Color m_fontColor = Color.white;

		private Color m_fontColorPreviews = Color.white;

		[SerializeField]
		private VertexGradient m_fontColorGradient = new VertexGradient(Color.white);

		[SerializeField]
		private bool m_enableVertexGradient;

		[SerializeField]
		private Vector3 m_fastScale = Vector3.one;

		private Vector3 m_fastScalePreviews = Vector3.one;

		[SerializeField]
		private Color32 m_faceColor = Color.white;

		[SerializeField]
		private Color32 m_outlineColor = Color.black;

		private float m_outlineWidth;

		[SerializeField]
		private float m_fontSize = 36f;

		[SerializeField]
		private float m_fontSizeMin;

		[SerializeField]
		private float m_fontSizeMax;

		[SerializeField]
		private float m_fontSizeBase = 36f;

		[SerializeField]
		private float m_charSpacingMax;

		[SerializeField]
		private float m_lineSpacingMax;

		private float m_currentFontSize;

		[SerializeField]
		private float m_characterSpacing;

		private float m_cSpacing;

		private float m_monoSpacing;

		[SerializeField]
		private Vector4 m_textRectangle;

		[SerializeField]
		private float m_lineSpacing;

		private float m_lineSpacingDelta;

		private float m_lineHeight;

		[SerializeField]
		private float m_paragraphSpacing;

		[SerializeField]
		private float m_lineLength;

		[SerializeField]
		private TMP_Compatibility.AnchorPositions m_anchor;

		[FormerlySerializedAs("m_lineJustification"), SerializeField]
		private TextAlignmentOptions m_textAlignment;

		private TextAlignmentOptions m_lineJustification;

		[SerializeField]
		private bool m_enableKerning;

		private bool m_anchorDampening;

		private float m_baseDampeningWidth;

		[SerializeField]
		private bool m_overrideHtmlColors;

		[SerializeField]
		private bool m_enableExtraPadding;

		[SerializeField]
		private bool checkPaddingRequired;

		[SerializeField]
		private bool m_enableWordWrapping;

		private bool m_isCharacterWrappingEnabled;

		[SerializeField]
		private TextOverflowModes m_overflowMode;

		[SerializeField]
		private float m_wordWrappingRatios = 0.4f;

		[SerializeField]
		private TextureMappingOptions m_horizontalMapping;

		[SerializeField]
		private TextureMappingOptions m_verticalMapping;

		[SerializeField]
		private Vector2 m_uvOffset = Vector2.zero;

		[SerializeField]
		private float m_uvLineOffset;

		[SerializeField]
		private bool isInputParsingRequired;

		[SerializeField]
		private bool havePropertiesChanged;

		[SerializeField]
		private bool haveColorChanged;

		[SerializeField]
		private bool haveFastScaleChanged;

		[SerializeField]
		private bool hasFontAssetChanged;

		[SerializeField]
		private bool m_isRichText = true;

		[SerializeField]
		private TextMeshPro.TextInputSources m_inputSource;

		private string old_text;

		private float old_arg0;

		private float old_arg1;

		private float old_arg2;

		private int m_fontIndex;

		private float m_fontScale;

		private bool m_isRecalculateScaleRequired;

		private Vector3 m_previousLossyScale;

		private float m_xAdvance;

		private float m_indent;

		private float m_maxXAdvance;

		private Vector3 m_anchorOffset;

		private TMP_TextInfo m_textInfo;

		private char[] m_htmlTag = new char[128];

		[SerializeField]
		private Renderer m_renderer;

		private MeshFilter m_meshFilter;

		private Mesh m_mesh;

		private Transform m_transform;

		private Color32 m_htmlColor = new Color(255f, 255f, 255f, 128f);

		private Color32[] m_colorStack = new Color32[32];

		private int m_colorStackIndex;

		private float m_tabSpacing;

		private float m_spacing;

		private Vector2[] m_spacePositions = new Vector2[8];

		private float m_baselineOffset;

		private float m_padding;

		private Vector4 m_alignmentPadding;

		private bool m_isUsingBold;

		private Vector2 k_InfinityVector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

		private bool m_isFirstAllocation;

		private int m_max_characters = 8;

		private int m_max_numberOfLines = 4;

		private int[] m_char_buffer;

		private char[] m_input_CharArray = new char[256];

		private int m_charArray_Length;

		private List<char> m_VisibleCharacters = new List<char>();

		private readonly float[] k_Power = new float[]
		{
			0.5f,
			0.05f,
			0.005f,
			0.0005f,
			5E-05f,
			5E-06f,
			5E-07f,
			5E-08f,
			5E-09f,
			5E-10f
		};

		private GlyphInfo m_cached_GlyphInfo;

		private GlyphInfo m_cached_Underline_GlyphInfo;

		private WordWrapState m_SavedWordWrapState = default(WordWrapState);

		private WordWrapState m_SavedLineState = default(WordWrapState);

		private int m_characterCount;

		private int m_visibleCharacterCount;

		private int m_firstVisibleCharacterOfLine;

		private int m_lastVisibleCharacterOfLine;

		private int m_lineNumber;

		private int m_pageNumber;

		private float m_maxAscender;

		private float m_maxDescender;

		private float m_maxFontScale;

		private float m_lineOffset;

		private Extents m_meshExtents;

		private float m_preferredWidth;

		private float m_preferredHeight;

		private Vector3[] m_vertices;

		private Vector3[] m_normals;

		private Vector4[] m_tangents;

		private Vector2[] m_uvs;

		private Vector2[] m_uv2s;

		private Color32[] m_vertColors;

		private int[] m_triangles;

		private Color[] m_tempColors;

		private Vector3[] m_tempVertices;

		private Bounds m_default_bounds = new Bounds(Vector3.zero, new Vector3(1000f, 1000f, 0f));

		[SerializeField]
		private bool m_ignoreCulling = true;

		[SerializeField]
		private bool m_isOrthographic;

		[SerializeField]
		private bool m_isCullingEnabled;

		private int m_sortingLayerID;

		private int m_maxVisibleCharacters = -1;

		private int m_maxVisibleLines = -1;

		[SerializeField]
		private int m_pageToDisplay;

		private bool m_isNewPage;

		private bool m_isTextTruncated;

		[SerializeField]
		private TextMeshProFont[] m_fontAssetArray;

		private List<Material> m_sharedMaterials = new List<Material>(16);

		private int m_selectedFontAsset;

		private MaterialPropertyBlock m_maskingPropertyBlock;

		private bool m_isMaskingEnabled;

		private bool isMaskUpdateRequired;

		private bool m_isMaterialBlockSet;

		[SerializeField]
		private MaskingTypes m_maskType;

		private Matrix4x4 m_EnvMapMatrix = default(Matrix4x4);

		private TextRenderFlags m_renderMode;

		[SerializeField]
		private bool m_isNewTextObject;

		private TextContainer m_textContainer;

		private float m_marginWidth;

		[SerializeField]
		private bool m_enableAutoSizing;

		private float m_maxFontSize;

		private float m_minFontSize;

		private Stopwatch m_StopWatch;

		private bool isDebugOutputDone;

		private int m_recursiveCount;

		private int loopCountA;

		private int loopCountB;

		private int loopCountC;

		private int loopCountD;

		private int loopCountE;

		private GameObject m_prefabParent;

		public string text
		{
			get
			{
				return this.m_text;
			}
			set
			{
				this.m_inputSource = TextMeshPro.TextInputSources.Text;
				this.havePropertiesChanged = true;
				this.isInputParsingRequired = true;
				this.m_text = value;
			}
		}

		public TextMeshProFont font
		{
			get
			{
				return this.m_fontAsset;
			}
			set
			{
				if (this.m_fontAsset != value)
				{
					this.m_fontAsset = value;
					this.LoadFontAsset();
					this.havePropertiesChanged = true;
				}
			}
		}

		public Material fontMaterial
		{
			get
			{
				if (this.m_fontMaterial == null)
				{
					this.SetFontMaterial(this.m_sharedMaterial);
					return this.m_sharedMaterial;
				}
				return this.m_sharedMaterial;
			}
			set
			{
				this.SetFontMaterial(value);
				this.havePropertiesChanged = true;
			}
		}

		public Material fontSharedMaterial
		{
			get
			{
				return this.m_renderer.sharedMaterial;
			}
			set
			{
				if (this.m_sharedMaterial != value)
				{
					this.SetSharedFontMaterial(value);
					this.havePropertiesChanged = true;
				}
			}
		}

		public bool isOverlay
		{
			get
			{
				return this.m_isOverlay;
			}
			set
			{
				this.m_isOverlay = value;
				this.SetShaderType();
				this.havePropertiesChanged = true;
			}
		}

		public Color color
		{
			get
			{
				return this.m_fontColor;
			}
			set
			{
				if (!this.m_fontColor.Compare(value))
				{
					this.haveColorChanged = true;
					this.m_fontColor = value;
				}
			}
		}

		public Vector3 fastScale
		{
			get
			{
				return this.m_fastScale;
			}
			set
			{
				if (!this.m_fastScale.Compare(value, 10000))
				{
					this.haveFastScaleChanged = true;
					this.m_fastScale = value;
				}
			}
		}

		public VertexGradient colorGradient
		{
			get
			{
				return this.m_fontColorGradient;
			}
			set
			{
				this.havePropertiesChanged = true;
				this.m_fontColorGradient = value;
			}
		}

		public bool enableVertexGradient
		{
			get
			{
				return this.m_enableVertexGradient;
			}
			set
			{
				this.havePropertiesChanged = true;
				this.m_enableVertexGradient = value;
			}
		}

		public Color32 faceColor
		{
			get
			{
				return this.m_faceColor;
			}
			set
			{
				if (!this.m_faceColor.Compare(value))
				{
					this.SetFaceColor(value);
					this.havePropertiesChanged = true;
					this.m_faceColor = value;
				}
			}
		}

		public Color32 outlineColor
		{
			get
			{
				return this.m_outlineColor;
			}
			set
			{
				if (!this.m_outlineColor.Compare(value))
				{
					this.SetOutlineColor(value);
					this.havePropertiesChanged = true;
					this.m_outlineColor = value;
				}
			}
		}

		public float outlineWidth
		{
			get
			{
				return this.m_outlineWidth;
			}
			set
			{
				this.SetOutlineThickness(value);
				this.havePropertiesChanged = true;
				this.checkPaddingRequired = true;
				this.m_outlineWidth = value;
			}
		}

		public float fontSize
		{
			get
			{
				return this.m_fontSize;
			}
			set
			{
				if (this.m_fontSize != value)
				{
					this.havePropertiesChanged = true;
					this.m_fontSize = value;
				}
			}
		}

		public float fontScale
		{
			get
			{
				return this.m_fontScale;
			}
		}

		public FontStyles fontStyle
		{
			get
			{
				return this.m_fontStyle;
			}
			set
			{
				this.m_fontStyle = value;
				this.havePropertiesChanged = true;
				this.checkPaddingRequired = true;
			}
		}

		public float characterSpacing
		{
			get
			{
				return this.m_characterSpacing;
			}
			set
			{
				if (this.m_characterSpacing != value)
				{
					this.havePropertiesChanged = true;
					this.m_characterSpacing = value;
				}
			}
		}

		public bool richText
		{
			get
			{
				return this.m_isRichText;
			}
			set
			{
				this.m_isRichText = value;
				this.havePropertiesChanged = true;
				this.isInputParsingRequired = true;
			}
		}

		[Obsolete("The length of the line is now controlled by the size of the text container and margins.")]
		public float lineLength
		{
			get
			{
				return this.m_lineLength;
			}
			set
			{
				Debug.Log("lineLength set called.");
			}
		}

		public TextOverflowModes OverflowMode
		{
			get
			{
				return this.m_overflowMode;
			}
			set
			{
				this.m_overflowMode = value;
				this.havePropertiesChanged = true;
			}
		}

		public Bounds bounds
		{
			get
			{
				if (this.m_mesh != null)
				{
					return this.m_mesh.bounds;
				}
				return default(Bounds);
			}
		}

		public float lineSpacing
		{
			get
			{
				return this.m_lineSpacing;
			}
			set
			{
				if (this.m_lineSpacing != value)
				{
					this.havePropertiesChanged = true;
					this.m_lineSpacing = value;
				}
			}
		}

		public float paragraphSpacing
		{
			get
			{
				return this.m_paragraphSpacing;
			}
			set
			{
				if (this.m_paragraphSpacing != value)
				{
					this.havePropertiesChanged = true;
					this.m_paragraphSpacing = value;
				}
			}
		}

		[Obsolete("The length of the line is now controlled by the size of the text container and margins.")]
		public TMP_Compatibility.AnchorPositions anchor
		{
			get
			{
				return this.m_anchor;
			}
		}

		public TextAlignmentOptions alignment
		{
			get
			{
				return this.m_textAlignment;
			}
			set
			{
				if (this.m_textAlignment != value)
				{
					this.havePropertiesChanged = true;
					this.m_textAlignment = value;
				}
			}
		}

		public bool enableKerning
		{
			get
			{
				return this.m_enableKerning;
			}
			set
			{
				if (this.m_enableKerning != value)
				{
					this.havePropertiesChanged = true;
					this.m_enableKerning = value;
				}
			}
		}

		public bool anchorDampening
		{
			get
			{
				return this.m_anchorDampening;
			}
			set
			{
				if (this.m_anchorDampening != value)
				{
					this.havePropertiesChanged = true;
					this.m_anchorDampening = value;
				}
			}
		}

		public bool overrideColorTags
		{
			get
			{
				return this.m_overrideHtmlColors;
			}
			set
			{
				if (this.m_overrideHtmlColors != value)
				{
					this.havePropertiesChanged = true;
					this.m_overrideHtmlColors = value;
				}
			}
		}

		public bool extraPadding
		{
			get
			{
				return this.m_enableExtraPadding;
			}
			set
			{
				if (this.m_enableExtraPadding != value)
				{
					this.havePropertiesChanged = true;
					this.checkPaddingRequired = true;
					this.m_enableExtraPadding = value;
				}
			}
		}

		public bool enableWordWrapping
		{
			get
			{
				return this.m_enableWordWrapping;
			}
			set
			{
				if (this.m_enableWordWrapping != value)
				{
					this.havePropertiesChanged = true;
					this.isInputParsingRequired = true;
					this.m_enableWordWrapping = value;
				}
			}
		}

		public TextureMappingOptions horizontalMapping
		{
			get
			{
				return this.m_horizontalMapping;
			}
			set
			{
				if (this.m_horizontalMapping != value)
				{
					this.havePropertiesChanged = true;
					this.m_horizontalMapping = value;
				}
			}
		}

		public TextureMappingOptions verticalMapping
		{
			get
			{
				return this.m_verticalMapping;
			}
			set
			{
				if (this.m_verticalMapping != value)
				{
					this.havePropertiesChanged = true;
					this.m_verticalMapping = value;
				}
			}
		}

		public bool ignoreVisibility
		{
			get
			{
				return this.m_ignoreCulling;
			}
			set
			{
				if (this.m_ignoreCulling != value)
				{
					this.havePropertiesChanged = true;
					this.m_ignoreCulling = value;
				}
			}
		}

		public bool isOrthographic
		{
			get
			{
				return this.m_isOrthographic;
			}
			set
			{
				this.havePropertiesChanged = true;
				this.m_isOrthographic = value;
			}
		}

		public bool enableCulling
		{
			get
			{
				return this.m_isCullingEnabled;
			}
			set
			{
				this.m_isCullingEnabled = value;
				this.SetCulling();
				this.havePropertiesChanged = true;
			}
		}

		public int sortingLayerID
		{
			get
			{
				return this.m_renderer.sortingLayerID;
			}
			set
			{
				this.m_renderer.sortingLayerID = value;
			}
		}

		public int sortingOrder
		{
			get
			{
				return this.m_renderer.sortingOrder;
			}
			set
			{
				this.m_renderer.sortingOrder = value;
			}
		}

		public bool hasChanged
		{
			get
			{
				return this.havePropertiesChanged;
			}
			set
			{
				this.havePropertiesChanged = value;
			}
		}

		public TextRenderFlags renderMode
		{
			get
			{
				return this.m_renderMode;
			}
			set
			{
				this.m_renderMode = value;
				this.havePropertiesChanged = true;
			}
		}

		public TextContainer textContainer
		{
			get
			{
				if (this.m_textContainer == null)
				{
					this.m_textContainer = base.GetComponent<TextContainer>();
				}
				return this.m_textContainer;
			}
		}

		public int maxVisibleCharacters
		{
			get
			{
				return this.m_maxVisibleCharacters;
			}
			set
			{
				if (this.m_maxVisibleCharacters != value)
				{
					this.havePropertiesChanged = true;
					this.m_maxVisibleCharacters = value;
				}
			}
		}

		public int maxVisibleLines
		{
			get
			{
				return this.m_maxVisibleLines;
			}
			set
			{
				if (this.m_maxVisibleLines != value)
				{
					this.havePropertiesChanged = true;
					this.isInputParsingRequired = true;
					this.m_maxVisibleLines = value;
				}
			}
		}

		public int pageToDisplay
		{
			get
			{
				return this.m_pageToDisplay;
			}
			set
			{
				this.havePropertiesChanged = true;
				this.m_pageToDisplay = value;
			}
		}

		public float preferredWidth
		{
			get
			{
				return this.m_preferredWidth;
			}
		}

		public Vector2[] spacePositions
		{
			get
			{
				return this.m_spacePositions;
			}
		}

		public bool enableAutoSizing
		{
			get
			{
				return this.m_enableAutoSizing;
			}
			set
			{
				this.m_enableAutoSizing = value;
			}
		}

		public float fontSizeMin
		{
			get
			{
				return this.m_fontSizeMin;
			}
			set
			{
				this.m_fontSizeMin = value;
			}
		}

		public float fontSizeMax
		{
			get
			{
				return this.m_fontSizeMax;
			}
			set
			{
				this.m_fontSizeMax = value;
			}
		}

		public MaskingTypes maskType
		{
			get
			{
				return this.m_maskType;
			}
			set
			{
				this.m_maskType = value;
				this.havePropertiesChanged = true;
				this.isMaskUpdateRequired = true;
			}
		}

		public TMP_TextInfo textInfo
		{
			get
			{
				return this.m_textInfo;
			}
		}

		public Mesh mesh
		{
			get
			{
				return this.m_mesh;
			}
		}

		private void Awake()
		{
			if (this.m_fontColor == Color.white && this.m_fontColor32 != Color.white)
			{
				Debug.LogWarning("Converting Vertex Colors from Color32 to Color.");
				this.m_fontColor = this.m_fontColor32;
			}
			this.m_textContainer = base.GetComponent<TextContainer>();
			if (this.m_textContainer == null)
			{
				this.m_textContainer = base.gameObject.AddComponent<TextContainer>();
			}
			this.m_renderer = base.GetComponent<Renderer>();
			if (this.m_renderer == null)
			{
				this.m_renderer = base.gameObject.AddComponent<Renderer>();
			}
			this.m_transform = base.gameObject.transform;
			this.m_meshFilter = base.GetComponent<MeshFilter>();
			if (this.m_meshFilter == null)
			{
				this.m_meshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			if (this.m_mesh == null)
			{
				this.m_mesh = new Mesh();
				this.m_mesh.hideFlags = HideFlags.HideAndDontSave;
				this.m_meshFilter.mesh = this.m_mesh;
			}
			this.m_meshFilter.hideFlags = HideFlags.HideInInspector;
			this.LoadFontAsset();
			this.m_char_buffer = new int[this.m_max_characters];
			this.m_cached_GlyphInfo = new GlyphInfo();
			this.m_vertices = new Vector3[0];
			this.m_isFirstAllocation = true;
			this.m_textInfo = new TMP_TextInfo();
			this.m_textInfo.wordInfo = new List<TMP_WordInfo>();
			this.m_textInfo.lineInfo = new TMP_LineInfo[this.m_max_numberOfLines];
			this.m_textInfo.pageInfo = new TMP_PageInfo[16];
			this.m_textInfo.meshInfo = default(TMP_MeshInfo);
			this.m_fontAssetArray = new TextMeshProFont[16];
			if (this.m_fontAsset == null)
			{
				Debug.LogWarning("Please assign a Font Asset to this " + base.transform.name + " gameobject.");
				return;
			}
			if (this.m_fontSizeMin == 0f)
			{
				this.m_fontSizeMin = this.m_fontSize / 2f;
			}
			if (this.m_fontSizeMax == 0f)
			{
				this.m_fontSizeMax = this.m_fontSize * 2f;
			}
			this.isInputParsingRequired = true;
			this.havePropertiesChanged = true;
			this.haveColorChanged = false;
			if (this.m_fastScale != Vector3.one)
			{
				this.haveFastScaleChanged = true;
			}
			else
			{
				this.haveFastScaleChanged = false;
			}
			this.ForceMeshUpdate();
		}

		private void OnEnable()
		{
			if (this.m_meshFilter.sharedMesh == null)
			{
				this.m_meshFilter.mesh = this.m_mesh;
			}
			this.haveFastScaleChanged = true;
			this.haveColorChanged = true;
		}

		private void OnDisable()
		{
		}

		private void OnDestroy()
		{
			if (this.m_mesh != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_mesh);
			}
		}

		private void Reset()
		{
			if (this.m_mesh != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_mesh);
			}
			this.Awake();
		}

		private void LateUpdate()
		{
			if (!this.haveFastScaleChanged && !this.m_fastScale.Compare(this.m_fastScalePreviews, 10000))
			{
				this.haveFastScaleChanged = true;
				this.m_fastScalePreviews = this.m_fastScale;
			}
			if (!this.haveColorChanged && !this.m_fontColor.Compare(this.m_fontColorPreviews))
			{
				this.haveColorChanged = true;
				this.m_fontColorPreviews = this.m_fontColor;
			}
		}

		private void LoadFontAsset()
		{
			ShaderUtilities.GetShaderPropertyIDs();
			if (this.m_fontAsset == null)
			{
				this.m_fontAsset = (Resources.Load("Fonts & Materials/ARIAL SDF", typeof(TextMeshProFont)) as TextMeshProFont);
				if (this.m_fontAsset == null)
				{
					Debug.LogWarning("The ARIAL SDF Font Asset was not found. There is no Font Asset assigned to " + base.gameObject.name + ".");
					return;
				}
				if (this.m_fontAsset.characterDictionary == null)
				{
					Debug.Log("Dictionary is Null!");
				}
				this.m_renderer.sharedMaterial = this.m_fontAsset.material;
				this.m_sharedMaterial = this.m_fontAsset.material;
				this.m_sharedMaterial.SetFloat("_CullMode", 0f);
				this.m_sharedMaterial.SetFloat("_ZTestMode", 4f);
				this.m_renderer.receiveShadows = false;
				this.m_renderer.castShadows = false;
			}
			else
			{
				if (this.m_fontAsset.characterDictionary == null)
				{
					this.m_fontAsset.ReadFontDefinition();
				}
				if (this.m_renderer.sharedMaterial == null || this.m_renderer.sharedMaterial.mainTexture == null || this.m_fontAsset.atlas.GetInstanceID() != this.m_renderer.sharedMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
				{
					this.m_renderer.sharedMaterial = this.m_fontAsset.material;
					this.m_sharedMaterial = this.m_fontAsset.material;
				}
				else
				{
					this.m_sharedMaterial = this.m_renderer.sharedMaterial;
				}
				this.m_sharedMaterial.SetFloat("_ZTestMode", 4f);
				if (this.m_sharedMaterial.passCount > 1)
				{
					this.m_renderer.receiveShadows = false;
					this.m_renderer.castShadows = true;
				}
				else
				{
					this.m_renderer.receiveShadows = false;
					this.m_renderer.castShadows = false;
				}
			}
			this.m_padding = ShaderUtilities.GetPadding(this.m_renderer.sharedMaterials, this.m_enableExtraPadding, this.m_isUsingBold);
			this.m_alignmentPadding = ShaderUtilities.GetFontExtent(this.m_sharedMaterial);
			this.m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(this.m_sharedMaterial);
			this.m_sharedMaterials.Add(this.m_sharedMaterial);
		}

		private void ScheduleUpdate()
		{
		}

		private void UpdateEnvMapMatrix()
		{
			if (!this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_EnvMap) || this.m_sharedMaterial.GetTexture(ShaderUtilities.ID_EnvMap) == null)
			{
				return;
			}
			Vector3 euler = this.m_sharedMaterial.GetVector(ShaderUtilities.ID_EnvMatrixRotation);
			this.m_EnvMapMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(euler), Vector3.one);
			this.m_sharedMaterial.SetMatrix(ShaderUtilities.ID_EnvMatrix, this.m_EnvMapMatrix);
		}

		private void EnableMasking()
		{
			if (this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_MaskCoord))
			{
				this.m_sharedMaterial.EnableKeyword("MASK_SOFT");
				this.m_sharedMaterial.DisableKeyword("MASK_HARD");
				this.m_sharedMaterial.DisableKeyword("MASK_OFF");
				this.m_isMaskingEnabled = true;
				this.UpdateMask();
			}
		}

		private void DisableMasking()
		{
			if (this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_MaskCoord))
			{
				this.m_sharedMaterial.EnableKeyword("MASK_OFF");
				this.m_sharedMaterial.DisableKeyword("MASK_HARD");
				this.m_sharedMaterial.DisableKeyword("MASK_SOFT");
				this.m_isMaskingEnabled = false;
				this.UpdateMask();
			}
		}

		private void UpdateMask()
		{
			if (!this.m_isMaskingEnabled)
			{
				return;
			}
			if (this.m_isMaskingEnabled && this.m_fontMaterial == null)
			{
				this.CreateMaterialInstance();
			}
			float num = Mathf.Min(Mathf.Min(this.m_textContainer.margins.x, this.m_textContainer.margins.z), this.m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessX));
			float num2 = Mathf.Min(Mathf.Min(this.m_textContainer.margins.y, this.m_textContainer.margins.w), this.m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessY));
			num = ((num <= 0f) ? 0f : num);
			num2 = ((num2 <= 0f) ? 0f : num2);
			float z = (this.m_textContainer.width - Mathf.Max(this.m_textContainer.margins.x, 0f) - Mathf.Max(this.m_textContainer.margins.z, 0f)) / 2f + num;
			float w = (this.m_textContainer.height - Mathf.Max(this.m_textContainer.margins.y, 0f) - Mathf.Max(this.m_textContainer.margins.w, 0f)) / 2f + num2;
			Vector2 vector = new Vector2((0.5f - this.m_textContainer.pivot.x) * this.m_textContainer.width + (Mathf.Max(this.m_textContainer.margins.x, 0f) - Mathf.Max(this.m_textContainer.margins.z, 0f)) / 2f, (0.5f - this.m_textContainer.pivot.y) * this.m_textContainer.height + (-Mathf.Max(this.m_textContainer.margins.y, 0f) + Mathf.Max(this.m_textContainer.margins.w, 0f)) / 2f);
			Vector4 vector2 = new Vector4(vector.x, vector.y, z, w);
			this.m_fontMaterial.SetVector(ShaderUtilities.ID_MaskCoord, vector2);
			this.m_fontMaterial.SetFloat(ShaderUtilities.ID_MaskSoftnessX, num);
			this.m_fontMaterial.SetFloat(ShaderUtilities.ID_MaskSoftnessY, num2);
		}

		private void SetMeshArrays(int size)
		{
			int num = size * 4;
			int num2 = size * 6;
			this.m_vertices = new Vector3[num];
			this.m_normals = new Vector3[num];
			this.m_tangents = new Vector4[num];
			this.m_uvs = new Vector2[num];
			this.m_uv2s = new Vector2[num];
			this.m_vertColors = new Color32[num];
			this.m_tempColors = new Color[num];
			this.m_tempVertices = new Vector3[num];
			this.m_triangles = new int[num2];
			for (int i = 0; i < size; i++)
			{
				int num3 = i * 4;
				int num4 = i * 6;
				this.m_vertices[0 + num3] = Vector3.zero;
				this.m_vertices[1 + num3] = Vector3.zero;
				this.m_vertices[2 + num3] = Vector3.zero;
				this.m_vertices[3 + num3] = Vector3.zero;
				this.m_uvs[0 + num3] = Vector2.zero;
				this.m_uvs[1 + num3] = Vector2.zero;
				this.m_uvs[2 + num3] = Vector2.zero;
				this.m_uvs[3 + num3] = Vector2.zero;
				this.m_normals[0 + num3] = new Vector3(0f, 0f, -1f);
				this.m_normals[1 + num3] = new Vector3(0f, 0f, -1f);
				this.m_normals[2 + num3] = new Vector3(0f, 0f, -1f);
				this.m_normals[3 + num3] = new Vector3(0f, 0f, -1f);
				this.m_tangents[0 + num3] = new Vector4(-1f, 0f, 0f, 1f);
				this.m_tangents[1 + num3] = new Vector4(-1f, 0f, 0f, 1f);
				this.m_tangents[2 + num3] = new Vector4(-1f, 0f, 0f, 1f);
				this.m_tangents[3 + num3] = new Vector4(-1f, 0f, 0f, 1f);
				this.m_triangles[0 + num4] = 0 + num3;
				this.m_triangles[1 + num4] = 1 + num3;
				this.m_triangles[2 + num4] = 2 + num3;
				this.m_triangles[3 + num4] = 3 + num3;
				this.m_triangles[4 + num4] = 2 + num3;
				this.m_triangles[5 + num4] = 1 + num3;
			}
			this.m_mesh.vertices = this.m_vertices;
			this.m_mesh.uv = this.m_uvs;
			this.m_mesh.normals = this.m_normals;
			this.m_mesh.tangents = this.m_tangents;
			this.m_mesh.triangles = this.m_triangles;
			this.m_mesh.bounds = this.m_default_bounds;
		}

		private void SetFontMaterial(Material mat)
		{
			if (this.m_renderer == null)
			{
				this.m_renderer = base.GetComponent<Renderer>();
			}
			this.m_renderer.material = mat;
			this.m_fontMaterial = this.m_renderer.material;
			this.m_sharedMaterial = this.m_fontMaterial;
			this.m_padding = ShaderUtilities.GetPadding(this.m_renderer.sharedMaterials, this.m_enableExtraPadding, this.m_isUsingBold);
			this.m_alignmentPadding = ShaderUtilities.GetFontExtent(this.m_sharedMaterial);
		}

		private void SetSharedFontMaterial(Material mat)
		{
			if (this.m_renderer == null)
			{
				this.m_renderer = base.GetComponent<Renderer>();
			}
			this.m_renderer.sharedMaterial = mat;
			this.m_sharedMaterial = this.m_renderer.sharedMaterial;
			this.m_padding = ShaderUtilities.GetPadding(this.m_renderer.sharedMaterials, this.m_enableExtraPadding, this.m_isUsingBold);
			this.m_alignmentPadding = ShaderUtilities.GetFontExtent(this.m_sharedMaterial);
		}

		private void SetOutlineThickness(float thickness)
		{
			thickness = Mathf.Clamp01(thickness);
			this.m_renderer.material.SetFloat(ShaderUtilities.ID_OutlineWidth, thickness);
			if (this.m_fontMaterial == null)
			{
				this.m_fontMaterial = this.m_renderer.material;
			}
			this.m_fontMaterial = this.m_renderer.material;
		}

		private void SetFaceColor(Color32 color)
		{
			this.m_renderer.material.SetColor(ShaderUtilities.ID_FaceColor, color);
			if (this.m_fontMaterial == null)
			{
				this.m_fontMaterial = this.m_renderer.material;
			}
		}

		private void SetOutlineColor(Color32 color)
		{
			this.m_renderer.material.SetColor(ShaderUtilities.ID_OutlineColor, color);
			if (this.m_fontMaterial == null)
			{
				this.m_fontMaterial = this.m_renderer.material;
			}
		}

		private void CreateMaterialInstance()
		{
			Material material = new Material(this.m_sharedMaterial);
			material.shaderKeywords = this.m_sharedMaterial.shaderKeywords;
			Material expr_1E = material;
			expr_1E.name += " Instance";
			this.m_fontMaterial = material;
		}

		private void SetShaderType()
		{
			if (this.m_isOverlay)
			{
				this.m_renderer.material.SetFloat("_ZTestMode", 8f);
				this.m_renderer.material.renderQueue = 4000;
				this.m_sharedMaterial = this.m_renderer.material;
			}
			else
			{
				this.m_renderer.material.SetFloat("_ZTestMode", 4f);
				this.m_renderer.material.renderQueue = -1;
				this.m_sharedMaterial = this.m_renderer.material;
			}
		}

		private void SetCulling()
		{
			if (this.m_isCullingEnabled)
			{
				this.m_renderer.material.SetFloat("_CullMode", 2f);
			}
			else
			{
				this.m_renderer.material.SetFloat("_CullMode", 0f);
			}
		}

		private void SetPerspectiveCorrection()
		{
			if (this.m_isOrthographic)
			{
				this.m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0f);
			}
			else
			{
				this.m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0.875f);
			}
		}

		private void AddIntToCharArray(int number, ref int index, int precision)
		{
			if (number < 0)
			{
				this.m_input_CharArray[index++] = '-';
				number = -number;
			}
			int num = index;
			do
			{
				this.m_input_CharArray[num++] = (char)(number % 10 + 48);
				number /= 10;
			}
			while (number > 0);
			int num2 = num;
			while (index + 1 < num)
			{
				num--;
				char c = this.m_input_CharArray[index];
				this.m_input_CharArray[index] = this.m_input_CharArray[num];
				this.m_input_CharArray[num] = c;
				index++;
			}
			index = num2;
		}

		private void AddFloatToCharArray(float number, ref int index, int precision)
		{
			if (number < 0f)
			{
				this.m_input_CharArray[index++] = '-';
				number = -number;
			}
			number += this.k_Power[Mathf.Min(9, precision)];
			int num = (int)number;
			this.AddIntToCharArray(num, ref index, precision);
			if (precision > 0)
			{
				this.m_input_CharArray[index++] = '.';
				number -= (float)num;
				for (int i = 0; i < precision; i++)
				{
					number *= 10f;
					int num2 = (int)number;
					this.m_input_CharArray[index++] = (char)(num2 + 48);
					number -= (float)num2;
				}
			}
		}

        private void StringToCharArray(string text, ref int[] chars)
        {
            if (text != null)
            {
                if (chars.Length <= text.Length)
                {
                    int num = (text.Length <= 0x400) ? Mathf.NextPowerOfTwo(text.Length + 1) : (text.Length + 0x100);
                    chars = new int[num];
                }
                int index = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if ((text[i] == '\\') && (i < (text.Length - 1)))
                    {
                        int num4 = text[i + 1];
                        switch (num4)
                        {
                            case 0x72:
                                {
                                    chars[index] = 13;
                                    i++;
                                    index++;
                                    continue;
                                }
                            case 0x74:
                                {
                                    chars[index] = 9;
                                    i++;
                                    index++;
                                    continue;
                                }
                        }
                        if (num4 == 110)
                        {
                            chars[index] = 10;
                            i++;
                            index++;
                            continue;
                        }
                    }
                    chars[index] = text[i];
                    index++;
                }
                chars[index] = 0;
            }
        }


        private void SetTextArrayToCharArray(char[] charArray, ref int[] charBuffer)
        {
            if ((charArray != null) && (this.m_charArray_Length != 0))
            {
                if (charBuffer.Length <= this.m_charArray_Length)
                {
                    int num = (this.m_charArray_Length <= 0x400) ? Mathf.NextPowerOfTwo(this.m_charArray_Length + 1) : (this.m_charArray_Length + 0x100);
                    charBuffer = new int[num];
                }
                int index = 0;
                for (int i = 0; i < this.m_charArray_Length; i++)
                {
                    if ((charArray[i] == '\\') && (i < (this.m_charArray_Length - 1)))
                    {
                        int num4 = charArray[i + 1];
                        switch (num4)
                        {
                            case 0x72:
                                {
                                    charBuffer[index] = 13;
                                    i++;
                                    index++;
                                    continue;
                                }
                            case 0x74:
                                {
                                    charBuffer[index] = 9;
                                    i++;
                                    index++;
                                    continue;
                                }
                        }
                        if (num4 == 110)
                        {
                            charBuffer[index] = 10;
                            i++;
                            index++;
                            continue;
                        }
                    }
                    charBuffer[index] = charArray[i];
                    index++;
                }
                charBuffer[index] = 0;
            }
        }


		private int GetArraySizes(int[] chars)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			this.m_isUsingBold = false;
			this.m_VisibleCharacters.Clear();
			int num4 = 0;
			while (chars[num4] != 0)
			{
				int num5 = chars[num4];
				if (this.m_isRichText && num5 == 60 && this.ValidateHtmlTag(chars, num4 + 1, out num3))
				{
					num4 = num3;
					if ((this.m_style & FontStyles.Underline) == FontStyles.Underline)
					{
						num += 3;
					}
					if ((this.m_style & FontStyles.Bold) == FontStyles.Bold)
					{
						this.m_isUsingBold = true;
					}
				}
				else
				{
					if (num5 != 32 && num5 != 9 && num5 != 10 && num5 != 13)
					{
						num++;
					}
					this.m_VisibleCharacters.Add((char)num5);
					num2++;
				}
				num4++;
			}
			return num2;
		}

		private int SetArraySizes(int[] chars)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			this.m_isUsingBold = false;
			this.m_VisibleCharacters.Clear();
			int num4 = 0;
			while (chars[num4] != 0)
			{
				int num5 = chars[num4];
				if (this.m_isRichText && num5 == 60 && this.ValidateHtmlTag(chars, num4 + 1, out num3))
				{
					num4 = num3;
					if ((this.m_style & FontStyles.Underline) == FontStyles.Underline)
					{
						num += 3;
					}
					if ((this.m_style & FontStyles.Bold) == FontStyles.Bold)
					{
						this.m_isUsingBold = true;
					}
				}
				else
				{
					if (num5 != 32 && num5 != 9 && num5 != 10 && num5 != 13)
					{
						num++;
					}
					this.m_VisibleCharacters.Add((char)num5);
					num2++;
				}
				num4++;
			}
			if (this.m_textInfo.characterInfo == null || num2 > this.m_textInfo.characterInfo.Length)
			{
				this.m_textInfo.characterInfo = new TMP_CharacterInfo[(num2 <= 1024) ? Mathf.NextPowerOfTwo(num2) : (num2 + 256)];
			}
			if (num * 4 > this.m_vertices.Length)
			{
				if (this.m_isFirstAllocation)
				{
					this.SetMeshArrays(num);
					this.m_isFirstAllocation = false;
				}
				else
				{
					this.SetMeshArrays((num <= 1024) ? Mathf.NextPowerOfTwo(num) : (num + 256));
				}
			}
			return num2;
		}

		private void ParseInputText()
		{
			this.isInputParsingRequired = false;
			switch (this.m_inputSource)
			{
			case TextMeshPro.TextInputSources.Text:
				this.StringToCharArray(this.m_text, ref this.m_char_buffer);
				break;
			case TextMeshPro.TextInputSources.SetText:
				this.SetTextArrayToCharArray(this.m_input_CharArray, ref this.m_char_buffer);
				break;
			}
		}

		private void OnPreRenderObject()
		{
		}

		private void OnWillRenderObject()
		{
			this.loopCountA = 0;
			if (this.m_fontAsset == null)
			{
				return;
			}
			if (this.havePropertiesChanged || this.m_fontAsset.propertiesChanged)
			{
				if (this.hasFontAssetChanged || this.m_fontAsset.propertiesChanged)
				{
					this.LoadFontAsset();
					this.hasFontAssetChanged = false;
					if (this.m_fontAsset == null || this.m_renderer.sharedMaterial == null)
					{
						return;
					}
					this.m_fontAsset.propertiesChanged = false;
				}
				if (this.isMaskUpdateRequired)
				{
					this.UpdateMask();
					this.isMaskUpdateRequired = false;
				}
				if (this.isInputParsingRequired || this.m_isTextTruncated)
				{
					this.ParseInputText();
				}
				if (this.m_enableAutoSizing)
				{
					this.m_fontSize = Mathf.Clamp(this.m_fontSize, this.m_fontSizeMin, this.m_fontSizeMax);
				}
				this.m_maxFontSize = this.m_fontSizeMax;
				this.m_minFontSize = this.m_fontSizeMin;
				this.m_lineSpacingDelta = 0f;
				this.m_recursiveCount = 0;
				this.m_isCharacterWrappingEnabled = false;
				this.m_isTextTruncated = false;
				this.GenerateTextMesh();
				this.havePropertiesChanged = false;
			}
			else if ((this.haveColorChanged && !this.enableVertexGradient) || this.haveFastScaleChanged)
			{
				this.GenerateFastScaleAndColor();
			}
		}

		private void GenerateFastScaleAndColor()
		{
			if (this.m_renderMode == TextRenderFlags.Render)
			{
				this.m_mesh.MarkDynamic();
				if (this.haveColorChanged)
				{
					for (int i = 0; i < this.m_vertColors.Length; i++)
					{
						this.m_tempColors[i] = this.m_fontColor;
					}
					this.m_mesh.colors = this.m_tempColors;
					this.haveColorChanged = false;
				}
				if (this.haveFastScaleChanged)
				{
					for (int j = 0; j < this.m_vertices.Length; j++)
					{
						this.m_tempVertices[j].Set(this.m_vertices[j].x * this.m_fastScale.x, this.m_vertices[j].y * this.m_fastScale.y, this.m_vertices[j].z * this.m_fastScale.z);
					}
					this.m_mesh.vertices = this.m_tempVertices;
					this.haveFastScaleChanged = false;
				}
			}
		}

		private void GenerateTextMesh()
		{
			if (this.m_fontAsset.characterDictionary == null)
			{
				Debug.Log("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + base.GetInstanceID());
				return;
			}
			if (this.m_textInfo != null)
			{
				this.m_textInfo.Clear();
			}
			else
			{
				this.m_textInfo = new TMP_TextInfo();
			}
			if (this.m_char_buffer == null || this.m_char_buffer[0] == 0)
			{
				if (this.m_vertices != null)
				{
					Array.Clear(this.m_vertices, 0, this.m_vertices.Length);
					this.m_mesh.vertices = this.m_vertices;
				}
				this.m_preferredWidth = 0f;
				this.m_preferredHeight = 0f;
				return;
			}
			int num = this.SetArraySizes(this.m_char_buffer);
			this.m_fontIndex = 0;
			this.m_fontAssetArray[this.m_fontIndex] = this.m_fontAsset;
			this.m_fontScale = this.m_fontSize / this.m_fontAssetArray[this.m_fontIndex].fontInfo.PointSize * ((!this.m_isOrthographic) ? 0.1f : 1f);
			float fontScale = this.m_fontScale;
			this.m_maxFontScale = 0f;
			float num2 = 0f;
			this.m_currentFontSize = this.m_fontSize;
			this.m_style = this.m_fontStyle;
			this.m_lineJustification = this.m_textAlignment;
			if (this.checkPaddingRequired)
			{
				this.checkPaddingRequired = false;
				this.m_padding = ShaderUtilities.GetPadding(this.m_renderer.sharedMaterials, this.m_enableExtraPadding, this.m_isUsingBold);
				this.m_alignmentPadding = ShaderUtilities.GetFontExtent(this.m_sharedMaterial);
				this.m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(this.m_sharedMaterial);
			}
			this.m_baselineOffset = 0f;
			bool flag = false;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			bool flag2 = false;
			Vector3 zero3 = Vector3.zero;
			Vector3 zero4 = Vector3.zero;
			this.m_fontColor32 = this.m_fontColor;
			this.m_htmlColor = this.m_fontColor32;
			this.m_colorStackIndex = 0;
			Array.Clear(this.m_colorStack, 0, this.m_colorStack.Length);
			this.m_lineOffset = 0f;
			this.m_lineHeight = 0f;
			this.m_cSpacing = 0f;
			this.m_monoSpacing = 0f;
			float num3 = 0f;
			this.m_xAdvance = 0f;
			this.m_indent = 0f;
			this.m_maxXAdvance = 0f;
			this.m_lineNumber = 0;
			this.m_pageNumber = 0;
			this.m_characterCount = 0;
			this.m_visibleCharacterCount = 0;
			this.m_firstVisibleCharacterOfLine = 0;
			this.m_lastVisibleCharacterOfLine = 0;
			int num4 = 0;
			Vector3[] corners = this.m_textContainer.corners;
			Vector4 margins = this.m_textContainer.margins;
			this.m_marginWidth = this.m_textContainer.rect.width - margins.z - margins.x;
			float num5 = this.m_textContainer.rect.height - margins.y - margins.w;
			float z = this.m_transform.lossyScale.z;
			this.m_preferredWidth = 0f;
			this.m_preferredHeight = 0f;
			bool flag3 = true;
			bool flag4 = false;
			this.m_SavedWordWrapState = default(WordWrapState);
			this.m_SavedLineState = default(WordWrapState);
			int num6 = 0;
			this.m_meshExtents = new Extents(this.k_InfinityVector, -this.k_InfinityVector);
			if (this.m_textInfo.lineInfo == null)
			{
				this.m_textInfo.lineInfo = new TMP_LineInfo[2];
			}
			for (int i = 0; i < this.m_textInfo.lineInfo.Length; i++)
			{
				this.m_textInfo.lineInfo[i] = default(TMP_LineInfo);
				this.m_textInfo.lineInfo[i].lineExtents = new Extents(this.k_InfinityVector, -this.k_InfinityVector);
				this.m_textInfo.lineInfo[i].ascender = -this.k_InfinityVector.x;
				this.m_textInfo.lineInfo[i].descender = this.k_InfinityVector.x;
			}
			this.m_maxAscender = 0f;
			this.m_maxDescender = 0f;
			this.m_isNewPage = false;
			float num7 = 0f;
			this.loopCountA++;
			int num8 = 0;
			int num9 = 0;
			while (this.m_char_buffer[num9] != 0)
			{
				int num10 = this.m_char_buffer[num9];
				this.loopCountE++;
				if (this.m_isRichText && num10 == 60 && this.ValidateHtmlTag(this.m_char_buffer, num9 + 1, out num8))
				{
					num9 = num8;
					if (this.m_isRecalculateScaleRequired)
					{
						this.m_fontScale = this.m_currentFontSize / this.m_fontAssetArray[this.m_fontIndex].fontInfo.PointSize * ((!this.m_isOrthographic) ? 0.1f : 1f);
						this.m_isRecalculateScaleRequired = false;
					}
				}
				else
				{
					bool flag5 = false;
					if ((this.m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
					{
						if (char.IsLower((char)num10))
						{
							num10 -= 32;
						}
					}
					else if ((this.m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
					{
						if (char.IsUpper((char)num10))
						{
							num10 += 32;
						}
					}
					else if ((this.m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (this.m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps)
					{
						if (char.IsLower((char)num10))
						{
							this.m_fontScale = this.m_currentFontSize * 0.8f / this.m_fontAssetArray[this.m_fontIndex].fontInfo.PointSize * ((!this.m_isOrthographic) ? 0.1f : 1f);
							num10 -= 32;
						}
						else
						{
							this.m_fontScale = this.m_currentFontSize / this.m_fontAssetArray[this.m_fontIndex].fontInfo.PointSize * ((!this.m_isOrthographic) ? 0.1f : 1f);
						}
					}
					this.m_fontAssetArray[this.m_fontIndex].characterDictionary.TryGetValue(num10, out this.m_cached_GlyphInfo);
					if (this.m_cached_GlyphInfo == null)
					{
						if (char.IsLower((char)num10))
						{
							if (this.m_fontAssetArray[this.m_fontIndex].characterDictionary.TryGetValue(num10 - 32, out this.m_cached_GlyphInfo))
							{
								num10 -= 32;
							}
						}
						else if (char.IsUpper((char)num10) && this.m_fontAssetArray[this.m_fontIndex].characterDictionary.TryGetValue(num10 + 32, out this.m_cached_GlyphInfo))
						{
							num10 += 32;
						}
						if (this.m_cached_GlyphInfo == null)
						{
							this.m_fontAssetArray[this.m_fontIndex].characterDictionary.TryGetValue(88, out this.m_cached_GlyphInfo);
							if (this.m_cached_GlyphInfo == null)
							{
								goto IL_29A1;
							}
							num10 = 88;
							flag5 = true;
						}
					}
					this.m_textInfo.characterInfo[this.m_characterCount].character = (char)num10;
					this.m_textInfo.characterInfo[this.m_characterCount].color = this.m_htmlColor;
					this.m_textInfo.characterInfo[this.m_characterCount].style = this.m_style;
					this.m_textInfo.characterInfo[this.m_characterCount].index = (short)num9;
					if (this.m_enableKerning && this.m_characterCount >= 1)
					{
						int character = (int)this.m_textInfo.characterInfo[this.m_characterCount - 1].character;
						KerningPairKey kerningPairKey = new KerningPairKey(character, num10);
						KerningPair kerningPair;
						this.m_fontAssetArray[this.m_fontIndex].kerningDictionary.TryGetValue(kerningPairKey.key, out kerningPair);
						if (kerningPair != null)
						{
							this.m_xAdvance += kerningPair.XadvanceOffset * this.m_fontScale;
						}
					}
					if (this.m_monoSpacing != 0f && this.m_xAdvance != 0f)
					{
						this.m_xAdvance -= (this.m_cached_GlyphInfo.width / 2f + this.m_cached_GlyphInfo.xOffset) * this.m_fontScale;
					}
					float num11;
					float num12;
					if ((this.m_style & FontStyles.Bold) == FontStyles.Bold || (this.m_fontStyle & FontStyles.Bold) == FontStyles.Bold)
					{
						num11 = this.m_fontAssetArray[this.m_fontIndex].BoldStyle * 2f;
						num12 = 1.07f;
					}
					else
					{
						num11 = this.m_fontAssetArray[this.m_fontIndex].NormalStyle * 2f;
						num12 = 1f;
					}
					Vector3 vector = new Vector3(0f + this.m_xAdvance + (this.m_cached_GlyphInfo.xOffset - this.m_padding - num11) * this.m_fontScale, (this.m_cached_GlyphInfo.yOffset + this.m_padding) * this.m_fontScale - this.m_lineOffset + this.m_baselineOffset, 0f);
					Vector3 vector2 = new Vector3(vector.x, vector.y - (this.m_cached_GlyphInfo.height + this.m_padding * 2f) * this.m_fontScale, 0f);
					Vector3 vector3 = new Vector3(vector2.x + (this.m_cached_GlyphInfo.width + this.m_padding * 2f + num11 * 2f) * this.m_fontScale, vector.y, 0f);
					Vector3 vector4 = new Vector3(vector3.x, vector2.y, 0f);
					if ((this.m_style & FontStyles.Italic) == FontStyles.Italic || (this.m_fontStyle & FontStyles.Italic) == FontStyles.Italic)
					{
						float num13 = (float)this.m_fontAssetArray[this.m_fontIndex].ItalicStyle * 0.01f;
						Vector3 b = new Vector3(num13 * ((this.m_cached_GlyphInfo.yOffset + this.m_padding + num11) * this.m_fontScale), 0f, 0f);
						Vector3 b2 = new Vector3(num13 * ((this.m_cached_GlyphInfo.yOffset - this.m_cached_GlyphInfo.height - this.m_padding - num11) * this.m_fontScale), 0f, 0f);
						vector += b;
						vector2 += b2;
						vector3 += b;
						vector4 += b2;
					}
					this.m_textInfo.characterInfo[this.m_characterCount].topLeft = vector;
					this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft = vector2;
					this.m_textInfo.characterInfo[this.m_characterCount].topRight = vector3;
					this.m_textInfo.characterInfo[this.m_characterCount].bottomRight = vector4;
					float num14 = (this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_fontScale + this.m_baselineOffset;
					if (this.m_lineNumber == 0)
					{
						this.m_maxAscender = ((this.m_maxAscender <= num14) ? num14 : this.m_maxAscender);
					}
					if (this.m_lineOffset == 0f)
					{
						num7 = ((num7 <= num14) ? num14 : num7);
					}
					float num15 = (this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_fontScale - this.m_lineOffset + this.m_baselineOffset;
					this.m_textInfo.characterInfo[this.m_characterCount].isVisible = false;
					if (num10 != 32 && num10 != 9 && num10 != 10 && num10 != 13)
					{
						int num16 = this.m_visibleCharacterCount * 4;
						this.m_textInfo.characterInfo[this.m_characterCount].isVisible = true;
						this.m_textInfo.characterInfo[this.m_characterCount].vertexIndex = (short)(0 + num16);
						if (this.m_vertices != null)
						{
							this.m_vertices[0 + num16] = this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft;
							this.m_vertices[1 + num16] = this.m_textInfo.characterInfo[this.m_characterCount].topLeft;
							this.m_vertices[2 + num16] = this.m_textInfo.characterInfo[this.m_characterCount].bottomRight;
							this.m_vertices[3 + num16] = this.m_textInfo.characterInfo[this.m_characterCount].topRight;
						}
						if (this.m_baselineOffset == 0f)
						{
							this.m_maxFontScale = Mathf.Max(this.m_maxFontScale, this.m_fontScale);
						}
						if (this.m_xAdvance + this.m_cached_GlyphInfo.xAdvance * this.m_fontScale > this.m_marginWidth + 0.0001f && !this.m_textContainer.isDefaultWidth)
						{
							num4 = this.m_characterCount - 1;
							if (this.enableWordWrapping && this.m_characterCount != this.m_firstVisibleCharacterOfLine)
							{
								if (num6 == this.m_SavedWordWrapState.previous_WordBreak || flag3)
								{
									if (this.m_enableAutoSizing && this.m_fontSize > this.m_fontSizeMin)
									{
										this.m_maxFontSize = this.m_fontSize;
										this.m_fontSize -= Mathf.Max((this.m_fontSize - this.m_minFontSize) / 2f, 0.05f);
										this.m_fontSize = (float)((int)(Mathf.Max(this.m_fontSize, this.m_fontSizeMin) * 20f + 0.5f)) / 20f;
										if (this.loopCountA > 20)
										{
											return;
										}
										this.GenerateTextMesh();
										return;
									}
									else
									{
										if (!this.m_isCharacterWrappingEnabled)
										{
											this.m_isCharacterWrappingEnabled = true;
										}
										else
										{
											flag4 = true;
										}
										this.m_recursiveCount++;
										if (this.m_recursiveCount > 20)
										{
											goto IL_29A1;
										}
									}
								}
								num9 = this.RestoreWordWrappingState(ref this.m_SavedWordWrapState);
								num6 = num9;
								if (this.m_lineNumber > 0 && this.m_maxFontScale != 0f && this.m_lineHeight == 0f && this.m_maxFontScale != num2 && !this.m_isNewPage)
								{
									float num17 = this.m_fontAssetArray[this.m_fontIndex].fontInfo.LineHeight - (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender - this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender);
									float num18 = (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender + num17 + this.m_lineSpacing + this.m_lineSpacingDelta) * this.m_maxFontScale - (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender - num17) * num2;
									this.m_lineOffset += num18 - num3;
									this.AdjustLineOffset(this.m_firstVisibleCharacterOfLine, this.m_characterCount - 1, num18 - num3);
								}
								this.m_isNewPage = false;
								float num19 = (this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_maxFontScale - this.m_lineOffset;
								float num20 = (this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_fontScale - this.m_lineOffset + this.m_baselineOffset;
								num19 = ((num19 <= num20) ? num20 : num19);
								float num21 = (this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_maxFontScale - this.m_lineOffset;
								float num22 = (this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_fontScale - this.m_lineOffset + this.m_baselineOffset;
								num21 = ((num21 >= num22) ? num22 : num21);
								if (this.m_textInfo.characterInfo[this.m_firstVisibleCharacterOfLine].isVisible)
								{
									this.m_maxDescender = ((this.m_maxDescender >= num21) ? num21 : this.m_maxDescender);
								}
								this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex = this.m_firstVisibleCharacterOfLine;
								this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex = ((this.m_characterCount - 1 <= 0) ? 1 : (this.m_characterCount - 1));
								this.m_textInfo.lineInfo[this.m_lineNumber].lastVisibleCharacterIndex = this.m_lastVisibleCharacterOfLine;
								this.m_textInfo.lineInfo[this.m_lineNumber].characterCount = this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex - this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex + 1;
								this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_firstVisibleCharacterOfLine].bottomLeft.x, num21);
								this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].topRight.x, num19);
								this.m_textInfo.lineInfo[this.m_lineNumber].lineLength = this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max.x - this.m_padding * this.m_maxFontScale;
								this.m_textInfo.lineInfo[this.m_lineNumber].maxAdvance = this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].xAdvance - this.m_characterSpacing * this.m_fontScale;
								this.m_firstVisibleCharacterOfLine = this.m_characterCount;
								this.m_preferredWidth += this.m_xAdvance;
								if (this.m_enableWordWrapping)
								{
									this.m_preferredHeight = this.m_maxAscender - this.m_maxDescender;
								}
								else
								{
									this.m_preferredHeight = Mathf.Max(this.m_preferredHeight, num19 - num21);
								}
								this.SaveWordWrappingState(ref this.m_SavedLineState, num9, this.m_characterCount - 1);
								this.m_lineNumber++;
								if (this.m_lineNumber >= this.m_textInfo.lineInfo.Length)
								{
									this.ResizeLineExtents(this.m_lineNumber);
								}
								if (this.m_lineHeight == 0f)
								{
									num3 = (this.m_fontAssetArray[this.m_fontIndex].fontInfo.LineHeight + this.m_lineSpacing + this.m_lineSpacingDelta) * this.m_fontScale;
									this.m_lineOffset += num3;
								}
								else
								{
									this.m_lineOffset += (this.m_lineHeight + this.m_lineSpacing) * fontScale;
								}
								num2 = this.m_fontScale;
								this.m_xAdvance = 0f + this.m_indent;
								this.m_maxFontScale = 0f;
								goto IL_29A1;
							}
							if (this.m_enableAutoSizing && this.m_fontSize > this.m_fontSizeMin)
							{
								this.m_maxFontSize = this.m_fontSize;
								this.m_fontSize -= Mathf.Max((this.m_fontSize - this.m_minFontSize) / 2f, 0.05f);
								this.m_fontSize = (float)((int)(Mathf.Max(this.m_fontSize, this.m_fontSizeMin) * 20f + 0.5f)) / 20f;
								this.m_recursiveCount = 0;
								if (this.loopCountA > 20)
								{
									return;
								}
								this.GenerateTextMesh();
								return;
							}
							else
							{
								switch (this.m_overflowMode)
								{
								case TextOverflowModes.Overflow:
									if (this.m_isMaskingEnabled)
									{
										this.DisableMasking();
									}
									break;
								case TextOverflowModes.Ellipsis:
									if (this.m_isMaskingEnabled)
									{
										this.DisableMasking();
									}
									this.m_isTextTruncated = true;
									if (this.m_characterCount >= 1)
									{
										this.m_char_buffer[num9 - 1] = 8230;
										this.m_char_buffer[num9] = 0;
										this.GenerateTextMesh();
										return;
									}
									this.m_textInfo.characterInfo[this.m_characterCount].isVisible = false;
									this.m_visibleCharacterCount--;
									break;
								case TextOverflowModes.Masking:
									if (!this.m_isMaskingEnabled)
									{
										this.EnableMasking();
									}
									break;
								case TextOverflowModes.Truncate:
									if (this.m_isMaskingEnabled)
									{
										this.DisableMasking();
									}
									this.m_textInfo.characterInfo[this.m_characterCount].isVisible = false;
									break;
								case TextOverflowModes.ScrollRect:
									if (!this.m_isMaskingEnabled)
									{
										this.EnableMasking();
									}
									break;
								}
							}
						}
						Color32 color;
						if (flag5)
						{
							color = Color.red;
						}
						else if (this.m_overrideHtmlColors)
						{
							color = this.m_fontColor32;
						}
						else
						{
							color = this.m_htmlColor;
						}
						color.a = ((this.m_fontColor32.a >= color.a) ? color.a : this.m_fontColor32.a);
						if (!this.m_enableVertexGradient)
						{
							this.m_vertColors[0 + num16] = color;
							this.m_vertColors[1 + num16] = color;
							this.m_vertColors[2 + num16] = color;
							this.m_vertColors[3 + num16] = color;
						}
						else
						{
							if (!this.m_overrideHtmlColors && !this.m_htmlColor.CompareRGB(this.m_fontColor32))
							{
								this.m_vertColors[0 + num16] = color;
								this.m_vertColors[1 + num16] = color;
								this.m_vertColors[2 + num16] = color;
								this.m_vertColors[3 + num16] = color;
							}
							else
							{
								this.m_vertColors[0 + num16] = this.m_fontColorGradient.bottomLeft;
								this.m_vertColors[1 + num16] = this.m_fontColorGradient.topLeft;
								this.m_vertColors[2 + num16] = this.m_fontColorGradient.bottomRight;
								this.m_vertColors[3 + num16] = this.m_fontColorGradient.topRight;
							}
							this.m_vertColors[0 + num16].a = color.a;
							this.m_vertColors[1 + num16].a = color.a;
							this.m_vertColors[2 + num16].a = color.a;
							this.m_vertColors[3 + num16].a = color.a;
						}
						if (!this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_WeightNormal))
						{
							num11 = 0f;
						}
						Vector2 vector5 = new Vector2((this.m_cached_GlyphInfo.x - this.m_padding - num11) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.AtlasWidth, 1f - (this.m_cached_GlyphInfo.y + this.m_padding + num11 + this.m_cached_GlyphInfo.height) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.AtlasHeight);
						Vector2 vector6 = new Vector2(vector5.x, 1f - (this.m_cached_GlyphInfo.y - this.m_padding - num11) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.AtlasHeight);
						Vector2 vector7 = new Vector2((this.m_cached_GlyphInfo.x + this.m_padding + num11 + this.m_cached_GlyphInfo.width) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.AtlasWidth, vector5.y);
						Vector2 vector8 = new Vector2(vector7.x, vector6.y);
						this.m_uvs[0 + num16] = vector5;
						this.m_uvs[1 + num16] = vector6;
						this.m_uvs[2 + num16] = vector7;
						this.m_uvs[3 + num16] = vector8;
						this.m_visibleCharacterCount++;
						if (this.m_textInfo.characterInfo[this.m_characterCount].isVisible)
						{
							this.m_lastVisibleCharacterOfLine = this.m_characterCount;
						}
					}
					else if (num10 == 9 || num10 == 32)
					{
						TMP_LineInfo[] expr_19CC_cp_0 = this.m_textInfo.lineInfo;
						int expr_19CC_cp_1 = this.m_lineNumber;
						expr_19CC_cp_0[expr_19CC_cp_1].spaceCount = expr_19CC_cp_0[expr_19CC_cp_1].spaceCount + 1;
						this.m_textInfo.spaceCount++;
					}
					this.m_textInfo.characterInfo[this.m_characterCount].lineNumber = (short)this.m_lineNumber;
					this.m_textInfo.characterInfo[this.m_characterCount].pageNumber = (short)this.m_pageNumber;
					if ((num10 != 10 && num10 != 13 && num10 != 8230) || this.m_textInfo.lineInfo[this.m_lineNumber].characterCount == 1)
					{
						this.m_textInfo.lineInfo[this.m_lineNumber].alignment = this.m_lineJustification;
					}
					if (this.m_maxAscender - num15 + this.m_alignmentPadding.w * 2f * this.m_fontScale > num5 && !this.m_textContainer.isDefaultHeight)
					{
						if (this.m_enableAutoSizing && this.m_lineSpacingDelta > this.m_lineSpacingMax)
						{
							this.m_lineSpacingDelta -= 1f;
							this.GenerateTextMesh();
							return;
						}
						if (this.m_enableAutoSizing && this.m_fontSize > this.m_fontSizeMin)
						{
							this.m_maxFontSize = this.m_fontSize;
							this.m_fontSize -= Mathf.Max((this.m_fontSize - this.m_minFontSize) / 2f, 0.05f);
							this.m_fontSize = (float)((int)(Mathf.Max(this.m_fontSize, this.m_fontSizeMin) * 20f + 0.5f)) / 20f;
							this.m_recursiveCount = 0;
							if (this.loopCountA > 20)
							{
								return;
							}
							this.GenerateTextMesh();
							return;
						}
						else
						{
							switch (this.m_overflowMode)
							{
							case TextOverflowModes.Overflow:
								if (this.m_isMaskingEnabled)
								{
									this.DisableMasking();
								}
								break;
							case TextOverflowModes.Ellipsis:
								if (this.m_isMaskingEnabled)
								{
									this.DisableMasking();
								}
								if (this.m_lineNumber > 0)
								{
									this.m_char_buffer[(int)this.m_textInfo.characterInfo[num4].index] = 8230;
									this.m_char_buffer[(int)(this.m_textInfo.characterInfo[num4].index + 1)] = 0;
									this.GenerateTextMesh();
									this.m_isTextTruncated = true;
									return;
								}
								this.m_char_buffer[0] = 0;
								this.GenerateTextMesh();
								this.m_isTextTruncated = true;
								return;
							case TextOverflowModes.Masking:
								if (!this.m_isMaskingEnabled)
								{
									this.EnableMasking();
								}
								break;
							case TextOverflowModes.Truncate:
								if (this.m_isMaskingEnabled)
								{
									this.DisableMasking();
								}
								if (this.m_lineNumber > 0)
								{
									this.m_char_buffer[(int)(this.m_textInfo.characterInfo[num4].index + 1)] = 0;
									this.GenerateTextMesh();
									this.m_isTextTruncated = true;
									return;
								}
								this.m_char_buffer[0] = 0;
								this.GenerateTextMesh();
								this.m_isTextTruncated = true;
								return;
							case TextOverflowModes.ScrollRect:
								if (!this.m_isMaskingEnabled)
								{
									this.EnableMasking();
								}
								break;
							case TextOverflowModes.Page:
								if (this.m_isMaskingEnabled)
								{
									this.DisableMasking();
								}
								if (num10 != 13 && num10 != 10)
								{
									num9 = this.RestoreWordWrappingState(ref this.m_SavedLineState);
									if (num9 == 0)
									{
										this.m_char_buffer[0] = 0;
										this.GenerateTextMesh();
										this.m_isTextTruncated = true;
										return;
									}
									this.m_isNewPage = true;
									this.m_xAdvance = 0f + this.m_indent;
									this.m_lineOffset = 0f;
									this.m_lineNumber++;
									this.m_pageNumber++;
									goto IL_29A1;
								}
								break;
							}
						}
					}
					if (num10 == 9)
					{
						this.m_xAdvance += this.m_fontAsset.fontInfo.TabWidth * this.m_fontScale * (float)this.m_fontAsset.TabSize;
					}
					else if (this.m_monoSpacing != 0f)
					{
						this.m_xAdvance += (this.m_monoSpacing + this.m_cached_GlyphInfo.width / 2f + this.m_cached_GlyphInfo.xOffset + this.m_characterSpacing + this.m_cSpacing) * this.m_fontScale;
					}
					else
					{
						this.m_xAdvance += this.m_cached_GlyphInfo.xAdvance * num12 * this.m_fontScale + (this.m_characterSpacing + this.m_cSpacing) * this.m_fontScale;
					}
					this.m_textInfo.characterInfo[this.m_characterCount].xAdvance = this.m_xAdvance;
					if (num10 == 13)
					{
						this.m_maxXAdvance = Mathf.Max(this.m_maxXAdvance, this.m_preferredWidth + this.m_xAdvance + this.m_alignmentPadding.z * this.m_fontScale);
						this.m_preferredWidth = 0f;
						this.m_xAdvance = 0f + this.m_indent;
					}
					if (num10 == 10 || this.m_characterCount == num - 1)
					{
						if (this.m_lineNumber > 0 && this.m_maxFontScale != 0f && this.m_lineHeight == 0f && this.m_maxFontScale != num2 && !this.m_isNewPage)
						{
							float num23 = this.m_fontAssetArray[this.m_fontIndex].fontInfo.LineHeight - (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender - this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender);
							float num24 = (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender + num23 + this.m_lineSpacing + this.m_paragraphSpacing + this.m_lineSpacingDelta) * this.m_maxFontScale - (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender - num23) * num2;
							this.m_lineOffset += num24 - num3;
							this.AdjustLineOffset(this.m_firstVisibleCharacterOfLine, this.m_characterCount, num24 - num3);
						}
						this.m_isNewPage = false;
						float num25 = (this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_maxFontScale - this.m_lineOffset;
						float num26 = (this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_fontScale - this.m_lineOffset + this.m_baselineOffset;
						num25 = ((num25 <= num26) ? num26 : num25);
						float num27 = (this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_maxFontScale - this.m_lineOffset;
						float num28 = (this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_fontScale - this.m_lineOffset + this.m_baselineOffset;
						num27 = ((num27 >= num28) ? num28 : num27);
						this.m_maxDescender = ((this.m_maxDescender >= num27) ? num27 : this.m_maxDescender);
						this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex = this.m_firstVisibleCharacterOfLine;
						this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex = this.m_characterCount;
						this.m_textInfo.lineInfo[this.m_lineNumber].lastVisibleCharacterIndex = this.m_lastVisibleCharacterOfLine;
						this.m_textInfo.lineInfo[this.m_lineNumber].characterCount = this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex - this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex + 1;
						this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_firstVisibleCharacterOfLine].bottomLeft.x, num27);
						this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].topRight.x, num25);
						this.m_textInfo.lineInfo[this.m_lineNumber].lineLength = this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max.x - this.m_padding * this.m_maxFontScale;
						this.m_textInfo.lineInfo[this.m_lineNumber].maxAdvance = this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].xAdvance - this.m_characterSpacing * this.m_fontScale;
						this.m_firstVisibleCharacterOfLine = this.m_characterCount + 1;
						if (num10 == 10 && this.m_characterCount != num - 1)
						{
							this.m_maxXAdvance = Mathf.Max(this.m_maxXAdvance, this.m_preferredWidth + this.m_xAdvance + this.m_alignmentPadding.z * this.m_fontScale);
							this.m_preferredWidth = 0f;
						}
						else
						{
							this.m_preferredWidth = Mathf.Max(this.m_maxXAdvance, this.m_preferredWidth + this.m_xAdvance + this.m_alignmentPadding.z * this.m_fontScale);
						}
						this.m_preferredHeight = this.m_maxAscender - this.m_maxDescender;
						if (num10 == 10)
						{
							this.SaveWordWrappingState(ref this.m_SavedLineState, num9, this.m_characterCount);
							this.SaveWordWrappingState(ref this.m_SavedWordWrapState, num9, this.m_characterCount);
							this.m_lineNumber++;
							if (this.m_lineNumber >= this.m_textInfo.lineInfo.Length)
							{
								this.ResizeLineExtents(this.m_lineNumber);
							}
							if (this.m_lineHeight == 0f)
							{
								num3 = (this.m_fontAssetArray[this.m_fontIndex].fontInfo.LineHeight + this.m_paragraphSpacing + this.m_lineSpacing + this.m_lineSpacingDelta) * this.m_fontScale;
								this.m_lineOffset += num3;
							}
							else
							{
								this.m_lineOffset += (this.m_lineHeight + this.m_paragraphSpacing + this.m_lineSpacing) * fontScale;
							}
							num2 = this.m_fontScale;
							this.m_maxFontScale = 0f;
							this.m_xAdvance = 0f + this.m_indent;
							num4 = this.m_characterCount - 1;
						}
					}
					this.m_textInfo.characterInfo[this.m_characterCount].baseLine = this.m_textInfo.characterInfo[this.m_characterCount].topRight.y - (this.m_cached_GlyphInfo.yOffset + this.m_padding) * this.m_fontScale;
					this.m_textInfo.characterInfo[this.m_characterCount].topLine = this.m_textInfo.characterInfo[this.m_characterCount].baseLine + (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_fontScale;
					this.m_textInfo.characterInfo[this.m_characterCount].bottomLine = this.m_textInfo.characterInfo[this.m_characterCount].baseLine + (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender - this.m_alignmentPadding.w) * this.m_fontScale;
					this.m_textInfo.characterInfo[this.m_characterCount].padding = this.m_padding * this.m_fontScale;
					this.m_textInfo.characterInfo[this.m_characterCount].aspectRatio = this.m_cached_GlyphInfo.width / this.m_cached_GlyphInfo.height;
					this.m_textInfo.characterInfo[this.m_characterCount].scale = this.m_fontScale;
					if (this.m_textInfo.characterInfo[this.m_characterCount].isVisible)
					{
						this.m_meshExtents.min = new Vector2(Mathf.Min(this.m_meshExtents.min.x, this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft.x), Mathf.Min(this.m_meshExtents.min.y, this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft.y));
						this.m_meshExtents.max = new Vector2(Mathf.Max(this.m_meshExtents.max.x, this.m_textInfo.characterInfo[this.m_characterCount].topRight.x), Mathf.Max(this.m_meshExtents.max.y, this.m_textInfo.characterInfo[this.m_characterCount].topLeft.y));
					}
					if (num10 != 13 && num10 != 10 && this.m_pageNumber < 16)
					{
						this.m_textInfo.pageInfo[this.m_pageNumber].ascender = num7;
						this.m_textInfo.pageInfo[this.m_pageNumber].descender = ((num15 >= this.m_textInfo.pageInfo[this.m_pageNumber].descender) ? this.m_textInfo.pageInfo[this.m_pageNumber].descender : num15);
						if (this.m_pageNumber == 0 && this.m_characterCount == 0)
						{
							this.m_textInfo.pageInfo[this.m_pageNumber].firstCharacterIndex = this.m_characterCount;
						}
						else if (this.m_characterCount > 0 && this.m_pageNumber != (int)this.m_textInfo.characterInfo[this.m_characterCount - 1].pageNumber)
						{
							this.m_textInfo.pageInfo[this.m_pageNumber - 1].lastCharacterIndex = this.m_characterCount - 1;
							this.m_textInfo.pageInfo[this.m_pageNumber].firstCharacterIndex = this.m_characterCount;
						}
						else if (this.m_characterCount == num - 1)
						{
							this.m_textInfo.pageInfo[this.m_pageNumber].lastCharacterIndex = this.m_characterCount;
						}
					}
					if (this.m_enableWordWrapping || this.m_overflowMode == TextOverflowModes.Truncate || this.m_overflowMode == TextOverflowModes.Ellipsis)
					{
						if (num10 == 9 || num10 == 32)
						{
							this.SaveWordWrappingState(ref this.m_SavedWordWrapState, num9, this.m_characterCount);
							this.m_isCharacterWrappingEnabled = false;
							flag3 = false;
						}
						else if (((flag3 || this.m_isCharacterWrappingEnabled) && this.m_characterCount < num - 1 && !this.m_fontAsset.lineBreakingInfo.leadingCharacters.ContainsKey(num10) && !this.m_fontAsset.lineBreakingInfo.followingCharacters.ContainsKey((int)this.m_VisibleCharacters[this.m_characterCount + 1])) || flag4)
						{
							this.SaveWordWrappingState(ref this.m_SavedWordWrapState, num9, this.m_characterCount);
						}
					}
					this.m_characterCount++;
				}
				IL_29A1:
				num9++;
			}
			float num29 = this.m_maxFontSize - this.m_minFontSize;
			if ((!this.m_textContainer.isDefaultWidth || !this.m_textContainer.isDefaultHeight) && !this.m_isCharacterWrappingEnabled && this.m_enableAutoSizing && num29 > 0.051f && this.m_fontSize < this.m_fontSizeMax)
			{
				this.m_minFontSize = this.m_fontSize;
				this.m_fontSize += Mathf.Max((this.m_maxFontSize - this.m_fontSize) / 2f, 0.05f);
				this.m_fontSize = (float)((int)(Mathf.Min(this.m_fontSize, this.m_fontSizeMax) * 20f + 0.5f)) / 20f;
				if (this.loopCountA > 20)
				{
					return;
				}
				this.GenerateTextMesh();
				return;
			}
			else
			{
				if (this.m_characterCount < this.m_textInfo.characterInfo.Length)
				{
					this.m_textInfo.characterInfo[this.m_characterCount].character = '\0';
				}
				this.m_isCharacterWrappingEnabled = false;
				if (this.m_renderMode == TextRenderFlags.GetPreferredSizes)
				{
					return;
				}
				if (this.m_visibleCharacterCount == 0)
				{
					if (this.m_vertices != null)
					{
						Array.Clear(this.m_vertices, 0, this.m_vertices.Length);
						this.m_mesh.vertices = this.m_vertices;
					}
					return;
				}
				int num30 = this.m_visibleCharacterCount * 4;
				Array.Clear(this.m_vertices, num30, this.m_vertices.Length - num30);
				switch (this.m_textAlignment)
				{
				case TextAlignmentOptions.TopLeft:
				case TextAlignmentOptions.Top:
				case TextAlignmentOptions.TopRight:
				case TextAlignmentOptions.TopJustified:
					if (this.m_overflowMode != TextOverflowModes.Page)
					{
						this.m_anchorOffset = corners[1] + new Vector3(0f + margins.x, 0f - this.m_maxAscender - margins.y, 0f);
					}
					else
					{
						this.m_anchorOffset = corners[1] + new Vector3(0f + margins.x, 0f - this.m_textInfo.pageInfo[this.m_pageToDisplay].ascender - margins.y, 0f);
					}
					break;
				case TextAlignmentOptions.Left:
				case TextAlignmentOptions.Center:
				case TextAlignmentOptions.Right:
				case TextAlignmentOptions.Justified:
					if (this.m_overflowMode != TextOverflowModes.Page)
					{
						this.m_anchorOffset = (corners[0] + corners[1]) / 2f + new Vector3(0f + margins.x, 0f - (this.m_maxAscender + margins.y + this.m_maxDescender - margins.w) / 2f, 0f);
					}
					else
					{
						this.m_anchorOffset = (corners[0] + corners[1]) / 2f + new Vector3(0f + margins.x, 0f - (this.m_textInfo.pageInfo[this.m_pageToDisplay].ascender + margins.y + this.m_textInfo.pageInfo[this.m_pageToDisplay].descender - margins.w) / 2f, 0f);
					}
					break;
				case TextAlignmentOptions.BottomLeft:
				case TextAlignmentOptions.Bottom:
				case TextAlignmentOptions.BottomRight:
				case TextAlignmentOptions.BottomJustified:
					if (this.m_overflowMode != TextOverflowModes.Page)
					{
						this.m_anchorOffset = corners[0] + new Vector3(0f + margins.x, 0f - this.m_maxDescender + margins.w, 0f);
					}
					else
					{
						this.m_anchorOffset = corners[0] + new Vector3(0f + margins.x, 0f - this.m_textInfo.pageInfo[this.m_pageToDisplay].descender + margins.w, 0f);
					}
					break;
				case TextAlignmentOptions.BaselineLeft:
				case TextAlignmentOptions.Baseline:
				case TextAlignmentOptions.BaselineRight:
					this.m_anchorOffset = (corners[0] + corners[1]) / 2f + new Vector3(0f + margins.x, 0f, 0f);
					break;
				case TextAlignmentOptions.MidlineLeft:
				case TextAlignmentOptions.Midline:
				case TextAlignmentOptions.MidlineRight:
				case TextAlignmentOptions.MidlineJustified:
					this.m_anchorOffset = (corners[0] + corners[1]) / 2f + new Vector3(0f + margins.x, 0f - (this.m_meshExtents.max.y + margins.y + this.m_meshExtents.min.y - margins.w) / 2f, 0f);
					break;
				}
				Vector3 vector9 = Vector3.zero;
				Vector3 b3 = Vector3.zero;
				int num31 = 0;
				int num32 = 0;
				int num33 = 0;
				int num34 = 0;
				Color32 color2 = new Color32(255, 255, 255, 127);
				bool flag6 = false;
				int num35 = 0;
				for (int j = 0; j < this.m_characterCount; j++)
				{
					int lineNumber = (int)this.m_textInfo.characterInfo[j].lineNumber;
					char character2 = this.m_textInfo.characterInfo[j].character;
					TMP_LineInfo tMP_LineInfo = this.m_textInfo.lineInfo[lineNumber];
					TextAlignmentOptions alignment = tMP_LineInfo.alignment;
					num33 = lineNumber + 1;
					switch (alignment)
					{
					case TextAlignmentOptions.TopLeft:
					case TextAlignmentOptions.Left:
					case TextAlignmentOptions.BottomLeft:
					case TextAlignmentOptions.BaselineLeft:
					case TextAlignmentOptions.MidlineLeft:
						vector9 = Vector3.zero;
						break;
					case TextAlignmentOptions.Top:
					case TextAlignmentOptions.Center:
					case TextAlignmentOptions.Bottom:
					case TextAlignmentOptions.Baseline:
					case TextAlignmentOptions.Midline:
						vector9 = new Vector3(this.m_marginWidth / 2f - tMP_LineInfo.maxAdvance / 2f, 0f, 0f);
						break;
					case TextAlignmentOptions.TopRight:
					case TextAlignmentOptions.Right:
					case TextAlignmentOptions.BottomRight:
					case TextAlignmentOptions.BaselineRight:
					case TextAlignmentOptions.MidlineRight:
						vector9 = new Vector3(this.m_marginWidth - tMP_LineInfo.maxAdvance, 0f, 0f);
						break;
					case TextAlignmentOptions.TopJustified:
					case TextAlignmentOptions.Justified:
					case TextAlignmentOptions.BottomJustified:
					case TextAlignmentOptions.MidlineJustified:
					{
						int num10 = (int)this.m_textInfo.characterInfo[j].character;
						char character3 = this.m_textInfo.characterInfo[tMP_LineInfo.lastCharacterIndex].character;
						if (char.IsWhiteSpace(character3) && !char.IsControl(character3) && lineNumber < this.m_lineNumber)
						{
							float num36 = corners[3].x - margins.z - (corners[0].x + margins.x) - tMP_LineInfo.maxAdvance;
							if (lineNumber != num34 || j == 0)
							{
								vector9 = Vector3.zero;
							}
							else if (num10 == 9 || num10 == 32)
							{
								vector9 += new Vector3(num36 * (1f - this.m_wordWrappingRatios) / (float)(tMP_LineInfo.spaceCount - 1), 0f, 0f);
							}
							else
							{
								vector9 += new Vector3(num36 * this.m_wordWrappingRatios / (float)(tMP_LineInfo.characterCount - tMP_LineInfo.spaceCount - 1), 0f, 0f);
							}
						}
						else
						{
							vector9 = Vector3.zero;
						}
						break;
					}
					}
					b3 = this.m_anchorOffset + vector9;
					if (this.m_textInfo.characterInfo[j].isVisible)
					{
						Extents lineExtents = tMP_LineInfo.lineExtents;
						float num37 = this.m_uvLineOffset * (float)lineNumber % 1f + this.m_uvOffset.x;
						switch (this.m_horizontalMapping)
						{
						case TextureMappingOptions.Character:
							this.m_uv2s[num31].x = 0f + this.m_uvOffset.x;
							this.m_uv2s[num31 + 1].x = 0f + this.m_uvOffset.x;
							this.m_uv2s[num31 + 2].x = 1f + this.m_uvOffset.x;
							this.m_uv2s[num31 + 3].x = 1f + this.m_uvOffset.x;
							break;
						case TextureMappingOptions.Line:
							if (this.m_textAlignment != TextAlignmentOptions.Justified)
							{
								this.m_uv2s[num31].x = (this.m_vertices[num31].x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num37;
								this.m_uv2s[num31 + 1].x = (this.m_vertices[num31 + 1].x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num37;
								this.m_uv2s[num31 + 2].x = (this.m_vertices[num31 + 2].x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num37;
								this.m_uv2s[num31 + 3].x = (this.m_vertices[num31 + 3].x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num37;
							}
							else
							{
								this.m_uv2s[num31].x = (this.m_vertices[num31].x + vector9.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num37;
								this.m_uv2s[num31 + 1].x = (this.m_vertices[num31 + 1].x + vector9.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num37;
								this.m_uv2s[num31 + 2].x = (this.m_vertices[num31 + 2].x + vector9.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num37;
								this.m_uv2s[num31 + 3].x = (this.m_vertices[num31 + 3].x + vector9.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num37;
							}
							break;
						case TextureMappingOptions.Paragraph:
							this.m_uv2s[num31].x = (this.m_vertices[num31].x + vector9.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num37;
							this.m_uv2s[num31 + 1].x = (this.m_vertices[num31 + 1].x + vector9.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num37;
							this.m_uv2s[num31 + 2].x = (this.m_vertices[num31 + 2].x + vector9.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num37;
							this.m_uv2s[num31 + 3].x = (this.m_vertices[num31 + 3].x + vector9.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num37;
							break;
						case TextureMappingOptions.MatchAspect:
						{
							switch (this.m_verticalMapping)
							{
							case TextureMappingOptions.Character:
								this.m_uv2s[num31].y = 0f + this.m_uvOffset.y;
								this.m_uv2s[num31 + 1].y = 1f + this.m_uvOffset.y;
								this.m_uv2s[num31 + 2].y = 0f + this.m_uvOffset.y;
								this.m_uv2s[num31 + 3].y = 1f + this.m_uvOffset.y;
								break;
							case TextureMappingOptions.Line:
								this.m_uv2s[num31].y = (this.m_vertices[num31].y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num37;
								this.m_uv2s[num31 + 1].y = (this.m_vertices[num31 + 1].y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num37;
								this.m_uv2s[num31 + 2].y = this.m_uv2s[num31].y;
								this.m_uv2s[num31 + 3].y = this.m_uv2s[num31 + 1].y;
								break;
							case TextureMappingOptions.Paragraph:
								this.m_uv2s[num31].y = (this.m_vertices[num31].y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y) + num37;
								this.m_uv2s[num31 + 1].y = (this.m_vertices[num31 + 1].y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y) + num37;
								this.m_uv2s[num31 + 2].y = this.m_uv2s[num31].y;
								this.m_uv2s[num31 + 3].y = this.m_uv2s[num31 + 1].y;
								break;
							case TextureMappingOptions.MatchAspect:
								Debug.Log("ERROR: Cannot Match both Vertical & Horizontal.");
								break;
							}
							float num38 = (1f - (this.m_uv2s[num31].y + this.m_uv2s[num31 + 1].y) * this.m_textInfo.characterInfo[j].aspectRatio) / 2f;
							this.m_uv2s[num31].x = this.m_uv2s[num31].y * this.m_textInfo.characterInfo[j].aspectRatio + num38 + num37;
							this.m_uv2s[num31 + 1].x = this.m_uv2s[num31].x;
							this.m_uv2s[num31 + 2].x = this.m_uv2s[num31 + 1].y * this.m_textInfo.characterInfo[j].aspectRatio + num38 + num37;
							this.m_uv2s[num31 + 3].x = this.m_uv2s[num31 + 2].x;
							break;
						}
						}
						switch (this.m_verticalMapping)
						{
						case TextureMappingOptions.Character:
							this.m_uv2s[num31].y = 0f + this.m_uvOffset.y;
							this.m_uv2s[num31 + 1].y = 1f + this.m_uvOffset.y;
							this.m_uv2s[num31 + 2].y = 0f + this.m_uvOffset.y;
							this.m_uv2s[num31 + 3].y = 1f + this.m_uvOffset.y;
							break;
						case TextureMappingOptions.Line:
							this.m_uv2s[num31].y = (this.m_vertices[num31].y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + this.m_uvOffset.y;
							this.m_uv2s[num31 + 1].y = (this.m_vertices[num31 + 1].y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + this.m_uvOffset.y;
							this.m_uv2s[num31 + 2].y = this.m_uv2s[num31].y;
							this.m_uv2s[num31 + 3].y = this.m_uv2s[num31 + 1].y;
							break;
						case TextureMappingOptions.Paragraph:
							this.m_uv2s[num31].y = (this.m_vertices[num31].y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y) + this.m_uvOffset.y;
							this.m_uv2s[num31 + 1].y = (this.m_vertices[num31 + 1].y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y) + this.m_uvOffset.y;
							this.m_uv2s[num31 + 2].y = this.m_uv2s[num31].y;
							this.m_uv2s[num31 + 3].y = this.m_uv2s[num31 + 1].y;
							break;
						case TextureMappingOptions.MatchAspect:
						{
							float num39 = (1f - (this.m_uv2s[num31].x + this.m_uv2s[num31 + 2].x) / this.m_textInfo.characterInfo[j].aspectRatio) / 2f;
							this.m_uv2s[num31].y = num39 + this.m_uv2s[num31].x / this.m_textInfo.characterInfo[j].aspectRatio + this.m_uvOffset.y;
							this.m_uv2s[num31 + 1].y = num39 + this.m_uv2s[num31 + 2].x / this.m_textInfo.characterInfo[j].aspectRatio + this.m_uvOffset.y;
							this.m_uv2s[num31 + 2].y = this.m_uv2s[num31].y;
							this.m_uv2s[num31 + 3].y = this.m_uv2s[num31 + 1].y;
							break;
						}
						}
						float num40 = this.m_textInfo.characterInfo[j].scale * z;
						if ((this.m_textInfo.characterInfo[j].style & FontStyles.Bold) == FontStyles.Bold)
						{
							num40 *= -1f;
						}
						float num41 = this.m_uv2s[num31].x;
						float num42 = this.m_uv2s[num31].y;
						float num43 = this.m_uv2s[num31 + 3].x;
						float num44 = this.m_uv2s[num31 + 3].y;
						float num45 = Mathf.Floor(num41);
						float num46 = Mathf.Floor(num42);
						num41 -= num45;
						num43 -= num45;
						num42 -= num46;
						num44 -= num46;
						this.m_uv2s[num31] = this.PackUV(num41, num42, num40);
						this.m_uv2s[num31 + 1] = this.PackUV(num41, num44, num40);
						this.m_uv2s[num31 + 2] = this.PackUV(num43, num42, num40);
						this.m_uv2s[num31 + 3] = this.PackUV(num43, num44, num40);
						if ((this.m_maxVisibleCharacters != -1 && j >= this.m_maxVisibleCharacters) || (this.m_maxVisibleLines != -1 && lineNumber >= this.m_maxVisibleLines) || (this.m_overflowMode == TextOverflowModes.Page && (int)this.m_textInfo.characterInfo[j].pageNumber != this.m_pageToDisplay))
						{
							this.m_vertices[num31] *= 0f;
							this.m_vertices[num31 + 1] *= 0f;
							this.m_vertices[num31 + 2] *= 0f;
							this.m_vertices[num31 + 3] *= 0f;
						}
						else
						{
							this.m_vertices[num31] += b3;
							this.m_vertices[num31 + 1] += b3;
							this.m_vertices[num31 + 2] += b3;
							this.m_vertices[num31 + 3] += b3;
						}
						num31 += 4;
					}
					TMP_CharacterInfo[] expr_4206_cp_0 = this.m_textInfo.characterInfo;
					int expr_4206_cp_1 = j;
					expr_4206_cp_0[expr_4206_cp_1].bottomLeft = expr_4206_cp_0[expr_4206_cp_1].bottomLeft + b3;
					TMP_CharacterInfo[] expr_422A_cp_0 = this.m_textInfo.characterInfo;
					int expr_422A_cp_1 = j;
					expr_422A_cp_0[expr_422A_cp_1].topRight = expr_422A_cp_0[expr_422A_cp_1].topRight + b3;
					TMP_CharacterInfo[] expr_424E_cp_0 = this.m_textInfo.characterInfo;
					int expr_424E_cp_1 = j;
					expr_424E_cp_0[expr_424E_cp_1].topLine = expr_424E_cp_0[expr_424E_cp_1].topLine + b3.y;
					TMP_CharacterInfo[] expr_4273_cp_0 = this.m_textInfo.characterInfo;
					int expr_4273_cp_1 = j;
					expr_4273_cp_0[expr_4273_cp_1].bottomLine = expr_4273_cp_0[expr_4273_cp_1].bottomLine + b3.y;
					TMP_CharacterInfo[] expr_4298_cp_0 = this.m_textInfo.characterInfo;
					int expr_4298_cp_1 = j;
					expr_4298_cp_0[expr_4298_cp_1].baseLine = expr_4298_cp_0[expr_4298_cp_1].baseLine + b3.y;
					this.m_textInfo.lineInfo[lineNumber].ascender = ((this.m_textInfo.characterInfo[j].topLine <= this.m_textInfo.lineInfo[lineNumber].ascender) ? this.m_textInfo.lineInfo[lineNumber].ascender : this.m_textInfo.characterInfo[j].topLine);
					this.m_textInfo.lineInfo[lineNumber].descender = ((this.m_textInfo.characterInfo[j].bottomLine >= this.m_textInfo.lineInfo[lineNumber].descender) ? this.m_textInfo.lineInfo[lineNumber].descender : this.m_textInfo.characterInfo[j].bottomLine);
					if (lineNumber != num34 || j == this.m_characterCount - 1)
					{
						if (lineNumber != num34)
						{
							this.m_textInfo.lineInfo[num34].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[num34].firstCharacterIndex].bottomLeft.x, this.m_textInfo.lineInfo[num34].descender);
							this.m_textInfo.lineInfo[num34].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[num34].lastVisibleCharacterIndex].topRight.x, this.m_textInfo.lineInfo[num34].ascender);
						}
						if (j == this.m_characterCount - 1)
						{
							this.m_textInfo.lineInfo[lineNumber].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[lineNumber].firstCharacterIndex].bottomLeft.x, this.m_textInfo.lineInfo[lineNumber].descender);
							this.m_textInfo.lineInfo[lineNumber].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[lineNumber].lastVisibleCharacterIndex].topRight.x, this.m_textInfo.lineInfo[lineNumber].ascender);
						}
					}
					if (char.IsLetterOrDigit(character2) && j < this.m_characterCount - 1)
					{
						if (!flag6)
						{
							flag6 = true;
							num35 = j;
						}
					}
					else if (((char.IsPunctuation(character2) || char.IsWhiteSpace(character2) || j == this.m_characterCount - 1) && flag6) || j == 0)
					{
						int num47 = (j != this.m_characterCount - 1 || !char.IsLetterOrDigit(character2)) ? (j - 1) : j;
						flag6 = false;
						num32++;
						TMP_LineInfo[] expr_461F_cp_0 = this.m_textInfo.lineInfo;
						int expr_461F_cp_1 = lineNumber;
						expr_461F_cp_0[expr_461F_cp_1].wordCount = expr_461F_cp_0[expr_461F_cp_1].wordCount + 1;
						TMP_WordInfo item = default(TMP_WordInfo);
						item.firstCharacterIndex = num35;
						item.lastCharacterIndex = num47;
						item.characterCount = num47 - num35 + 1;
						this.m_textInfo.wordInfo.Add(item);
					}
					bool flag7 = (this.m_textInfo.characterInfo[j].style & FontStyles.Underline) == FontStyles.Underline;
					if (flag7)
					{
						if (!flag && character2 != ' ' && character2 != '\n' && character2 != '\r')
						{
							flag = true;
							zero = new Vector3(this.m_textInfo.characterInfo[j].bottomLeft.x, this.m_textInfo.characterInfo[j].baseLine + this.font.fontInfo.Underline * this.m_fontScale, 0f);
							color2 = this.m_textInfo.characterInfo[j].color;
						}
						if (this.m_characterCount == 1)
						{
							flag = false;
							zero2 = new Vector3(this.m_textInfo.characterInfo[j].topRight.x, this.m_textInfo.characterInfo[j].baseLine + this.font.fontInfo.Underline * this.m_fontScale, 0f);
							this.DrawUnderlineMesh(zero, zero2, ref num30, color2);
						}
						else if (j == tMP_LineInfo.lastCharacterIndex)
						{
							if (character2 == ' ' || character2 == '\n' || character2 == '\r')
							{
								int lastVisibleCharacterIndex = tMP_LineInfo.lastVisibleCharacterIndex;
								zero2 = new Vector3(this.m_textInfo.characterInfo[lastVisibleCharacterIndex].topRight.x, this.m_textInfo.characterInfo[lastVisibleCharacterIndex].baseLine + this.font.fontInfo.Underline * this.m_fontScale, 0f);
							}
							else
							{
								zero2 = new Vector3(this.m_textInfo.characterInfo[j].topRight.x, this.m_textInfo.characterInfo[j].baseLine + this.font.fontInfo.Underline * this.m_fontScale, 0f);
							}
							flag = false;
							this.DrawUnderlineMesh(zero, zero2, ref num30, color2);
						}
					}
					else if (flag)
					{
						flag = false;
						zero2 = new Vector3(this.m_textInfo.characterInfo[j - 1].topRight.x, this.m_textInfo.characterInfo[j - 1].baseLine + this.font.fontInfo.Underline * this.m_fontScale, 0f);
						this.DrawUnderlineMesh(zero, zero2, ref num30, color2);
					}
					bool flag8 = (this.m_textInfo.characterInfo[j].style & FontStyles.Strikethrough) == FontStyles.Strikethrough;
					if (flag8)
					{
						if (!flag2 && character2 != ' ' && character2 != '\n' && character2 != '\r')
						{
							flag2 = true;
							zero3 = new Vector3(this.m_textInfo.characterInfo[j].bottomLeft.x, this.m_textInfo.characterInfo[j].baseLine + (this.font.fontInfo.Ascender + this.font.fontInfo.Descender) / 2f * this.m_fontScale, 0f);
							color2 = this.m_textInfo.characterInfo[j].color;
						}
						if (this.m_characterCount == 1)
						{
							flag2 = false;
							zero4 = new Vector3(this.m_textInfo.characterInfo[j].topRight.x, this.m_textInfo.characterInfo[j].baseLine + (this.font.fontInfo.Ascender + this.font.fontInfo.Descender) / 2f * this.m_fontScale, 0f);
							this.DrawUnderlineMesh(zero3, zero4, ref num30, color2);
						}
						else if (j == tMP_LineInfo.lastCharacterIndex)
						{
							if (character2 == ' ' || character2 == '\n' || character2 == '\r')
							{
								int lastVisibleCharacterIndex2 = tMP_LineInfo.lastVisibleCharacterIndex;
								zero4 = new Vector3(this.m_textInfo.characterInfo[lastVisibleCharacterIndex2].topRight.x, this.m_textInfo.characterInfo[lastVisibleCharacterIndex2].baseLine + (this.font.fontInfo.Ascender + this.font.fontInfo.Descender) / 2f * this.m_fontScale, 0f);
							}
							else
							{
								zero4 = new Vector3(this.m_textInfo.characterInfo[j].topRight.x, this.m_textInfo.characterInfo[j].baseLine + (this.font.fontInfo.Ascender + this.font.fontInfo.Descender) / 2f * this.m_fontScale, 0f);
							}
							flag2 = false;
							this.DrawUnderlineMesh(zero3, zero4, ref num30, color2);
						}
					}
					else if (flag2)
					{
						flag2 = false;
						zero4 = new Vector3(this.m_textInfo.characterInfo[j - 1].topRight.x, this.m_textInfo.characterInfo[j - 1].baseLine + (this.font.fontInfo.Ascender + this.font.fontInfo.Descender) / 2f * this.m_fontScale, 0f);
						this.DrawUnderlineMesh(zero3, zero4, ref num30, color2);
					}
					num34 = lineNumber;
				}
				this.m_textInfo.characterCount = (int)((short)this.m_characterCount);
				this.m_textInfo.lineCount = (int)((short)num33);
				this.m_textInfo.wordCount = (int)((num32 == 0 || this.m_characterCount <= 0) ? (short)1 : ((short)num32));
				this.m_textInfo.pageCount = this.m_pageNumber;
				this.m_textInfo.meshInfo.vertices = this.m_vertices;
				this.m_textInfo.meshInfo.uv0s = this.m_uvs;
				this.m_textInfo.meshInfo.uv2s = this.m_uv2s;
				this.m_textInfo.meshInfo.vertexColors = this.m_vertColors;
				if (this.m_renderMode == TextRenderFlags.Render)
				{
					this.m_mesh.MarkDynamic();
					if (!this.haveFastScaleChanged)
					{
						this.m_mesh.vertices = this.m_vertices;
					}
					else
					{
						for (int k = 0; k < this.m_vertices.Length; k++)
						{
							this.m_tempVertices[k].Set(this.m_vertices[k].x * this.m_fastScale.x, this.m_vertices[k].y * this.m_fastScale.y, this.m_vertices[k].z * this.m_fastScale.z);
						}
						this.m_mesh.vertices = this.m_tempVertices;
					}
					this.m_mesh.uv = this.m_uvs;
					this.m_mesh.uv2 = this.m_uv2s;
					this.m_mesh.colors32 = this.m_vertColors;
				}
				this.m_mesh.RecalculateBounds();
				if ((this.m_textContainer.isDefaultWidth || this.m_textContainer.isDefaultHeight) && this.m_textContainer.isAutoFitting)
				{
					if (this.m_textContainer.isDefaultWidth)
					{
						this.m_textContainer.width = this.m_preferredWidth + margins.x + margins.z;
					}
					if (this.m_textContainer.isDefaultHeight)
					{
						this.m_textContainer.height = this.m_preferredHeight + margins.y + margins.w;
					}
					if (this.m_isMaskingEnabled)
					{
						this.isMaskUpdateRequired = true;
					}
					this.GenerateTextMesh();
					return;
				}
				return;
			}
		}

		private void DrawUnderlineMesh(Vector3 start, Vector3 end, ref int index, Color32 underlineColor)
		{
			int num = index + 12;
			if (num > this.m_vertices.Length)
			{
				this.ResizeMeshBuffers(num / 4 + 12);
			}
			start.y = Mathf.Min(start.y, end.y);
			end.y = Mathf.Min(start.y, end.y);
			float num2 = this.m_cached_Underline_GlyphInfo.width / 2f * this.m_fontScale;
			if (end.x - start.x < this.m_cached_Underline_GlyphInfo.width * this.m_fontScale)
			{
				num2 = (end.x - start.x) / 2f;
			}
			float height = this.m_cached_Underline_GlyphInfo.height;
			this.m_vertices[index] = start + new Vector3(0f, 0f - (height + this.m_padding) * this.m_fontScale, 0f);
			this.m_vertices[index + 1] = start + new Vector3(0f, this.m_padding * this.m_fontScale, 0f);
			this.m_vertices[index + 2] = this.m_vertices[index] + new Vector3(num2, 0f, 0f);
			this.m_vertices[index + 3] = start + new Vector3(num2, this.m_padding * this.m_fontScale, 0f);
			this.m_vertices[index + 4] = this.m_vertices[index + 2];
			this.m_vertices[index + 5] = this.m_vertices[index + 3];
			this.m_vertices[index + 6] = end + new Vector3(-num2, -(height + this.m_padding) * this.m_fontScale, 0f);
			this.m_vertices[index + 7] = end + new Vector3(-num2, this.m_padding * this.m_fontScale, 0f);
			this.m_vertices[index + 8] = this.m_vertices[index + 6];
			this.m_vertices[index + 9] = this.m_vertices[index + 7];
			this.m_vertices[index + 10] = end + new Vector3(0f, -(height + this.m_padding) * this.m_fontScale, 0f);
			this.m_vertices[index + 11] = end + new Vector3(0f, this.m_padding * this.m_fontScale, 0f);
			Vector2 vector = new Vector2((this.m_cached_Underline_GlyphInfo.x - this.m_padding) / this.m_fontAsset.fontInfo.AtlasWidth, 1f - (this.m_cached_Underline_GlyphInfo.y + this.m_padding + this.m_cached_Underline_GlyphInfo.height) / this.m_fontAsset.fontInfo.AtlasHeight);
			Vector2 vector2 = new Vector2(vector.x, 1f - (this.m_cached_Underline_GlyphInfo.y - this.m_padding) / this.m_fontAsset.fontInfo.AtlasHeight);
			Vector2 vector3 = new Vector2((this.m_cached_Underline_GlyphInfo.x + this.m_padding + this.m_cached_Underline_GlyphInfo.width / 2f) / this.m_fontAsset.fontInfo.AtlasWidth, vector.y);
			Vector2 vector4 = new Vector2(vector3.x, vector2.y);
			Vector2 vector5 = new Vector2((this.m_cached_Underline_GlyphInfo.x + this.m_padding + this.m_cached_Underline_GlyphInfo.width) / this.m_fontAsset.fontInfo.AtlasWidth, vector.y);
			Vector2 vector6 = new Vector2(vector5.x, vector2.y);
			this.m_uvs[0 + index] = vector;
			this.m_uvs[1 + index] = vector2;
			this.m_uvs[2 + index] = vector3;
			this.m_uvs[3 + index] = vector4;
			this.m_uvs[4 + index] = new Vector2(vector3.x - vector3.x * 0.001f, vector.y);
			this.m_uvs[5 + index] = new Vector2(vector3.x - vector3.x * 0.001f, vector2.y);
			this.m_uvs[6 + index] = new Vector2(vector3.x + vector3.x * 0.001f, vector.y);
			this.m_uvs[7 + index] = new Vector2(vector3.x + vector3.x * 0.001f, vector2.y);
			this.m_uvs[8 + index] = vector3;
			this.m_uvs[9 + index] = vector4;
			this.m_uvs[10 + index] = vector5;
			this.m_uvs[11 + index] = vector6;
			float x = (this.m_vertices[index + 2].x - start.x) / (end.x - start.x);
			float num3 = this.m_fontScale * this.m_transform.lossyScale.z;
			float scale = num3;
			this.m_uv2s[0 + index] = this.PackUV(0f, 0f, num3);
			this.m_uv2s[1 + index] = this.PackUV(0f, 1f, num3);
			this.m_uv2s[2 + index] = this.PackUV(x, 0f, num3);
			this.m_uv2s[3 + index] = this.PackUV(x, 1f, num3);
			float x2 = (this.m_vertices[index + 4].x - start.x) / (end.x - start.x);
			x = (this.m_vertices[index + 6].x - start.x) / (end.x - start.x);
			this.m_uv2s[4 + index] = this.PackUV(x2, 0f, scale);
			this.m_uv2s[5 + index] = this.PackUV(x2, 1f, scale);
			this.m_uv2s[6 + index] = this.PackUV(x, 0f, scale);
			this.m_uv2s[7 + index] = this.PackUV(x, 1f, scale);
			x2 = (this.m_vertices[index + 8].x - start.x) / (end.x - start.x);
			x = (this.m_vertices[index + 6].x - start.x) / (end.x - start.x);
			this.m_uv2s[8 + index] = this.PackUV(x2, 0f, num3);
			this.m_uv2s[9 + index] = this.PackUV(x2, 1f, num3);
			this.m_uv2s[10 + index] = this.PackUV(1f, 0f, num3);
			this.m_uv2s[11 + index] = this.PackUV(1f, 1f, num3);
			this.m_vertColors[0 + index] = underlineColor;
			this.m_vertColors[1 + index] = underlineColor;
			this.m_vertColors[2 + index] = underlineColor;
			this.m_vertColors[3 + index] = underlineColor;
			this.m_vertColors[4 + index] = underlineColor;
			this.m_vertColors[5 + index] = underlineColor;
			this.m_vertColors[6 + index] = underlineColor;
			this.m_vertColors[7 + index] = underlineColor;
			this.m_vertColors[8 + index] = underlineColor;
			this.m_vertColors[9 + index] = underlineColor;
			this.m_vertColors[10 + index] = underlineColor;
			this.m_vertColors[11 + index] = underlineColor;
			index += 12;
		}

		private void UpdateSDFScale(float prevScale, float newScale)
		{
			for (int i = 0; i < this.m_uv2s.Length; i++)
			{
				this.m_uv2s[i].y = this.m_uv2s[i].y / prevScale * newScale;
			}
			this.m_mesh.uv2 = this.m_uv2s;
		}

		private void ResizeMeshBuffers(int size)
		{
			int newSize = size * 4;
			int newSize2 = size * 6;
			int num = this.m_vertices.Length / 4;
			Array.Resize<Vector3>(ref this.m_vertices, newSize);
			Array.Resize<Vector3>(ref this.m_normals, newSize);
			Array.Resize<Vector4>(ref this.m_tangents, newSize);
			Array.Resize<Color32>(ref this.m_vertColors, newSize);
			Array.Resize<Vector2>(ref this.m_uvs, newSize);
			Array.Resize<Vector2>(ref this.m_uv2s, newSize);
			Array.Resize<int>(ref this.m_triangles, newSize2);
			Array.Resize<Color>(ref this.m_tempColors, newSize);
			Array.Resize<Vector3>(ref this.m_tempVertices, newSize);
			for (int i = num; i < size; i++)
			{
				int num2 = i * 4;
				int num3 = i * 6;
				this.m_normals[0 + num2] = new Vector3(0f, 0f, -1f);
				this.m_normals[1 + num2] = new Vector3(0f, 0f, -1f);
				this.m_normals[2 + num2] = new Vector3(0f, 0f, -1f);
				this.m_normals[3 + num2] = new Vector3(0f, 0f, -1f);
				this.m_tangents[0 + num2] = new Vector4(-1f, 0f, 0f, 1f);
				this.m_tangents[1 + num2] = new Vector4(-1f, 0f, 0f, 1f);
				this.m_tangents[2 + num2] = new Vector4(-1f, 0f, 0f, 1f);
				this.m_tangents[3 + num2] = new Vector4(-1f, 0f, 0f, 1f);
				this.m_triangles[0 + num3] = 0 + num2;
				this.m_triangles[1 + num3] = 1 + num2;
				this.m_triangles[2 + num3] = 2 + num2;
				this.m_triangles[3 + num3] = 3 + num2;
				this.m_triangles[4 + num3] = 2 + num2;
				this.m_triangles[5 + num3] = 1 + num2;
			}
			this.m_mesh.vertices = this.m_vertices;
			this.m_mesh.normals = this.m_normals;
			this.m_mesh.tangents = this.m_tangents;
			this.m_mesh.triangles = this.m_triangles;
		}

		private void UpdateMeshData(TMP_CharacterInfo[] characterInfo, int characterCount, Mesh mesh, Vector3[] vertices, Vector2[] uv0s, Vector2[] uv2s, Color32[] vertexColors, Vector3[] normals, Vector4[] tangents)
		{
		}

		private void AdjustLineOffset(int startIndex, int endIndex, float offset)
		{
			Vector3 b = new Vector3(0f, offset, 0f);
			for (int i = startIndex; i <= endIndex; i++)
			{
				if (this.m_textInfo.characterInfo[i].isVisible)
				{
					int vertexIndex = (int)this.m_textInfo.characterInfo[i].vertexIndex;
					TMP_CharacterInfo[] expr_5C_cp_0 = this.m_textInfo.characterInfo;
					int expr_5C_cp_1 = i;
					expr_5C_cp_0[expr_5C_cp_1].bottomLeft = expr_5C_cp_0[expr_5C_cp_1].bottomLeft - b;
					TMP_CharacterInfo[] expr_7E_cp_0 = this.m_textInfo.characterInfo;
					int expr_7E_cp_1 = i;
					expr_7E_cp_0[expr_7E_cp_1].topRight = expr_7E_cp_0[expr_7E_cp_1].topRight - b;
					TMP_CharacterInfo[] expr_A0_cp_0 = this.m_textInfo.characterInfo;
					int expr_A0_cp_1 = i;
					expr_A0_cp_0[expr_A0_cp_1].bottomLine = expr_A0_cp_0[expr_A0_cp_1].bottomLine - b.y;
					TMP_CharacterInfo[] expr_C4_cp_0 = this.m_textInfo.characterInfo;
					int expr_C4_cp_1 = i;
					expr_C4_cp_0[expr_C4_cp_1].topLine = expr_C4_cp_0[expr_C4_cp_1].topLine - b.y;
					this.m_vertices[0 + vertexIndex] -= b;
					this.m_vertices[1 + vertexIndex] -= b;
					this.m_vertices[2 + vertexIndex] -= b;
					this.m_vertices[3 + vertexIndex] -= b;
				}
			}
		}

		private void SaveWordWrappingState(ref WordWrapState state, int index, int count)
		{
			state.previous_WordBreak = index;
			state.total_CharacterCount = count;
			state.visible_CharacterCount = this.m_visibleCharacterCount;
			state.firstVisibleCharacterIndex = this.m_firstVisibleCharacterOfLine;
			state.lastVisibleCharIndex = this.m_lastVisibleCharacterOfLine;
			state.xAdvance = this.m_xAdvance;
			state.maxAscender = this.m_maxAscender;
			state.maxDescender = this.m_maxDescender;
			state.preferredWidth = this.m_preferredWidth;
			state.preferredHeight = this.m_preferredHeight;
			state.fontScale = this.m_fontScale;
			state.maxFontScale = this.m_maxFontScale;
			state.currentFontSize = this.m_currentFontSize;
			state.lineNumber = this.m_lineNumber;
			state.lineOffset = this.m_lineOffset;
			state.baselineOffset = this.m_baselineOffset;
			state.fontStyle = this.m_style;
			state.vertexColor = this.m_htmlColor;
			state.colorStackIndex = this.m_colorStackIndex;
			state.meshExtents = this.m_meshExtents;
			state.lineInfo = this.m_textInfo.lineInfo[this.m_lineNumber];
			state.textInfo = this.m_textInfo;
		}

		private int RestoreWordWrappingState(ref WordWrapState state)
		{
			this.m_textInfo.lineInfo[this.m_lineNumber] = state.lineInfo;
			this.m_textInfo = ((state.textInfo == null) ? this.m_textInfo : state.textInfo);
			this.m_currentFontSize = state.currentFontSize;
			this.m_fontScale = state.fontScale;
			this.m_baselineOffset = state.baselineOffset;
			this.m_style = state.fontStyle;
			this.m_htmlColor = state.vertexColor;
			this.m_colorStackIndex = state.colorStackIndex;
			this.m_characterCount = state.total_CharacterCount + 1;
			this.m_visibleCharacterCount = state.visible_CharacterCount;
			this.m_firstVisibleCharacterOfLine = state.firstVisibleCharacterIndex;
			this.m_lastVisibleCharacterOfLine = state.lastVisibleCharIndex;
			this.m_meshExtents = state.meshExtents;
			this.m_xAdvance = state.xAdvance;
			this.m_maxAscender = state.maxAscender;
			this.m_maxDescender = state.maxDescender;
			this.m_preferredWidth = state.preferredWidth;
			this.m_preferredHeight = state.preferredHeight;
			this.m_lineNumber = state.lineNumber;
			this.m_lineOffset = state.lineOffset;
			this.m_maxFontScale = state.maxFontScale;
			return state.previous_WordBreak;
		}

		private Vector2 PackUV(float x, float y, float scale)
		{
			x = x % 5f / 5f;
			y = y % 5f / 5f;
			return new Vector2(Mathf.Round(x * 4096f) + y, scale);
		}

		private void ResizeLineExtents(int size)
		{
			size = ((size <= 1024) ? Mathf.NextPowerOfTwo(size + 1) : (size + 256));
			TMP_LineInfo[] array = new TMP_LineInfo[size];
			for (int i = 0; i < size; i++)
			{
				if (i < this.m_textInfo.lineInfo.Length)
				{
					array[i] = this.m_textInfo.lineInfo[i];
				}
				else
				{
					array[i].lineExtents = new Extents(this.k_InfinityVector, -this.k_InfinityVector);
					array[i].ascender = -this.k_InfinityVector.x;
					array[i].descender = this.k_InfinityVector.x;
				}
			}
			this.m_textInfo.lineInfo = array;
		}

        private int HexToInt(char hex)
        {
            switch (hex)
            {
                case '0':
                    return 0;

                case '1':
                    return 1;

                case '2':
                    return 2;

                case '3':
                    return 3;

                case '4':
                    return 4;

                case '5':
                    return 5;

                case '6':
                    return 6;

                case '7':
                    return 7;

                case '8':
                    return 8;

                case '9':
                    return 9;

                case 'A':
                    return 10;

                case 'B':
                    return 11;

                case 'C':
                    return 12;

                case 'D':
                    return 13;

                case 'E':
                    return 14;

                case 'F':
                    return 15;

                case 'a':
                    return 10;

                case 'b':
                    return 11;

                case 'c':
                    return 12;

                case 'd':
                    return 13;

                case 'e':
                    return 14;

                case 'f':
                    return 15;
            }
            return 15;
        }


		private Color32 HexCharsToColor(char[] hexChars, int tagCount)
		{
			if (tagCount == 7)
			{
				byte r = (byte)(this.HexToInt(hexChars[1]) * 16 + this.HexToInt(hexChars[2]));
				byte g = (byte)(this.HexToInt(hexChars[3]) * 16 + this.HexToInt(hexChars[4]));
				byte b = (byte)(this.HexToInt(hexChars[5]) * 16 + this.HexToInt(hexChars[6]));
				return new Color32(r, g, b, 255);
			}
			if (tagCount == 9)
			{
				byte r2 = (byte)(this.HexToInt(hexChars[1]) * 16 + this.HexToInt(hexChars[2]));
				byte g2 = (byte)(this.HexToInt(hexChars[3]) * 16 + this.HexToInt(hexChars[4]));
				byte b2 = (byte)(this.HexToInt(hexChars[5]) * 16 + this.HexToInt(hexChars[6]));
				byte a = (byte)(this.HexToInt(hexChars[7]) * 16 + this.HexToInt(hexChars[8]));
				return new Color32(r2, g2, b2, a);
			}
			if (tagCount == 13)
			{
				byte r3 = (byte)(this.HexToInt(hexChars[7]) * 16 + this.HexToInt(hexChars[8]));
				byte g3 = (byte)(this.HexToInt(hexChars[9]) * 16 + this.HexToInt(hexChars[10]));
				byte b3 = (byte)(this.HexToInt(hexChars[11]) * 16 + this.HexToInt(hexChars[12]));
				return new Color32(r3, g3, b3, 255);
			}
			if (tagCount == 15)
			{
				byte r4 = (byte)(this.HexToInt(hexChars[7]) * 16 + this.HexToInt(hexChars[8]));
				byte g4 = (byte)(this.HexToInt(hexChars[9]) * 16 + this.HexToInt(hexChars[10]));
				byte b4 = (byte)(this.HexToInt(hexChars[11]) * 16 + this.HexToInt(hexChars[12]));
				byte a2 = (byte)(this.HexToInt(hexChars[13]) * 16 + this.HexToInt(hexChars[14]));
				return new Color32(r4, g4, b4, a2);
			}
			return new Color32(255, 255, 255, 255);
		}

		private float ConvertToFloat(char[] chars, int startIndex, int endIndex, int decimalPointIndex)
		{
			float num = 0f;
			float num2 = 1f;
			decimalPointIndex = ((decimalPointIndex <= 0) ? (endIndex + 1) : decimalPointIndex);
			if (chars[startIndex] == '-')
			{
				startIndex++;
				num2 = -1f;
			}
			if (chars[startIndex] == '+' || chars[startIndex] == '%')
			{
				startIndex++;
			}
			for (int i = startIndex; i < endIndex + 1; i++)
			{
				int num3 = decimalPointIndex - i;
				switch (num3 + 3)
				{
				case 0:
					num += (float)(chars[i] - '0') * 0.001f;
					break;
				case 1:
					num += (float)(chars[i] - '0') * 0.01f;
					break;
				case 2:
					num += (float)(chars[i] - '0') * 0.1f;
					break;
				case 4:
					num += (float)(chars[i] - '0');
					break;
				case 5:
					num += (float)((chars[i] - '0') * '\n');
					break;
				case 6:
					num += (float)((chars[i] - '0') * 'd');
					break;
				case 7:
					num += (float)((chars[i] - '0') * 'Ϩ');
					break;
				}
			}
			return num * num2;
		}

        private bool ValidateHtmlTag(int[] chars, int startIndex, out int endIndex)
        {
            Array.Clear(this.m_htmlTag, 0, this.m_htmlTag.Length);
            int index = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int decimalPointIndex = 0;
            endIndex = startIndex;
            bool flag = false;
            int num7 = 1;
            for (int i = startIndex; ((chars[i] != 0) && (index < this.m_htmlTag.Length)) && (chars[i] != 60); i++)
            {
                if (chars[i] == 0x3e)
                {
                    flag = true;
                    endIndex = i;
                    this.m_htmlTag[index] = '\0';
                    break;
                }
                this.m_htmlTag[index] = (char)chars[i];
                index++;
                if (chars[i] == 0x3d)
                {
                    num7 = 0;
                }
                num2 += (chars[i] * index) * num7;
                num3 += (chars[i] * index) * (1 - num7);
                switch (chars[i])
                {
                    case 0x2e:
                        decimalPointIndex = index - 1;
                        break;

                    case 0x3d:
                        num4 = index;
                        break;
                }
            }
            if (flag)
            {
                float num9;
                if ((this.m_htmlTag[0] == '#') && (index == 7))
                {
                    this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, index);
                    this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                    this.m_colorStackIndex++;
                    return true;
                }
                if ((this.m_htmlTag[0] == '#') && (index == 9))
                {
                    this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, index);
                    this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                    this.m_colorStackIndex++;
                    return true;
                }
                switch (num2)
                {
                    case 0x73:
                        this.m_style |= FontStyles.Strikethrough;
                        return true;

                    case 0x75:
                        this.m_style |= FontStyles.Underline;
                        return true;

                    case 0xf1:
                        return true;

                    case 0xf3:
                        if ((this.m_fontStyle & FontStyles.Bold) != FontStyles.Bold)
                        {
                            this.m_style &= ~FontStyles.Bold;
                        }
                        return true;

                    case 0x3fb:
                        if (this.m_overflowMode == TextOverflowModes.Page)
                        {
                            this.m_xAdvance = 0f + this.m_indent;
                            this.m_lineOffset = 0f;
                            this.m_pageNumber++;
                            this.m_isNewPage = true;
                        }
                        return true;

                    case 0x3fc:
                        this.m_currentFontSize /= (this.m_fontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_fontAsset.fontInfo.SubSize;
                        this.m_baselineOffset = 0f;
                        this.m_fontScale = (this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize) * (!this.m_isOrthographic ? 0.1f : 1f);
                        return true;

                    case 0x62:
                        this.m_style |= FontStyles.Bold;
                        return true;

                    case 0x69:
                        this.m_style |= FontStyles.Italic;
                        return true;

                    case 0x101:
                        this.m_style &= ~FontStyles.Italic;
                        return true;

                    case 0x115:
                        if ((this.m_fontStyle & FontStyles.Strikethrough) != FontStyles.Strikethrough)
                        {
                            this.m_style &= ~FontStyles.Strikethrough;
                        }
                        return true;

                    case 0x119:
                        if ((this.m_fontStyle & FontStyles.Underline) != FontStyles.Underline)
                        {
                            this.m_style &= ~FontStyles.Underline;
                        }
                        return true;

                    case 0x283:
                        this.m_currentFontSize *= (this.m_fontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_fontAsset.fontInfo.SubSize;
                        this.m_fontScale = (this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize) * (!this.m_isOrthographic ? 0.1f : 1f);
                        this.m_baselineOffset = this.m_fontAsset.fontInfo.SubscriptOffset * this.m_fontScale;
                        return true;

                    case 0x2a7:
                        num9 = this.ConvertToFloat(this.m_htmlTag, num4, index - 1, decimalPointIndex);
                        this.m_xAdvance = (num9 * this.m_fontScale) * this.m_fontAsset.fontInfo.TabWidth;
                        return true;

                    case 0x2ad:
                        this.m_currentFontSize *= (this.m_fontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_fontAsset.fontInfo.SubSize;
                        this.m_fontScale = (this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize) * (!this.m_isOrthographic ? 0.1f : 1f);
                        this.m_baselineOffset = this.m_fontAsset.fontInfo.SuperscriptOffset * this.m_fontScale;
                        return true;

                    case 0x434:
                        this.m_currentFontSize /= (this.m_fontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_fontAsset.fontInfo.SubSize;
                        this.m_baselineOffset = 0f;
                        this.m_fontScale = (this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize) * (!this.m_isOrthographic ? 0.1f : 1f);
                        return true;

                    case 0x447:
                        {
                            num5 = index - 1;
                            float num10 = 0f;
                            if (this.m_htmlTag[5] == '%')
                            {
                                num10 = this.ConvertToFloat(this.m_htmlTag, num4, num5, decimalPointIndex);
                                this.m_currentFontSize = (this.m_fontSize * num10) / 100f;
                                this.m_isRecalculateScaleRequired = true;
                                return true;
                            }
                            if (this.m_htmlTag[5] == '+')
                            {
                                num10 = this.ConvertToFloat(this.m_htmlTag, num4, num5, decimalPointIndex);
                                this.m_currentFontSize = this.m_fontSize + num10;
                                this.m_isRecalculateScaleRequired = true;
                                return true;
                            }
                            if (this.m_htmlTag[5] == '-')
                            {
                                num10 = this.ConvertToFloat(this.m_htmlTag, num4, num5, decimalPointIndex);
                                this.m_currentFontSize = this.m_fontSize + num10;
                                this.m_isRecalculateScaleRequired = true;
                                return true;
                            }
                            num10 = this.ConvertToFloat(this.m_htmlTag, num4, num5, decimalPointIndex);
                            if (num10 == 73493f)
                            {
                                return false;
                            }
                            this.m_currentFontSize = num10;
                            this.m_isRecalculateScaleRequired = true;
                            return true;
                        }
                    case 0x45e:
                        Debug.Log("Font Tag used.");
                        return true;

                    case 0x5fb:
                        num9 = this.ConvertToFloat(this.m_htmlTag, num4, index - 1, decimalPointIndex);
                        this.m_xAdvance += (num9 * this.m_fontScale) * this.m_fontAsset.fontInfo.TabWidth;
                        return true;

                    case 0x60e:
                        this.m_htmlColor.a = (byte)((this.HexToInt(this.m_htmlTag[7]) * 0x10) + this.HexToInt(this.m_htmlTag[8]));
                        return true;

                    case 0x631:
                        this.m_currentFontSize = this.m_fontSize;
                        this.m_isRecalculateScaleRequired = true;
                        return true;

                    case 0x636:
                        switch (num3)
                        {
                            case 0xfa8:
                                this.m_lineJustification = TextAlignmentOptions.Left;
                                return true;

                            case 0x147f:
                                this.m_lineJustification = TextAlignmentOptions.Right;
                                return true;

                            case 0x1960:
                                this.m_lineJustification = TextAlignmentOptions.Center;
                                return true;

                            case 0x2a91:
                                this.m_lineJustification = TextAlignmentOptions.Justified;
                                return true;
                        }
                        return false;

                    case 0x67b:
                        if ((this.m_htmlTag[6] == '#') && (index == 13))
                        {
                            this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, index);
                            this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                            this.m_colorStackIndex++;
                            return true;
                        }
                        if ((this.m_htmlTag[6] == '#') && (index == 15))
                        {
                            this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, index);
                            this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                            this.m_colorStackIndex++;
                            return true;
                        }
                        switch (num3)
                        {
                            case 0xb38:
                                this.m_htmlColor = Color.red;
                                this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                                this.m_colorStackIndex++;
                                return true;

                            case 0xf8b:
                                this.m_htmlColor = Color.blue;
                                this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                                this.m_colorStackIndex++;
                                return true;

                            case 0x135c:
                                this.m_htmlColor = Color.black;
                                this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                                this.m_colorStackIndex++;
                                return true;

                            case 0x1408:
                                this.m_htmlColor = Color.green;
                                this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                                this.m_colorStackIndex++;
                                return true;

                            case 0x147f:
                                this.m_htmlColor = Color.white;
                                this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                                this.m_colorStackIndex++;
                                return true;

                            case 0x18e5:
                                this.m_htmlColor = new Color32(0xff, 0x80, 0, 0xff);
                                this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                                this.m_colorStackIndex++;
                                return true;

                            case 0x19e8:
                                this.m_htmlColor = new Color32(160, 0x20, 240, 0xff);
                                this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                                this.m_colorStackIndex++;
                                return true;

                            case 0x1a42:
                                this.m_htmlColor = Color.yellow;
                                this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                                this.m_colorStackIndex++;
                                return true;
                        }
                        return false;

                    case 0x7ee:
                        return true;

                    case 0x86a:
                        this.m_cSpacing = this.ConvertToFloat(this.m_htmlTag, num4, index - 1, decimalPointIndex);
                        return true;

                    case 0x870:
                        this.m_lineJustification = this.m_textAlignment;
                        return true;

                    case 0x874:
                        this.m_monoSpacing = this.ConvertToFloat(this.m_htmlTag, num4, index - 1, decimalPointIndex);
                        return true;

                    case 0x8c9:
                        this.m_colorStackIndex--;
                        if (this.m_colorStackIndex <= 0)
                        {
                            this.m_htmlColor = this.m_fontColor32;
                            this.m_colorStackIndex = 0;
                        }
                        else
                        {
                            this.m_htmlColor = this.m_colorStack[this.m_colorStackIndex - 1];
                        }
                        return true;

                    case 0x8e3:
                        this.m_indent = (this.ConvertToFloat(this.m_htmlTag, num4, index - 1, decimalPointIndex) * this.m_fontScale) * this.m_fontAsset.fontInfo.TabWidth;
                        this.m_xAdvance = this.m_indent;
                        return true;

                    case 0x8ef:
                        Debug.Log("Sprite Tag used.");
                        return true;

                    case 0xb08:
                        this.m_cSpacing = 0f;
                        return true;

                    case 0xb1c:
                        this.m_monoSpacing = 0f;
                        return true;

                    case 0xb94:
                        this.m_indent = 0f;
                        return true;

                    case 0xbb3:
                        this.m_style |= FontStyles.UpperCase;
                        return true;

                    case 0xec2:
                        this.m_style &= ~FontStyles.UpperCase;
                        return true;

                    case 0x12c0:
                        this.m_style |= FontStyles.SmallCaps;
                        return true;

                    case 0x16af:
                        this.m_currentFontSize = this.m_fontSize;
                        this.m_style &= ~FontStyles.SmallCaps;
                        this.m_isRecalculateScaleRequired = true;
                        return true;

                    case 0x1a23:
                        this.m_lineHeight = this.ConvertToFloat(this.m_htmlTag, num4, index - 1, decimalPointIndex);
                        return true;

                    case 0x1ea0:
                        this.m_lineHeight = 0f;
                        return true;
                }
            }
            return false;
        }


		public void UpdateMeshPadding()
		{
			this.m_padding = ShaderUtilities.GetPadding(this.m_renderer.sharedMaterials, this.m_enableExtraPadding, this.m_isUsingBold);
			this.havePropertiesChanged = true;
		}

		public void ForceMeshUpdate()
		{
			this.havePropertiesChanged = true;
			this.OnWillRenderObject();
		}

		public void UpdateFontAsset()
		{
			this.LoadFontAsset();
		}

		public TMP_TextInfo GetTextInfo(string text)
		{
			this.StringToCharArray(text, ref this.m_char_buffer);
			this.m_renderMode = TextRenderFlags.DontRender;
			this.GenerateTextMesh();
			this.m_renderMode = TextRenderFlags.Render;
			return this.textInfo;
		}

		public void SetText(string text, float arg0)
		{
			this.SetText(text, arg0, 255f, 255f);
		}

		public void SetText(string text, float arg0, float arg1)
		{
			this.SetText(text, arg0, arg1, 255f);
		}

		public void SetText(string text, float arg0, float arg1, float arg2)
		{
			if (text == this.old_text && arg0 == this.old_arg0 && arg1 == this.old_arg1 && arg2 == this.old_arg2)
			{
				return;
			}
			if (this.m_input_CharArray.Length < text.Length)
			{
				this.m_input_CharArray = new char[Mathf.NextPowerOfTwo(text.Length + 1)];
			}
			this.old_text = text;
			this.old_arg1 = 255f;
			this.old_arg2 = 255f;
			int precision = 0;
			int num = 0;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (c == '{')
				{
					if (text[i + 2] == ':')
					{
						precision = (int)(text[i + 3] - '0');
					}
					switch (text[i + 1])
					{
					case '0':
						this.old_arg0 = arg0;
						this.AddFloatToCharArray(arg0, ref num, precision);
						break;
					case '1':
						this.old_arg1 = arg1;
						this.AddFloatToCharArray(arg1, ref num, precision);
						break;
					case '2':
						this.old_arg2 = arg2;
						this.AddFloatToCharArray(arg2, ref num, precision);
						break;
					}
					if (text[i + 2] == ':')
					{
						i += 4;
					}
					else
					{
						i += 2;
					}
				}
				else
				{
					this.m_input_CharArray[num] = c;
					num++;
				}
			}
			this.m_input_CharArray[num] = '\0';
			this.m_charArray_Length = num;
			this.m_inputSource = TextMeshPro.TextInputSources.SetText;
			this.isInputParsingRequired = true;
			this.havePropertiesChanged = true;
		}

        public void SetCharArray(char[] charArray)
        {
            if ((charArray != null) && (charArray.Length != 0))
            {
                if (this.m_char_buffer.Length <= charArray.Length)
                {
                    int num = Mathf.NextPowerOfTwo(charArray.Length + 1);
                    this.m_char_buffer = new int[num];
                }
                int index = 0;
                for (int i = 0; i < charArray.Length; i++)
                {
                    if ((charArray[i] == '\\') && (i < (charArray.Length - 1)))
                    {
                        int num4 = charArray[i + 1];
                        switch (num4)
                        {
                            case 0x72:
                                {
                                    this.m_char_buffer[index] = 13;
                                    i++;
                                    index++;
                                    continue;
                                }
                            case 0x74:
                                {
                                    this.m_char_buffer[index] = 9;
                                    i++;
                                    index++;
                                    continue;
                                }
                        }
                        if (num4 == 110)
                        {
                            this.m_char_buffer[index] = 10;
                            i++;
                            index++;
                            continue;
                        }
                    }
                    this.m_char_buffer[index] = charArray[i];
                    index++;
                }
                this.m_char_buffer[index] = 0;
                this.m_inputSource = TextInputSources.SetCharArray;
                this.havePropertiesChanged = true;
                this.isInputParsingRequired = true;
            }
        }

	}
}
