using System;

[Serializable]
public class PhotonTransformViewScaleModel
{
	public enum InterpolateOptions
	{
		Disabled,
		MoveTowards,
		Lerp
	}

	public bool SynchronizeEnabled;

	public PhotonTransformViewScaleModel.InterpolateOptions InterpolateOption;

	public float InterpolateMoveTowardsSpeed = 1f;

	public float InterpolateLerpSpeed;
}
