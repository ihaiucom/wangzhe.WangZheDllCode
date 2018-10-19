using System;
using UnityEngine;

public class Delay : MonoBehaviour, IPooledMonoBehaviour
{
	public float delayTime = 1f;

	private bool _started;

	public void OnCreate()
	{
	}

	public void OnGet()
	{
		if (this._started)
		{
			return;
		}
		this.DoStart();
	}

	public void OnRecycle()
	{
		this._started = false;
	}

	private void Start()
	{
		if (this._started)
		{
			return;
		}
		this.DoStart();
	}

	private void DoStart()
	{
		base.gameObject.SetActive(false);
		base.Invoke("DelayFunc", this.delayTime);
		this._started = true;
	}

	private void DelayFunc()
	{
		base.gameObject.SetActive(true);
	}
}
