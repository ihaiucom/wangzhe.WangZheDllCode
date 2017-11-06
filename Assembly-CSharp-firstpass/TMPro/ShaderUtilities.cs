using System;
using UnityEngine;

namespace TMPro
{
	public static class ShaderUtilities
	{
		public static int ID_MainTex;

		public static int ID_FaceTex;

		public static int ID_FaceColor;

		public static int ID_FaceDilate;

		public static int ID_Shininess;

		public static int ID_UnderlayColor;

		public static int ID_UnderlayOffsetX;

		public static int ID_UnderlayOffsetY;

		public static int ID_UnderlayDilate;

		public static int ID_UnderlaySoftness;

		public static int ID_WeightNormal;

		public static int ID_WeightBold;

		public static int ID_OutlineTex;

		public static int ID_OutlineWidth;

		public static int ID_OutlineSoftness;

		public static int ID_OutlineColor;

		public static int ID_GradientScale;

		public static int ID_ScaleX;

		public static int ID_ScaleY;

		public static int ID_PerspectiveFilter;

		public static int ID_TextureWidth;

		public static int ID_TextureHeight;

		public static int ID_BevelAmount;

		public static int ID_GlowColor;

		public static int ID_GlowOffset;

		public static int ID_GlowPower;

		public static int ID_GlowOuter;

		public static int ID_LightAngle;

		public static int ID_EnvMap;

		public static int ID_EnvMatrix;

		public static int ID_EnvMatrixRotation;

		public static int ID_MaskID;

		public static int ID_MaskCoord;

		public static int ID_MaskSoftnessX;

		public static int ID_MaskSoftnessY;

		public static int ID_VertexOffsetX;

		public static int ID_VertexOffsetY;

		public static int ID_StencilID;

		public static int ID_StencilComp;

		public static int ID_ShaderFlags;

		public static int ID_ScaleRatio_A;

		public static int ID_ScaleRatio_B;

		public static int ID_ScaleRatio_C;

		public static string Keyword_Bevel = "BEVEL_ON";

		public static string Keyword_Glow = "GLOW_ON";

		public static string Keyword_Underlay = "UNDERLAY_ON";

		public static string Keyword_Ratios = "RATIOS_OFF";

		public static string Keyword_MASK_OFF = "MASK_OFF";

		public static string Keyword_MASK_SOFT = "MASK_SOFT";

		public static string Keyword_MASK_HARD = "MASK_HARD";

		public static string ShaderTag_ZTestMode = "_ZTestMode";

		public static string ShaderTag_CullMode = "_CullMode";

		private static float m_clamp = 1f;

		public static bool isInitialized;

