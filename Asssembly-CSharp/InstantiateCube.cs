using System;
using UnityEngine;

public class InstantiateCube : MonoBehaviour
{
	public GameObject Prefab;

	public int InstantiateType;

	public bool showGui;

	private void OnClick()
	{
		if (PhotonNetwork.connectionStateDetailed != ClientState.Joined)
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
			PhotonNetwork.Instantiate(this.Prefab.name, base.transform.position + 3f * Vector3.up, Quaternion.identity, 0);
		}
	}
}
