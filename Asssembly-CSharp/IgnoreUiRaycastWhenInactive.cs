using System;
using UnityEngine;

public class IgnoreUiRaycastWhenInactive : MonoBehaviour, ICanvasRaycastFilter
{
	public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
	{
		return base.gameObject.activeInHierarchy;
	}
}
