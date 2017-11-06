using System;
using UnityEngine;

public class GUICustomAuth : MonoBehaviour
{
	private enum GuiState
	{
		AuthOrNot,
		AuthInput,
		AuthHelp,
		AuthFailed
	}

	public Rect GuiRect;

	private string authName = "usr";

	private string authToken = "usr";

	private string authDebugMessage = string.Empty;

	private GUICustomAuth.GuiState guiState;

	public GameObject RootOf3dButtons;

	public void Start()
	{
		this.GuiRect = new Rect((float)(Screen.width / 4), 80f, (float)(Screen.width / 2), (float)(Screen.height - 100));
	}

	public void OnJoinedLobby()
	{
		base.enabled = false;
	}

	public void OnConnectedToMaster()
	{
		base.enabled = false;
	}

	public void OnCustomAuthenticationFailed(string debugMessage)
	{
		this.authDebugMessage = debugMessage;
		this.SetStateAuthFailed();
	}

	public void SetStateAuthInput()
	{
		this.RootOf3dButtons.SetActive(false);
		this.guiState = GUICustomAuth.GuiState.AuthInput;
	}

	public void SetStateAuthHelp()
	{
		this.RootOf3dButtons.SetActive(false);
		this.guiState = GUICustomAuth.GuiState.AuthHelp;
	}

	public void SetStateAuthOrNot()
	{
		this.RootOf3dButtons.SetActive(true);
		this.guiState = GUICustomAuth.GuiState.AuthOrNot;
	}

	public void SetStateAuthFailed()
	{
		this.RootOf3dButtons.SetActive(false);
		this.guiState = GUICustomAuth.GuiState.AuthFailed;
	}

	public void ConnectWithNickname()
	{
		this.RootOf3dButtons.SetActive(false);
		PhotonNetwork.AuthValues = new AuthenticationValues
		{
			UserId = PhotonNetwork.playerName
		};
		PhotonNetwork.playerName += "Nick";
		PhotonNetwork.ConnectUsingSettings("1.0");
	}

	private void OnGUI()
	{
		if (PhotonNetwork.connected)
		{
			GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString(), new GUILayoutOption[0]);
			return;
		}
		GUILayout.BeginArea(this.GuiRect);
		switch (this.guiState)
		{
		case GUICustomAuth.GuiState.AuthInput:
			GUILayout.Label("Authenticate yourself", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.authName = GUILayout.TextField(this.authName, new GUILayoutOption[]
			{
				GUILayout.Width((float)(Screen.width / 4 - 5))
			});
			GUILayout.FlexibleSpace();
			this.authToken = GUILayout.TextField(this.authToken, new GUILayoutOption[]
			{
				GUILayout.Width((float)(Screen.width / 4 - 5))
			});
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Authenticate", new GUILayoutOption[0]))
			{
				PhotonNetwork.AuthValues = new AuthenticationValues();
				PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Custom;
				PhotonNetwork.AuthValues.AddAuthParameter("username", this.authName);
				PhotonNetwork.AuthValues.AddAuthParameter("token", this.authToken);
				PhotonNetwork.ConnectUsingSettings("1.0");
			}
			GUILayout.Space(10f);
			if (GUILayout.Button("Help", new GUILayoutOption[]
			{
				GUILayout.Width(100f)
			}))
			{
				this.SetStateAuthHelp();
			}
			break;
		case GUICustomAuth.GuiState.AuthHelp:
			GUILayout.Label("By default, any player can connect to Photon.\n'Custom Authentication' can be enabled to reject players without valid user-account.", new GUILayoutOption[0]);
			GUILayout.Label("The actual authentication must be done by a web-service which you host and customize. Example sourcecode for these services is available on the docs page.", new GUILayoutOption[0]);
			GUILayout.Label("For this demo set the Authentication URL in the Dashboard to:\nhttp://photon.webscript.io/auth-demo-equals", new GUILayoutOption[0]);
			GUILayout.Label("That authentication-service has no user-database. It confirms any user if 'name equals password'.", new GUILayoutOption[0]);
			GUILayout.Space(10f);
			if (GUILayout.Button("Configure Authentication (Dashboard)", new GUILayoutOption[0]))
			{
				Application.OpenURL("https://www.photonengine.com/dashboard");
			}
			if (GUILayout.Button("Authentication Docs", new GUILayoutOption[0]))
			{
				Application.OpenURL("http://doc.exitgames.com/en/pun/current/tutorials/pun-and-facebook-custom-authentication");
			}
			GUILayout.Space(10f);
			if (GUILayout.Button("Back to input", new GUILayoutOption[0]))
			{
				this.SetStateAuthInput();
			}
			break;
		case GUICustomAuth.GuiState.AuthFailed:
			GUILayout.Label("Authentication Failed", new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.Label("Error message:\n'" + this.authDebugMessage + "'", new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.Label("For this demo set the Authentication URL in the Dashboard to:\nhttp://photon.webscript.io/auth-demo-equals", new GUILayoutOption[0]);
			GUILayout.Label("That authentication-service has no user-database. It confirms any user if 'name equals password'.", new GUILayoutOption[0]);
			GUILayout.Label("The error message comes from that service and can be customized.", new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Back", new GUILayoutOption[0]))
			{
				this.SetStateAuthInput();
			}
			if (GUILayout.Button("Help", new GUILayoutOption[0]))
			{
				this.SetStateAuthHelp();
			}
			GUILayout.EndHorizontal();
			break;
		}
		GUILayout.EndArea();
	}
}
