using System;
using UnityEngine;

namespace TMPro
{
	[AddComponentMenu("UI/Inline Graphics Manager", 13), ExecuteInEditMode]
	public class InlineGraphicManager : MonoBehaviour
	{
		[SerializeField]
		private SpriteAsset m_spriteAsset;

		[HideInInspector, SerializeField]
		private InlineGraphic m_inlineGraphic;

		[HideInInspector, SerializeField]
		private CanvasRenderer m_inlineGraphicCanvasRenderer;

		private UIVertex[] m_uiVertex;

		private RectTransform m_inlineGraphicRectTransform;

		private TextMeshPro m_TextMeshPro;

		private TextMeshProUGUI m_TextMeshProUI;

		public SpriteAsset spriteAsset
		{
			get
			{
				return this.m_spriteAsset;
			}
			set
			{
				this.LoadSpriteAsset(value);
			}
		}

		public InlineGraphic inlineGraphic
		{
			get
			{
				return this.m_inlineGraphic;
			}
			set
			{
				if (this.m_inlineGraphic != value)
				{
					this.m_inlineGraphic = value;
				}
			}
		}

		public CanvasRenderer canvasRenderer
		{
			get
			{
				return this.m_inlineGraphicCanvasRenderer;
			}
		}

		public UIVertex[] uiVertex
		{
			get
			{
				return this.m_uiVertex;
			}
		}

		private void Awake()
		{
			if (base.gameObject.GetComponent<TextMeshPro>() == null && base.gameObject.GetComponent<TextMeshProUGUI>() == null)
			{
				Debug.LogWarning("The InlineGraphics Component must be attached to a TextMesh Pro Object");
			}
			this.AddInlineGraphicsChild();
		}

		private void OnEnable()
		{
			if (this.m_TextMeshPro == null)
			{
				this.m_TextMeshPro = base.gameObject.GetComponent<TextMeshPro>();
			}
			if (this.m_TextMeshProUI == null)
			{
				this.m_TextMeshProUI = base.gameObject.GetComponent<TextMeshProUGUI>();
			}
			this.m_uiVertex = new UIVertex[4];
			this.LoadSpriteAsset(this.m_spriteAsset);
		}

		private void OnDisable()
		{
		}

		private void OnDestroy()
		{
			if (this.m_inlineGraphic != null)
			{
				Object.DestroyImmediate(this.m_inlineGraphic.gameObject);
			}
		}

		private void LoadSpriteAsset(SpriteAsset spriteAsset)
		{
			if (!(spriteAsset != null))
			{
				spriteAsset = (Resources.Load("Sprites/Default Sprite Atlas") as SpriteAsset);
			}
			this.m_spriteAsset = spriteAsset;
			this.m_inlineGraphic.texture = this.m_spriteAsset.spriteSheet;
			if (this.m_TextMeshPro != null)
			{
				this.m_TextMeshPro.hasChanged = true;
			}
			if (this.m_TextMeshProUI != null)
			{
				this.m_TextMeshProUI.hasChanged = true;
			}
		}

		public void AddInlineGraphicsChild()
		{
			if (this.m_inlineGraphic != null)
			{
				return;
			}
			GameObject gameObject = new GameObject("Inline Graphic");
			this.m_inlineGraphic = gameObject.AddComponent<InlineGraphic>();
			this.m_inlineGraphicRectTransform = gameObject.GetComponent<RectTransform>();
			this.m_inlineGraphicCanvasRenderer = gameObject.GetComponent<CanvasRenderer>();
			this.m_inlineGraphicRectTransform.SetParent(base.transform, false);
			this.m_inlineGraphicRectTransform.localPosition = Vector3.zero;
			this.m_inlineGraphicRectTransform.anchoredPosition3D = Vector3.zero;
			this.m_inlineGraphicRectTransform.sizeDelta = Vector2.zero;
			this.m_inlineGraphicRectTransform.anchorMin = Vector2.zero;
			this.m_inlineGraphicRectTransform.anchorMax = Vector2.one;
			this.m_TextMeshPro = base.gameObject.GetComponent<TextMeshPro>();
			this.m_TextMeshProUI = base.gameObject.GetComponent<TextMeshProUGUI>();
		}

		public void AllocatedVertexBuffers(int size)
		{
			if (this.m_inlineGraphic == null)
			{
				this.AddInlineGraphicsChild();
				this.LoadSpriteAsset(this.m_spriteAsset);
			}
			if (this.m_uiVertex == null)
			{
				this.m_uiVertex = new UIVertex[4];
			}
			int num = size * 4;
			if (num > this.m_uiVertex.Length)
			{
				this.m_uiVertex = new UIVertex[Mathf.NextPowerOfTwo(num)];
			}
		}

		public void UpdatePivot(Vector2 pivot)
		{
			if (this.m_inlineGraphicRectTransform == null)
			{
				this.m_inlineGraphicRectTransform = this.m_inlineGraphic.GetComponent<RectTransform>();
			}
			this.m_inlineGraphicRectTransform.pivot = pivot;
		}

		public void ClearUIVertex()
		{
			if (this.uiVertex != null && this.uiVertex.Length > 0)
			{
				Array.Clear(this.uiVertex, 0, this.uiVertex.Length);
				this.m_inlineGraphicCanvasRenderer.Clear();
			}
		}

		public void DrawSprite(UIVertex[] uiVertices, int spriteCount)
		{
			if (this.m_inlineGraphicCanvasRenderer == null)
			{
				this.m_inlineGraphicCanvasRenderer = this.m_inlineGraphic.GetComponent<CanvasRenderer>();
			}
			this.m_inlineGraphicCanvasRenderer.SetVertices(uiVertices, spriteCount * 4);
			this.m_inlineGraphic.UpdateMaterial();
		}

		public SpriteInfo GetSprite(int index)
		{
			if (this.m_spriteAsset == null)
			{
				Debug.LogWarning("No Sprite Asset is assigned.");
				return null;
			}
			if (this.m_spriteAsset.spriteInfoList == null || index > this.m_spriteAsset.spriteInfoList.get_Count() - 1)
			{
				Debug.LogWarning("Sprite index exceeds the number of sprites in this Sprite Asset.");
				return null;
			}
			return this.m_spriteAsset.spriteInfoList.get_Item(index);
		}

		public void SetUIVertex(UIVertex[] uiVertex)
		{
			this.m_uiVertex = uiVertex;
		}
	}
}
