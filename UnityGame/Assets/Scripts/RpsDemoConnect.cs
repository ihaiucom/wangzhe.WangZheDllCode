using Photon;
using System;
using UnityEngine;
using UnityEngine.UI;

public class RpsDemoConnect : PunBehaviour
{
	private const string MainSceneName = "DemoRPS-Scene";

	private const string NickNamePlayerPrefsKey = "NickName";

	public InputField InputField;

	public string UserId;

	private string previousRoomPlayerPrefKey = "PUN:Demo:RPS:PreviousRoom";

	public string previousRoom;

	private void Start()
	{
		this.InputField.set_text((!PlayerPrefs.HasKey("NickName")) ? string.Empty : PlayerPrefs.GetString("NickName"));
	}

	public void ApplyUserIdAndConnect()
	{
		string text = "DemoNick";
		if (this.InputField != null && !string.IsNullOrEmpty(this.InputField.get_text()))
		{
			text = this.InputField.get_text();
			PlayerPrefs.SetString("NickName", text);
		}
		if (PhotonNetwork.AuthValues == null)
		{
			PhotonNetwork.AuthValues = new AuthenticationValues();
		}
		PhotonNetwork.AuthValues.UserId = text;
		Debug.Log("Nickname: " + text + " userID: " + this.UserId, this);
		PhotonNetwork.playerName = text;
		PhotonNetwork.ConnectUsingSettings("0.5");
		PhotonHandler.StopFallbackSendAckThread();
	}

	public override void OnConnectedToMaster()
	{
		this.UserId = PhotonNetwork.player.UserId;
		if (PlayerPrefs.HasKey(this.previousRoomPlayerPrefKey))
		{
			Debug.Log("getting previous room from prefs: ");
			this.previousRoom = PlayerPrefs.GetString(this.previousRoomPlayerPrefKey);
			PlayerPrefs.DeleteKey(this.previousRoomPlayerPrefKey);
		}
		if (!string.IsNullOrEmpty(this.previousRoom))
		{
			Debug.Log("ReJoining previous room: " + this.previousRoom);
			PhotonNetwork.ReJoinRoom(this.previousRoom);
			this.previousRoom = null;
		}
		else
		{
			PhotonNetwork.JoinRandomRoom();
		}
	}

	public override void OnJoinedLobby()
	{
		this.OnConnectedToMaster();
	}

	public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
	{
		Debug.Log("OnPhotonRandomJoinFailed");
		PhotonNetwork.CreateRoom(null, new RoomOptions
		{
			MaxPlayers = 2,
			PlayerTtl = 20000
		}, null);
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("Joined room: " + PhotonNetwork.room.Name);
		this.previousRoom = PhotonNetwork.room.Name;
		PlayerPrefs.SetString(this.previousRoomPlayerPrefKey, this.previousRoom);
	}

	public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
	{
		Debug.Log("OnPhotonJoinRoomFailed");
		this.previousRoom = null;
		PlayerPrefs.DeleteKey(this.previousRoomPlayerPrefKey);
	}

	public override void OnConnectionFail(DisconnectCause cause)
	{
		Debug.Log(string.Concat(new object[]
		{
			"Disconnected due to: ",
			cause,
			". this.previousRoom: ",
			this.previousRoom
		}));
	}

	public override void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer)
	{
		Debug.Log(string.Concat(new object[]
		{
			"OnPhotonPlayerActivityChanged() for ",
			otherPlayer.NickName,
			" IsInactive: ",
			otherPlayer.IsInactive
		}));
	}
}
