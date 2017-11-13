using Photon;
using System;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnClickRequestOwnership : Photon.MonoBehaviour
{
	public void OnClick()
	{
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			Vector3 vector = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
			base.photonView.RPC("ColorRpc", PhotonTargets.AllBufferedViaServer, new object[]
			{
				vector
			});
		}
		else
		{
			if (base.photonView.ownerId == PhotonNetwork.player.ID)
			{
				Debug.Log("Not requesting ownership. Already mine.");
				return;
			}
			base.photonView.RequestOwnership();
		}
	}

	[PunRPC]
	public void ColorRpc(Vector3 col)
	{
		Color color = new Color(col.x, col.y, col.z);
		base.gameObject.GetComponent<Renderer>().material.color = color;
	}
}
