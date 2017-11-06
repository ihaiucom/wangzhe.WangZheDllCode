using Photon;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorkerInGame : Photon.MonoBehaviour
{
	public Transform playerPrefab;

	public void Awake()
	{
		if (!PhotonNetwork.connected)
		{
			SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
			return;
		}
		PhotonNetwork.Instantiate(this.playerPrefab.name, base.transform.position, Quaternion.identity, 0);
	}

	public void OnGUI()
	{
		if (GUILayout.Button("Return to Lobby", new GUILayoutOption[0]))
		{
			PhotonNetwork.LeaveRoom();
		}
	}

	public void OnMasterClientSwitched(PhotonPlayer player)
	{
		Debug.Log("OnMasterClientSwitched: " + player);
		InRoomChat component = base.GetComponent<InRoomChat>();
		if (component != null)
		{
			string newLine;
			if (player.IsLocal)
			{
				newLine = "You are Master Client now.";
			}
			else
			{
				newLine = player.NickName + " is Master Client now.";
			}
			component.AddLine(newLine);
		}
	}

	public void OnLeftRoom()
	{
		Debug.Log("OnLeftRoom (local)");
		SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
	}

	public void OnDisconnectedFromPhoton()
	{
		Debug.Log("OnDisconnectedFromPhoton");
		SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
	}

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		Debug.Log("OnPhotonInstantiate " + info.sender);
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerConnected: " + player);
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		Debug.Log("OnPlayerDisconneced: " + player);
	}

	public void OnFailedToConnectToPhoton()
	{
		Debug.Log("OnFailedToConnectToPhoton");
		SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
	}
}
