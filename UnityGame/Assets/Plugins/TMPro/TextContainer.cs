using System;
using UnityEngine;

namespace TMPro
{
	[AddComponentMenu("Layout/Text Container"), ExecuteInEditMode]
	public class TextContainer : MonoBehaviour
	{
		private bool m_hasChanged;

		[SerializeField]
		private Vector2 m_pivot;

		[SerializeField]
		private TextContainerAnchors m_anchorPosition = TextContainerAnchors.Middle;

		[SerializeField]
		private Rect m_rect;

		private bool m_isDefaultWidth;

		private bool m_isDefaultHeight;

		private bool m_isAutoFitting = true;

		private Vector3[] m_corners = new Vector3[4];

		private Vector3[] m_worldCorners = new Vector3[4];

		private Vector3 m_normal;

		[SerializeField]
		private Vector4 m_margins;

		private Transform m_transform;

		private TextMeshPro m_textMeshPro;

		public bool hasChanged
		{
			get
			{
				return this.m_hasChanged;
			}
			set
			{
				this.m_hasChanged = value;
			}
		}

		public Vector2 pivot
		{
			get
			{
				return this.m_pivot;
			}
			set
			{
				if (this.m_pivot != value)
				{
					this.m_pivot = value;
					this.m_anchorPosition = this.GetAnchorPosition(this.m_pivot);
					this.m_hasChanged = true;
					this.OnContainerChanged();
				}
			}
		}

		public TextContainerAnchors anchorPosition
		{
			get
			{
				return this.m_anchorPosition;
			}
			set
			{
				if (this.m_anchorPosition != value)
				{
					this.m_anchorPosition = value;
					this.m_pivot = this.GetPivot(this.m_anchorPosition);
					this.m_hasChanged = true;
					this.OnContainerChanged();
				}
			}
		}

		public Rect rect
		{
			get
			{
				return this.m_rect;
			}
			set
			{
				if (this.m_rect != value)
				{
					this.m_rect = value;
					this.m_hasChanged = true;
					this.OnContainerChanged();
				}
			}
		}

		public Vector2 size
		{
			get
			{
				return new Vector2(this.m_rect.width, this.m_rect.height);
			}
			set
			{
				if (new Vector2(this.m_rect.width, this.m_rect.height) != value)
				{
					this.SetRect(value);
					this.m_hasChanged = true;
					this.m_isDefaultWidth = false;
					this.m_isDefaultHeight = false;
					this.OnContainerChanged();
				}
			}
		}

		public float width
		{
			get
			{
				return this.m_rect.width;
			}
			set
			{
				this.SetRect(new Vector2(value, this.m_rect.height));
				this.m_hasChanged = true;
				this.m_isDefaultWidth = false;
				this.OnContainerChanged();
			}
		}

		public float height
		{
			get
			{
				return this.m_rect.height;
			}
			set
			{
				this.SetRect(new Vector2(this.m_rect.width, value));
				this.m_hasChanged = true;
				this.m_isDefaultHeight = false;
				this.OnContainerChanged();
			}
		}

		public bool isDefaultWidth
		{
			get
			{
				return this.m_isDefaultWidth;
			}
		}

		public bool isDefaultHeight
		{
			get
			{
				return this.m_isDefaultHeight;
			}
		}

		public bool isAutoFitting
		{
			get
			{
				return this.m_isAutoFitting;
			}
			set
			{
				this.m_isAutoFitting = value;
			}
		}

		public Vector3[] corners
		{
			get
			{
				return this.m_corners;
			}
		}

		public Vector3[] worldCorners
		{
			get
			{
				return this.m_worldCorners;
			}
		}

		public Vector3 normal
		{
			get
			{
				return this.m_normal;
			}
		}

		public Vector4 margins
		{
			get
			{
				return this.m_margins;
			}
			set
			{
				if (this.m_margins != value)
				{
					this.m_margins = value;
					this.m_hasChanged = true;
					this.OnContainerChanged();
				}
			}
		}

		private void Awake()
		{
			this.m_transform = (base.GetComponent(typeof(Transform)) as Transform);
			this.m_textMeshPro = (base.GetComponent(typeof(TextMeshPro)) as TextMeshPro);
			if (this.m_rect.width == 0f || this.m_rect.height == 0f)
			{
				if (this.m_textMeshPro != null && this.m_textMeshPro.lineLength != 72f)
				{
					Debug.LogWarning("Converting from using anchor and lineLength properties to Text Container.");
					this.m_isDefaultHeight = true;
					int anchor = (int)this.m_textMeshPro.anchor;
					this.m_anchorPosition = (TextContainerAnchors)anchor;
					this.m_pivot = this.GetPivot(this.m_anchorPosition);
					this.m_rect.width = this.m_textMeshPro.lineLength;
				}
				else
				{
					this.m_isDefaultWidth = true;
					this.m_isDefaultHeight = true;
					this.m_pivot = this.GetPivot(this.m_anchorPosition);
					this.m_rect.width = 0f;
					this.m_rect.height = 0f;
				}
				this.m_margins = new Vector4(0f, 0f, 0f, 0f);
				this.UpdateCorners();
			}
		}

