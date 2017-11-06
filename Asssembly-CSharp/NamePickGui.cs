using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ChatGui))]
public class NamePickGui : MonoBehaviour
{
	private const string UserNamePlayerPref = "NamePickUserName";

	public ChatGui chatNewComponent;

	public InputField idInput;

	public void Start()
	{
		this.chatNewComponent = Object.FindObjectOfType<ChatGui>();
		string @string = PlayerPrefs.GetString("NamePickUserName");
		if (!string.IsNullOrEmpty(@string))
		{
			this.idInput.set_text(@string);
		}
	}

	public void EndEditOnEnter()
	{
		if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
		{
			this.StartChat();
		}
	}

	public void StartChat()
	{
		ChatGui chatGui = Object.FindObjectOfType<ChatGui>();
		chatGui.UserName = this.idInput.get_text().Trim();
		chatGui.Connect();
		base.enabled = false;
		PlayerPrefs.SetString("NamePickUserName", chatGui.UserName);
	}
}
