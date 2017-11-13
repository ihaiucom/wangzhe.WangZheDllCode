using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TMPro
{
	[AddComponentMenu("UI/TextMeshPro Text", 12), DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(RectTransform)), RequireComponent(typeof(CanvasRenderer))]
	public class TextMeshProUGUI : Graphic, ILayoutElement, IMaskable
	{
		private enum TextInputSources
		{
			Text,
			SetText,
			SetCharArray
		}

		private enum AutoLayoutPhase
		{
			Horizontal,
			Vertical
		}

		[SerializeField]
		private string m_text;

		[SerializeField]
		private TextMeshProFont m_fontAsset;

		[SerializeField]
		private Material m_fontMaterial;

		[SerializeField]
		private Material m_sharedMaterial;

		[SerializeField]
		private FontStyles m_fontStyle;

		private FontStyles m_style;

		private bool m_isOverlay;

		[SerializeField]
		private Color32 m_fontColor32 = Color.white;

		[SerializeField]
		private Color m_fontColor = Color.white;

		[SerializeField]
		private bool m_enableVertexGradient;

		[SerializeField]
		private VertexGradient m_fontColorGradient = new VertexGradient(Color.white);

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
		private float m_lineSpacing;

		private float m_lineSpacingDelta;

		private float m_lineHeight;

		[SerializeField]
		private float m_paragraphSpacing;

		[FormerlySerializedAs("m_lineJustification"), SerializeField]
		private TextAlignmentOptions m_textAlignment;

		private TextAlignmentOptions m_lineJustification;

		[SerializeField]
		private bool m_enableKerning;

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
		private bool hasFontAssetChanged;

		[SerializeField]
		private bool m_isRichText = true;

		[SerializeField]
		private TextMeshProUGUI.TextInputSources m_inputSource;

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

		private Vector3 m_anchorOffset;

		private TMP_TextInfo m_textInfo;

		private List<TMP_CharacterInfo> m_characterInfoList = new List<TMP_CharacterInfo>(256);

		private char[] m_htmlTag = new char[16];

		private CanvasRenderer m_uiRenderer;

		private Canvas m_canvas;

		private RectTransform m_rectTransform;

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

		private WordWrapState m_SavedWordWrapState;

		private WordWrapState m_SavedLineState;

		private int m_characterCount;

		private int m_visibleCharacterCount;

		private int m_visibleSpriteCount;

		private int m_firstVisibleCharacterOfLine;

		private int m_lastVisibleCharacterOfLine;

		private int m_lineNumber;

		private int m_pageNumber;

		private float m_maxAscender;

		private float m_maxDescender;

		private float m_maxFontScale;

		private float m_lineOffset;

		private Extents m_meshExtents;

		private bool m_isCalculateSizeRequired;

		private ILayoutController m_layoutController;

		[SerializeField]
		private UIVertex[] m_uiVertices;

		private Bounds m_bounds;

		[SerializeField]
		private bool m_ignoreCulling = true;

		[SerializeField]
		private bool m_isOrthographic;

		[SerializeField]
		private bool m_isCullingEnabled;

		[SerializeField]
		private int m_sortingLayerID;

		[SerializeField]
		private int m_sortingOrder;

		private int m_maxVisibleCharacters = -1;

		private int m_maxVisibleLines = -1;

		[SerializeField]
		private int m_pageToDisplay;

		private bool m_isNewPage;

		private bool m_isTextTruncated;

		[SerializeField]
		private TextMeshProFont[] m_fontAssetArray;

		private int[] m_meshAllocCount = new int[17];

		private GameObject[] subObjects = new GameObject[17];

		private List<Material> m_sharedMaterials = new List<Material>(16);

		private int m_selectedFontAsset;

		private bool m_isMaskingEnabled;

		private bool m_isStencilUpdateRequired;

		[SerializeField]
		private Material m_baseMaterial;

		private Material m_lastBaseMaterial;

		[SerializeField]
		private bool m_isNewBaseMaterial;

		private Material m_maskingMaterial;

		private bool m_isScrollRegionSet;

		private int m_stencilID;

		[SerializeField]
		private Vector4 m_maskOffset;

		private Matrix4x4 m_EnvMapMatrix = default(Matrix4x4);

		private TextRenderFlags m_renderMode;

		private float m_maxXAdvance;

		private int m_spriteCount;

		private bool m_isSprite;

		private int m_spriteIndex;

		private InlineGraphicManager m_inlineGraphics;

		[SerializeField]
		private Vector4 m_margin = new Vector4(0f, 0f, 0f, 0f);

		private float m_marginWidth;

		private float m_marginHeight;

		private bool m_marginsHaveChanged;

		private bool IsRectTransformDriven;

		private float m_width;

		[SerializeField]
		private bool m_rectTransformDimensionsChanged;

		private Vector3[] m_rectCorners = new Vector3[4];

		[SerializeField]
		private bool m_enableAutoSizing;

		private float m_maxFontSize;

		private float m_minFontSize;

		private bool m_isAwake;

		private bool m_isEnabled;

		private int m_recursiveCount;

		private int loopCountA;

		private float m_flexibleHeight;

		private float m_flexibleWidth;

		private float m_minHeight;

		private float m_minWidth;

		private float m_preferredWidth = 9999f;

		private float m_preferredHeight = 9999f;

		private float m_renderedWidth;

		private float m_renderedHeight;

		private int m_layoutPriority;

		private bool m_isRebuildingLayout;

		private bool m_isLayoutDirty;

		private TextMeshProUGUI.AutoLayoutPhase m_LayoutPhase;

		private TextOverflowModes m_currentOverflowMode;

		private bool m_currentAutoSizeMode;

		public string text
		{
			get
			{
				return this.m_text;
			}
			set
			{
				this.m_inputSource = TextMeshProUGUI.TextInputSources.Text;
				this.havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.isInputParsingRequired = true;
				this.MarkLayoutForRebuild();
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
					this.m_isCalculateSizeRequired = true;
					this.MarkLayoutForRebuild();
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
				return this.m_uiRenderer.GetMaterial();
			}
			set
			{
				if (this.m_uiRenderer.GetMaterial() != value)
				{
					this.m_isNewBaseMaterial = true;
					this.SetSharedFontMaterial(value);
					this.havePropertiesChanged = true;
				}
			}
		}

		protected Material fontBaseMaterial
		{
			get
			{
				return this.m_baseMaterial;
			}
			set
			{
				if (this.m_baseMaterial != value)
				{
					this.SetFontBaseMaterial(value);
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
				if (this.m_isOverlay != value)
				{
					this.m_isOverlay = value;
					this.SetShaderDepth();
					this.havePropertiesChanged = true;
				}
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
				if (this.m_fontColor != value)
				{
					this.havePropertiesChanged = true;
					this.m_fontColor = value;
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
				this.havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.MarkLayoutForRebuild();
				this.m_fontSize = value;
				if (!this.m_enableAutoSizing)
				{
					this.m_fontSizeBase = this.m_fontSize;
				}
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
					this.m_isCalculateSizeRequired = true;
					this.MarkLayoutForRebuild();
					this.m_characterSpacing = value;
				}
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
					this.m_isCalculateSizeRequired = true;
					this.MarkLayoutForRebuild();
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
					this.m_isCalculateSizeRequired = true;
					this.MarkLayoutForRebuild();
					this.m_paragraphSpacing = value;
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
				this.m_isCalculateSizeRequired = true;
				this.MarkLayoutForRebuild();
				this.isInputParsingRequired = true;
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
				this.m_isRecalculateScaleRequired = true;
			}
		}

		public Bounds bounds
		{
			get
			{
				if (this.m_uiVertices != null)
				{
					return this.m_bounds;
				}
				return default(Bounds);
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
					this.m_isCalculateSizeRequired = true;
					this.MarkLayoutForRebuild();
					this.m_enableKerning = value;
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
					this.m_isCalculateSizeRequired = true;
					this.MarkLayoutForRebuild();
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
					this.m_isRecalculateScaleRequired = true;
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
				return this.m_sortingLayerID;
			}
			set
			{
				this.m_sortingLayerID = value;
			}
		}

		public int sortingOrder
		{
			get
			{
				return this.m_sortingOrder;
			}
			set
			{
				this.m_sortingOrder = value;
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

		public Vector4 margin
		{
			get
			{
				return this.m_margin;
			}
			set
			{
				if (this.m_margin != value)
				{
					this.m_margin = value;
					this.havePropertiesChanged = true;
					this.m_marginsHaveChanged = true;
				}
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

		public RectTransform rectTransform
		{
			get
			{
				if (this.m_rectTransform == null)
				{
					this.m_rectTransform = base.GetComponent<RectTransform>();
				}
				return this.m_rectTransform;
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

		public float flexibleHeight
		{
			get
			{
				return this.m_flexibleHeight;
			}
		}

		public float flexibleWidth
		{
			get
			{
				return this.m_flexibleWidth;
			}
		}

		public float minHeight
		{
			get
			{
				return this.m_minHeight;
			}
		}

		public float minWidth
		{
			get
			{
				return this.m_minWidth;
			}
		}

		public float preferredWidth
		{
			get
			{
				return (this.m_preferredWidth == 9999f) ? this.m_renderedWidth : this.m_preferredWidth;
			}
		}

		public float preferredHeight
		{
			get
			{
				return (this.m_preferredHeight == 9999f) ? this.m_renderedHeight : this.m_preferredHeight;
			}
		}

		public int layoutPriority
		{
			get
			{
				return this.m_layoutPriority;
			}
		}

		public Vector4 maskOffset
		{
			get
			{
				return this.m_maskOffset;
			}
			set
			{
				this.m_maskOffset = value;
				this.UpdateMask();
				this.havePropertiesChanged = true;
			}
		}

		public TMP_TextInfo textInfo
		{
			get
			{
				return this.m_textInfo;
			}
		}

		public UIVertex[] mesh
		{
			get
			{
				return this.m_uiVertices;
			}
		}

		public CanvasRenderer canvasRenderer
		{
			get
			{
				return this.m_uiRenderer;
			}
		}

		protected override void Awake()
		{
			this.m_isAwake = true;
			this.m_canvas = (base.GetComponentInParent(typeof(Canvas)) as Canvas);
			this.m_rectTransform = base.gameObject.GetComponent<RectTransform>();
			if (this.m_rectTransform == null)
			{
				this.m_rectTransform = base.gameObject.AddComponent<RectTransform>();
			}
			this.m_uiRenderer = base.GetComponent<CanvasRenderer>();
			if (this.m_uiRenderer == null)
			{
				this.m_uiRenderer = base.gameObject.AddComponent<CanvasRenderer>();
			}
			ILayoutController layoutController;
			if ((layoutController = (base.GetComponent(typeof(ILayoutController)) as ILayoutController)) == null)
			{
				if (base.transform.parent != null)
				{
					ILayoutController layoutController2 = base.transform.parent.GetComponent(typeof(ILayoutController)) as ILayoutController;
					layoutController = layoutController2;
				}
				else
				{
					layoutController = null;
				}
			}
			this.m_layoutController = layoutController;
			if (this.m_layoutController != null)
			{
				this.IsRectTransformDriven = true;
			}
			this.LoadFontAsset();
			this.m_char_buffer = new int[this.m_max_characters];
			this.m_cached_GlyphInfo = new GlyphInfo();
			this.m_uiVertices = new UIVertex[0];
			this.m_isFirstAllocation = true;
			this.m_textInfo = new TMP_TextInfo();
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
			this.m_rectTransformDimensionsChanged = true;
			this.ForceMeshUpdate();
		}

		protected override void OnEnable()
		{
			this.m_isEnabled = true;
			Canvas.willRenderCanvases += new Canvas.WillRenderCanvases(this.OnPreRenderCanvas);
			if (this.m_canvas == null)
			{
				this.m_canvas = (base.GetComponentInParent(typeof(Canvas)) as Canvas);
			}
			if (this.m_uiRenderer.GetMaterial() == null)
			{
				if (this.m_sharedMaterial != null)
				{
					this.m_uiRenderer.SetMaterial(this.m_sharedMaterial, null);
				}
				else
				{
					this.m_isNewBaseMaterial = true;
					this.fontSharedMaterial = this.m_baseMaterial;
					this.ParentMaskStateChanged();
				}
				this.havePropertiesChanged = true;
				this.m_rectTransformDimensionsChanged = true;
			}
			LayoutRebuilder.MarkLayoutForRebuild(this.m_rectTransform);
		}

		protected override void OnDisable()
		{
			this.m_isEnabled = false;
			Canvas.willRenderCanvases -= new Canvas.WillRenderCanvases(this.OnPreRenderCanvas);
			this.m_uiRenderer.Clear();
			LayoutRebuilder.MarkLayoutForRebuild(this.m_rectTransform);
		}

		protected override void OnDestroy()
		{
			if (this.m_maskingMaterial != null)
			{
				MaterialManager.ReleaseStencilMaterial(this.m_maskingMaterial);
				this.m_maskingMaterial = null;
			}
			if (this.m_fontMaterial != null)
			{
				Object.DestroyImmediate(this.m_fontMaterial);
			}
		}

		protected void Reset()
		{
			this.isInputParsingRequired = true;
			this.havePropertiesChanged = true;
		}

		protected override void OnTransformParentChanged()
		{
			int stencilID = this.m_stencilID;
			this.m_stencilID = MaterialManager.GetStencilID(base.gameObject);
			if (stencilID != this.m_stencilID)
			{
				this.ParentMaskStateChanged();
			}
			LayoutRebuilder.MarkLayoutForRebuild(this.m_rectTransform);
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
				this.m_baseMaterial = this.m_fontAsset.material;
				this.m_sharedMaterial = this.m_baseMaterial;
				this.m_isNewBaseMaterial = true;
			}
			else
			{
				if (this.m_fontAsset.characterDictionary == null)
				{
					this.m_fontAsset.ReadFontDefinition();
				}
				this.m_sharedMaterial = this.m_baseMaterial;
				this.m_isNewBaseMaterial = true;
				if (this.m_sharedMaterial == null || this.m_sharedMaterial.mainTexture == null || this.m_fontAsset.atlas.GetInstanceID() != this.m_sharedMaterial.mainTexture.GetInstanceID())
				{
					this.m_sharedMaterial = this.m_fontAsset.material;
					this.m_baseMaterial = this.m_sharedMaterial;
					this.m_isNewBaseMaterial = true;
				}
			}
			if (!this.m_fontAsset.characterDictionary.TryGetValue(95, ref this.m_cached_Underline_GlyphInfo))
			{
				Debug.LogWarning("Underscore character wasn't found in the current Font Asset. No characters assigned for Underline.");
			}
			this.m_stencilID = MaterialManager.GetStencilID(base.gameObject);
			if (this.m_stencilID == 0)
			{
				if (this.m_maskingMaterial != null)
				{
					MaterialManager.ReleaseStencilMaterial(this.m_maskingMaterial);
					this.m_maskingMaterial = null;
				}
				this.m_sharedMaterial = this.m_baseMaterial;
			}
			else
			{
				if (this.m_maskingMaterial == null)
				{
					this.m_maskingMaterial = MaterialManager.GetStencilMaterial(this.m_baseMaterial, this.m_stencilID);
				}
				else if (this.m_maskingMaterial.GetInt(ShaderUtilities.ID_StencilID) != this.m_stencilID || this.m_isNewBaseMaterial)
				{
					MaterialManager.ReleaseStencilMaterial(this.m_maskingMaterial);
					this.m_maskingMaterial = MaterialManager.GetStencilMaterial(this.m_baseMaterial, this.m_stencilID);
				}
				this.m_sharedMaterial = this.m_maskingMaterial;
			}
			this.m_isNewBaseMaterial = false;
			this.m_sharedMaterials.Add(this.m_sharedMaterial);
			this.SetShaderDepth();
			if (this.m_uiRenderer == null)
			{
				this.m_uiRenderer = base.GetComponent<CanvasRenderer>();
			}
			this.m_uiRenderer.SetMaterial(this.m_sharedMaterial, null);
			this.m_padding = ShaderUtilities.GetPadding(this.m_sharedMaterial, this.m_enableExtraPadding, this.m_isUsingBold);
			this.m_alignmentPadding = ShaderUtilities.GetFontExtent(this.m_sharedMaterial);
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
			Debug.Log("Updating Env Matrix...");
			Vector3 euler = this.m_sharedMaterial.GetVector(ShaderUtilities.ID_EnvMatrixRotation);
			this.m_EnvMapMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(euler), Vector3.one);
			this.m_sharedMaterial.SetMatrix(ShaderUtilities.ID_EnvMatrix, this.m_EnvMapMatrix);
		}

		private void EnableMasking()
		{
			if (this.m_fontMaterial == null)
			{
				this.m_fontMaterial = this.CreateMaterialInstance(this.m_sharedMaterial);
				this.m_uiRenderer.SetMaterial(this.m_fontMaterial, null);
			}
			this.m_sharedMaterial = this.m_fontMaterial;
			if (this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_MaskCoord))
			{
				this.m_sharedMaterial.EnableKeyword("MASK_SOFT");
				this.m_sharedMaterial.DisableKeyword("MASK_HARD");
				this.m_sharedMaterial.DisableKeyword("MASK_OFF");
				this.UpdateMask();
			}
			this.m_isMaskingEnabled = true;
		}

		private void DisableMasking()
		{
			if (this.m_fontMaterial != null)
			{
				if (this.m_stencilID > 0)
				{
					this.m_sharedMaterial = this.m_maskingMaterial;
				}
				else
				{
					this.m_sharedMaterial = this.m_baseMaterial;
				}
				this.m_uiRenderer.SetMaterial(this.m_sharedMaterial, null);
				Object.DestroyImmediate(this.m_fontMaterial);
			}
			this.m_isMaskingEnabled = false;
		}

		private void UpdateMask()
		{
			if (this.m_rectTransform != null)
			{
				if (!ShaderUtilities.isInitialized)
				{
					ShaderUtilities.GetShaderPropertyIDs();
				}
				this.m_isScrollRegionSet = true;
				float num = Mathf.Min(Mathf.Min(this.m_margin.x, this.m_margin.z), this.m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessX));
				float num2 = Mathf.Min(Mathf.Min(this.m_margin.y, this.m_margin.w), this.m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessY));
				num = ((num > 0f) ? num : 0f);
				num2 = ((num2 > 0f) ? num2 : 0f);
				float z = (this.m_rectTransform.rect.width - Mathf.Max(this.m_margin.x, 0f) - Mathf.Max(this.m_margin.z, 0f)) / 2f + num;
				float w = (this.m_rectTransform.rect.height - Mathf.Max(this.m_margin.y, 0f) - Mathf.Max(this.m_margin.w, 0f)) / 2f + num2;
				Vector2 vector = this.m_rectTransform.localPosition + new Vector3((0.5f - this.m_rectTransform.pivot.x) * this.m_rectTransform.rect.width + (Mathf.Max(this.m_margin.x, 0f) - Mathf.Max(this.m_margin.z, 0f)) / 2f, (0.5f - this.m_rectTransform.pivot.y) * this.m_rectTransform.rect.height + (-Mathf.Max(this.m_margin.y, 0f) + Mathf.Max(this.m_margin.w, 0f)) / 2f);
				Vector4 vector2 = new Vector4(vector.x, vector.y, z, w);
				this.m_sharedMaterial.SetVector(ShaderUtilities.ID_MaskCoord, vector2);
			}
		}

		private void SetFontMaterial(Material mat)
		{
			ShaderUtilities.GetShaderPropertyIDs();
			if (this.m_uiRenderer == null)
			{
				this.m_uiRenderer = base.GetComponent<CanvasRenderer>();
			}
			if (this.m_fontMaterial != null)
			{
				Object.DestroyImmediate(this.m_fontMaterial);
			}
			if (this.m_maskingMaterial != null)
			{
				MaterialManager.ReleaseStencilMaterial(this.m_maskingMaterial);
				this.m_maskingMaterial = null;
			}
			this.m_stencilID = MaterialManager.GetStencilID(base.gameObject);
			this.m_fontMaterial = this.CreateMaterialInstance(mat);
			if (this.m_stencilID > 0)
			{
				this.m_fontMaterial = MaterialManager.SetStencil(this.m_fontMaterial, this.m_stencilID);
			}
			this.m_sharedMaterial = this.m_fontMaterial;
			this.SetShaderDepth();
			this.m_uiRenderer.SetMaterial(this.m_sharedMaterial, null);
			this.m_padding = ShaderUtilities.GetPadding(this.m_sharedMaterial, this.m_enableExtraPadding, this.m_isUsingBold);
			this.m_alignmentPadding = ShaderUtilities.GetFontExtent(this.m_sharedMaterial);
		}

		private void SetSharedFontMaterial(Material mat)
		{
			ShaderUtilities.GetShaderPropertyIDs();
			if (this.m_uiRenderer == null)
			{
				this.m_uiRenderer = base.GetComponent<CanvasRenderer>();
			}
			if (mat == null)
			{
				mat = this.m_baseMaterial;
				this.m_isNewBaseMaterial = true;
			}
			this.m_stencilID = MaterialManager.GetStencilID(base.gameObject);
			if (this.m_stencilID == 0)
			{
				if (this.m_maskingMaterial != null)
				{
					MaterialManager.ReleaseStencilMaterial(this.m_maskingMaterial);
					this.m_maskingMaterial = null;
				}
				this.m_baseMaterial = mat;
			}
			else
			{
				if (this.m_maskingMaterial == null)
				{
					this.m_maskingMaterial = MaterialManager.GetStencilMaterial(mat, this.m_stencilID);
				}
				else if (this.m_maskingMaterial.GetInt(ShaderUtilities.ID_StencilID) != this.m_stencilID || this.m_isNewBaseMaterial)
				{
					MaterialManager.ReleaseStencilMaterial(this.m_maskingMaterial);
					this.m_maskingMaterial = MaterialManager.GetStencilMaterial(mat, this.m_stencilID);
				}
				mat = this.m_maskingMaterial;
			}
			this.m_isNewBaseMaterial = false;
			this.m_sharedMaterial = mat;
			this.SetShaderDepth();
			this.m_uiRenderer.SetMaterial(this.m_sharedMaterial, null);
			this.m_padding = ShaderUtilities.GetPadding(this.m_sharedMaterial, this.m_enableExtraPadding, this.m_isUsingBold);
			this.m_alignmentPadding = ShaderUtilities.GetFontExtent(this.m_sharedMaterial);
		}

		private void SetFontBaseMaterial(Material mat)
		{
			Debug.Log(string.Concat(new string[]
			{
				"Changing Base Material from [",
				(this.m_lastBaseMaterial == null) ? "Null" : this.m_lastBaseMaterial.name,
				"] to [",
				mat.name,
				"]."
			}));
			this.m_baseMaterial = mat;
			this.m_lastBaseMaterial = mat;
		}

		private void SetOutlineThickness(float thickness)
		{
			if (this.m_fontMaterial == null)
			{
				this.m_fontMaterial = this.CreateMaterialInstance(this.m_sharedMaterial);
				this.m_uiRenderer.SetMaterial(this.m_fontMaterial, null);
			}
			thickness = Mathf.Clamp01(thickness);
			this.m_uiRenderer.GetMaterial().SetFloat(ShaderUtilities.ID_OutlineWidth, thickness);
		}

		private void SetFaceColor(Color32 color)
		{
			if (this.m_fontMaterial == null)
			{
				this.m_fontMaterial = this.CreateMaterialInstance(this.m_sharedMaterial);
				this.m_uiRenderer.SetMaterial(this.m_fontMaterial, null);
			}
			this.m_uiRenderer.GetMaterial().SetColor(ShaderUtilities.ID_FaceColor, color);
		}

		private void SetOutlineColor(Color32 color)
		{
			if (this.m_fontMaterial == null)
			{
				this.m_fontMaterial = this.CreateMaterialInstance(this.m_sharedMaterial);
				this.m_uiRenderer.SetMaterial(this.m_fontMaterial, null);
			}
			this.m_uiRenderer.GetMaterial().SetColor(ShaderUtilities.ID_OutlineColor, color);
		}

		private Material CreateMaterialInstance(Material source)
		{
			Material material = new Material(source);
			material.shaderKeywords = source.shaderKeywords;
			material.hideFlags = HideFlags.DontSave;
			Material material2 = material;
			Material expr_1D = material2;
			expr_1D.name += " (Instance)";
			return material;
		}

		private void SetShaderDepth()
		{
			if (this.m_canvas == null)
			{
				return;
			}
			if (this.m_canvas.renderMode == RenderMode.ScreenSpaceOverlay || this.m_isOverlay)
			{
				this.m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 0f);
			}
			else
			{
				this.m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 4f);
			}
		}

		private void SetCulling()
		{
			if (this.m_isCullingEnabled)
			{
				this.m_uiRenderer.GetMaterial().SetFloat("_CullMode", 2f);
			}
			else
			{
				this.m_uiRenderer.GetMaterial().SetFloat("_CullMode", 0f);
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

		private void SetMeshArrays(int size)
		{
			int num = size * 4;
			this.m_uiVertices = new UIVertex[num];
			for (int i = 0; i < size; i++)
			{
				int num2 = i * 4;
				this.m_uiVertices[0 + num2].position = Vector3.zero;
				this.m_uiVertices[1 + num2].position = Vector3.zero;
				this.m_uiVertices[2 + num2].position = Vector3.zero;
				this.m_uiVertices[3 + num2].position = Vector3.zero;
				this.m_uiVertices[0 + num2].normal = new Vector3(0f, 0f, -1f);
				this.m_uiVertices[1 + num2].normal = new Vector3(0f, 0f, -1f);
				this.m_uiVertices[2 + num2].normal = new Vector3(0f, 0f, -1f);
				this.m_uiVertices[3 + num2].normal = new Vector3(0f, 0f, -1f);
				this.m_uiVertices[0 + num2].tangent = new Vector4(-1f, 0f, 0f, 1f);
				this.m_uiVertices[1 + num2].tangent = new Vector4(-1f, 0f, 0f, 1f);
				this.m_uiVertices[2 + num2].tangent = new Vector4(-1f, 0f, 0f, 1f);
				this.m_uiVertices[3 + num2].tangent = new Vector4(-1f, 0f, 0f, 1f);
			}
			this.m_uiRenderer.SetVertices(this.m_uiVertices, num);
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
				if (chars.Length <= text.get_Length())
				{
					int num = (text.get_Length() > 1024) ? (text.get_Length() + 256) : Mathf.NextPowerOfTwo(text.get_Length() + 1);
					chars = new int[num];
				}
				int num2 = 0;
				int i = 0;
				while (i < text.get_Length())
				{
					if (text.get_Chars(i) != '\\' || i >= text.get_Length() - 1)
					{
						goto IL_DA;
					}
					int num3 = (int)text.get_Chars(i + 1);
					switch (num3)
					{
					case 114:
						chars[num2] = 13;
						i++;
						num2++;
						goto IL_E9;
					case 116:
						chars[num2] = 9;
						i++;
						num2++;
						goto IL_E9;
					}
					if (num3 != 110)
					{
						goto IL_DA;
					}
					chars[num2] = 10;
					i++;
					num2++;
					IL_E9:
					i++;
					continue;
					IL_DA:
					chars[num2] = (int)text.get_Chars(i);
					num2++;
					goto IL_E9;
				}
				chars[num2] = 0;
			}
		}

		private void SetTextArrayToCharArray(char[] charArray, ref int[] charBuffer)
		{
			if (charArray != null && this.m_charArray_Length != 0)
			{
				if (charBuffer.Length <= this.m_charArray_Length)
				{
					int num = (this.m_charArray_Length > 1024) ? (this.m_charArray_Length + 256) : Mathf.NextPowerOfTwo(this.m_charArray_Length + 1);
					charBuffer = new int[num];
				}
				int num2 = 0;
				int i = 0;
				while (i < this.m_charArray_Length)
				{
					if (charArray[i] != '\\' || i >= this.m_charArray_Length - 1)
					{
						goto IL_DD;
					}
					int num3 = (int)charArray[i + 1];
					switch (num3)
					{
					case 114:
						charBuffer[num2] = 13;
						i++;
						num2++;
						goto IL_E8;
					case 116:
						charBuffer[num2] = 9;
						i++;
						num2++;
						goto IL_E8;
					}
					if (num3 != 110)
					{
						goto IL_DD;
					}
					charBuffer[num2] = 10;
					i++;
					num2++;
					IL_E8:
					i++;
					continue;
					IL_DD:
					charBuffer[num2] = (int)charArray[i];
					num2++;
					goto IL_E8;
				}
				charBuffer[num2] = 0;
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
			int num4 = 0;
			this.m_isUsingBold = false;
			this.m_isSprite = false;
			this.m_fontIndex = 0;
			this.m_VisibleCharacters.Clear();
			Array.Clear(this.m_meshAllocCount, 0, 17);
			int num5 = 0;
			while (chars[num5] != 0)
			{
				int num6 = chars[num5];
				if (this.m_isRichText && num6 == 60 && this.ValidateHtmlTag(chars, num5 + 1, out num3))
				{
					num5 = num3;
					if ((this.m_style & FontStyles.Underline) == FontStyles.Underline)
					{
						num += 3;
					}
					if ((this.m_style & FontStyles.Bold) == FontStyles.Bold)
					{
						this.m_isUsingBold = true;
					}
					if (this.m_isSprite)
					{
						num4++;
						num2++;
						this.m_VisibleCharacters.Add((char)(57344 + this.m_spriteIndex));
						this.m_isSprite = false;
					}
				}
				else
				{
					if (num6 != 32 && num6 != 9 && num6 != 10 && num6 != 13)
					{
						num++;
						this.m_meshAllocCount[this.m_fontIndex]++;
					}
					this.m_VisibleCharacters.Add((char)num6);
					num2++;
				}
				num5++;
			}
			if (num4 > 0)
			{
				if (this.m_inlineGraphics == null)
				{
					this.m_inlineGraphics = (base.GetComponent<InlineGraphicManager>() ?? base.gameObject.AddComponent<InlineGraphicManager>());
				}
				this.m_inlineGraphics.AllocatedVertexBuffers(num4);
			}
			else if (this.m_inlineGraphics != null)
			{
				this.m_inlineGraphics.ClearUIVertex();
			}
			this.m_spriteCount = num4;
			if (this.m_textInfo.characterInfo == null || num2 > this.m_textInfo.characterInfo.Length)
			{
				this.m_textInfo.characterInfo = new TMP_CharacterInfo[(num2 > 1024) ? (num2 + 256) : Mathf.NextPowerOfTwo(num2)];
			}
			if (this.m_uiVertices == null)
			{
				this.m_uiVertices = new UIVertex[0];
			}
			if (num * 4 > this.m_uiVertices.Length)
			{
				if (this.m_isFirstAllocation)
				{
					this.SetMeshArrays(num);
					this.m_isFirstAllocation = false;
				}
				else
				{
					this.SetMeshArrays((num > 1024) ? (num + 256) : Mathf.NextPowerOfTwo(num));
				}
			}
			return num2;
		}

		private void MarkLayoutForRebuild()
		{
			if (this.m_rectTransform == null)
			{
				this.m_rectTransform = base.GetComponent<RectTransform>();
			}
			LayoutRebuilder.MarkLayoutForRebuild(this.m_rectTransform);
		}

		private void ParseInputText()
		{
			this.isInputParsingRequired = false;
			TextMeshProUGUI.TextInputSources inputSource = this.m_inputSource;
			if (inputSource != TextMeshProUGUI.TextInputSources.Text)
			{
				if (inputSource == TextMeshProUGUI.TextInputSources.SetText)
				{
					this.SetTextArrayToCharArray(this.m_input_CharArray, ref this.m_char_buffer);
				}
			}
			else
			{
				this.StringToCharArray(this.m_text, ref this.m_char_buffer);
			}
		}

		private void ComputeMarginSize()
		{
			if (this.m_rectTransform != null)
			{
				if (this.m_rectTransform.rect.width == 0f)
				{
					this.m_marginWidth = float.PositiveInfinity;
				}
				else
				{
					this.m_marginWidth = this.m_rectTransform.rect.width - this.m_margin.x - this.m_margin.z;
				}
				if (this.m_rectTransform.rect.height == 0f)
				{
					this.m_marginHeight = float.PositiveInfinity;
				}
				else
				{
					this.m_marginHeight = this.m_rectTransform.rect.height - this.m_margin.y - this.m_margin.w;
				}
			}
		}

		protected override void OnRectTransformDimensionsChange()
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			this.ComputeMarginSize();
			if (this.m_rectTransform != null)
			{
				this.m_rectTransform.hasChanged = true;
			}
			else
			{
				this.m_rectTransform = base.GetComponent<RectTransform>();
				this.m_rectTransform.hasChanged = true;
			}
			if (this.m_isRebuildingLayout)
			{
				this.m_isLayoutDirty = true;
			}
			else
			{
				this.havePropertiesChanged = true;
			}
		}

		private void OnPreRenderCanvas()
		{
			this.loopCountA = 0;
			if (this.m_fontAsset == null)
			{
				return;
			}
			if (this.m_rectTransform.hasChanged || this.m_marginsHaveChanged)
			{
				if (this.m_inlineGraphics != null)
				{
					this.m_inlineGraphics.UpdatePivot(this.m_rectTransform.pivot);
				}
				if (this.m_rectTransformDimensionsChanged || this.m_marginsHaveChanged)
				{
					this.ComputeMarginSize();
					if (this.m_marginsHaveChanged)
					{
						this.m_isScrollRegionSet = false;
					}
					this.m_rectTransformDimensionsChanged = false;
					this.m_marginsHaveChanged = false;
					this.m_isCalculateSizeRequired = true;
					this.havePropertiesChanged = true;
				}
				if (this.m_isMaskingEnabled)
				{
					this.UpdateMask();
				}
				this.m_rectTransform.hasChanged = false;
				Vector3 lossyScale = this.m_rectTransform.lossyScale;
				if (lossyScale != this.m_previousLossyScale)
				{
					if (!this.havePropertiesChanged && this.m_previousLossyScale.z != 0f && this.m_text != string.Empty)
					{
						this.UpdateSDFScale(this.m_previousLossyScale.z, lossyScale.z);
					}
					else
					{
						this.havePropertiesChanged = true;
					}
					this.m_previousLossyScale = lossyScale;
				}
			}
			if (this.havePropertiesChanged || this.m_fontAsset.propertiesChanged || this.m_isLayoutDirty)
			{
				if (this.m_canvas == null)
				{
					this.m_canvas = base.GetComponentInParent<Canvas>();
				}
				if (this.m_canvas == null)
				{
					return;
				}
				if (this.hasFontAssetChanged || this.m_fontAsset.propertiesChanged)
				{
					this.LoadFontAsset();
					this.hasFontAssetChanged = false;
					if (this.m_fontAsset == null || this.m_uiRenderer.GetMaterial() == null)
					{
						return;
					}
					this.m_fontAsset.propertiesChanged = false;
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
				this.m_isLayoutDirty = false;
				this.GenerateTextMesh();
				this.havePropertiesChanged = false;
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
			if (this.m_char_buffer == null || this.m_char_buffer.Length == 0 || this.m_char_buffer[0] == 0)
			{
				if (this.m_uiVertices != null)
				{
					this.m_uiRenderer.SetVertices(this.m_uiVertices, 0);
					if (this.m_inlineGraphics != null)
					{
						this.m_inlineGraphics.ClearUIVertex();
					}
				}
				this.m_preferredWidth = 0f;
				this.m_preferredHeight = 0f;
				this.m_renderedWidth = 0f;
				this.m_renderedHeight = 0f;
				LayoutRebuilder.MarkLayoutForRebuild(this.m_rectTransform);
				return;
			}
			int num = this.SetArraySizes(this.m_char_buffer);
			this.m_fontIndex = 0;
			this.m_fontAssetArray[this.m_fontIndex] = this.m_fontAsset;
			this.m_fontScale = this.m_fontSize / this.m_fontAssetArray[this.m_fontIndex].fontInfo.PointSize;
			float fontScale = this.m_fontScale;
			this.m_maxFontScale = 0f;
			float num2 = 0f;
			this.m_currentFontSize = this.m_fontSize;
			this.m_style = this.m_fontStyle;
			this.m_lineJustification = this.m_textAlignment;
			if (this.checkPaddingRequired)
			{
				this.checkPaddingRequired = false;
				this.m_padding = ShaderUtilities.GetPadding(this.m_uiRenderer.GetMaterial(), this.m_enableExtraPadding, this.m_isUsingBold);
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
			this.m_visibleSpriteCount = 0;
			this.m_firstVisibleCharacterOfLine = 0;
			this.m_lastVisibleCharacterOfLine = 0;
			int num4 = 0;
			this.m_rectTransform.GetLocalCorners(this.m_rectCorners);
			Vector4 margin = this.m_margin;
			float marginWidth = this.m_marginWidth;
			float marginHeight = this.m_marginHeight;
			this.m_width = 0f;
			this.m_renderedWidth = 0f;
			this.m_renderedHeight = 0f;
			bool flag3 = true;
			bool flag4 = false;
			this.m_SavedLineState = default(WordWrapState);
			this.m_SavedWordWrapState = default(WordWrapState);
			int num5 = 0;
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
			float num6 = 0f;
			this.m_isNewPage = false;
			this.loopCountA++;
			int num7 = 0;
			int num8 = 0;
			while (this.m_char_buffer[num8] != 0)
			{
				int num9 = this.m_char_buffer[num8];
				this.m_isSprite = false;
				float num10 = 1f;
				if (!this.m_isRichText || num9 != 60 || !this.ValidateHtmlTag(this.m_char_buffer, num8 + 1, out num7))
				{
					goto IL_4E0;
				}
				num8 = num7;
				if (this.m_isRecalculateScaleRequired)
				{
					this.m_fontScale = this.m_currentFontSize / this.m_fontAssetArray[this.m_fontIndex].fontInfo.PointSize;
					this.m_isRecalculateScaleRequired = false;
				}
				if (this.m_isSprite)
				{
					goto IL_4E0;
				}
				IL_4D5:
				num8++;
				continue;
				IL_4E0:
				bool flag5 = false;
				if ((this.m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
				{
					if (char.IsLower((char)num9))
					{
						num9 -= 32;
					}
				}
				else if ((this.m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
				{
					if (char.IsUpper((char)num9))
					{
						num9 += 32;
					}
				}
				else if ((this.m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (this.m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps)
				{
					if (char.IsLower((char)num9))
					{
						this.m_fontScale = this.m_currentFontSize * 0.8f / this.m_fontAssetArray[this.m_fontIndex].fontInfo.PointSize;
						num9 -= 32;
					}
					else
					{
						this.m_fontScale = this.m_currentFontSize / this.m_fontAssetArray[this.m_fontIndex].fontInfo.PointSize;
					}
				}
				if (this.m_isSprite)
				{
					SpriteInfo sprite = this.m_inlineGraphics.GetSprite(this.m_spriteIndex);
					if (sprite == null)
					{
						goto IL_4D5;
					}
					num9 = 57344 + this.m_spriteIndex;
					this.m_cached_GlyphInfo = new GlyphInfo();
					this.m_cached_GlyphInfo.x = sprite.x;
					this.m_cached_GlyphInfo.y = sprite.y;
					this.m_cached_GlyphInfo.width = sprite.width;
					this.m_cached_GlyphInfo.height = sprite.height;
					this.m_cached_GlyphInfo.xOffset = sprite.pivot.x + sprite.xOffset;
					this.m_cached_GlyphInfo.yOffset = sprite.pivot.y + sprite.yOffset;
					num10 = this.m_fontAsset.fontInfo.Ascender / sprite.height * sprite.scale;
					this.m_cached_GlyphInfo.xAdvance = sprite.xAdvance * num10;
					this.m_visibleSpriteCount++;
					this.m_textInfo.characterInfo[this.m_characterCount].type = TMP_CharacterType.Sprite;
				}
				else
				{
					this.m_fontAssetArray[this.m_fontIndex].characterDictionary.TryGetValue(num9, ref this.m_cached_GlyphInfo);
					if (this.m_cached_GlyphInfo == null)
					{
						if (char.IsLower((char)num9))
						{
							if (this.m_fontAssetArray[this.m_fontIndex].characterDictionary.TryGetValue(num9 - 32, ref this.m_cached_GlyphInfo))
							{
								num9 -= 32;
							}
						}
						else if (char.IsUpper((char)num9) && this.m_fontAssetArray[this.m_fontIndex].characterDictionary.TryGetValue(num9 + 32, ref this.m_cached_GlyphInfo))
						{
							num9 += 32;
						}
						if (this.m_cached_GlyphInfo == null)
						{
							this.m_fontAssetArray[this.m_fontIndex].characterDictionary.TryGetValue(88, ref this.m_cached_GlyphInfo);
							if (this.m_cached_GlyphInfo == null)
							{
								Debug.LogWarning("Character with ASCII value of " + num9 + " was not found in the Font Asset Glyph Table.");
								goto IL_4D5;
							}
							Debug.LogWarning("Character with ASCII value of " + num9 + " was not found in the Font Asset Glyph Table.");
							num9 = 88;
							flag5 = true;
						}
					}
					this.m_textInfo.characterInfo[this.m_characterCount].type = TMP_CharacterType.Character;
				}
				this.m_textInfo.characterInfo[this.m_characterCount].character = (char)num9;
				this.m_textInfo.characterInfo[this.m_characterCount].color = this.m_htmlColor;
				this.m_textInfo.characterInfo[this.m_characterCount].style = this.m_style;
				this.m_textInfo.characterInfo[this.m_characterCount].index = (short)num8;
				if (this.m_enableKerning && this.m_characterCount >= 1)
				{
					int character = (int)this.m_textInfo.characterInfo[this.m_characterCount - 1].character;
					KerningPairKey kerningPairKey = new KerningPairKey(character, num9);
					KerningPair kerningPair;
					this.m_fontAssetArray[this.m_fontIndex].kerningDictionary.TryGetValue(kerningPairKey.key, ref kerningPair);
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
				float num13 = this.m_isSprite ? ((float)(this.m_enableExtraPadding ? 4 : 0)) : this.m_padding;
				Vector3 vector = new Vector3(0f + this.m_xAdvance + (this.m_cached_GlyphInfo.xOffset - num13 - num11) * this.m_fontScale * num10, (this.m_cached_GlyphInfo.yOffset + num13) * this.m_fontScale * num10 - this.m_lineOffset + this.m_baselineOffset, 0f);
				Vector3 vector2 = new Vector3(vector.x, vector.y - (this.m_cached_GlyphInfo.height + num13 * 2f) * this.m_fontScale * num10, 0f);
				Vector3 vector3 = new Vector3(vector2.x + (this.m_cached_GlyphInfo.width + num13 * 2f + num11 * 2f) * this.m_fontScale * num10, vector.y, 0f);
				Vector3 vector4 = new Vector3(vector3.x, vector2.y, 0f);
				if ((this.m_style & FontStyles.Italic) == FontStyles.Italic || (this.m_fontStyle & FontStyles.Italic) == FontStyles.Italic)
				{
					float num14 = (float)this.m_fontAssetArray[this.m_fontIndex].ItalicStyle * 0.01f;
					Vector3 b = new Vector3(num14 * ((this.m_cached_GlyphInfo.yOffset + num13 + num11) * this.m_fontScale * num10), 0f, 0f);
					Vector3 b2 = new Vector3(num14 * ((this.m_cached_GlyphInfo.yOffset - this.m_cached_GlyphInfo.height - num13 - num11) * this.m_fontScale * num10), 0f, 0f);
					vector += b;
					vector2 += b2;
					vector3 += b;
					vector4 += b2;
				}
				this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft = vector2;
				this.m_textInfo.characterInfo[this.m_characterCount].topLeft = vector;
				this.m_textInfo.characterInfo[this.m_characterCount].topRight = vector3;
				this.m_textInfo.characterInfo[this.m_characterCount].bottomRight = vector4;
				float num15 = (this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_fontScale + this.m_baselineOffset;
				float num16 = (this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_fontScale - this.m_lineOffset + this.m_baselineOffset;
				if (this.m_isSprite)
				{
					num15 = Mathf.Max(num15, vector.y - num13 * this.m_fontScale * num10);
					num16 = Mathf.Min(num16, vector4.y - num13 * this.m_fontScale * num10);
				}
				if (this.m_lineNumber == 0)
				{
					this.m_maxAscender = ((this.m_maxAscender > num15) ? this.m_maxAscender : num15);
				}
				if (this.m_lineOffset == 0f)
				{
					num6 = ((num6 > num15) ? num6 : num15);
				}
				this.m_textInfo.characterInfo[this.m_characterCount].isVisible = false;
				if ((num9 != 32 && num9 != 9 && num9 != 10 && num9 != 13) || this.m_isSprite)
				{
					this.m_textInfo.characterInfo[this.m_characterCount].isVisible = true;
					if (this.m_baselineOffset == 0f)
					{
						this.m_maxFontScale = Mathf.Max(this.m_maxFontScale, this.m_fontScale);
					}
					float num17 = marginWidth + 0.0001f;
					if (this.m_xAdvance + this.m_cached_GlyphInfo.xAdvance * this.m_fontScale > num17)
					{
						num4 = this.m_characterCount - 1;
						if (this.enableWordWrapping && this.m_characterCount != this.m_firstVisibleCharacterOfLine)
						{
							if (num5 == this.m_SavedWordWrapState.previous_WordBreak || flag3)
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
										Debug.Log("Recursive count exceeded!");
										goto IL_4D5;
									}
								}
							}
							num8 = this.RestoreWordWrappingState(ref this.m_SavedWordWrapState);
							num5 = num8;
							if (this.m_lineNumber > 0 && this.m_maxFontScale != 0f && this.m_lineHeight == 0f && this.m_maxFontScale != num2 && !this.m_isNewPage)
							{
								float num18 = this.m_fontAssetArray[this.m_fontIndex].fontInfo.LineHeight - (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender - this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender);
								float num19 = (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender + this.m_lineSpacing + this.m_paragraphSpacing + num18 + this.m_lineSpacingDelta) * this.m_maxFontScale - (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender - num18) * num2;
								this.m_lineOffset += num19 - num3;
								this.AdjustLineOffset(this.m_firstVisibleCharacterOfLine, this.m_characterCount - 1, num19 - num3);
							}
							this.m_isNewPage = false;
							float num20 = (this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_maxFontScale - this.m_lineOffset;
							float num21 = (this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_fontScale - this.m_lineOffset + this.m_baselineOffset;
							num20 = ((num20 > num21) ? num20 : num21);
							float num22 = (this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_maxFontScale - this.m_lineOffset;
							float num23 = (this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_fontScale - this.m_lineOffset + this.m_baselineOffset;
							num22 = ((num22 < num23) ? num22 : num23);
							if (this.m_textInfo.characterInfo[this.m_firstVisibleCharacterOfLine].isVisible)
							{
								this.m_maxDescender = ((this.m_maxDescender < num22) ? this.m_maxDescender : num22);
							}
							this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex = this.m_firstVisibleCharacterOfLine;
							this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex = ((this.m_characterCount - 1 > 0) ? (this.m_characterCount - 1) : 1);
							this.m_textInfo.lineInfo[this.m_lineNumber].lastVisibleCharacterIndex = this.m_lastVisibleCharacterOfLine;
							this.m_textInfo.lineInfo[this.m_lineNumber].characterCount = this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex - this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex + 1;
							this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_firstVisibleCharacterOfLine].bottomLeft.x, num22);
							this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].topRight.x, num20);
							this.m_textInfo.lineInfo[this.m_lineNumber].lineLength = this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max.x - num13 * this.m_maxFontScale;
							this.m_textInfo.lineInfo[this.m_lineNumber].maxAdvance = this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].xAdvance - this.m_characterSpacing * this.m_fontScale;
							this.m_firstVisibleCharacterOfLine = this.m_characterCount;
							this.m_renderedWidth += this.m_xAdvance;
							if (this.m_enableWordWrapping)
							{
								this.m_renderedHeight = this.m_maxAscender - this.m_maxDescender;
							}
							else
							{
								this.m_renderedHeight = Mathf.Max(this.m_renderedHeight, num20 - num22);
							}
							this.SaveWordWrappingState(ref this.m_SavedLineState, num8, this.m_characterCount - 1);
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
							goto IL_4D5;
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
									this.m_char_buffer[num8 - 1] = 8230;
									this.m_char_buffer[num8] = 0;
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
					Color32 vertexColor;
					if (flag5)
					{
						vertexColor = Color.red;
					}
					else if (this.m_overrideHtmlColors)
					{
						vertexColor = this.m_fontColor32;
					}
					else
					{
						vertexColor = this.m_htmlColor;
					}
					if (!this.m_isSprite)
					{
						this.SaveGlyphVertexInfo(num11, vertexColor);
					}
					else
					{
						this.SaveSpriteVertexInfo(vertexColor);
					}
					if (!this.m_isSprite)
					{
						this.m_visibleCharacterCount++;
					}
					if (this.m_textInfo.characterInfo[this.m_characterCount].isVisible || this.m_isSprite)
					{
						this.m_lastVisibleCharacterOfLine = this.m_characterCount;
					}
				}
				else if (num9 == 9 || num9 == 32)
				{
					TMP_LineInfo[] lineInfo = this.m_textInfo.lineInfo;
					int lineNumber = this.m_lineNumber;
					lineInfo[lineNumber].spaceCount = lineInfo[lineNumber].spaceCount + 1;
					this.m_textInfo.spaceCount++;
				}
				this.m_textInfo.characterInfo[this.m_characterCount].lineNumber = (short)this.m_lineNumber;
				this.m_textInfo.characterInfo[this.m_characterCount].pageNumber = (short)this.m_pageNumber;
				if ((num9 != 10 && num9 != 13 && num9 != 8230) || this.m_textInfo.lineInfo[this.m_lineNumber].characterCount == 1)
				{
					this.m_textInfo.lineInfo[this.m_lineNumber].alignment = this.m_lineJustification;
				}
				if (this.m_maxAscender - num16 + this.m_alignmentPadding.w * 2f * this.m_fontScale > marginHeight)
				{
					if (this.m_enableAutoSizing && this.m_lineSpacingDelta > this.m_lineSpacingMax && this.m_lineNumber > 0)
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
							if (num9 != 13 && num9 != 10)
							{
								num8 = this.RestoreWordWrappingState(ref this.m_SavedLineState);
								if (num8 == 0)
								{
									this.m_char_buffer[0] = 0;
									this.GenerateTextMesh();
									this.m_isTextTruncated = true;
									return;
								}
								this.m_isNewPage = true;
								this.m_xAdvance = 0f + this.m_indent;
								this.m_lineOffset = 0f;
								this.m_pageNumber++;
								this.m_lineNumber++;
								goto IL_4D5;
							}
							break;
						}
					}
				}
				if (num9 == 9)
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
				if (num9 == 13)
				{
					this.m_maxXAdvance = Mathf.Max(this.m_maxXAdvance, this.m_renderedWidth + this.m_xAdvance + this.m_alignmentPadding.z * this.m_fontScale);
					this.m_renderedWidth = 0f;
					this.m_xAdvance = 0f + this.m_indent;
				}
				if (num9 == 10 || this.m_characterCount == num - 1)
				{
					if (this.m_lineNumber > 0 && this.m_maxFontScale != 0f && this.m_lineHeight == 0f && this.m_maxFontScale != num2 && !this.m_isNewPage)
					{
						float num24 = this.m_fontAssetArray[this.m_fontIndex].fontInfo.LineHeight - (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender - this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender);
						float num25 = (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender + this.m_lineSpacing + this.m_paragraphSpacing + num24 + this.m_lineSpacingDelta) * this.m_maxFontScale - (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender - num24) * num2;
						this.m_lineOffset += num25 - num3;
						this.AdjustLineOffset(this.m_firstVisibleCharacterOfLine, this.m_characterCount, num25 - num3);
					}
					this.m_isNewPage = false;
					float num26 = (this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_maxFontScale - this.m_lineOffset;
					float num27 = (this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_fontScale - this.m_lineOffset + this.m_baselineOffset;
					num26 = ((num26 > num27) ? num26 : num27);
					float num28 = (this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_maxFontScale - this.m_lineOffset;
					float num29 = (this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_fontScale - this.m_lineOffset + this.m_baselineOffset;
					num28 = ((num28 < num29) ? num28 : num29);
					this.m_maxDescender = ((this.m_maxDescender < num28) ? this.m_maxDescender : num28);
					this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex = this.m_firstVisibleCharacterOfLine;
					this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex = this.m_characterCount;
					this.m_textInfo.lineInfo[this.m_lineNumber].lastVisibleCharacterIndex = this.m_lastVisibleCharacterOfLine;
					this.m_textInfo.lineInfo[this.m_lineNumber].characterCount = this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex - this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex + 1;
					this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_firstVisibleCharacterOfLine].bottomLeft.x, num28);
					this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].topRight.x, num26);
					this.m_textInfo.lineInfo[this.m_lineNumber].lineLength = this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max.x - num13 * this.m_maxFontScale;
					this.m_textInfo.lineInfo[this.m_lineNumber].maxAdvance = this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].xAdvance - this.m_characterSpacing * this.m_fontScale;
					this.m_firstVisibleCharacterOfLine = this.m_characterCount + 1;
					if (num9 == 10 && this.m_characterCount != num - 1)
					{
						this.m_maxXAdvance = Mathf.Max(this.m_maxXAdvance, this.m_renderedWidth + this.m_xAdvance + this.m_alignmentPadding.z * this.m_fontScale);
						this.m_renderedWidth = 0f;
					}
					else
					{
						this.m_renderedWidth = Mathf.Max(this.m_maxXAdvance, this.m_renderedWidth + this.m_xAdvance + this.m_alignmentPadding.z * this.m_fontScale);
					}
					this.m_renderedHeight = this.m_maxAscender - this.m_maxDescender;
					if (num9 == 10)
					{
						this.SaveWordWrappingState(ref this.m_SavedLineState, num8, this.m_characterCount);
						this.SaveWordWrappingState(ref this.m_SavedWordWrapState, num8, this.m_characterCount);
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
							this.m_lineOffset += (this.m_lineHeight + this.m_lineSpacing + this.m_paragraphSpacing) * fontScale;
						}
						num2 = this.m_fontScale;
						this.m_maxFontScale = 0f;
						this.m_xAdvance = 0f + this.m_indent;
						num4 = this.m_characterCount - 1;
					}
				}
				this.m_textInfo.characterInfo[this.m_characterCount].baseLine = this.m_textInfo.characterInfo[this.m_characterCount].topRight.y - (this.m_cached_GlyphInfo.yOffset + num13) * this.m_fontScale;
				this.m_textInfo.characterInfo[this.m_characterCount].topLine = this.m_textInfo.characterInfo[this.m_characterCount].baseLine + (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_fontScale;
				this.m_textInfo.characterInfo[this.m_characterCount].bottomLine = this.m_textInfo.characterInfo[this.m_characterCount].baseLine + (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender - this.m_alignmentPadding.w) * this.m_fontScale;
				this.m_textInfo.characterInfo[this.m_characterCount].padding = num13 * this.m_fontScale;
				this.m_textInfo.characterInfo[this.m_characterCount].aspectRatio = this.m_cached_GlyphInfo.width / this.m_cached_GlyphInfo.height;
				this.m_textInfo.characterInfo[this.m_characterCount].scale = this.m_fontScale;
				this.m_textInfo.characterInfo[this.m_characterCount].meshIndex = this.m_fontIndex;
				if (this.m_textInfo.characterInfo[this.m_characterCount].isVisible)
				{
					this.m_meshExtents.min = new Vector2(Mathf.Min(this.m_meshExtents.min.x, this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.position.x), Mathf.Min(this.m_meshExtents.min.y, this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.position.y));
					this.m_meshExtents.max = new Vector2(Mathf.Max(this.m_meshExtents.max.x, this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.position.x), Mathf.Max(this.m_meshExtents.max.y, this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.position.y));
				}
				if (num9 != 13 && num9 != 10 && this.m_pageNumber < 16)
				{
					this.m_textInfo.pageInfo[this.m_pageNumber].ascender = num6;
					this.m_textInfo.pageInfo[this.m_pageNumber].descender = ((num16 < this.m_textInfo.pageInfo[this.m_pageNumber].descender) ? num16 : this.m_textInfo.pageInfo[this.m_pageNumber].descender);
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
					if (num9 == 9 || num9 == 32)
					{
						this.SaveWordWrappingState(ref this.m_SavedWordWrapState, num8, this.m_characterCount);
						this.m_isCharacterWrappingEnabled = false;
						flag3 = false;
					}
					else if (((flag3 || this.m_isCharacterWrappingEnabled) && this.m_characterCount < num - 1 && !this.m_fontAsset.lineBreakingInfo.leadingCharacters.ContainsKey(num9) && !this.m_fontAsset.lineBreakingInfo.followingCharacters.ContainsKey((int)this.m_VisibleCharacters.get_Item(this.m_characterCount + 1))) || flag4)
					{
						this.SaveWordWrappingState(ref this.m_SavedWordWrapState, num8, this.m_characterCount);
					}
				}
				this.m_characterCount++;
				goto IL_4D5;
			}
			float num30 = this.m_maxFontSize - this.m_minFontSize;
			if (!this.m_isCharacterWrappingEnabled && this.m_enableAutoSizing && num30 > 0.051f && this.m_fontSize < this.m_fontSizeMax)
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
				this.m_isCharacterWrappingEnabled = false;
				this.m_renderedHeight += ((this.m_margin.y > 0f) ? this.m_margin.y : 0f);
				if (this.m_renderMode == TextRenderFlags.GetPreferredSizes)
				{
					return;
				}
				if (!this.IsRectTransformDriven)
				{
					this.m_preferredWidth = this.m_renderedWidth;
					this.m_preferredHeight = this.m_renderedHeight;
				}
				if (this.m_visibleCharacterCount == 0 && this.m_visibleSpriteCount == 0)
				{
					if (this.m_uiVertices != null)
					{
						this.m_uiRenderer.SetVertices(this.m_uiVertices, 0);
					}
					return;
				}
				int num31 = this.m_visibleCharacterCount * 4;
				Array.Clear(this.m_uiVertices, num31, this.m_uiVertices.Length - num31);
				switch (this.m_textAlignment)
				{
				case TextAlignmentOptions.TopLeft:
				case TextAlignmentOptions.Top:
				case TextAlignmentOptions.TopRight:
				case TextAlignmentOptions.TopJustified:
					if (this.m_overflowMode != TextOverflowModes.Page)
					{
						this.m_anchorOffset = this.m_rectCorners[1] + new Vector3(0f + margin.x, 0f - this.m_maxAscender - margin.y, 0f);
					}
					else
					{
						this.m_anchorOffset = this.m_rectCorners[1] + new Vector3(0f + margin.x, 0f - this.m_textInfo.pageInfo[this.m_pageToDisplay].ascender - margin.y, 0f);
					}
					break;
				case TextAlignmentOptions.Left:
				case TextAlignmentOptions.Center:
				case TextAlignmentOptions.Right:
				case TextAlignmentOptions.Justified:
					if (this.m_overflowMode != TextOverflowModes.Page)
					{
						this.m_anchorOffset = (this.m_rectCorners[0] + this.m_rectCorners[1]) / 2f + new Vector3(0f + margin.x, 0f - (this.m_maxAscender + margin.y + this.m_maxDescender - margin.w) / 2f, 0f);
					}
					else
					{
						this.m_anchorOffset = (this.m_rectCorners[0] + this.m_rectCorners[1]) / 2f + new Vector3(0f + margin.x, 0f - (this.m_textInfo.pageInfo[this.m_pageToDisplay].ascender + margin.y + this.m_textInfo.pageInfo[this.m_pageToDisplay].descender - margin.w) / 2f, 0f);
					}
					break;
				case TextAlignmentOptions.BottomLeft:
				case TextAlignmentOptions.Bottom:
				case TextAlignmentOptions.BottomRight:
				case TextAlignmentOptions.BottomJustified:
					if (this.m_overflowMode != TextOverflowModes.Page)
					{
						this.m_anchorOffset = this.m_rectCorners[0] + new Vector3(0f + margin.x, 0f - this.m_maxDescender + margin.w, 0f);
					}
					else
					{
						this.m_anchorOffset = this.m_rectCorners[0] + new Vector3(0f + margin.x, 0f - this.m_textInfo.pageInfo[this.m_pageToDisplay].descender + margin.w, 0f);
					}
					break;
				case TextAlignmentOptions.BaselineLeft:
				case TextAlignmentOptions.Baseline:
				case TextAlignmentOptions.BaselineRight:
					this.m_anchorOffset = (this.m_rectCorners[0] + this.m_rectCorners[1]) / 2f + new Vector3(0f + margin.x, 0f, 0f);
					break;
				case TextAlignmentOptions.MidlineLeft:
				case TextAlignmentOptions.Midline:
				case TextAlignmentOptions.MidlineRight:
				case TextAlignmentOptions.MidlineJustified:
					this.m_anchorOffset = (this.m_rectCorners[0] + this.m_rectCorners[1]) / 2f + new Vector3(0f + margin.x, 0f - (this.m_meshExtents.max.y + margin.y + this.m_meshExtents.min.y - margin.w) / 2f, 0f);
					break;
				}
				Vector3 vector5 = Vector3.zero;
				Vector3 b3 = Vector3.zero;
				int num32 = 0;
				int num33 = 0;
				Array.Clear(this.m_meshAllocCount, 0, 17);
				int num34 = 0;
				Color32 color = new Color32(255, 255, 255, 127);
				int num35 = 0;
				int num36 = 0;
				int num37 = 0;
				bool flag6 = false;
				int num38 = 0;
				float z = this.m_rectTransform.lossyScale.z;
				RenderMode renderMode = this.m_canvas.renderMode;
				float scaleFactor = this.m_canvas.scaleFactor;
				bool flag7 = !(this.m_canvas.worldCamera == null);
				for (int j = 0; j < this.m_characterCount; j++)
				{
					TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
					int lineNumber2 = (int)characterInfo[j].lineNumber;
					char character2 = characterInfo[j].character;
					TMP_LineInfo tMP_LineInfo = this.m_textInfo.lineInfo[lineNumber2];
					TextAlignmentOptions alignment = tMP_LineInfo.alignment;
					num36 = lineNumber2 + 1;
					switch (alignment)
					{
					case TextAlignmentOptions.TopLeft:
					case TextAlignmentOptions.Left:
					case TextAlignmentOptions.BottomLeft:
					case TextAlignmentOptions.BaselineLeft:
					case TextAlignmentOptions.MidlineLeft:
						vector5 = Vector3.zero;
						break;
					case TextAlignmentOptions.Top:
					case TextAlignmentOptions.Center:
					case TextAlignmentOptions.Bottom:
					case TextAlignmentOptions.Baseline:
					case TextAlignmentOptions.Midline:
						vector5 = new Vector3(marginWidth / 2f - tMP_LineInfo.maxAdvance / 2f, 0f, 0f);
						break;
					case TextAlignmentOptions.TopRight:
					case TextAlignmentOptions.Right:
					case TextAlignmentOptions.BottomRight:
					case TextAlignmentOptions.BaselineRight:
					case TextAlignmentOptions.MidlineRight:
						vector5 = new Vector3(marginWidth - tMP_LineInfo.maxAdvance, 0f, 0f);
						break;
					case TextAlignmentOptions.TopJustified:
					case TextAlignmentOptions.Justified:
					case TextAlignmentOptions.BottomJustified:
					case TextAlignmentOptions.BaselineJustified:
					case TextAlignmentOptions.MidlineJustified:
					{
						int character3 = (int)this.m_textInfo.characterInfo[j].character;
						char character4 = this.m_textInfo.characterInfo[tMP_LineInfo.lastCharacterIndex].character;
						if (char.IsWhiteSpace(character4) && !char.IsControl(character4) && lineNumber2 < this.m_lineNumber)
						{
							float num39 = this.m_rectCorners[3].x - margin.z - (this.m_rectCorners[0].x + margin.x) - tMP_LineInfo.maxAdvance;
							if (lineNumber2 != num37 || j == 0)
							{
								vector5 = Vector3.zero;
							}
							else if (character3 == 9 || character3 == 32)
							{
								vector5 += new Vector3(num39 * (1f - this.m_wordWrappingRatios) / (float)(tMP_LineInfo.spaceCount - 1), 0f, 0f);
							}
							else
							{
								vector5 += new Vector3(num39 * this.m_wordWrappingRatios / (float)(tMP_LineInfo.characterCount - tMP_LineInfo.spaceCount - 1), 0f, 0f);
							}
						}
						else
						{
							vector5 = Vector3.zero;
						}
						break;
					}
					}
					b3 = this.m_anchorOffset + vector5;
					if (characterInfo[j].isVisible)
					{
						TMP_CharacterType type = characterInfo[j].type;
						TMP_CharacterType tMP_CharacterType = type;
						if (tMP_CharacterType != TMP_CharacterType.Character)
						{
							if (tMP_CharacterType != TMP_CharacterType.Sprite)
							{
							}
						}
						else
						{
							Extents lineExtents = tMP_LineInfo.lineExtents;
							float num40 = this.m_uvLineOffset * (float)lineNumber2 % 1f + this.m_uvOffset.x;
							switch (this.m_horizontalMapping)
							{
							case TextureMappingOptions.Character:
								characterInfo[j].vertex_BL.uv2.x = 0f + this.m_uvOffset.x;
								characterInfo[j].vertex_TL.uv2.x = 0f + this.m_uvOffset.x;
								characterInfo[j].vertex_TR.uv2.x = 1f + this.m_uvOffset.x;
								characterInfo[j].vertex_BR.uv2.x = 1f + this.m_uvOffset.x;
								break;
							case TextureMappingOptions.Line:
								if (this.m_textAlignment != TextAlignmentOptions.Justified)
								{
									characterInfo[j].vertex_BL.uv2.x = (characterInfo[j].vertex_BL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num40;
									characterInfo[j].vertex_TL.uv2.x = (characterInfo[j].vertex_TL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num40;
									characterInfo[j].vertex_TR.uv2.x = (characterInfo[j].vertex_TR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num40;
									characterInfo[j].vertex_BR.uv2.x = (characterInfo[j].vertex_BR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num40;
								}
								else
								{
									characterInfo[j].vertex_BL.uv2.x = (characterInfo[j].vertex_BL.position.x + vector5.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num40;
									characterInfo[j].vertex_TL.uv2.x = (characterInfo[j].vertex_TL.position.x + vector5.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num40;
									characterInfo[j].vertex_TR.uv2.x = (characterInfo[j].vertex_TR.position.x + vector5.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num40;
									characterInfo[j].vertex_BR.uv2.x = (characterInfo[j].vertex_BR.position.x + vector5.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num40;
								}
								break;
							case TextureMappingOptions.Paragraph:
							{
								characterInfo[j].vertex_BL.uv2.x = (characterInfo[j].vertex_BL.position.x + vector5.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num40;
								characterInfo[j].vertex_TL.uv2.x = (characterInfo[j].vertex_TL.position.x + vector5.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num40;
								characterInfo[j].vertex_TR.uv2.x = (characterInfo[j].vertex_TR.position.x + vector5.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num40;
								characterInfo[j].vertex_BR.uv2.x = (characterInfo[j].vertex_BR.position.x + vector5.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num40;
								Vector3 vector6 = characterInfo[j].vertex_BL.position + vector5;
								Vector3 vector7 = characterInfo[j].vertex_TL.position + vector5;
								Vector3 vector8 = characterInfo[j].vertex_TR.position + vector5;
								Vector3 vector9 = characterInfo[j].vertex_BR.position + vector5;
								Debug.DrawLine(vector6, vector7, Color.green, 60f);
								Debug.DrawLine(vector7, vector8, Color.green, 60f);
								Debug.DrawLine(vector8, vector9, Color.green, 60f);
								Debug.DrawLine(vector9, vector6, Color.green, 60f);
								vector6 = this.m_meshExtents.min + new Vector2(vector5.x * 0f, vector5.y);
								vector7 = new Vector3(this.m_meshExtents.min.x, this.m_meshExtents.max.y, 0f) + new Vector3(vector5.x * 0f, vector5.y, 0f);
								vector8 = this.m_meshExtents.max + new Vector2(vector5.x * 0f, vector5.y);
								vector9 = new Vector3(this.m_meshExtents.max.x, this.m_meshExtents.min.y, 0f) + new Vector3(vector5.x * 0f, vector5.y, 0f);
								Debug.DrawLine(vector6, vector7, Color.red, 60f);
								Debug.DrawLine(vector7, vector8, Color.red, 60f);
								Debug.DrawLine(vector8, vector9, Color.red, 60f);
								Debug.DrawLine(vector9, vector6, Color.red, 60f);
								break;
							}
							case TextureMappingOptions.MatchAspect:
							{
								switch (this.m_verticalMapping)
								{
								case TextureMappingOptions.Character:
									characterInfo[j].vertex_BL.uv2.y = 0f + this.m_uvOffset.y;
									characterInfo[j].vertex_TL.uv2.y = 1f + this.m_uvOffset.y;
									characterInfo[j].vertex_TR.uv2.y = 0f + this.m_uvOffset.y;
									characterInfo[j].vertex_BR.uv2.y = 1f + this.m_uvOffset.y;
									break;
								case TextureMappingOptions.Line:
									characterInfo[j].vertex_BL.uv2.y = (characterInfo[j].vertex_BL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num40;
									characterInfo[j].vertex_TL.uv2.y = (characterInfo[j].vertex_TL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num40;
									characterInfo[j].vertex_TR.uv2.y = characterInfo[j].vertex_BL.uv2.y;
									characterInfo[j].vertex_BR.uv2.y = characterInfo[j].vertex_TL.uv2.y;
									break;
								case TextureMappingOptions.Paragraph:
									characterInfo[j].vertex_BL.uv2.y = (characterInfo[j].vertex_BL.position.y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y) + num40;
									characterInfo[j].vertex_TL.uv2.y = (characterInfo[j].vertex_TL.position.y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y) + num40;
									characterInfo[j].vertex_TR.uv2.y = characterInfo[j].vertex_BL.uv2.y;
									characterInfo[j].vertex_BR.uv2.y = characterInfo[j].vertex_TL.uv2.y;
									break;
								case TextureMappingOptions.MatchAspect:
									Debug.Log("ERROR: Cannot Match both Vertical & Horizontal.");
									break;
								}
								float num41 = (1f - (characterInfo[j].vertex_BL.uv2.y + characterInfo[j].vertex_TL.uv2.y) * characterInfo[j].aspectRatio) / 2f;
								characterInfo[j].vertex_BL.uv2.x = characterInfo[j].vertex_BL.uv2.y * characterInfo[j].aspectRatio + num41 + num40;
								characterInfo[j].vertex_TL.uv2.x = characterInfo[j].vertex_BL.uv2.x;
								characterInfo[j].vertex_TR.uv2.x = characterInfo[j].vertex_TL.uv2.y * characterInfo[j].aspectRatio + num41 + num40;
								characterInfo[j].vertex_BR.uv2.x = characterInfo[j].vertex_TR.uv2.x;
								break;
							}
							}
							switch (this.m_verticalMapping)
							{
							case TextureMappingOptions.Character:
								characterInfo[j].vertex_BL.uv2.y = 0f + this.m_uvOffset.y;
								characterInfo[j].vertex_TL.uv2.y = 1f + this.m_uvOffset.y;
								characterInfo[j].vertex_TR.uv2.y = 1f + this.m_uvOffset.y;
								characterInfo[j].vertex_BR.uv2.y = 0f + this.m_uvOffset.y;
								break;
							case TextureMappingOptions.Line:
								characterInfo[j].vertex_BL.uv2.y = (characterInfo[j].vertex_BL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + this.m_uvOffset.y;
								characterInfo[j].vertex_TL.uv2.y = (characterInfo[j].vertex_TL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + this.m_uvOffset.y;
								characterInfo[j].vertex_BR.uv2.y = characterInfo[j].vertex_BL.uv2.y;
								characterInfo[j].vertex_TR.uv2.y = characterInfo[j].vertex_TL.uv2.y;
								break;
							case TextureMappingOptions.Paragraph:
								characterInfo[j].vertex_BL.uv2.y = (characterInfo[j].vertex_BL.position.y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y) + this.m_uvOffset.y;
								characterInfo[j].vertex_TL.uv2.y = (characterInfo[j].vertex_TL.position.y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y) + this.m_uvOffset.y;
								characterInfo[j].vertex_BR.uv2.y = characterInfo[j].vertex_BL.uv2.y;
								characterInfo[j].vertex_TR.uv2.y = characterInfo[j].vertex_TL.uv2.y;
								break;
							case TextureMappingOptions.MatchAspect:
							{
								float num42 = (1f - (characterInfo[j].vertex_BL.uv2.x + characterInfo[j].vertex_TR.uv2.x) / characterInfo[j].aspectRatio) / 2f;
								characterInfo[j].vertex_BL.uv2.y = num42 + characterInfo[j].vertex_BL.uv2.x / characterInfo[j].aspectRatio + this.m_uvOffset.y;
								characterInfo[j].vertex_TL.uv2.y = num42 + characterInfo[j].vertex_TR.uv2.x / characterInfo[j].aspectRatio + this.m_uvOffset.y;
								characterInfo[j].vertex_BR.uv2.y = characterInfo[j].vertex_BL.uv2.y;
								characterInfo[j].vertex_TR.uv2.y = characterInfo[j].vertex_TL.uv2.y;
								break;
							}
							}
							float num43 = characterInfo[j].scale;
							if ((characterInfo[j].style & FontStyles.Bold) == FontStyles.Bold)
							{
								num43 *= -1f;
							}
							switch (renderMode)
							{
							case RenderMode.ScreenSpaceOverlay:
								num43 *= z / scaleFactor;
								break;
							case RenderMode.ScreenSpaceCamera:
								num43 *= (flag7 ? z : 1f);
								break;
							case RenderMode.WorldSpace:
								num43 *= z;
								break;
							}
							float num44 = characterInfo[j].vertex_BL.uv2.x;
							float num45 = characterInfo[j].vertex_BL.uv2.y;
							float num46 = characterInfo[j].vertex_TR.uv2.x;
							float num47 = characterInfo[j].vertex_TR.uv2.y;
							float num48 = Mathf.Floor(num44);
							float num49 = Mathf.Floor(num45);
							num44 -= num48;
							num46 -= num48;
							num45 -= num49;
							num47 -= num49;
							characterInfo[j].vertex_BL.uv2 = this.PackUV(num44, num45, num43);
							characterInfo[j].vertex_TL.uv2 = this.PackUV(num44, num47, num43);
							characterInfo[j].vertex_TR.uv2 = this.PackUV(num46, num47, num43);
							characterInfo[j].vertex_BR.uv2 = this.PackUV(num46, num45, num43);
						}
						if ((this.m_maxVisibleCharacters != -1 && j >= this.m_maxVisibleCharacters) || (this.m_maxVisibleLines != -1 && lineNumber2 >= this.m_maxVisibleLines) || (this.m_overflowMode == TextOverflowModes.Page && (int)characterInfo[j].pageNumber != this.m_pageToDisplay))
						{
							TMP_CharacterInfo[] array = characterInfo;
							int num50 = j;
							array[num50].vertex_BL.position = array[num50].vertex_BL.position * 0f;
							TMP_CharacterInfo[] array2 = characterInfo;
							int num51 = j;
							array2[num51].vertex_TL.position = array2[num51].vertex_TL.position * 0f;
							TMP_CharacterInfo[] array3 = characterInfo;
							int num52 = j;
							array3[num52].vertex_TR.position = array3[num52].vertex_TR.position * 0f;
							TMP_CharacterInfo[] array4 = characterInfo;
							int num53 = j;
							array4[num53].vertex_BR.position = array4[num53].vertex_BR.position * 0f;
						}
						else
						{
							TMP_CharacterInfo[] array5 = characterInfo;
							int num54 = j;
							array5[num54].vertex_BL.position = array5[num54].vertex_BL.position + b3;
							TMP_CharacterInfo[] array6 = characterInfo;
							int num55 = j;
							array6[num55].vertex_TL.position = array6[num55].vertex_TL.position + b3;
							TMP_CharacterInfo[] array7 = characterInfo;
							int num56 = j;
							array7[num56].vertex_TR.position = array7[num56].vertex_TR.position + b3;
							TMP_CharacterInfo[] array8 = characterInfo;
							int num57 = j;
							array8[num57].vertex_BR.position = array8[num57].vertex_BR.position + b3;
						}
						if (type == TMP_CharacterType.Character)
						{
							this.FillCharacterVertexBuffers(j, num32);
							num32 += 4;
						}
						else if (type == TMP_CharacterType.Sprite)
						{
							this.FillSpriteVertexBuffers(j, num33);
							num33 += 4;
						}
					}
					TMP_CharacterInfo[] characterInfo2 = this.m_textInfo.characterInfo;
					int num58 = j;
					characterInfo2[num58].bottomLeft = characterInfo2[num58].bottomLeft + b3;
					TMP_CharacterInfo[] characterInfo3 = this.m_textInfo.characterInfo;
					int num59 = j;
					characterInfo3[num59].topRight = characterInfo3[num59].topRight + b3;
					TMP_CharacterInfo[] characterInfo4 = this.m_textInfo.characterInfo;
					int num60 = j;
					characterInfo4[num60].topLine = characterInfo4[num60].topLine + b3.y;
					TMP_CharacterInfo[] characterInfo5 = this.m_textInfo.characterInfo;
					int num61 = j;
					characterInfo5[num61].bottomLine = characterInfo5[num61].bottomLine + b3.y;
					TMP_CharacterInfo[] characterInfo6 = this.m_textInfo.characterInfo;
					int num62 = j;
					characterInfo6[num62].baseLine = characterInfo6[num62].baseLine + b3.y;
					this.m_textInfo.lineInfo[lineNumber2].ascender = ((this.m_textInfo.characterInfo[j].topLine > this.m_textInfo.lineInfo[lineNumber2].ascender) ? this.m_textInfo.characterInfo[j].topLine : this.m_textInfo.lineInfo[lineNumber2].ascender);
					this.m_textInfo.lineInfo[lineNumber2].descender = ((this.m_textInfo.characterInfo[j].bottomLine < this.m_textInfo.lineInfo[lineNumber2].descender) ? this.m_textInfo.characterInfo[j].bottomLine : this.m_textInfo.lineInfo[lineNumber2].descender);
					if (lineNumber2 != num37 || j == this.m_characterCount - 1)
					{
						if (lineNumber2 != num37)
						{
							this.m_textInfo.lineInfo[num37].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[num37].firstCharacterIndex].bottomLeft.x, this.m_textInfo.lineInfo[num37].descender);
							this.m_textInfo.lineInfo[num37].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[num37].lastVisibleCharacterIndex].topRight.x, this.m_textInfo.lineInfo[num37].ascender);
						}
						if (j == this.m_characterCount - 1)
						{
							this.m_textInfo.lineInfo[lineNumber2].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[lineNumber2].firstCharacterIndex].bottomLeft.x, this.m_textInfo.lineInfo[lineNumber2].descender);
							this.m_textInfo.lineInfo[lineNumber2].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[lineNumber2].lastVisibleCharacterIndex].topRight.x, this.m_textInfo.lineInfo[lineNumber2].ascender);
						}
					}
					if (char.IsLetterOrDigit(character2) && j < this.m_characterCount - 1)
					{
						if (!flag6)
						{
							flag6 = true;
							num38 = j;
						}
					}
					else if (((char.IsPunctuation(character2) || char.IsWhiteSpace(character2) || j == this.m_characterCount - 1) && flag6) || j == 0)
					{
						int num63 = (j == this.m_characterCount - 1 && char.IsLetterOrDigit(character2)) ? j : (j - 1);
						flag6 = false;
						num35++;
						TMP_LineInfo[] lineInfo2 = this.m_textInfo.lineInfo;
						int num64 = lineNumber2;
						lineInfo2[num64].wordCount = lineInfo2[num64].wordCount + 1;
						TMP_WordInfo tMP_WordInfo = default(TMP_WordInfo);
						tMP_WordInfo.firstCharacterIndex = num38;
						tMP_WordInfo.lastCharacterIndex = num63;
						tMP_WordInfo.characterCount = num63 - num38 + 1;
						this.m_textInfo.wordInfo.Add(tMP_WordInfo);
					}
					bool flag8 = (this.m_textInfo.characterInfo[j].style & FontStyles.Underline) == FontStyles.Underline;
					if (flag8)
					{
						if (!flag && character2 != ' ' && character2 != '\n' && character2 != '\r')
						{
							flag = true;
							zero = new Vector3(this.m_textInfo.characterInfo[j].bottomLeft.x, this.m_textInfo.characterInfo[j].baseLine + this.font.fontInfo.Underline * this.m_fontScale, 0f);
							color = this.m_textInfo.characterInfo[j].color;
						}
						if (this.m_characterCount == 1)
						{
							flag = false;
							zero2 = new Vector3(this.m_textInfo.characterInfo[j].topRight.x, this.m_textInfo.characterInfo[j].baseLine + this.font.fontInfo.Underline * this.m_fontScale, 0f);
							this.DrawUnderlineMesh(zero, zero2, ref num31, color);
							num34++;
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
							this.DrawUnderlineMesh(zero, zero2, ref num31, color);
							num34++;
						}
					}
					else if (flag)
					{
						flag = false;
						zero2 = new Vector3(this.m_textInfo.characterInfo[j - 1].topRight.x, this.m_textInfo.characterInfo[j - 1].baseLine + this.font.fontInfo.Underline * this.m_fontScale, 0f);
						this.DrawUnderlineMesh(zero, zero2, ref num31, color);
						num34++;
					}
					bool flag9 = (this.m_textInfo.characterInfo[j].style & FontStyles.Strikethrough) == FontStyles.Strikethrough;
					if (flag9)
					{
						if (!flag2 && character2 != ' ' && character2 != '\n' && character2 != '\r')
						{
							flag2 = true;
							zero3 = new Vector3(this.m_textInfo.characterInfo[j].bottomLeft.x, this.m_textInfo.characterInfo[j].baseLine + (this.font.fontInfo.Ascender + this.font.fontInfo.Descender) / 2f * this.m_fontScale, 0f);
							color = this.m_textInfo.characterInfo[j].color;
						}
						if (this.m_characterCount == 1)
						{
							flag2 = false;
							zero4 = new Vector3(this.m_textInfo.characterInfo[j].topRight.x, this.m_textInfo.characterInfo[j].baseLine + (this.font.fontInfo.Ascender + this.font.fontInfo.Descender) / 2f * this.m_fontScale, 0f);
							this.DrawUnderlineMesh(zero3, zero4, ref num31, color);
							num34++;
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
							this.DrawUnderlineMesh(zero3, zero4, ref num31, color);
							num34++;
						}
					}
					else if (flag2)
					{
						flag2 = false;
						zero4 = new Vector3(this.m_textInfo.characterInfo[j - 1].topRight.x, this.m_textInfo.characterInfo[j - 1].baseLine + (this.font.fontInfo.Ascender + this.font.fontInfo.Descender) / 2f * this.m_fontScale, 0f);
						this.DrawUnderlineMesh(zero3, zero4, ref num31, color);
						num34++;
					}
					num37 = lineNumber2;
				}
				this.m_textInfo.characterCount = (int)((short)this.m_characterCount);
				this.m_textInfo.spriteCount = this.m_spriteCount;
				this.m_textInfo.lineCount = (int)((short)num36);
				this.m_textInfo.wordCount = (int)((num35 != 0 && this.m_characterCount > 0) ? ((short)num35) : 1);
				this.m_textInfo.pageCount = this.m_pageNumber;
				this.m_textInfo.meshInfo.uiVertices = this.m_uiVertices;
				if (this.m_renderMode == TextRenderFlags.Render)
				{
					this.m_uiRenderer.SetVertices(this.m_uiVertices, num32 + num34 * 12);
					if (this.m_spriteCount > 0 && this.m_inlineGraphics != null)
					{
						this.m_inlineGraphics.DrawSprite(this.m_inlineGraphics.uiVertex, this.m_spriteCount);
					}
				}
				this.m_bounds = new Bounds(new Vector3((this.m_meshExtents.max.x + this.m_meshExtents.min.x) / 2f, (this.m_meshExtents.max.y + this.m_meshExtents.min.y) / 2f, 0f) + b3, new Vector3(this.m_meshExtents.max.x - this.m_meshExtents.min.x, this.m_meshExtents.max.y - this.m_meshExtents.min.y, 0f));
				return;
			}
		}

		private void SaveGlyphVertexInfo(float style_padding, Color32 vertexColor)
		{
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.position = this.m_textInfo.characterInfo[this.m_characterCount].topLeft;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.position = this.m_textInfo.characterInfo[this.m_characterCount].topRight;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomRight;
			vertexColor.a = ((this.m_fontColor32.a < vertexColor.a) ? this.m_fontColor32.a : vertexColor.a);
			if (!this.m_enableVertexGradient)
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = vertexColor;
			}
			else
			{
				if (!this.m_overrideHtmlColors && !this.m_htmlColor.CompareRGB(this.m_fontColor32))
				{
					this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = vertexColor;
					this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = vertexColor;
					this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = vertexColor;
					this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = vertexColor;
				}
				else
				{
					this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = this.m_fontColorGradient.bottomLeft;
					this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = this.m_fontColorGradient.topLeft;
					this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = this.m_fontColorGradient.topRight;
					this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = this.m_fontColorGradient.bottomRight;
				}
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color.a = vertexColor.a;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color.a = vertexColor.a;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color.a = vertexColor.a;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color.a = vertexColor.a;
			}
			if (!this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_WeightNormal))
			{
				style_padding = 0f;
			}
			Vector2 uv = new Vector2((this.m_cached_GlyphInfo.x - this.m_padding - style_padding) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.AtlasWidth, 1f - (this.m_cached_GlyphInfo.y + this.m_padding + style_padding + this.m_cached_GlyphInfo.height) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.AtlasHeight);
			Vector2 uv2 = new Vector2(uv.x, 1f - (this.m_cached_GlyphInfo.y - this.m_padding - style_padding) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.AtlasHeight);
			Vector2 uv3 = new Vector2((this.m_cached_GlyphInfo.x + this.m_padding + style_padding + this.m_cached_GlyphInfo.width) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.AtlasWidth, uv.y);
			Vector2 uv4 = new Vector2(uv3.x, uv2.y);
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.uv = uv;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.uv = uv2;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.uv = uv4;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.uv = uv3;
			Vector3 normal = new Vector3(0f, 0f, -1f);
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.normal = normal;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.normal = normal;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.normal = normal;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.normal = normal;
			Vector4 tangent = new Vector4(-1f, 0f, 0f, 1f);
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.tangent = tangent;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.tangent = tangent;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.tangent = tangent;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.tangent = tangent;
		}

		private void SaveSpriteVertexInfo(Color32 vertexColor)
		{
			int num = this.m_enableExtraPadding ? 4 : 0;
			Vector2 uv = new Vector2((this.m_cached_GlyphInfo.x - (float)num) / (float)this.m_inlineGraphics.spriteAsset.spriteSheet.width, (this.m_cached_GlyphInfo.y - (float)num) / (float)this.m_inlineGraphics.spriteAsset.spriteSheet.height);
			Vector2 uv2 = new Vector2(uv.x, (this.m_cached_GlyphInfo.y + (float)num + this.m_cached_GlyphInfo.height) / (float)this.m_inlineGraphics.spriteAsset.spriteSheet.height);
			Vector2 uv3 = new Vector2((this.m_cached_GlyphInfo.x + (float)num + this.m_cached_GlyphInfo.width) / (float)this.m_inlineGraphics.spriteAsset.spriteSheet.width, uv.y);
			Vector2 uv4 = new Vector2(uv3.x, uv2.y);
			vertexColor.a = ((this.m_fontColor32.a < vertexColor.a) ? this.m_fontColor32.a : vertexColor.a);
			TMP_Vertex tMP_Vertex = default(TMP_Vertex);
			tMP_Vertex.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft;
			tMP_Vertex.uv = uv;
			tMP_Vertex.color = vertexColor;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL = tMP_Vertex;
			tMP_Vertex.position = this.m_textInfo.characterInfo[this.m_characterCount].topLeft;
			tMP_Vertex.uv = uv2;
			tMP_Vertex.color = vertexColor;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL = tMP_Vertex;
			tMP_Vertex.position = this.m_textInfo.characterInfo[this.m_characterCount].topRight;
			tMP_Vertex.uv = uv4;
			tMP_Vertex.color = vertexColor;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR = tMP_Vertex;
			tMP_Vertex.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomRight;
			tMP_Vertex.uv = uv3;
			tMP_Vertex.color = vertexColor;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR = tMP_Vertex;
		}

		private void FillCharacterVertexBuffers(int i, int index_X4)
		{
			TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
			this.m_textInfo.characterInfo[i].vertexIndex = (short)index_X4;
			UIVertex uIVertex = default(UIVertex);
			uIVertex.position = characterInfo[i].vertex_BL.position;
			uIVertex.uv0 = characterInfo[i].vertex_BL.uv;
			uIVertex.uv1 = characterInfo[i].vertex_BL.uv2;
			uIVertex.color = characterInfo[i].vertex_BL.color;
			uIVertex.normal = characterInfo[i].vertex_BL.normal;
			uIVertex.tangent = characterInfo[i].vertex_BL.tangent;
			this.m_uiVertices[0 + index_X4] = uIVertex;
			UIVertex uIVertex2 = default(UIVertex);
			uIVertex2.position = characterInfo[i].vertex_TL.position;
			uIVertex2.uv0 = characterInfo[i].vertex_TL.uv;
			uIVertex2.uv1 = characterInfo[i].vertex_TL.uv2;
			uIVertex2.color = characterInfo[i].vertex_TL.color;
			uIVertex2.normal = characterInfo[i].vertex_TL.normal;
			uIVertex2.tangent = characterInfo[i].vertex_TL.tangent;
			this.m_uiVertices[1 + index_X4] = uIVertex2;
			UIVertex uIVertex3 = default(UIVertex);
			uIVertex3.position = characterInfo[i].vertex_TR.position;
			uIVertex3.uv0 = characterInfo[i].vertex_TR.uv;
			uIVertex3.uv1 = characterInfo[i].vertex_TR.uv2;
			uIVertex3.color = characterInfo[i].vertex_TR.color;
			uIVertex3.normal = characterInfo[i].vertex_TR.normal;
			uIVertex3.tangent = characterInfo[i].vertex_TR.tangent;
			this.m_uiVertices[2 + index_X4] = uIVertex3;
			UIVertex uIVertex4 = default(UIVertex);
			uIVertex4.position = characterInfo[i].vertex_BR.position;
			uIVertex4.uv0 = characterInfo[i].vertex_BR.uv;
			uIVertex4.uv1 = characterInfo[i].vertex_BR.uv2;
			uIVertex4.color = characterInfo[i].vertex_BR.color;
			uIVertex4.normal = characterInfo[i].vertex_BR.normal;
			uIVertex4.tangent = characterInfo[i].vertex_BR.tangent;
			this.m_uiVertices[3 + index_X4] = uIVertex4;
		}

		private void FillSpriteVertexBuffers(int i, int spriteIndex_X4)
		{
			this.m_textInfo.characterInfo[i].vertexIndex = (short)spriteIndex_X4;
			TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
			UIVertex[] uiVertex = this.m_inlineGraphics.uiVertex;
			UIVertex uIVertex = default(UIVertex);
			uIVertex.position = characterInfo[i].vertex_BL.position;
			uIVertex.uv0 = characterInfo[i].vertex_BL.uv;
			uIVertex.color = characterInfo[i].vertex_BL.color;
			uiVertex[spriteIndex_X4] = uIVertex;
			uIVertex.position = characterInfo[i].vertex_TL.position;
			uIVertex.uv0 = characterInfo[i].vertex_TL.uv;
			uIVertex.color = characterInfo[i].vertex_TL.color;
			uiVertex[spriteIndex_X4 + 1] = uIVertex;
			uIVertex.position = characterInfo[i].vertex_TR.position;
			uIVertex.uv0 = characterInfo[i].vertex_TR.uv;
			uIVertex.color = characterInfo[i].vertex_TR.color;
			uiVertex[spriteIndex_X4 + 2] = uIVertex;
			uIVertex.position = characterInfo[i].vertex_BR.position;
			uIVertex.uv0 = characterInfo[i].vertex_BR.uv;
			uIVertex.color = characterInfo[i].vertex_BR.color;
			uiVertex[spriteIndex_X4 + 3] = uIVertex;
			this.m_inlineGraphics.SetUIVertex(uiVertex);
		}

		private void DrawUnderlineMesh(Vector3 start, Vector3 end, ref int index, Color32 underlineColor)
		{
			int num = index + 12;
			if (num > this.m_uiVertices.Length)
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
			this.m_uiVertices[index].position = start + new Vector3(0f, 0f - (height + this.m_padding) * this.m_fontScale, 0f);
			this.m_uiVertices[index + 1].position = start + new Vector3(0f, this.m_padding * this.m_fontScale, 0f);
			this.m_uiVertices[index + 2].position = start + new Vector3(num2, this.m_padding * this.m_fontScale, 0f);
			this.m_uiVertices[index + 3].position = this.m_uiVertices[index].position + new Vector3(num2, 0f, 0f);
			this.m_uiVertices[index + 4].position = this.m_uiVertices[index + 3].position;
			this.m_uiVertices[index + 5].position = this.m_uiVertices[index + 2].position;
			this.m_uiVertices[index + 6].position = end + new Vector3(-num2, this.m_padding * this.m_fontScale, 0f);
			this.m_uiVertices[index + 7].position = end + new Vector3(-num2, -(height + this.m_padding) * this.m_fontScale, 0f);
			this.m_uiVertices[index + 8].position = this.m_uiVertices[index + 7].position;
			this.m_uiVertices[index + 9].position = this.m_uiVertices[index + 6].position;
			this.m_uiVertices[index + 10].position = end + new Vector3(0f, this.m_padding * this.m_fontScale, 0f);
			this.m_uiVertices[index + 11].position = end + new Vector3(0f, -(height + this.m_padding) * this.m_fontScale, 0f);
			Vector2 uv = new Vector2((this.m_cached_Underline_GlyphInfo.x - this.m_padding) / this.m_fontAsset.fontInfo.AtlasWidth, 1f - (this.m_cached_Underline_GlyphInfo.y + this.m_padding + this.m_cached_Underline_GlyphInfo.height) / this.m_fontAsset.fontInfo.AtlasHeight);
			Vector2 uv2 = new Vector2(uv.x, 1f - (this.m_cached_Underline_GlyphInfo.y - this.m_padding) / this.m_fontAsset.fontInfo.AtlasHeight);
			Vector2 uv3 = new Vector2((this.m_cached_Underline_GlyphInfo.x + this.m_padding + this.m_cached_Underline_GlyphInfo.width / 2f) / this.m_fontAsset.fontInfo.AtlasWidth, uv2.y);
			Vector2 uv4 = new Vector2(uv3.x, uv.y);
			Vector2 uv5 = new Vector2((this.m_cached_Underline_GlyphInfo.x + this.m_padding + this.m_cached_Underline_GlyphInfo.width) / this.m_fontAsset.fontInfo.AtlasWidth, uv2.y);
			Vector2 uv6 = new Vector2(uv5.x, uv.y);
			this.m_uiVertices[0 + index].uv0 = uv;
			this.m_uiVertices[1 + index].uv0 = uv2;
			this.m_uiVertices[2 + index].uv0 = uv3;
			this.m_uiVertices[3 + index].uv0 = uv4;
			this.m_uiVertices[4 + index].uv0 = new Vector2(uv3.x - uv3.x * 0.001f, uv.y);
			this.m_uiVertices[5 + index].uv0 = new Vector2(uv3.x - uv3.x * 0.001f, uv2.y);
			this.m_uiVertices[6 + index].uv0 = new Vector2(uv3.x + uv3.x * 0.001f, uv2.y);
			this.m_uiVertices[7 + index].uv0 = new Vector2(uv3.x + uv3.x * 0.001f, uv.y);
			this.m_uiVertices[8 + index].uv0 = uv4;
			this.m_uiVertices[9 + index].uv0 = uv3;
			this.m_uiVertices[10 + index].uv0 = uv5;
			this.m_uiVertices[11 + index].uv0 = uv6;
			float x = (this.m_uiVertices[index + 2].position.x - start.x) / (end.x - start.x);
			float num3 = this.m_fontScale * this.m_rectTransform.lossyScale.z;
			float scale = num3;
			this.m_uiVertices[0 + index].uv1 = this.PackUV(0f, 0f, num3);
			this.m_uiVertices[1 + index].uv1 = this.PackUV(0f, 1f, num3);
			this.m_uiVertices[2 + index].uv1 = this.PackUV(x, 1f, num3);
			this.m_uiVertices[3 + index].uv1 = this.PackUV(x, 0f, num3);
			float x2 = (this.m_uiVertices[index + 4].position.x - start.x) / (end.x - start.x);
			x = (this.m_uiVertices[index + 6].position.x - start.x) / (end.x - start.x);
			this.m_uiVertices[4 + index].uv1 = this.PackUV(x2, 0f, scale);
			this.m_uiVertices[5 + index].uv1 = this.PackUV(x2, 1f, scale);
			this.m_uiVertices[6 + index].uv1 = this.PackUV(x, 1f, scale);
			this.m_uiVertices[7 + index].uv1 = this.PackUV(x, 0f, scale);
			x2 = (this.m_uiVertices[index + 8].position.x - start.x) / (end.x - start.x);
			x = (this.m_uiVertices[index + 6].position.x - start.x) / (end.x - start.x);
			this.m_uiVertices[8 + index].uv1 = this.PackUV(x2, 0f, num3);
			this.m_uiVertices[9 + index].uv1 = this.PackUV(x2, 1f, num3);
			this.m_uiVertices[10 + index].uv1 = this.PackUV(1f, 1f, num3);
			this.m_uiVertices[11 + index].uv1 = this.PackUV(1f, 0f, num3);
			this.m_uiVertices[0 + index].color = underlineColor;
			this.m_uiVertices[1 + index].color = underlineColor;
			this.m_uiVertices[2 + index].color = underlineColor;
			this.m_uiVertices[3 + index].color = underlineColor;
			this.m_uiVertices[4 + index].color = underlineColor;
			this.m_uiVertices[5 + index].color = underlineColor;
			this.m_uiVertices[6 + index].color = underlineColor;
			this.m_uiVertices[7 + index].color = underlineColor;
			this.m_uiVertices[8 + index].color = underlineColor;
			this.m_uiVertices[9 + index].color = underlineColor;
			this.m_uiVertices[10 + index].color = underlineColor;
			this.m_uiVertices[11 + index].color = underlineColor;
			index += 12;
		}

		private void UpdateSDFScale(float prevScale, float newScale)
		{
			for (int i = 0; i < this.m_uiVertices.Length; i++)
			{
				this.m_uiVertices[i].uv1.y = this.m_uiVertices[i].uv1.y / prevScale * newScale;
			}
			this.m_uiRenderer.SetVertices(this.m_uiVertices, this.m_uiVertices.Length);
		}

		private void ResizeMeshBuffers(int size)
		{
			int num = size * 4;
			int num2 = this.m_uiVertices.Length / 4;
			Array.Resize<UIVertex>(ref this.m_uiVertices, num);
			for (int i = num2; i < size; i++)
			{
				int num3 = i * 4;
				this.m_uiVertices[0 + num3].normal = new Vector3(0f, 0f, -1f);
				this.m_uiVertices[1 + num3].normal = new Vector3(0f, 0f, -1f);
				this.m_uiVertices[2 + num3].normal = new Vector3(0f, 0f, -1f);
				this.m_uiVertices[3 + num3].normal = new Vector3(0f, 0f, -1f);
				this.m_uiVertices[0 + num3].tangent = new Vector4(-1f, 0f, 0f, 1f);
				this.m_uiVertices[1 + num3].tangent = new Vector4(-1f, 0f, 0f, 1f);
				this.m_uiVertices[2 + num3].tangent = new Vector4(-1f, 0f, 0f, 1f);
				this.m_uiVertices[3 + num3].tangent = new Vector4(-1f, 0f, 0f, 1f);
			}
		}

		private void UpdateMeshData(TMP_CharacterInfo[] characterInfo, int characterCount, Mesh mesh, Vector3[] vertices, Vector2[] uv0s, Vector2[] uv2s, Color32[] vertexColors, Vector3[] normals, Vector4[] tangents)
		{
			this.m_textInfo.characterInfo = characterInfo;
			this.m_textInfo.characterCount = (int)((short)characterCount);
		}

		private void AdjustLineOffset(int startIndex, int endIndex, float offset)
		{
			Vector3 b = new Vector3(0f, offset, 0f);
			for (int i = startIndex; i <= endIndex; i++)
			{
				if (this.m_textInfo.characterInfo[i].isVisible)
				{
					TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
					int num = i;
					TMP_CharacterInfo[] characterInfo2 = this.m_textInfo.characterInfo;
					int num2 = i;
					characterInfo[num].vertex_BL.position = (characterInfo2[num2].bottomLeft = characterInfo2[num2].bottomLeft - b);
					TMP_CharacterInfo[] characterInfo3 = this.m_textInfo.characterInfo;
					int num3 = i;
					TMP_CharacterInfo[] characterInfo4 = this.m_textInfo.characterInfo;
					int num4 = i;
					characterInfo3[num3].vertex_TL.position = (characterInfo4[num4].topLeft = characterInfo4[num4].topLeft - b);
					TMP_CharacterInfo[] characterInfo5 = this.m_textInfo.characterInfo;
					int num5 = i;
					TMP_CharacterInfo[] characterInfo6 = this.m_textInfo.characterInfo;
					int num6 = i;
					characterInfo5[num5].vertex_TR.position = (characterInfo6[num6].topRight = characterInfo6[num6].topRight - b);
					TMP_CharacterInfo[] characterInfo7 = this.m_textInfo.characterInfo;
					int num7 = i;
					TMP_CharacterInfo[] characterInfo8 = this.m_textInfo.characterInfo;
					int num8 = i;
					characterInfo7[num7].vertex_BR.position = (characterInfo8[num8].bottomRight = characterInfo8[num8].bottomRight - b);
					TMP_CharacterInfo[] characterInfo9 = this.m_textInfo.characterInfo;
					int num9 = i;
					characterInfo9[num9].bottomLine = characterInfo9[num9].bottomLine - b.y;
					TMP_CharacterInfo[] characterInfo10 = this.m_textInfo.characterInfo;
					int num10 = i;
					characterInfo10[num10].topLine = characterInfo10[num10].topLine - b.y;
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
			state.visible_SpriteCount = this.m_visibleSpriteCount;
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
			this.m_textInfo = ((state.textInfo != null) ? state.textInfo : this.m_textInfo);
			this.m_currentFontSize = state.currentFontSize;
			this.m_fontScale = state.fontScale;
			this.m_baselineOffset = state.baselineOffset;
			this.m_style = state.fontStyle;
			this.m_htmlColor = state.vertexColor;
			this.m_colorStackIndex = state.colorStackIndex;
			this.m_characterCount = state.total_CharacterCount + 1;
			this.m_visibleCharacterCount = state.visible_CharacterCount;
			this.m_visibleSpriteCount = state.visible_SpriteCount;
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
			size = ((size > 1024) ? (size + 256) : Mathf.NextPowerOfTwo(size + 1));
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
			case ':':
			case ';':
			case '<':
			case '=':
			case '>':
			case '?':
			case '@':
				IL_67:
				switch (hex)
				{
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
				default:
					return 15;
				}
				break;
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
			}
			goto IL_67;
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
			decimalPointIndex = ((decimalPointIndex > 0) ? decimalPointIndex : (endIndex + 1));
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
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int startIndex2 = 0;
			int decimalPointIndex = 0;
			endIndex = startIndex;
			bool flag = false;
			int num4 = 1;
			int num5 = startIndex;
			while (chars[num5] != 0 && num < this.m_htmlTag.Length && chars[num5] != 60)
			{
				if (chars[num5] == 62)
				{
					flag = true;
					endIndex = num5;
					this.m_htmlTag[num] = '\0';
					break;
				}
				this.m_htmlTag[num] = (char)chars[num5];
				num++;
				if (chars[num5] == 61)
				{
					num4 = 0;
				}
				num2 += chars[num5] * num * num4;
				num3 += chars[num5] * num * (1 - num4);
				int num6 = chars[num5];
				if (num6 != 46)
				{
					if (num6 == 61)
					{
						startIndex2 = num;
					}
				}
				else
				{
					decimalPointIndex = num - 1;
				}
				num5++;
			}
			if (flag)
			{
				if (this.m_htmlTag[0] == '#' && num == 7)
				{
					this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, num);
					this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
					this.m_colorStackIndex++;
					return true;
				}
				if (this.m_htmlTag[0] == '#' && num == 9)
				{
					this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, num);
					this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
					this.m_colorStackIndex++;
					return true;
				}
				int num6 = num2;
				switch (num6)
				{
				case 115:
					this.m_style |= FontStyles.Strikethrough;
					return true;
				case 116:
					IL_1AD:
					switch (num6)
					{
					case 241:
						return true;
					case 242:
						IL_1C6:
						if (num6 == 1019)
						{
							if (this.m_overflowMode == TextOverflowModes.Page)
							{
								this.m_xAdvance = 0f + this.m_indent;
								this.m_lineOffset = 0f;
								this.m_pageNumber++;
								this.m_isNewPage = true;
							}
							return true;
						}
						if (num6 == 1020)
						{
							this.m_currentFontSize /= ((this.m_fontAsset.fontInfo.SubSize > 0f) ? this.m_fontAsset.fontInfo.SubSize : 1f);
							this.m_baselineOffset = 0f;
							this.m_fontScale = this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize;
							return true;
						}
						if (num6 == 98)
						{
							this.m_style |= FontStyles.Bold;
							return true;
						}
						if (num6 == 105)
						{
							this.m_style |= FontStyles.Italic;
							return true;
						}
						if (num6 == 257)
						{
							this.m_style &= (FontStyles)(-3);
							return true;
						}
						if (num6 == 277)
						{
							if ((this.m_fontStyle & FontStyles.Strikethrough) != FontStyles.Strikethrough)
							{
								this.m_style &= (FontStyles)(-65);
							}
							return true;
						}
						if (num6 == 281)
						{
							if ((this.m_fontStyle & FontStyles.Underline) != FontStyles.Underline)
							{
								this.m_style &= (FontStyles)(-5);
							}
							return true;
						}
						if (num6 == 643)
						{
							this.m_currentFontSize *= ((this.m_fontAsset.fontInfo.SubSize > 0f) ? this.m_fontAsset.fontInfo.SubSize : 1f);
							this.m_fontScale = this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize;
							this.m_baselineOffset = this.m_fontAsset.fontInfo.SubscriptOffset * this.m_fontScale;
							return true;
						}
						if (num6 == 679)
						{
							float num7 = this.ConvertToFloat(this.m_htmlTag, startIndex2, num - 1, decimalPointIndex);
							this.m_xAdvance = num7 * this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth;
							return true;
						}
						if (num6 == 685)
						{
							this.m_currentFontSize *= ((this.m_fontAsset.fontInfo.SubSize > 0f) ? this.m_fontAsset.fontInfo.SubSize : 1f);
							this.m_fontScale = this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize;
							this.m_baselineOffset = this.m_fontAsset.fontInfo.SuperscriptOffset * this.m_fontScale;
							return true;
						}
						if (num6 == 1076)
						{
							this.m_currentFontSize /= ((this.m_fontAsset.fontInfo.SubSize > 0f) ? this.m_fontAsset.fontInfo.SubSize : 1f);
							this.m_baselineOffset = 0f;
							this.m_fontScale = this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize;
							return true;
						}
						if (num6 != 1095)
						{
							if (num6 == 1118)
							{
								this.m_fontIndex = (int)this.ConvertToFloat(this.m_htmlTag, startIndex2, num - 1, decimalPointIndex);
								if (this.m_fontAssetArray[this.m_fontIndex] == null)
								{
									this.m_fontAssetArray[this.m_fontIndex] = this.m_fontAsset;
								}
								return true;
							}
							if (num6 == 1531)
							{
								float num7 = this.ConvertToFloat(this.m_htmlTag, startIndex2, num - 1, decimalPointIndex);
								this.m_xAdvance += num7 * this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth;
								return true;
							}
							if (num6 == 1550)
							{
								this.m_htmlColor.a = (byte)(this.HexToInt(this.m_htmlTag[7]) * 16 + this.HexToInt(this.m_htmlTag[8]));
								return true;
							}
							if (num6 == 1585)
							{
								this.m_currentFontSize = this.m_fontSize;
								this.m_isRecalculateScaleRequired = true;
								return true;
							}
							if (num6 != 1590)
							{
								if (num6 == 1613)
								{
									this.m_width = this.ConvertToFloat(this.m_htmlTag, startIndex2, num - 1, decimalPointIndex);
									return true;
								}
								if (num6 != 1659)
								{
									if (num6 == 2030)
									{
										return true;
									}
									if (num6 == 2154)
									{
										this.m_cSpacing = this.ConvertToFloat(this.m_htmlTag, startIndex2, num - 1, decimalPointIndex);
										return true;
									}
									if (num6 == 2160)
									{
										this.m_lineJustification = this.m_textAlignment;
										return true;
									}
									if (num6 == 2164)
									{
										this.m_monoSpacing = this.ConvertToFloat(this.m_htmlTag, startIndex2, num - 1, decimalPointIndex);
										return true;
									}
									if (num6 == 2204)
									{
										this.m_width = 0f;
										return true;
									}
									if (num6 == 2249)
									{
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
									}
									if (num6 == 2275)
									{
										this.m_indent = this.ConvertToFloat(this.m_htmlTag, startIndex2, num - 1, decimalPointIndex) * this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth;
										this.m_xAdvance = this.m_indent;
										return true;
									}
									if (num6 == 2287)
									{
										this.m_spriteIndex = (int)this.ConvertToFloat(this.m_htmlTag, startIndex2, num - 1, decimalPointIndex);
										this.m_isSprite = true;
										return true;
									}
									if (num6 == 2824)
									{
										this.m_cSpacing = 0f;
										return true;
									}
									if (num6 == 2844)
									{
										this.m_monoSpacing = 0f;
										return true;
									}
									if (num6 == 2964)
									{
										this.m_indent = 0f;
										return true;
									}
									if (num6 == 2995)
									{
										this.m_style |= FontStyles.UpperCase;
										return true;
									}
									if (num6 == 3778)
									{
										this.m_style &= (FontStyles)(-17);
										return true;
									}
									if (num6 == 4800)
									{
										this.m_style |= FontStyles.SmallCaps;
										return true;
									}
									if (num6 == 5807)
									{
										this.m_currentFontSize = this.m_fontSize;
										this.m_style &= (FontStyles)(-33);
										this.m_isRecalculateScaleRequired = true;
										return true;
									}
									if (num6 == 6691)
									{
										this.m_lineHeight = this.ConvertToFloat(this.m_htmlTag, startIndex2, num - 1, decimalPointIndex);
										return true;
									}
									if (num6 != 7840)
									{
										return false;
									}
									this.m_lineHeight = 0f;
									return true;
								}
								else
								{
									if (this.m_htmlTag[6] == '#' && num == 13)
									{
										this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, num);
										this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
										this.m_colorStackIndex++;
										return true;
									}
									if (this.m_htmlTag[6] == '#' && num == 15)
									{
										this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, num);
										this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
										this.m_colorStackIndex++;
										return true;
									}
									int num8 = num3;
									if (num8 == 2872)
									{
										this.m_htmlColor = Color.red;
										this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
										this.m_colorStackIndex++;
										return true;
									}
									if (num8 == 3979)
									{
										this.m_htmlColor = Color.blue;
										this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
										this.m_colorStackIndex++;
										return true;
									}
									if (num8 == 4956)
									{
										this.m_htmlColor = Color.black;
										this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
										this.m_colorStackIndex++;
										return true;
									}
									if (num8 == 5128)
									{
										this.m_htmlColor = Color.green;
										this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
										this.m_colorStackIndex++;
										return true;
									}
									if (num8 == 5247)
									{
										this.m_htmlColor = Color.white;
										this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
										this.m_colorStackIndex++;
										return true;
									}
									if (num8 == 6373)
									{
										this.m_htmlColor = new Color32(255, 128, 0, 255);
										this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
										this.m_colorStackIndex++;
										return true;
									}
									if (num8 == 6632)
									{
										this.m_htmlColor = new Color32(160, 32, 240, 255);
										this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
										this.m_colorStackIndex++;
										return true;
									}
									if (num8 != 6722)
									{
										return false;
									}
									this.m_htmlColor = Color.yellow;
									this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
									this.m_colorStackIndex++;
									return true;
								}
							}
							else
							{
								int num8 = num3;
								if (num8 == 4008)
								{
									this.m_lineJustification = TextAlignmentOptions.Left;
									return true;
								}
								if (num8 == 5247)
								{
									this.m_lineJustification = TextAlignmentOptions.Right;
									return true;
								}
								if (num8 == 6496)
								{
									this.m_lineJustification = TextAlignmentOptions.Center;
									return true;
								}
								if (num8 != 10897)
								{
									return false;
								}
								this.m_lineJustification = TextAlignmentOptions.Justified;
								return true;
							}
						}
						else
						{
							int endIndex2 = num - 1;
							float num9;
							if (this.m_htmlTag[5] == '%')
							{
								num9 = this.ConvertToFloat(this.m_htmlTag, startIndex2, endIndex2, decimalPointIndex);
								this.m_currentFontSize = this.m_fontSize * num9 / 100f;
								this.m_isRecalculateScaleRequired = true;
								return true;
							}
							if (this.m_htmlTag[5] == '+')
							{
								num9 = this.ConvertToFloat(this.m_htmlTag, startIndex2, endIndex2, decimalPointIndex);
								this.m_currentFontSize = this.m_fontSize + num9;
								this.m_isRecalculateScaleRequired = true;
								return true;
							}
							if (this.m_htmlTag[5] == '-')
							{
								num9 = this.ConvertToFloat(this.m_htmlTag, startIndex2, endIndex2, decimalPointIndex);
								this.m_currentFontSize = this.m_fontSize + num9;
								this.m_isRecalculateScaleRequired = true;
								return true;
							}
							num9 = this.ConvertToFloat(this.m_htmlTag, startIndex2, endIndex2, decimalPointIndex);
							if (num9 == 73493f)
							{
								return false;
							}
							this.m_currentFontSize = num9;
							this.m_isRecalculateScaleRequired = true;
							return true;
						}
						break;
					case 243:
						if ((this.m_fontStyle & FontStyles.Bold) != FontStyles.Bold)
						{
							this.m_style &= (FontStyles)(-2);
						}
						return true;
					}
					goto IL_1C6;
				case 117:
					this.m_style |= FontStyles.Underline;
					return true;
				}
				goto IL_1AD;
			}
			return false;
		}

		private float GetPreferredWidth()
		{
			TextOverflowModes overflowMode = this.m_overflowMode;
			this.m_overflowMode = TextOverflowModes.Overflow;
			this.m_renderMode = TextRenderFlags.GetPreferredSizes;
			this.GenerateTextMesh();
			this.m_renderMode = TextRenderFlags.Render;
			this.m_overflowMode = overflowMode;
			Debug.Log("GetPreferredWidth() Called. Returning width of " + this.m_preferredWidth);
			return this.m_preferredWidth;
		}

		private float GetPreferredHeight()
		{
			TextOverflowModes overflowMode = this.m_overflowMode;
			this.m_overflowMode = TextOverflowModes.Overflow;
			this.m_renderMode = TextRenderFlags.GetPreferredSizes;
			this.GenerateTextMesh();
			this.m_renderMode = TextRenderFlags.Render;
			this.m_overflowMode = overflowMode;
			Debug.Log("GetPreferredHeight() Called. Returning height of " + this.m_preferredHeight);
			return this.m_preferredHeight;
		}

		public void CalculateLayoutInputHorizontal()
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			this.m_currentAutoSizeMode = this.m_enableAutoSizing;
			this.m_LayoutPhase = TextMeshProUGUI.AutoLayoutPhase.Horizontal;
			this.m_isRebuildingLayout = true;
			this.m_minWidth = 0f;
			this.m_flexibleWidth = 0f;
			this.m_renderMode = TextRenderFlags.GetPreferredSizes;
			if (this.m_enableAutoSizing)
			{
				this.m_fontSize = this.m_fontSizeMax;
			}
			this.m_marginWidth = float.PositiveInfinity;
			this.m_marginHeight = float.PositiveInfinity;
			if (this.isInputParsingRequired || this.m_isTextTruncated)
			{
				this.ParseInputText();
			}
			this.GenerateTextMesh();
			this.m_renderMode = TextRenderFlags.Render;
			this.m_preferredWidth = this.m_renderedWidth;
			this.ComputeMarginSize();
			this.m_isLayoutDirty = true;
		}

		public void CalculateLayoutInputVertical()
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			this.m_LayoutPhase = TextMeshProUGUI.AutoLayoutPhase.Vertical;
			this.m_isRebuildingLayout = true;
			this.m_minHeight = 0f;
			this.m_flexibleHeight = 0f;
			this.m_renderMode = TextRenderFlags.GetPreferredSizes;
			if (this.m_enableAutoSizing)
			{
				this.m_currentAutoSizeMode = true;
				this.m_enableAutoSizing = false;
			}
			this.m_marginHeight = float.PositiveInfinity;
			this.GenerateTextMesh();
			this.m_enableAutoSizing = this.m_currentAutoSizeMode;
			this.m_renderMode = TextRenderFlags.Render;
			this.ComputeMarginSize();
			this.m_preferredHeight = this.m_renderedHeight;
			this.m_isLayoutDirty = true;
			this.m_isCalculateSizeRequired = false;
		}

		public void ParentMaskStateChanged()
		{
			if (this.m_fontAsset == null)
			{
				return;
			}
			this.m_stencilID = MaterialManager.GetStencilID(base.gameObject);
			if (!this.m_isAwake)
			{
				return;
			}
			if (this.m_stencilID == 0)
			{
				if (this.m_maskingMaterial != null)
				{
					MaterialManager.ReleaseStencilMaterial(this.m_maskingMaterial);
					this.m_maskingMaterial = null;
					this.m_sharedMaterial = this.m_baseMaterial;
				}
				else if (this.m_fontMaterial != null)
				{
					this.m_sharedMaterial = MaterialManager.SetStencil(this.m_fontMaterial, 0);
				}
			}
			else
			{
				ShaderUtilities.GetShaderPropertyIDs();
				if (this.m_fontMaterial != null)
				{
					this.m_sharedMaterial = MaterialManager.SetStencil(this.m_fontMaterial, this.m_stencilID);
				}
				else if (this.m_maskingMaterial == null)
				{
					this.m_maskingMaterial = MaterialManager.GetStencilMaterial(this.m_baseMaterial, this.m_stencilID);
					this.m_sharedMaterial = this.m_maskingMaterial;
				}
				else if (this.m_maskingMaterial.GetInt(ShaderUtilities.ID_StencilID) != this.m_stencilID || this.m_isNewBaseMaterial)
				{
					MaterialManager.ReleaseStencilMaterial(this.m_maskingMaterial);
					this.m_maskingMaterial = MaterialManager.GetStencilMaterial(this.m_baseMaterial, this.m_stencilID);
					this.m_sharedMaterial = this.m_maskingMaterial;
				}
				if (this.m_isMaskingEnabled)
				{
					this.EnableMasking();
				}
			}
			this.m_uiRenderer.SetMaterial(this.m_sharedMaterial, null);
			this.m_padding = ShaderUtilities.GetPadding(this.m_sharedMaterial, this.m_enableExtraPadding, this.m_isUsingBold);
			this.m_alignmentPadding = ShaderUtilities.GetFontExtent(this.m_sharedMaterial);
		}

		public void UpdateMeshPadding()
		{
			this.m_padding = ShaderUtilities.GetPadding(new Material[]
			{
				this.m_uiRenderer.GetMaterial()
			}, this.m_enableExtraPadding, this.m_isUsingBold);
			this.havePropertiesChanged = true;
		}

		public void ForceMeshUpdate()
		{
			this.OnPreRenderCanvas();
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
			this.old_text = text;
			this.old_arg1 = 255f;
			this.old_arg2 = 255f;
			int precision = 0;
			int num = 0;
			for (int i = 0; i < text.get_Length(); i++)
			{
				char c = text.get_Chars(i);
				if (c == '{')
				{
					if (text.get_Chars(i + 2) == ':')
					{
						precision = (int)(text.get_Chars(i + 3) - '0');
					}
					switch (text.get_Chars(i + 1))
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
					if (text.get_Chars(i + 2) == ':')
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
			this.m_inputSource = TextMeshProUGUI.TextInputSources.SetText;
			this.isInputParsingRequired = true;
			this.havePropertiesChanged = true;
		}

		public void SetCharArray(char[] charArray)
		{
			if (charArray != null && charArray.Length != 0)
			{
				if (this.m_char_buffer.Length <= charArray.Length)
				{
					int num = Mathf.NextPowerOfTwo(charArray.Length + 1);
					this.m_char_buffer = new int[num];
				}
				int num2 = 0;
				int i = 0;
				while (i < charArray.Length)
				{
					if (charArray[i] != '\\' || i >= charArray.Length - 1)
					{
						goto IL_C4;
					}
					int num3 = (int)charArray[i + 1];
					switch (num3)
					{
					case 114:
						this.m_char_buffer[num2] = 13;
						i++;
						num2++;
						goto IL_D3;
					case 116:
						this.m_char_buffer[num2] = 9;
						i++;
						num2++;
						goto IL_D3;
					}
					if (num3 != 110)
					{
						goto IL_C4;
					}
					this.m_char_buffer[num2] = 10;
					i++;
					num2++;
					IL_D3:
					i++;
					continue;
					IL_C4:
					this.m_char_buffer[num2] = (int)charArray[i];
					num2++;
					goto IL_D3;
				}
				this.m_char_buffer[num2] = 0;
				this.m_inputSource = TextMeshProUGUI.TextInputSources.SetCharArray;
				this.havePropertiesChanged = true;
				this.isInputParsingRequired = true;
			}
		}
	}
}
