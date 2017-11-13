using System;
using UnityEngine;

public class RotateInWorldSpace : MonoBehaviour
{
	public Vector3 initAngle;

	public Vector3 speedPerSecond;

	private Vector3 currentAngle;

	private Vector3 speedAnglePerFrame;

	private void OnEnable()
	{
		this.currentAngle = this.initAngle;
		int targetFrameRate = Application.targetFrameRate;
		this.speedAnglePerFrame = this.speedPerSecond / (float)targetFrameRate;
	}

	private void LateUpdate()
	{
		this.currentAngle += this.speedAnglePerFrame;
		base.gameObject.transform.rotation = Quaternion.Euler(this.currentAngle);
	}
}
