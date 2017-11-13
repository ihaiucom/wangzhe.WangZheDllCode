using System;
using UnityEngine;

public class SupportLogger : MonoBehaviour
{
	public bool LogTrafficStats = true;

	public void Start()
	{
		GameObject gameObject = GameObject.Find("PunSupportLogger");
		if (gameObject == null)
		{
			gameObject = new GameObject("PunSupportLogger");
			Object.DontDestroyOnLoad(gameObject);
			SupportLogging supportLogging = gameObject.AddComponent<SupportLogging>();
			supportLogging.LogTrafficStats = this.LogTrafficStats;
		}
	}
}
