using System;
using UnityEngine;

public static class GeometryUtilityUser
{
	private enum EPlaneSide
	{
		Left,
		Right,
		Bottom,
		Top,
		Near,
		Far
	}

	private static float[] RootVector = new float[4];

	private static float[] ComVector = new float[4];

	public static void CalculateFrustumPlanes(Camera InCamera, ref Plane[] OutPlanes)
	{
		Matrix4x4 projectionMatrix = InCamera.projectionMatrix;
		Matrix4x4 worldToCameraMatrix = InCamera.worldToCameraMatrix;
		Matrix4x4 matrix4x = projectionMatrix * worldToCameraMatrix;
		GeometryUtilityUser.RootVector[0] = matrix4x[3, 0];
		GeometryUtilityUser.RootVector[1] = matrix4x[3, 1];
		GeometryUtilityUser.RootVector[2] = matrix4x[3, 2];
		GeometryUtilityUser.RootVector[3] = matrix4x[3, 3];
		GeometryUtilityUser.ComVector[0] = matrix4x[0, 0];
		GeometryUtilityUser.ComVector[1] = matrix4x[0, 1];
		GeometryUtilityUser.ComVector[2] = matrix4x[0, 2];
		GeometryUtilityUser.ComVector[3] = matrix4x[0, 3];
		GeometryUtilityUser.CalcPlane(ref OutPlanes[0], GeometryUtilityUser.ComVector[0] + GeometryUtilityUser.RootVector[0], GeometryUtilityUser.ComVector[1] + GeometryUtilityUser.RootVector[1], GeometryUtilityUser.ComVector[2] + GeometryUtilityUser.RootVector[2], GeometryUtilityUser.ComVector[3] + GeometryUtilityUser.RootVector[3]);
		GeometryUtilityUser.CalcPlane(ref OutPlanes[1], -GeometryUtilityUser.ComVector[0] + GeometryUtilityUser.RootVector[0], -GeometryUtilityUser.ComVector[1] + GeometryUtilityUser.RootVector[1], -GeometryUtilityUser.ComVector[2] + GeometryUtilityUser.RootVector[2], -GeometryUtilityUser.ComVector[3] + GeometryUtilityUser.RootVector[3]);
		GeometryUtilityUser.ComVector[0] = matrix4x[1, 0];
		GeometryUtilityUser.ComVector[1] = matrix4x[1, 1];
		GeometryUtilityUser.ComVector[2] = matrix4x[1, 2];
		GeometryUtilityUser.ComVector[3] = matrix4x[1, 3];
		GeometryUtilityUser.CalcPlane(ref OutPlanes[2], GeometryUtilityUser.ComVector[0] + GeometryUtilityUser.RootVector[0], GeometryUtilityUser.ComVector[1] + GeometryUtilityUser.RootVector[1], GeometryUtilityUser.ComVector[2] + GeometryUtilityUser.RootVector[2], GeometryUtilityUser.ComVector[3] + GeometryUtilityUser.RootVector[3]);
		GeometryUtilityUser.CalcPlane(ref OutPlanes[3], -GeometryUtilityUser.ComVector[0] + GeometryUtilityUser.RootVector[0], -GeometryUtilityUser.ComVector[1] + GeometryUtilityUser.RootVector[1], -GeometryUtilityUser.ComVector[2] + GeometryUtilityUser.RootVector[2], -GeometryUtilityUser.ComVector[3] + GeometryUtilityUser.RootVector[3]);
		GeometryUtilityUser.ComVector[0] = matrix4x[2, 0];
		GeometryUtilityUser.ComVector[1] = matrix4x[2, 1];
		GeometryUtilityUser.ComVector[2] = matrix4x[2, 2];
		GeometryUtilityUser.ComVector[3] = matrix4x[2, 3];
		GeometryUtilityUser.CalcPlane(ref OutPlanes[4], GeometryUtilityUser.ComVector[0] + GeometryUtilityUser.RootVector[0], GeometryUtilityUser.ComVector[1] + GeometryUtilityUser.RootVector[1], GeometryUtilityUser.ComVector[2] + GeometryUtilityUser.RootVector[2], GeometryUtilityUser.ComVector[3] + GeometryUtilityUser.RootVector[3]);
		GeometryUtilityUser.CalcPlane(ref OutPlanes[5], -GeometryUtilityUser.ComVector[0] + GeometryUtilityUser.RootVector[0], -GeometryUtilityUser.ComVector[1] + GeometryUtilityUser.RootVector[1], -GeometryUtilityUser.ComVector[2] + GeometryUtilityUser.RootVector[2], -GeometryUtilityUser.ComVector[3] + GeometryUtilityUser.RootVector[3]);
	}

	private static void CalcPlane(ref Plane InPlane, float InA, float InB, float InC, float InDistance)
	{
		Vector3 vector = new Vector3(InA, InB, InC);
		float num = 1f / (float)Math.Sqrt((double)(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z));
		InPlane.normal = new Vector3(vector.x * num, vector.y * num, vector.z * num);
		InPlane.distance = InDistance * num;
	}
}
