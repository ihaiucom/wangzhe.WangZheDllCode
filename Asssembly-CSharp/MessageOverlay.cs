using System;
using UnityEngine;

public class MessageOverlay : MonoBehaviour
{
	public GameObject[] Objects;

	public void Start()
	{
		this.SetActive(true);
	}

	public void OnJoinedRoom()
	{
		this.SetActive(false);
	}

	public void OnLeftRoom()
	{
		this.SetActive(true);
	}

	private void SetActive(bool enable)
	{
		GameObject[] objects = this.Objects;
		for (int i = 0; i < objects.Length; i++)
		{
			GameObject gameObject = objects[i];
			gameObject.SetActive(enable);
		}
	}
}
