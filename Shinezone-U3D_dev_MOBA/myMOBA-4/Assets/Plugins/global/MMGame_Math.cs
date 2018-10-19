using System;
using UnityEngine;

public static class MMGame_Math
{
	public static float Dot3(this Vector3 a, Vector4 b)
	{
		return a.x * b.x + a.y * b.y + a.z * b.z;
	}

	public static float Dot3(this Vector3 a, ref Vector4 b)
	{
		return a.x * b.x + a.y * b.y + a.z * b.z;
	}

	public static float Dot3(this Vector3 a, Vector3 b)
	{
		return a.x * b.x + a.y * b.y + a.z * b.z;
	}

	public static float Dot3(this Vector3 a, ref Vector3 b)
	{
		return a.x * b.x + a.y * b.y + a.z * b.z;
	}

	public static float DotXZ(this Vector3 a, Vector3 b)
	{
		return a.x * b.x + a.z * b.z;
	}

	public static float DotXZ(this Vector3 a, ref Vector3 b)
	{
		return a.x * b.x + a.z * b.z;
	}

	public static Vector3 Mul(this Vector3 a, Vector3 b)
	{
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	public static Vector3 Mul(this Vector3 a, ref Vector3 b)
	{
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	public static Vector3 Mul(this Vector3 a, Vector3 b, float f)
	{
		return new Vector3(a.x * b.x * f, a.y * b.y * f, a.z * b.z * f);
	}

	public static Vector3 Mul(this Vector3 a, ref Vector3 b, float f)
	{
		return new Vector3(a.x * b.x * f, a.y * b.y * f, a.z * b.z * f);
	}

	public static float XZSqrMagnitude(this Vector3 a, Vector3 b)
	{
		float num = a.x - b.x;
		float num2 = a.z - b.z;
		return num * num + num2 * num2;
	}

	public static float XZSqrMagnitude(this Vector3 a, ref Vector3 b)
	{
		float num = a.x - b.x;
		float num2 = a.z - b.z;
		return num * num + num2 * num2;
	}

	public static Vector2 xz(this Vector3 a)
	{
		return new Vector2(a.x, a.z);
	}

	public static string ToString2(this Vector3 a)
	{
		return string.Format("({0},{1},{2})", a.x, a.y, a.z);
	}

	public static Vector3 toVec3(this Vector4 a)
	{
		return new Vector3(a.x, a.y, a.z);
	}

	public static Vector4 toVec4(this Vector3 v, float a)
	{
		return new Vector4(v.x, v.y, v.z, a);
	}

	public static Vector3 RotateY(this Vector3 v, float angle)
	{
		float num = Mathf.Sin(angle);
		float num2 = Mathf.Cos(angle);
		Vector3 result;
		result.x = v.x * num2 + v.z * num;
		result.z = -v.x * num + v.z * num2;
		result.y = v.y;
		return result;
	}

	public static bool isMirror(Matrix4x4 m)
	{
		Vector3 lhs = m.GetColumn(0).toVec3();
		Vector3 rhs = m.GetColumn(1).toVec3();
		Vector3 a = m.GetColumn(2).toVec3();
		Vector3 vector = Vector3.Cross(lhs, rhs);
		a.Normalize();
		vector.Normalize();
		float num = a.Dot3(ref vector);
		return num < 0f;
	}

	public static void SetLayer(this GameObject go, string layerName, bool bFileSkillIndicator = false)
	{
		int layer = LayerMask.NameToLayer(layerName);
		MMGame_Math.SetLayerRecursively(go, layer, bFileSkillIndicator);
	}

	public static void SetLayer(this GameObject go, int layer, bool bFileSkillIndicator)
	{
		MMGame_Math.SetLayerRecursively(go, layer, bFileSkillIndicator);
	}

	public static void SetLayerRecursively(GameObject go, int layer, bool bFileSkillIndicator)
	{
		if (bFileSkillIndicator && go.CompareTag("SCI"))
		{
			return;
		}
		go.layer = layer;
		int childCount = go.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			MMGame_Math.SetLayerRecursively(go.transform.GetChild(i).gameObject, layer, bFileSkillIndicator);
		}
	}

	public static void SetGameObjVisible(this GameObject go, bool bVisible)
	{
		if (go.IsGameObjHidden() == !bVisible)
		{
			return;
		}
		if (bVisible)
		{
			go.SetLayer("Actor", "Particles", true);
		}
		else
		{
			go.SetLayer("Hide", true);
		}
	}

	public static bool IsGameObjHidden(this GameObject go)
	{
		string a = LayerMask.LayerToName(go.layer);
		return a == "Hide";
	}

	public static void SetVisibleSameAs(this GameObject go, GameObject tarGo)
	{
		if (tarGo.IsGameObjHidden())
		{
			go.SetGameObjVisible(false);
		}
		else
		{
			go.SetGameObjVisible(true);
		}
	}

	public static void SetLayer(this GameObject go, string layerName, string layerNameParticles, bool bFileSkillIndicator = false)
	{
		int layer = LayerMask.NameToLayer(layerName);
		int layerParticles = LayerMask.NameToLayer(layerNameParticles);
		MMGame_Math.SetLayerRecursively(go, layer, layerParticles, bFileSkillIndicator);
	}

	public static void SetLayer(this GameObject go, int layer, int layerParticles, bool bFileSkillIndicator)
	{
		MMGame_Math.SetLayerRecursively(go, layer, layerParticles, bFileSkillIndicator);
	}

	public static void SetLayerRecursively(GameObject go, int layer, int layerParticles, bool bFileSkillIndicator)
	{
		if (bFileSkillIndicator && go.CompareTag("SCI"))
		{
			return;
		}
		if (go.GetComponent<ParticleSystem>() != null)
		{
			go.layer = layerParticles;
		}
		else
		{
			go.layer = layer;
		}
		int childCount = go.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			MMGame_Math.SetLayerRecursively(go.transform.GetChild(i).gameObject, layer, layerParticles, bFileSkillIndicator);
		}
	}

