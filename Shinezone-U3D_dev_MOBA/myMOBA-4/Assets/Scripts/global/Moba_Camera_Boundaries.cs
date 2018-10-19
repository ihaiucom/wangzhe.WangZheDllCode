using System;
using UnityEngine;

public class Moba_Camera_Boundaries
{
	public enum BoundaryType
	{
		cube,
		sphere,
		none
	}

	public static string boundaryLayer = "mobaCameraBoundaryLayer";

	private static bool boundaryLayerExists = true;

	private static ListView<Moba_Camera_Boundary> cube_boundaries = new ListView<Moba_Camera_Boundary>();

	private static ListView<Moba_Camera_Boundary> sphere_boundaries = new ListView<Moba_Camera_Boundary>();

	public static int GetNumberOfBoundaries()
	{
		return Moba_Camera_Boundaries.cube_boundaries.Count + Moba_Camera_Boundaries.sphere_boundaries.Count;
	}

	public static bool AddBoundary(Moba_Camera_Boundary boundary, Moba_Camera_Boundaries.BoundaryType type)
	{
		if (boundary == null)
		{
			return false;
		}
		if (type == Moba_Camera_Boundaries.BoundaryType.cube)
		{
			Moba_Camera_Boundaries.cube_boundaries.Add(boundary);
			return true;
		}
		if (type == Moba_Camera_Boundaries.BoundaryType.sphere)
		{
			Moba_Camera_Boundaries.sphere_boundaries.Add(boundary);
			return true;
		}
		return false;
	}

	public static bool RemoveBoundary(Moba_Camera_Boundary boundary, Moba_Camera_Boundaries.BoundaryType type)
	{
		if (type == Moba_Camera_Boundaries.BoundaryType.cube)
		{
			return Moba_Camera_Boundaries.cube_boundaries.Remove(boundary);
		}
		return type == Moba_Camera_Boundaries.BoundaryType.sphere && Moba_Camera_Boundaries.cube_boundaries.Remove(boundary);
	}

	public static void SetBoundaryLayerExist(bool value)
	{
		if (Moba_Camera_Boundaries.boundaryLayerExists)
		{
			Moba_Camera_Boundaries.boundaryLayerExists = false;
		}
	}

	private static Vector3 calBoxRelations(BoxCollider box, Vector3 point, bool containedToBox, out bool isPointInBox)
	{
		Vector3 b = box.transform.position + box.center;
		float num = box.size.x / 2f * box.transform.localScale.x;
		float num2 = box.size.y / 2f * box.transform.localScale.y;
		float num3 = box.size.z / 2f * box.transform.localScale.z;
		float d = Vector3.Dot(point - b, box.transform.up);
		Vector3 vector = point + d * -box.transform.up;
		float d2 = Vector3.Dot(vector - b, box.transform.right);
		Vector3 vector2 = vector + d2 * -box.transform.right;
		Vector3 rhs = vector2 - b;
		Vector3 rhs2 = point - vector;
		Vector3 rhs3 = vector - vector2;
		float num4 = rhs.magnitude;
		float num5 = rhs2.magnitude;
		float num6 = rhs3.magnitude;
		isPointInBox = true;
		if (num4 > num3)
		{
			if (containedToBox)
			{
				num4 = num3;
			}
			isPointInBox = false;
		}
		if (num5 > num2)
		{
			if (containedToBox)
			{
				num5 = num2;
			}
			isPointInBox = false;
		}
		if (num6 > num)
		{
			if (containedToBox)
			{
				num6 = num;
			}
			isPointInBox = false;
		}
		num4 *= ((Vector3.Dot(box.transform.forward, rhs) < 0f) ? -1f : 1f);
		num5 *= ((Vector3.Dot(box.transform.up, rhs2) < 0f) ? -1f : 1f);
		num6 *= ((Vector3.Dot(box.transform.right, rhs3) < 0f) ? -1f : 1f);
		return new Vector3(num6, num5, num4);
	}

	private static Vector3 getClosestPointOnSurfaceBox(BoxCollider box, Vector3 point)
	{
		bool flag;
		Vector3 vector = Moba_Camera_Boundaries.calBoxRelations(box, point, true, out flag);
		return box.transform.position + box.transform.forward * vector.z + box.transform.right * vector.x + box.transform.up * vector.y;
	}

	public static bool isPointInBoundary(Vector3 point)
	{
		bool result = false;
		ListView<Moba_Camera_Boundary>.Enumerator enumerator = Moba_Camera_Boundaries.cube_boundaries.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Moba_Camera_Boundary current = enumerator.Current;
			if (current.isActive)
			{
				BoxCollider component = current.GetComponent<BoxCollider>();
				if (!(component == null))
				{
					bool flag;
					Moba_Camera_Boundaries.calBoxRelations(component, point, false, out flag);
					if (flag)
					{
						result = true;
					}
				}
			}
		}
		ListView<Moba_Camera_Boundary>.Enumerator enumerator2 = Moba_Camera_Boundaries.sphere_boundaries.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			Moba_Camera_Boundary current2 = enumerator2.Current;
			if (current2.isActive)
			{
				SphereCollider component2 = current2.GetComponent<SphereCollider>();
				if (!(component2 == null))
				{
					if ((current2.transform.position + component2.center - point).magnitude < component2.radius)
					{
						result = true;
					}
				}
			}
		}
		return result;
	}

	public static Moba_Camera_Boundary GetClosestBoundary(Vector3 point)
	{
		Moba_Camera_Boundary result = null;
		float num = 999999f;
		ListView<Moba_Camera_Boundary>.Enumerator enumerator = Moba_Camera_Boundaries.cube_boundaries.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Moba_Camera_Boundary current = enumerator.Current;
			if (!(current == null))
			{
				if (current.isActive)
				{
					BoxCollider component = current.GetComponent<BoxCollider>();
					Vector3 closestPointOnSurfaceBox = Moba_Camera_Boundaries.getClosestPointOnSurfaceBox(component, point);
					float magnitude = (point - closestPointOnSurfaceBox).magnitude;
					if (magnitude < num)
					{
						result = current;
						num = magnitude;
					}
				}
			}
		}
		ListView<Moba_Camera_Boundary>.Enumerator enumerator2 = Moba_Camera_Boundaries.sphere_boundaries.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			Moba_Camera_Boundary current2 = enumerator2.Current;
			if (current2.isActive)
			{
				SphereCollider component2 = current2.GetComponent<SphereCollider>();
				Vector3 vector = current2.transform.position + component2.center;
				float radius = component2.radius;
				Vector3 b = vector + (point - vector).normalized * radius;
				float magnitude2 = (point - b).magnitude;
				if (magnitude2 < num)
				{
					result = current2;
					num = magnitude2;
				}
			}
		}
		return result;
	}

	public static Vector3 GetClosestPointOnBoundary(Moba_Camera_Boundary boundary, Vector3 point)
	{
		Vector3 result = point;
		if (boundary.type == Moba_Camera_Boundaries.BoundaryType.cube)
		{
			BoxCollider component = boundary.GetComponent<BoxCollider>();
			result = Moba_Camera_Boundaries.getClosestPointOnSurfaceBox(component, point);
		}
		else if (boundary.type == Moba_Camera_Boundaries.BoundaryType.sphere)
		{
			SphereCollider component2 = boundary.GetComponent<SphereCollider>();
			Vector3 vector = boundary.transform.position + component2.center;
			float radius = component2.radius;
			result = vector + (point - vector).normalized * radius;
		}
		return result;
	}
}
