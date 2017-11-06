using System;
using UnityEngine;

public class Canvas3D : MonoBehaviour
{
	public void Awake()
	{
		this.Reset();
	}

	public void OnDestroy()
	{
		this.Clear();
	}

	public void Reset()
	{
		Singleton<Canvas3DImpl>.GetInstance().Reset();
	}

	public void Clear()
	{
		Singleton<Canvas3DImpl>.GetInstance().Clear();
	}

	public void LateUpdate()
	{
		Singleton<Canvas3DImpl>.GetInstance().Update(base.transform);
	}
}
