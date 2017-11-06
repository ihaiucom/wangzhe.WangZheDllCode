using System;
using UnityEngine;

[Serializable]
public class Moba_Camera_Settings_Rotation
{
	public bool constRotationRate;

	public bool lockRotationX = true;

	public bool lockRotationY = true;

	public Vector2 defualtRotation = new Vector2(-45f, 0f);

	public Vector2 cameraRotationRate = new Vector2(100f, 100f);
}