		public static void GetShaderPropertyIDs()
		{
			if (!ShaderUtilities.isInitialized)
			{
				ShaderUtilities.isInitialized = true;
				ShaderUtilities.ID_MainTex = Shader.PropertyToID("_MainTex");
				ShaderUtilities.ID_FaceTex = Shader.PropertyToID("_FaceTex");
				ShaderUtilities.ID_FaceColor = Shader.PropertyToID("_FaceColor");
				ShaderUtilities.ID_FaceDilate = Shader.PropertyToID("_FaceDilate");
				ShaderUtilities.ID_Shininess = Shader.PropertyToID("_FaceShininess");
				ShaderUtilities.ID_UnderlayColor = Shader.PropertyToID("_UnderlayColor");
				ShaderUtilities.ID_UnderlayOffsetX = Shader.PropertyToID("_UnderlayOffsetX");
				ShaderUtilities.ID_UnderlayOffsetY = Shader.PropertyToID("_UnderlayOffsetY");
				ShaderUtilities.ID_UnderlayDilate = Shader.PropertyToID("_UnderlayDilate");
				ShaderUtilities.ID_UnderlaySoftness = Shader.PropertyToID("_UnderlaySoftness");
				ShaderUtilities.ID_WeightNormal = Shader.PropertyToID("_WeightNormal");
				ShaderUtilities.ID_WeightBold = Shader.PropertyToID("_WeightBold");
				ShaderUtilities.ID_OutlineTex = Shader.PropertyToID("_OutlineTex");
				ShaderUtilities.ID_OutlineWidth = Shader.PropertyToID("_OutlineWidth");
				ShaderUtilities.ID_OutlineSoftness = Shader.PropertyToID("_OutlineSoftness");
				ShaderUtilities.ID_OutlineColor = Shader.PropertyToID("_OutlineColor");
				ShaderUtilities.ID_GradientScale = Shader.PropertyToID("_GradientScale");
				ShaderUtilities.ID_ScaleX = Shader.PropertyToID("_ScaleX");
				ShaderUtilities.ID_ScaleY = Shader.PropertyToID("_ScaleY");
				ShaderUtilities.ID_PerspectiveFilter = Shader.PropertyToID("_PerspectiveFilter");
				ShaderUtilities.ID_TextureWidth = Shader.PropertyToID("_TextureWidth");
				ShaderUtilities.ID_TextureHeight = Shader.PropertyToID("_TextureHeight");
				ShaderUtilities.ID_BevelAmount = Shader.PropertyToID("_Bevel");
				ShaderUtilities.ID_LightAngle = Shader.PropertyToID("_LightAngle");
				ShaderUtilities.ID_EnvMap = Shader.PropertyToID("_Cube");
				ShaderUtilities.ID_EnvMatrix = Shader.PropertyToID("_EnvMatrix");
				ShaderUtilities.ID_EnvMatrixRotation = Shader.PropertyToID("_EnvMatrixRotation");
				ShaderUtilities.ID_GlowColor = Shader.PropertyToID("_GlowColor");
				ShaderUtilities.ID_GlowOffset = Shader.PropertyToID("_GlowOffset");
				ShaderUtilities.ID_GlowPower = Shader.PropertyToID("_GlowPower");
				ShaderUtilities.ID_GlowOuter = Shader.PropertyToID("_GlowOuter");
				ShaderUtilities.ID_MaskID = Shader.PropertyToID("_MaskID");
				ShaderUtilities.ID_MaskCoord = Shader.PropertyToID("_MaskCoord");
				ShaderUtilities.ID_MaskSoftnessX = Shader.PropertyToID("_MaskSoftnessX");
				ShaderUtilities.ID_MaskSoftnessY = Shader.PropertyToID("_MaskSoftnessY");
				ShaderUtilities.ID_VertexOffsetX = Shader.PropertyToID("_VertexOffsetX");
				ShaderUtilities.ID_VertexOffsetY = Shader.PropertyToID("_VertexOffsetY");
				ShaderUtilities.ID_StencilID = Shader.PropertyToID("_Stencil");
				ShaderUtilities.ID_StencilComp = Shader.PropertyToID("_StencilComp");
				ShaderUtilities.ID_ShaderFlags = Shader.PropertyToID("_ShaderFlags");
				ShaderUtilities.ID_ScaleRatio_A = Shader.PropertyToID("_ScaleRatioA");
				ShaderUtilities.ID_ScaleRatio_B = Shader.PropertyToID("_ScaleRatioB");
				ShaderUtilities.ID_ScaleRatio_C = Shader.PropertyToID("_ScaleRatioC");
			}
		}

