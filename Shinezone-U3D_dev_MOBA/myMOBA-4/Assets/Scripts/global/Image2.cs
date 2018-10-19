using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

[AddComponentMenu("UI/Image2", 11)]
public class Image2 : Image
{
	[SerializeField]
	protected ImageAlphaTexLayout m_alphaTexLayout;

	public bool WriteTexcoordToNormal;

	private static Vector2[] s_sizeScaling = new Vector2[]
	{
		new Vector2(1f, 1f),
		new Vector2(0.5f, 1f),
		new Vector2(1f, 0.5f)
	};

	private static readonly Vector2[] s_VertScratch = new Vector2[4];

	private static readonly Vector2[] s_UVScratch = new Vector2[4];

	private static readonly Vector2[] s_Xy = new Vector2[4];

	private static readonly Vector2[] s_Uv = new Vector2[4];

	private static List<Component> s_components = new List<Component>();

	private static DictionaryObjectView<Material, Material> s_materialList = new DictionaryObjectView<Material, Material>();

	private static Material s_defaultMaterial;

	private bool m_initialized;

	public ImageAlphaTexLayout alphaTexLayout
	{
		get
		{
			return this.m_alphaTexLayout;
		}
		set
		{
			if (this.m_alphaTexLayout != value)
			{
				this.m_alphaTexLayout = value;
				this.SetMaterialDirty();
			}
		}
	}

	public new static Material defaultMaterial
	{
		get
		{
			if (Image2.s_defaultMaterial == null)
			{
				Image2.s_defaultMaterial = (Resources.Load("Shaders/UI/Default2", typeof(Material)) as Material);
			}
			return Image2.s_defaultMaterial;
		}
	}

	public Material baseMaterial
	{
		get
		{
			if (this.m_Material == null || this.m_Material == Image2.defaultMaterial)
			{
				return (this.alphaTexLayout != ImageAlphaTexLayout.None) ? Image2.defaultMaterial : Graphic.defaultGraphicMaterial;
			}
			if (this.alphaTexLayout == ImageAlphaTexLayout.None)
			{
				return this.m_Material;
			}
			Material material = null;
			if (!Image2.s_materialList.TryGetValue(this.m_Material, out material))
			{
				material = new Material(this.m_Material);
				material.shaderKeywords = this.m_Material.shaderKeywords;
				material.EnableKeyword("_ALPHATEX_ON");
				Image2.s_materialList.Add(this.m_Material, material);
			}
			return material;
		}
	}

	public override Material material
	{
		get
		{
			Material baseMaterial = this.baseMaterial;
			this.UpdateInternalState();
			if (this.m_IncludeForMasking && this.m_MaskMaterial == null)
			{
				this.m_MaskMaterial = StencilMaterial.Add(baseMaterial, (1 << this.m_StencilValue) - 1);
				if (this.m_MaskMaterial != null)
				{
					this.m_MaskMaterial.shaderKeywords = baseMaterial.shaderKeywords;
					return this.m_MaskMaterial;
				}
			}
			return baseMaterial;
		}
		set
		{
			base.material = value;
		}
	}

	public override float preferredWidth
	{
		get
		{
			float num = base.preferredWidth;
			if (this.alphaTexLayout == ImageAlphaTexLayout.Horizonatal)
			{
				num *= 0.5f;
			}
			return num;
		}
	}

	public override float preferredHeight
	{
		get
		{
			float num = base.preferredHeight;
			if (this.alphaTexLayout == ImageAlphaTexLayout.Vertical)
			{
				num *= 0.5f;
			}
			return num;
		}
	}

	public void SetMaterialVector(string name, Vector4 factor)
	{
		if (this.m_Material == null)
		{
			return;
		}
		if (!this.m_Material.name.Contains("(Clone)"))
		{
			Material material = new Material(this.m_Material);
			material.name = this.m_Material.name + "(Clone)";
			material.CopyPropertiesFromMaterial(this.m_Material);
			material.shaderKeywords = this.m_Material.shaderKeywords;
			material.SetVector(name, factor);
			this.material = material;
		}
		else
		{
			this.m_Material.SetVector(name, factor);
			this.SetMaterialDirty();
		}
	}

	private int GetStencilForGraphic()
	{
		int num = 0;
		Transform parent = base.transform.parent;
		Image2.s_components.Clear();
		while (parent != null)
		{
			parent.GetComponents(typeof(IMask), Image2.s_components);
			for (int i = 0; i < Image2.s_components.Count; i++)
			{
				IMask mask = Image2.s_components[i] as IMask;
				if (mask != null && mask.MaskEnabled())
				{
					num++;
					num = Mathf.Clamp(num, 0, 8);
					break;
				}
			}
			parent = parent.parent;
		}
		Image2.s_components.Clear();
		return num;
	}

