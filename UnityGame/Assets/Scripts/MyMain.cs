using Photon;
using System;
using UnityEngine;

public class MyMain : PunBehaviour
{
	[Tooltip("The maximum number of players per room")]
	public byte maxPlayersPerRoom = 11;

	private bool isConnecting;

	private string _gameVersion = "1";

	private void Start()
	{
		PhotonNetwork.autoJoinLobby = false;
		PhotonNetwork.automaticallySyncScene = true;
	}

	private void Update()
	{
	}

	public void Connect()
	{
		Debug.Log("[PUN] Call connect...");
		this.isConnecting = true;
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.JoinRandomRoom();
		}
		else
		{
			PhotonNetwork.ConnectUsingSettings(this._gameVersion);
		}
	}

	public override void OnConnectedToMaster()
	{
		if (this.isConnecting)
		{
			Debug.Log("[PUN] OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.");
			PhotonNetwork.JoinRandomRoom();
		}
	}

	public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
	{
		Debug.Log("[PUN] OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.");
		PhotonNetwork.CreateRoom(null, new RoomOptions
		{
			MaxPlayers = this.maxPlayersPerRoom
		}, null);
	}

	public override void OnDisconnectedFromPhoton()
	{
		Debug.LogError("[PUN] Disconnected");
		this.isConnecting = false;
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("[PUN] OnJoinedRoom() called by PUN. Now this client is in a room.");
		Debug.Log(string.Format("Player count: {0}", PhotonNetwork.room.PlayerCount));
	}
}
