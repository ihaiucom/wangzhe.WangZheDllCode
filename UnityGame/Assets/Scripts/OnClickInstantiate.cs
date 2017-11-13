using System;
using UnityEngine;

public class OnClickInstantiate : MonoBehaviour
{
	public GameObject Prefab;

	public int InstantiateType;

	private string[] InstantiateTypeNames = new string[]
	{
		"Mine",
		"Scene"
	};

	public bool showGui;

	private void OnClick()
	{
		if (!PhotonNetwork.inRoom)
		{
			return;
		}
		int instantiateType = this.InstantiateType;
		if (instantiateType != 0)
		{
			if (instantiateType == 1)
			{
				PhotonNetwork.InstantiateSceneObject(this.Prefab.name, InputToEvent.inputHitPos + new Vector3(0f, 5f, 0f), Quaternion.identity, 0, null);
			}
		}
		else
		{
			PhotonNetwork.Instantiate(this.Prefab.name, InputToEvent.inputHitPos + new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
		}
	}

	private void OnGUI()
	{
		if (this.showGui)
		{
			GUILayout.BeginArea(new Rect((float)(Screen.width - 180), 0f, 180f, 50f));
			this.InstantiateType = GUILayout.Toolbar(this.InstantiateType, this.InstantiateTypeNames, new GUILayoutOption[0]);
			GUILayout.EndArea();
		}
	}
}
