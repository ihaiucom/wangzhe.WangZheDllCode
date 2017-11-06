using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MoveCam : MonoBehaviour
{
	private Vector3 originalPos;

	private Vector3 randomPos;

	private Transform camTransform;

	public Transform lookAt;

	private void Start()
	{
		this.camTransform = base.GetComponent<Camera>().transform;
		this.originalPos = this.camTransform.position;
		this.randomPos = this.originalPos + new Vector3((float)Random.Range(-2, 2), (float)Random.Range(-2, 2), (float)Random.Range(-1, 1));
	}

	private void Update()
	{
		this.camTransform.position = Vector3.Slerp(this.camTransform.position, this.randomPos, Time.deltaTime);
		this.camTransform.LookAt(this.lookAt);
		if (Vector3.Distance(this.camTransform.position, this.randomPos) < 0.5f)
		{
			this.randomPos = this.originalPos + new Vector3((float)Random.Range(-2, 2), (float)Random.Range(-2, 2), (float)Random.Range(-1, 1));
		}
	}
}
