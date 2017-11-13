using System;

[Serializable]
public class Moba_Camera_Settings_Movement
{
	public float lockTransitionRate = 1f;

	public float cameraMovementRate = 1f;

	public bool edgeHoverMovement = true;

	public float edgeHoverOffset = 5f;

	public float defualtHeight;

	public bool useDefualtHeight = true;

	public bool useLockTargetHeight = true;
}
