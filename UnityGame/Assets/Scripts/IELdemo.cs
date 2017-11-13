using System;
using UnityEngine;

public class IELdemo : MonoBehaviour
{
	public GUISkin Skin;

	public void OnGUI()
	{
		if (this.Skin != null)
		{
			GUI.skin = this.Skin;
		}
		if (PhotonNetwork.isMasterClient)
		{
			GUILayout.Label("Controlling client.\nPing: " + PhotonNetwork.GetPing(), new GUILayoutOption[0]);
			if (GUILayout.Button("disconnect", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				PhotonNetwork.Disconnect();
			}
		}
		else if (PhotonNetwork.isNonMasterClientInRoom)
		{
			GUILayout.Label("Receiving updates.\nPing: " + PhotonNetwork.GetPing(), new GUILayoutOption[0]);
			if (GUILayout.Button("disconnect", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				PhotonNetwork.Disconnect();
			}
		}
		else
		{
			GUILayout.Label("Not in room yet\n" + PhotonNetwork.connectionStateDetailed, new GUILayoutOption[0]);
		}
		if (!PhotonNetwork.connected && !PhotonNetwork.connecting && GUILayout.Button("connect", new GUILayoutOption[]
		{
			GUILayout.Width(80f)
		}))
		{
			PhotonNetwork.ConnectUsingSettings(null);
		}
	}
}
