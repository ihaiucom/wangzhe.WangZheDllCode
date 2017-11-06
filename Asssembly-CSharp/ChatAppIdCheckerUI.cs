using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ChatAppIdCheckerUI : MonoBehaviour
{
	public Text Description;

	public void Update()
	{
		if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.ChatAppID))
		{
			this.Description.set_text("<Color=Red>WARNING:</Color>\nTo run this demo, please set the Chat AppId in the PhotonServerSettings file.");
		}
		else
		{
			this.Description.set_text(string.Empty);
		}
	}
}
