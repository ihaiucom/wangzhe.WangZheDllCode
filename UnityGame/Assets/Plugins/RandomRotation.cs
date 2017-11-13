using System;
using UnityEngine;

public class RandomRotation : MonoBehaviour, IPooledMonoBehaviour
{
	public float rotationMaxX;

	public float rotationMaxY = 360f;

	public float rotationMaxZ;

	private void Update()
	{
		float xAngle = Random.Range(-this.rotationMaxX, this.rotationMaxX);
		float yAngle = Random.Range(-this.rotationMaxY, this.rotationMaxY);
		float zAngle = Random.Range(-this.rotationMaxZ, this.rotationMaxZ);
		base.transform.Rotate(xAngle, yAngle, zAngle);
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
