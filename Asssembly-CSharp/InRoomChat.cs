using Photon;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class InRoomChat : Photon.MonoBehaviour
{
	public Rect GuiRect = new Rect(0f, 0f, 250f, 300f);

	public bool IsVisible = true;

	public bool AlignBottom;

	public List<string> messages = new List<string>();

	private string inputLine = string.Empty;

	private Vector2 scrollPos = Vector2.zero;

	public static readonly string ChatRPC = "Chat";

	public void Start()
	{
		if (this.AlignBottom)
		{
			this.GuiRect.y = (float)Screen.height - this.GuiRect.height;
		}
	}

	public void OnGUI()
	{
		if (!this.IsVisible || !PhotonNetwork.inRoom)
		{
			return;
		}
		if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
		{
			if (!string.IsNullOrEmpty(this.inputLine))
			{
				base.photonView.RPC("Chat", PhotonTargets.All, new object[]
				{
					this.inputLine
				});
				this.inputLine = string.Empty;
				GUI.FocusControl(string.Empty);
				return;
			}
			GUI.FocusControl("ChatInput");
		}
		GUI.SetNextControlName(string.Empty);
		GUILayout.BeginArea(this.GuiRect);
		this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		for (int i = this.messages.get_Count() - 1; i >= 0; i--)
		{
			GUILayout.Label(this.messages.get_Item(i), new GUILayoutOption[0]);
		}
		GUILayout.EndScrollView();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUI.SetNextControlName("ChatInput");
		this.inputLine = GUILayout.TextField(this.inputLine, new GUILayoutOption[0]);
		if (GUILayout.Button("Send", new GUILayoutOption[]
		{
			GUILayout.ExpandWidth(false)
		}))
		{
			base.photonView.RPC("Chat", PhotonTargets.All, new object[]
			{
				this.inputLine
			});
			this.inputLine = string.Empty;
			GUI.FocusControl(string.Empty);
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	[PunRPC]
	public void Chat(string newLine, PhotonMessageInfo mi)
	{
		string text = "anonymous";
		if (mi.sender != null)
		{
			if (!string.IsNullOrEmpty(mi.sender.NickName))
			{
				text = mi.sender.NickName;
			}
			else
			{
				text = "player " + mi.sender.ID;
			}
		}
		this.messages.Add(text + ": " + newLine);
	}

	public void AddLine(string newLine)
	{
		this.messages.Add(newLine);
	}
}
