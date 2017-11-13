using Photon;
using System;
using UnityEngine;

public class RandomMatchmaker : PunBehaviour
{
	private PhotonView myPhotonView;

	public void Start()
	{
		PhotonNetwork.ConnectUsingSettings("0.1");
	}

	public override void OnJoinedLobby()
	{
		Debug.Log("JoinRandom");
		PhotonNetwork.JoinRandomRoom();
	}

	public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinRandomRoom();
	}

	public void OnPhotonRandomJoinFailed()
	{
		PhotonNetwork.CreateRoom(null);
	}

	public override void OnJoinedRoom()
	{
		GameObject gameObject = PhotonNetwork.Instantiate("monsterprefab", Vector3.zero, Quaternion.identity, 0);
		gameObject.GetComponent<myThirdPersonController>().isControllable = true;
		this.myPhotonView = gameObject.GetComponent<PhotonView>();
	}

	public void OnGUI()
	{
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString(), new GUILayoutOption[0]);
		if (PhotonNetwork.inRoom)
		{
			bool flag = PunGameLogic.playerWhoIsIt == PhotonNetwork.player.ID;
			if (flag && GUILayout.Button("Marco!", new GUILayoutOption[0]))
			{
				this.myPhotonView.RPC("Marco", PhotonTargets.All, new object[0]);
			}
			if (!flag && GUILayout.Button("Polo!", new GUILayoutOption[0]))
			{
				this.myPhotonView.RPC("Polo", PhotonTargets.All, new object[0]);
			}
		}
	}
}
