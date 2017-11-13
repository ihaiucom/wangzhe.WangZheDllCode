using Photon;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ExitGames.Demos.DemoAnimator
{
	public class GameManager : Photon.MonoBehaviour
	{
		public static GameManager Instance;

		[Tooltip("The prefab to use for representing the player")]
		public GameObject playerPrefab;

		private GameObject instance;

		private void Start()
		{
			GameManager.Instance = this;
			if (!PhotonNetwork.connected)
			{
				SceneManager.LoadScene("PunBasics-Launcher");
				return;
			}
			if (this.playerPrefab == null)
			{
				Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
			}
			else if (PlayerManager.LocalPlayerInstance == null)
			{
				Debug.Log("We are Instantiating LocalPlayer from " + SceneManagerHelper.ActiveSceneName);
				PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
			}
			else
			{
				Debug.Log("Ignoring scene load for " + SceneManagerHelper.ActiveSceneName);
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				this.QuitApplication();
			}
		}

		public void OnPhotonPlayerConnected(PhotonPlayer other)
		{
			Debug.Log("OnPhotonPlayerConnected() " + other.NickName);
			if (PhotonNetwork.isMasterClient)
			{
				Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);
				this.LoadArena();
			}
		}

		public void OnPhotonPlayerDisconnected(PhotonPlayer other)
		{
			Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName);
			if (PhotonNetwork.isMasterClient)
			{
				Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);
				this.LoadArena();
			}
		}

		public virtual void OnLeftRoom()
		{
			SceneManager.LoadScene("PunBasics-Launcher");
		}

		public void LeaveRoom()
		{
			PhotonNetwork.LeaveRoom();
		}

		public void QuitApplication()
		{
			Application.Quit();
		}

		private void LoadArena()
		{
			if (!PhotonNetwork.isMasterClient)
			{
				Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
			}
			Debug.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);
			PhotonNetwork.LoadLevel("PunBasics-Room for " + PhotonNetwork.room.PlayerCount);
		}
	}
}
