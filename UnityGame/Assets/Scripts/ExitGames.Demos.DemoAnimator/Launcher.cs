using Photon;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoAnimator
{
	public class Launcher : PunBehaviour
	{
		[Tooltip("The Ui Panel to let the user enter name, connect and play")]
		public GameObject controlPanel;

		[Tooltip("The Ui Text to inform the user about the connection progress")]
		public Text feedbackText;

		[Tooltip("The maximum number of players per room")]
		public byte maxPlayersPerRoom = 4;

		[Tooltip("The UI Loader Anime")]
		public LoaderAnime loaderAnime;

		private bool isConnecting;

		private string _gameVersion = "1";

		private void Awake()
		{
			if (this.loaderAnime == null)
			{
				Debug.LogError("<Color=Red><b>Missing</b></Color> loaderAnime Reference.", this);
			}
			PhotonNetwork.autoJoinLobby = false;
			PhotonNetwork.automaticallySyncScene = true;
		}

		public void Connect()
		{
			this.feedbackText.set_text(string.Empty);
			this.isConnecting = true;
			this.controlPanel.SetActive(false);
			if (this.loaderAnime != null)
			{
				this.loaderAnime.StartLoaderAnimation();
			}
			if (PhotonNetwork.connected)
			{
				this.LogFeedback("Joining Room...");
				PhotonNetwork.JoinRandomRoom();
				PhotonNetwork.JoinRandomRoom(null, 2);
			}
			else
			{
				this.LogFeedback("Connecting...");
				PhotonNetwork.ConnectUsingSettings(this._gameVersion);
			}
		}

		private void LogFeedback(string message)
		{
			if (this.feedbackText == null)
			{
				return;
			}
			Text expr_18 = this.feedbackText;
			expr_18.set_text(expr_18.get_text() + Environment.get_NewLine() + message);
		}

		public override void OnConnectedToMaster()
		{
			Debug.Log("Region:" + PhotonNetwork.networkingPeer.CloudRegion);
			if (this.isConnecting)
			{
				this.LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
				Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");
				PhotonNetwork.JoinRandomRoom();
			}
		}

		public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
		{
			this.LogFeedback("<Color=Red>OnPhotonRandomJoinFailed</Color>: Next -> Create a new Room");
			Debug.Log("DemoAnimator/Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
			PhotonNetwork.CreateRoom(null, new RoomOptions
			{
				MaxPlayers = this.maxPlayersPerRoom
			}, null);
		}

		public override void OnDisconnectedFromPhoton()
		{
			this.LogFeedback("<Color=Red>OnDisconnectedFromPhoton</Color>");
			Debug.LogError("DemoAnimator/Launcher:Disconnected");
			this.loaderAnime.StopLoaderAnimation();
			this.isConnecting = false;
			this.controlPanel.SetActive(true);
		}

		public override void OnJoinedRoom()
		{
			this.LogFeedback("<Color=Green>OnJoinedRoom</Color> with " + PhotonNetwork.room.PlayerCount + " Player(s)");
			Debug.Log("DemoAnimator/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
			if (PhotonNetwork.room.PlayerCount == 1)
			{
				Debug.Log("We load the 'Room for 1' ");
				PhotonNetwork.LoadLevel("PunBasics-Room for 1");
			}
		}
	}
}
