using System;
using UnityEngine;

public class RandomRotation : MonoBehaviour, IPooledMonoBehaviour
{
	public float rotationMaxX;

	public float rotationMaxY = 360f;

	public float rotationMaxZ;

	private void Update()
	{
		float xAngle = UnityEngine.Random.Range(-this.rotationMaxX, this.rotationMaxX);
		float yAngle = UnityEngine.Random.Range(-this.rotationMaxY, this.rotationMaxY);
		float zAngle = UnityEngine.Random.Range(-this.rotationMaxZ, this.rotationMaxZ);
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
