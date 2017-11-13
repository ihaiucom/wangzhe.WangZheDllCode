using Photon;
using System;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class HighlightOwnedGameObj : Photon.MonoBehaviour
{
	public GameObject PointerPrefab;

	public float Offset = 0.5f;

	private Transform markerTransform;

	private void Update()
	{
		if (base.photonView.isMine)
		{
			if (this.markerTransform == null)
			{
				GameObject gameObject = (GameObject)Object.Instantiate(this.PointerPrefab);
				gameObject.transform.parent = base.gameObject.transform;
				this.markerTransform = gameObject.transform;
			}
			Vector3 position = base.gameObject.transform.position;
			this.markerTransform.position = new Vector3(position.x, position.y + this.Offset, position.z);
			this.markerTransform.rotation = Quaternion.identity;
		}
		else if (this.markerTransform != null)
		{
			Object.Destroy(this.markerTransform.gameObject);
			this.markerTransform = null;
		}
	}
}
