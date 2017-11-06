using System;
using UnityEngine;

public class RandomScale : MonoBehaviour, IPooledMonoBehaviour
{
	private bool bGetted;

	public float minScale = 1f;

	public float maxScale = 2f;

	private void Start()
	{
		this.OnGet();
	}

	public void OnCreate()
	{
	}

	public void OnGet()
	{
		if (!this.bGetted)
		{
			this.bGetted = true;
			float num = Random.Range(this.minScale, this.maxScale);
			base.transform.localScale = new Vector3(num, num, num);
		}
	}

	public void OnRecycle()
	{
		this.bGetted = false;
	}
}
