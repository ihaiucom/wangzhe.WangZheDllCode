using System;
using UnityEngine;

public class WorkerMenu : MonoBehaviour
{
	public GUISkin Skin;

	public Vector2 WidthAndHeight = new Vector2(600f, 400f);

	private string roomName = "myRoom";

	private Vector2 scrollPos = Vector2.zero;

	private bool connectFailed;

	public static readonly string SceneNameMenu = "DemoWorker-Scene";

	public static readonly string SceneNameGame = "DemoWorkerGame-Scene";

	private string errorDialog;

	private double timeToClearDialog;

	public string ErrorDialog
	{
		get
		{
			return this.errorDialog;
		}
		private set
		{
			this.errorDialog = value;
			if (!string.IsNullOrEmpty(value))
			{
				this.timeToClearDialog = (double)(Time.time + 4f);
			}
		}
	}

	public void Awake()
	{
		PhotonNetwork.automaticallySyncScene = true;
		if (PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated)
		{
			PhotonNetwork.ConnectUsingSettings("0.9");
		}
		if (string.IsNullOrEmpty(PhotonNetwork.playerName))
		{
			PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
		}
	}

	public void OnGUI()
	{
		if (this.Skin != null)
		{
			GUI.skin = this.Skin;
		}
		if (!PhotonNetwork.connected)
		{
			if (PhotonNetwork.connecting)
			{
				GUILayout.Label("Connecting to: " + PhotonNetwork.ServerAddress, new GUILayoutOption[0]);
			}
			else
			{
				GUILayout.Label(string.Concat(new object[]
				{
					"Not connected. Check console output. Detailed connection state: ",
					PhotonNetwork.connectionStateDetailed,
					" Server: ",
					PhotonNetwork.ServerAddress
				}), new GUILayoutOption[0]);
			}
			if (this.connectFailed)
			{
				GUILayout.Label("Connection failed. Check setup and use Setup Wizard to fix configuration.", new GUILayoutOption[0]);
				GUILayout.Label(string.Format("Server: {0}", new object[]
				{
					PhotonNetwork.ServerAddress
				}), new GUILayoutOption[0]);
				GUILayout.Label("AppId: " + PhotonNetwork.PhotonServerSettings.AppID.Substring(0, 8) + "****", new GUILayoutOption[0]);
				if (GUILayout.Button("Try Again", new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				}))
				{
					this.connectFailed = false;
					PhotonNetwork.ConnectUsingSettings("0.9");
				}
			}
			return;
		}
		Rect rect = new Rect(((float)Screen.width - this.WidthAndHeight.x) / 2f, ((float)Screen.height - this.WidthAndHeight.y) / 2f, this.WidthAndHeight.x, this.WidthAndHeight.y);
		GUI.Box(rect, "Join or Create Room");
		GUILayout.BeginArea(rect);
		GUILayout.Space(40f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Player name:", new GUILayoutOption[]
		{
			GUILayout.Width(150f)
		});
		PhotonNetwork.playerName = GUILayout.TextField(PhotonNetwork.playerName, new GUILayoutOption[0]);
		GUILayout.Space(158f);
		if (GUI.changed)
		{
			PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(15f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Roomname:", new GUILayoutOption[]
		{
			GUILayout.Width(150f)
		});
		this.roomName = GUILayout.TextField(this.roomName, new GUILayoutOption[0]);
		if (GUILayout.Button("Create Room", new GUILayoutOption[]
		{
			GUILayout.Width(150f)
		}))
		{
			PhotonNetwork.CreateRoom(this.roomName, new RoomOptions
			{
				MaxPlayers = 10
			}, null);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Join Room", new GUILayoutOption[]
		{
			GUILayout.Width(150f)
		}))
		{
			PhotonNetwork.JoinRoom(this.roomName);
		}
		GUILayout.EndHorizontal();
		if (!string.IsNullOrEmpty(this.ErrorDialog))
		{
			GUILayout.Label(this.ErrorDialog, new GUILayoutOption[0]);
			if (this.timeToClearDialog < (double)Time.time)
			{
				this.timeToClearDialog = 0.0;
				this.ErrorDialog = string.Empty;
			}
		}
		GUILayout.Space(15f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label(string.Concat(new object[]
		{
			PhotonNetwork.countOfPlayers,
			" users are online in ",
			PhotonNetwork.countOfRooms,
			" rooms."
		}), new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Join Random", new GUILayoutOption[]
		{
			GUILayout.Width(150f)
		}))
		{
			PhotonNetwork.JoinRandomRoom();
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(15f);
		if (PhotonNetwork.GetRoomList().Length == 0)
		{
			GUILayout.Label("Currently no games are available.", new GUILayoutOption[0]);
			GUILayout.Label("Rooms will be listed here, when they become available.", new GUILayoutOption[0]);
		}
		else
		{
			GUILayout.Label(PhotonNetwork.GetRoomList().Length + " rooms available:", new GUILayoutOption[0]);
			this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, new GUILayoutOption[0]);
			RoomInfo[] roomList = PhotonNetwork.GetRoomList();
			for (int i = 0; i < roomList.Length; i++)
			{
				RoomInfo roomInfo = roomList[i];
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label(string.Concat(new object[]
				{
					roomInfo.Name,
					" ",
					roomInfo.PlayerCount,
					"/",
					roomInfo.MaxPlayers
				}), new GUILayoutOption[0]);
				if (GUILayout.Button("Join", new GUILayoutOption[]
				{
					GUILayout.Width(150f)
				}))
				{
					PhotonNetwork.JoinRoom(roomInfo.Name);
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
		}
		GUILayout.EndArea();
	}

	public void OnJoinedRoom()
	{
		Debug.Log("OnJoinedRoom");
	}

	public void OnPhotonCreateRoomFailed()
	{
		this.ErrorDialog = "Error: Can't create room (room name maybe already used).";
		Debug.Log("OnPhotonCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
	}

	public void OnPhotonJoinRoomFailed(object[] cause)
	{
		this.ErrorDialog = "Error: Can't join room (full or unknown room name). " + cause[1];
		Debug.Log("OnPhotonJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
	}

	public void OnPhotonRandomJoinFailed()
	{
		this.ErrorDialog = "Error: Can't join random room (none found).";
		Debug.Log("OnPhotonRandomJoinFailed got called. Happens if no room is available (or all full or invisible or closed). JoinrRandom filter-options can limit available rooms.");
	}

	public void OnCreatedRoom()
	{
		Debug.Log("OnCreatedRoom");
		PhotonNetwork.LoadLevel(WorkerMenu.SceneNameGame);
	}

	public void OnDisconnectedFromPhoton()
	{
		Debug.Log("Disconnected from Photon.");
	}

	public void OnFailedToConnectToPhoton(object parameters)
	{
		this.connectFailed = true;
		Debug.Log(string.Concat(new object[]
		{
			"OnFailedToConnectToPhoton. StatusCode: ",
			parameters,
			" ServerAddress: ",
			PhotonNetwork.ServerAddress
		}));
	}

	public void OnConnectedToMaster()
	{
		Debug.Log("As OnConnectedToMaster() got called, the PhotonServerSetting.AutoJoinLobby must be off. Joining lobby by calling PhotonNetwork.JoinLobby().");
		PhotonNetwork.JoinLobby();
	}
}
