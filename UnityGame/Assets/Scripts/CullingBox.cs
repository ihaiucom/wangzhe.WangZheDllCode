using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class CullingBox : MonoBehaviour
{
	public Vector3 center;

	public Vector3 size;

	private MeshFilter mesh;

	private void Awake()
	{
		this.mesh = base.GetComponent<MeshFilter>();
		this.ChangeBounds();
	}

	private void ChangeBounds()
	{
		Bounds bounds = default(Bounds);
		bounds.center = this.center;
		bounds.size = this.size;
		this.mesh.mesh.bounds = bounds;
	}

	private void OnDrawGizmos()
	{
		Bounds bounds = this.mesh.mesh.bounds;
		Vector3 vector = base.transform.TransformPoint(bounds.center + bounds.extents);
		Vector3 from = base.transform.TransformPoint(bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1f, 1f, 1f)));
		Vector3 vector2 = base.transform.TransformPoint(bounds.center + Vector3.Scale(bounds.extents, new Vector3(1f, 1f, -1f)));
		Vector3 vector3 = base.transform.TransformPoint(bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1f, 1f, -1f)));
		Vector3 vector4 = base.transform.TransformPoint(bounds.center + Vector3.Scale(bounds.extents, new Vector3(1f, -1f, 1f)));
		Vector3 vector5 = base.transform.TransformPoint(bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1f, -1f, 1f)));
		Vector3 to = base.transform.TransformPoint(bounds.center + Vector3.Scale(bounds.extents, new Vector3(1f, -1f, -1f)));
		Vector3 vector6 = base.transform.TransformPoint(bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1f, -1f, -1f)));
		Gizmos.color = Color.white;
		Gizmos.DrawLine(from, vector);
		Gizmos.DrawLine(vector5, vector4);
		Gizmos.DrawLine(vector3, vector2);
		Gizmos.DrawLine(vector6, to);
		Gizmos.DrawLine(from, vector3);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector5, vector6);
		Gizmos.DrawLine(vector4, to);
		Gizmos.DrawLine(from, vector5);
		Gizmos.DrawLine(vector, vector4);
		Gizmos.DrawLine(vector3, vector6);
		Gizmos.DrawLine(vector2, to);
	}
}
