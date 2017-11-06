using System;
using UnityEngine;

[Serializable]
public class PhotonTransformViewPositionModel
{
	public enum InterpolateOptions
	{
		Disabled,
		FixedSpeed,
		EstimatedSpeed,
		SynchronizeValues,
		Lerp
	}

	public enum ExtrapolateOptions
	{
		Disabled,
		SynchronizeValues,
		EstimateSpeedAndTurn,
		FixedSpeed
	}

	public bool SynchronizeEnabled;

	public bool TeleportEnabled = true;

	public float TeleportIfDistanceGreaterThan = 3f;

	public PhotonTransformViewPositionModel.InterpolateOptions InterpolateOption = PhotonTransformViewPositionModel.InterpolateOptions.EstimatedSpeed;

	public float InterpolateMoveTowardsSpeed = 1f;

	public float InterpolateLerpSpeed = 1f;

	public float InterpolateMoveTowardsAcceleration = 2f;

	public float InterpolateMoveTowardsDeceleration = 2f;

	public AnimationCurve InterpolateSpeedCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(-1f, 0f, 0f, float.PositiveInfinity),
		new Keyframe(0f, 1f, 0f, 0f),
		new Keyframe(1f, 1f, 0f, 1f),
		new Keyframe(4f, 4f, 1f, 0f)
	});

	public PhotonTransformViewPositionModel.ExtrapolateOptions ExtrapolateOption;

	public float ExtrapolateSpeed = 1f;

	public bool ExtrapolateIncludingRoundTripTime = true;

	public int ExtrapolateNumberOfStoredPositions = 1;

	public bool DrawErrorGizmo = true;
}
