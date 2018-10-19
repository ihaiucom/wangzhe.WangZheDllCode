using System;
using UnityEngine;

public class RotateThis : MonoBehaviour, IPooledMonoBehaviour
{
	public float rotationSpeedX = 90f;

	public float rotationSpeedY;

	public float rotationSpeedZ;

	private void Update()
	{
		base.transform.Rotate(this.rotationSpeedX * Time.deltaTime, this.rotationSpeedY * Time.deltaTime, this.rotationSpeedZ * Time.deltaTime);
	}

	public void OnCreate()
	{
	}

	public void OnGet()
	{
	}

	public void OnRecycle()
	{
	}
}
