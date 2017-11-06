using System;
using System.Collections.Generic;
using UnityEngine;

public class GUIFriendFinding : MonoBehaviour
{
	private string[] friendListOfSomeCommunity;

	public Rect GuiRect;

	private string ExpectedUsers;

	private void Start()
	{
		PhotonNetwork.playerName = "usr" + Random.Range(0, 9);
		this.friendListOfSomeCommunity = GUIFriendFinding.FetchFriendsFromCommunity();
		this.GuiRect = new Rect((float)(Screen.width / 4), 80f, (float)(Screen.width / 2), (float)(Screen.height - 100));
	}

	public static string[] FetchFriendsFromCommunity()
	{
		string[] array = new string[9];
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			string text = "usr" + num++;
			if (text.Equals(PhotonNetwork.playerName))
			{
				text = "usr" + num++;
			}
			array[i] = text;
		}
		return array;
	}

	public void OnUpdatedFriendList()
	{
		Debug.Log("OnUpdatedFriendList is called when the list PhotonNetwork.Friends is refreshed.");
	}

	public void OnGUI()
	{
		if (!PhotonNetwork.connectedAndReady || PhotonNetwork.Server != ServerConnection.MasterServer)
		{
			return;
		}
		GUILayout.BeginArea(this.GuiRect);
		GUILayout.Label("Your (random) name: " + PhotonNetwork.playerName, new GUILayoutOption[0]);
		GUILayout.Label("Your friends: " + string.Join(", ", this.friendListOfSomeCommunity), new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("Find Friends", new GUILayoutOption[0]))
		{
			PhotonNetwork.FindFriends(this.friendListOfSomeCommunity);
		}
		if (GUILayout.Button("Create Room", new GUILayoutOption[0]))
		{
			PhotonNetwork.CreateRoom(null);
		}
		this.ExpectedUsers = GUILayout.TextField("Expected Users", this.ExpectedUsers, new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		if (PhotonNetwork.Friends != null)
		{
			using (List<FriendInfo>.Enumerator enumerator = PhotonNetwork.Friends.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FriendInfo current = enumerator.get_Current();
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Label(current.ToString(), new GUILayoutOption[0]);
					if (current.IsInRoom && GUILayout.Button("join", new GUILayoutOption[0]))
					{
						PhotonNetwork.JoinRoom(current.Room);
					}
					GUILayout.EndHorizontal();
				}
			}
		}
		GUILayout.EndArea();
	}
}
