using System;
using UnityEngine;
using UnityEngine.UI;

public class ImageBlockRaycast : Image
{
	[SerializeField]
	public bool blocksRaycasts;

	public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
	{
		return this.blocksRaycasts && base.IsRaycastLocationValid(screenPoint, eventCamera);
	}
}
