using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace ExitGames.Demos
{
	public class ToDemoHubButton : MonoBehaviour
	{
		private static ToDemoHubButton instance;

		private CanvasGroup _canvasGroup;

		public static ToDemoHubButton Instance
		{
			get
			{
				if (ToDemoHubButton.instance == null)
				{
					ToDemoHubButton.instance = (Object.FindObjectOfType(typeof(ToDemoHubButton)) as ToDemoHubButton);
				}
				return ToDemoHubButton.instance;
			}
		}

		public void Awake()
		{
			if (ToDemoHubButton.Instance != null && ToDemoHubButton.Instance != this)
			{
				Object.Destroy(base.gameObject);
			}
		}

		public void Start()
		{
			Object.DontDestroyOnLoad(base.gameObject);
			this._canvasGroup = base.GetComponent<CanvasGroup>();
		}

		private void OnLevelWasLoaded(int level)
		{
			this.CalledOnLevelWasLoaded(level);
		}

		private void CalledOnLevelWasLoaded(int level)
		{
			Debug.Log("CalledOnLevelWasLoaded");
			if (EventSystem.get_current() == null)
			{
				Debug.LogError("no eventSystem");
			}
		}

		public void Update()
		{
			bool flag = Application.loadedLevel == 0;
			if (flag && this._canvasGroup.alpha != 0f)
			{
				this._canvasGroup.alpha = 0f;
				this._canvasGroup.set_interactable(false);
			}
			if (!flag && this._canvasGroup.alpha != 1f)
			{
				this._canvasGroup.alpha = 1f;
				this._canvasGroup.set_interactable(true);
			}
		}

		public void BackToHub()
		{
			PhotonNetwork.Disconnect();
			SceneManager.LoadScene(0);
		}
	}
}
