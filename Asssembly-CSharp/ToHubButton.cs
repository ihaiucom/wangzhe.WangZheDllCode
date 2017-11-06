using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToHubButton : MonoBehaviour
{
	public Texture2D ButtonTexture;

	private Rect ButtonRect;

	private static ToHubButton instance;

	public static ToHubButton Instance
	{
		get
		{
			if (ToHubButton.instance == null)
			{
				ToHubButton.instance = (Object.FindObjectOfType(typeof(ToHubButton)) as ToHubButton);
			}
			return ToHubButton.instance;
		}
	}

	public void Awake()
	{
		if (ToHubButton.Instance != null && ToHubButton.Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void Start()
	{
		if (this.ButtonTexture == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void OnGUI()
	{
		if (Application.loadedLevel != 0)
		{
			int num = this.ButtonTexture.width + 4;
			int num2 = this.ButtonTexture.height + 4;
			this.ButtonRect = new Rect((float)(Screen.width - num), (float)(Screen.height - num2), (float)num, (float)num2);
			if (GUI.Button(this.ButtonRect, this.ButtonTexture, GUIStyle.none))
			{
				PhotonNetwork.Disconnect();
				SceneManager.LoadScene(0);
			}
		}
	}
}
