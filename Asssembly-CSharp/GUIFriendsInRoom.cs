using System;
using UnityEngine;

public class GUIFriendsInRoom : MonoBehaviour
{
	public Rect GuiRect;

	private void Start()
	{
		this.GuiRect = new Rect((float)(Screen.width / 4), 80f, (float)(Screen.width / 2), (float)(Screen.height - 100));
	}

	public void OnGUI()
	{
		if (!PhotonNetwork.inRoom)
		{
			return;
		}
		GUILayout.BeginArea(this.GuiRect);
		GUILayout.Label("In-Game", new GUILayoutOption[0]);
		GUILayout.Label("For simplicity, this demo just shows the players in this room. The list will expand when more join.", new GUILayoutOption[0]);
		GUILayout.Label("Your (random) name: " + PhotonNetwork.playerName, new GUILayoutOption[0]);
		GUILayout.Label(PhotonNetwork.playerList.Length + " players in this room.", new GUILayoutOption[0]);
		GUILayout.Label("The others are:", new GUILayoutOption[0]);
		PhotonPlayer[] otherPlayers = PhotonNetwork.otherPlayers;
		for (int i = 0; i < otherPlayers.Length; i++)
		{
			PhotonPlayer photonPlayer = otherPlayers[i];
			GUILayout.Label(photonPlayer.ToString(), new GUILayoutOption[0]);
		}
		if (GUILayout.Button("Leave", new GUILayoutOption[0]))
		{
			PhotonNetwork.LeaveRoom();
		}
		GUILayout.EndArea();
	}
}
