using System;
using UnityEngine;

public class UV_Rotate : MonoBehaviour
{
	public int rotateSpeed = 30;

	public Texture texture;

	public Vector2 rotationCenter = Vector2.zero;

	private void Start()
	{
		Material material = new Material(Shader.Find("Rotating Texture"));
		material.mainTexture = this.texture;
		base.GetComponent<Renderer>().material = material;
	}

	private void Update()
	{
		Quaternion q = Quaternion.Euler(0f, 0f, Time.time * (float)this.rotateSpeed);
		Matrix4x4 rhs = Matrix4x4.TRS(Vector3.zero, q, new Vector3(1f, 1f, 1f));
		Matrix4x4 rhs2 = Matrix4x4.TRS(-this.rotationCenter, Quaternion.identity, new Vector3(1f, 1f, 1f));
		Matrix4x4 lhs = Matrix4x4.TRS(this.rotationCenter, Quaternion.identity, new Vector3(1f, 1f, 1f));
		base.GetComponent<Renderer>().material.SetMatrix("_Rotation", lhs * rhs * rhs2);
	}
}
