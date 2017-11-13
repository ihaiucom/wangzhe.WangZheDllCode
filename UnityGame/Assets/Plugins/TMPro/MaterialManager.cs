using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMPro
{
	public static class MaterialManager
	{
		private class MaskingMaterial
		{
			public Material baseMaterial;

			public Material stencilMaterial;

			public int count;

			public int stencilID;
		}

		private static List<MaterialManager.MaskingMaterial> m_materialList = new List<MaterialManager.MaskingMaterial>();

		private static Mask[] m_maskComponents = new Mask[0];

		public static Material GetStencilMaterial(Material baseMaterial, int stencilID)
		{
			if (!baseMaterial.HasProperty(ShaderUtilities.ID_StencilID))
			{
				Debug.LogWarning("Selected Shader does not support Stencil Masking. Please select the Distance Field or Mobile Distance Field Shader.");
				return baseMaterial;
			}
			int num = MaterialManager.m_materialList.FindIndex((MaterialManager.MaskingMaterial item) => item.baseMaterial == baseMaterial && item.stencilID == stencilID);
			Material material;
			if (num == -1)
			{
				material = new Material(baseMaterial);
				material.hideFlags = HideFlags.HideAndDontSave;
				Material material2 = material;
				material2.name = material2.name + " Masking ID:" + stencilID;
				material.shaderKeywords = baseMaterial.shaderKeywords;
				ShaderUtilities.GetShaderPropertyIDs();
				material.SetFloat(ShaderUtilities.ID_StencilID, (float)stencilID);
				material.SetFloat(ShaderUtilities.ID_StencilComp, 3f);
				MaterialManager.MaskingMaterial maskingMaterial = new MaterialManager.MaskingMaterial();
				maskingMaterial.baseMaterial = baseMaterial;
				maskingMaterial.stencilMaterial = material;
				maskingMaterial.stencilID = stencilID;
				maskingMaterial.count = 1;
				MaterialManager.m_materialList.Add(maskingMaterial);
			}
			else
			{
				material = MaterialManager.m_materialList.get_Item(num).stencilMaterial;
				MaterialManager.m_materialList.get_Item(num).count++;
			}
			MaterialManager.ListMaterials();
			return material;
		}

		public static Material GetBaseMaterial(Material stencilMaterial)
		{
			int num = MaterialManager.m_materialList.FindIndex((MaterialManager.MaskingMaterial item) => item.stencilMaterial == stencilMaterial);
			if (num == -1)
			{
				return null;
			}
			return MaterialManager.m_materialList.get_Item(num).baseMaterial;
		}

		public static Material SetStencil(Material material, int stencilID)
		{
			material.SetFloat(ShaderUtilities.ID_StencilID, (float)stencilID);
			if (stencilID == 0)
			{
				material.SetFloat(ShaderUtilities.ID_StencilComp, 8f);
			}
			else
			{
				material.SetFloat(ShaderUtilities.ID_StencilComp, 3f);
			}
			return material;
		}

		public static void AddMaskingMaterial(Material baseMaterial, Material stencilMaterial, int stencilID)
		{
			int num = MaterialManager.m_materialList.FindIndex((MaterialManager.MaskingMaterial item) => item.stencilMaterial == stencilMaterial);
			if (num == -1)
			{
				MaterialManager.MaskingMaterial maskingMaterial = new MaterialManager.MaskingMaterial();
				maskingMaterial.baseMaterial = baseMaterial;
				maskingMaterial.stencilMaterial = stencilMaterial;
				maskingMaterial.stencilID = stencilID;
				maskingMaterial.count = 1;
				MaterialManager.m_materialList.Add(maskingMaterial);
			}
			else
			{
				stencilMaterial = MaterialManager.m_materialList.get_Item(num).stencilMaterial;
				MaterialManager.m_materialList.get_Item(num).count++;
			}
		}

		public static void RemoveStencilMaterial(Material stencilMaterial)
		{
			int num = MaterialManager.m_materialList.FindIndex((MaterialManager.MaskingMaterial item) => item.stencilMaterial == stencilMaterial);
			if (num != -1)
			{
				MaterialManager.m_materialList.RemoveAt(num);
			}
			MaterialManager.ListMaterials();
		}

		public static void ReleaseBaseMaterial(Material baseMaterial)
		{
			int num = MaterialManager.m_materialList.FindIndex((MaterialManager.MaskingMaterial item) => item.baseMaterial == baseMaterial);
			if (num == -1)
			{
				Debug.Log("No Masking Material exists for " + baseMaterial.name);
			}
			else if (MaterialManager.m_materialList.get_Item(num).count > 1)
			{
				MaterialManager.m_materialList.get_Item(num).count--;
				Debug.Log(string.Concat(new object[]
				{
					"Removed (1) reference to ",
					MaterialManager.m_materialList.get_Item(num).stencilMaterial.name,
					". There are ",
					MaterialManager.m_materialList.get_Item(num).count,
					" references left."
				}));
			}
			else
			{
				Debug.Log(string.Concat(new object[]
				{
					"Removed last reference to ",
					MaterialManager.m_materialList.get_Item(num).stencilMaterial.name,
					" with ID ",
					MaterialManager.m_materialList.get_Item(num).stencilMaterial.GetInstanceID()
				}));
				Object.DestroyImmediate(MaterialManager.m_materialList.get_Item(num).stencilMaterial);
				MaterialManager.m_materialList.RemoveAt(num);
			}
			MaterialManager.ListMaterials();
		}

		public static void ReleaseStencilMaterial(Material stencilMaterial)
		{
			int num = MaterialManager.m_materialList.FindIndex((MaterialManager.MaskingMaterial item) => item.stencilMaterial == stencilMaterial);
			if (num != -1)
			{
				if (MaterialManager.m_materialList.get_Item(num).count > 1)
				{
					MaterialManager.m_materialList.get_Item(num).count--;
				}
				else
				{
					Object.DestroyImmediate(MaterialManager.m_materialList.get_Item(num).stencilMaterial);
					MaterialManager.m_materialList.RemoveAt(num);
				}
			}
			MaterialManager.ListMaterials();
		}

		public static void ClearMaterials()
		{
			if (MaterialManager.m_materialList.get_Count() == 0)
			{
				Debug.Log("Material List has already been cleared.");
				return;
			}
			for (int i = 0; i < MaterialManager.m_materialList.get_Count(); i++)
			{
				Material stencilMaterial = MaterialManager.m_materialList.get_Item(i).stencilMaterial;
				Object.DestroyImmediate(stencilMaterial);
				MaterialManager.m_materialList.RemoveAt(i);
			}
		}

		public static void ListMaterials()
		{
		}

		public static int GetStencilID(GameObject obj)
		{
			int num = 0;
			MaterialManager.m_maskComponents = obj.GetComponentsInParent<Mask>();
			for (int i = 0; i < MaterialManager.m_maskComponents.Length; i++)
			{
				if (MaterialManager.m_maskComponents[i].MaskEnabled())
				{
					num++;
				}
			}
			switch (num)
			{
			case 0:
				return 0;
			case 1:
				return 1;
			case 2:
				return 3;
			case 3:
				return 11;
			default:
				return 0;
			}
		}
	}
}
