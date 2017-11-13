using System;
using UnityEngine;
using UnityEngine.UI;

public class RpsDebug : MonoBehaviour
{
	[SerializeField]
	private Button ConnectionDebugButton;

	public bool ShowConnectionDebug;

	public void ToggleConnectionDebug()
	{
		this.ShowConnectionDebug = !this.ShowConnectionDebug;
	}

	public void Update()
	{
		if (this.ShowConnectionDebug)
		{
			this.ConnectionDebugButton.GetComponentInChildren<Text>().set_text(PhotonNetwork.connectionStateDetailed.ToString());
		}
		else
		{
			this.ConnectionDebugButton.GetComponentInChildren<Text>().set_text(string.Empty);
		}
	}
}
