using UnityEngine;
using System;
using System.Collections;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System.IO;

public class MyMain : Photon.PunBehaviour {

	[Tooltip("The maximum number of players per room")]
	public byte maxPlayersPerRoom = 11;  // 1 master client, players: 5 vs 5

	#region Private Variables
	/// <summary>
	/// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
	/// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
	/// Typically this is used for the OnConnectedToMaster() callback.
	/// </summary>
	bool isConnecting;
	
	/// <summary>
	/// This client's version number. Users are separated from each other by gameversion (which allows you to make breaking changes).
	/// </summary>
	string _gameVersion = "1";
	
	#endregion
	
	// Use this for initialization
	void Start () {
		PhotonNetwork.autoJoinLobby = false;
		PhotonNetwork.automaticallySyncScene = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Start the connection process. 
	/// - If already connected, we attempt joining a random room
	/// - if not yet connected, Connect this application instance to Photon Cloud Network
	/// </summary>
	public void Connect()
	{
		Debug.Log("[PUN] Call connect...");
		// keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
		isConnecting = true;
		
		// we check if we are connected or not, we join if we are , else we initiate the connection to the server.
		if (PhotonNetwork.connected)
		{
			// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
			PhotonNetwork.JoinRandomRoom();
			//PhotonNetwork.JoinRandomRoom(null,(byte)(2));
		}else{
			// #Critical, we must first and foremost connect to Photon Online Server.
			PhotonNetwork.ConnectUsingSettings(_gameVersion);
		}
	}

	#region Photon.PunBehaviour CallBacks
	// below, we implement some callbacks of PUN
	// you can find PUN's callbacks in the class PunBehaviour or in enum PhotonNetworkingMessage
	
	
	/// <summary>
	/// Called after the connection to the master is established and authenticated but only when PhotonNetwork.autoJoinLobby is false.
	/// </summary>
	public override void OnConnectedToMaster()
	{
		// we don't want to do anything if we are not attempting to join a room. 
		// this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
		// we don't want to do anything.
		if (isConnecting)
		{
			Debug.Log("[PUN] OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.");
			
			// #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()
			PhotonNetwork.JoinRandomRoom();
		}
	}
	
	/// <summary>
	/// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
	/// </summary>
	/// <remarks>
	/// Most likely all rooms are full or no rooms are available. <br/>
	/// </remarks>
	/// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg.</param>
	public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
	{
		Debug.Log("[PUN] OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.");
		
		// #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
		PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = this.maxPlayersPerRoom}, null);
	}
	
	
	/// <summary>
	/// Called after disconnecting from the Photon server.
	/// </summary>
	/// <remarks>
	/// In some cases, other callbacks are called before OnDisconnectedFromPhoton is called.
	/// Examples: OnConnectionFail() and OnFailedToConnectToPhoton().
	/// </remarks>
	public override void OnDisconnectedFromPhoton()
	{
		Debug.LogError("[PUN] Disconnected");
		
		isConnecting = false;
	}
	
	/// <summary>
	/// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
	/// </summary>
	/// <remarks>
	/// This method is commonly used to instantiate player characters.
	/// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
	///
	/// When this is called, you can usually already access the existing players in the room via PhotonNetwork.playerList.
	/// Also, all custom properties should be already available as Room.customProperties. Check Room..PlayerCount to find out if
	/// enough players are in the room to start playing.
	/// </remarks>
	public override void OnJoinedRoom()
	{
		Debug.Log("[PUN] OnJoinedRoom() called by PUN. Now this client is in a room.");
		Debug.Log(string.Format("Player count: {0}", PhotonNetwork.room.PlayerCount));
	}

	#endregion
}
