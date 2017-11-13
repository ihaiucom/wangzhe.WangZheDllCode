using System;
using UnityEngine;

public class PunGameLogic : MonoBehaviour
{
	public static int playerWhoIsIt;

	private static PhotonView ScenePhotonView;

	public void Start()
	{
		PunGameLogic.ScenePhotonView = base.GetComponent<PhotonView>();
	}

	public void OnJoinedRoom()
	{
		if (PhotonNetwork.playerList.Length == 1)
		{
			PunGameLogic.playerWhoIsIt = PhotonNetwork.player.ID;
		}
		Debug.Log("playerWhoIsIt: " + PunGameLogic.playerWhoIsIt);
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerConnected: " + player);
		if (PhotonNetwork.isMasterClient)
		{
			PunGameLogic.TagPlayer(PunGameLogic.playerWhoIsIt);
		}
	}

	public static void TagPlayer(int playerID)
	{
		Debug.Log("TagPlayer: " + playerID);
		PunGameLogic.ScenePhotonView.RPC("TaggedPlayer", PhotonTargets.All, new object[]
		{
			playerID
		});
	}

	[PunRPC]
	public void TaggedPlayer(int playerID)
	{
		PunGameLogic.playerWhoIsIt = playerID;
		Debug.Log("TaggedPlayer: " + playerID);
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerDisconnected: " + player);
		if (PhotonNetwork.isMasterClient && player.ID == PunGameLogic.playerWhoIsIt)
		{
			PunGameLogic.TagPlayer(PhotonNetwork.player.ID);
		}
	}

	public void OnMasterClientSwitched()
	{
		Debug.Log("OnMasterClientSwitched");
	}
}
