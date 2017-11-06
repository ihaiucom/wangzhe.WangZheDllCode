using System;
using UnityEngine;

public class MaterialMarker : TemplateMarkerBase
{
	[Serializable]
	public class TextureCheckParam
	{
		[Tooltip("贴图在shader中对应的名字")]
		public string m_texNameInShader;

		[Tooltip("贴图尺寸,填0表示不检查")]
		public int m_texSize;

		[Tooltip("贴图的命名规则")]
		public TemplateMarkerBase.NamePattern m_textureNamePattern;

		[Tooltip("贴图是否允许带alpha")]
		public bool m_Alpha;
	}

	[Serializable]
	public class ShaderCheckParam
	{
		[Tooltip("shader参数的名字")]
		public string m_paramNameInShader;

		[Tooltip("该参数是否需要勾上")]
		public bool m_Enable;
	}

	[Tooltip("材质的命名规则，不填表示不检测")]
	public TemplateMarkerBase.NamePattern m_materialNamePattern;

	[Tooltip("Shader的命名规则，不填表示不检测")]
	public TemplateMarkerBase.NamePattern[] m_shaderNamePatterns;

	[Tooltip("shader参数设置，填入想要检测的参数的个数")]
	public MaterialMarker.ShaderCheckParam[] m_shaderCheckParams;

	[Tooltip("贴图参数设置，填入该材质需要检测的贴图的张数")]
	public MaterialMarker.TextureCheckParam[] m_texCheckParams;

	public override bool Check(GameObject targetObject, out string errorInfo)
	{
		errorInfo = string.Empty;
		Renderer component = targetObject.GetComponent<Renderer>();
		if (null == component)
		{
			errorInfo = "没有Render组件";
			return false;
		}
		Material sharedMaterial = component.sharedMaterial;
		if (null == sharedMaterial)
		{
			errorInfo = "没有Material";
			return false;
		}
		if (!string.IsNullOrEmpty(this.m_materialNamePattern.namePattern) && !base.isWildCardMatch(sharedMaterial.name, this.m_materialNamePattern.namePattern, this.m_materialNamePattern.ignoreCase))
		{
			errorInfo = string.Format("材质名称不符合规范,要求为{0}({1})，实际为{2}", this.m_materialNamePattern.namePattern, this.m_materialNamePattern.IgnoreCaseStr, sharedMaterial.name);
			return false;
		}
		if (this.m_shaderNamePatterns != null && this.m_shaderNamePatterns.Length > 0)
		{
			bool flag = false;
			string name = sharedMaterial.shader.name;
			TemplateMarkerBase.NamePattern[] shaderNamePatterns = this.m_shaderNamePatterns;
			for (int i = 0; i < shaderNamePatterns.Length; i++)
			{
				TemplateMarkerBase.NamePattern namePattern = shaderNamePatterns[i];
				flag |= (string.IsNullOrEmpty(namePattern.namePattern) || base.isWildCardMatch(name, namePattern.namePattern, namePattern.ignoreCase));
				if (flag)
				{
					break;
				}
			}
			if (!flag)
			{
				string text = string.Empty;
				TemplateMarkerBase.NamePattern[] shaderNamePatterns2 = this.m_shaderNamePatterns;
				for (int j = 0; j < shaderNamePatterns2.Length; j++)
				{
					TemplateMarkerBase.NamePattern namePattern2 = shaderNamePatterns2[j];
					text += string.Format("{0}({1})或", namePattern2.namePattern, namePattern2.IgnoreCaseStr);
				}
				text = text.TrimEnd(new char[]
				{
					'或'
				});
				errorInfo = string.Format("Shader名称不符合规范,要求为{0}，实际为{1}", text, name);
				return false;
			}
		}
		if (this.m_shaderCheckParams != null && this.m_shaderCheckParams.Length > 0)
		{
			string[] shaderKeywords = sharedMaterial.shaderKeywords;
			MaterialMarker.ShaderCheckParam[] shaderCheckParams = this.m_shaderCheckParams;
			for (int k = 0; k < shaderCheckParams.Length; k++)
			{
				MaterialMarker.ShaderCheckParam shaderCheckParam = shaderCheckParams[k];
				if (!string.IsNullOrEmpty(shaderCheckParam.m_paramNameInShader))
				{
					if (shaderCheckParam.m_Enable)
					{
						bool flag2 = false;
						string[] array = shaderKeywords;
						for (int l = 0; l < array.Length; l++)
						{
							string text2 = array[l];
							if (text2 == shaderCheckParam.m_paramNameInShader)
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							errorInfo = string.Format("Shader参数勾选不符合规范,要求为：必须勾上{0}，实际为：没有勾上{0}", shaderCheckParam.m_paramNameInShader);
							return false;
						}
					}
					else
					{
						bool flag3 = true;
						string[] array2 = shaderKeywords;
						for (int m = 0; m < array2.Length; m++)
						{
							string text3 = array2[m];
							if (text3 == shaderCheckParam.m_paramNameInShader)
							{
								flag3 = false;
								break;
							}
						}
						if (!flag3)
						{
							errorInfo = string.Format("Shader参数勾选不符合规范,要求为：不得勾上{0}，实际为：勾上了{0}", shaderCheckParam.m_paramNameInShader);
							return false;
						}
					}
				}
			}
		}
		if (this.m_texCheckParams != null)
		{
			MaterialMarker.TextureCheckParam[] texCheckParams = this.m_texCheckParams;
			for (int n = 0; n < texCheckParams.Length; n++)
			{
				MaterialMarker.TextureCheckParam textureCheckParam = texCheckParams[n];
				if (textureCheckParam != null && !string.IsNullOrEmpty(textureCheckParam.m_texNameInShader))
				{
					Texture texture = null;
					if (sharedMaterial.HasProperty(textureCheckParam.m_texNameInShader))
					{
						texture = sharedMaterial.GetTexture(textureCheckParam.m_texNameInShader);
					}
					if (!texture)
					{
						errorInfo = string.Format("材质{0}上{1}槽没有挂贴图", sharedMaterial.name, textureCheckParam.m_texNameInShader);
						return false;
					}
					if (!base.isWildCardMatch(texture.name, textureCheckParam.m_textureNamePattern.namePattern, textureCheckParam.m_textureNamePattern.ignoreCase))
					{
						errorInfo = string.Format("贴图名称不符合规范，要求为：{0}({1})，实际为：{2}", textureCheckParam.m_textureNamePattern.namePattern, textureCheckParam.m_textureNamePattern.IgnoreCaseStr, texture.name);
						return false;
					}
					if (textureCheckParam.m_texSize > 0 && (texture.width > textureCheckParam.m_texSize || texture.height > textureCheckParam.m_texSize))
					{
						errorInfo = string.Format("材质{0}上的贴图{1}尺寸超标，要求为：{2}x{2}，实际贴图为：{3}x{4}", new object[]
						{
							sharedMaterial.name,
							texture.name,
							textureCheckParam.m_texSize,
							texture.width,
							texture.height
						});
						return false;
					}
				}
			}
		}
		return true;
	}
}