		public static void UpdateShaderRatios(Material mat, bool isBold)
		{
			bool flag = !ShaderUtilities.StringContains(mat.shaderKeywords, ShaderUtilities.Keyword_Ratios);
			float @float = mat.GetFloat(ShaderUtilities.ID_GradientScale);
			float float2 = mat.GetFloat(ShaderUtilities.ID_FaceDilate);
			float float3 = mat.GetFloat(ShaderUtilities.ID_OutlineWidth);
			float float4 = mat.GetFloat(ShaderUtilities.ID_OutlineSoftness);
			float num = (!isBold) ? (mat.GetFloat(ShaderUtilities.ID_WeightNormal) * 2f / @float) : (mat.GetFloat(ShaderUtilities.ID_WeightBold) * 2f / @float);
			float num2 = Mathf.Max(1f, num + float2 + float3 + float4);
			float value = flag ? ((@float - ShaderUtilities.m_clamp) / (@float * num2)) : 1f;
			mat.SetFloat(ShaderUtilities.ID_ScaleRatio_A, value);
			if (mat.HasProperty(ShaderUtilities.ID_GlowOffset))
			{
				float float5 = mat.GetFloat(ShaderUtilities.ID_GlowOffset);
				float float6 = mat.GetFloat(ShaderUtilities.ID_GlowOuter);
				float num3 = (num + float2) * (@float - ShaderUtilities.m_clamp);
				num2 = Mathf.Max(1f, float5 + float6);
				float value2 = flag ? (Mathf.Max(0f, @float - ShaderUtilities.m_clamp - num3) / (@float * num2)) : 1f;
				mat.SetFloat(ShaderUtilities.ID_ScaleRatio_B, value2);
			}
			if (mat.HasProperty(ShaderUtilities.ID_UnderlayOffsetX))
			{
				float float7 = mat.GetFloat(ShaderUtilities.ID_UnderlayOffsetX);
				float float8 = mat.GetFloat(ShaderUtilities.ID_UnderlayOffsetY);
				float float9 = mat.GetFloat(ShaderUtilities.ID_UnderlayDilate);
				float float10 = mat.GetFloat(ShaderUtilities.ID_UnderlaySoftness);
				float num4 = (num + float2) * (@float - ShaderUtilities.m_clamp);
				num2 = Mathf.Max(1f, Mathf.Max(Mathf.Abs(float7), Mathf.Abs(float8)) + float9 + float10);
				float value3 = flag ? (Mathf.Max(0f, @float - ShaderUtilities.m_clamp - num4) / (@float * num2)) : 1f;
				mat.SetFloat(ShaderUtilities.ID_ScaleRatio_C, value3);
			}
		}

		public static Vector4 GetFontExtent(Material material)
		{
			if (material == null || !material.HasProperty(ShaderUtilities.ID_GradientScale))
			{
				return Vector4.zero;
			}
			float @float = material.GetFloat(ShaderUtilities.ID_ScaleRatio_A);
			float num = material.GetFloat(ShaderUtilities.ID_FaceDilate) * @float;
			float num2 = material.GetFloat(ShaderUtilities.ID_OutlineWidth) * @float;
			float num3 = Mathf.Min(1f, num + num2);
			num3 *= material.GetFloat(ShaderUtilities.ID_GradientScale);
			return new Vector4(num3, num3, num3, num3);
		}

		public static bool IsMaskingEnabled(Material material)
		{
			return !(material == null) && material.HasProperty(ShaderUtilities.ID_MaskCoord) && (ShaderUtilities.StringContains(material.shaderKeywords, ShaderUtilities.Keyword_MASK_SOFT) || ShaderUtilities.StringContains(material.shaderKeywords, ShaderUtilities.Keyword_MASK_HARD));
		}