	public static Renderer GetRendererInChildren(this GameObject go)
	{
		if (go.GetComponent<Renderer>())
		{
			return go.GetComponent<Renderer>();
		}
		int childCount = go.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = go.transform.GetChild(i);
			if (child && child.gameObject)
			{
				Renderer rendererInChildren = child.gameObject.GetRendererInChildren();
				if (rendererInChildren)
				{
					return rendererInChildren;
				}
			}
		}
		return null;
	}

	public static SkinnedMeshRenderer GetSkinnedMeshRendererInChildren(this GameObject go)
	{
		SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponent<Renderer>() as SkinnedMeshRenderer;
		if (skinnedMeshRenderer)
		{
			return skinnedMeshRenderer;
		}
		int childCount = go.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = go.transform.GetChild(i);
			if (child && child.gameObject)
			{
				skinnedMeshRenderer = child.gameObject.GetSkinnedMeshRendererInChildren();
				if (skinnedMeshRenderer)
				{
					return skinnedMeshRenderer;
				}
			}
		}
		return null;
	}

	public static MeshRenderer GetMeshRendererInChildren(this GameObject go)
	{
		MeshRenderer meshRenderer = go.GetComponent<Renderer>() as MeshRenderer;
		if (meshRenderer)
		{
			return meshRenderer;
		}
		int childCount = go.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = go.transform.GetChild(i);
			if (child && child.gameObject)
			{
				meshRenderer = child.gameObject.GetMeshRendererInChildren();
				if (meshRenderer)
				{
					return meshRenderer;
				}
			}
		}
		return null;
	}

	public static void SetOffsetX(this Camera camera, float offsetX)
	{
		float num = 2f * Mathf.Tan(0.0174532924f * camera.fieldOfView * 0.5f) * camera.nearClipPlane;
		float num2 = num * camera.aspect;
		float num3 = -Mathf.Clamp(offsetX, -1f, 1f) * num2;
		float num4 = (num2 + num3) * 0.5f;
		float left = num4 - num2;
		camera.SetPerspectiveOffCenter(left, num4, -num * 0.5f, num * 0.5f, camera.nearClipPlane, camera.farClipPlane);
	}

	public static void SetPerspectiveOffCenter(this Camera camera, float left, float right, float bottom, float top, float near, float far)
	{
		float num = 1f / (right - left);
		float num2 = 1f / (top - bottom);
		float num3 = 1f / (near - far);
		camera.projectionMatrix = new Matrix4x4
		{
			m00 = 2f * near * num,
			m10 = 0f,
			m20 = 0f,
			m30 = 0f,
			m01 = 0f,
			m11 = 2f * near * num2,
			m21 = 0f,
			m31 = 0f,
			m02 = (right + left) * num,
			m12 = (top + bottom) * num2,
			m22 = far * num3,
			m32 = -1f,
			m03 = 0f,
			m13 = 0f,
			m23 = 2f * far * near * num3,
			m33 = 0f
		};
	}

	public static Vector2 Lerp(this Vector2 left, Vector2 right, float lerp)
	{
		return new Vector2(Mathf.Lerp(left.x, right.x, lerp), Mathf.Lerp(left.y, right.y, lerp));
	}

	public static Vector3 Lerp(this Vector3 left, Vector3 right, float lerp)
	{
		return new Vector3(Mathf.Lerp(left.x, right.x, lerp), Mathf.Lerp(left.y, right.y, lerp), Mathf.Lerp(left.z, right.z, lerp));
	}

	public static Vector4 Lerp(this Vector4 left, Vector4 right, float lerp)
	{
		return new Vector4(Mathf.Lerp(left.x, right.x, lerp), Mathf.Lerp(left.y, right.y, lerp), Mathf.Lerp(left.z, right.z, lerp), Mathf.Lerp(left.w, right.w, lerp));
	}

	public static int RoundToInt(double x)
	{
		if (x >= 0.0)
		{
			return (int)(x + 0.5);
		}
		return (int)(x - 0.5);
	}

	public static double Round(double x)
	{
		return (double)MMGame_Math.RoundToInt(x);
	}
}
