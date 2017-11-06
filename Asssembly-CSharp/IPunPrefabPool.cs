using System;
using UnityEngine;

public interface IPunPrefabPool
{
	GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation);

	void Destroy(GameObject gameObject);
}
