using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClickLoadSomething : MonoBehaviour
{
	public enum ResourceTypeOption : byte
	{
		Scene,
		Web
	}

	public OnClickLoadSomething.ResourceTypeOption ResourceTypeToLoad;

	public string ResourceToLoad;

	public void OnClick()
	{
		OnClickLoadSomething.ResourceTypeOption resourceTypeToLoad = this.ResourceTypeToLoad;
		if (resourceTypeToLoad != OnClickLoadSomething.ResourceTypeOption.Scene)
		{
			if (resourceTypeToLoad == OnClickLoadSomething.ResourceTypeOption.Web)
			{
				Application.OpenURL(this.ResourceToLoad);
			}
		}
		else
		{
			SceneManager.LoadScene(this.ResourceToLoad);
		}
	}
}