		public static float GetPadding(Material material, bool enableExtraPadding, bool isBold)
		{
			if (!ShaderUtilities.isInitialized)
			{
				ShaderUtilities.GetShaderPropertyIDs();
			}
			if (material == null)
			{
				return 0f;
			}
			int num = enableExtraPadding ? 4 : 0;
			if (!material.HasProperty(ShaderUtilities.ID_GradientScale))
			{
				return (float)num;
			}
			Vector4 a = Vector4.zero;
			Vector4 zero = Vector4.zero;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			ShaderUtilities.UpdateShaderRatios(material, isBold);
			string[] shaderKeywords = material.shaderKeywords;
			if (material.HasProperty(ShaderUtilities.ID_ScaleRatio_A))
			{
				num5 = material.GetFloat(ShaderUtilities.ID_ScaleRatio_A);
			}
			if (material.HasProperty(ShaderUtilities.ID_FaceDilate))
			{
				num2 = material.GetFloat(ShaderUtilities.ID_FaceDilate) * num5;
			}
			if (material.HasProperty(ShaderUtilities.ID_OutlineSoftness))
			{
				num3 = material.GetFloat(ShaderUtilities.ID_OutlineSoftness) * num5;
			}
			if (material.HasProperty(ShaderUtilities.ID_OutlineWidth))
			{
				num4 = material.GetFloat(ShaderUtilities.ID_OutlineWidth) * num5;
			}
			float num10 = num4 + num3 + num2;
			if (material.HasProperty(ShaderUtilities.ID_GlowOffset) && ShaderUtilities.StringContains(shaderKeywords, ShaderUtilities.Keyword_Glow))
			{
				if (material.HasProperty(ShaderUtilities.ID_ScaleRatio_B))
				{
					num6 = material.GetFloat(ShaderUtilities.ID_ScaleRatio_B);
				}
				num8 = material.GetFloat(ShaderUtilities.ID_GlowOffset) * num6;
				num9 = material.GetFloat(ShaderUtilities.ID_GlowOuter) * num6;
			}
			num10 = Mathf.Max(num10, num2 + num8 + num9);
			if (material.HasProperty(ShaderUtilities.ID_UnderlaySoftness) && ShaderUtilities.StringContains(shaderKeywords, ShaderUtilities.Keyword_Underlay))
			{
				if (material.HasProperty(ShaderUtilities.ID_ScaleRatio_C))
				{
					num7 = material.GetFloat(ShaderUtilities.ID_ScaleRatio_C);
				}
				float num11 = material.GetFloat(ShaderUtilities.ID_UnderlayOffsetX) * num7;
				float num12 = material.GetFloat(ShaderUtilities.ID_UnderlayOffsetY) * num7;
				float num13 = material.GetFloat(ShaderUtilities.ID_UnderlayDilate) * num7;
				float num14 = material.GetFloat(ShaderUtilities.ID_UnderlaySoftness) * num7;
				a.x = Mathf.Max(a.x, num2 + num13 + num14 - num11);
				a.y = Mathf.Max(a.y, num2 + num13 + num14 - num12);
				a.z = Mathf.Max(a.z, num2 + num13 + num14 + num11);
				a.w = Mathf.Max(a.w, num2 + num13 + num14 + num12);
			}
			a.x = Mathf.Max(a.x, num10);
			a.y = Mathf.Max(a.y, num10);
			a.z = Mathf.Max(a.z, num10);
			a.w = Mathf.Max(a.w, num10);
			a.x += (float)num;
			a.y += (float)num;
			a.z += (float)num;
			a.w += (float)num;
			a.x = Mathf.Min(a.x, 1f);
			a.y = Mathf.Min(a.y, 1f);
			a.z = Mathf.Min(a.z, 1f);
			a.w = Mathf.Min(a.w, 1f);
			zero.x = ((zero.x < a.x) ? a.x : zero.x);
			zero.y = ((zero.y < a.y) ? a.y : zero.y);
			zero.z = ((zero.z < a.z) ? a.z : zero.z);
			zero.w = ((zero.w < a.w) ? a.w : zero.w);
			float @float = material.GetFloat(ShaderUtilities.ID_GradientScale);
			a *= @float;
			num10 = Mathf.Max(a.x, a.y);
			num10 = Mathf.Max(a.z, num10);
			num10 = Mathf.Max(a.w, num10);
			return num10 + 0.25f;
		}

