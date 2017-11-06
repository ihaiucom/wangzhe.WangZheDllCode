using ExitGames.UtilityScripts;
using System;
using UnityEngine;

public class DemoOwnershipGui : MonoBehaviour
{
	public GUISkin Skin;

	public bool TransferOwnershipOnRequest = true;

	public void OnOwnershipRequest(object[] viewAndPlayer)
	{
		PhotonView photonView = viewAndPlayer[0] as PhotonView;
		PhotonPlayer photonPlayer = viewAndPlayer[1] as PhotonPlayer;
		Debug.Log(string.Concat(new object[]
		{
			"OnOwnershipRequest(): Player ",
			photonPlayer,
			" requests ownership of: ",
			photonView,
			"."
		}));
		if (this.TransferOwnershipOnRequest)
		{
			photonView.TransferOwnership(photonPlayer.ID);
		}
	}

	public void OnOwnershipTransfered(object[] viewAndPlayers)
	{
		PhotonView photonView = viewAndPlayers[0] as PhotonView;
		PhotonPlayer photonPlayer = viewAndPlayers[1] as PhotonPlayer;
		PhotonPlayer photonPlayer2 = viewAndPlayers[2] as PhotonPlayer;
		Debug.Log(string.Concat(new object[]
		{
			"OnOwnershipTransfered for PhotonView",
			photonView.ToString(),
			" from ",
			photonPlayer2,
			" to ",
			photonPlayer
		}));
	}

	public void OnGUI()
	{
		GUI.skin = this.Skin;
		GUILayout.BeginArea(new Rect((float)(Screen.width - 200), 0f, 200f, (float)Screen.height));
		string text = (!this.TransferOwnershipOnRequest) ? "rejecting to pass" : "passing objects";
		if (GUILayout.Button(text, new GUILayoutOption[0]))
		{
			this.TransferOwnershipOnRequest = !this.TransferOwnershipOnRequest;
		}
		GUILayout.EndArea();
		if (PhotonNetwork.inRoom)
		{
			int iD = PhotonNetwork.player.ID;
			string text2 = (!PhotonNetwork.player.IsMasterClient) ? string.Empty : "(master) ";
			string colorName = this.GetColorName(PhotonNetwork.player.ID);
			GUILayout.Label(string.Format("player {0}, {1} {2}(you)", iD, colorName, text2), new GUILayoutOption[0]);
			PhotonPlayer[] otherPlayers = PhotonNetwork.otherPlayers;
			for (int i = 0; i < otherPlayers.Length; i++)
			{
				PhotonPlayer photonPlayer = otherPlayers[i];
				iD = photonPlayer.ID;
				text2 = ((!photonPlayer.IsMasterClient) ? string.Empty : "(master)");
				colorName = this.GetColorName(photonPlayer.ID);
				GUILayout.Label(string.Format("player {0}, {1} {2}", iD, colorName, text2), new GUILayoutOption[0]);
			}
			if (PhotonNetwork.inRoom && PhotonNetwork.otherPlayers.Length == 0)
			{
				GUILayout.Label("Join more clients to switch object-control.", new GUILayoutOption[0]);
			}
		}
		else
		{
			GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString(), new GUILayoutOption[0]);
		}
	}

	private string GetColorName(int playerId)
	{
		switch (Array.IndexOf<int>(PlayerRoomIndexing.instance.PlayerIds, playerId))
		{
		case 0:
			return "red";
		case 1:
			return "blue";
		case 2:
			return "yellow";
		case 3:
			return "green";
		default:
			return string.Empty;
		}
	}
}
