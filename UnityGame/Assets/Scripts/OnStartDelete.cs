using System;
using UnityEngine;

public class OnStartDelete : MonoBehaviour
{
	private void Start()
	{
		Object.Destroy(base.gameObject);
	}
}
