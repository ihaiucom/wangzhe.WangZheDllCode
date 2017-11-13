using System;
using UnityEngine;
using UnityEngine.UI;

public class FriendItem : MonoBehaviour
{
	public Text NameLabel;

	public Text StatusLabel;

	public Text Health;

	[HideInInspector]
	public string FriendId
	{
		get
		{
			return this.NameLabel.get_text();
		}
		set
		{
			this.NameLabel.set_text(value);
		}
	}

	public void Awake()
	{
		this.Health.set_text(string.Empty);
	}

	public void OnFriendStatusUpdate(int status, bool gotMessage, object message)
	{
		string text;
		switch (status)
		{
		case 1:
			text = "Invisible";
			break;
		case 2:
			text = "Online";
			break;
		case 3:
			text = "Away";
			break;
		case 4:
			text = "Do not disturb";
			break;
		case 5:
			text = "Looking For Game/Group";
			break;
		case 6:
			text = "Playing";
			break;
		default:
			text = "Offline";
			break;
		}
		this.StatusLabel.set_text(text);
		if (gotMessage)
		{
			string text2 = string.Empty;
			if (message != null)
			{
				string[] array = message as string[];
				if (array != null && array.Length >= 2)
				{
					text2 = array[1] + "%";
				}
			}
			this.Health.set_text(text2);
		}
	}
}