		public static float GetPadding(Material[] materials, bool enableExtraPadding, bool isBold)
		{
			if (!ShaderUtilities.isInitialized)
			{
				ShaderUtilities.GetShaderPropertyIDs();
			}
			if (materials == null)
			{
				return 0f;
			}
			int num = enableExtraPadding ? 4 : 0;
			if (!materials[0].HasProperty(ShaderUtilities.ID_GradientScale))
			{
				return (float)num;
			}
			Vector4 a = Vector4.zero;
			Vector4 zero = Vector4.zero;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			float num10;
			for (int i = 0; i < materials.Length; i++)
			{
				ShaderUtilities.UpdateShaderRatios(materials[i], isBold);
				string[] shaderKeywords = materials[i].shaderKeywords;
				if (materials[i].HasProperty(ShaderUtilities.ID_ScaleRatio_A))
				{
					num5 = materials[i].GetFloat(ShaderUtilities.ID_ScaleRatio_A);
				}
				if (materials[i].HasProperty(ShaderUtilities.ID_FaceDilate))
				{
					num2 = materials[i].GetFloat(ShaderUtilities.ID_FaceDilate) * num5;
				}
				if (materials[i].HasProperty(ShaderUtilities.ID_OutlineSoftness))
				{
					num3 = materials[i].GetFloat(ShaderUtilities.ID_OutlineSoftness) * num5;
				}
				if (materials[i].HasProperty(ShaderUtilities.ID_OutlineWidth))
				{
					num4 = materials[i].GetFloat(ShaderUtilities.ID_OutlineWidth) * num5;
				}
				num10 = num4 + num3 + num2;
				if (materials[i].HasProperty(ShaderUtilities.ID_GlowOffset) && ShaderUtilities.StringContains(shaderKeywords, ShaderUtilities.Keyword_Glow))
				{
					if (materials[i].HasProperty(ShaderUtilities.ID_ScaleRatio_B))
					{
						num6 = materials[i].GetFloat(ShaderUtilities.ID_ScaleRatio_B);
					}
					num8 = materials[i].GetFloat(ShaderUtilities.ID_GlowOffset) * num6;
					num9 = materials[i].GetFloat(ShaderUtilities.ID_GlowOuter) * num6;
				}
				num10 = Mathf.Max(num10, num2 + num8 + num9);
				if (materials[i].HasProperty(ShaderUtilities.ID_UnderlaySoftness) && ShaderUtilities.StringContains(shaderKeywords, ShaderUtilities.Keyword_Underlay))
				{
					if (materials[i].HasProperty(ShaderUtilities.ID_ScaleRatio_C))
					{
						num7 = materials[i].GetFloat(ShaderUtilities.ID_ScaleRatio_C);
					}
					float num11 = materials[i].GetFloat(ShaderUtilities.ID_UnderlayOffsetX) * num7;
					float num12 = materials[i].GetFloat(ShaderUtilities.ID_UnderlayOffsetY) * num7;
					float num13 = materials[i].GetFloat(ShaderUtilities.ID_UnderlayDilate) * num7;
					float num14 = materials[i].GetFloat(ShaderUtilities.ID_UnderlaySoftness) * num7;
					a.x = Mathf.Max(a.x, num2 + num13 + num14 - num11);
					a.y = Mathf.Max(a.y, num2 + num13 + num14 - num12);
					a.z = Mathf.Max(a.z, num2 + num13 + num14 + num11);
					a.w = Mathf.Max(a.w, num2 + num13 + num14 + num12);
				}
				a.x = Mathf.Max(a.x, num10);
				a.y = Mathf.Max(a.y, num10);
				a.z = Mathf.Max(a.z, num10);
				a.w = Mathf.Max(a.w, num10);
				a.x += (float)num;
				a.y += (float)num;
				a.z += (float)num;
				a.w += (float)num;
				a.x = Mathf.Min(a.x, 1f);
				a.y = Mathf.Min(a.y, 1f);
				a.z = Mathf.Min(a.z, 1f);
				a.w = Mathf.Min(a.w, 1f);
				zero.x = ((zero.x < a.x) ? a.x : zero.x);
				zero.y = ((zero.y < a.y) ? a.y : zero.y);
				zero.z = ((zero.z < a.z) ? a.z : zero.z);
				zero.w = ((zero.w < a.w) ? a.w : zero.w);
			}
			float @float = materials[0].GetFloat(ShaderUtilities.ID_GradientScale);
			a *= @float;
			num10 = Mathf.Max(a.x, a.y);
			num10 = Mathf.Max(a.z, num10);
			num10 = Mathf.Max(a.w, num10);
			return num10 + 0.25f;
		}

		public static bool StringContains(string[] strs, string key)
		{
			if (strs != null)
			{
				for (int i = 0; i < strs.Length; i++)
				{
					if (key == strs[i])
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
