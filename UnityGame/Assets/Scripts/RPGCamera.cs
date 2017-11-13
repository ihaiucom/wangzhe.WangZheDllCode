using System;
using UnityEngine;

public class RPGCamera : MonoBehaviour
{
	public Transform Target;

	public float MaximumDistance;

	public float MinimumDistance;

	public float ScrollModifier;

	public float TurnModifier;

	private Transform m_CameraTransform;

	private Vector3 m_LookAtPoint;

	private Vector3 m_LocalForwardVector;

	private float m_Distance;

	private void Start()
	{
		this.m_CameraTransform = base.transform.GetChild(0);
		this.m_LocalForwardVector = this.m_CameraTransform.forward;
		this.m_Distance = -this.m_CameraTransform.localPosition.z / this.m_CameraTransform.forward.z;
		this.m_Distance = Mathf.Clamp(this.m_Distance, this.MinimumDistance, this.MaximumDistance);
		this.m_LookAtPoint = this.m_CameraTransform.localPosition + this.m_LocalForwardVector * this.m_Distance;
	}

	private void LateUpdate()
	{
		this.UpdateDistance();
		this.UpdateZoom();
		this.UpdatePosition();
		this.UpdateRotation();
	}

	private void UpdateDistance()
	{
		this.m_Distance = Mathf.Clamp(this.m_Distance - Input.GetAxis("Mouse ScrollWheel") * this.ScrollModifier, this.MinimumDistance, this.MaximumDistance);
	}

	private void UpdateZoom()
	{
		this.m_CameraTransform.localPosition = this.m_LookAtPoint - this.m_LocalForwardVector * this.m_Distance;
	}

	private void UpdatePosition()
	{
		if (this.Target == null)
		{
			return;
		}
		base.transform.position = this.Target.transform.position;
	}

	private void UpdateRotation()
	{
		if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetButton("Fire1") || Input.GetButton("Fire2"))
		{
			base.transform.Rotate(0f, Input.GetAxis("Mouse X") * this.TurnModifier, 0f);
		}
		if ((Input.GetMouseButton(1) || Input.GetButton("Fire2")) && this.Target != null)
		{
			this.Target.rotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
		}
	}
}
