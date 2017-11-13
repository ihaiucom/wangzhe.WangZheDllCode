using System;
using UnityEngine;

public class DemoRPGMovement : MonoBehaviour
{
	public RPGCamera Camera;

	private void OnJoinedRoom()
	{
		this.CreatePlayerObject();
	}

	private void CreatePlayerObject()
	{
		Vector3 position = new Vector3(33.5f, 1.5f, 20.5f);
		GameObject gameObject = PhotonNetwork.Instantiate("Robot Kyle RPG", position, Quaternion.identity, 0);
		this.Camera.Target = gameObject.transform;
	}
}
