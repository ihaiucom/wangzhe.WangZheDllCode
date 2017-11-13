using System;
using UnityEngine;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoAnimator
{
	[RequireComponent(typeof(InputField))]
	public class PlayerNameInputField : MonoBehaviour
	{
		private static string playerNamePrefKey = "PlayerName";

		private void Start()
		{
			string text = string.Empty;
			InputField component = base.GetComponent<InputField>();
			if (component != null && PlayerPrefs.HasKey(PlayerNameInputField.playerNamePrefKey))
			{
				text = PlayerPrefs.GetString(PlayerNameInputField.playerNamePrefKey);
				component.set_text(text);
			}
			PhotonNetwork.playerName = text;
		}

		public void SetPlayerName(string value)
		{
			PhotonNetwork.playerName = value + " ";
			PlayerPrefs.SetString(PlayerNameInputField.playerNamePrefKey, value);
		}
	}
}
