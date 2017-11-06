using System;
using UnityEngine;

public class OnJoinedInstantiate : MonoBehaviour
{
	public Transform SpawnPosition;

	public float PositionOffset = 2f;

	public GameObject[] PrefabsToInstantiate;

	public void OnJoinedRoom()
	{
		if (this.PrefabsToInstantiate != null)
		{
			GameObject[] prefabsToInstantiate = this.PrefabsToInstantiate;
			for (int i = 0; i < prefabsToInstantiate.Length; i++)
			{
				GameObject gameObject = prefabsToInstantiate[i];
				Debug.Log("Instantiating: " + gameObject.name);
				Vector3 a = Vector3.up;
				if (this.SpawnPosition != null)
				{
					a = this.SpawnPosition.position;
				}
				Vector3 a2 = Random.get_insideUnitSphere();
				a2.y = 0f;
				a2 = a2.normalized;
				Vector3 position = a + this.PositionOffset * a2;
				PhotonNetwork.Instantiate(gameObject.name, position, Quaternion.identity, 0);
			}
		}
	}
}
