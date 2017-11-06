using System;
using UnityEngine;

public class Rotation : MonoBehaviour
{
	public int rotateSpeed;

	public int rotateDirection = 1;

	private void Update()
	{
		base.gameObject.transform.Rotate(Vector3.forward * Time.deltaTime * (float)this.rotateSpeed * (float)this.rotateDirection);
	}
}
