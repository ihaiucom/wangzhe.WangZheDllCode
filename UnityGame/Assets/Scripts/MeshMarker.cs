using System;
using UnityEngine;

public class MeshMarker : TemplateMarkerBase
{
	[Tooltip("Mesh的命名规则")]
	public TemplateMarkerBase.NamePattern m_fbxNamePattern;

	[Tooltip("最大面数，填0或负数表示不检测")]
	public int m_maxTriangleNum;

	[Tooltip("材质数，填0或负数表示不检测")]
	public int m_materialNum;

	public override bool Check(GameObject targetObject, out string errorInfo)
	{
		errorInfo = string.Empty;
		MeshFilter component = targetObject.GetComponent<MeshFilter>();
		SkinnedMeshRenderer component2 = targetObject.GetComponent<SkinnedMeshRenderer>();
		if (null == component && component2 == null)
		{
			errorInfo = "没有MeshFilter组件或者SkinnedMeshRender组件";
			return false;
		}
		Mesh sharedMesh;
		if (null != component)
		{
			sharedMesh = component.sharedMesh;
		}
		else
		{
			sharedMesh = component2.sharedMesh;
		}
		if (null == sharedMesh)
		{
			errorInfo = "没有Mesh";
			return false;
		}
		if (!base.isWildCardMatch(sharedMesh.name, this.m_fbxNamePattern.namePattern, this.m_fbxNamePattern.ignoreCase))
		{
			errorInfo = string.Format("Mesh命名不符合规范，要求：{0}({1})，实际：{2}", this.m_fbxNamePattern.namePattern, this.m_fbxNamePattern.IgnoreCaseStr, sharedMesh.name);
			return false;
		}
		if (this.m_maxTriangleNum > 0)
		{
			int num = sharedMesh.triangles.Length / 3;
			if (num > this.m_maxTriangleNum)
			{
				errorInfo = string.Format("Mesh{0}最大面数超标，限定最大面数:{1}，实际面数:{2}", sharedMesh.name, this.m_maxTriangleNum, num);
				return false;
			}
		}
		if (this.m_materialNum > 0)
		{
			Renderer component3 = targetObject.GetComponent<Renderer>();
			int num2 = component3.sharedMaterials.Length;
			if (num2 != this.m_materialNum)
			{
				errorInfo = string.Format("材质数量不符合标准，限定数量:{0}，实际数量:{1}", this.m_materialNum, num2);
				return false;
			}
		}
		return true;
	}
}