		private void OnEnable()
		{
			if (this.m_transform == null)
			{
				this.m_transform = (base.GetComponent(typeof(Transform)) as Transform);
			}
			this.OnContainerChanged();
		}

		private void OnDisable()
		{
		}

		private void OnContainerChanged()
		{
			this.UpdateCorners();
			this.UpdateWorldCorners();
			if (this.m_transform != null)
			{
				this.m_transform.hasChanged = true;
			}
		}

		private void OnValidate()
		{
			this.m_hasChanged = true;
			this.OnContainerChanged();
		}

		private void SetRect(Vector2 size)
		{
			this.m_rect = new Rect(this.m_rect.x, this.m_rect.y, size.x, size.y);
		}

		private void UpdateCorners()
		{
			this.m_corners[0] = new Vector3(-this.m_pivot.x * this.m_rect.width, -this.m_pivot.y * this.m_rect.height);
			this.m_corners[1] = new Vector3(-this.m_pivot.x * this.m_rect.width, (1f - this.m_pivot.y) * this.m_rect.height);
			this.m_corners[2] = new Vector3((1f - this.m_pivot.x) * this.m_rect.width, (1f - this.m_pivot.y) * this.m_rect.height);
			this.m_corners[3] = new Vector3((1f - this.m_pivot.x) * this.m_rect.width, -this.m_pivot.y * this.m_rect.height);
		}

		private void UpdateWorldCorners()
		{
			if (this.m_transform == null)
			{
				return;
			}
			Vector3 position = this.m_transform.position;
			this.m_worldCorners[0] = position + this.m_transform.TransformDirection(this.m_corners[0]);
			this.m_worldCorners[1] = position + this.m_transform.TransformDirection(this.m_corners[1]);
			this.m_worldCorners[2] = position + this.m_transform.TransformDirection(this.m_corners[2]);
			this.m_worldCorners[3] = position + this.m_transform.TransformDirection(this.m_corners[3]);
			this.m_normal = Vector3.Cross(this.worldCorners[1] - this.worldCorners[0], this.worldCorners[3] - this.worldCorners[0]);
		}

		public Vector3[] GetWorldCorners()
		{
			this.UpdateWorldCorners();
			return this.m_worldCorners;
		}

		private Vector2 GetPivot(TextContainerAnchors anchor)
		{
			Vector2 zero = Vector2.zero;
			switch (anchor)
			{
			case TextContainerAnchors.TopLeft:
				zero = new Vector2(0f, 1f);
				break;
			case TextContainerAnchors.Top:
				zero = new Vector2(0.5f, 1f);
				break;
			case TextContainerAnchors.TopRight:
				zero = new Vector2(1f, 1f);
				break;
			case TextContainerAnchors.Left:
				zero = new Vector2(0f, 0.5f);
				break;
			case TextContainerAnchors.Middle:
				zero = new Vector2(0.5f, 0.5f);
				break;
			case TextContainerAnchors.Right:
				zero = new Vector2(1f, 0.5f);
				break;
			case TextContainerAnchors.BottomLeft:
				zero = new Vector2(0f, 0f);
				break;
			case TextContainerAnchors.Bottom:
				zero = new Vector2(0.5f, 0f);
				break;
			case TextContainerAnchors.BottomRight:
				zero = new Vector2(1f, 0f);
				break;
			}
			return zero;
		}

		private TextContainerAnchors GetAnchorPosition(Vector2 pivot)
		{
			if (pivot == new Vector2(0f, 1f))
			{
				return TextContainerAnchors.TopLeft;
			}
			if (pivot == new Vector2(0.5f, 1f))
			{
				return TextContainerAnchors.Top;
			}
			if (pivot == new Vector2(1f, 1f))
			{
				return TextContainerAnchors.TopRight;
			}
			if (pivot == new Vector2(0f, 0.5f))
			{
				return TextContainerAnchors.Left;
			}
			if (pivot == new Vector2(0.5f, 0.5f))
			{
				return TextContainerAnchors.Middle;
			}
			if (pivot == new Vector2(1f, 0.5f))
			{
				return TextContainerAnchors.Right;
			}
			if (pivot == new Vector2(0f, 0f))
			{
				return TextContainerAnchors.BottomLeft;
			}
			if (pivot == new Vector2(0.5f, 0f))
			{
				return TextContainerAnchors.Bottom;
			}
			if (pivot == new Vector2(1f, 0f))
			{
				return TextContainerAnchors.BottomRight;
			}
			return TextContainerAnchors.Custom;
		}
	}
}
