using System;
using UnityEngine;

public class NcTransformTool
{
	public Vector3 m_vecPos;

	public Quaternion m_vecRot;

	public Vector3 m_vecRotHint;

	public Vector3 m_vecScale;

	public NcTransformTool()
	{
		this.m_vecPos = default(Vector3);
		this.m_vecRot = default(Quaternion);
		this.m_vecRotHint = default(Vector3);
		this.m_vecScale = new Vector3(1f, 1f, 1f);
	}

	public NcTransformTool(Transform val)
	{
		this.SetLocalTransform(val);
	}

	public static Vector3 GetZeroVector()
	{
		return Vector3.zero;
	}

	public static Vector3 GetUnitVector()
	{
		return new Vector3(1f, 1f, 1f);
	}

	public static Quaternion GetIdenQuaternion()
	{
		return Quaternion.identity;
	}

	public static void InitLocalTransform(Transform dst)
	{
		dst.localPosition = NcTransformTool.GetZeroVector();
		dst.localRotation = NcTransformTool.GetIdenQuaternion();
		dst.localScale = NcTransformTool.GetUnitVector();
	}

	public static void InitWorldTransform(Transform dst)
	{
		dst.position = NcTransformTool.GetZeroVector();
		dst.rotation = NcTransformTool.GetIdenQuaternion();
		NcTransformTool.InitWorldScale(dst);
	}

	public static void InitWorldScale(Transform dst)
	{
		dst.localScale = NcTransformTool.GetUnitVector();
		dst.localScale = new Vector3((dst.lossyScale.x == 0f) ? 1f : (1f / dst.lossyScale.x), (dst.lossyScale.y == 0f) ? 1f : (1f / dst.lossyScale.y), (dst.lossyScale.z == 0f) ? 1f : (1f / dst.lossyScale.z));
	}

	public static void CopyLocalTransform(Transform src, Transform dst)
	{
		dst.localPosition = src.localPosition;
		dst.localRotation = src.localRotation;
		dst.localScale = src.localScale;
	}

	public static void CopyLossyToLocalScale(Vector3 srcLossyScale, Transform dst)
	{
		dst.localScale = NcTransformTool.GetUnitVector();
		dst.localScale = new Vector3((dst.lossyScale.x == 0f) ? srcLossyScale.x : (srcLossyScale.x / dst.lossyScale.x), (dst.lossyScale.y == 0f) ? srcLossyScale.y : (srcLossyScale.y / dst.lossyScale.y), (dst.lossyScale.z == 0f) ? srcLossyScale.z : (srcLossyScale.z / dst.lossyScale.z));
	}

	public void CopyToLocalTransform(Transform dst)
	{
		dst.localPosition = this.m_vecPos;
		dst.localRotation = this.m_vecRot;
		dst.localScale = this.m_vecScale;
	}

	public void CopyToTransform(Transform dst)
	{
		dst.position = this.m_vecPos;
		dst.rotation = this.m_vecRot;
		NcTransformTool.CopyLossyToLocalScale(this.m_vecScale, dst);
	}

	public void AddLocalTransform(Transform val)
	{
		this.m_vecPos += val.localPosition;
		this.m_vecRot = Quaternion.Euler(this.m_vecRot.eulerAngles + val.localRotation.eulerAngles);
		this.m_vecScale = Vector3.Scale(this.m_vecScale, val.localScale);
	}

	public void SetLocalTransform(Transform val)
	{
		this.m_vecPos = val.localPosition;
		this.m_vecRot = val.localRotation;
		this.m_vecScale = val.localScale;
	}

	public bool IsLocalEquals(Transform val)
	{
		return !(this.m_vecPos != val.localPosition) && !(this.m_vecRot != val.localRotation) && !(this.m_vecScale != val.localScale);
	}

	public void AddTransform(Transform val)
	{
		this.m_vecPos += val.position;
		this.m_vecRot = Quaternion.Euler(this.m_vecRot.eulerAngles + val.rotation.eulerAngles);
		this.m_vecScale = Vector3.Scale(this.m_vecScale, val.lossyScale);
	}

	public void SetTransform(Transform val)
	{
		this.m_vecPos = val.position;
		this.m_vecRot = val.rotation;
		this.m_vecScale = val.lossyScale;
	}

	public bool IsEquals(Transform val)
	{
		return !(this.m_vecPos != val.position) && !(this.m_vecRot != val.rotation) && !(this.m_vecScale != val.lossyScale);
	}

	public void SetTransform(NcTransformTool val)
	{
		this.m_vecPos = val.m_vecPos;
		this.m_vecRot = val.m_vecRot;
		this.m_vecScale = val.m_vecScale;
	}

	public static float GetTransformScaleMeanValue(Transform srcTrans)
	{
		return (srcTrans.lossyScale.x + srcTrans.lossyScale.y + srcTrans.lossyScale.z) / 3f;
	}

	public static Vector3 GetTransformScaleMeanVector(Transform srcTrans)
	{
		float transformScaleMeanValue = NcTransformTool.GetTransformScaleMeanValue(srcTrans);
		return new Vector3(transformScaleMeanValue, transformScaleMeanValue, transformScaleMeanValue);
	}
}
