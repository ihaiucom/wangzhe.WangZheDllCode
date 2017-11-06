using System;
using UnityEngine;

public class Demo2DJumpAndRun : MonoBehaviour
{
	private void OnJoinedRoom()
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		PhotonNetwork.InstantiateSceneObject("Physics Box", new Vector3(-4.5f, 5.5f, 0f), Quaternion.identity, 0, null);
		PhotonNetwork.InstantiateSceneObject("Physics Box", new Vector3(-4.5f, 4.5f, 0f), Quaternion.identity, 0, null);
		PhotonNetwork.InstantiateSceneObject("Physics Box", new Vector3(-4.5f, 3.5f, 0f), Quaternion.identity, 0, null);
		PhotonNetwork.InstantiateSceneObject("Physics Box", new Vector3(4.5f, 5.5f, 0f), Quaternion.identity, 0, null);
		PhotonNetwork.InstantiateSceneObject("Physics Box", new Vector3(4.5f, 4.5f, 0f), Quaternion.identity, 0, null);
		PhotonNetwork.InstantiateSceneObject("Physics Box", new Vector3(4.5f, 3.5f, 0f), Quaternion.identity, 0, null);
	}
}