	private void UpdateInternalState()
	{
		if (this.m_ShouldRecalculate)
		{
			this.m_StencilValue = this.GetStencilForGraphic();
			Transform parent = base.transform.parent;
			this.m_IncludeForMasking = false;
			Image2.s_components.Clear();
			while (base.maskable && parent != null)
			{
				parent.GetComponents(typeof(IMask), Image2.s_components);
				if (Image2.s_components.Count > 0)
				{
					this.m_IncludeForMasking = true;
					break;
				}
				parent = parent.parent;
			}
			this.m_ShouldRecalculate = false;
			Image2.s_components.Clear();
		}
	}

	private Vector4 GetDrawingDimensions(bool shouldPreserveAspect, Vector2 sizeScaling)
	{
		Vector4 vector = (base.overrideSprite == null) ? Vector4.zero : DataUtility.GetPadding(base.overrideSprite);
		Vector2 vector2 = (base.overrideSprite == null) ? Vector2.zero : new Vector2(base.overrideSprite.rect.width * sizeScaling.x, base.overrideSprite.rect.height * sizeScaling.y);
		Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
		int num = Mathf.RoundToInt(vector2.x);
		int num2 = Mathf.RoundToInt(vector2.y);
		Vector4 result = new Vector4(vector.x / (float)num, vector.y / (float)num2, ((float)num - vector.z) / (float)num, ((float)num2 - vector.w) / (float)num2);
		if (shouldPreserveAspect && (double)vector2.sqrMagnitude > 0.0)
		{
			float num3 = vector2.x / vector2.y;
			float num4 = pixelAdjustedRect.width / pixelAdjustedRect.height;
			if ((double)num3 > (double)num4)
			{
				float height = pixelAdjustedRect.height;
				pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / num3);
				pixelAdjustedRect.y += (height - pixelAdjustedRect.height) * base.rectTransform.pivot.y;
			}
			else
			{
				float width = pixelAdjustedRect.width;
				pixelAdjustedRect.width = pixelAdjustedRect.height * num3;
				pixelAdjustedRect.x += (width - pixelAdjustedRect.width) * base.rectTransform.pivot.x;
			}
		}
		result = new Vector4(pixelAdjustedRect.x + pixelAdjustedRect.width * result.x, pixelAdjustedRect.y + pixelAdjustedRect.height * result.y, pixelAdjustedRect.x + pixelAdjustedRect.width * result.z, pixelAdjustedRect.y + pixelAdjustedRect.height * result.w);
		return result;
	}

	private void GenerateSimpleSprite(List<UIVertex> vbo, bool preserveAspect)
	{
		Vector2 sizeScaling = Image2.s_sizeScaling[(int)this.alphaTexLayout];
		UIVertex simpleVert = UIVertex.simpleVert;
		simpleVert.color = base.color;
		Vector4 drawingDimensions = this.GetDrawingDimensions(preserveAspect, sizeScaling);
		Vector4 vector = (base.overrideSprite != null) ? DataUtility.GetOuterUV(base.overrideSprite) : Vector4.zero;
		float y = vector.y;
		float w = vector.w;
		float y2 = (this.alphaTexLayout != ImageAlphaTexLayout.Vertical) ? w : ((y + w) * 0.5f);
		float y3 = (this.alphaTexLayout != ImageAlphaTexLayout.Vertical) ? y : ((y + w) * 0.5f);
		float x = vector.x;
		float z = vector.z;
		float x2 = (this.alphaTexLayout != ImageAlphaTexLayout.Horizonatal) ? z : ((x + z) * 0.5f);
		float x3 = (this.alphaTexLayout != ImageAlphaTexLayout.Horizonatal) ? x : ((x + z) * 0.5f);
		simpleVert.position = new Vector3(drawingDimensions.x, drawingDimensions.y);
		simpleVert.uv0 = new Vector2(x, y);
		simpleVert.uv1 = new Vector2(x3, y3);
		vbo.Add(simpleVert);
		simpleVert.position = new Vector3(drawingDimensions.x, drawingDimensions.w);
		simpleVert.uv0 = new Vector2(x, y2);
		simpleVert.uv1 = new Vector2(x3, w);
		vbo.Add(simpleVert);
		simpleVert.position = new Vector3(drawingDimensions.z, drawingDimensions.w);
		simpleVert.uv0 = new Vector2(x2, y2);
		simpleVert.uv1 = new Vector2(z, w);
		vbo.Add(simpleVert);
		simpleVert.position = new Vector3(drawingDimensions.z, drawingDimensions.y);
		simpleVert.uv0 = new Vector2(x2, y);
		simpleVert.uv1 = new Vector2(z, y3);
		vbo.Add(simpleVert);
	}

	private void GenerateSimpleSprite_Normal(List<UIVertex> vbo, bool preserveAspect)
	{
		Vector2 sizeScaling = Image2.s_sizeScaling[(int)this.alphaTexLayout];
		UIVertex simpleVert = UIVertex.simpleVert;
		simpleVert.color = base.color;
		Vector4 drawingDimensions = this.GetDrawingDimensions(preserveAspect, sizeScaling);
		Vector4 vector = (base.overrideSprite != null) ? DataUtility.GetOuterUV(base.overrideSprite) : Vector4.zero;
		float y = vector.y;
		float w = vector.w;
		float y2 = (this.alphaTexLayout != ImageAlphaTexLayout.Vertical) ? w : ((y + w) * 0.5f);
		float y3 = (this.alphaTexLayout != ImageAlphaTexLayout.Vertical) ? y : ((y + w) * 0.5f);
		float x = vector.x;
		float z = vector.z;
		float x2 = (this.alphaTexLayout != ImageAlphaTexLayout.Horizonatal) ? z : ((x + z) * 0.5f);
		float x3 = (this.alphaTexLayout != ImageAlphaTexLayout.Horizonatal) ? x : ((x + z) * 0.5f);
		simpleVert.position = new Vector3(drawingDimensions.x, drawingDimensions.y);
		simpleVert.uv0 = new Vector2(x, y);
		simpleVert.uv1 = new Vector2(x3, y3);
		simpleVert.normal = new Vector3(-1f, -1f, 0f);
		vbo.Add(simpleVert);
		simpleVert.position = new Vector3(drawingDimensions.x, drawingDimensions.w);
		simpleVert.uv0 = new Vector2(x, y2);
		simpleVert.uv1 = new Vector2(x3, w);
		simpleVert.normal = new Vector3(-1f, 1f, 0f);
		vbo.Add(simpleVert);
		simpleVert.position = new Vector3(drawingDimensions.z, drawingDimensions.w);
		simpleVert.uv0 = new Vector2(x2, y2);
		simpleVert.uv1 = new Vector2(z, w);
		simpleVert.normal = new Vector3(1f, 1f, 0f);
		vbo.Add(simpleVert);
		simpleVert.position = new Vector3(drawingDimensions.z, drawingDimensions.y);
		simpleVert.uv0 = new Vector2(x2, y);
		simpleVert.uv1 = new Vector2(z, y3);
		simpleVert.normal = new Vector3(1f, -1f, 0f);
		vbo.Add(simpleVert);
	}

	private Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
	{
		for (int i = 0; i <= 1; i++)
		{
			float num = border[i] + border[i + 2];
			if ((double)rect.size[i] < (double)num && (double)num != 0.0)
			{
				float num2 = rect.size[i] / num;
				int index;
				int expr_5D = index = i;
				float num3 = border[index];
				border[expr_5D] = num3 * num2;
				int expr_7C = index = i + 2;
				num3 = border[index];
				border[expr_7C] = num3 * num2;
			}
		}
		return border;
	}

	private void AddQuad(List<UIVertex> vbo, UIVertex v, Vector2 posMin, Vector2 posMax, Vector2 uvMin, Vector2 uvMax)
	{
		v.position = new Vector3(posMin.x, posMin.y, 0f);
		v.uv0 = new Vector2(uvMin.x, uvMin.y);
		vbo.Add(v);
		v.position = new Vector3(posMin.x, posMax.y, 0f);
		v.uv0 = new Vector2(uvMin.x, uvMax.y);
		vbo.Add(v);
		v.position = new Vector3(posMax.x, posMax.y, 0f);
		v.uv0 = new Vector2(uvMax.x, uvMax.y);
		vbo.Add(v);
		v.position = new Vector3(posMax.x, posMin.y, 0f);
		v.uv0 = new Vector2(uvMax.x, uvMin.y);
		vbo.Add(v);
	}

	private void AddQuad(List<UIVertex> vbo, UIVertex v, Vector2 posMin, Vector2 posMax, Vector2 uvMin, Vector2 uvMax, Vector2 offset)
	{
		v.position = new Vector3(posMin.x, posMin.y, 0f);
		v.uv0 = new Vector2(uvMin.x, uvMin.y);
		v.uv1 = v.uv0 + offset;
		vbo.Add(v);
		v.position = new Vector3(posMin.x, posMax.y, 0f);
		v.uv0 = new Vector2(uvMin.x, uvMax.y);
		v.uv1 = v.uv0 + offset;
		vbo.Add(v);
		v.position = new Vector3(posMax.x, posMax.y, 0f);
		v.uv0 = new Vector2(uvMax.x, uvMax.y);
		v.uv1 = v.uv0 + offset;
		vbo.Add(v);
		v.position = new Vector3(posMax.x, posMin.y, 0f);
		v.uv0 = new Vector2(uvMax.x, uvMin.y);
		v.uv1 = v.uv0 + offset;
		vbo.Add(v);
	}

	private static Vector4 GetOuterUV(Sprite sprite, ImageAlphaTexLayout layout, out Vector2 offset)
	{
		Vector4 outerUV = DataUtility.GetOuterUV(sprite);
		offset = Vector2.zero;
		if (layout != ImageAlphaTexLayout.Horizonatal)
		{
			if (layout == ImageAlphaTexLayout.Vertical)
			{
				offset.y = (outerUV.w - outerUV.y) * 0.5f;
				outerUV.w = (outerUV.w + outerUV.y) * 0.5f;
			}
		}
		else
		{
			offset.x = (outerUV.z - outerUV.x) * 0.5f;
			outerUV.z = (outerUV.z + outerUV.x) * 0.5f;
		}
		return outerUV;
	}

	private static Vector4 GetInnerUV(Sprite sprite, Vector2 sizeScaling)
	{
		Texture texture = sprite.texture;
		if (texture == null)
		{
			return new Vector4(0f, 0f, sizeScaling.x, sizeScaling.y);
		}
		Rect textureRect = sprite.textureRect;
		textureRect.width *= sizeScaling.x;
		textureRect.height *= sizeScaling.y;
		float num = 1f / (float)texture.width;
		float num2 = 1f / (float)texture.height;
		Vector4 padding = DataUtility.GetPadding(sprite);
		Vector4 border = sprite.border;
		float num3 = textureRect.x + padding.x;
		float num4 = textureRect.y + padding.y;
		Vector4 result = default(Vector4);
		result.x = num3 + border.x;
		result.y = num4 + border.y;
		result.z = textureRect.x + textureRect.width - border.z;
		result.w = textureRect.y + textureRect.height - border.w;
		result.x *= num;
		result.y *= num2;
		result.z *= num;
		result.w *= num2;
		return result;
	}

	private static bool RadialCut(Vector2[] xy, Vector2[] uv, float fill, bool invert, int corner)
	{
		if ((double)fill < 0.001)
		{
			return false;
		}
		if ((corner & 1) == 1)
		{
			invert = !invert;
		}
		if (!invert && (double)fill > 0.999000012874603)
		{
			return true;
		}
		float num = Mathf.Clamp01(fill);
		if (invert)
		{
			num = 1f - num;
		}
		float f = num * 1.570796f;
		float cos = Mathf.Cos(f);
		float sin = Mathf.Sin(f);
		Image2.RadialCut(xy, cos, sin, invert, corner);
		Image2.RadialCut(uv, cos, sin, invert, corner);
		return true;
	}

	private static void RadialCut(Vector2[] xy, float cos, float sin, bool invert, int corner)
	{
		int num = (corner + 1) % 4;
		int num2 = (corner + 2) % 4;
		int num3 = (corner + 3) % 4;
		if ((corner & 1) == 1)
		{
			if ((double)sin > (double)cos)
			{
				cos /= sin;
				sin = 1f;
				if (invert)
				{
					xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
					xy[num2].x = xy[num].x;
				}
			}
			else if ((double)cos > (double)sin)
			{
				sin /= cos;
				cos = 1f;
				if (!invert)
				{
					xy[num2].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
					xy[num3].y = xy[num2].y;
				}
			}
			else
			{
				cos = 1f;
				sin = 1f;
			}
			if (!invert)
			{
				xy[num3].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
			}
			else
			{
				xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
			}
		}
		else
		{
			if ((double)cos > (double)sin)
			{
				sin /= cos;
				cos = 1f;
				if (!invert)
				{
					xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
					xy[num2].y = xy[num].y;
				}
			}
			else if ((double)sin > (double)cos)
			{
				cos /= sin;
				sin = 1f;
				if (invert)
				{
					xy[num2].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
					xy[num3].x = xy[num2].x;
				}
			}
			else
			{
				cos = 1f;
				sin = 1f;
			}
			if (invert)
			{
				xy[num3].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
			}
			else
			{
				xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
			}
		}
	}

	private void GenerateSlicedSprite(List<UIVertex> vbo)
	{
		if (!base.hasBorder)
		{
			this.GenerateSimpleSprite(vbo, false);
		}
		else
		{
			Vector2 zero = Vector2.zero;
			Vector4 vector;
			Vector4 vector2;
			Vector4 a;
			Vector4 a2;
			if (base.overrideSprite != null)
			{
				vector = Image2.GetOuterUV(base.overrideSprite, this.alphaTexLayout, out zero);
				vector2 = Image2.GetInnerUV(base.overrideSprite, Image2.s_sizeScaling[(int)this.alphaTexLayout]);
				a = DataUtility.GetPadding(base.overrideSprite);
				a2 = base.overrideSprite.border;
			}
			else
			{
				vector = Vector4.zero;
				vector2 = Vector4.zero;
				a = Vector4.zero;
				a2 = Vector4.zero;
			}
			Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
			Vector4 adjustedBorders = this.GetAdjustedBorders(a2 / base.pixelsPerUnit, pixelAdjustedRect);
			Vector4 vector3 = a / base.pixelsPerUnit;
			Image2.s_VertScratch[0] = new Vector2(vector3.x, vector3.y);
			Image2.s_VertScratch[3] = new Vector2(pixelAdjustedRect.width - vector3.z, pixelAdjustedRect.height - vector3.w);
			Image2.s_VertScratch[1].x = adjustedBorders.x;
			Image2.s_VertScratch[1].y = adjustedBorders.y;
			Image2.s_VertScratch[2].x = pixelAdjustedRect.width - adjustedBorders.z;
			Image2.s_VertScratch[2].y = pixelAdjustedRect.height - adjustedBorders.w;
			for (int i = 0; i < 4; i++)
			{
				Vector2[] expr_19C_cp_0 = Image2.s_VertScratch;
				int expr_19C_cp_1 = i;
				expr_19C_cp_0[expr_19C_cp_1].x = expr_19C_cp_0[expr_19C_cp_1].x + pixelAdjustedRect.x;
				Vector2[] expr_1BB_cp_0 = Image2.s_VertScratch;
				int expr_1BB_cp_1 = i;
				expr_1BB_cp_0[expr_1BB_cp_1].y = expr_1BB_cp_0[expr_1BB_cp_1].y + pixelAdjustedRect.y;
			}
			Image2.s_UVScratch[0] = new Vector2(vector.x, vector.y);
			Image2.s_UVScratch[1] = new Vector2(vector2.x, vector2.y);
			Image2.s_UVScratch[2] = new Vector2(vector2.z, vector2.w);
			Image2.s_UVScratch[3] = new Vector2(vector.z, vector.w);
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.color = base.color;
			for (int j = 0; j < 3; j++)
			{
				int num = j + 1;
				for (int k = 0; k < 3; k++)
				{
					if (base.fillCenter || j != 1 || k != 1)
					{
						int num2 = k + 1;
						this.AddQuad(vbo, simpleVert, new Vector2(Image2.s_VertScratch[j].x, Image2.s_VertScratch[k].y), new Vector2(Image2.s_VertScratch[num].x, Image2.s_VertScratch[num2].y), new Vector2(Image2.s_UVScratch[j].x, Image2.s_UVScratch[k].y), new Vector2(Image2.s_UVScratch[num].x, Image2.s_UVScratch[num2].y), zero);
					}
				}
			}
		}
	}

	private void GenerateFilledSprite(List<UIVertex> vbo, bool preserveAspect)
	{
		if ((double)base.fillAmount < 0.001)
		{
			return;
		}
		Vector4 drawingDimensions = this.GetDrawingDimensions(preserveAspect, Image2.s_sizeScaling[(int)this.alphaTexLayout]);
		Vector2 zero = Vector2.zero;
		Vector4 vector = Vector4.zero;
		if (base.overrideSprite != null)
		{
			vector = Image2.GetOuterUV(base.overrideSprite, this.alphaTexLayout, out zero);
		}
		UIVertex simpleVert = UIVertex.simpleVert;
		simpleVert.color = base.color;
		float num = vector.x;
		float num2 = vector.y;
		float num3 = vector.z;
		float num4 = vector.w;
		if (base.fillMethod == Image.FillMethod.Horizontal || base.fillMethod == Image.FillMethod.Vertical)
		{
			if (base.fillMethod == Image.FillMethod.Horizontal)
			{
				float num5 = (num3 - num) * base.fillAmount;
				if (base.fillOrigin == 1)
				{
					drawingDimensions.x = drawingDimensions.z - (drawingDimensions.z - drawingDimensions.x) * base.fillAmount;
					num = num3 - num5;
				}
				else
				{
					drawingDimensions.z = drawingDimensions.x + (drawingDimensions.z - drawingDimensions.x) * base.fillAmount;
					num3 = num + num5;
				}
			}
			else if (base.fillMethod == Image.FillMethod.Vertical)
			{
				float num6 = (num4 - num2) * base.fillAmount;
				if (base.fillOrigin == 1)
				{
					drawingDimensions.y = drawingDimensions.w - (drawingDimensions.w - drawingDimensions.y) * base.fillAmount;
					num2 = num4 - num6;
				}
				else
				{
					drawingDimensions.w = drawingDimensions.y + (drawingDimensions.w - drawingDimensions.y) * base.fillAmount;
					num4 = num2 + num6;
				}
			}
		}
		Image2.s_Xy[0] = new Vector2(drawingDimensions.x, drawingDimensions.y);
		Image2.s_Xy[1] = new Vector2(drawingDimensions.x, drawingDimensions.w);
		Image2.s_Xy[2] = new Vector2(drawingDimensions.z, drawingDimensions.w);
		Image2.s_Xy[3] = new Vector2(drawingDimensions.z, drawingDimensions.y);
		Image2.s_Uv[0] = new Vector2(num, num2);
		Image2.s_Uv[1] = new Vector2(num, num4);
		Image2.s_Uv[2] = new Vector2(num3, num4);
		Image2.s_Uv[3] = new Vector2(num3, num2);
		if ((double)base.fillAmount < 1.0)
		{
			if (base.fillMethod == Image.FillMethod.Radial90)
			{
				if (!Image2.RadialCut(Image2.s_Xy, Image2.s_Uv, base.fillAmount, base.fillClockwise, base.fillOrigin))
				{
					return;
				}
				for (int i = 0; i < 4; i++)
				{
					simpleVert.position = Image2.s_Xy[i];
					simpleVert.uv0 = Image2.s_Uv[i];
					simpleVert.uv1 = simpleVert.uv0 + zero;
					vbo.Add(simpleVert);
				}
				return;
			}
			else
			{
				if (base.fillMethod == Image.FillMethod.Radial180)
				{
					for (int j = 0; j < 2; j++)
					{
						int num7 = (base.fillOrigin > 1) ? 1 : 0;
						float t;
						float t2;
						float t3;
						float t4;
						if (base.fillOrigin == 0 || base.fillOrigin == 2)
						{
							t = 0f;
							t2 = 1f;
							if (j == num7)
							{
								t3 = 0f;
								t4 = 0.5f;
							}
							else
							{
								t3 = 0.5f;
								t4 = 1f;
							}
						}
						else
						{
							t3 = 0f;
							t4 = 1f;
							if (j == num7)
							{
								t = 0.5f;
								t2 = 1f;
							}
							else
							{
								t = 0f;
								t2 = 0.5f;
							}
						}
						Image2.s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t3);
						Image2.s_Xy[1].x = Image2.s_Xy[0].x;
						Image2.s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t4);
						Image2.s_Xy[3].x = Image2.s_Xy[2].x;
						Image2.s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t);
						Image2.s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t2);
						Image2.s_Xy[2].y = Image2.s_Xy[1].y;
						Image2.s_Xy[3].y = Image2.s_Xy[0].y;
						Image2.s_Uv[0].x = Mathf.Lerp(num, num3, t3);
						Image2.s_Uv[1].x = Image2.s_Uv[0].x;
						Image2.s_Uv[2].x = Mathf.Lerp(num, num3, t4);
						Image2.s_Uv[3].x = Image2.s_Uv[2].x;
						Image2.s_Uv[0].y = Mathf.Lerp(num2, num4, t);
						Image2.s_Uv[1].y = Mathf.Lerp(num2, num4, t2);
						Image2.s_Uv[2].y = Image2.s_Uv[1].y;
						Image2.s_Uv[3].y = Image2.s_Uv[0].y;
						float value = base.fillClockwise ? (base.fillAmount * 2f - (float)j) : (base.fillAmount * 2f - (float)(1 - j));
						if (Image2.RadialCut(Image2.s_Xy, Image2.s_Uv, Mathf.Clamp01(value), base.fillClockwise, (j + base.fillOrigin + 3) % 4))
						{
							for (int k = 0; k < 4; k++)
							{
								simpleVert.position = Image2.s_Xy[k];
								simpleVert.uv0 = Image2.s_Uv[k];
								simpleVert.uv1 = simpleVert.uv0 + zero;
								vbo.Add(simpleVert);
							}
						}
					}
					return;
				}
				if (base.fillMethod == Image.FillMethod.Radial360)
				{
					for (int l = 0; l < 4; l++)
					{
						float t5;
						float t6;
						if (l < 2)
						{
							t5 = 0f;
							t6 = 0.5f;
						}
						else
						{
							t5 = 0.5f;
							t6 = 1f;
						}
						float t7;
						float t8;
						if (l == 0 || l == 3)
						{
							t7 = 0f;
							t8 = 0.5f;
						}
						else
						{
							t7 = 0.5f;
							t8 = 1f;
						}
						Image2.s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t5);
						Image2.s_Xy[1].x = Image2.s_Xy[0].x;
						Image2.s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t6);
						Image2.s_Xy[3].x = Image2.s_Xy[2].x;
						Image2.s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t7);
						Image2.s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t8);
						Image2.s_Xy[2].y = Image2.s_Xy[1].y;
						Image2.s_Xy[3].y = Image2.s_Xy[0].y;
						Image2.s_Uv[0].x = Mathf.Lerp(num, num3, t5);
						Image2.s_Uv[1].x = Image2.s_Uv[0].x;
						Image2.s_Uv[2].x = Mathf.Lerp(num, num3, t6);
						Image2.s_Uv[3].x = Image2.s_Uv[2].x;
						Image2.s_Uv[0].y = Mathf.Lerp(num2, num4, t7);
						Image2.s_Uv[1].y = Mathf.Lerp(num2, num4, t8);
						Image2.s_Uv[2].y = Image2.s_Uv[1].y;
						Image2.s_Uv[3].y = Image2.s_Uv[0].y;
						float value2 = base.fillClockwise ? (base.fillAmount * 4f - (float)((l + base.fillOrigin) % 4)) : (base.fillAmount * 4f - (float)(3 - (l + base.fillOrigin) % 4));
						if (Image2.RadialCut(Image2.s_Xy, Image2.s_Uv, Mathf.Clamp01(value2), base.fillClockwise, (l + 2) % 4))
						{
							for (int m = 0; m < 4; m++)
							{
								simpleVert.position = Image2.s_Xy[m];
								simpleVert.uv0 = Image2.s_Uv[m];
								simpleVert.uv1 = simpleVert.uv0 + zero;
								vbo.Add(simpleVert);
							}
						}
					}
					return;
				}
			}
		}
		for (int n = 0; n < 4; n++)
		{
			simpleVert.position = Image2.s_Xy[n];
			simpleVert.uv0 = Image2.s_Uv[n];
			simpleVert.uv1 = simpleVert.uv0 + zero;
			vbo.Add(simpleVert);
		}
	}

	private void GenerateTiledSprite(List<UIVertex> vbo)
	{
		Vector2 zero;
		Vector4 vector;
		Vector4 vector2;
		Vector4 a;
		Vector2 vector3;
		if (base.overrideSprite != null)
		{
			Vector2 sizeScaling = Image2.s_sizeScaling[(int)this.alphaTexLayout];
			vector = Image2.GetOuterUV(base.overrideSprite, this.alphaTexLayout, out zero);
			vector2 = Image2.GetInnerUV(base.overrideSprite, sizeScaling);
			a = base.overrideSprite.border;
			vector3 = base.overrideSprite.rect.size;
			vector3.x *= sizeScaling.x;
			vector3.y *= sizeScaling.y;
		}
		else
		{
			vector = Vector4.zero;
			vector2 = Vector4.zero;
			a = Vector4.zero;
			vector3 = Vector2.one * 100f;
			zero = Vector2.zero;
		}
		Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
		float num = (vector3.x - a.x - a.z) / base.pixelsPerUnit;
		float num2 = (vector3.y - a.y - a.w) / base.pixelsPerUnit;
		a = this.GetAdjustedBorders(a / base.pixelsPerUnit, pixelAdjustedRect);
		Vector2 uvMin = new Vector2(vector2.x, vector2.y);
		Vector2 vector4 = new Vector2(vector2.z, vector2.w);
		UIVertex simpleVert = UIVertex.simpleVert;
		simpleVert.color = base.color;
		float x = a.x;
		float num3 = pixelAdjustedRect.width - a.z;
		float y = a.y;
		float num4 = pixelAdjustedRect.height - a.w;
		if ((double)num3 - (double)x > (double)num * 100.0 || (double)num4 - (double)y > (double)num2 * 100.0)
		{
			num = (float)(((double)num3 - (double)x) / 100.0);
			num2 = (float)(((double)num4 - (double)y) / 100.0);
		}
		Vector2 uvMax = vector4;
		if (base.fillCenter)
		{
			float num5 = y;
			while ((double)num5 < (double)num4)
			{
				float num6 = num5 + num2;
				if ((double)num6 > (double)num4)
				{
					uvMax.y = uvMin.y + (float)(((double)vector4.y - (double)uvMin.y) * ((double)num4 - (double)num5) / ((double)num6 - (double)num5));
					num6 = num4;
				}
				uvMax.x = vector4.x;
				float num7 = x;
				while ((double)num7 < (double)num3)
				{
					float num8 = num7 + num;
					if ((double)num8 > (double)num3)
					{
						uvMax.x = uvMin.x + (float)(((double)vector4.x - (double)uvMin.x) * ((double)num3 - (double)num7) / ((double)num8 - (double)num7));
						num8 = num3;
					}
					this.AddQuad(vbo, simpleVert, new Vector2(num7, num5) + pixelAdjustedRect.position, new Vector2(num8, num6) + pixelAdjustedRect.position, uvMin, uvMax, zero);
					num7 += num;
				}
				num5 += num2;
			}
		}
		if (!base.hasBorder)
		{
			return;
		}
		Vector2 vector5 = vector4;
		float num9 = y;
		while ((double)num9 < (double)num4)
		{
			float num10 = num9 + num2;
			if ((double)num10 > (double)num4)
			{
				vector5.y = uvMin.y + (float)(((double)vector4.y - (double)uvMin.y) * ((double)num4 - (double)num9) / ((double)num10 - (double)num9));
				num10 = num4;
			}
			this.AddQuad(vbo, simpleVert, new Vector2(0f, num9) + pixelAdjustedRect.position, new Vector2(x, num10) + pixelAdjustedRect.position, new Vector2(vector.x, uvMin.y), new Vector2(uvMin.x, vector5.y), zero);
			this.AddQuad(vbo, simpleVert, new Vector2(num3, num9) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, num10) + pixelAdjustedRect.position, new Vector2(vector4.x, uvMin.y), new Vector2(vector.z, vector5.y), zero);
			num9 += num2;
		}
		vector5 = vector4;
		float num11 = x;
		while ((double)num11 < (double)num3)
		{
			float num12 = num11 + num;
			if ((double)num12 > (double)num3)
			{
				vector5.x = uvMin.x + (float)(((double)vector4.x - (double)uvMin.x) * ((double)num3 - (double)num11) / ((double)num12 - (double)num11));
				num12 = num3;
			}
			this.AddQuad(vbo, simpleVert, new Vector2(num11, 0f) + pixelAdjustedRect.position, new Vector2(num12, y) + pixelAdjustedRect.position, new Vector2(uvMin.x, vector.y), new Vector2(vector5.x, uvMin.y), zero);
			this.AddQuad(vbo, simpleVert, new Vector2(num11, num4) + pixelAdjustedRect.position, new Vector2(num12, pixelAdjustedRect.height) + pixelAdjustedRect.position, new Vector2(uvMin.x, vector4.y), new Vector2(vector5.x, vector.w), zero);
			num11 += num;
		}
		this.AddQuad(vbo, simpleVert, new Vector2(0f, 0f) + pixelAdjustedRect.position, new Vector2(x, y) + pixelAdjustedRect.position, new Vector2(vector.x, vector.y), new Vector2(uvMin.x, uvMin.y), zero);
		this.AddQuad(vbo, simpleVert, new Vector2(num3, 0f) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, y) + pixelAdjustedRect.position, new Vector2(vector4.x, vector.y), new Vector2(vector.z, uvMin.y), zero);
		this.AddQuad(vbo, simpleVert, new Vector2(0f, num4) + pixelAdjustedRect.position, new Vector2(x, pixelAdjustedRect.height) + pixelAdjustedRect.position, new Vector2(vector.x, vector4.y), new Vector2(uvMin.x, vector.w), zero);
		this.AddQuad(vbo, simpleVert, new Vector2(num3, num4) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, pixelAdjustedRect.height) + pixelAdjustedRect.position, new Vector2(vector4.x, vector4.y), new Vector2(vector.z, vector.w), zero);
	}

	public override void SetNativeSize()
	{
		if (!base.overrideSprite)
		{
			return;
		}
		Vector2 vector = Image2.s_sizeScaling[(int)this.alphaTexLayout];
		float x = base.overrideSprite.rect.width * vector.x / base.pixelsPerUnit;
		float y = base.overrideSprite.rect.height * vector.y / base.pixelsPerUnit;
		base.rectTransform.anchorMax = base.rectTransform.anchorMin;
		base.rectTransform.sizeDelta = new Vector2(x, y);
		this.SetAllDirty();
	}

	protected override void OnFillVBO(List<UIVertex> vbo)
	{
		if (base.overrideSprite == null || (this.alphaTexLayout == ImageAlphaTexLayout.None && !this.WriteTexcoordToNormal))
		{
			base.OnFillVBO(vbo);
		}
		else
		{
			switch (base.type)
			{
			case Image.Type.Simple:
				if (this.WriteTexcoordToNormal)
				{
					this.GenerateSimpleSprite_Normal(vbo, base.preserveAspect);
				}
				else
				{
					this.GenerateSimpleSprite(vbo, base.preserveAspect);
				}
				break;
			case Image.Type.Sliced:
				this.GenerateSlicedSprite(vbo);
				break;
			case Image.Type.Tiled:
				this.GenerateTiledSprite(vbo);
				break;
			case Image.Type.Filled:
				this.GenerateFilledSprite(vbo, base.preserveAspect);
				break;
			default:
				DebugHelper.Assert(false);
				break;
			}
		}
	}

	protected override void OnDestroy()
	{
		base.sprite = null;
		base.overrideSprite = null;
	}

	protected override void OnCanvasHierarchyChanged()
	{
	}
}
